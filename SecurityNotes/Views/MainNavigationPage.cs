using System;
using Xamarin.Forms;

namespace SecurityNotes {
    public class MainNavigationPage : NavigationPage {
        static MainNavigationPage instance = null;
        public static MainNavigationPage Instance {
            get {
                if(instance == null) {
                    instance = new MainNavigationPage();
                }
                return instance;
            }
        }

        protected MainNavigationPage() { }
    }
}
