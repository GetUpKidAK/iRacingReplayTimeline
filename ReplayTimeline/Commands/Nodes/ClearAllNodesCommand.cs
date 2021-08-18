﻿using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ClearAllNodesCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ClearAllNodesCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.TimelineNodes.Count > 0;
		}

		public void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"Are you sure? This can't be undone.",
						"Delete all nodes?", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No);

			if (confirmationPopUp == MessageBoxResult.Yes)
			{
				ReplayDirectorVM.TimelineNodes.Clear();
				ReplayDirectorVM.CurrentTimelineNode = null;

				ReplayDirectorVM.SaveProjectChanges();
			}
		}
	}
}