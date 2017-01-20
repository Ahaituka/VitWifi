using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
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
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

using NotificationsExtensions.Tiles;

namespace Network
{
    public static class Constants
    {
        public static readonly Color ApplicationBackgroundColor = Color.FromArgb(255, 51, 51, 51);
        public static readonly Uri Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.png");
        public static readonly Uri Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png");
        public static readonly Uri Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
        public static readonly Uri Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");

        public static string ApplicationDisplayName { get; set; }
    }
    public static class Ext
    {
        public static bool In<T>(this T item, params T[] items)
        {
            return items.Contains(item);
        }


    }
    public class DataList
    {
        public List<string> planList { get; set; }
        public List<string> usageList { get; set; }
        public List<string> errorList { get; set; }
        public DataList()
        {
            planList = new List<string>();
            usageList = new List<string>();
            errorList = new List<string>();


        }
    }
    public class Pronto
    {
        private static Microsoft.Toolkit.Uwp.Notifications.TileContent _tileContent;

        public static bool pinned = false;

        private const string uriString = "http://115.248.50.60/registration/chooseAuth.do";
        private const string checkUriString = "http://115.248.50.60/registration/Main.jsp?wispId=1&nasId=00:15:17:c8:09:b1";
        private const string planString = "http://115.248.50.60/registration/main.do?content_key=%2FSelectedPlan.jsp";
        private const string histroyString = "http://115.248.50.60/registration/customerSessionHistory.do";
        //Create an HTTP client object
        private static Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
        private const string loginString = "http://phc.prontonetworks.com/cgi-bin/authlogin?URI=http://phc.prontonetworks.com/";
        private const string logoutUriString = "http://phc.prontonetworks.com/cgi-bin/authlogout";
        private const string checkInternetUriString = "http://www.google.com";
        //Add a user-agent header to the GET request. 
        //  private const string WP_USER_AGENT = "Mozilla/5.0 (Mobile; Windows Phone 8.1; Android 4.0; ARM; Trident/7.0; Touch; rv:11.0; IEMobile/11.0; NOKIA; Lumia 520) like iPhone OS 7_0_3 Mac OS X AppleWebKit/537 (KHTML, like Gecko) Mobile Safari/537";
        private const string WP_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
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
                httpResponse = await httpClient.GetAsync(new Uri(checkInternetUriString));

                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

            }
            catch 
            {

                return false;
            }
            var succes = httpResponseBody.Contains("authlogin");
            var gSuccess = httpResponseBody.Contains("Google");
            if (!succes)
                return true;
            else
                return false;

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

           // return 0;

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

