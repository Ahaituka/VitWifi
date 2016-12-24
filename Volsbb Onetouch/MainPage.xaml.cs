using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Volsbb_Onetouch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
      

        
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async  void Button_Click(object sender, RoutedEventArgs e)
        {
          

            var access= await BackgroundExecutionManager.RequestAccessAsync();
           //   var wlanConnectionProfileDetails = connectionProfile.wlanConnectionProfileDetails;

           
        //    var SSID = WlanConnectionProfileDetailss.GetConnectedSsid();

            BackgroundTaskHelper.Register("SampleBackgroundTask", "Tasks.SampleBackgroundTask", new SystemTrigger(SystemTriggerType.NetworkStateChange, false), false, true, null);
            BackgroundTaskHelper.Register("ToastBackgroundTask", "Tasks.ToastBackgroundTask", new ToastNotificationActionTrigger(), false,false, null);



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BackgroundTaskHelper.Unregister("SampleBackgroundTask");
            BackgroundTaskHelper.Unregister("ToastBackgroundTask");
        }
    }
}
