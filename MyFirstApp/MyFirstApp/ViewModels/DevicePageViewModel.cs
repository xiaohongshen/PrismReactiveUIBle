using Plugin.BluetoothLE;
using Prism.Commands;
using Prism.Common;
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
    public class DevicePageViewModel : ViewModelBase
    {
        IDevice device;

        [Reactive] public string Name { get; private set; }
        [Reactive] public Guid Uuid { get; private set; }
        [Reactive] public string PairingText { get; private set; } = "unknow";
        [Reactive] public string ConnectText { get; private set; } = "unknow";

        public ICommand ConnectionToggle { get; }
        public ICommand SelectCharacteristic { get; }
        public ICommand PairToDevice { get; }
        public ICommand RequestMtu { get; }
        public ObservableCollection<Group<GattCharacteristicViewModel>> GattCharacteristics { get; } = new ObservableCollection<Group<GattCharacteristicViewModel>>();


        public DevicePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //this.SelectCharacteristic = ReactiveCommand.Create<GattCharacteristicViewModel>(x => x.Select());

            this.ConnectionToggle = ReactiveCommand.Create(() =>
            {
                // don't cleanup connection - force user to d/c
                if (this.device.Status == ConnectionStatus.Disconnected)
                {
                    this.device.Connect(new ConnectionConfig { AutoConnect = true, AndroidConnectionPriority = ConnectionPriority.Normal });                   
                }
                else
                {
                    this.device.CancelConnection();                   
                }               

            });

            

            this.PairToDevice = ReactiveCommand.Create(() =>
            {
                if (this.device.PairingStatus == PairingStatus.Paired)
                {
                    this.PairingText = "Paired";
                    this.RaisePropertyChanged(nameof(this.PairingText));
                }
                else
                {
                    this.device
                        .PairingRequest()
                        .Subscribe(x =>
                        {
                            var txt = x ? "Device Paired Successfully" : "Device Pairing Failed";
                            this.PairingText = txt;
                            this.RaisePropertyChanged(nameof(this.PairingText));
                        });
                }
            });

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                this.device = parameters.GetValue<IDevice>("Device");
                this.Name = this.device.Name;
                this.Uuid = this.device.Uuid;
                this.PairingText = this.device.PairingStatus == PairingStatus.Paired ? "Device Paired" : "Pair Device";
                this.Title = "coming";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"something is wrong {ex.Message}");
            }

            this.device
                .WhenStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(status =>
                {
                    switch (status)
                    {
                        case ConnectionStatus.Connecting:
                            this.ConnectText = "Cancel Connection";
                            break;

                        case ConnectionStatus.Connected:
                            this.ConnectText = "Disconnect";
                            break;

                        case ConnectionStatus.Disconnected:
                            this.ConnectText = "Connect";
                            try
                            {
                                this.GattCharacteristics.Clear();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                            break;
                    }
                })
                .DisposeWith(this.DeactivateWith);

            this.device
                .WhenAnyCharacteristicDiscovered()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(chs =>
                {
                    try
                    {
                        var service = this.GattCharacteristics.FirstOrDefault(x => x.ShortName.Equals(chs.Service.Uuid.ToString()));
                        if (service == null)
                        {
                            service = new Group<GattCharacteristicViewModel>(
                                $"{chs.Service.Description} ({chs.Service.Uuid})",
                                chs.Service.Uuid.ToString()
                            );
                            this.GattCharacteristics.Add(service);
                        }

                        service.Add(new GattCharacteristicViewModel(chs));
                    }
                    catch (Exception ex)
                    {
                        // eat it
                        Console.WriteLine($"wrong check charac {ex}");
                    }
                })
                .DisposeWith(this.DeactivateWith);
        }
    }
}
