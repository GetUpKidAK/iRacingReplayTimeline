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

			return _process != null;
		}

		public override bool IsReadyToRecord()
		{
			return IsAvailable();
		}

		public override void ToggleRecording(bool enabled)
		{
			ExternalProcessHelper.SendToggleRecordHotkey(_process, _recordHotkey);
		}
	}
}
