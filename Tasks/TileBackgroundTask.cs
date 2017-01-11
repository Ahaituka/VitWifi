//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.Toolkit.Uwp.Notifications;
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
    public sealed class TileBackgroundTask : IBackgroundTask
    {
       
       /// private string savedProfileName = null;
    
        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        volatile bool _cancelRequested = false;
        BackgroundTaskDeferral _deferral = null;
        ThreadPoolTimer _periodicTimer = null;
        uint _progress = 0;
        IBackgroundTaskInstance _taskInstance = null;

        public  void Update()
        {
            // In a real app, these would be initialized with actual data
            string from = "Jennifbn,lhjer Parker";
            string subject = "Photos from our trip";
            string body = "Check out these awessdgfsdfghdfome photos I took while in New Zealand!";


            // Construct the tile content
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = from
                                },

                                new AdaptiveText()
                                {
                                    Text = subject,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = body,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = from,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = subject,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = body,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                }
            };


            // Then create the tile notification
            var notification = new TileNotification(content.GetXml());


            // And send the notification
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
            //var x = 0;

        }

        //
        // The Run method is the entry point of a background task.
        //
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");

            //
            // Query BackgroundWorkCost
            // Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
            // of work in the background task and return immediately.
            //
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["BackgroundWorkCost"] = cost.ToString();

            Update();
            //bool result = await Check();         


            //
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            //
            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            //
            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;

            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1));
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

               
            };
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        //public static ToastContent GenerateToastContent()
        //{
        //    return new ToastContent()
        //    {
        //        Launch = "action=viewEvent&eventId=1983",
        //        Scenario = ToastScenario.Reminder,

        //        Visual = new ToastVisual()
        //        {
        //            BindingGeneric = new ToastBindingGeneric()
        //            {
        //                Children =
        //        {
        //            new AdaptiveText()
        //            {
        //                Text = "Adaptive Tiles Meeting"
        //            },

        //            new AdaptiveText()
        //            {
        //                Text = "Conf Room 2001 / Building 135"
        //            },

        //            new AdaptiveText()
        //            {
        //                Text = "10:00 AM - 10:30 AM"
        //            }
        //        }
        //            }
        //        },

        //        Actions = new ToastActionsCustom()
        //        {
        //            Inputs =
        //    {
        //        new ToastSelectionBox("snoozeTime")
        //        {
        //            DefaultSelectionBoxItemId = "15",
        //            Items =
        //            {
        //                new ToastSelectionBoxItem("1", "1 minute"),
        //                new ToastSelectionBoxItem("15", "15 minutes"),
        //                new ToastSelectionBoxItem("60", "1 hour"),
        //                new ToastSelectionBoxItem("240", "4 hours"),
        //                new ToastSelectionBoxItem("1440", "1 day")
        //            }
        //        }
        //    },

        //            Buttons =
        //    {
        //        new ToastButtonSnooze()
        //        {
        //            SelectionBoxId = "snoozeTime"
        //        },

        //        new ToastButtonDismiss()
        //    }
        //        }
        //    };
        //}

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //
            _cancelRequested = true;
            _cancelReason = reason;

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }

        //
        // Simulate the background task activity.
        //
        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if ((_cancelRequested == false) && (_progress < 100))
            {
                _progress += 10;
                _taskInstance.Progress = _progress;
            }
            else
            {
                _periodicTimer.Cancel();

                var key = _taskInstance.Task.Name;

                //
                // Record that this background task ran.
                //
                String taskStatus = (_progress < 100) ? "Canceled with reason: " + _cancelReason.ToString() : "Completed";
                var settings = ApplicationData.Current.LocalSettings;
                settings.Values[key] = taskStatus;
                Debug.WriteLine("Background " + _taskInstance.Task.Name + settings.Values[key]);

                //
                // Indicate that the background task has completed.
                //
                _deferral.Complete();
            }
        }
    }
}
