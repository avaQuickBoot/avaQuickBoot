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
		Account account;
		AvaQuickBootClass a = null;
		Timer closeTimer = null;
		
		private loginState()
		{
			init();
		}

		public loginState(Account _account)
		{
			init();
			account = _account;
		}

		public loginState(string _id, string _pass)
		{
			init();
			account = new Account(_id, _pass, "", "");
		}

		void init()
		{
			InitializeComponent();
			closeTimer = new Timer();
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
			a = new AvaQuickBootClass(account.id, account.password);	//Account.csとAvaQuickBootClass.csは別物なのでAccountのインターフェースを持ちません
			a.OnCompleteHandler += new EventHandler(completed);
			a.OnStateChangeHandler += new EventHandler(stateChanged);
			this.progressBar1.Maximum = a.getFinalStateNumber;
			a.doLoginAsync();
		}

		private void completed(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Called completed()");
			bool succeed = (bool)sender;
			if (succeed)
			{
				this.Refresh();	//この時点でformはbusy
				this.TopMost = false;
				closeTimer.Interval = 3000;
				closeTimer.Tick += new EventHandler(closeTimer_Tick);
				closeTimer.Start();
			}
			else
			{
				this.TopMost = false;
				string errorMessage = (a.getMessage.Length > 0) ? a.getMessage : "不明";
				MessageBox.Show("ログインに失敗しました\n原因: " + errorMessage, "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
			}
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

		private void closeTimer_Tick(object sender, EventArgs e)
		{
			closeTimer.Stop();
			//System.Threading.Thread.Sleep(3000);
			this.Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			a.cancel();
		}

	}
}
