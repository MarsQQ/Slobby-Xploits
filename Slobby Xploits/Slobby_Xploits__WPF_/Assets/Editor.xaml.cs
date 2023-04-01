using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using CefSharp.Wpf;

namespace Slobby_Xploits__WPF_.Assets
{
	public partial class Editor : UserControl, IStyleConnector
	{
		private event EventHandler TabChanged;

		public Editor()
		{
			this.InitializeComponent();
			this.AddTab();
		}

		private string TabName()
		{
			return string.Format("Tab {0}", this.Tab.Items.Count + 1);
		}

		public async void AddTab()
		{
			ChromiumWebBrowser editor = new ChromiumWebBrowser
			{
				Address = "file:///" + AppDomain.CurrentDomain.BaseDirectory + "/bin/Monaco/Monaco.html"
			};
			TabItem item = new TabItem
			{
				Content = editor
			};
			StackPanel stackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal
			};
			TextBlock textBlock = new TextBlock
			{
				Text = this.TabName(),
				Focusable = true,
				Foreground = Brushes.White
			};
			Button removeButton = new Button
			{
				Content = "\uea76",
				FontSize = 14.0,
				Visibility = Visibility.Collapsed,
				Margin = new Thickness(5.0, 0.0, 0.0, 0.0),
				FontFamily = (base.TryFindResource("Codicon") as FontFamily),
				Background = null,
				BorderBrush = null,
				Foreground = Brushes.White
			};
			stackPanel.Children.Add(textBlock);
			stackPanel.Children.Add(removeButton);
			removeButton.Click += delegate
			{
				if (this.Tab.Items.Count > 1)
				{
					editor.Dispose();
					this.Tab.Items.Remove(item);
				}
			};
			item.Header = stackPanel;
			item.Height = 24.0;
			this.Tab.SelectionChanged += delegate
			{
				removeButton.Visibility = (item.IsSelected ? Visibility.Visible : Visibility.Collapsed);
				if (item.IsSelected)
				{
					Editor.SelectedEditor = editor;
				}
				EventHandler tabChanged = this.TabChanged;
				if (tabChanged == null)
				{
					return;
				}
				tabChanged(item, EventArgs.Empty);
			};
			this.Tab.Items.Add(item);
			await Task.Delay(100);
			Editor.SelectedEditor = editor;
			this.Tab.SelectedIndex = this.Tab.Items.Count;
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			this.AddTab();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				((Button)target).Click += this.AddButton_Click;
			}
		}

		public static ChromiumWebBrowser SelectedEditor;
	}
}
