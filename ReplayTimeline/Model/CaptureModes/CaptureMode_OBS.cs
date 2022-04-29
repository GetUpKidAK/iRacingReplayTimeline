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

		public override void ToggleRecording(bool enabled)
		{
			ExternalProcessHelper.SendToggleRecordHotkey(_process, _recordHotkey);
		}
	}
}