            var error = "No credentials entered";
            string user, pass;
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                user = localSettings.Values["user"].ToString();
                pass = localSettings.Values["pass"].ToString();
            }
            catch
            {

                return error;
            }



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
                httpResponse = await httpClient.PostAsync(new Uri(loginString), postContent);
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
            if (succes)
            {
                return "Login Succesful";


            }
            else if (invalid)
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
            if (exist)
            {

                return "You have successfully logged out";
            }

            else
            {
                return "No active Session To Logout";

            }


        }
        public static async Task<DataList> DataUsage()
        {
            var _level = await GetNetworkLevelUsingGoogle();
            DataList mainList = new DataList();
            if (_level)
            {
               

                string user, pass;
                try
                {
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    user = localSettings.Values["user"].ToString();
                    pass = localSettings.Values["pass"].ToString();
                }
                catch
                {
                    List<string> errorList = new List<string>();
                    string httpResponseBody = "No credentials entered";
                    errorList.Add(httpResponseBody);
                    mainList.errorList = errorList;
                    return mainList;
                }

                //  DataList mainList = new DataList();
                try
                {
                    var postContent = new FormUrlEncodedContent(
                                            new KeyValuePair<string, string>[4] {
                                          new KeyValuePair<string, string>("loginUserId", user),
                                          new KeyValuePair<string, string>("authType", "Pronto"),
                                          new KeyValuePair<string, string>("loginPassword", pass),
                                          new KeyValuePair<string, string>("submit", "Login")
                                            });
                    System.Net.Http.HttpResponseMessage httpResponse;
                    // Create a new parser front-end (can be re-used)
                    var parser = new HtmlParser();
                    System.Net.CookieContainer cookies = new System.Net.CookieContainer();
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.CookieContainer = cookies;
                    System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(handler);
                    //  _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(WP_USER_AGENT);
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(WP_USER_AGENT);
                    httpResponse = await httpClient.GetAsync(new Uri(checkUriString));
                    string planRessponseBody = await httpResponse.Content.ReadAsStringAsync();
                    httpResponse = await httpClient.PostAsync(new Uri(uriString), postContent);
                    string postRespondeBody = await httpResponse.Content.ReadAsStringAsync();
                    var invalid = postRespondeBody.Contains("again");
                    if (invalid)
                    {
                        List<string> errorList = new List<string>();
                        string _invalid = "Invalid credentials entered";
                        errorList.Add(_invalid);
                        mainList.errorList = errorList;
                        return mainList;

                    }

                    httpResponse = await httpClient.GetAsync(new Uri(planString));
                    string planResponseBody = await httpResponse.Content.ReadAsStringAsync();
                    var planDocument = parser.Parse(planResponseBody);
                    var planItemsCssSelector = planDocument.QuerySelectorAll("td[class = 'mainTextLeft']");
                    List<string> planList = new List<string>();
                    foreach (var item in planItemsCssSelector)
                    {
                        planList.Add(item.TextContent.Trim());

                    }
                    planList.RemoveAt(1); planList.RemoveAt(3);
                    IFormatProvider culture = new System.Globalization.CultureInfo("en-US");
                    DateTime startDate = DateTime.Parse(planList[1], culture);
                    DateTime endDate = DateTime.Parse(planList[2], culture);
                    DateTime today = DateTime.Today.AddMonths(-1);
                    var x = startDate.Day.ToString();
                    var historyParams = new FormUrlEncodedContent(
                 new KeyValuePair<string, string>[]
                 {
                     new KeyValuePair<string, string>("location", "allLocations"),
                     new KeyValuePair<string, string>("parameter", "custom"),
                     new KeyValuePair<string, string>("customStartMonth",today.Month.ToString()),
                     new KeyValuePair<string, string>("customStartDay", x),
                     new KeyValuePair<string, string>("customStartYear", today.Year.ToString()),
                     new KeyValuePair<string, string>("customEndMonth", "00"),
                     new KeyValuePair<string, string>("customEndDay", "01"),
                     new KeyValuePair<string, string>("customEndYear", 2022.ToString()),
                     new KeyValuePair<string, string>("button", "View"),
                 });
                    httpResponse = await httpClient.PostAsync(new Uri(histroyString), historyParams);
                    string httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                    //Just get the DOM representation for Data usage
                    var usagedocument = parser.Parse(httpResponseBody);
                    var usageItemsCssSelector = usagedocument.QuerySelectorAll("td[colspan = '3']");

                    List<string> usageList = new List<string>();
                    foreach (var item in usageItemsCssSelector)
                    {
                        var usageTime = item.NextElementSibling;
                        usageList.Add(usageTime.Text().Trim());
                        var uploadData = usageTime.NextElementSibling;
                        usageList.Add(uploadData.Text().Trim());
                        var downloadData = uploadData.NextElementSibling;
                        usageList.Add(downloadData.Text().Trim());
                        var totalData = downloadData.NextElementSibling;
                        usageList.Add(totalData.Text().Trim());
                    }
                    mainList.planList = planList;
                    mainList.usageList = usageList;
                    return mainList;
                }
                catch (Exception ex)
                {
                    List<string> errorList = new List<string>();
                    string httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                    errorList.Add(httpResponseBody);
                    mainList.errorList = errorList;
                    return mainList;
                } 
            }

            else
            {
                List<string> errorList = new List<string>();
                string _internet = "No Internet connection";
                errorList.Add(_internet);
                mainList.errorList = errorList;
                return mainList;
            }
        }

        public static void PopToast(string msg)
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
            var toastNotification = new ToastNotification(content.GetXml());
            var notification = ToastNotificationManager.CreateToastNotifier();
            notification.Show(toastNotification);
            //  ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
            toastNotification.ExpirationTime = DateTime.Now.AddSeconds(5);
        }
        private static Microsoft.Toolkit.Uwp.Notifications.TileBinding GenerateTileBindingMedium(string username)
        {
            var time = DateTime.Now;
            return new Microsoft.Toolkit.Uwp.Notifications.TileBinding()
            {
                Content = new Microsoft.Toolkit.Uwp.Notifications.TileBindingContentAdaptive()
                {

                    TextStacking = Microsoft.Toolkit.Uwp.Notifications.TileTextStacking.Center,

                    Children =
                    {

                        new AdaptiveText()
                        {
                            Text = username,
                            HintAlign = AdaptiveTextAlign.Center,
                            HintStyle = AdaptiveTextStyle.Base

                        },
                         new AdaptiveText()
                        {
                            Text = "Consumed",
                            HintAlign = AdaptiveTextAlign.Center,
                            HintStyle = AdaptiveTextStyle.Caption

                        }

                    }
                }
            };
        }

        private static Microsoft.Toolkit.Uwp.Notifications.TileBinding GenerateTileBindingWide(string username)
        {
            return new Microsoft.Toolkit.Uwp.Notifications.TileBinding()
            {
                Content = new Microsoft.Toolkit.Uwp.Notifications.TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                new AdaptiveSubgroup()
                                {
                                    HintTextStacking = AdaptiveSubgroupTextStacking.Center,

                                    Children =
                                    {

                                        new AdaptiveText()
                                        {
                                            Text = username,
                                             HintStyle = AdaptiveTextStyle.Title

                                        },
                                        new AdaptiveText()
                                        {
                                            Text = "Consumed",
                                            HintStyle = AdaptiveTextStyle.SubtitleSubtle
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static Microsoft.Toolkit.Uwp.Notifications.TileBinding GenerateTileBindingLarge(string username)
        {
            return new Microsoft.Toolkit.Uwp.Notifications.TileBinding()
            {
                Content = new Microsoft.Toolkit.Uwp.Notifications.TileBindingContentAdaptive()
                {
                    TextStacking = Microsoft.Toolkit.Uwp.Notifications.TileTextStacking.Center,

                    Children =
                    {
                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                new AdaptiveSubgroup()
                                {
                                    HintWeight = 1
                                },

                                new AdaptiveSubgroup()
                                {
                                    HintWeight = 1
                                }
                            }
                        },

                        new AdaptiveText()
                        {
                            Text = username,
                            HintAlign = AdaptiveTextAlign.Center,
                            HintStyle = AdaptiveTextStyle.SubtitleSubtle
                        } ,
                          new AdaptiveText()
                        {
                            Text = "Consumed",
                            HintAlign = AdaptiveTextAlign.Center,
                            HintStyle = AdaptiveTextStyle.Title
                        }


                    }
                }
            };
        }
        public static Microsoft.Toolkit.Uwp.Notifications.TileContent GenerateTileContent(string username)
        {
            return new Microsoft.Toolkit.Uwp.Notifications.TileContent()
            {
                Visual = new Microsoft.Toolkit.Uwp.Notifications.TileVisual()
                {
                    TileMedium = GenerateTileBindingMedium(username),
                    TileWide = GenerateTileBindingWide(username),
                    TileLarge = GenerateTileBindingLarge(username)
                }
            };
        }
        public static async Task TileUpdater()
        {
            //    SecondaryTile tile = new SecondaryTile(DateTime.Now.Ticks.ToString())
            //    {
            //        DisplayName = "Vit 2.4G",
            //        Arguments = "args"
            //    };
            //    tile.VisualElements.Square150x150Logo = Constants.Square150x150Logo;
            //    tile.VisualElements.Wide310x150Logo = Constants.Wide310x150Logo;
            //    tile.VisualElements.Square310x310Logo = Constants.Square310x310Logo;
            //    tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            //    tile.VisualElements.ShowNameOnSquare310x310Logo = true;
            //    tile.VisualElements.ShowNameOnWide310x150Logo = true;

            //    if(pinned)
            //    {
            //        TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(new TileNotification(_tilecContent.GetXml()));
            //        return;
            //    }

            //    else if (!await tile.RequestCreateAsync())
            //    {
            //        return;
            //    }
            //    TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(new TileNotification(_tileContent.GetXml()));
            //    pinned = true;
            //
            try
            {
                var x = await DataUsage();           
                if (x.errorList.Count == 0)
                {
                    _tileContent = GenerateTileContent(x.usageList[3]);
                    var notification = new TileNotification(_tileContent.GetXml());
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
                                 
                    if(x.usageList[3].Contains("9."))
                    {
                        Pronto.PopToast("Your Monthly Data Usage have exceded 9 GB");
                    } 
                }
                else
                {
                    return;
                }

            }
            catch
            {

                return;
            }

        }




    }
}
