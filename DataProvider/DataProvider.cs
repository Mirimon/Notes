using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SecurityNotes.Core.Model;

namespace SecurityNotes.Data {
    public class DataProvider {
        static DataProvider instance = null;
        public static DataProvider Instance {
            get {
                if(instance == null) {
                    instance = new DataProvider();
                }
                return instance;
            }
        }

        public ObservableCollection<NoteModel> Notes { get; set; }

        private Guid NewNoteId { get; set; }

        private DataProvider() {
            Notes = new ObservableCollection<NoteModel>();
        }

        public async Task LoadNotes() {
            await FileHandler.Instance.LoadNotes(Notes);
        }

        public NoteModel CreateNote() {
            NewNoteId = Guid.NewGuid();
            return new NoteModel() { Id = NewNoteId };
        }

        public async Task SaveNote(NoteModel noteModel) {
            if(noteModel.Id == NewNoteId) {
                await FileHandler.Instance.AddNote(noteModel, Notes);
                NewNoteId = Guid.Empty;
            } else {
                await FileHandler.Instance.ChangeNote(noteModel, Notes);
            }
        }

        public async Task DeleteNote(Guid id) {
            await FileHandler.Instance.DeleteNote(id, Notes);
        }

        public async Task SetAccessToken(string accessToken) {
            FileHandler.Instance.SetAccessToken(accessToken);
            await LoadNotes();
        }
    }
}
