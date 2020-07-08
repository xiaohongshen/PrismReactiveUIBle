﻿using System;
using Acr.UserDialogs;
using ReactiveUI;
using Autofac;


namespace MySecondApp
{
    public class GlobalExceptionHandler : IObserver<Exception>, IStartable
    {
        readonly IUserDialogs dialogs;
        public GlobalExceptionHandler(IUserDialogs dialogs) => this.dialogs = dialogs;


        public void Start() => RxApp.DefaultExceptionHandler = this;
        public void OnCompleted() {}
        public void OnError(Exception error) {}


        public void OnNext(Exception value)
        {
            Console.WriteLine(value);
            this.dialogs.Alert(value.ToString(), "ERROR");
        }
    }
}