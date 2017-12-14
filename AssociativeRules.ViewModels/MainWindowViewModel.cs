using AssociativeRules.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;

namespace AssociativeRules.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            InitCommands();
            Transactions = new ObservableCollection<Transaction>();
        }

        public void CalculateResult()
        {
            AprioriCalculation apriori = new AprioriCalculation();

            apriori.MinSupport = MinSupport;
            apriori.MinConfidence = MinConfidence;

            foreach (var t in Transactions)
            {
                if (t.Elements == "")
                {
                    continue;
                }

                List<string> elements = new List<string>();
                string[] elementStrings = t.Elements.Split(',');

                foreach (var e in elementStrings)
                {
                    elements.Add(e);
                }

                apriori.AddTransaction(t.Name, elements);
            }

            Result = "Згенеровані множини та їх значення support, %:\n";
            var resultSets = apriori.GenerateSets();
            foreach (var item in resultSets)
            {
                foreach (var subItem in item.Key)
                {
                    Result += subItem + " ";
                }
                Result += "- " + item.Value + Environment.NewLine;
            }

            Result += "\nЗгенеровані правила та їх значення сonfidence, %:\n";
            var resultRules = apriori.GenerateRules();
            foreach (var rule in resultRules)
            {
                foreach (var ifItem in rule.IfSet)
                {
                    Result += ifItem + " ";
                }

                Result += "\x2192 ";

                foreach (var thenItem in rule.ThenSet)
                {
                    Result += thenItem + " ";
                }

                Result += "- " + rule.Confidence + Environment.NewLine;
            }
        }

        public class Transaction
        {
            public string Name { get; set; }
            public string Elements { get; set; }
        }

        public ObservableCollection<Transaction> Transactions
        {
            get
            {
                return _transactions;
            }
            set
            {
                _transactions = value;
                OnPropertyChanged("Transactions");
            }
        }
        public double MinSupport
        {
            get { return _minSupport; }
            set
            {
                if (value >=0 && value <= 100)
                {
                    _minSupport = value;
                    OnPropertyChanged("MinSupport");
                }
            }
        }
        public double MinConfidence
        {
            get { return _minConfidence; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    _minConfidence = value;
                    OnPropertyChanged("MinConfidence");
                }
            }
        }
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands
        public CalculateResultCommand CalculateResultCommand { get; private set; }
        #endregion

        private void InitCommands()
        {
            CalculateResultCommand = new CalculateResultCommand(this);
        }

        private ObservableCollection<Transaction> _transactions;
        private double _minSupport;
        private double _minConfidence;
        private string _result;
    }
}