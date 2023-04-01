using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CefSharp;
using Microsoft.Win32;

namespace Slobby_Xploits__WPF_.Assets
{
	public static class Functions
	{
		private static string ReadScript
		{
			get
			{
				return WebBrowserExtensions.EvaluateScriptAsync(Editor.SelectedEditor, "(function() { return GetText() })();", null, false).GetAwaiter().GetResult()
					.Result.ToString() ?? "";
			}
		}

		public static void OpenScripts()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Lua (*.lua)|*.lua|Text (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 3;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Title = "Open Script";
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				try
				{
					string text = File.ReadAllText(openFileDialog.FileName);
					WebBrowserExtensions.EvaluateScriptAsync(Editor.SelectedEditor, "SetText(`" + text.Replace("`", "\\`").Replace("\\", "\\\\") + "`)", null, false);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error : Could not read file from disk. Original error: " + ex.Message);
				}
			}
		}

		public static void SaveScripts()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Lua (*.lua)|*.lua|Text (*.txt)|*.txt|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.Title = "Save Script";
			bool? flag = saveFileDialog.ShowDialog();
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				File.WriteAllText(saveFileDialog.FileName, Functions.ReadScript);
			}
		}

		public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}
			return from f in dir.EnumerateFiles()
				where extensions.Contains(f.Extension)
				select f;
		}

		[CompilerGenerated]
		private sealed class <>c__DisplayClass4_0
		{
			public <>c__DisplayClass4_0()
			{
			}

			internal bool <GetFilesByExtensions>b__0(FileInfo f)
			{
				return this.extensions.Contains(f.Extension);
			}

			public string[] extensions;
		}
	}
}
