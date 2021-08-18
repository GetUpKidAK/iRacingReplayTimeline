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

			if (e.PreviousSize.Height == 0 || e.PreviousSize.Width == 0)
				return;

			if (e.PreviousSize.Height < _vm.HeightToDisableOneControl)
			{
				_vm.ShowVisualTimeline = e.NewSize.Height > _vm.HeightToDisableOneControl;
			}

			if (e.PreviousSize.Height < _vm.HeightToDisableAllControls)
			{
				_vm.ShowSessionLapSkipControls = e.NewSize.Height > _vm.HeightToDisableAllControls;
				_vm.ShowRecordingControls = e.NewSize.Height > _vm.HeightToDisableAllControls;
			}
		}
	}
}
