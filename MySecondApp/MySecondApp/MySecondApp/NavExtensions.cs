using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.BluetoothLE;
using Prism.Navigation;


namespace MySecondApp
{
    public static class NavExtensions
    {
        public static Task NavToDevice(this INavigationService navigator, IDevice device)
            => navigator.Navigate("DevicePage", new NavigationParameters
            {                
                { nameof(device), device }
            });


        public static Task NavToAdapter(this INavigationService navigator, IAdapter adapter)
            => navigator.Navigate("ScanPage", new NavigationParameters
            {
                { nameof(adapter), adapter }
            });


        public static async Task Navigate(this INavigationService navigator, string page, INavigationParameters parameters)
        {
            var result = await navigator.NavigateAsync(page, parameters);
            if (!result.Success)
            {
                Debug.WriteLine($"something is wrong to {page}");
                Debug.WriteLine(result.Exception);
            }
                
        }
    }
}
