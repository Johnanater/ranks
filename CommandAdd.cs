using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Ranks
{
    class CommandAdd : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "add";

        public string Help => "Add a player to the ranks database";

        public string Syntax => "/add <player/steam64> <rank>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "ranks.add" };

        public async void Execute(IRocketPlayer caller, string[] command)
        {
            if (!Main.Config.EnableCommands)
            {
                UnturnedChat.Say(caller, Main.Instance.Translate("cmds_not_enabled"), Color.red);
                return;
            }

            if (command.Length < 2)
            {
                UnturnedChat.Say(caller, Syntax, Color.red);
                return;
            }

            string steam64 = "";
            string name = "";
            string rank = command[1];

            // Check if command[0] is a player's name
            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(command[0]);

            if (targetPlayer?.Player)
            {
                steam64 = targetPlayer.Id;
                name = targetPlayer.DisplayName;
            }
            else
            {
                steam64 = command[0];
            }

            await Main.Instance.AddToRank(steam64, name, rank);
			UnturnedChat.Say(caller, Main.Instance.Translate("has_been_added_to", steam64, rank), Color.green);
        }
    }
}
