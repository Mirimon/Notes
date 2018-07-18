using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dropbox.Api;
using SecurityNotes.Data;
using Xamarin.Forms;

namespace SecurityNotes {
    public partial class AuthLogin : ContentPage {
        const string redirectUrl = @"http://localhost:42434/login";
        const string apiKey = "q895k94ejejcblq";
        const string appSecret = "4r9x6ab8d4t0lat";
        string ConnectState { get; set; }

        public AuthLogin() {
            InitializeComponent();
            ConnectState = Guid.NewGuid().ToString("N");
            Uri authorizeUrl = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, apiKey, redirectUrl, ConnectState);
            //Uri authorizeUrl = DropboxOAuth2Helper.GetAuthorizeUri(apiKey);

            //authorizeUrl = new Uri(authorizeUrl.AbsoluteUri.Replace("%2F", "/"));
            webView.Source = authorizeUrl;
        }

        async Task WebView_Navigating(object sender, WebNavigatingEventArgs e) {
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

            string code = e.Url.Substring(codePosition + codeId.Length);
            if (string.IsNullOrEmpty(code))
                return;
            
            OAuth2Response auth2Response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, apiKey, appSecret, redirectUrl);
            DataProvider.Instance.SetAccessToken(auth2Response.AccessToken);

            await MainNavigationPage.Instance.PopAsync(false);
        }

    }
}
