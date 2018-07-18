using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Core.Model;

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

        public void SaveNote(NoteModel noteModel) {
            if(noteModel.Id == NewNoteId) {
                FileHandler.Instance.AddNote(noteModel);
                NewNoteId = Guid.Empty;
            } else {
                FileHandler.Instance.ChangeNote(noteModel);
            }
        }

        public void DeleteNote(Guid id) {
            FileHandler.Instance.DeleteNote(id);
        }

        public void SetAccessToken(string accessToken) {
            FileHandler.Instance.SetAccessToken(accessToken);
        }
    }
}
