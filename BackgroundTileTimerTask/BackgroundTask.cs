using Network;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTileTimerTask
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var backgroundTaskDeferral = taskInstance.GetDeferral();
            try { await UpdateTile(); }
            catch (Exception ex) { Debug.WriteLine(ex); }
            finally { backgroundTaskDeferral.Complete(); }
        }

        private async Task UpdateTile()
        {
            await Pronto.TileUpdater();
        }
    }
}
