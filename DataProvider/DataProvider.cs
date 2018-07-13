using System;
using System.Collections.ObjectModel;
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

        private Guid NewNoteId { get; set; }

        private DataProvider() { }

        public ObservableCollection<NoteModel> GetNotes() {
            return FileHandler.Instance.GetNotes();
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

        public void SetAuthCode(string code) {
            FileHandler.Instance.SetAuthCode(code);
        }
    }
}
