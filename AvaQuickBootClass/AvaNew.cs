using System;
using System.Collections.Generic;
using System.Drawing;
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

		public Color getForeColor()
		{
			Color c;
			switch (genre)
			{
				case "重要":
					c = Color.Firebrick;
					break;
				case "イベント":
					c = Color.MidnightBlue;
					break;
				case "オン大会":
					c = Color.DarkCyan;
					break;
				case "オフ大会":
					c = Color.DarkGreen;
					break;
				case "お知らせ":
					c = Color.DimGray;
					break;
				case "キャンペーン":
					c = Color.DarkViolet;
					break;
				case "重要なお知らせ":
					c = Color.Red;
					break;
				default:
					c = SystemColors.WindowText;
					break;
			}
			return c;
		}

		public Color getBackColor()
		{
			if (genre == "重要なお知らせ")
				return System.Drawing.Color.Pink;
			return SystemColors.Window;
		}
	}
}
