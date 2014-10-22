using System;
using System.Net.Http.Headers;

namespace Amica.vNext.Http
{
	public class BasicAuthenticator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Amica.vNext.Http.BasicAuthenticator"/> class.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		public BasicAuthenticator (string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public string UserName { get; set;}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string Password { get; set;}

		internal AuthenticationHeaderValue AuthenticationHeader() {
			var s = string.Format("{0}:{1}", UserName, Password);
			return new AuthenticationHeaderValue ( "Basic", Convert.ToBase64String (ToAscii (s)));
		}

		private static byte[] ToAscii(string s) {
	        var r = new byte[s.Length];
	        for (var ix = 0; ix < s.Length; ++ix) {
	            var ch = s[ix];
	            if (ch <= 0x7f) r[ix] = (byte)ch;
	            else r[ix] = (byte)'?';
	        }
	        return r;
	    }	
	}
}

