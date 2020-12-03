// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Configuration;
using System.Linq;

namespace AMWE_Administrator
{
    public static class ConfigurationRequest
    {
        public static void WriteValueByKey(string key, string value)
        {
            var appSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var item = Array.Find(appSettings.AppSettings.Settings.OfType<KeyValueConfigurationElement>().ToArray(), x => x.Key == key);
            item.Value = value;
            appSettings.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("appSettings");
        }
        public static string GetValueByKey(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }
    }
}
