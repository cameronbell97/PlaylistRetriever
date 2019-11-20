using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistRetriever.Services
{
    internal static class UriRegistrationService
    {
        // Constants //

        private const string URI_SCHEME = "sptretrieve";
        private const string URL_LOGGEDIN = "loggedin";


        // Static Methods //

        public static void RegisterUriScheme()
        {
            using var key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\Classes\\{URI_SCHEME}");

            string applicationLocation = typeof(App).Assembly.Location;

            key.SetValue("", $"URL:{URL_LOGGEDIN}");
            key.SetValue("URL Protocol", "");

            using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                defaultIcon.SetValue("", $"{applicationLocation},1");

            using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                commandKey.SetValue("", $"\"{applicationLocation}\" \"%1\"");
        }
    }
}
