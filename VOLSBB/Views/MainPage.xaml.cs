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

namespace VOLSBB.Views
{
    public sealed partial class MainPage : Page
    {
       private const string TASK_NAME = "TILE_UPDATE_TIMER_TASK_SAMPLE";
       public static bool Registered;        
        
     
        private async void ShowDialog(string message)
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
         
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "SampleBackgroundTask")
                {
                    Registered = true;
                }
            }
            if (Registered)
            {
                register.Content = "Unregister";

            }
        }

      private async void Register(object sender, RoutedEventArgs e)
        {
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
                BackgroundTaskHelper.Register("SampleBackgroundTask", "Tasks.SampleBackgroundTask", new SystemTrigger(SystemTriggerType.NetworkStateChange, false), false, true, null);
                BackgroundTaskHelper.Register("ToastBackgroundTask", "Tasks.ToastBackgroundTask", new ToastNotificationActionTrigger(), false, false, null);
                RegisterTask();
                register.Content = "UnRegister";
                Registered = true;
            }
        }

  
        private void Logout(object sender, RoutedEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
               
            }
        }

        private void Login(object sender, RoutedEventArgs e)
        {
           
        }



    }
}
