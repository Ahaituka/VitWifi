//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Notifications;
using Network;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Notifications;

//
// The namespace for the background tasks.
//
namespace Tasks
{
    //
    // A background task always implements the IBackgroundTask interface.
    //
    public sealed class SampleBackgroundTask : IBackgroundTask
    {
       

       
        BackgroundTaskDeferral _deferral = null;     
        private async  Task BackgroundLogin()
        {
            bool level = await Pronto.GetNetworkLevelUsingGoogle();
            if (level)
            {
                return;
            }
            else
            {
                var networkName = await Network.Pronto.GetNetwoksSSid();
                if (networkName.Equals("OK", StringComparison.CurrentCultureIgnoreCase))
                {
                    PopToast("Vit 2.4G");
                }

                else
                {
                    return;
                }

            }
           
        }

        //var x = 0;

    

        //
        // The Run method is the entry point of a background task.
        //
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

             await BackgroundLogin();
            _deferral.Complete();


        }

        private void PopToast(string msg)
        {
            // Generate the toast notification content and pop the toast
            ToastContent content = new ToastContent()
            {
                Launch = "action=viewEvent&eventId=1983",
                Scenario = ToastScenario.Reminder,

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = msg
                    },              
                  
                }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                  

                    Buttons =
            {
                new ToastButton("Login", "Login")
            {
                ActivationType = ToastActivationType.Background
            },


            }
                }
            };
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

      
    }
}
