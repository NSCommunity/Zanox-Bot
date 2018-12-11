using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZanoxDiscordBot.Core.UserAccounts;
using System.IO;

namespace ZanoxDiscordBot.Core
{
    public static class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filepPath)
        {
            string json = JsonConvert.SerializeObject(accounts);
            File.WriteAllText(filepPath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
        
    }
}
