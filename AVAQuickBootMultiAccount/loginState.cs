using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AvaQuickBoot;

namespace AVAQuickBootMultiAccount
{
	public partial class loginState : Form, IDisposable
	{
		string id;
		string pass;
		bool isWindowMode = false;
		AvaQuickBootClass a = null;
		
		private loginState()
		{
			InitializeComponent();
		}

		public loginState(string _id, string _pass, bool _isWindow)
		{
			this.id = _id;
			this.pass = _pass;
			this.isWindowMode = _isWindow;
			InitializeComponent();
			//login();
		}

		~loginState()
		{
			this.Dispose();
		}

		public new void Dispose()
		{
		}

		public void login()
		{
			a = new AvaQuickBootClass(this.id, this.pass, this.isWindowMode);
			a.OnCompleteHandler += new EventHandler(completed);
			a.OnStateChangeHandler += new EventHandler(stateChanged);
			a.doLoginAsync();
		}

		private void completed(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Called completed()");
			bool succeed = (bool)sender;
			if (succeed)
			{
				System.Threading.Thread.Sleep(4000);
			}
			else
			{
				this.TopMost = false;
				MessageBox.Show("ログインに失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Close();
		}

		private void stateChanged(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Called stateChanged() value = " + ((int)sender).ToString());

			MethodInvoker p = (MethodInvoker)delegate
			{
				this.progressBar1.Value = (int)sender;
				this.Refresh();
			};

			if (this.progressBar1.InvokeRequired)
				this.progressBar1.Invoke(p);
			else
				p.Invoke();
		}
	}
}
