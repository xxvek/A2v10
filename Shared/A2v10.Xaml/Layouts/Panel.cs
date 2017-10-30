﻿// Copyright © 2015-2017 Alex Kukhtin. All rights reserved.

using System;

namespace A2v10.Xaml
{
    public enum PaneStyle
    {
        Default,
        Info,
        Warning,
        Danger,
        Error,
        Success,
        Green,
        Cyan,
        Red,
        Yellow
    }

    public class Panel : Container, ITableControl
    {
        public Object Header { get; set; }

        public Boolean Collapsible { get; set; }

        public Boolean? Collapsed { get; set; }

        public PaneStyle Style { get; set; }

        internal override void RenderElement(RenderContext context, Action<TagBuilder> onRender = null)
        {
            var panel = new TagBuilder("a2-panel", null, IsInGrid);
            MergeBindingAttributeBool(panel, context, ":initial-collapsed", nameof(Collapsed), Collapsed);
            MergeBindingAttributeBool(panel, context, ":collapsible", nameof(Collapsible), Collapsible);
            if (!HasHeader)
                panel.MergeAttribute(":no-header", "true");
            var sb = GetBinding(nameof(Style));
            if (sb != null)
                panel.MergeAttribute(":panel-style", sb.GetPathFormat(context));
            else if (Style != PaneStyle.Default)
                panel.MergeAttribute("panel-style", Style.ToString().ToLowerInvariant());
            MergeAttributes(panel, context, MergeAttrMode.Visibility);
            panel.RenderStart(context);
            RenderHeader(context);
            var content = new TagBuilder("div", "panel-content");
            MergeAttributes(content, context, MergeAttrMode.Margin | MergeAttrMode.Wrap | MergeAttrMode.Tip);
            content.RenderStart(context);
            RenderChildren(context);
            content.RenderEnd(context);
            panel.RenderEnd(context);
        }

        Boolean HasHeader => GetBinding(nameof(Header)) != null || Header != null;

        void RenderHeader(RenderContext context)
        {
            if (!HasHeader)
                return;
            var header = new TagBuilder("div", "panel-header-slot");
            header.MergeAttribute("slot", "header");
            var hBind = GetBinding(nameof(Header));
            if (hBind != null)
            {
                header.MergeAttribute("v-text", hBind.GetPathFormat(context));
            }
            header.RenderStart(context);

            if (Header is UIElementBase)
            {
                (Header as UIElementBase).RenderElement(context);
            }
            else if (Header != null)
            {
                context.Writer.Write(Header.ToString());
            }
            header.RenderEnd(context);
        }
    }
}
