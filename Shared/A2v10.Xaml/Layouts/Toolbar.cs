﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Xaml
{
    public class Toolbar : Container
    {

        public enum ToolbarAlign
        {
            Left,
            Right
        }

        #region Attached Properties
        static Lazy<IDictionary<Object, ToolbarAlign>> _attachedPart = new Lazy<IDictionary<Object, ToolbarAlign>>(() => new Dictionary<Object, ToolbarAlign>());

        public static void SetAlign(Object obj, ToolbarAlign aln)
        {
            AttachedHelpers.SetAttached(_attachedPart, obj, aln);
        }

        public static ToolbarAlign GetAlgin(Object obj)
        {
            return AttachedHelpers.GetAttached(_attachedPart, obj);
        }
        #endregion

        internal override void RenderElement(RenderContext context, Action<TagBuilder> onRender = null)
        {
            var tb = new TagBuilder("div", "toolbar");
            if (onRender != null)
                onRender(tb);
            tb.RenderStart(context);
            RenderChildren(context);
            tb.RenderEnd(context);
        }

        internal override void RenderChildren(RenderContext context)
        {
            if (_attachedPart == null)
            {
                base.RenderChildren(context);
                return;
            }
            List<UIElementBase> rightList = new List<UIElementBase>();
            // Те, что влево и не установлены
            foreach (var ch in Children)
            {
                if (GetAlgin(ch) == ToolbarAlign.Right)
                    rightList.Add(ch);
                else
                    ch.RenderElement(context);
            }
            if (rightList.Count == 0)
                return;
            // aligner
            new TagBuilder("div", "aligner").Render(context);

            // Те, что справа
            foreach (var ch in rightList)
                ch.RenderElement(context);
        }
    }
}
