using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZanoxDiscordBot.Core.UserAccounts
{
    public static class UserAccounts
    {
        private static List<UserAccount> accounts;

        private static string accountsFile = "Resources/accounts.json";

        static UserAccounts()
        {
            if(DataStorage.SaveExists(accountsFile))
            {
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        public static UserAccount GetOrCreateAccount(ulong id)
        {
            var resault = from a in accounts
                          where a.ID == id
                          select a;

            var account = resault.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account; 
        }

        public static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                ID = id,
                XP = 0,
                GCooldown = 0,
                DefaultChannelID = 522795148598575135,
                Rep = 0,
                repCooldown = 0,
                Money = 0,
                NumberOfWarnings = 0,
                levelingAlert = 1,
                GBlock = 0,
                prefix = "z!",
                pastebinDev = "undefined"
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
