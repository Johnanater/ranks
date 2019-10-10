using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Ranks
{
    class CommandRemove : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "remove";

        public string Help => "Remove a player from the Ranks database";

        public string Syntax => "/remove <steam64> <rank>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "ranks.remove" };

        public async void Execute(IRocketPlayer caller, string[] command)
        {
            if (!Main.Config.EnableCommands)
            {
                UnturnedChat.Say(caller, Main.Instance.Translate("cmds_not_enabled"), Color.red);
                return;
            }

            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax, Color.red);
                return;
            }

            string steam64 = "";

            // Check if command[0] is a player's name
            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(command[0]);

            if (targetPlayer?.Player)
                steam64 = targetPlayer.Id;
            else
                steam64 = command[0];

            if (!await Main.MySQLUtils.CheckExists(steam64))
            {
                UnturnedChat.Say(Main.Instance.Translate("not_in_database", steam64));
                return;
            }

            // Get rid of all ranks
            if (command.Length < 2)
            {
                await Main.MySQLUtils.RemoveRanks(steam64);
                UnturnedChat.Say(caller, Main.Instance.Translate("removed_from_database", steam64));
            }

            // Specific
            else
            {
                string rank = command[1];

                await Main.Instance.RemoveFromRank(steam64, rank);
                UnturnedChat.Say(caller, Main.Instance.Translate("has_been_removed_from", steam64, rank), Color.green);
            }
        }
    }
}
