// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace AMWE_Administrator
{
    public static class ConfigurationRequest
    {
        public static void WriteValueByKey(string key, string value)
        {
            Configuration appSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            KeyValueConfigurationElement item = Array.Find(appSettings.AppSettings.Settings.OfType<KeyValueConfigurationElement>().ToArray(), x => x.Key == key);
            item.Value = value;
            appSettings.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static string GetValueByKey(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        public static void LoadCheckModels(List<CheckModel> Apps, List<CheckModel> Sites)
        {
            CheckModelIndex checkModelIndex = CheckModelIndex.Apps;
            string[] s = File.ReadAllLines(@"./ReportBlackList.txt");
            foreach (string item in s)
            {
                if (item is "[Apps]" or "[Sites]")
                {
                    checkModelIndex = (CheckModelIndex)Enum.Parse(typeof(CheckModelIndex), item[1..^1]);
                    continue;
                }

                string[] cbparams = item.Split(';');

                CheckModel cb = new()
                {
                    FrameworkName = cbparams[0],
                    Content = cbparams[1],
                    Transcription = cbparams[2],
                    IsEnabled = Convert.ToBoolean(cbparams[3])
                };

                switch (checkModelIndex)
                {
                    case CheckModelIndex.Apps:
                        Apps.Add(cb);
                        break;
                    case CheckModelIndex.Sites:
                        Sites.Add(cb);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void SaveCheckModels()
        {
            string res = string.Empty;
            res += "[Apps]\n";
            foreach (CheckModel item in App.AppsToCheck)
            {
                res += $"{item.FrameworkName};{item.Content};{item.Transcription};{item.IsEnabled}\n";
            }
            res += "[Sites]\n";
            foreach (CheckModel item in App.SitesToCheck)
            {
                res += $"{item.FrameworkName};{item.Content};{item.Transcription};{item.IsEnabled}\n";
            }
            File.WriteAllText("./ReportBlackList.txt", res.Remove(res.Length - 1));
        }
    }

    public enum CheckModelIndex
    {
        Apps = 0,
        Sites = 1
    }
}
