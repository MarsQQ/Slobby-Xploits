using System;
using System.Windows;

namespace Slobby_Xploits__WPF_.Assets
{
	public static class Messages
	{
		public static bool ShowGenericWarningMessage(string message = "This action is irreversible, do it anyways?")
		{
			return MessageBox.Show(message, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
		}

		public static bool ShowGenericQuestionMessage(string message = "This is a question.")
		{
			return MessageBox.Show(message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		}

		public static void ShowGenericErrorMessage(string message = "An error has occured!")
		{
			MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
