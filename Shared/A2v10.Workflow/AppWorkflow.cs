﻿// Copyright © 2012-2017 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.Tracking;
using System.Runtime.DurableInstancing;

using Microsoft.Activities.Extensions.Tracking;

using A2v10.Infrastructure;
using System.Diagnostics;
using System.Threading.Tasks;

namespace A2v10.Workflow
{
    public class AppWorkflow
    {
        private WorkflowApplication _application;
        private static TimeSpan _wfTimeSpan = TimeSpan.FromSeconds(30);
        private ManualResetEvent _endEvent = new ManualResetEvent(false);
        private const Int32 RUNNABLE_INSTANCES_DETECTION_PERIOD = 10;

        private StateMachineStateTracker _tracker;
        private IList<TrackInfo> _trackingRecords;

        private Exception _unhandledException;

        IDbContext _dbContext;

        public static async Task<WorkflowResult> StartWorkflow(IApplicationHost host, IDbContext dbContext, StartWorkflowInfo info)
        {
            AppWorkflow aw = null;
            var result = new WorkflowResult();
            result.InboxIds = new List<Int64>();
            try
            {
                var def = WorkflowDefinition.Create(info.Source);
                Activity root = def.LoadFromSource(host);
                Process process = Process.Create(def, info);
                // workflow arguments
                IDictionary<String, Object> args = new Dictionary<String, Object>();
                args.Add("Process", process);
                args.Add("Comment", info.Comment);
                aw = Create(dbContext, root, args, def.Identity);
                aw._application.Extensions.Add(result);
                process.WorkflowId = aw._application.Id;
                await process.Start(dbContext);
                aw._application.Run(_wfTimeSpan);
                result.ProcessId = process.Id;
                return result;
            }
            catch (Exception ex)
            {
                if (!CatchWorkflow(aw, ex))
                    throw;
                return result;
            }
            finally
            {
                ProcessFinally(aw);
            }
        }

        public static async Task<WorkflowResult> ResumeWorkflow(IApplicationHost host, IDbContext dbContext, ResumeWorkflowInfo info)
        {
            AppWorkflow aw = null;
            var result = new WorkflowResult();
            result.InboxIds = new List<Int64>();
            try
            {
                InboxInfo inbox = await InboxInfo.Load(dbContext, info.Id, info.UserId);
                result.ProcessId = inbox.ProcessId;
                var def = WorkflowDefinition.Load(inbox);
                Activity root = def.LoadFromSource(host);
                aw = Create(dbContext, root, null, def.Identity);
                aw._application.Extensions.Add(result);
                WorkflowApplicationInstance instance = WorkflowApplication.GetInstance(inbox.WorkflowId, aw._application.InstanceStore);
                aw._application.Load(instance, _wfTimeSpan);
                foreach (var bm in aw._application.GetBookmarks())
                {
                    if (bm.BookmarkName == inbox.Bookmark)
                    {
                        var rr = new RequestResult();
                        rr.Answer = info.Answer;
                        rr.Comment = info.Comment;
                        rr.Params = info.Params;
                        rr.UserId = info.UserId;
                        rr.InboxId = info.Id;
                        aw._application.ResumeBookmark(bm.BookmarkName, rr);
                        return result; // already resumed
                    }
                }
                // if a bookmark is not found
                aw._application.Unload();
            }
            catch (Exception ex)
            {
                if (!CatchWorkflow(aw, ex))
                    throw;
            }
            finally
            {
                ProcessFinally(aw);
            }
            return result;
        }

        internal void Track(TrackingRecord record)
        {
            if (_trackingRecords == null)
                _trackingRecords = new List<TrackInfo>();
            var ti = new TrackInfo(record);
            if (_tracker != null)
            {
                ti.State = _tracker.CurrentState;
            }
            _trackingRecords.Add(ti);
        }

