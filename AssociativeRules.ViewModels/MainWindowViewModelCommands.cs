using System;
using System.Windows.Input;

namespace AssociativeRules.ViewModels
{
    public class CalculateResultCommand : ICommand
    {
        public CalculateResultCommand(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            _viewModel.CalculateResult();
        }

        public event EventHandler CanExecuteChanged;

        private MainWindowViewModel _viewModel;
    }
}