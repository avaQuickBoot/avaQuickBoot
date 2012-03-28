namespace AVAQuickBootMultiAccount
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.listView1 = new System.Windows.Forms.ListView();
			this.Nickname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.startMumble = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.削除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.ショートカット作成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.button2 = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.button1 = new System.Windows.Forms.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Nickname,
            this.startMumble});
			this.listView1.ContextMenuStrip = this.contextMenuStrip1;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(12, 12);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(537, 303);
			this.listView1.TabIndex = 0;
			this.toolTip1.SetToolTip(this.listView1, "行をダブルクリックすることで、そのアカウントのログインが実行されます。\r\n右クリックすると、そのアカウントの編集や削除、ショートカット作成が行えます。");
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.DoubleClick += new System.EventHandler(this.launchAva);
			// 
			// Nickname
			// 
			this.Nickname.Text = "名前";
			this.Nickname.Width = 400;
			// 
			// startMumble
			// 
			this.startMumble.Text = "Mumbleを起動する";
			this.startMumble.Width = 120;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.編集ToolStripMenuItem,
            this.削除ToolStripMenuItem,
            this.toolStripSeparator1,
            this.ショートカット作成ToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(221, 98);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem1.Text = "このアカウントでAVA実行";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
			// 
			// 編集ToolStripMenuItem
			// 
			this.編集ToolStripMenuItem.Name = "編集ToolStripMenuItem";
			this.編集ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.編集ToolStripMenuItem.Text = "アカウント情報変更";
			this.編集ToolStripMenuItem.Click += new System.EventHandler(this.編集ToolStripMenuItem_Click);
			// 
			// 削除ToolStripMenuItem
			// 
			this.削除ToolStripMenuItem.Name = "削除ToolStripMenuItem";
			this.削除ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.削除ToolStripMenuItem.Text = "アカウント削除";
			this.削除ToolStripMenuItem.Click += new System.EventHandler(this.削除ToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
			// 
			// ショートカット作成ToolStripMenuItem
			// 
			this.ショートカット作成ToolStripMenuItem.Name = "ショートカット作成ToolStripMenuItem";
			this.ショートカット作成ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.ショートカット作成ToolStripMenuItem.Text = "ショートカット作成";
			this.ショートカット作成ToolStripMenuItem.Click += new System.EventHandler(this.ショートカット作成ToolStripMenuItem_Click);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(12, 321);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(125, 41);
			this.button2.TabIndex = 1;
			this.button2.Text = "アカウント追加";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(424, 321);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(125, 41);
			this.button1.TabIndex = 2;
			this.button1.Text = "AVA実行";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(561, 374);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.button2);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader Nickname;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem 編集ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 削除ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ショートカット作成ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ColumnHeader startMumble;
		private System.Windows.Forms.Button button1;

	}
}

