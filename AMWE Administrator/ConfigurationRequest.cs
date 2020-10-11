using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
