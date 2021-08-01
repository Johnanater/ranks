using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Logger = Rocket.Core.Logging.Logger;

namespace Ranks
{
    public class Main : RocketPlugin<Configuration>
    {

        public static Main Instance;
        public static Configuration Config;
        public static MySQLUtils MySQLUtils;
        public const string Version = "1.1.3";

        protected override void Load()
        {
            Instance = this;
            Config = Configuration.Instance;
            MySQLUtils = new MySQLUtils();
            
            Provider.onServerConnected += OnServerConnected;

            Logger.Log($"Ranks by Johnanater, version: {Version}");
        }
        
        protected override void Unload()
        {
            Provider.onServerConnected -= OnServerConnected;
        }
        
        private void OnServerConnected(CSteamID steamId)
        {
            var untPlayer = UnturnedPlayer.FromCSteamID(steamId);
            Task.Run(() => CheckForRanks(untPlayer));
        }

        // Check players ranks to be added or removed
        public async Task CheckForRanks(UnturnedPlayer untPlayer)
        {
            List<RocketPermissionsGroup> playerGroups = R.Permissions.GetGroups(untPlayer, false);
            List<string> playerRanks = await MySQLUtils.GetRanks(untPlayer.Id);


            // Check for groups to add
            foreach (string playerRank in playerRanks)
            {
                // Check if player already has that rank
                if (playerGroups.Any(pg => pg.Id.Equals(playerRank)) && IsWhitelisted(playerRank))
                    continue; 

                // Else, give them the rank
                if (IsWhitelisted(playerRank))
                    AddToGroup(untPlayer, playerRank);
            }

            // Check for groups to remove
            foreach (RocketPermissionsGroup playerGroup in playerGroups)
            {
                // If not in MySQL, remove
                if (!playerRanks.Contains(playerGroup.Id) && IsWhitelisted(playerGroup.Id))
                    RemoveFromGroup(untPlayer, playerGroup.Id);
            }
        }

        public void AddToGroup(UnturnedPlayer untPlayer, string group)
        {
            RocketPermissionsManager rocketPerms = R.Instance.GetComponent<RocketPermissionsManager>();

            if(GroupExists(group))
            {
                rocketPerms.AddPlayerToGroup(group, untPlayer);

                if (Config.LogChanges)
                    Logger.Log(Translate("log_rank_added_to", untPlayer.DisplayName, untPlayer.Id, group));
            }
        }

        public void RemoveFromGroup(UnturnedPlayer untPlayer, string group)
        {
            RocketPermissionsManager rocketPerms = R.Instance.GetComponent<RocketPermissionsManager>();

            if (GroupExists(group))
            {
                rocketPerms.RemovePlayerFromGroup(group, untPlayer);

                if (Config.LogChanges)
                    Logger.Log(Translate("log_rank_removed_from", untPlayer.DisplayName, untPlayer.Id, group));
            }
        }

        public bool GroupExists(string group)
        {
            RocketPermissionsManager rocketPerms = R.Instance.GetComponent<RocketPermissionsManager>();
            RocketPermissionsGroup rpGroup = rocketPerms.GetGroup(group);

            if (rpGroup == null)
                return false;
            return true;
        }

        public bool IsWhitelisted(string group)
        {
            if (!Config.UseWhitelist)
                return true;

            if (Config.Ranks.Contains(group))
                return true;
            return false;
        }

        public async Task AddToRank(string steam64, string name, string addedRank)
        {
            List<string> oldRanks = await MySQLUtils.GetRanks(steam64);

            oldRanks.Add(addedRank.Trim());

            // Join and remove the annoying leading ,
            string newRanks = string.Join(",", oldRanks).TrimStart(',');

            if (await MySQLUtils.CheckExists(steam64))
            {
                await MySQLUtils.UpdateRanks(steam64, newRanks);
            }
            else
            {
                await MySQLUtils.AddRanks(steam64, name, newRanks);
            }
        }

        public async Task RemoveFromRank(string steam64, string removedRank)
        {
            List<string> oldRanks = await MySQLUtils.GetRanks(steam64);

            oldRanks.Remove(removedRank);

            // Join and remove the annoying leading ,
            string newRanks = string.Join(",", oldRanks).TrimStart(',');

            await MySQLUtils.UpdateRanks(steam64, newRanks);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            // Messages
            {"has_been_added_to", "{0} has been added to rank {1}!"},
            {"has_been_removed_from", "{0} has been removed from rank {1}!"},
            {"removed_from_database", "{0} has been removed from the Ranks database!"},

            // Warnings / Errors
            {"cmds_not_enabled", "Commands aren't enabled! Enable them in the config!"},
            {"not_in_database", "{0} is not in the Ranks database!"},

            // Change messages
            {"log_rank_added_to", "{0} ({1}) has been added to {2}!"},
            {"log_rank_removed_from", "{0} ({1}) has been removed from {2}!"}
        };
    }
}