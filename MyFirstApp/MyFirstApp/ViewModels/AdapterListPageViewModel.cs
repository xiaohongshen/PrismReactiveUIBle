using DryIoc;
using Plugin.BluetoothLE;
using Prism.AppModel;
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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace MyFirstApp.ViewModels
{
    public class AdapterListPageViewModel : ViewModelBase, IPageLifecycleAware
    {
        readonly IAdapterScanner adapterScanner;

        IAdapter adapter = CrossBleAdapter.Current;
        IDisposable scan;

        public ObservableCollection<ScanResultViewModel> Devices { get; set; }
        public ObservableCollection<IAdapter> Adapters { get; } = new ObservableCollection<IAdapter>();
        public ICommand ScanToggle { get; }
        public ICommand SelectDevice { get; }

        #region adapter flag
        [Reactive] public bool IsScanning { get; private set; }
        [Reactive] public string adapter_is_scanning { get; private set; }
        [Reactive] public string connected_devices { get; private set; }
        [Reactive] public string adapter_status { get; private set; }
        #endregion
            
        public AdapterListPageViewModel(INavigationService navigationService) :base(navigationService)
        {
            //create the icommand of item select
            this.SelectDevice = ReactiveCommand.CreateFromTask<ScanResultViewModel>(
                async x =>
                {
                    try
                    {
                        adapter.StopScan();
                        var result = await NavigationService.NavigateAsync("DevicePage", new NavigationParameters { { nameof(x.Device), x.Device } });
                        if (!result.Success)
                            Console.WriteLine(result.Exception);                        
                    }
                    catch (Exception ex)
                    {
                       
                    }
                }
            );

            // carry over from BLE A2
            Devices = new ObservableCollection<ScanResultViewModel>();
            //this.connected_devices = this.adapter.IsScanning.ToString();

            //this.FindAdapter = ReactiveCommand.Create(
            //    () =>
            //    {
            //        adapterScanner
            //            .FindAdapters()
            //            .ObserveOn(RxApp.MainThreadScheduler)
            //            .Subscribe(
            //                async () =>
            //                {

            //                }
            //            )
            //    }
            //    );
            //this.connected_devices = 10;
            //this.adapter_is_scanning = this.adapter.IsScanning.ToString();
            this.adapter_is_scanning = this.adapter.IsScanning.ToString();
            this.adapter_status = this.adapter.Status.ToString();
            this.ScanToggle = ReactiveCommand.Create(
                () =>
                {
                    if (!IsScanning) //when (Press to scan)
                    {
                        //this.connected_devices = this.adapter.Status.ToString();
                        //this.adapter.SetAdapterState(true);
                        //this.adapter_status = this.adapter.Status.ToString();
                        this.IsScanning = true;
                        this.scan = this.adapter.Scan().Buffer(TimeSpan.FromSeconds(1)).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                            results =>
                            {
                                this.connected_devices = results.Count().ToString();
                                this.adapter_is_scanning = this.adapter.IsScanning.ToString();
                                this.adapter_status = this.adapter.Status.ToString();

                                foreach (var r in results)
                                {
                                    //  var mydev=r.Device;

                                    var dev = this.Devices.FirstOrDefault(x => x.Uuid.Equals(r.Device.Uuid));

                                    if (dev != null)
                                    {
                                        dev.TrySet(r);
                                    }
                                    else
                                    {
                                        dev = new ScanResultViewModel();
                                        dev.TrySet(r);
                                    }
                                    Devices.Add(dev);
                                }
                                //if (result.Device.Name == "Nordic_Blinky")
                                //{
                                //    this.connected_devices += 1;
                                //}
                            }
                        ).DisposeWith(this.DeactivateWith);
                        //this.scan = this.adapter
                        //    .Scan()
                        //    .Buffer(TimeSpan.FromSeconds(1))
                        //    .ObserveOn(RxApp.MainThreadScheduler)
                        //    .Subscribe(
                        //        scanResult =>
                        //        {
                        //            //if (scanResult[0].Device.Name == "Nordic_Blinky"){
                        //            //    this.connected_devices += 1;
                        //            //}
                        //            ////this.connected_devices = 3;
                        //            ////this.connected_devices += (1+scanResult.Count());
                        //            //foreach (var result in scanResult)
                        //            //{
                        //            //    this.connected_devices = 9;
                        //            //    if (result.Device.Name == "Nordic_Blinky")
                        //            //    {

                        //            //        //this.connected_devices = 1;
                        //            //    }
                        //            //}
                        //        }
                        //    ).DisposeWith(this.DeactivateWith);
                        //.DisposeWith(this.scan); ;
                        //this.connected_devices = Devices.Count();

                    }
                    else
                    {
                        this.scan?.Dispose();
                        //this.Devices.Clear();
                        this.IsScanning = false;
                        this.adapter_is_scanning = this.adapter.IsScanning.ToString();
                        this.adapter_status = this.adapter.Status.ToString();

                    }
                }
             );
        }



        //to using onappearing and ondisappering in view model interface IPageLifecycleAware has to be added. 
        public void OnAppearing()
        {
            this.IsScanning = false;
        }

        public void OnDisappearing()
        {
            
        }
    }
}
