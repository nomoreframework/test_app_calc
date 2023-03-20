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
using Calculator.DB.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        public CalculatorViewModel()
        {
            LogMessages = new ObservableCollection<LogModel>();
            myCalculator = new MyCalculator();
            Results = new Stack<Double>();
            MyPanel = new LogModel() { LogMessage=""};
            myDBContext = new CalculatorDBContext();
            logDBContext = new LogDBContext();
            myDBContext.Database.EnsureCreated();
            myDBContext.Operations.Load();
            if (myDBContext.Operations.Any())
                foreach (var o in myDBContext.Operations.ToList().TakeLast(10)) LogMessages.Add(o); 
        }
        private CalculatorCommand? addCommand;
        private CalculatorCommand? getResultCommand;
        private CalculatorCommand? removeCommand;
        private CalculatorCommand? clearAllCommand;
        private readonly MyCalculator? myCalculator;
        private readonly Stack<Double>? Results;
        public LogModel? MyPanel { get; set; }
        private CalculatorCommand? clearPanelCommand;
        private CalculatorDBContext? myDBContext;
        private LogDBContext? logDBContext;

        public ObservableCollection<LogModel>? LogMessages { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public CalculatorCommand? AddCommand 
        {
            get
            {
                return addCommand ??= new CalculatorCommand(obj =>
                  {
                      var op = obj as string;
                      MyPanel!.LogMessage += op;
                  });
            }
        }
        public CalculatorCommand? RemoveCommand => removeCommand ??= new CalculatorCommand(s =>
        {
                var length = MyPanel!.LogMessage.Length;
                if (length > 0) MyPanel.LogMessage = MyPanel!.LogMessage.Remove(length - 1);
        });
        public CalculatorCommand? ClearPanelCommand => clearPanelCommand ??= new CalculatorCommand(s =>
        {
            MyPanel!.LogMessage = String.Empty;
        });
        public CalculatorCommand? ResultCommand => getResultCommand ??= new CalculatorCommand(obj =>
        {
            try
            {
                var expression = MyPanel!.LogMessage;
                var logModel = new LogModel() { LogMessage = expression };
                var result = myCalculator!.GetResult(expression);
                Results!.Push(result);
                logModel.LogMessage += $"{"\n"}Result: {Results.Peek()}";
                MyPanel!.LogMessage = result.ToString();
                LogMessages!.Add(logModel);
                Task t = new(() =>
                {
                    myDBContext!.Operations.Add(logModel);
                    myDBContext.SaveChanges();
                });
                t.Start();
            }
            catch (CalculatorException calcEx)
            {
                var logModel = new LogModel() { LogMessage = calcEx.Message };
                LogMessages!.Add(logModel);
                MyPanel!.LogMessage = calcEx.Message;
                Task t = new(() =>
                {
                    logDBContext!.OpErrors!.Add(logModel);
                    logDBContext.SaveChanges();
                });
                t.Start();
                MyPanel!.LogMessage = calcEx.Message;
            }
            catch (Exception sysEx) {
                var logModel = new LogModel() { LogMessage = sysEx.Message };
                LogMessages!.Add(logModel);
                MyPanel!.LogMessage = "0";
                Task t = new(() =>
                {
                    logDBContext!.SysErrors.Add(logModel);
                    myDBContext!.SaveChanges();
                });
                t.Start();
            }
        });
        public CalculatorCommand? ClearAllCommand => clearAllCommand ??= new CalculatorCommand(obj => {

            Task t = new(() =>
            {
                myDBContext!.Operations.RemoveRange(LogMessages!);
                myDBContext!.SaveChanges();
            });
            t.Start();
            t.Wait();
            LogMessages!.Clear();
        });
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
