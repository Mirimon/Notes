using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SecurityNotes.Core.Model;
using SecurityNotes.Data;
using Xamarin.Forms;

namespace SecurityNotes {
    public partial class NoteDetail : ContentPage {
        bool noteWasEdited = false;

        public NoteDetail() {
            InitializeComponent();
            MainNavigationPage.Instance.Popped += PagePopped;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            if(propertyName == "BindingContext") {
                NoteModel noteModel = BindingContext as NoteModel;

                if (noteModel != null) {
                    noteModel.PropertyChanged += NoteModel_PropertyChanged;
                }
            }
        }

        void PagePopped(object sender, NavigationEventArgs e) {
            if(e.Page == this) {
                MainNavigationPage.Instance.Popped -= PagePopped;
                if(noteWasEdited)
                    SaveNote();

                NoteModel noteModel = BindingContext as NoteModel;

                if (noteModel != null) {
                    noteModel.PropertyChanged -= NoteModel_PropertyChanged;
                }
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

        void NoteModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            noteWasEdited = true;
        }
    }
}
