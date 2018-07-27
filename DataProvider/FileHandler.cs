using System;
using System.Collections.ObjectModel;
using System.Text;
using SecurityNotes.Core.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using Dropbox.Api;
using Dropbox.Api.FileRequests;
using System.Threading.Tasks;
using Dropbox.Api.Files;
using System.Collections.Generic;
using SecurityNotes.Data.FileWorkers;
using SecurityNotes.Core.Utils;

namespace SecurityNotes.Data {
    internal class FileHandler {
        static FileHandler instance = null;
        public static FileHandler Instance {
            get {
                if(instance == null) {
                    instance = new FileHandler();
                }
                return instance;
            }
        }

        List<IFileWorker> FileWorkers { get; set; }

        readonly string accessTokenFileName = "skdfb.sn";
        string AccessTokenFilePath {
            get {
                string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(currentDirectory, accessTokenFileName);
            }
        }

        private FileHandler() {
            FileWorkers = new List<IFileWorker>();
            FileWorkers.Add(new LocalFileWorker());

            if (File.Exists(AccessTokenFilePath)) {
                byte[] accessTokenBytes = File.ReadAllBytes(AccessTokenFilePath);
                if ((accessTokenBytes == null) || (accessTokenBytes.Length == 0))
                    return;

                try {
                    string accessToken = EncryptionHelper.Decrypt(accessTokenBytes);
                    if (string.IsNullOrEmpty(accessToken))
                        return;

                    FileWorkers.Add(new DropboxFileWorker(accessToken));
                } catch { }
            }
        }

        public async Task LoadNotes(ObservableCollection<NoteModel> notes) {
            notes.Clear();
            foreach(IFileWorker fileWorker in FileWorkers) {
                await fileWorker.ReadNotes(notes);
                SortHelper.Sort<NoteModel>(notes, (n1, n2) => n1.ChangedTime > n2.ChangedTime ? true : false);
            }
        }

        public async Task AddNote(NoteModel noteModel, ObservableCollection<NoteModel> notes) {
            notes.Add(noteModel);
            SortHelper.Sort<NoteModel>(notes, (n1, n2) => n1.ChangedTime > n2.ChangedTime ? true : false);
            await SaveNotes(notes);
        }

        public async Task ChangeNote(NoteModel noteModel, ObservableCollection<NoteModel> notes) {
            // noteModel may be useful in the future if I decide to do event sourcing
            SortHelper.Sort<NoteModel>(notes, (n1, n2) => n1.ChangedTime > n2.ChangedTime ? true : false);
            await SaveNotes(notes);
        }

        public async Task DeleteNote(Guid id, ObservableCollection<NoteModel> notes) {
            NoteModel noteModel = notes.FirstOrDefault(m => m.Id == id);
            if (noteModel == null)
                return;

            notes.Remove(noteModel);
            await SaveNotes(notes);
        }

        public void SetAccessToken(string accessToken) {
            byte[] fileContent = EncryptionHelper.Encrypt(accessToken);
            File.WriteAllBytes(AccessTokenFilePath, fileContent);

            IFileWorker dropboxFileWorker = FileWorkers.FirstOrDefault(fw => fw is DropboxFileWorker);
            if(dropboxFileWorker != null) {
                FileWorkers.Remove(dropboxFileWorker);
            }

            FileWorkers.Add(new DropboxFileWorker(accessToken));
        }

        public async Task SaveNotes(ObservableCollection<NoteModel> notes) {
            foreach (IFileWorker fileWorker in FileWorkers) {
                await fileWorker.SaveNotes(notes);
            }
        }
    }
}
