﻿// Copyright © 2019 Alex Kukhtin. All rights reserved.

using System;

namespace A2v10.Interop.ExportTo
{
	public class ExClassList
	{
		public HorizontalAlign Align { get; set; }
		public Boolean Bold { get; set; }
		public RowRole Role { get; set; }
	}


	public static class Utils
	{
		public static ExClassList ParseClasses(String strClass)
		{
			var lst = new ExClassList();
			if (String.IsNullOrEmpty(strClass))
				return lst;
			var split = strClass.Split(' ');
			foreach (var cls in split)
			{
				switch (cls)
				{
					case "row-header":
						lst.Role = RowRole.Header;
						break;
					case "row-footer":
						lst.Role = RowRole.Footer;
						break;
					case "row-title":
						lst.Role = RowRole.Title;
						break;
					case "row-total":
						lst.Role = RowRole.Total;
						break;
					case "row-parameter":
						lst.Role = RowRole.Parameter;
						break;
				}
				if (cls.StartsWith("text-"))
				{
					switch (cls)
					{
						case "text-center":
							lst.Align = HorizontalAlign.Center;
							break;
						case "text-right":
							lst.Align = HorizontalAlign.Right;
							break;
						case "text-left":
							lst.Align = HorizontalAlign.Left;
							break;
					}
				}
				if (cls == "bold")
					lst.Bold = true;
			}
			return lst;
		}
	}
}
