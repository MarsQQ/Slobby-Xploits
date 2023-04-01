using System;

namespace Slobby_Xploits__WPF_.Assets
{
	public struct FilterInstance
	{
		public static string ToString(FilterInstance[] f)
		{
			string text = "";
			for (int i = 0; i < f.Length; i++)
			{
				FilterInstance filterInstance = f[i];
				string text2 = filterInstance.Title;
				if (filterInstance.IncludeFilter)
				{
					text2 = text2 + " (" + filterInstance.Filter + ")";
				}
				text2 = text2 + "|" + filterInstance.Filter;
				if (i < f.Length - 1)
				{
					text2 += "|";
				}
				text += text2;
			}
			return text;
		}

		public string Title;

		public string Filter;

		public bool IncludeFilter;
	}
}
