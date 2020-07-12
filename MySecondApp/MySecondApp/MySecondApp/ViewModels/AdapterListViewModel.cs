using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.BluetoothLE;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveUI;


namespace MySecondApp.ViewModels
{
    public class AdapterListViewModel : ViewModel
    {
        readonly IAdapterScanner adapterScanner;
        readonly INavigationService navigationService;

        public AdapterListViewModel(INavigationService navigationService,
                                    IAdapterScanner adapterScanner,
                                    IUserDialogs dialogs)
        {
            this.adapterScanner = adapterScanner;
            this.navigationService = navigationService;
            this.Select = ReactiveCommand.CreateFromTask<IAdapter>(navigationService.NavToAdapter);

            this.Scan = ReactiveCommand.Create(() =>
            {                
                this.IsBusy = true;
                var dsi= adapterScanner
                    .FindAdapters()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        this.Adapters.Add,
                        ex => dialogs.Alert(ex.ToString(), "Error"),
                        async () =>
                        {
                            this.IsBusy = false;
                            switch (this.Adapters.Count)
                            {
                                case 0:
                                    dialogs.Alert("No BluetoothLE Adapters Found");
                                    break;

                                case 1:
                                    var adapter = this.Adapters.First();
                                    await navigationService.NavToAdapter(adapter);
                                    break;
                            }
                        }
                    );
            },
            this.WhenAny(x => x.IsBusy, x => !x.Value));
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("OnAppearing is called");
            if (this.adapterScanner.IsSupported)
            {
                this.Scan.Execute(null);
            }
            else
            {
                this.navigationService.NavToAdapter(CrossBleAdapter.Current);
                Debug.WriteLine("OnAppearing is called jump to device");
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Debug.WriteLine("disapper adaplist is done");
        }

        public override void Initialize(INavigationParameters parameters)
        {
            
        }

        public ObservableCollection<IAdapter> Adapters { get; } = new ObservableCollection<IAdapter>();
        public ICommand Select { get; }
        public ICommand Scan { get; }
    }
}
