using System;
using System.IO;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Stone;

namespace SecurityNotes.Data.FileWorkers {
    public class DropboxFileWorker : FileWorkerBase {
        string AccessToken { get; set; }
        string NotesPath { get { return "/" + notesFileName; } }

        public DropboxFileWorker(string accessToken) {
            AccessToken = accessToken;
        }

        protected override async Task<byte[]> GetNotesFileContent() {
            DropboxClient dropboxClient = GetDropBoxClient();
            if (dropboxClient == null)
                return null;

            using (IDownloadResponse<FileMetadata> response = await dropboxClient.Files.DownloadAsync(NotesPath)) {
                return await response.GetContentAsByteArrayAsync();
            }
        }

        public override async Task SaveToFile(byte[] fileContent) {
            DropboxClient dropboxClient = GetDropBoxClient();
            if (dropboxClient == null)
                return;
            
            using (MemoryStream memoryStream = new MemoryStream(fileContent)) {
                await dropboxClient.Files.UploadAsync(NotesPath, WriteMode.Overwrite.Instance, body: memoryStream);
            }
        }

        DropboxClient GetDropBoxClient() {
            if (string.IsNullOrEmpty(AccessToken))
                return null;

            return new DropboxClient(AccessToken);
        }
    }
}
