using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Web.Http;

namespace Network
{
    public class Pronto
        {
        //Create an HTTP client object
         private static Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
         private const string uriString = "http://phc.prontonetworks.com/cgi-bin/authlogin?URI=http://phc.prontonetworks.com/";
        private const string logoutUriString = "http://ap.logout";
        //Add a user-agent header to the GET request. 
        private const string WP_USER_AGENT = "Mozilla/5.0 (Mobile; Windows Phone 8.1; Android 4.0; ARM; Trident/7.0; Touch; rv:11.0; IEMobile/11.0; NOKIA; Lumia 520) like iPhone OS 7_0_3 Mac OS X AppleWebKit/537 (KHTML, like Gecko) Mobile Safari/537";
         private static Windows.Web.Http.Headers.HttpRequestHeaderCollection headers = httpClient.DefaultRequestHeaders;
         private static readonly Windows.Web.Http.HttpClient _httpClient = new Windows.Web.Http.HttpClient();
       public static async Task<string> Login()
         {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string user = localSettings.Values["user"].ToString();
            string pass = localSettings.Values["pass"].ToString();
            var postContent = new HttpFormUrlEncodedContent(
                                    new KeyValuePair<string, string>[3] {
                                          new KeyValuePair<string, string>("serviceName", "ProntoAuthentication"),
                                          new KeyValuePair<string, string>("userId", user),
                                          new KeyValuePair<string, string>("password", pass)
                                    });
    
             //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.           
            if (!headers.UserAgent.TryParseAdd(WP_USER_AGENT))
            {
                throw new Exception("Invalid header value: " + WP_USER_AGENT);
            }
           //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                //Send the GET request
                httpResponse = await httpClient.PostAsync(new Uri(uriString),postContent);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            return httpResponseBody;
        
    }

        public static async Task<string> Logout()
        {
            if (!headers.UserAgent.TryParseAdd(WP_USER_AGENT))
            {
                throw new Exception("Invalid header value: " + WP_USER_AGENT);
            }
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(new Uri(logoutUriString));
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            return httpResponseBody;

        }


        public static async Task<string> DataUsage()
        {
            return null;

        }

        public static  void PopToast(string msg)
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



    }
}
