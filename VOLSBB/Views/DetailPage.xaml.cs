using VOLSBB.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Network;
using Windows.Storage;
using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Toolkit.Uwp;

namespace VOLSBB.Views
{
    public sealed partial class DetailPage : Page
    {
        bool credentialEnterned = false;
        string user, pass;
        public string keyLargeObject = "dataUsage.txt";
        public DetailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
         
         

        }

        protected async  override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                user = localSettings.Values["user"].ToString();
                pass = localSettings.Values["pass"].ToString();
                credentialEnterned = true;
            }
            catch
            {
                refershButton.IsEnabled = false;
                return;
            }
            if(credentialEnterned)
            {
                refershButton.IsEnabled = true;
            }

            var helper = new LocalObjectStorageHelper();
            if (await helper.FileExistsAsync(keyLargeObject))
            {
                var dataList = await helper.ReadFileAsync<DataList>(keyLargeObject);
                dataLimit.Text = dataList.planList[0].ToString();
                dataStartDate.Text = dataList.planList[1].ToString();
                dataEndDate.Text = dataList.planList[2].ToString();
                dataTime.Text = dataList.usageList[0].ToString();
                dataUploaded.Text = dataList.usageList[1].ToString();
                dataDownloaded.Text = dataList.usageList[2].ToString();
                dataTotal.Text = dataList.usageList[3].ToString();
            }
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            try
            {
                Busy.SetBusy(true, "Checking Usage");
                var dataList = await Pronto.DataUsage();
                if (dataList.errorList.Count == 0)
                {
                    dataLimit.Text = dataList.planList[0].ToString();
                    dataStartDate.Text = dataList.planList[1].ToString();
                    dataEndDate.Text = dataList.planList[2].ToString();
                    dataTime.Text = dataList.usageList[0].ToString();
                    dataUploaded.Text = dataList.usageList[1].ToString();
                    dataDownloaded.Text = dataList.usageList[2].ToString();
                    dataTotal.Text = dataList.usageList[3].ToString();
                    Busy.SetBusy(false);
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile sampleFile = await localFolder.CreateFileAsync("dataUsage.txt", CreationCollisionOption.ReplaceExisting);

                    ////Read the first line of dataFile.txt in LocalFolder and store it in a String
                    //StorageFile sampleFiile = await localFolder.GetFileAsync("dataFittlee.txt");
                    //IList<string> fileContent = await FileIO.ReadLinesAsync(sampleFiile);

                    // Read complex/large objects 
                    var helper = new LocalObjectStorageHelper();
                    await helper.SaveFileAsync(keyLargeObject, dataList);

                    Pronto.ValueTileUpdater(dataList.usageList[2].ToString());

                   

                }
                else
                {
                    Busy.SetBusy(false);
                    MainPage.ShowDialog(dataList.errorList[0]);
                }
         

         


            }
            catch (System.Exception)
            {
                Busy.SetBusy(false);
                return;
            }

        }

      
    }
}

