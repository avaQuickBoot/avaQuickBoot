using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;


/*こいつがやること
 * 外からインスタンス生成
 * 外からdoLoginAsync()呼び出す
 * doLoginAsync()が呼び出されるとtimer or webBrowser_DocumentComletedがdoLoginTimer()を呼び出す
 * doLoginTimer()はdoLogin()を呼び出す
 * doLogin()は各手続きを行ってstateを変化させる
 * avaが実行される
 * stateが最終状態になればdoLogin()はtrueを返すようになる
 * doLoginTimerはdoLogin()からtrueが帰ってくるようになればOnCompleteHandlerを投げてtimerを終了させる
 * おわり
 * 
 * 注意
 *	こいつからOnCompleteHandlerが帰ってきてもすぐに終了させないこと
 *	何故か起動時にブラウザが生きてないと正常にavaが実行されない
 */


namespace AvaQuickBoot
{
	public class AvaQuickBootClass : IDisposable
	{
#region 変数等定義
		private AvaQuickBootClassParameter p;
		WebBrowser webBrowser;
		Timer loginTimer = new Timer();
		int state = 0;	//現在のステート
		int loopCount = 0;	//何回doLogin()したか
		int stateStayCount = 0;		//何回同じstateを試行したか
		bool isCancel = false;
		string message = "";
		public EventHandler OnCompleteHandler;
		public EventHandler OnStateChangeHandler;
		public EventHandler OnGetNewsHandler;
		List<AvaNew> avaNews = new List<AvaNew>();

		public string getMessage
		{
			get { return message; }
			set { }
		}
		public int getFinalStateNumber
		{
			get { return p.finalStateNumber; }
			set { }
		}
#endregion

		public AvaQuickBootClass(string _accountid, string _password, bool _startMumble)
		{
			p = new AvaQuickBootClassParameter(_accountid, _password, _startMumble);
			init();
		}

		public AvaQuickBootClass(AvaQuickBootClassParameter _p)
		{
			p = _p;	
			init();
		}

		void init()
		{
			state = 0;
			loopCount = 0;
			//第一引数bool ログオンが完了したか true:完了, false:失敗
			OnCompleteHandler += new EventHandler(empty);			 
			//第一引数int 内部状態 0~getFinalStateNumber(p.finalStateNumber)
			OnStateChangeHandler += new EventHandler(empty);
			//第一引数List<AvaNew>
			OnGetNewsHandler += new EventHandler(empty);
			webBrowser = new WebBrowser();
			webBrowser.AllowWebBrowserDrop = false;
			webBrowser.ScriptErrorsSuppressed = true;
			webBrowser.AllowNavigation = true;
			webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
			//webBrowser.Navigate(p.logoutUri);	//logoutする
			webBrowser.Navigate(p.targetUri);
		}

		~AvaQuickBootClass()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (p != null) p.Dispose();
		}

		

