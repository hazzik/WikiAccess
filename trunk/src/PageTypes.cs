using System;

namespace WikiTools.Access
{
	/// <summary>
	/// Page type for AllPages filtering
	/// </summary>
	public enum PageTypes
	{
		/// <summary>
		/// Allow all pages
		/// </summary>
		All,
		/// <summary>
		/// Allow redirects only
		/// </summary>
		Redirects,
		/// <summary>
		/// Allow everything but redirects
		/// </summary>
		NonRedirects
	}
}