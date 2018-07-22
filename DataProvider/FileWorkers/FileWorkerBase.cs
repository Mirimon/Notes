using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SecurityNotes.Core.Model;
using SecurityNotes.Core.Utils;
using Newtonsoft.Json;

namespace SecurityNotes.Data.FileWorkers {
    public class FileWorkerBase : IFileWorker {
        

        protected readonly string notesFileName = "sgwfef.sn";

        string deletedNotesFileName { get { return "sgsev.sn"; } }
        string DeletedNotesFilePath {
            get {
                string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(currentDirectory, deletedNotesFileName);
            }
        }

        public async Task ReadNotes(ObservableCollection<NoteModel> notes) {
            byte[] fileBytes = await GetNotesFileContent();
            if ((fileBytes == null) || (fileBytes.Length == 0))
                return;

            ObservableCollection<NoteModel> notesFromFile = DecryptNotesFile(fileBytes);
            UnionNotes(notes, notesFromFile);
            if (NeedUpdateSource(notes, notesFromFile))
                await SaveNotes(notes);
        }

        public async Task SaveNotes(ObservableCollection<NoteModel> notes) {
            string json = JsonConvert.SerializeObject(notes);
            byte[] fileContent = EncryptionHelper.Encrypt(json);
            await SaveToFile(fileContent);
        }

        public virtual async Task SaveToFile(byte[] fileContent) {
        }

        ObservableCollection<NoteModel> DecryptNotesFile(byte[] bytes) {
            string json = EncryptionHelper.Decrypt(bytes);
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonConvert.DeserializeObject<ObservableCollection<NoteModel>>(json);
        }

        protected virtual async Task<byte[]> GetNotesFileContent() {
            return await Task.FromResult(new byte[0]);
        }

        void UnionNotes(ObservableCollection<NoteModel> currentNotes, ObservableCollection<NoteModel> sourceNotes) {
            if (sourceNotes == null)
                return;
            
            List<Guid> deletedNotes = GetDeletedNotes();
            foreach (NoteModel note in sourceNotes) {
                NoteModel sameNote = currentNotes.FirstOrDefault(n => n.Id == note.Id);
                if (sameNote == null) {
                    if (!deletedNotes.Contains(note.Id))
                        currentNotes.Add(note);
                } else {
                    if (sameNote.ChangedTime < note.ChangedTime) {
                        NoteModel.Clone(note, sameNote);
                    }
                }
            }
        }

        protected virtual bool NeedUpdateSource(ObservableCollection<NoteModel> currentNotes, ObservableCollection<NoteModel> sourceNotes) {
            if (currentNotes.Count != sourceNotes.Count)
                return true;

            List<Guid> deletedNotes = GetDeletedNotes();
            foreach (NoteModel note in sourceNotes) {
                NoteModel sameNote = currentNotes.FirstOrDefault(n => n.Id == note.Id);
                if (sameNote == null) {
                    if (deletedNotes.Contains(note.Id))
                        return true;
                }

                if (note.ChangedTime > sameNote.ChangedTime)
                    continue;

                if (!NoteModel.AreEqual(sameNote, note)) {
                    return true;
                }
            }

            return false;
        }

        List<Guid> GetDeletedNotes() {
            string json = EncryptionHelper.Decrypt(GetLocalFileContent(DeletedNotesFilePath));
            if (string.IsNullOrEmpty(json))
                return new List<Guid>();

            try {
                return JsonConvert.DeserializeObject<List<Guid>>(json);
            } catch {
                return new List<Guid>();
            }
        }

        protected byte[] GetLocalFileContent(string filePath) {
            if (File.Exists(filePath))
                return File.ReadAllBytes(filePath);
            else
                return null;
        }
    }
}