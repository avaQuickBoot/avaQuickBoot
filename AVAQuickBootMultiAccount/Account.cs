
namespace AVAQuickBootMultiAccount
{
	public class Account
	{
		public string id = "";
		public string password = "";
		public bool isWindow = false;
		public string nickName = "";
		public string guid = "";

		public Account(string _id, string _pass, string _nickName, bool _window, string _guid)
		{
			id = _id;
			password = _pass;
			nickName = _nickName;
			isWindow = _window;
			guid = _guid;
		}

		public Account()
		{
			id = "";
			password = "";
			isWindow = false;
			guid = "";
			nickName = "";
		}

	}
}
