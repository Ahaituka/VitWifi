using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VOLSBB.Views
{
    public sealed partial class SettingsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage()
        {
            //UseLightThemeToggleSwitch.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            MyPivot.SelectedIndex = index;
        }

        private async void CallButton(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            var uriSkype = new Uri(@"Skype:(mailshubh@yahoo.in)?call");


            // Set the option to show a warning
            var promptOptions = new Windows.System.LauncherOptions();
        promptOptions.TreatAsUntrusted = true;

    // Launch the URI
    var success = await Windows.System.Launcher.LaunchUriAsync(uriSkype, promptOptions);
  
        }

        private async void MailButton(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                EmailMessage emailMessage = new EmailMessage()
                {
                    Subject = "FeedBack Regarding Windows VitWifi App: ",
                    Body = ""
                };
                emailMessage.To.Add(new EmailRecipient() { Address = "bitloks@gmail.com" });
                await EmailManager.ShowComposeNewEmailAsync(emailMessage);
            }
            catch (Exception ec)
            {
                MainPage.ShowDialog(ec.ToString());
            }


        }
    }
}
