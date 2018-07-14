﻿using System;
using System.Collections.ObjectModel;
using System.Text;
using Core.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using Dropbox.Api;
using Dropbox.Api.FileRequests;
using System.Threading.Tasks;
using Dropbox.Api.Files;

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

        string notesFileName { get { return "notes.sn"; } }
        string NotesFilePath {
            get {
                string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(currentDirectory, notesFileName);
            }
        }
        string DropboxNotesPath { get { "/" + notesFileName; } }

        readonly string accessTokenFileName = "at.sn";
        string AccessTokenFilePath {
            get {
                string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(currentDirectory, accessTokenFileName);
            }
        }

        private FileHandler() { }

        ObservableCollection<NoteModel> Notes { get; set; }

        public async Task<ObservableCollection<NoteModel>> GetNotes() {
            ReadNotesFile();
            // return local notes => download notes from dropbos async => update local notes by downloaded notes

            //byte[] remoteNotes = await DownloadFromDropbox();

            //Notes = new ObservableCollection<NoteModel>();
            //for (int i = 0; i < 5; i++) {
            //    Notes.Add(new NoteModel() { Header = "header " + i, Content = "content " + i, ChangedTime = DateTime.Now, Id = Guid.NewGuid() });
            //}
            return Notes;
        }

        public async Task AddNote(NoteModel noteModel) {
            Notes.Add(noteModel);
            await SaveNotesToFile();
        }

        public async Task ChangeNote(NoteModel noteModel) {
            await SaveNotesToFile();
        }

        public async Task DeleteNote(Guid id) {
            NoteModel noteModel = Notes.FirstOrDefault(m => m.Id == id);
            if (noteModel == null)
                return;

            Notes.Remove(noteModel);
            await SaveNotesToFile();
        }

        public void SetAccessToken(string accessToken) {
            SaveToFileCore(accessToken, AccessTokenFilePath);
        }

        static readonly string passwordHash = "pass";
        static readonly string saltKey = "gjAbsk5aZs12s0jv";
        static readonly string VIKey = "*aFgthXfdbs4Bfh2sv";

        async Task SaveNotesToFile() {
            string json = JsonConvert.SerializeObject(Notes);
            byte[] fileContent = SaveToFileCore(json, NotesFilePath);

            await UploadToDropbox(fileContent);
        }

        async Task UploadToDropbox(byte[] file) {
            DropboxClient dropboxClient = GetDropBoxClient();
            if (dropboxClient == null)
                return;
            
            using (MemoryStream ms = new MemoryStream(file)) {
                try {
                    FileMetadata fileMetadata = await dropboxClient.Files.UploadAsync(DropboxNotesPath, WriteMode.Overwrite.Instance, body: ms);    
                } catch (Exception ex) {
                    string s = ex.Message;
                }
            }
        }

        async Task<byte[]> DownloadFromDropbox() {
            DropboxClient dropboxClient = GetDropBoxClient();
            if (dropboxClient == null)
                return null;

            using (var response = await dropboxClient.Files.DownloadAsync(DropboxNotesPath)) {
                return await response.GetContentAsByteArrayAsync();
            }
        }

        byte[] SaveToFileCore(string content, string path) {
            byte[] fileContent = Encrypt(content);
            File.WriteAllBytes(path, fileContent);

            return fileContent;
        }

        void ReadNotesFile() {
            Notes = new ObservableCollection<NoteModel>();
            string json = ReadFileCore(NotesFilePath);
            if (string.IsNullOrEmpty(json))
                return;

            try {
                Notes = JsonConvert.DeserializeObject<ObservableCollection<NoteModel>>(json);
            } catch { }
        }

        string ReadFileCore(string path) {
            if (File.Exists(path)) {
                try {
                    return Decrypt(File.ReadAllBytes(path));
                } catch {
                    return null;
                }
            }
            return null;
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

        DropboxClient GetDropBoxClient() {
            string authToken = ReadFileCore(AccessTokenFilePath);
            if (string.IsNullOrEmpty(authToken))
                return null;

            return new DropboxClient(authToken);
        }
    }
}
