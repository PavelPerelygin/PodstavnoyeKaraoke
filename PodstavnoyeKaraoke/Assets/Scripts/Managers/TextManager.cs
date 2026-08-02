using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class TextManager
    {
        public static TextManager _textManager;
        private static DateTime _lastUpdate;
        
        private Dictionary<int, string> _lines = new Dictionary<int, string>();
    
        public TextManager()
        {
            _lastUpdate = DateTime.Now;
            
            string preFix = "_ru";
            if (Application.systemLanguage == SystemLanguage.Russian)
                preFix = "_ru";
        
            LoadGameText("main_text"+preFix);
            LoadGameText("game_text"+preFix);
        }

        public static TextManager Instance()
        {
            if (_textManager == null)
                _textManager = new TextManager();

            if(DateTime.Now.Subtract(_lastUpdate).Seconds > 3)
                _textManager = new TextManager();

            return _textManager;
        }
    
        private void LoadGameText(string nameJson)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine("localizations", nameJson));
            if (textAsset == null)
                return;
        
            string [] lines = Regex.Split ( textAsset.text, "\n|\r|\r\n" );
            foreach (string line in lines)
            {
                int number = -1;
                string value = "";

                ParseLine(line, ref number, ref value);

                if (number >= 0)
                {
                    if (!_lines.ContainsKey(number))
                        _lines.Add(number,value);
                    else
                        Log.Assert("duplicate string " + number);
                }
            }
        }
    
        private void ParseLine(string line,ref int number, ref string value)
        {
            Regex regexNumber = new Regex(@"^[0-9]*");
            Regex regexValue = new Regex(@"\s+.*");
        
            MatchCollection matchesNumber = regexNumber.Matches(line);
            if (matchesNumber.Count > 0)
            {
                foreach (Match match in matchesNumber)
                    if (!int.TryParse(match.Value, out number))
                        number = -1;
            }
        
            MatchCollection matchesValue = regexValue.Matches(line);
            if (matchesValue.Count > 0)
            {
                foreach (Match match in matchesValue)
                {
                    value = match.Value;
                    value = Regex.Replace(value, @"^\s+", "");
                }
            }
        }
    
        public string GetText(int id, bool isUpper = false)
        {
            var value = _lines.ContainsKey(id) ? _lines[id] : "not text";
            if (isUpper)
                value = value.ToUpper();
            
            return value;
        }
    }
}