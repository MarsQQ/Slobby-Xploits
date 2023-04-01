using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CefSharp;
using DiscordRPC;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using Slobby_Xploits__WPF.Assets;
using Slobby_Xploits__WPF_.Assets;
using SlxpAPI;

namespace Slobby_Xploits__WPF_
{
	public partial class MainWindow : Window
	{
		private string ReadScript
		{
			get
			{
				return WebBrowserExtensions.EvaluateScriptAsync(Editor.SelectedEditor, "(function() { return GetText() })();", null, false).GetAwaiter().GetResult()
					.Result.ToString() ?? "";
			}
		}

		private string latestVersion
		{
			get
			{
				return this.httpClient.GetStringAsync("https://raw.githubusercontent.com/Snobbish-slob/Nothing-Special/main/version_executor.txt").Result;
			}
		}

		public MainWindow()
		{
			this.InitializeComponent();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				base.DragMove();
			}
		}

		private void logo_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://discord.gg/hWqcUMWQUs");
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		private void CloseAppButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void AttachButton_Click(object sender, RoutedEventArgs e)
		{
			this.slxpApi.Inject();
		}

		private void ExecuteButton_Click(object sender, RoutedEventArgs e)
		{
			this.slxpApi.ExecuteScript(this.ReadScript);
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			Functions.SaveScripts();
		}

		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			Functions.OpenScripts();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.latestVersion.Contains(this.appVersion))
			{
				this.PopulateTreeView(this.ScriptList);
				this.LoadSettings();
				await this.LoadScriptHub();
				this.FileWatcher();
				this.DiscordRPC();
				this.Timer();
			}
			else
			{
				this.NewUpdate.Visibility = Visibility.Visible;
				this.Main.Visibility = Visibility.Collapsed;
				this.Misc.Visibility = Visibility.Collapsed;
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			this.discordRpcClient.Dispose();
			this.SaveSettings();
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			((Storyboard)base.TryFindResource("SettingsOpenStoryboard")).Begin();
		}

		private void ApiSettingsButton_Click(object sender, RoutedEventArgs e)
		{
			((Storyboard)base.TryFindResource("ApiSettingsOpenStoryboard")).Begin();
		}

		private void ScriptHubButton_Click(object sender, RoutedEventArgs e)
		{
			((Storyboard)base.TryFindResource("ScriptHubOpenStoryboard")).Begin();
		}

		private void Panels_Click(object sender, RoutedEventArgs e)
		{
			((Storyboard)base.TryFindResource("PanelClosedStoryboard")).Begin();
		}

		private void TopMost_Changed(object sender, RoutedEventArgs e)
		{
			if (this.TopMost.IsChecked.Value)
			{
				base.Topmost = true;
				return;
			}
			base.Topmost = false;
		}

		private void AutoAttach_Changed(object sender, RoutedEventArgs e)
		{
			if (this.AutoAttach.IsChecked.Value)
			{
				this.processWatcher = new ProcessWatcher("RobloxPlayerBeta");
				this.processWatcher.Created += async delegate(object send, Process proc)
				{
					await Task.Delay(5000);
					this.slxpApi.Inject();
				};
				return;
			}
			this.processWatcher.Dispose();
		}

		private void UnlockFPS_Changed(object sender, RoutedEventArgs e)
		{
			if (this.UnlockFPS.IsChecked.Value & this.slxpApi.IsInjected())
			{
				this.slxpApi.ExecuteScript("setfpscap(999)");
				return;
			}
			if ((!this.UnlockFPS.IsChecked).Value)
			{
				this.slxpApi.ExecuteScript("setfpscap(60)");
			}
		}

		private void WeAreDevs_Changed(object sender, RoutedEventArgs e)
		{
			if (this.WeAreDevs.IsChecked.Value)
			{
				this.Krnl.IsChecked = new bool?(false);
				this.Comet.IsChecked = new bool?(false);
				this.Fluxus.IsChecked = new bool?(false);
				this.Oxygen.IsChecked = new bool?(false);
				this.Electron.IsChecked = new bool?(false);
				this.FurkUltra.IsChecked = new bool?(false);
			}
			this.slxpApi.wrd = this.WeAreDevs.IsChecked.Value;
			this.slxpApi.Initialize();
		}

		private void FurkUltra_Changed(object sender, RoutedEventArgs e)
		{
			if (this.FurkUltra.IsChecked.Value)
			{
				this.Krnl.IsChecked = new bool?(false);
				this.Comet.IsChecked = new bool?(false);
				this.Fluxus.IsChecked = new bool?(false);
				this.Oxygen.IsChecked = new bool?(false);
				this.Electron.IsChecked = new bool?(false);
				this.WeAreDevs.IsChecked = new bool?(false);
			}
			this.slxpApi.furkUltra = this.FurkUltra.IsChecked.Value;
			this.slxpApi.Initialize();
		}

		private void Electron_Changed(object sender, RoutedEventArgs e)
		{
			if (this.Electron.IsChecked.Value)
			{
				this.Krnl.IsChecked = new bool?(false);
				this.Comet.IsChecked = new bool?(false);
				this.Fluxus.IsChecked = new bool?(false);
				this.Oxygen.IsChecked = new bool?(false);
				this.FurkUltra.IsChecked = new bool?(false);
				this.WeAreDevs.IsChecked = new bool?(false);
			}
			this.slxpApi.electron = this.Electron.IsChecked.Value;
			this.slxpApi.Initialize();
		}

		private void Oxygen_Changed(object sender, RoutedEventArgs e)
		{
			if (this.Oxygen.IsChecked.Value)
			{
				this.Krnl.IsChecked = new bool?(false);
				this.Comet.IsChecked = new bool?(false);
				this.Fluxus.IsChecked = new bool?(false);
				this.Electron.IsChecked = new bool?(false);
				this.FurkUltra.IsChecked = new bool?(false);
				this.WeAreDevs.IsChecked = new bool?(false);
			}
			this.slxpApi.oxygen = this.Oxygen.IsChecked.Value;
			this.slxpApi.Initialize();
		}

		private void Fluxus_Changed(object sender, RoutedEventArgs e)
		{
			if (this.Fluxus.IsChecked.Value)
			{
				this.Krnl.IsChecked = new bool?(false);
				this.Comet.IsChecked = new bool?(false);
				this.Oxygen.IsChecked = new bool?(false);
				this.Electron.IsChecked = new bool?(false);
				this.FurkUltra.IsChecked = new bool?(false);
				this.WeAreDevs.IsChecked = new bool?(false);
			}
			this.slxpApi.fluxus = this.Fluxus.IsChecked.Value;
			this.slxpApi.Initialize();
		}

		private void Comet_Changed(object sender, RoutedEventArgs e)
		{
			if (this.Comet.IsChecked.Value)
			{
				this.Krnl.IsChecked = new bool?(false);
				this.Fluxus.IsChecked = new bool?(false);
				this.Oxygen.IsChecked = new bool?(false);
				this.Electron.IsChecked = new bool?(false);
				this.FurkUltra.IsChecked = new bool?(false);
				this.WeAreDevs.IsChecked = new bool?(false);
			}
			this.slxpApi.comet = this.Comet.IsChecked.Value;
			this.slxpApi.Initialize();
		}

		private void Krnl_Changed(object sender, RoutedEventArgs e)
		{
			if (this.Krnl.IsChecked.Value)
			{
				this.Comet.IsChecked = new bool?(false);
				this.Fluxus.IsChecked = new bool?(false);
				this.Oxygen.IsChecked = new bool?(false);
				this.Electron.IsChecked = new bool?(false);
				this.FurkUltra.IsChecked = new bool?(false);
				this.WeAreDevs.IsChecked = new bool?(false);
			}
			this.slxpApi.krnl = this.Krnl.IsChecked.Value;
			this.slxpApi.Initialize();
		}

		private void KillRobloxButton_Click(object sender, RoutedEventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			for (int i = 0; i < processesByName.Length; i++)
			{
				processesByName[i].Kill();
			}
		}

		private async void ScriptHubSearchBar_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.pageNum = 1;
				this.ScriptHubPanel.Children.Clear();
				await this.LoadScriptHub();
				this.scriptHubScrollBar.ScrollToTop();
			}
		}

		private async void scriptHubScrollBar_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if ((this.scriptHubScrollBar.VerticalOffset == this.scriptHubScrollBar.ScrollableHeight) & this.isLoaded)
			{
				this.pageNum++;
				await this.LoadScriptHub();
			}
		}

		private void Timer()
		{
			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += this.Timer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if ((Process.GetProcessesByName("Discord").Length != 0) & !this.discordRpcClient.IsInitialized)
			{
				this.DiscordRPC();
			}
			else if (Process.GetProcessesByName("Discord").Length == 0)
			{
				this.discordRpcClient.Dispose();
			}
			if (this.slxpApi.IsInjected() & ((string)this.ExploitStatus.Content == "Not attached"))
			{
				this.ExploitStatus.Content = "Exploit attached";
				this.ExploitStatusColor.Fill = new SolidColorBrush(Colors.Lime);
			}
			else if (!this.slxpApi.IsInjected() & ((string)this.ExploitStatus.Content == "Exploit attached"))
			{
				this.ExploitStatus.Content = "Not attached";
				this.ExploitStatusColor.Fill = new SolidColorBrush(Colors.Red);
			}
			if (this.slxpApi.IsInjected() & this.UnlockFPS.IsChecked.Value & !this.unlockedFps)
			{
				this.slxpApi.ExecuteScript("setfpscap(999)");
				this.unlockedFps = true;
				return;
			}
			if (!this.slxpApi.IsInjected() & (!this.UnlockFPS.IsChecked).Value & this.unlockedFps)
			{
				this.unlockedFps = false;
			}
		}

		private async void PopulateTreeView(TreeView treeView)
		{
			treeView.Items.Clear();
			string[] array = Directory.GetDirectories("scripts");
			for (int i = 0; i < array.Length; i++)
			{
				MainWindow.<>c__DisplayClass44_0 CS$<>8__locals1 = new MainWindow.<>c__DisplayClass44_0();
				CS$<>8__locals1.path = array[i];
				DirectoryInfo directoryInfo = new DirectoryInfo(CS$<>8__locals1.path);
				StackPanel stackPanel = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Margin = new Thickness(2.0)
				};
				TextBlock textBlock = new TextBlock
				{
					FontFamily = (base.TryFindResource("Codicon") as FontFamily),
					FontSize = 14.0,
					Text = "\uea83",
					Margin = new Thickness(0.0, 0.0, 3.0, 0.0)
				};
				TextBlock textBlock2 = new TextBlock
				{
					FontSize = 12.0,
					Text = directoryInfo.Name,
					VerticalAlignment = VerticalAlignment.Center
				};
				stackPanel.Children.Add(textBlock);
				stackPanel.Children.Add(textBlock2);
				TreeViewItem treeViewItem = new TreeViewItem
				{
					Header = stackPanel
				};
				ContextMenu contextMenu = new ContextMenu();
				MenuItem menuItem = new MenuItem
				{
					Header = "Delete Directory"
				};
				menuItem.Click += CS$<>8__locals1.<PopulateTreeView>g__OnDeleteDirectoryClick|0;
				contextMenu.Items.Add(menuItem);
				treeViewItem.ContextMenu = contextMenu;
				treeView.Items.Add(treeViewItem);
				await this.PopulateScriptTree(treeViewItem, CS$<>8__locals1.path);
			}
			array = null;
			using (IEnumerator<FileInfo> enumerator = new DirectoryInfo("scripts").GetFilesByExtensions(new string[] { ".txt", ".lua" }).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MainWindow.<>c__DisplayClass44_1 CS$<>8__locals2 = new MainWindow.<>c__DisplayClass44_1();
					CS$<>8__locals2.<>4__this = this;
					CS$<>8__locals2.file = enumerator.Current;
					FileInfo fileInfo = new FileInfo(CS$<>8__locals2.file.FullName);
					StackPanel stackPanel2 = new StackPanel();
					stackPanel2.Orientation = Orientation.Horizontal;
					stackPanel2.Margin = new Thickness(2.0);
					TextBlock textBlock3 = new TextBlock();
					textBlock3.FontFamily = base.TryFindResource("Codicon") as FontFamily;
					textBlock3.FontSize = 14.0;
					textBlock3.Text = "\uea7b";
					textBlock3.Margin = new Thickness(0.0, 0.0, 3.0, 0.0);
					TextBlock textBlock4 = new TextBlock();
					textBlock4.FontSize = 12.0;
					textBlock4.Text = fileInfo.Name;
					textBlock4.VerticalAlignment = VerticalAlignment.Center;
					stackPanel2.Children.Add(textBlock3);
					stackPanel2.Children.Add(textBlock4);
					TreeViewItem treeViewItem2 = new TreeViewItem();
					treeViewItem2.Header = stackPanel2;
					ContextMenu contextMenu2 = new ContextMenu();
					MenuItem menuItem2 = new MenuItem();
					menuItem2.Header = "Execute Script";
					menuItem2.Click += CS$<>8__locals2.<PopulateTreeView>g__OnExecuteClick|1;
					MenuItem menuItem3 = new MenuItem();
					menuItem3.Header = "Load Script";
					menuItem3.Click += CS$<>8__locals2.<PopulateTreeView>g__OnLoadScriptClick|2;
					MenuItem menuItem4 = new MenuItem();
					menuItem4.Header = "Delete Script";
					menuItem4.Click += CS$<>8__locals2.<PopulateTreeView>g__OnDeleteScriptClick|3;
					contextMenu2.Items.Add(menuItem2);
					contextMenu2.Items.Add(menuItem3);
					contextMenu2.Items.Add(menuItem4);
					treeViewItem2.ContextMenu = contextMenu2;
					treeView.Items.Add(treeViewItem2);
				}
			}
		}

		private async Task PopulateScriptTree(ItemsControl treeView, string directory)
		{
			treeView.Items.Clear();
			string[] array = Directory.GetDirectories(directory);
			for (int i = 0; i < array.Length; i++)
			{
				MainWindow.<>c__DisplayClass45_0 CS$<>8__locals1 = new MainWindow.<>c__DisplayClass45_0();
				CS$<>8__locals1.path = array[i];
				DirectoryInfo directoryInfo = new DirectoryInfo(CS$<>8__locals1.path);
				StackPanel stackPanel = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Margin = new Thickness(2.0)
				};
				TextBlock textBlock = new TextBlock
				{
					FontFamily = (base.TryFindResource("Codicon") as FontFamily),
					FontSize = 14.0,
					Text = "\uea83",
					Margin = new Thickness(0.0, 0.0, 3.0, 0.0)
				};
				TextBlock textBlock2 = new TextBlock
				{
					FontSize = 12.0,
					Text = directoryInfo.Name,
					VerticalAlignment = VerticalAlignment.Center
				};
				stackPanel.Children.Add(textBlock);
				stackPanel.Children.Add(textBlock2);
				TreeViewItem treeViewItem = new TreeViewItem
				{
					Header = stackPanel
				};
				ContextMenu contextMenu = new ContextMenu();
				MenuItem menuItem = new MenuItem
				{
					Header = "Delete Directory"
				};
				menuItem.Click += CS$<>8__locals1.<PopulateScriptTree>g__OnDeleteDirectoryClick|0;
				contextMenu.Items.Add(menuItem);
				treeViewItem.ContextMenu = contextMenu;
				treeView.Items.Add(treeViewItem);
				await this.PopulateScriptTree(treeViewItem, CS$<>8__locals1.path);
			}
			array = null;
			using (IEnumerator<FileInfo> enumerator = new DirectoryInfo(directory).GetFilesByExtensions(new string[] { ".txt", ".lua" }).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MainWindow.<>c__DisplayClass45_1 CS$<>8__locals2 = new MainWindow.<>c__DisplayClass45_1();
					CS$<>8__locals2.<>4__this = this;
					CS$<>8__locals2.file = enumerator.Current;
					FileInfo fileInfo = new FileInfo(CS$<>8__locals2.file.FullName);
					StackPanel stackPanel2 = new StackPanel();
					stackPanel2.Orientation = Orientation.Horizontal;
					stackPanel2.Margin = new Thickness(2.0);
					TextBlock textBlock3 = new TextBlock();
					textBlock3.FontFamily = base.TryFindResource("Codicon") as FontFamily;
					textBlock3.FontSize = 14.0;
					textBlock3.Text = "\uea7b";
					textBlock3.Margin = new Thickness(0.0, 0.0, 3.0, 0.0);
					TextBlock textBlock4 = new TextBlock();
					textBlock4.FontSize = 12.0;
					textBlock4.Text = fileInfo.Name;
					textBlock4.VerticalAlignment = VerticalAlignment.Center;
					stackPanel2.Children.Add(textBlock3);
					stackPanel2.Children.Add(textBlock4);
					TreeViewItem treeViewItem2 = new TreeViewItem();
					treeViewItem2.Header = stackPanel2;
					ContextMenu contextMenu2 = new ContextMenu();
					MenuItem menuItem2 = new MenuItem();
					menuItem2.Header = "Execute Script";
					menuItem2.Click += CS$<>8__locals2.<PopulateScriptTree>g__OnExecuteClick|1;
					MenuItem menuItem3 = new MenuItem();
					menuItem3.Header = "Load Script";
					menuItem3.Click += CS$<>8__locals2.<PopulateScriptTree>g__OnLoadScriptClick|2;
					MenuItem menuItem4 = new MenuItem();
					menuItem4.Header = "Delete Script";
					menuItem4.Click += CS$<>8__locals2.<PopulateScriptTree>g__OnDeleteScriptClick|3;
					contextMenu2.Items.Add(menuItem2);
					contextMenu2.Items.Add(menuItem3);
					contextMenu2.Items.Add(menuItem4);
					treeViewItem2.ContextMenu = contextMenu2;
					treeView.Items.Add(treeViewItem2);
				}
			}
		}

		private void DiscordRPC()
		{
			this.discordRpcClient = new DiscordRpcClient("1081841469226242149");
			this.discordRpcClient.Initialize();
			RichPresence richPresence = new RichPresence
			{
				State = "Exploiting Roblox",
				Details = "Find your perfect roblox exploit with Slobby Xploits",
				Assets = new Assets
				{
					LargeImageText = "Slobby Xploits",
					LargeImageKey = "slxp_logo"
				},
				Buttons = new Button[]
				{
					new Button
					{
						Label = "Experience Slobby Xploits",
						Url = "https://direct-link.net/414115/gateway-to-heaven"
					},
					new Button
					{
						Label = "Join our Discord server!",
						Url = "https://discord.gg/hWqcUMWQUs"
					}
				}
			};
			this.discordRpcClient.SetPresence(richPresence);
		}

		private void FileWatcher()
		{
			foreach (string text in new string[] { "*.txt", "*.lua" })
			{
				FileSystemWatcher fileSystemWatcher = new FileSystemWatcher("scripts");
				fileSystemWatcher.Filter = text;
				fileSystemWatcher.Changed += this.OnChanged;
				fileSystemWatcher.Created += this.OnChanged;
				fileSystemWatcher.Deleted += this.OnChanged;
				fileSystemWatcher.Renamed += new RenamedEventHandler(this.OnChanged);
				fileSystemWatcher.IncludeSubdirectories = true;
				fileSystemWatcher.EnableRaisingEvents = true;
			}
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				this.PopulateTreeView(this.ScriptList);
			});
		}

		private void SaveSettings()
		{
			JObject jobject = new JObject(new object[]
			{
				new JProperty("Krnl", this.Krnl.IsChecked),
				new JProperty("Comet", this.Comet.IsChecked),
				new JProperty("Fluxus", this.Fluxus.IsChecked),
				new JProperty("Oxygen", this.Oxygen.IsChecked),
				new JProperty("Electron", this.Electron.IsChecked),
				new JProperty("WeAreDevs", this.WeAreDevs.IsChecked),
				new JProperty("FurkUltra", this.FurkUltra.IsChecked),
				new JProperty("Top Most", this.TopMost.IsChecked),
				new JProperty("Auto Attach", this.AutoAttach.IsChecked),
				new JProperty("Unlock FPS", this.UnlockFPS.IsChecked)
			});
			if (!Directory.Exists(this.settingsFolder))
			{
				Directory.CreateDirectory(this.settingsFolder);
			}
			else
			{
				File.Delete(this.settingsPath);
			}
			File.WriteAllText(this.settingsPath, jobject.ToString());
		}

		private void LoadSettings()
		{
			if (File.Exists(this.settingsPath))
			{
				object obj = JObject.Parse(File.ReadAllText(this.settingsPath));
				ToggleButton krnl = this.Krnl;
				if (MainWindow.<>o__50.<>p__1 == null)
				{
					MainWindow.<>o__50.<>p__1 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target = MainWindow.<>o__50.<>p__1.Target;
				CallSite <>p__ = MainWindow.<>o__50.<>p__1;
				if (MainWindow.<>o__50.<>p__0 == null)
				{
					MainWindow.<>o__50.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				krnl.IsChecked = target(<>p__, MainWindow.<>o__50.<>p__0.Target(MainWindow.<>o__50.<>p__0, obj, "Krnl"));
				ToggleButton comet = this.Comet;
				if (MainWindow.<>o__50.<>p__3 == null)
				{
					MainWindow.<>o__50.<>p__3 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target2 = MainWindow.<>o__50.<>p__3.Target;
				CallSite <>p__2 = MainWindow.<>o__50.<>p__3;
				if (MainWindow.<>o__50.<>p__2 == null)
				{
					MainWindow.<>o__50.<>p__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				comet.IsChecked = target2(<>p__2, MainWindow.<>o__50.<>p__2.Target(MainWindow.<>o__50.<>p__2, obj, "Comet"));
				ToggleButton fluxus = this.Fluxus;
				if (MainWindow.<>o__50.<>p__5 == null)
				{
					MainWindow.<>o__50.<>p__5 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target3 = MainWindow.<>o__50.<>p__5.Target;
				CallSite <>p__3 = MainWindow.<>o__50.<>p__5;
				if (MainWindow.<>o__50.<>p__4 == null)
				{
					MainWindow.<>o__50.<>p__4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				fluxus.IsChecked = target3(<>p__3, MainWindow.<>o__50.<>p__4.Target(MainWindow.<>o__50.<>p__4, obj, "Fluxus"));
				ToggleButton oxygen = this.Oxygen;
				if (MainWindow.<>o__50.<>p__7 == null)
				{
					MainWindow.<>o__50.<>p__7 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target4 = MainWindow.<>o__50.<>p__7.Target;
				CallSite <>p__4 = MainWindow.<>o__50.<>p__7;
				if (MainWindow.<>o__50.<>p__6 == null)
				{
					MainWindow.<>o__50.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				oxygen.IsChecked = target4(<>p__4, MainWindow.<>o__50.<>p__6.Target(MainWindow.<>o__50.<>p__6, obj, "Oxygen"));
				ToggleButton electron = this.Electron;
				if (MainWindow.<>o__50.<>p__9 == null)
				{
					MainWindow.<>o__50.<>p__9 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target5 = MainWindow.<>o__50.<>p__9.Target;
				CallSite <>p__5 = MainWindow.<>o__50.<>p__9;
				if (MainWindow.<>o__50.<>p__8 == null)
				{
					MainWindow.<>o__50.<>p__8 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				electron.IsChecked = target5(<>p__5, MainWindow.<>o__50.<>p__8.Target(MainWindow.<>o__50.<>p__8, obj, "Electron"));
				ToggleButton weAreDevs = this.WeAreDevs;
				if (MainWindow.<>o__50.<>p__11 == null)
				{
					MainWindow.<>o__50.<>p__11 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target6 = MainWindow.<>o__50.<>p__11.Target;
				CallSite <>p__6 = MainWindow.<>o__50.<>p__11;
				if (MainWindow.<>o__50.<>p__10 == null)
				{
					MainWindow.<>o__50.<>p__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				weAreDevs.IsChecked = target6(<>p__6, MainWindow.<>o__50.<>p__10.Target(MainWindow.<>o__50.<>p__10, obj, "WeAreDevs"));
				ToggleButton furkUltra = this.FurkUltra;
				if (MainWindow.<>o__50.<>p__13 == null)
				{
					MainWindow.<>o__50.<>p__13 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target7 = MainWindow.<>o__50.<>p__13.Target;
				CallSite <>p__7 = MainWindow.<>o__50.<>p__13;
				if (MainWindow.<>o__50.<>p__12 == null)
				{
					MainWindow.<>o__50.<>p__12 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				furkUltra.IsChecked = target7(<>p__7, MainWindow.<>o__50.<>p__12.Target(MainWindow.<>o__50.<>p__12, obj, "FurkUltra"));
				ToggleButton topMost = this.TopMost;
				if (MainWindow.<>o__50.<>p__15 == null)
				{
					MainWindow.<>o__50.<>p__15 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target8 = MainWindow.<>o__50.<>p__15.Target;
				CallSite <>p__8 = MainWindow.<>o__50.<>p__15;
				if (MainWindow.<>o__50.<>p__14 == null)
				{
					MainWindow.<>o__50.<>p__14 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				topMost.IsChecked = target8(<>p__8, MainWindow.<>o__50.<>p__14.Target(MainWindow.<>o__50.<>p__14, obj, "Top Most"));
				ToggleButton autoAttach = this.AutoAttach;
				if (MainWindow.<>o__50.<>p__17 == null)
				{
					MainWindow.<>o__50.<>p__17 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target9 = MainWindow.<>o__50.<>p__17.Target;
				CallSite <>p__9 = MainWindow.<>o__50.<>p__17;
				if (MainWindow.<>o__50.<>p__16 == null)
				{
					MainWindow.<>o__50.<>p__16 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				autoAttach.IsChecked = target9(<>p__9, MainWindow.<>o__50.<>p__16.Target(MainWindow.<>o__50.<>p__16, obj, "Auto Attach"));
				ToggleButton unlockFPS = this.UnlockFPS;
				if (MainWindow.<>o__50.<>p__19 == null)
				{
					MainWindow.<>o__50.<>p__19 = CallSite<Func<CallSite, object, bool?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(bool?), typeof(MainWindow)));
				}
				Func<CallSite, object, bool?> target10 = MainWindow.<>o__50.<>p__19.Target;
				CallSite <>p__10 = MainWindow.<>o__50.<>p__19;
				if (MainWindow.<>o__50.<>p__18 == null)
				{
					MainWindow.<>o__50.<>p__18 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(MainWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				unlockFPS.IsChecked = target10(<>p__10, MainWindow.<>o__50.<>p__18.Target(MainWindow.<>o__50.<>p__18, obj, "Unlock FPS"));
			}
		}

		private async Task LoadScriptHub()
		{
			this.isLoaded = false;
			string text = this.ScriptHubSearchBar.Text;
			if (string.IsNullOrEmpty(this.ScriptHubSearchBar.Text))
			{
				text = "a";
			}
			foreach (JToken jtoken in JObject.Parse(await this.httpClient.GetStringAsync("https://scriptblox.com/api/script/search?q=" + text.ToLower() + "&mode=free&page=" + this.pageNum.ToString()))["result"]["scripts"])
			{
				JToken script = jtoken["script"];
				JToken jtoken2 = jtoken["game"];
				string text2 = jtoken["title"].ToString();
				string text3 = jtoken2["name"].ToString();
				string text4 = jtoken2["imageUrl"].ToString();
				if (text4.First<char>() == '/')
				{
					text4 = "https://scriptblox.com" + text4;
				}
				if (text3.Contains("Universal"))
				{
					text3 = "Universal Script!";
				}
				scriptItem scriptItem = new scriptItem();
				scriptItem.Width = 130.0;
				scriptItem.Height = 100.0;
				scriptItem.Margin = new Thickness(5.0);
				scriptItem.scriptName.Content = text2;
				scriptItem.scriptName.ToolTip = text2;
				scriptItem.scriptImage.Source = new BitmapImage(new Uri(text4));
				scriptItem.scriptImage.ToolTip = text3;
				scriptItem.executeBtn.Click += delegate(object _, RoutedEventArgs __)
				{
					this.slxpApi.ExecuteScript(script.ToString());
				};
				scriptItem.copyBtn.Click += delegate(object _, RoutedEventArgs __)
				{
					Clipboard.SetText(script.ToString());
				};
				this.ScriptHubPanel.Children.Add(scriptItem);
			}
			this.isLoaded = true;
		}

		private SlxpApi slxpApi = new SlxpApi();

		private ProcessWatcher processWatcher;

		private DiscordRpcClient discordRpcClient;

		private HttpClient httpClient = new HttpClient();

		private string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Slobby Xploits\\settings.json";

		private string settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Slobby Xploits\\";

		private string appVersion = "1.0.0.0";

		private bool isLoaded;

		private bool unlockedFps;

		private int pageNum = 1;
	}
}
