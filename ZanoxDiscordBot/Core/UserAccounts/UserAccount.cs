﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZanoxDiscordBot.Core.UserAccounts
{
    public class UserAccount
    {
        //User Settings

        public ulong ID { get; set; }

        public uint XP { get; set; }

        public uint Rep { get; set; }

        public uint Money { get; set; }

        public uint NumberOfWarnings { get; set; }

        public int GCooldown { get; set; }

        public int repCooldown { get; set; }

        public int GBlock { get; set; }

        public string prefix { get; set; }

        public string pastebinDev { get; set; }

        public uint LevelNumber
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }

        //Server Settings

        public ulong DefaultChannelID { get; set; }

        public int levelingAlert { get; set; }
    }
}
