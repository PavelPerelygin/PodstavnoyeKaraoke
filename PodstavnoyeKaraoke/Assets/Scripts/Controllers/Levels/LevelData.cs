using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using File = Utilities.Files.File;

namespace Controllers.Levels
{
    [Serializable]
    public class LevelData
    {
        public string guid = "";
        public string version = "";
        public string checksum = "";
        public string nameLevel = "";
        public int startDelay;
        public float lenghtFinalAudioFile;
        public int speed = 250;
        public string textFont = "";
        public int sizeFont = 70;
        public List<ObstacleData> obstacles = new List<ObstacleData>();
        public List<TextData> texts = new List<TextData>();
        public List<BombData> bombs = new List<BombData>();
        public List<StarData> stars = new List<StarData>();
        public List<CoinData> coins = new List<CoinData>();
        public List<GiftData> gifts = new List<GiftData>();
        public List<RubyData> rubies = new List<RubyData>();
        
        [NonSerialized] private AudioClip _finalAudioClip;
        [NonSerialized] private bool _isFinished;
        
        private const string _nameFinalAudioClip = "final_audio.wav";
        private const string _nameJson = "level_data.json";

        public event Action OnFinished;

        #region Version

        public string GetVersion()
        {
            return version;
        }

        #endregion
        
        #region Check sum

        public string GetChecksum()
        {
            return checksum;
        }

        public void SetChecksum(string value)
        {
            checksum = value;
        }

        #endregion

        #region Name

        public string GetNameLevel()
        {
            return nameLevel;
        }

        #endregion

        #region Guid

        public string GetGuid()
        {
            return guid;
        }

        #endregion
        
        #region Final audio clip

        public AudioClip GetFinalAudioClip()
        {
            if (_finalAudioClip == null)
            {
                var path = $"{File.GetPathToStreamingAssets()}/Content/Levels/{GetGuid()}/{_nameFinalAudioClip}";
                _finalAudioClip = File.LoadAudioClip(path);
            }
            
            return _finalAudioClip;
        }

        #endregion

        #region Speed

        public void SetSpeed(int value)
        {
            speed = value;
            if (speed <= 0) speed = 1;
        }

        public int GetSpeed()
        {
            return speed;
        }

        #endregion
        
        #region Text font

        public string GetTextFont()
        {
            return textFont;
        }

        #endregion
        
        #region Size font

        public int GetSizeFont()
        {
            return sizeFont;
        }

        #endregion
        
        #region Start delay

        public void SetStartDelay(int value)
        {
            startDelay = value;
        }

        public int GetStartDelay()
        {
            return startDelay;
        }

        #endregion

        #region Lenght final audio file

        public void SetLenghtFinalAudioClip(float value)
        {
            lenghtFinalAudioFile = value;
        }

        public float GetLenghtFinalAudioClip()
        {
            return lenghtFinalAudioFile;
        }

        #endregion

        #region Obstacles

        public List<ObstacleData> GetObstacles()
        {
            return obstacles;
        }

        #endregion

        #region Other items

        public List<BombData> GetBombs()
        {
            return bombs;
        }
        
        public List<StarData> GetStars()
        {
            return stars;
        }
        
        public List<CoinData> GetCoins()
        {
            return coins;
        }

        public List<GiftData> GetGifts()
        {
            return gifts;
        }

        public List<RubyData> GetRubies()
        {
            return rubies;
        }
        
        #endregion

        #region Text content

        public List<TextData> GetTexts()
        {
            return texts;
        }

        public void AddText(TextData textData)
        {
            texts.Add(textData);
        }

        public void RemoveText(TextData textData)
        {
            if(!texts.Contains(textData))
                return;
            
            texts.Remove(textData);
        }

        #endregion

        #region Finished

        public bool GetIsFinished()
        {
            return _isFinished;
        }

        public void SetIsFinished(bool value)
        {
            _isFinished = value;
            
            OnFinished?.Invoke();
        }

        #endregion
        
        #region Get / set
        
        public string GetPathToSave()
        {
            return File.PathCombine(File.GetPathToStreamingAssets(), $"Content/Levels/{GetGuid()}");
        }

        public List<string> GetPathsToContents()
        {
            var result = new List<string>();
            
            result.Add($"{GetPathToSave()}/{_nameFinalAudioClip}");
            
            return result;
        }

        #endregion

        #region Save / remove

        public void Remove()
        {
            var path = $"{File.GetPathToStreamingAssets()}/Content/Levels/{GetGuid()}";
            
            Directory.Delete(path,true);
        }
        
        public static LevelData Load(string path)
        {
            var pathToJson = $"{path}/{_nameJson}";

            LevelData tarckData = null;
            
            if (File.FileExists(pathToJson))
            {
                try
                {
                    tarckData = JsonUtility.FromJson<LevelData>(File.ReadAllText(pathToJson));
                }
                catch (Exception e)
                {
                    tarckData = new LevelData();
                }
            }
            else
            {
                tarckData = new LevelData();
            }
            
            return tarckData;
        }

        #endregion
    }
}