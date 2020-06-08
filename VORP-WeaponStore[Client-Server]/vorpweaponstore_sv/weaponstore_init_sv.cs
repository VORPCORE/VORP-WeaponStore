using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_sv
{
    public class weaponstore_init_sv : BaseScript
    {
        public weaponstore_init_sv()
        {
            EventHandlers["vorpweaponstore:BuyWeapon"] += new Action<Player, int>(buyItems);
        }

        private void buyItems([FromSource]Player source, int index)
        {
            int _source = int.Parse(source.Handle);

            string sid = "steam:" + source.Identifiers["steam"];

            string weaponName = LoadConfig.Config["Weapons"][index]["Name"].ToObject<string>();
            string weaponHash = LoadConfig.Config["Weapons"][index]["HashName"].ToObject<string>();
            double cost = LoadConfig.Config["Weapons"][index]["Price"].ToObject<double>();

            TriggerEvent("vorp:getCharacter", _source, new Action<dynamic>((user) =>
            {
                double money = user.money;
                if (cost <= money)
                {

                    TriggerEvent("vorp:removeMoney", _source, 0, cost);
                    TriggerEvent("vorpCore:registerWeapon", _source, weaponHash);
                    source.TriggerEvent("vorp:Tip", string.Format(LoadConfig.Langs["YouBoughtWeapon"], weaponName, cost.ToString()), 4000);
                }
                else
                {
                    source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoMoney"], 4000);
                }

            }));
        }
    }
}
