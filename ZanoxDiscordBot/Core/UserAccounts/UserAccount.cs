using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZanoxDiscordBot.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong ID { get; set; }

        public uint Level { get; set; }

        public uint XP { get; set; }
    }
}
