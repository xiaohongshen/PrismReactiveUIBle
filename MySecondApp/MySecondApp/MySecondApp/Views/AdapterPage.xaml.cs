using System;
using Prism.Common;
using Prism.Navigation;
using Xamarin.Forms;


namespace MySecondApp.Views
{
    public partial class AdapterPage : TabbedPage
    {
        public AdapterPage ()
        {
            this.InitializeComponent();
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            foreach (var child in this.Children)
            {
                PageUtilities.OnNavigatedTo(child, parameters);
            }
        }
    }
}