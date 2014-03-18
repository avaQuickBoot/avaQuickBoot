using System;
using System.Windows.Forms;

namespace AVAQuickBootMultiAccount
{
	public partial class inputAccount : Form
	{
		private Account account;
		public EventHandler OnAccountChangedHandler;

		public inputAccount()
		{
			account = new Account("", "", "", Guid.NewGuid().ToString(), false);
			init();
			button1.Text = "追加";
		}

		public inputAccount(Account a)
		{
			account = a;
			init();
			button1.Text = "変更";
		}

		private void init()
		{
			InitializeComponent();
			OnAccountChangedHandler += new EventHandler(empty);

			textBox1.Text = account.nickName;
			textBox2.Text = account.id;
			textBox3.Text = "";
			checkBox1.Checked = account.startMumble;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			
			if (account.nickName.Length == 0) account.nickName = account.id;
			account.nickName = textBox1.Text;
			account.id = textBox2.Text;
			account.password = Crypto.EncryptString(textBox3.Text, "5a479051fdc4f85e452370f5d7cb1ba1c2fc560c");
			account.startMumble = checkBox1.Checked;

			OnAccountChangedHandler((object)account, e);
		}

		private void button2_Click(object sender, EventArgs e)
		{
		}

		private void empty(object sender, EventArgs e)
		{
			/* 何もしません */
		}
	}
}