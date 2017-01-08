using System;
using VOLSBB.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Background;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.UI.Popups;
using Network;
using Windows.Networking.Connectivity;

namespace VOLSBB.Views
{
   
    public sealed partial class MainPage : Page
    {
     

        private const string TASK_NAME = "TILE_UPDATE_TIMER_TASK_SAMPLE";
        public static bool Registered;


        public async static void ShowDialog(string message)
        {
            var dlg = new MessageDialog(message);
            await dlg.ShowAsync();
        }

        public void RegisterTask()
        {
            var timeTrigger = new TimeTrigger(15, false);

            var backgroundTaskBuilder = new BackgroundTaskBuilder();
            backgroundTaskBuilder.Name = TASK_NAME;
            backgroundTaskBuilder.TaskEntryPoint = typeof(BackgroundTileTimerTask.BackgroundTask).FullName;
            backgroundTaskBuilder.SetTrigger(timeTrigger);

            backgroundTaskBuilder.Register();
        }
        public MainPage()
        {


            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            //      CredentialsVerifier();

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "SampleBackgroundTask")
                {
                    Registered = true;
                    register.IsEnabled = true;
                }
            }
            if (Registered)
            {
                register.Content = "Unregister";

            }
        }

        //private async Task<bool> CredentialsVerifier()
        //{
        //    if (User.Text.Length != 0 && Pass.Password.Length != 0)
        //    {
        //        register.IsEnabled = true;
        //        LoginButton.IsEnabled = true;
        //        LogoutButton.IsEnabled = true;
        //        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        //        localSettings.Values["user"] = User.Text;
        //        localSettings.Values["pass"] = Pass.Password;
        //        await Network.Pronto.Login();
        //        return true;
        //    }
        //    else
        //        register.IsEnabled = false;
        //    LoginButton.IsEnabled = false;
        //    LogoutButton.IsEnabled = false;

        //    return false;

        //}
        private async void Register(object sender, RoutedEventArgs e)
        {
            if(!isValid)
            {
                ShowDialog("Please Fill Your Credentials Correctly");
                return;
            }

            if (Registered)
            {
                BackgroundTaskHelper.Unregister("SampleBackgroundTask");
                BackgroundTaskHelper.Unregister("ToastBackgroundTask");
                BackgroundTaskHelper.Unregister(TASK_NAME);
                register.Content = "Regsiter";
                Registered = false;
            }
            else
            {
                var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backgroundAccessStatus == BackgroundAccessStatus.Denied) { return; }
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["user"] = User.Text;
                localSettings.Values["pass"] = Pass.Password;
                BackgroundTaskHelper.Register("SampleBackgroundTask", "Tasks.SampleBackgroundTask", new SystemTrigger(SystemTriggerType.NetworkStateChange, false), false, true ,new SystemCondition(SystemConditionType.InternetNotAvailable));

                BackgroundTaskHelper.Register("ToastBackgroundTask", "Tasks.ToastBackgroundTask", new ToastNotificationActionTrigger(), false, false, null);
                RegisterTask();
                register.Content = "UnRegister";
                Registered = true;
                
            }
        }


        private async void Logout(object sender, RoutedEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {

            }          
            bool level = await Pronto.GetNetworkLevelUsingGoogle();
            if (!level)             
            {
                ShowDialog("Already DisConnected ");
                return;
            }
            else
            {
                var networkName = await Network.Pronto.GetNetwoksSSid();
                if (networkName.Equals("OK"))
                {
                    Busy.SetBusy(true, "Logging Out");
                    var response = await Pronto.Logout();                   
                    Busy.SetBusy(false);
                    ShowDialog(response);
                }
                else
                {
                    Busy.SetBusy(false);
                    ShowDialog("Not Connected To Vit 2.4G");
                }
            }

        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["user"] = User.Text;
            localSettings.Values["pass"] = Pass.Password;
            NetworkConnectivityLevel _level = await Network.Pronto.GetNetworkLevel();
            bool level = await Pronto.GetNetworkLevelUsingGoogle();
            if (_level.ToString().ToLower().In("internetaccess") && level)
            {
                ShowDialog("Already Connected ");
                return;

            }
            else {
                var networkName = await Network.Pronto.GetNetwoksSSid();
                 if (networkName.Equals("OK", StringComparison.CurrentCultureIgnoreCase))
                    {
                    Busy.SetBusy(true, "Logging In");
                    var response = await Network.Pronto.Login();
                    Busy.SetBusy(false);
                    ShowDialog(response);
                    }
                else
                {
                    Busy.SetBusy(false);
                    ShowDialog("Not Connected To Vit 2.4G");
                }
            }
        }


      private  bool isValid
        {
            get
            {

           
                if (User.Text.Length != 0 && Pass.Password.Length != 0)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }

        }

        private void Pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (isValid)
            {
                register.IsEnabled = true;
                LoginButton.IsEnabled = true;
                LogoutButton.IsEnabled = true;

            }
            else
            {
                 register.IsEnabled = false;
                LoginButton.IsEnabled = false;
                LogoutButton.IsEnabled = false;

            }
            if(Registered)
            {
                register.IsEnabled = true;
            }

        }

        private void User_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isValid)
            {
                register.IsEnabled = true;
                LoginButton.IsEnabled = true;
                LogoutButton.IsEnabled = true;

            }
            else
            {
                register.IsEnabled = false;
                LoginButton.IsEnabled = false;
                LogoutButton.IsEnabled = false;

            }

            if (Registered)
            {
                register.IsEnabled = true;
            }

        }
    }
}
