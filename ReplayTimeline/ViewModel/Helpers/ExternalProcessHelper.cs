using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;


namespace iRacingReplayDirector
{
	public class ExternalProcessHelper
	{
		[DllImport("User32.dll")]
		static extern int SetForegroundWindow(IntPtr point);

		public static Process GetObsProcess()
		{
			return Process.GetProcessesByName("obs64").FirstOrDefault();
		}

		public static void SendToggleRecordMessage(Process p)
		{
			IntPtr h = p.MainWindowHandle;
			SetForegroundWindow(h);
			System.Windows.Forms.SendKeys.SendWait("^+(R)");
		}
	}
}
