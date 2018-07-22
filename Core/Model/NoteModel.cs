using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SecurityNotes.Core.Model {
    public class NoteModel : INotifyPropertyChanged {
        public static void Clone(NoteModel source, NoteModel target) {
            target.ChangedTime = source.ChangedTime;
            target.Content = source.Content;
            target.Header = source.Header;
            target.Id = source.Id;
        }

        public static bool AreEqual(NoteModel a, NoteModel b) {
            return (a.Id == b.Id) && (a.Content == b.Content) && (a.Header == b.Header) && (a.ChangedTime == b.ChangedTime);
        }

        public Guid Id { get; set; }

        string header = "";
        public string Header { 
            get { return header; }
            set {
                if(header != value) {
                    header = value;
                    RaisePropertyChanged();
                }
            }
        }

        string content = "";
        public string Content {
            get { return content; }
            set {
                if (content != value) {
                    content = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime changedTime = DateTime.MinValue;
        public DateTime ChangedTime {
            get { return changedTime; }
            set {
                if (changedTime != value) {
                    changedTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
