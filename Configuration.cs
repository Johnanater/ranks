using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

namespace Ranks
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public int DatabasePort;
        public string DatabaseName;
        public string DatabaseTableName;
        public string DatabaseUsername;
        public string DatabasePassword;

        public bool UseWhitelist;
        public bool EnableCommands;
        public bool LogChanges;

        [XmlArray(ElementName = "Ranks")]
        [XmlArrayItem(ElementName = "Rank")]
        public List<string> Ranks;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "ranks";
            DatabasePort = 3306;
            UseWhitelist = true;
            EnableCommands = true;
            LogChanges = true;

            Ranks = new List<string>()
            {
                "Admin",
                "Mod",
                "VIP"
            };
        }
    }

}