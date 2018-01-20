﻿// Copyright © 2015-2017 Alex Kukhtin. All rights reserved.

using A2v10.Data;
using A2v10.Infrastructure;
using A2v10.Messaging;
using A2v10.Web.Mvc.Configuration;
using A2v10.Workflow;
using A2v10.Xaml;
using System.Web;

namespace A2v10.Web.Mvc.Start
{
    public partial class Startup
    {
        public void StartServices()
        {
            // DI ready
            ServiceLocator.Start = (IServiceLocator locator) =>
            {
                IProfiler profiler = new WebProfiler();
                IApplicationHost host = new WebApplicationHost(profiler);
                IDbContext dbContext = new SqlDbContext(host);
                IRenderer renderer = new XamlRenderer(profiler);
                IWorkflowEngine workflowEngine = new WorkflowEngine(host, dbContext);
                IMessaging messaging = new MessageProcessor(host, dbContext);

                locator.RegisterService<IDbContext>(dbContext);
                locator.RegisterService<IProfiler>(profiler);
                locator.RegisterService<IApplicationHost>(host);
                locator.RegisterService<IRenderer>(renderer);
                locator.RegisterService<IWorkflowEngine>(workflowEngine);
                locator.RegisterService<IMessaging>(messaging);

                HttpContext.Current.Items.Add("ServiceLocator", locator);
            };

            ServiceLocator.GetCurrentLocator = () =>
            {
                var locator = HttpContext.Current.Items["ServiceLocator"];
                if (locator == null)
                    new ServiceLocator();
                return HttpContext.Current.Items["ServiceLocator"] as IServiceLocator;
            };
        }
    }
}
