using Plugin.BluetoothLE;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace MyFirstApp.ViewModels
{
    public class AdapterListPageViewModel : ViewModelBase
    {
        readonly IAdapterScanner adapterScanner;
        IAdapter adapter = CrossBleAdapter.Current;
        public ObservableCollection<ScanResultViewModel> Devices { get; set; }
        public ICommand Scan { get; }
        public ObservableCollection<IAdapter> Adapters { get; } = new ObservableCollection<IAdapter>();

        [Reactive] public bool IsBusy { get; protected set; }
        public AdapterListPageViewModel(INavigationService navigationService) :base(navigationService)
        {
            this.adapterScanner = adapterScanner;

            this.Scan = ReactiveCommand.Create(() =>
            {
                this.IsBusy = true;
                adapterScanner
                    .FindAdapters()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        this.Adapters.Add,
                        //ex => dialogs.Alert(ex.ToString(), "Error"),
                        async () =>
                        {
                            this.IsBusy = false;
                            switch (this.Adapters.Count)
                            {
                                case 0:
                                    
                                    break;

                                case 1:
                                    var adapter = this.Adapters.First();
                                    Debug.WriteLine($"find adapter {adapter.DeviceName}");
                                   // await navigationService.NavToAdapter(adapter);
                                    break;
                            }
                        }
                    );
            },
            this.WhenAny(x => x.IsBusy, x => !x.Value));
        }
    }
}
