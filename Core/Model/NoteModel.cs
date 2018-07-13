using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core.Model {
    public class NoteModel : INotifyPropertyChanged {
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
