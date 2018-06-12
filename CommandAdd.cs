using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using UnityEngine;

namespace Ranks
{
    class CommandAdd : IRocketCommand
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
                return "add";
            }
        }

        public string Help
        {
            get
            {
                return "Add a player to the ranks database";
            }
        }

        public string Syntax
        {
            get
            {
                return "/add (Steam64) (Rank) (Name - Optional)";
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
                    "ranks.add"
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (Ranks.Instance.Configuration.Instance.RanksDatabase.EnableCommands == true)
            {
                if (!(command.Length < 1))
                {
                    if (command.Length > 2)
                    {
                        Ranks.Instance.Utils.AddRank(command[0], command[2], command[1]);
						UnturnedChat.Say(caller, Ranks.Instance.Translate("ranks_addedmsg"), Color.green);
                    }
                    else
                    {
                        Ranks.Instance.Utils.AddRank(command[0], "0", command[1]);
						UnturnedChat.Say(caller, Ranks.Instance.Translate("ranks_addedmsg"), Color.green);
                    }
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
