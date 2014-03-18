using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace AVAQuickBootMultiAccount
{
	public partial class Form1 : Form
	{
		List<Account> accountList = new List<Account>();

		public Form1()
		{
			InitializeComponent();

			FileVersionInfo ver = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
			this.Text = ver.ProductName + " ver: " + ver.ProductVersion;
			loadAccountFromXml();
			loadListviewItemsFromAccount();

			if (listView1.Items.Count > 0)
			{
				listView1.Items[0].Selected = true;
			}
		}

		void loadAccountFromXml()
		{
			try
			{
				string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				if (!File.Exists(exePath + @"\account.xml")) return;

				TextReader tr = new StreamReader(exePath + @"\account.xml");
				XmlSerializer xs = new XmlSerializer(typeof(List<Account>));
				accountList = xs.Deserialize(tr) as List<Account>;
				tr.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}

		void loadListviewItemsFromAccount()
		{
			foreach (Account account in accountList)
			{
				addListView(account);
			}
		}

		void addAccount(Account account)
		{
			//listview
			addListView(account);

			//accountlist
			accountList.Add(account);
		}

		void addListView(Account account)
		{
			ListViewItem cItem = new ListViewItem(account.nickName);
			cItem.SubItems.Add(account.startMumble ? "Yes" : "No");
			cItem.SubItems.Add(account.guid);
			listView1.Items.Add(cItem);
		}

		void removeAccout(Account account)
		{
			//listview
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				//GUID
				if (listView1.Items[i].SubItems[2].Text.Equals(account.guid))
				{
					listView1.Items.RemoveAt(i);
				}
			}

			//accountlist
			for (int i = 0; i < accountList.Count; i++)
			{
				//もしguidがかぶったら(編集時)
				if (accountList[i].guid.Equals(account.guid))
				{
					accountList.RemoveAt(i);
					break;
				}
			}
		}


		private void button2_Click(object sender, EventArgs e)
		{
			inputAccount inputForm = new inputAccount();
			inputForm.OnAccountChangedHandler += new EventHandler(onAddAccount);
			inputForm.ShowDialog();
		}

		private void launchAva(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count < 1) return;
			string guid = getGuidFromListViewSelectedItem();

			foreach (Account account in accountList)
			{
				if (account.guid.Equals(guid))
				{
					loginState form = new loginState(account.id, Crypto.DecryptString(account.password, "5a479051fdc4f85e452370f5d7cb1ba1c2fc560c"), account.startMumble);
					this.Hide();
					form.ShowDialog();
					form.Dispose();
					this.Close();
					break;
				}
			}
		}

		private void onAddAccount(object sender, EventArgs e)
		{
			Account account = sender as Account;

			for (int i = 0; i < accountList.Count; i++)
			{
				//もしguidがかぶったら(編集時)
				if (accountList[i].guid.Equals(account.guid))
				{
					removeAccout(accountList[i]);
					break;
				}
			}

			addAccount(account);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				TextWriter tw = new StreamWriter(exePath + @"\account.xml");
				XmlSerializer xs = new XmlSerializer(typeof(List<Account>));
				xs.Serialize(tw, accountList);
				tw.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}

		private void 編集ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Account account = new Account();
			if (listView1.SelectedItems.Count < 1) return;
			string guid = getGuidFromListViewSelectedItem();

			foreach (Account a in accountList)
			{
				if (a.guid.Equals(guid))
				{
					account = a;
				}
			}

			inputAccount inputForm = new inputAccount(account);
			inputForm.OnAccountChangedHandler += new EventHandler(onAddAccount);
			inputForm.ShowDialog();
		}

		private void 削除ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count < 1) return;
			if (MessageBox.Show("本当にアカウント\"" + listView1.SelectedItems[0].Text + "\"を削除してもよろしいですか？", "確認",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}

			string guid = getGuidFromListViewSelectedItem();
			removeAccout(new Account("", "", "", guid, false));
		}

		private void ショートカット作成ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count < 1) return;
			string guid = getGuidFromListViewSelectedItem();
			Account account = null;

			foreach (Account a in accountList)
			{
				if (a.guid.Equals(guid))
				{
					account = a;
				}
			}
			if (account == null) return;


			SaveFileDialog saveDialog = new SaveFileDialog();
			string arg = " \"" + account.id + "\" \"" + account.password + "\" \"" + account.startMumble.ToString() + "\"";

			saveDialog.FileName = "AVA_" + validFileName(account.nickName);
			saveDialog.Filter = "shortcut file|*.lnk";
			if (saveDialog.ShowDialog() != DialogResult.OK) return;

			Type shellType = Type.GetTypeFromProgID("WScript.Shell");
			object shell = Activator.CreateInstance(shellType);
			object shortCut = shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { saveDialog.FileName });

			Type shortcutType = shell.GetType();
			shortcutType.InvokeMember("TargetPath", BindingFlags.SetProperty, null, shortCut, new object[] { Application.ExecutablePath });
			shortcutType.InvokeMember("Arguments", BindingFlags.SetProperty, null, shortCut, new object[] { arg });
			shortcutType.InvokeMember("Description", BindingFlags.SetProperty, null, shortCut, new object[] { "Shortcut" });
			shortcutType.InvokeMember("Save", BindingFlags.InvokeMethod, null, shortCut, null);

			MessageBox.Show("ショートカットを作成しました", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count < 1) return;
			launchAva(sender, e);
		}

		private string getGuidFromListViewSelectedItem()
		{
			if (listView1.SelectedItems.Count < 1) return "";
			return listView1.SelectedItems[0].SubItems[2].Text;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			launchAva(sender, e);
		}

		private string validFileName(string s)
		{
			string valid = s;
			char[] invalidch = Path.GetInvalidFileNameChars();

			foreach (char c in invalidch)
			{
				valid = valid.Replace(c, '_');
			}
			return valid.Clone() as string;
		}
	}
}
