using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using System.Collections.Generic;

namespace Ranks
{
    class CommandRemove : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Name
        {
            get
            {
                return "remove";
            }
        }

        public string Help
        {
            get
            {
                return "Remove a player from the ranks database";
            }
        }

        public string Syntax
        {
            get
            {
                return "/remove (Steam64)";
            }
        }

        public List<string> Aliases
        {
            get
            {
                return new List<string>();
            }
        }
        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "ranks.remove"
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (Ranks.Instance.Configuration.Instance.RanksDatabase.EnableCommands == true)
            {
                if (!(command.Length < 1))
                {
                    Ranks.Instance.Utils.RemoveRank(command[0]);
                    UnturnedChat.Say(caller, Ranks.Instance.Translate("ranks_removedmsg"), Color.green);
                }
                else
                {
                    UnturnedChat.Say(caller, Syntax, Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(caller, Ranks.Instance.Translate("ranks_cmdsnotenabled"), Color.red);
            }
        }
    }
}
