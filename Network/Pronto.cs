using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Notifications;
using Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.UI.Notifications;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Network
{
    public static class Ext
    {
        public static bool In<T>(this T item, params T[] items)
        {
            return items.Contains(item);
        }

      
    }
    public class Pronto
        {
        //Create an HTTP client object
         private static Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
         private const string uriString = "http://phc.prontonetworks.com/cgi-bin/authlogin?URI=http://phc.prontonetworks.com/";
         private const string logoutUriString = "http://phc.prontonetworks.com/cgi-bin/authlogout";
        private const string checkUriString = "http://www.google.com";
        //Add a user-agent header to the GET request. 
         private const string WP_USER_AGENT = "Mozilla/5.0 (Mobile; Windows Phone 8.1; Android 4.0; ARM; Trident/7.0; Touch; rv:11.0; IEMobile/11.0; NOKIA; Lumia 520) like iPhone OS 7_0_3 Mac OS X AppleWebKit/537 (KHTML, like Gecko) Mobile Safari/537";
         private static Windows.Web.Http.Headers.HttpRequestHeaderCollection headers = httpClient.DefaultRequestHeaders;
         private static readonly Windows.Web.Http.HttpClient _httpClient = new Windows.Web.Http.HttpClient();







        /// <summary>
        /// Gets connection level for the current Wifi Connection.
        /// </summary>
        /// <returns> string value of level/></returns>     
        /// 
        public static async Task<bool> GetNetworkLevelUsingGoogle()
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
                httpResponse = await httpClient.GetAsync(new Uri(checkUriString));
               
            }
            catch(Exception ex)
            {
               
                return false;
            }

            return true;

        }

        
        /// <summary>
        /// Gets connection level for the current Wifi Connection.
        /// </summary>
        /// <returns> string value of level/></returns>     
        /// 
        public static async Task<NetworkConnectivityLevel> GetNetworkLevel()
        {
            try
            {
                WiFiAdapter firstAdapter;
                var access = await WiFiAdapter.RequestAccessAsync();
                if (access != WiFiAccessStatus.Allowed)
                {
                    return 0;
                }
                else
                {
                    var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
                    if (result.Count >= 1)
                    {
                        firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);
                    }
                    else
                    {
                        return 0;
                    }
                    var connectedProfile = await firstAdapter.NetworkAdapter.GetConnectedProfileAsync();

                    if (connectedProfile != null)
                    {
                        var x = connectedProfile.GetNetworkConnectivityLevel();
                        
                        return x;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch
            {
                return 0;
            }

            return 0;

        }

        /// <summary>
        /// Gets connection ssid for the current Wifi Connection.
        /// </summary>
        /// <returns> string value of current ssid/></returns>
        /// 
        public static async Task<string> GetNetwoksSSid()
        {
            try
            {
                WiFiAdapter firstAdapter;
                var access = await WiFiAdapter.RequestAccessAsync();
                if (access != WiFiAccessStatus.Allowed)
                {
                    return "Acess Denied";
                }
                else
                {
                    var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
                    if (result.Count >= 1)
                    {
                        firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);
                    }
                    else
                    {
                        return "No WiFi Adapters Detected";
                    }
                    var connectedProfile = await firstAdapter.NetworkAdapter.GetConnectedProfileAsync();
                
                    if (connectedProfile != null)
                    {
                        if (connectedProfile.ProfileName.ToLower().Equals("vit2.4g") || connectedProfile.ProfileName.ToLower().Equals("vit5g"))
                            return "OK";
                        else
                            return connectedProfile.ProfileName;
                    }
                    else if (connectedProfile == null)
                    {

                        return "WiFi adapter disconnected";
                    }
                }
            }
            catch
            {

            }

            return null;

        }


       public static async Task<string> Login()
         {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
             string user = localSettings.Values["user"].ToString();
            string pass = localSettings.Values["pass"].ToString();

            //string user = "wsfd";
            //string pass = "wsfd";
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
                return httpResponseBody;
            }
            NetworkConnectivityLevel _level = await Network.Pronto.GetNetworkLevel();
            var exist = httpResponseBody.Contains("account");
            var invalid = httpResponseBody.Contains("again");
            var succes = httpResponseBody.Contains("Congratulations ");
            if (_level.ToString().ToLower().In("internetaccess") || succes)
            {
                return "Login Succesful";
              

            }                     
             else if(invalid)
                {
                 return "Invalid Credentials";
                }
            else
            {
                return "Sorry that Account does  Not Exist";
            }

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
                return httpResponseBody;
            }

            var exist = httpResponseBody.Contains("successfully");
            if(exist)
            {

                return "You have successfully logged out";
            }

            else
            {
                return "No active Session To Logout";

            }

 
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
