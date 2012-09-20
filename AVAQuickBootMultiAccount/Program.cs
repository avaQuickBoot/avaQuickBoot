using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
 
namespace AVAQuickBootMultiAccount
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		static System.Threading.Mutex mutex;

		[STAThread]
		static void Main(string[] arg)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			try
			{
				mutex = new System.Threading.Mutex(false, @"{747690E4-BCE9-4949-9B79-FBAC7E700D63}");
				if (!mutex.WaitOne(0, false))
				{
					Process prevProcess = GetPreviousProcess();
					if (prevProcess != null && prevProcess.MainWindowHandle != IntPtr.Zero)
					{
						// 起動中のアプリケーションを最前面に表示
						WakeupWindow(prevProcess.MainWindowHandle);
					}
					return;
				}

				switch (arg.Length)
				{
					case 0:
						Application.Run(new Form1());
						break;
					case 3:
						loginState form = new loginState(arg[0], arg[1], bool.Parse(arg[2]));
						//form.login();
						Application.Run(form);
						break;
				}
			}
			finally
			{
				mutex.Close();
			}
		}


		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd);
		private const int SW_RESTORE = 9;  // 画面を元の大きさに戻す

		public static void WakeupWindow(IntPtr hWnd)
		{
			if (IsIconic(hWnd))
			{
				ShowWindowAsync(hWnd, SW_RESTORE);
			}

			SetForegroundWindow(hWnd);
		}

		public static Process GetPreviousProcess()
		{
			Process curProcess = Process.GetCurrentProcess();
			Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

			foreach (Process checkProcess in allProcesses)
			{
				// 自分自身のプロセスIDは無視する
				if (checkProcess.Id != curProcess.Id)
				{
					if (String.Compare(checkProcess.MainModule.FileName, curProcess.MainModule.FileName, true) == 0)
					{
						return checkProcess;
					}
				}
			}
			return null;
		}

	}
}
