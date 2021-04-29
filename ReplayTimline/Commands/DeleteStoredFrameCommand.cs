﻿using System;
using System.Windows;
using System.Windows.Input;

namespace ReplayTimeline
{
	public class DeleteStoredFrameCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public DeleteStoredFrameCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.CurrentTimelineNode != null;
		}

		public void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"Are you sure? This can't be undone.",
						"Delete stored frame?", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No);

			if (confirmationPopUp == MessageBoxResult.Yes)
				ReplayTimelineVM.DeleteStoredFrame();
		}
	}
}
