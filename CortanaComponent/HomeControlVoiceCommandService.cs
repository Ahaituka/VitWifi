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
                        string x = NetworkNames.ToString();
                        VoiceCommandUserMessage userMessage = new VoiceCommandUserMessage();
                        userMessage.DisplayMessage = string.Format("The current networks is {0} ",x);
                        userMessage.SpokenMessage = string.Format("The current networks is {0} ", x);

                        VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                        break;

                    case "Logout":
                        string logoutMessage = NetworkNames.ToString();
                        VoiceCommandUserMessage userLogoutMessage = new VoiceCommandUserMessage();
                        userLogoutMessage.DisplayMessage = string.Format("The current networks is {0} ", userLogoutMessage);
                        userLogoutMessage.SpokenMessage = string.Format("The current networks is {0} ", userLogoutMessage);
                        VoiceCommandResponse logoutResponse = VoiceCommandResponse.CreateResponse(userLogoutMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(logoutResponse);
                        break;

                    case "Usage":
                        string usageMessage = NetworkNames.ToString();
                        VoiceCommandUserMessage userUsageMessage = new VoiceCommandUserMessage();
                        userUsageMessage.DisplayMessage = string.Format("The current networks is {0} ", userUsageMessage);
                        userUsageMessage.SpokenMessage = string.Format("The current networks is {0} ", userUsageMessage);
                        VoiceCommandResponse usageResponse = VoiceCommandResponse.CreateResponse(userUsageMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(usageResponse);
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