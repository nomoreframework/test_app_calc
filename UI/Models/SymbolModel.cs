using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calculator.Models
{
    public class SymbolModel : INotifyPropertyChanged
    {
        private char symbol;
        public char Symbol
        {
            get => symbol;
            set  { symbol = value;OnPropertyChanged(nameof(Symbol));}
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