		void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			bool isOverLoopLimit = this.loopCount > p.loopLimit;
			bool isOverStateStayLimit = this.stateStayCount > p.stateStayLimit;
			if (isCancel || isOverLoopLimit || isOverStateStayLimit) return;
			doLoginTimer(sender, e);
		}

		/// <summary>
		/// (たぶん)非同期でAVAにログインします
		/// 内部状態が変化するとOnStateCHangeHandlerが、ログインが終了したらOnCompleteHandlerが呼ばれます
		/// </summary>
		public void doLoginAsync()
		{
			loginTimer.Tick += new EventHandler(doLoginTimer);
			loginTimer.Interval = 1000;		//doLoginTimerは基本的にwebBrowser_DocumentCompletedに回してもらう 万が一止まったときのために
			loginTimer.Start();
		}

		void doLoginTimer(object sender, EventArgs e)
		{
			bool isOverLoopLimit = this.loopCount > p.loopLimit;
			bool isOverStateStayLimit = this.stateStayCount > p.stateStayLimit;

#region ログイン出来ないときの処理
			if (isCancel || isOverLoopLimit || isOverStateStayLimit)
			{
				if (isOverLoopLimit)
					this.message = "ログイン試行回数";
				if(isCancel)
					this.message = "ログイン処理がキャンセルされました。";
				if (isOverStateStayLimit)
				{
					switch (state)
					{
						case 0:
							this.message = "AVAウェブサイトに接続出来ません。";
							break;
						case 1:
							this.message = "アカウントにログインできません。";
							break;
						case 2:
							this.message = "セキュリティーロック画面から移行できません。携帯電話を使用したワンタイムパスワードなどを設定している場合は解除してください。"
								+ "\nメンテナンス中の可能性もあります。";
							break;
						case 3:
							this.message = "起動に必要な鍵が取得できません。アカウント情報が間違っていないか確認してください。";
							break;
						default:
							this.message = "不明な状態に遷移しています。";
							break;
					}
				}

				loginTimer.Stop();
				OnCompleteHandler((object)false, e);
				return;
			}
#endregion

			bool loginSucceed = doLogin();

			//ログイン成功
			if (loginSucceed)
			{
				loginTimer.Stop();
				System.Threading.Thread.Sleep(1000);
				OnCompleteHandler((object)loginSucceed, e);
			}
			
			System.Diagnostics.Debug.WriteLine("loopCount = " + loopCount);
		 	System.Threading.Interlocked.Increment(ref this.loopCount);
		}

		bool doLogin()	//同時に複数走る可能性がある
		{
			//最終状態なら
			if (state == p.finalStateNumber)
				return true;

			bool isSucceed = false;
			int previousState = state;

			switch (state)
			{
				case 0:
					isSucceed = logoutAva();
					break;
				case 1:
					if (parseAvaNews()) OnGetNewsHandler(avaNews, null);
					isSucceed = true;	//データが取ってこれる保証はない しかし取れなくても別に致命的ではない
					break;
				case 2:
					isSucceed = loginAva();
					break;
				case 3:
					isSucceed = skip1timePasswordPage();
					break;
				case 4:
					isSucceed = executeAva();
					break;
			}

			if (isSucceed)	//ステート処理成功
			{
				System.Threading.Interlocked.Increment(ref state);
				System.Threading.Interlocked.Exchange(ref stateStayCount, 0);	// state = 0;
				OnStateChangeHandler((object)state, null);
			}
			else
			{
				System.Threading.Interlocked.Increment(ref stateStayCount);
			}

			Debug.WriteLine("state = " + state);
			Debug.WriteLine("stateStayCount = " + stateStayCount);

			return false;
		}

		bool logoutAva()
		{
			if (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
				return false;
			if (!webBrowser.Url.Equals(p.targetUri))
				return false;

			return true;
		}

		private bool loginAva()
		{
			if (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
				return false;
			if (!webBrowser.Url.Equals(p.targetUri))
				return false;

			HtmlElementCollection accountElements = webBrowser.Document.All.GetElementsByName(p.accountBox);
			HtmlElementCollection passwordElements = webBrowser.Document.All.GetElementsByName(p.passwordBox);
			if (accountElements.Count != 1 || passwordElements.Count != 1)
			{
				Debug.WriteLine("Fatal Error: Elementが取得できませんでした");
				return false;
			}

			webBrowser.Document.All.GetElementsByName(p.accountBox)[0].InnerText = p.accountid;
			webBrowser.Document.All.GetElementsByName(p.passwordBox)[0].InnerText = p.password;
 
			webBrowser.Document.InvokeScript(p.loginButton);

			return true;
		}

		private bool skip1timePasswordPage()
		{
			if (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
				return false;
			if (webBrowser.Url.Equals(p.targetUri))
				return true;

			webBrowser.Navigate(p.targetUri);

			return true;
		}

		private bool executeAva()
		{
			if (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
				return false;
			if (!webBrowser.Url.Equals(p.targetUri))
			{
				webBrowser.Navigate(p.targetUri);
				return false;
			}

			System.Text.RegularExpressions.Match reg = null;
			foreach (HtmlElement html in webBrowser.Document.All)
			{
				if (html.OuterHtml == null) continue;
				reg = System.Text.RegularExpressions.Regex.Match(html.OuterHtml, p.gameStartRegex);
				if (reg.Success)break;
			}
			if (reg == null || !reg.Success) return false;

			string[] gameParam = { reg.Groups["NUM1"].Value, reg.Groups["NUM2"].Value, reg.Groups["NUM3"].Value };
			for (int i = 0; i < gameParam.Length; i++)
				Debug.WriteLine("num" + i + " = " + gameParam[i]);

			webBrowser.Document.InvokeScript(p.gameStartFlashButtonArgument, gameParam);
			if (p.startMumble) webBrowser.Document.InvokeScript(p.mumbleStartButton);

			return true;
		}

		bool parseAvaNews()
		{
			//重要なお知らせ(取得できない可能性がある)
			while (true)
			{
				HtmlElement impHe = webBrowser.Document.GetElementById("news_container");
				if (impHe == null) break;

				HtmlElementCollection impHeCollection = impHe.All;
				foreach (HtmlElement he in impHeCollection)
				{
					if (he.TagName == "DIV" && he.GetAttribute("ClassName") == "win_imp")
					{
						foreach (HtmlElement he2 in he.All)
						{
							if (he2.TagName == "A")
							{
								System.Diagnostics.Debug.WriteLine(he2.InnerText);
								System.Diagnostics.Debug.WriteLine(he2.GetAttribute("Href"));
								AvaNew avaImpNew = new AvaNew();
								avaImpNew.genre = "重要なお知らせ";
								avaImpNew.content = he2.InnerText;
								avaImpNew.date = "";
								avaImpNew.url = he2.GetAttribute("Href");
								avaNews.Add(avaImpNew);
							}
						}
					}
				}
				break;
			}


			HtmlElement newsContentHe = webBrowser.Document.GetElementById("news1");
			if (newsContentHe == null) return false;

			HtmlElementCollection heCollection = newsContentHe.All;
			List<HtmlElement> contentHeList = new List<HtmlElement>();
			foreach (HtmlElement he in heCollection)
			{
				if (he.TagName == "LI" && he.GetAttribute("ClassName") == "normal")
				{
					foreach (HtmlElement he2 in he.All)
					{
						contentHeList.Add(he2);
					}
				}
			}
			if (contentHeList.Count == 0) return false;

			AvaNew avaNew = new AvaNew();
			foreach (HtmlElement he in contentHeList)
			{
				bool isGetDate = false;

				if (he.TagName == "DIV" && he.GetAttribute("ClassName") == "iocn")
				{
					if (he.All.Count == 0) return false;
					avaNew.genre = he.All[0].GetAttribute("Alt");
				}
				if (he.TagName == "DIV" && he.GetAttribute("ClassName") == "cont")
				{
					if (he.All.Count == 0) return false;
					avaNew.content = he.All[0].InnerText.Replace("\r\n", "");
					avaNew.url = he.All[0].GetAttribute("Href");
				}
				if (he.TagName == "DIV" && he.GetAttribute("ClassName") == "date")
				{
					avaNew.date = he.InnerText;
					isGetDate = true;
				}

				if (isGetDate)
				{
					avaNews.Add(avaNew.Clone() as AvaNew);	//AvaNewは参照型のため、Cloneしないとaddしたものが全て同じオブジェクトを参照してしまう。
				}
			}

			if (avaNews.Count > 0)
				return true;
			return false;
		}

		public void cancel()
		{
			isCancel = true;
		}

		void empty(object sender, EventArgs e) { /* 何もしません */ }
	}

	public class AvaQuickBootClassParameter : IDisposable
	{
		public readonly Uri targetUri = new Uri("http://ava.pmang.jp/");
		public readonly Uri logoutUri = new Uri("https://api.pmang.jp/login/logoff?service=ava");
		public readonly string accountBox = "accountid";
		public readonly string passwordBox = "password";
		public readonly string loginButton = "fo_finish";
		public readonly string gameStartFlashButton = "flash_gamestart_loginSWF";
		public readonly string gameStartFlashButtonArgument = "gameStart";
		public readonly string mumbleStartButton = "readyVoicechat";
		public int loopLimit = 100;	//味付け 開発環境では、25回程度で起動
		public int stateStayLimit = 30;		//同じステートを何回再試行するか
		public readonly string gameStartRegex = @"gameStart\(\s*'(?<NUM1>([0-9])+)'\s*,\s*(?<NUM2>([0-9])+)\s*,\s*'(?<NUM3>([0-9])+)'\s*\)";
		public readonly int finalStateNumber = 5;

		public string accountid = "";
		public string password = "";
		public bool startMumble = false;

		public AvaQuickBootClassParameter()
		{
		}

		public AvaQuickBootClassParameter(string _accountid, string _password, bool _startMumble)
		{
			accountid = _accountid;
			password = _password;
			startMumble = _startMumble;
		}

		~AvaQuickBootClassParameter()
		{
			this.Dispose();
		}

		public void Dispose()
		{
		}
	}

}
