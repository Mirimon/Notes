using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SecurityNotes.Data;
using Core.Model;

namespace SecurityNotes.Views {
    public partial class NotesList : ContentPage {
        public NotesList() {
            InitializeComponent();
            listView.ItemsSource = DataProvider.Instance.Notes;
            listView.ItemTapped += ListView_ItemTapped;

            DataProvider.Instance.LoadNotes();
        }

        void ListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            this.Navigation.PushAsync(new NoteDetail() { BindingContext = e.Item }, true);
        }

        void AddNew_Clicked(object sender, EventArgs e) {
            this.Navigation.PushAsync(new NoteDetail() { BindingContext = DataProvider.Instance.CreateNote() }, true);
        }

        void Dropbox_Clicked(object sender, EventArgs e) {
            this.Navigation.PushAsync(new AuthLogin(), true);
        }

        void DeleteItem_Clicked(object sender, EventArgs e) {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null)
                return;
            
            NoteModel noteModel = menuItem.CommandParameter as NoteModel;
            if (noteModel == null)
                return;

            DataProvider.Instance.DeleteNote(noteModel.Id);
        }
    }
}
