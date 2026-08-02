using System;
using System.Collections.Generic;
using System.Globalization;
using Dialogs;
using Dialogs.Base;
using UnityEngine;
using Utilities;

namespace Controllers.GameChanges
{
    public class GameChangesController
    {
        private Controllers.GameChanges.GameChanges _gameChanges;
        public GameChangesController()
        {
            _gameChanges = Resources.Load<Controllers.GameChanges.GameChanges>("Data/GameChanges");
            if(_gameChanges == null)
                Log.Assert();
        }
        
        public void CheckNeedShowChangeLogDialog()
        {
            if(_gameChanges == null)
                return;
            
            if(MainController.Instance.DialogsController.GetCountDialogs() > 0)
                return;
            
            var currentVersion = MainController.Instance.GetApplicationVersion();
            
            if (!MainController.Instance.UserSettings.IsNotNullFlag("latest_installed_version"))
            {
                MainController.Instance.UserSettings.SetFlag("latest_installed_version",currentVersion.ToString(CultureInfo.InvariantCulture.NumberFormat));
                return;
            }

            var lastSaveVersion = MainController.Instance.UserSettings.GetFlagFloat("latest_installed_version");
            if(Math.Abs(currentVersion - lastSaveVersion) == 0 || lastSaveVersion > currentVersion)
                return;

            var startFindVersion = lastSaveVersion;

            var pages = new List<GameChangesPage>();
            
            while (startFindVersion < currentVersion)
            {
                startFindVersion += 0.1f;
                pages.AddRange( GetChangeLogPageByVersion(startFindVersion));
            }

            if (pages.Count > 0)
            {
                var dialog =
                    MainController.Instance.DialogsController.CreateDialog(TypeDialog.GameChanges) as GameChangesDialog;
                if (dialog != null)
                {
                    dialog.Init(pages);
                    dialog.Show(0.4f);
                }
            }

            MainController.Instance.UserSettings.SetFlag("latest_installed_version",currentVersion.ToString(CultureInfo.InvariantCulture.NumberFormat));
        }

        private List<GameChangesPage> GetChangeLogPageByVersion(float version)
        {
            if (_gameChanges == null)
                return null;

            var pages = new List<GameChangesPage>();

            for (int i = 0; i < _gameChanges._data.Count; i++)
            {
                var gameChanges = _gameChanges._data[i];
                if (gameChanges == null)
                {
                    Log.Assert();
                    continue;
                }

                if (gameChanges._typePlatform != TypePlatform.Any)
                {
#if UNITY_STANDALONE_WIN
                    if(gameChanges._typePlatform != TypePlatform.Windows)
                        continue; 
#elif UNITY_STANDALONE_OSX
                    if(gameChanges._typePlatform != TypePlatform.MacOs)
                        continue;
#endif
                }

                if (Math.Abs(gameChanges._version - version) == 0)
                {
                    for (int j = 0; j < gameChanges._pathToPages.Count; j++)
                    {
                        var path = gameChanges._pathToPages[j];
                        if (path == "")
                        {
                            Log.Assert();
                            continue;
                        }

                        var page = Resources.Load<GameChangesPage>(path);
                        if (page == null)
                        {
                            Log.Assert();
                            continue;
                        }
                        
                        pages.Add(page);
                    }
                }
            }

            return pages;
        }
    }
}