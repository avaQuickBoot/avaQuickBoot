using System;
using System.Windows.Forms;
using System.Diagnostics;

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
		private AvaQuickBootClassParameter p;
		WebBrowser webBrowser;
		Timer loginTimer = new Timer();
		int state = 0;
		int loopCount = 0;
		public EventHandler OnCompleteHandler;
		public EventHandler OnStateChangeHandler;

		public AvaQuickBootClass(string _accountid, string _password, bool _isWindowMode)
		{
			p = new AvaQuickBootClassParameter(_accountid, _password, _isWindowMode);
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
			//第一引数int 内部状態 0~5(変更あるかも どこに最大値を入れようか...)
			OnStateChangeHandler += new EventHandler(empty);
			webBrowser = new WebBrowser();
			webBrowser.AllowNavigation = true;
			webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
			webBrowser.Navigate(p.logoutUri);
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
			doLoginTimer(sender, e);
		}

		public WebBrowser getWebBrowser()
		{
			return webBrowser;	//debug用 実際の動作を目でみたい場合に
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
			bool loginSucceed = doLogin();

			if (this.loopCount > p.loopLimit || loginSucceed)
			{
				loginTimer.Stop();
				System.Threading.Thread.Sleep(1000);
				OnCompleteHandler((object)loginSucceed, e);
			}

		 	System.Threading.Interlocked.Increment(ref this.loopCount);
		}

		bool doLogin()
		{
			if (state == 5)
				return true;

			int previousState = state;

			switch (state)
			{
				case 0:
					if (logoutAva())
						System.Threading.Interlocked.Increment(ref state);
					break;
				case 1:
					if(loginAva())
						System.Threading.Interlocked.Increment(ref state);
					break;
				case 2:
					if(skip1timePasswordPage())
						System.Threading.Interlocked.Increment(ref state);
					break;
				case 3:
					if(setWindowMode())
						System.Threading.Interlocked.Increment(ref state);
					break;
				case 4:
					if(executeAva())
						System.Threading.Interlocked.Increment(ref state);
					break;
			}

			if(previousState != state)
				OnStateChangeHandler((object)state, null);

			Debug.WriteLine("state = " + state);

			return false;
		}

		bool logoutAva()
		{
			if (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
				return false;

			//p.webBrowser.Navigate(p.logoutUri);
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

		private bool setWindowMode()
		{
			if (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
				return false;
			if (!webBrowser.Url.Equals(p.targetUri))
			{
				webBrowser.Navigate(p.targetUri);
				return false;
			}
			HtmlElementCollection windowModeElements = webBrowser.Document.All.GetElementsByName(p.windowModeCheckbox);
			if (windowModeElements.Count < 1) return false;
			
			
			webBrowser.Document.All.GetElementsByName(p.windowModeCheckbox)[0].SetAttribute("checked", (p.isWindowMode) ? "true" : "");
			webBrowser.Document.InvokeScript(p.windowModeButton);
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

			HtmlElement flashElement = webBrowser.Document.GetElementById(p.gameStartFlashButton);
			if (flashElement == null) return false;

			var reg = System.Text.RegularExpressions.Regex.Match(flashElement.OuterHtml, p.gameStartRegex);
			if (!reg.Success)
			{
				return false;
			}

			Debug.WriteLine("num1 = " + reg.Groups["NUM1"]);
			Debug.WriteLine("num2 = " + reg.Groups["NUM2"]);
			Debug.WriteLine("num3 = " + reg.Groups["NUM3"]);

			string[] gameParam = { reg.Groups["NUM1"].Value, reg.Groups["NUM2"].Value, reg.Groups["NUM3"].Value };

			webBrowser.Document.InvokeScript("gameStart", gameParam);

			return true;
		}

		void empty(object sender, EventArgs e) { /* 何もしません */ }
	}

	public class AvaQuickBootClassParameter : IDisposable
	{
		public readonly Uri targetUri = new Uri("http://ava.gamechu.jp/");
		public readonly Uri logoutUri = new Uri("https://api.gamechu.jp/login/logoff?service=ava");
		public readonly string accountBox = "accountid";
		public readonly string passwordBox = "password";
		public readonly string loginButton = "fo_finish";
		public readonly string windowModeButton = "set_window_mode";
		public readonly string windowModeCheckbox = "window_mode";
		public readonly string gameStartFlashButton = "flash_gamestart_loginSWF";
		public readonly string gameStartFlashButtonArgument = "gameStart";
		public readonly int loopLimit = 100;
		public readonly string gameStartRegex = @"gameStart\(\s*'(?<NUM1>([0-9])+)'\s*,\s*(?<NUM2>([0-9])+)\s*,\s*'(?<NUM3>([0-9])+)'\s*\)";

		public string accountid = "";
		public string password = "";
		public bool isWindowMode = false;

		public AvaQuickBootClassParameter()
		{
		}

		public AvaQuickBootClassParameter(string _accountid, string _password, bool _isWindowMode)
		{
			accountid = _accountid;
			password = _password;
			isWindowMode = _isWindowMode;
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
