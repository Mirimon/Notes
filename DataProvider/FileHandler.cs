using System;
using System.Collections.ObjectModel;
using System.Text;
using Core.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.IO;

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

        readonly string notesFileName = "notes.sn";
        string NotesFilePath {
            get {
                string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(currentDirectory, notesFileName);
            }
        }

        readonly string codeFileName = "ac.sn";
        string CodeFilePath {
            get {
                string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(currentDirectory, codeFileName);
            }
        }

        private FileHandler() { }

        ObservableCollection<NoteModel> Notes { get; set; }

        public ObservableCollection<NoteModel> GetNotes() {
            ReadFile();
            //Notes = new ObservableCollection<NoteModel>();
            //for (int i = 0; i < 5; i++) {
            //    Notes.Add(new NoteModel() { Header = "header " + i, Content = "content " + i, ChangedTime = DateTime.Now, Id = Guid.NewGuid() });
            //}
            return Notes;
        }

        public void AddNote(NoteModel noteModel) {
            Notes.Add(noteModel);
            SaveNotesToFile();
        }

        public void ChangeNote(NoteModel noteModel) {
            SaveNotesToFile();
        }

        public void DeleteNote(Guid id) {
            NoteModel noteModel = Notes.FirstOrDefault(m => m.Id == id);
            if (noteModel == null)
                return;

            Notes.Remove(noteModel);
            SaveNotesToFile();
        }

        public void SetAuthCode(string code) {
            SaveToFileCore(code, CodeFilePath);
        }

        static readonly string passwordHash = "pass";
        static readonly string saltKey = "gjAbsk5aZs12s0jv";
        static readonly string VIKey = "*aFgthXfdbs4Bfh2sv";

        void SaveNotesToFile() {
            string json = JsonConvert.SerializeObject(Notes);
            SaveToFileCore(json, NotesFilePath);
        }

        void SaveToFileCore(string content, string path) {
            byte[] fileContent = Encrypt(content);
            File.WriteAllBytes(path, fileContent);
        }

        void ReadFile() {
            Notes = new ObservableCollection<NoteModel>();
            if(File.Exists(NotesFilePath)) {
                try {
                    string result = Decrypt(File.ReadAllBytes(NotesFilePath));
                    Notes = JsonConvert.DeserializeObject<ObservableCollection<NoteModel>>(result);
                } catch { }
            }
        }

        byte[] Encrypt(string text) {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);

            byte[] keyBytes = new Rfc2898DeriveBytes(passwordHash, Encoding.ASCII.GetBytes(saltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return cipherTextBytes;
        }

        string Decrypt(byte[] fileBytes) {
            byte[] keyBytes = new Rfc2898DeriveBytes(passwordHash, Encoding.ASCII.GetBytes(saltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(fileBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[fileBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}
