using System.Windows;
using System.Windows.Controls;


namespace iRacingReplayDirector
{
	public partial class MainWindow : Window
	{
		ReplayDirectorVM _vm;

		public MainWindow()
		{
			InitializeComponent();

			_vm = this.DataContext as ReplayDirectorVM;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_vm.ApplicationClosing(this.RenderSize);
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox listBox = sender as ListBox;
			listBox.ScrollIntoView(listBox.SelectedItem);
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_vm.ShowDriverCameraPanels = e.NewSize.Width > _vm.WidthToDisableSidePanels;

			if (!e.HeightChanged)
				return;

			bool previouslyLowEnoughToDisable = e.PreviousSize.Height < _vm.HeightToDisableControls;
			bool nowHighEnoughToEnable = e.NewSize.Height > _vm.HeightToDisableControls;
			
			// If height was short and is now tall enough
			if (previouslyLowEnoughToDisable && nowHighEnoughToEnable)
			{
				
			}

			// If height was tall enough and is now short enough
			if (!previouslyLowEnoughToDisable && !nowHighEnoughToEnable)
			{
				_vm.ShowVisualTimeline = e.NewSize.Height > _vm.HeightToDisableControls;
				_vm.ShowSessionLapSkipControls = e.NewSize.Height > _vm.HeightToDisableControls;
				_vm.ShowRecordingControls = e.NewSize.Height > _vm.HeightToDisableControls;
			}
		}
	}
}
