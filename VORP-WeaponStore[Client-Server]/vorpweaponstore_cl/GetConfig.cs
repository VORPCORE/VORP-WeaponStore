using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_cl
{
    public class GetConfig : BaseScript
    {
        public static JObject Config = new JObject();
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();
        public static JArray PlayerWeapons = new JArray();

        public GetConfig()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:SendConfig"] += new Action<string, ExpandoObject>(LoadDefaultConfig);
            EventHandlers[$"{API.GetCurrentResourceName()}:SendWeapons"] += new Action<string>(LoadPlayerWeapons);
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getConfig");
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getWeapons");
        }

        private void LoadDefaultConfig(string dc, ExpandoObject dl)
        {
            Config = JObject.Parse(dc);

            foreach (var l in dl)
            {
                Langs[l.Key] = l.Value.ToString();
            }

            weaponstore_init.InitStores();
        }

        private void LoadPlayerWeapons(string w)
        {
            PlayerWeapons = JArray.Parse(w);
        }

        public static void ForceLoadWeapons()
        {
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getWeapons");
        }
    }
}
