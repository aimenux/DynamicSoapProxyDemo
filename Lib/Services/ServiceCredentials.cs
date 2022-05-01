using System;

namespace Lib.Services
{
    public class SoapServiceCredentials
    {
        private const string Suffix = "?wsdl";

        public string Url { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public SoapServiceCredentials(string url, string username = null, string password = null)
        {
            Url = url;
            Username = username;
            Password = password;
            if (string.IsNullOrWhiteSpace(Url)) return;
            if (!Url.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
            {
                Url = $"{Url}{Suffix}";
            }
        }

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            try
            {
                var queryUri = new UriBuilder(url).Query.ToLower();
                return queryUri.Equals(Suffix);
            }
            catch
            {
                return false;
            }
        }
    }
}