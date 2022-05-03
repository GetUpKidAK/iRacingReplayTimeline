using System.Diagnostics;


namespace iRacingReplayDirector
{
	public class CaptureMode_ShadowPlay : CaptureModeBase
	{
		private Process _process;
		private string _recordHotkey = "%{F9}";

		public CaptureMode_ShadowPlay() : base()
		{
			Name = "Nvidia ShadowPlay";
			ProcessName = "nvsphelper64";
		}

		public override bool IsAvailable()
		{
			_process = ExternalProcessHelper.GetExternalProcess(ProcessName);

			CaptureModeAvailable = _process != null;
			CaptureAvailabilityMessage = CaptureModeAvailable ? "" : "Couldn't find the ShadowPlay process, please ensure the application is running.";

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
