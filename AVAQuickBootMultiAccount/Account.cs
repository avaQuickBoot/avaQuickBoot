
namespace AVAQuickBootMultiAccount
{
	public class Account
	{
		public string id = "";
		public string password = "";
		public string nickName = "";
		public string guid = "";

		public Account(string _id, string _pass, string _nickName, string _guid)
		{
			id = _id;
			password = _pass;
			nickName = _nickName;
			guid = _guid;
		}

		public Account()
		{
			id = "";
			password = "";
			guid = "";
			nickName = "";
		}

	}
}
