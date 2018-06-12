using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using Logger = Rocket.Core.Logging.Logger;

namespace Ranks
{
    public class Ranks : RocketPlugin<RanksConfiguration>
    {
        public RanksUtils Utils;
        public static Ranks Instance;
        private DateTime? lastQuery = DateTime.Now;
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"rank_given", "{1} had their rank changed to {0}!" },
                    {"rank_removed", "{1} had their rank {0} removed!" },
                    {"ranks_cmdsnotenabled", "Commands aren't enabled! Enable them in the config!" },
                    {"ranks_addedmsg", "{1} has been added to {0}!" },
                    {"ranks_removedmsg", "{1} has been removed from {0}!"},
                };
            }
        }

        protected override void Load()
        {
            Instance = this;
            Utils = new RanksUtils();
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;

            Logger.Log("Ranks has been loaded!");
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;

            Logger.Log("Ranks has been unloaded!");
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            CheckRanks(player);
        }

        private void CheckRanks(UnturnedPlayer player)
        {
            Rocket.Core.Permissions.RocketPermissionsManager a = Rocket.Core.R.Instance.GetComponent<Rocket.Core.Permissions.RocketPermissionsManager>();
            List<RocketPermissionsGroup> groups = R.Permissions.GetGroups(player, false).ToList();
            string[] rankInfo = Ranks.Instance.Utils.GetAccountBySteamID(player.CSteamID.ToString());

            // Absolutely necessary commenting and debugging!
            foreach (RocketPermissionsGroup group in groups)
            {
                foreach (Rank rank in Ranks.Instance.Configuration.Instance.RanksList)
                {
                    // If their rank is in XML
                    if (group.Id == rank.RankName)
                    {
                        // If their rank is in DB, then do nothing!
                        if (rankInfo[1] == rank.RankName)
                        {
                            if (Ranks.Instance.Configuration.Instance.RanksDatabase.DebugMode == true)
                            {
                                Logger.Log(player.SteamName + " | " + rankInfo[1] + " | " + group.Id);
                                Logger.Log(player.SteamName + " is in the XML and the DB for " + rank.RankName + "!");
                            }
                        }
                        // If their rank isn't in DB, then remove them from the XML!
                        else
                        {
                            if (Ranks.Instance.Configuration.Instance.RanksDatabase.DebugMode == true)
                            {
                                Logger.Log(player.SteamName + " | " + rankInfo[1] + " | " + group.Id);
                                Logger.Log(player.SteamName + " is in the XML for " + rank.RankName + ", but not the DB, removing!");
                            }
                            RemoveRank(rank.RankName, player);
                        }
                    }
                    // If their rank isn't in XML
                    else
                    {
                        // If they aren't in XML, but they are in the DB, add to the XML!
                        if (rankInfo[1] == rank.RankName)
                        {
                            if (Ranks.Instance.Configuration.Instance.RanksDatabase.DebugMode == true)
                            {
                                Logger.Log(player.SteamName + " | " + rankInfo[1] + " | " + group.Id);
                                Logger.Log(player.SteamName + " isn't in the XML for " + rank.RankName + ", but is in the DB, adding!");
                            }
                            GiveRank(rank.RankName, player);
                        }
                        // If they aren't in XML or the DB, then do nothing!
                        else
                        {
                            if (Ranks.Instance.Configuration.Instance.RanksDatabase.DebugMode == true)
                            {
                                Logger.Log(player.SteamName + " | " + rankInfo[1] + " | " + group.Id);
                                Logger.Log(player.SteamName + " isn't in the XML or the DB for " + rank.RankName + "!");
                            }
                        }
                        if (Ranks.Instance.Configuration.Instance.RanksDatabase.DebugMode == true)
                        {
                            Logger.Log("-----done staff " + rank.RankName + "----");
                        }
                    }
                    if (Ranks.Instance.Configuration.Instance.RanksDatabase.DebugMode == true)
                    {
                        Logger.Log("-----done group " + group.Id + "----");
                    }
                }
            }
        }
        private void GiveRank(string rank, UnturnedPlayer player)
        {
            Rocket.Core.Permissions.RocketPermissionsManager a = Rocket.Core.R.Instance.GetComponent<Rocket.Core.Permissions.RocketPermissionsManager>();
            try
            {
                a.GetGroup(rank);
            }
            catch (Exception)
            {
                Logger.LogWarning("Rank " + rank + " does not exist. Group was not given to " + player.DisplayName + ".");
                return;
            }
            a.AddPlayerToGroup(rank, player);

            if (Ranks.Instance.Configuration.Instance.RanksDatabase.LogChanges == true)
            {
                Logger.Log(Translate("rank_given", rank, player.DisplayName));
            }
        }

        private void RemoveRank(string rank, UnturnedPlayer player)
        {
            Rocket.Core.Permissions.RocketPermissionsManager a = Rocket.Core.R.Instance.GetComponent<Rocket.Core.Permissions.RocketPermissionsManager>();
            try
            {
                a.GetGroup(rank);
            }
            catch (Exception)
            {
                Logger.LogWarning("Rank " + rank + " does not exist. Group was not removed from " + player.DisplayName + ".");
                return;
            }
            a.RemovePlayerFromGroup(rank, player);

            if (Ranks.Instance.Configuration.Instance.RanksDatabase.LogChanges == true)
            {
                Logger.Log(Translate("rank_removed", rank, player.DisplayName));
            }
        }
    }
}