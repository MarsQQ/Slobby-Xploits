using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Slobby_Xploits__WPF.Assets
{
	public partial class newUpdate : UserControl
	{
		public newUpdate()
		{
			this.InitializeComponent();
		}

		private void DownloadSlxp_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://direct-link.net/414115/gateway-to-heaven");
		}
	}
}
