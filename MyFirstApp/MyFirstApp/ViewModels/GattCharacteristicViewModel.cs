using Plugin.BluetoothLE;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFirstApp.ViewModels
{
    public class GattCharacteristicViewModel : BindableBase
    {

        public IGattCharacteristic Characteristic { get; }
        public GattCharacteristicViewModel()
        {

        }

        public GattCharacteristicViewModel(IGattCharacteristic characteristic)
        {
            this.Characteristic = characteristic;
        }

        public void Select()
        {

        }


    }
}
