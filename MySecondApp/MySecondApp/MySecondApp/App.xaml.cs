using Prism;
using Prism.Ioc;
using MySecondApp.ViewModels;
using MySecondApp.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Mvvm;
using System;
using Plugin.BluetoothLE;
using Autofac;
using Acr.UserDialogs;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MySecondApp
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
           
            var result = await NavigationService.NavigateAsync("NavigationPage/AdapterListPage");
            if (!result.Success)
                Console.WriteLine(result.Exception);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IUserDialogs>(UserDialogs.Instance);
            containerRegistry.RegisterInstance<IAdapterScanner>(CrossBleAdapter.AdapterScanner);
            containerRegistry.RegisterSingleton<ILogService, LogService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            

            containerRegistry.RegisterForNavigation<AdapterListPage, AdapterListViewModel>();
            containerRegistry.RegisterForNavigation<AdapterPage>();

            containerRegistry.RegisterForNavigation<ScanPage,ScanViewModel>();
            containerRegistry.RegisterForNavigation<LogPage,LogViewModel>();
            containerRegistry.RegisterForNavigation<ServerPage,ServerViewModel>();

            containerRegistry.RegisterForNavigation<DevicePage,DeviceViewModel>();
        }

        //protected override IContainerExtension CreateContainerExtension()
        //{
        //    var builder = new ContainerBuilder();
        //    //builder.Register(_ => UserDialogs.Instance).As<IUserDialogs>().SingleInstance();
        //    builder.RegisterType<GlobalExceptionHandler>().As<IStartable>().AutoActivate().SingleInstance();
        //    return new AutofacContainerExtension(builder);
        //}
    }
}

/*
 *  public class App : PrismApplication
    {
        public App() : base(null) { }


        


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IUserDialogs>(UserDialogs.Instance);
            containerRegistry.RegisterInstance<IAdapterScanner>(CrossBleAdapter.AdapterScanner);
            containerRegistry.RegisterSingleton<ILogService, LogService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<AdapterPage>();

            containerRegistry.RegisterForNavigation<AdapterListPage>();

            containerRegistry.RegisterForNavigation<ScanPage>();
            containerRegistry.RegisterForNavigation<LogPage>();
            containerRegistry.RegisterForNavigation<ServerPage>();

            containerRegistry.RegisterForNavigation<DevicePage>();
        }


        protected override IContainerExtension CreateContainerExtension()
        {
            var builder = new ContainerBuilder();
            //builder.Register(_ => UserDialogs.Instance).As<IUserDialogs>().SingleInstance();
            builder.RegisterType<GlobalExceptionHandler>().As<IStartable>().AutoActivate().SingleInstance();
            return new AutofacContainerExtension(builder);
        }
    }
 */
