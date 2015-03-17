#if REMICONTROL

using System;
using System.Text.RegularExpressions;
using TsdLib.Configuration;
using TsdLib.Configuration.Utilities;
using TsdLib.Configuration.Connections;
using TsdLib.Measurements;
using TsdLib.TestSystem;


namespace TSD_Slider.Configuration.Connections
{
    class DatabaseConfigConnection : IConfigConnection
    {
        private readonly string _appVersionFilter;

        public DatabaseConfigConnection(string appVersionFilter = @"\d+\.\d+")
        {
            _appVersionFilter = appVersionFilter;
        }

        public bool WriteString(string testSystemName, Version testSystemVersion, OperatingMode testSystemMode, Type configType, string data)
        {
            string baseTypeName = ConfigExtensions.GetBaseTypeName(configType);
            Match match = Regex.Match(testSystemVersion.ToString(), _appVersionFilter);
            string appVersion = match.Success ? match.Value : testSystemVersion.ToString();
            return DBControl.DAL.Config.SaveConfig(testSystemName, appVersion, testSystemMode.ToString(), baseTypeName, data);
        }

        public bool TryReadString(string testSystemName, Version testSystemVersion, OperatingMode testSystemMode, Type configType, out string data)
        {
            string baseTypeName = ConfigExtensions.GetBaseTypeName(configType);
            Match match = Regex.Match(testSystemVersion.ToString(), _appVersionFilter);
            string appVersion = match.Success ? match.Value : testSystemVersion.ToString();
            data = DBControl.DAL.Config.GetConfig(testSystemName, appVersion, testSystemMode.ToString(), baseTypeName);
            if (string.IsNullOrEmpty(data))
                return false;
            

            return true;
        }
    }
}

#endif