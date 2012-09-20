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
			account = _account;
			init();
		}

		public loginState(string _id, string _pass, bool _startMumble)
		{
			account = new Account(_id, _pass, "", "", _startMumble);
			init();
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
			a = new AvaQuickBootClass(account.id, account.password, account.startMumble);	//Account.csとAvaQuickBootClass.csは別物なのでAccountのインターフェースを持ちません
			a.OnCompleteHandler += new EventHandler(completed);
			a.OnStateChangeHandler += new EventHandler(stateChanged);
			a.OnGetNewsHandler += new EventHandler(getAvaNews);
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
				//closeTimer.Interval = 10000;
				//closeTimer.Tick += new EventHandler(closeTimer_Tick);
				//closeTimer.Start();

				this.Close();
			}
			else
			{
				this.TopMost = false;
				string errorMessage = (a.getMessage.Length > 0) ? a.getMessage : "不明";
				MessageBox.Show("ログインに失敗しました\n原因: " + errorMessage, "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//this.Close();
				button1.Enabled = false;
			}
		}

		private void stateChanged(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Called stateChanged() value = " + ((int)sender).ToString());

			MethodInvoker p = (MethodInvoker)delegate
			{
				this.progressBar1.Value = (int)sender;

				//this.Refresh();
			};

			if (this.progressBar1.InvokeRequired)
				this.progressBar1.Invoke(p);
			else
				p.Invoke();
		}

		private void getAvaNews(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Called getAvaNews()");

			MethodInvoker p = (MethodInvoker)delegate
			{
				List<AvaNew> news = sender as List<AvaNew>;
				foreach (AvaNew avanew in news)
				{
					ListViewItem cItem = new ListViewItem(avanew.genre);
					cItem.UseItemStyleForSubItems = false;
					cItem.ForeColor = avanew.getForeColor();	//	foreColorは左端だけ
					cItem.SubItems.Add(avanew.content);
					cItem.SubItems.Add(avanew.date);
					cItem.SubItems.Add(avanew.url);
					for (int i = 0; i < cItem.SubItems.Count; i++)
						cItem.SubItems[i].BackColor = avanew.getBackColor();	//backColorは全部
					listView1.Items.Add(cItem);
				}
			};

			if (this.listView1.InvokeRequired)
				this.listView1.Invoke(p);
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

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count < 1) return;
			System.Diagnostics.Process.Start(getUrlFromListViewSelectedItem());
		}

		private string getUrlFromListViewSelectedItem()
		{
			if (listView1.SelectedItems.Count < 1) return "";
			return listView1.SelectedItems[0].SubItems[3].Text;
		}

		private void loginState_Shown(object sender, EventArgs e)
		{
			this.login();
		}


	}
}
