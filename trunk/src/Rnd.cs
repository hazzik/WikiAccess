using System;

namespace WikiTools
{
	public static class Rnd
	{
		private static readonly Random _rnd = new Random();

		public static byte[] RandomBytes(int length)
		{
			var rndbytes = new byte[length];
			_rnd.NextBytes(rndbytes);
			return rndbytes;
		}
	}
}