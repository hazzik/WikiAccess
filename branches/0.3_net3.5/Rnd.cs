using System;
using System.Collections.Generic;
using System.Text;

namespace WikiTools 
{
	public static class Rnd 
	{
		private static readonly Random _rnd = new Random();

		public static byte[] RandomBytes(int length) 
		{
			byte[] rndbytes = new byte[length];
			_rnd.NextBytes(rndbytes);
			return rndbytes;
		}
	}
}
