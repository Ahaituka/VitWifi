using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Networking.Connectivity;

namespace VitWifi.VoiceCommands
{
    /// <summary>
    /// The VitWifiVoiceCommandService implements the entry point for all voice commands.
    /// The individual commands supported are described in the VCD xml file. 
    /// The service entry point is defined in the appxmanifest.
    /// </summary>
    public sealed class VitWifiVoiceCommandService : IBackgroundTask
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

        /// <summary>
        /// The background task entrypoint. 
        /// 
        /// Background tasks must respond to activation by Cortana within 0.5 seconds, and must 
        /// report progress to Cortana every 5 seconds (unless Cortana is waiting for user
        /// input). There is no execution time limit on the background task managed by Cortana,
        /// but developers should use plmdebug (https://msdn.microsoft.com/library/windows/hardware/jj680085%28v=vs.85%29.aspx)
        /// on the Cortana app package in order to prevent Cortana timing out the task during
        /// debugging.
        /// 
        /// The Cortana UI is dismissed if Cortana loses focus. 
        /// The background task is also dismissed even if being debugged. 
        /// Use of Remote Debugging is recommended in order to debug background task behaviors. 
        /// Open the project properties for the app package (not the background task project), 
        /// and enable Debug -> "Do not launch, but debug my code when it starts". 
        /// Alternatively, add a long initial progress screen, and attach to the background task process while it executes.
        /// </summary>
        /// <param name="taskInstance">Connection to the hosting background service process.</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Create the deferral by requesting it from the task instance
            serviceDeferral = taskInstance.GetDeferral();

            AppServiceTriggerDetails triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null && triggerDetails.Name.Equals("VitWifiVoiceCommandService"))
            {
                voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                // Perform the appropriate command depending on the operation defined in VCD
                switch (voiceCommand.CommandName)
                {
                    case "Login":
                        string x = NetworkNames.ToString();
                        VoiceCommandUserMessage userMessage = new VoiceCommandUserMessage();
                        userMessage.DisplayMessage = string.Format("The current networks is {0} ", x);
                        userMessage.SpokenMessage = string.Format("The current networks is {0} ", x);

                        VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                        break;

                    case "Logout":
                        string logoutMessage = NetworkNames.ToString();
                        VoiceCommandUserMessage userLogoutMessage = new VoiceCommandUserMessage();
                        userLogoutMessage.DisplayMessage = string.Format("The current networks is {0} ", logoutMessage);
                        userLogoutMessage.SpokenMessage = string.Format("The current networks is {0} ", logoutMessage);
                        VoiceCommandResponse logoutResponse = VoiceCommandResponse.CreateResponse(userLogoutMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(logoutResponse);
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
