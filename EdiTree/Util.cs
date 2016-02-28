namespace EdiTree {
	internal class Util {
		public static string Ordinal(int num) {
			var s = num.ToString();
			if (num < 1) { return s; }

			num %= 100;
			if (num == 11 | num == 12 | num == 13) { return s + "th"; }

			switch (num % 10) {
				case 1:
					return s + "st";
				case 2:
					return s + "nd";
				case 3:
					return s + "rd";
				default:
					return s + "th";
			}
		}
	}
}