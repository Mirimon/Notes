using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using SecurityNotes.Core.Model;

namespace SecurityNotes.Data.FileWorkers {
    public class LocalFileWorker : FileWorkerBase {
        string NotesFilePath {
            get {
                string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(currentDirectory, notesFileName);
            }
        }

        protected override async Task<byte[]> GetNotesFileContent() {
            return await Task.FromResult(GetLocalFileContent(NotesFilePath));
        }

        protected override bool NeedUpdateSource(ObservableCollection<NoteModel> currentNotes, ObservableCollection<NoteModel> sourceNotes) {
            return false;
        }

        public override async Task SaveToFile(byte[] fileContent) {
            await Task.Run(() => { File.WriteAllBytes(NotesFilePath, fileContent); });
        }
    }
}
