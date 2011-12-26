using System;
using System.Collections.Generic;
using System.Text;

namespace AvaQuickBoot
{
	public class AvaNew : ICloneable
	{
		public string genre;
		public string content;
		public string date;
		public string url;

		public Object Clone()
		{
			return MemberwiseClone();
		}
	}
}
