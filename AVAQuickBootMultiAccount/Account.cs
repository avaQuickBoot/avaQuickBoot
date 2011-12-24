
namespace AVAQuickBootMultiAccount
{
	public class Account
	{
		public string id = "";
		public string password = "";
		public string nickName = "";
		public string guid = "";
		public bool startMumble = false;

		public Account(string _id, string _pass, string _nickName, string _guid, bool _startMumble)
		{
			id = _id;
			password = _pass;
			nickName = _nickName;
			guid = _guid;
			startMumble = _startMumble;
		}

		public Account()
		{
			id = "";
			password = "";
			guid = "";
			nickName = "";
			startMumble = false;
		}

	}
}
