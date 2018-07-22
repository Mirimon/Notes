using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SecurityNotes.Core.Model;

namespace SecurityNotes.Data.FileWorkers {
    public interface IFileWorker {
        Task ReadNotes(ObservableCollection<NoteModel> notes);
        Task SaveNotes(ObservableCollection<NoteModel> notes);
    }
}
