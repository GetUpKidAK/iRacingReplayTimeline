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

			CaptureModeAvailable = _process != null;
			CaptureAvailabilityMessage = CaptureModeAvailable ? "" : "Couldn't find OBS process, please ensure the application is running.";

			return CaptureModeAvailable;
		}

		public override bool IsReadyToRecord()
		{
			return IsAvailable();
		}

		public override void StartRecording()
		{
			ToggleRecording();
		}

		public override void StopRecording()
		{
			ToggleRecording();
		}

		private void ToggleRecording()
		{
			ExternalProcessHelper.SendToggleRecordHotkey(_process, _recordHotkey);
		}
	}
}
