using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dropbox.Api;
using Xamarin.Forms;

namespace SecurityNotes {
    public partial class AuthLogin : ContentPage {
        const string redirectUrl = @"http://localhost:42434/login";
        const string apiKey = "q895k94ejejcblq";
        string ConnectState { get; set; }
        string Code { get; set; }

        public AuthLogin() {
            InitializeComponent();
            ConnectState = Guid.NewGuid().ToString("N");
            Uri authorizeUrl = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, apiKey, redirectUrl, ConnectState);
            //Uri authorizeUrl = DropboxOAuth2Helper.GetAuthorizeUri(apiKey);

            //authorizeUrl = new Uri(authorizeUrl.AbsoluteUri.Replace("%2F", "/"));
            webView.Source = authorizeUrl;
        }

        void WebView_Navigating(object sender, WebNavigatingEventArgs e) {
            //Regex responceUrlRegex = new Regex(@"http://localhost:42434/login?state={}&code={}");
            string start = @"http://localhost:42434/login?state=";
            string codeId = @"&code=";
            if (!e.Url.StartsWith(start, StringComparison.InvariantCultureIgnoreCase))
                return;
            
            int codePosition = e.Url.IndexOf(codeId, StringComparison.InvariantCultureIgnoreCase);
            if (codePosition == -1)
                return;

            string state = e.Url.Substring(start.Length, codePosition - start.Length);
            if (!state.ToLower().Equals(ConnectState.ToLower()))
                return;

            Code = e.Url.Substring(codePosition + codeId.Length);
            //"http://localhost:42434/login?state=0fc252cb204e46878f080712dbf5a524&code=WzAwaaRn3JMAAAAAAAAGBtGJSms2wP_gsgCLPPYqR9c"
        }

    }
}
