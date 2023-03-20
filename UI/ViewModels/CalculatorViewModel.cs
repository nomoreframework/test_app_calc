using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Calculator.Commands;
using Calculator.Models;
using Calculator.Models.LogModels;
using CalcLib;
using Calculator;
using System.Windows.Controls;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        public CalculatorViewModel()
        {
            Symbols = new ObservableCollection<SymbolModel>();
            LogMessages = new ObservableCollection<LogModel>();
            myCalculator = new MyCalculator();
            Results = new Stack<Double>();
            myLog = new LogModel() { LogMessage=""};
        }
        private CalculatorCommand? addCommand;
        private CalculatorCommand? getResultCommand;
        private CalculatorCommand? removeCommand;
        private CalculatorCommand? clearAllCommand;
        private readonly MyCalculator? myCalculator;
        private readonly Stack<Double>? Results;
        public LogModel? myLog { get; set; }

        internal string Expression { get; set; } = "";
        public ObservableCollection<LogModel>? LogMessages { get; set; }
        public ObservableCollection<SymbolModel>? Symbols{ get; set; }
        public ObservableCollection<string> Logs { get; set; }  
        public event PropertyChangedEventHandler? PropertyChanged;
        public CalculatorCommand? AddCommand 
        {
            get
            {
                return addCommand ??= new CalculatorCommand(obj =>
                  {
                      var op = obj as string;
                      myLog!.LogMessage += op;
                  });
            }
        }
        public CalculatorCommand? RemoveCommand => removeCommand ??= new CalculatorCommand(s =>
        {
                var length = myLog!.LogMessage.Length;
                if (length > 0) myLog.LogMessage = myLog!.LogMessage.Remove(length - 2);
        });
        public CalculatorCommand? ResultCommand => getResultCommand ??= new CalculatorCommand(obj =>
        {
            try
            {
                var expression = myLog!.LogMessage;
                var logModel = new LogModel() { LogMessage = expression };
                var result = myCalculator!.GetResult(expression);
                Results!.Push(result);
                logModel.LogMessage += $"{"\n"}Result: {Results.Peek()}";
                myLog!.LogMessage = result.ToString();
                LogMessages!.Add(logModel);
            }
            catch (CalculatorException calcEx)
            {
                LogMessages!.Add(new LogModel() { LogMessage = calcEx.Message});
            }
            catch (ArithmeticException arithEx) {
                LogMessages!.Add(new LogModel() { LogMessage = arithEx.Message });
            }
            catch (Exception sysEx) {
                LogMessages!.Add(new LogModel() { LogMessage = sysEx.Message });
            }
        });
        public CalculatorCommand? ClearAllCommand => clearAllCommand ??= new CalculatorCommand(obj => { LogMessages!.Clear(); });
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
