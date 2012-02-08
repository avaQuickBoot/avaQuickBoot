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

		public System.Drawing.Color getForeColor()
		{
			System.Drawing.Color c;
			switch (genre)
			{
				case "重要":
					c = System.Drawing.Color.Firebrick;
					break;
				case "イベント":
					c = System.Drawing.Color.MidnightBlue;
					break;
				case "オン大会":
					c = System.Drawing.Color.DarkCyan;
					break;
				case "オフ大会":
					c = System.Drawing.Color.DarkGreen;
					break;
				case "お知らせ":
					c = System.Drawing.Color.DimGray;
					break;
				case "キャンペーン":
					c = System.Drawing.Color.DarkViolet;
					break;
				case "重要なお知らせ":
					c = System.Drawing.Color.Red;
					break;
				default:
					c = System.Drawing.SystemColors.WindowText;
					break;
			}
			return c;
		}

		public System.Drawing.Color getBackColor()
		{
			if (genre == "重要なお知らせ")
				return System.Drawing.Color.Pink;
			return System.Drawing.SystemColors.Window;
		}
	}
}
