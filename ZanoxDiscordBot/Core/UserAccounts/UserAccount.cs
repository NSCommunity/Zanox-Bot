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

        public uint XP { get; set; }

        public uint Rep { get; set; }

        public uint Money { get; set; }

<<<<<<< HEAD
        public uint NumberOfWarnings { get; set; }

=======
>>>>>>> 8ef9413b8526888c5aea595236ee1e1c1412c848
        public int GCooldown { get; set; }

        public int repCooldown { get; set; }

        public uint LevelNumber
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }
    }
}
