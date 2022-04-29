using System.Diagnostics;


namespace iRacingReplayDirector
{
	public class CaptureMode_OBS : CaptureModeBase
	{
		private Process _process;
		private string _recordHotkey = "^+(R)";

		public CaptureMode_OBS() : base()
		{
			Name = "OBS Studio";
			ProcessName = "obs64";
		}

		public override bool IsAvailable()
		{
			_process = ExternalProcessHelper.GetExternalProcess(ProcessName);

			return _process != null;
		}

		public override bool IsReadyToRecord()
		{
			return IsAvailable();
		}

		public override bool ToggleRecording(bool enabled)
		{
			ExternalProcessHelper.SendToggleRecordHotkey(_process, _recordHotkey);

			//if (UseOBSCapture)
			//{
			//	var obsProcess = ExternalProcessHelper.GetExternalProcess();
			//	if (obsProcess != null)
			//	{
			//		RecordBtnText = enabled ? "Stop Rec" : "Record";
			//		ExternalCaptureActive = enabled;
			//		ExternalProcessHelper.SendToggleRecordMessage(obsProcess);
			//	}
			//	else
			//	{
			//		MessageBox.Show("Couldn't find OBS process, please ensure the application is running.", "Error");

			//		InSimUIEnabled = true;
			//		Sim.Instance.Sdk.Replay.SetPlaybackSpeed(0);
			//		RecordBtnText = "Record";
			//	}
			//	return;
			//}

			return false;
		}
	}
}
