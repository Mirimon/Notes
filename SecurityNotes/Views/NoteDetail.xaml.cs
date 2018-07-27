using System;
using System.Collections.Generic;
using SecurityNotes.Core.Model;
using SecurityNotes.Data;
using Xamarin.Forms;

namespace SecurityNotes {
    public partial class NoteDetail : ContentPage {
        public NoteDetail() {
            InitializeComponent();
            MainNavigationPage.Instance.Popped += PagePopped;
        }

        void PagePopped(object sender, NavigationEventArgs e) {
            if(e.Page == this) {
                MainNavigationPage.Instance.Popped -= PagePopped;
                SaveNote();
            }
        }


        void Done_Clicked(object sender, EventArgs e) {
            SaveNote();
        }

        void SaveNote() {
            NoteModel noteModel = BindingContext as NoteModel;

            if(noteModel != null) {
                noteModel.ChangedTime = DateTime.Now;
                DataProvider.Instance.SaveNote(noteModel);
            }
        }
    }
}
