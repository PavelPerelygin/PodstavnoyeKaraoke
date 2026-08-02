using System.Collections.Generic;
using System.Globalization;
using Dialogs;
using Dialogs.Base;
using UnityEngine;
using Utilities.Network;

namespace Controllers.Update
{
    public class UpdateController
    {
        public async void CheckUpdateGame()
        {
            if(MainController.Instance.DialogsController.GetCountDialogs() > 0)
                return;
            
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"NameGame", Application.productName},
                {"KeyGame", MainController.Instance.GameSettings.GameKeyCode}
            };

            string url = $"{MainController.Instance.RemoteSettings.WebsiteUrl}/v2/UpdateManager.php";
            await HttpClient.GetRequest(url, parameters, (string responseStr) =>
            {
                UpdateInfo updateGame = JsonUtility.FromJson<UpdateInfo>(responseStr);
        
                if(updateGame == null)
                    return;

                float newVersion = 0f;
                if (float.TryParse(updateGame.Version,NumberStyles.Float,CultureInfo.InvariantCulture.NumberFormat, out newVersion))
                {
                    float currentVersion = MainController.Instance.GetApplicationVersion();
                    if (currentVersion > 0 && newVersion > currentVersion)
                    {
                        var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Update) as UpdateDialog;
                        dialog.Init(updateGame);
                        dialog.Show();
                    }
                }
            });
        }
    }
}