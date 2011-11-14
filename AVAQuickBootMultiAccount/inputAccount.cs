using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AVAQuickBootMultiAccount
{
	public partial class inputAccount : Form
	{
	   	Account account;
		public EventHandler OnAccountChangedHandler;

		public inputAccount()
		{
			account = new Account("", "", "", false, System.Guid.NewGuid().ToString());
			init();
			button1.Text = "追加";
		}

		public inputAccount(Account a)
		{
			account = a;
			init();
			button1.Text = "変更";
		}

		void init()
		{
			InitializeComponent();
			OnAccountChangedHandler += new EventHandler(empty);

			textBox1.Text = account.nickName;
			textBox2.Text = account.id;
			textBox3.Text = account.password;
			checkBox1.Checked = account.isWindow;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (account.nickName.Length == 0) account.nickName = account.id;
			account.nickName = textBox1.Text;
			account.id = textBox2.Text;
			account.password = textBox3.Text;
			account.isWindow = checkBox1.Checked;

			OnAccountChangedHandler((object)account, e);
		}

		private void button2_Click(object sender, EventArgs e)
		{

		}
		
		void empty(object sender, EventArgs e) { /* 何もしません */ }
	}
}
