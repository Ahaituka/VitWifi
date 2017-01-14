using Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Networking.Connectivity;

namespace CortanaComponent
{
    public sealed class HomeControlVoiceCommandService : IBackgroundTask
    {
        private VoiceCommandServiceConnection voiceServiceConnection;
        private BackgroundTaskDeferral serviceDeferral;
        public static IReadOnlyList<string> NetworkNames
        {
            get
            {
                ConnectionProfile profile = null;

                try
                {
                    profile = NetworkInformation.GetInternetConnectionProfile();
                }
                catch
                {
                }

                if (profile != null)
                {
                    return profile.GetNetworkNames();
                }

                return null;
            }
        }
        private static string Login()
        {
            return null;
        }
        private static string DataUsage()
        {
            return null;
        }
        private static string Logout()
        {
            return null;
        }


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Create the deferral by requesting it from the task instance
            serviceDeferral = taskInstance.GetDeferral();

            AppServiceTriggerDetails triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null && triggerDetails.Name.Equals("VoiceCommandService"))
            {
                voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                // Perform the appropriate command depending on the operation defined in VCD
                switch (voiceCommand.CommandName)
                {
                    case "Login":
                        string loginMessage;
                        bool loginlevel = await Pronto.GetNetworkLevelUsingGoogle();
                        if (loginlevel)
                        {
                            loginMessage = " You are already Connected ";

                        }
                        else
                        {
                            var networkName = await Network.Pronto.GetNetwoksSSid();
                            if (networkName.Equals("OK"))
                            {
                                loginMessage = await Pronto.Login();
                            }
                            else
                            {
                                loginMessage = "You are Not Connected To Vit 2.4G";
                            }
                        }
                        VoiceCommandUserMessage userLoginMessage = new VoiceCommandUserMessage();
                        userLoginMessage.DisplayMessage = loginMessage;
                        userLoginMessage.SpokenMessage = loginMessage;
                        VoiceCommandResponse loginResponse = VoiceCommandResponse.CreateResponse(userLoginMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(loginResponse);
                        break;

                    case "Logout":
                        string logoutMessage;
                        bool level = await Pronto.GetNetworkLevelUsingGoogle();
                        if (!level)
                        {
                            logoutMessage=" You are already Disconnected ";
                            
                        }
                        else
                        {
                            var networkName = await Network.Pronto.GetNetwoksSSid();
                            if (networkName.Equals("OK"))
                            {                               
                                logoutMessage = await Pronto.Logout();                              
                            }
                            else
                            {                                
                                logoutMessage="You are Not Connected To Vit 2.4G";
                            }
                        }
                        VoiceCommandUserMessage userLogoutMessage = new VoiceCommandUserMessage();
                        userLogoutMessage.DisplayMessage = logoutMessage;
                        userLogoutMessage.SpokenMessage = logoutMessage;
                        VoiceCommandResponse logoutResponse = VoiceCommandResponse.CreateResponse(userLogoutMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(logoutResponse);
                        break;

                    case "Usage":
                        string dataMessage;
                        bool usageLevel = await Pronto.GetNetworkLevelUsingGoogle();
                        if (!usageLevel)
                        {
                            dataMessage = "Sorry Internet is Unavialable";

                        }
                        else
                        {                          
                            var data = await Pronto.DataUsage();
                            var consumed = data.usageList[3];
                            dataMessage = string.Format("Your Monthly Data Consumed is :" + consumed);
                            
                            
                        }
                        VoiceCommandUserMessage userDataMessage = new VoiceCommandUserMessage();
                        userDataMessage.DisplayMessage = dataMessage;
                        userDataMessage.SpokenMessage = dataMessage;
                        VoiceCommandResponse dataUsageResponse = VoiceCommandResponse.CreateResponse(userDataMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(dataUsageResponse);
                        break;

                    default:
                        break;
                }
            }

            // Once the asynchronous method(s) are done, close the deferral
            serviceDeferral.Complete();
        }
    }
}