        static AppWorkflow Create(IDbContext dbContext, Activity root, IDictionary<String, Object> args, WorkflowIdentity identity)
        {
            var aw = new AppWorkflow();
            var store = aw.CreateInstanceStore(dbContext.ConnectionString(null));
            if (args == null)
                aw._application = new WorkflowApplication(root, identity);
            else
                aw._application = new WorkflowApplication(root, args, identity);
            aw.SetApplicationHandlers();
            aw._dbContext = dbContext;
            aw._tracker = StateMachineStateTracker.Attach(aw._application);
            aw._application.Extensions.Add(new WorkflowTracker(aw));
            aw._application.InstanceStore = store;
            return aw;
        }

        static bool CatchWorkflow(AppWorkflow aw, Exception ex)
        {
            if ((aw != null) && (aw._application != null))
            {
                String msg = ex.Message;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                aw.Track(new CustomTrackingRecord(aw._application.Id, msg, TraceLevel.Error));
                aw._application.Unload();
            }
            if (ex.InnerException != null)
                throw ex.InnerException;
            else
                return false;
        }

        static void ProcessFinally(AppWorkflow aw /*,IProfiler profiler*/)
        {
            if (aw == null)
                return;
            aw._endEvent.WaitOne(_wfTimeSpan);
            if (aw._unhandledException != null)
            {
                if (aw._unhandledException.InnerException != null)
                    throw aw._unhandledException.InnerException;
                throw aw._unhandledException;
            }
            if (aw._application.InstanceStore != null)
                aw._application.InstanceStore.DefaultInstanceOwner = null;
            aw.WriteTrackingRecords();
            //TODO:if (profiler != null)
                //profiler.EndWorkflow(aw._token);
        }


        void WriteTrackingRecords()
        {
            if ((_trackingRecords == null) || (_trackingRecords.Count == 0))
                return;
            try
            {
                var prm = new
                {
                    InstanceId = _application.Id
                };
                _dbContext.SaveList<TrackInfo>(null, "[a2workflow].[WriteLog]", prm, _trackingRecords);
            }
            catch (Exception /*ex*/)
            {
                // eat all ?
            }
        }

        void SetApplicationHandlers()
        {
            _application.PersistableIdle = (e) =>
            {
                RefreshWorkflowState();
                return PersistableIdleAction.Unload;
            };
            _application.Completed = (e) =>
            {
                RefreshWorkflowState();
                _endEvent.Set();
            };
            _application.Unloaded = (e) =>
            {
                /* state is already unloaded in PersistableIdle */
                _endEvent.Set();
            };
            _application.Aborted = (e) =>
            {
                _unhandledException = e.Reason;
                WriteTrackingRecords();
                RefreshWorkflowState();
                _endEvent.Set();
            };
            _application.OnUnhandledException = (e) =>
            {
                _unhandledException = e.UnhandledException;
                WriteTrackingRecords();
                RefreshWorkflowState();
                _endEvent.Set();
                return UnhandledExceptionAction.Terminate;
            };
        }

        void RefreshWorkflowState()
        {
            // Nothing to do is not necessary.
            // Everything is saved in the TrackingRecord.
        }

         SqlWorkflowInstanceStore CreateInstanceStore(String cnnString)
        {
            String cnnStr = cnnString;
            if (cnnStr.IndexOf("Asynchronous Processing") == -1)
                cnnStr += ";Asynchronous Processing=True";
            var _store = new SqlWorkflowInstanceStore(cnnStr);
            _store.InstanceLockedExceptionAction = InstanceLockedExceptionAction.AggressiveRetry;
            _store.HostLockRenewalPeriod = _wfTimeSpan;
            _store.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(RUNNABLE_INSTANCES_DETECTION_PERIOD);
            _store.InstanceCompletionAction = InstanceCompletionAction.DeleteNothing;
            InstanceHandle handle = _store.CreateInstanceHandle();
            InstanceView view = _store.Execute(handle, new CreateWorkflowOwnerCommand(), _wfTimeSpan);
            _store.DefaultInstanceOwner = view.InstanceOwner;
            handle.Free();
            return _store;
        }
    }
}
