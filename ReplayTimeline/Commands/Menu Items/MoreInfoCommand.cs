﻿using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class MoreInfoCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public MoreInfoCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"Visit the GitHub project page for more info?",
						"More Info", MessageBoxButton.OKCancel, MessageBoxImage.None, MessageBoxResult.OK);

			if (confirmationPopUp == MessageBoxResult.OK)
			{
				var destinationurl = "https://github.com/GetUpKidAK/iRacingSequenceDirector";
				var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
				{
					UseShellExecute = true,
				};
				System.Diagnostics.Process.Start(sInfo);
			}
		}
	}
}