using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calculator.Models.LogModels
{
    public class LogModel : INotifyPropertyChanged
    {
        private string? logMessage;
        private string? date;
        public DateTime DateCreated { get;}
        public LogModel()
        {
            DateCreated = DateTime.Now;
            date = DateTime.Now.ToString();
        }
        public required string LogMessage
        {
            get => logMessage ?? "empty";
            set {logMessage = value; OnPropertyChanged("LogMessage"); }
        }
        public string Date
        {
            get => date ?? "Date is empty";
            set { date = value; OnPropertyChanged(nameof(Date)); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
