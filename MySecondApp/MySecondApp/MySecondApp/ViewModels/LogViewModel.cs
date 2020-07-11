using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.Collections;
using Acr.UserDialogs;
using Prism.Navigation;
using ReactiveUI;


namespace MySecondApp.ViewModels
{
    public class LogViewModel : ViewModel
    {
        readonly ILogService logs;


        public LogViewModel(ILogService logs)
        {
           
        }


        public override void OnAppearing()
        {
            
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Debug.WriteLine("Initialize is called in logview");
        }

        public ObservableList<LogItem> Logs { get; } = new ObservableList<LogItem>();
        public ICommand Show { get; }
        public ICommand Clear { get; }
    }
}
