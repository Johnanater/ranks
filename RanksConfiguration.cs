using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ranks
{
    public class RanksConfiguration : IRocketPluginConfiguration
    {
        public classDatabase RanksDatabase;

        [XmlArrayItem(ElementName = "RanksList")]
        public List<Rank> RanksList;

        public void LoadDefaults()
        {
            RanksDatabase = new classDatabase()
            {
                DatabaseAddress = "localhost",
                DatabaseUsername = "unturned",
                DatabasePassword = "password",
                DatabaseName = "unturned",
                DatabaseTableName = "ranks",
                DatabasePort = 3306,
                EnableCommands = true,
                LogChanges = false,
                DebugMode = false,
            };
            RanksList = new List<Rank>()
            {
                new Rank { RankName = "Admin" },
                new Rank { RankName = "Mod" },
            };
        }
    }

    public class classDatabase
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int DatabasePort;
        public bool EnableCommands;
        public bool LogChanges;
        public bool DebugMode;
    }
    public class Rank
    {
        public Rank() { }
        public string RankName;
    }
}