using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using MenuAPI;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;

namespace vorpweaponstore_cl
{
    public class ActionStore : BaseScript
    {

        public ActionStore()
        {

        }

        public static int ObjectStore;
        private static int CamStore;
        private static int LaststoreId;
        public static async Task EnterBuyStore(int storeId)
        {
            LaststoreId = storeId;
            float Camerax = float.Parse(GetConfig.Config["Stores"][storeId]["CameraMain"][0].ToString());
            float Cameray = float.Parse(GetConfig.Config["Stores"][storeId]["CameraMain"][1].ToString());
            float Cameraz = float.Parse(GetConfig.Config["Stores"][storeId]["CameraMain"][2].ToString());
            float CameraRotx = float.Parse(GetConfig.Config["Stores"][storeId]["CameraMain"][3].ToString());
            float CameraRoty = float.Parse(GetConfig.Config["Stores"][storeId]["CameraMain"][4].ToString());
            float CameraRotz = float.Parse(GetConfig.Config["Stores"][storeId]["CameraMain"][5].ToString());

            TriggerEvent("vorp:setInstancePlayer", true);
            //NetworkSetInSpectatorMode(true, PlayerPedId());
            FreezeEntityPosition(PlayerPedId(), true);
            SetEntityVisible(PlayerPedId(), false);

            CamStore = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", Camerax, Cameray, Cameraz, CameraRotx, CameraRoty, CameraRotz, 50.00f, false, 0);
            SetCamActive(CamStore, true);
            RenderScriptCams(true, true, 500, true, true, 0);

            MenuController.MainMenu.MenuTitle = GetConfig.Config["Stores"][storeId]["name"].ToString();

            MenuController.MainMenu.OpenMenu();
        }

        public static async Task CreateObjectOnTable(int index, string list, int LastObjectStore)
        {
            await Delay(10);
            DeleteObject(ref LastObjectStore);
            float objectX = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][0].ToString());
            float objectY = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][1].ToString());
            float objectZ = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][2].ToString());
            float objectH = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][3].ToString());

            if (list.Contains("Manage"))
            {
                var myweap = GetConfig.PlayerWeapons.ElementAt(index);
                var wp = GetConfig.Config["Weapons"].FirstOrDefault(x => x["HashName"].ToString().Contains(myweap["name"].ToString()));
                uint idObject = (uint)GetHashKey(wp["WeaponModel"].ToString());

                foreach (JObject c in wp["CompsHash"].Children<JObject>())
                {
                    foreach (JProperty comp in c.Properties())
                    {
                        weaponstore_init.LoadModel((uint)GetHashKey(comp.Name));
                    }
                }

                await weaponstore_init.LoadModel(idObject);
                //ObjectStore = CreateObject(idObject, objectX, objectY, objectZ, false, true, true, true, true);
                //SetModelAsNoLongerNeeded(idObject);
                ObjectStore = Function.Call<int>((Hash)0x9888652B8BA77F73, GetHashKey(wp["HashName"].ToString()), 0, objectX, objectY, objectZ, true, 1.0);
            }
            else
            {
                uint idObject = (uint)GetHashKey(GetConfig.Config[list][index]["WeaponModel"].ToString());

                foreach (JObject c in GetConfig.Config[list][index]["CompsHash"].Children<JObject>())
                {
                    foreach (JProperty comp in c.Properties())
                    {
                        weaponstore_init.LoadModel((uint)GetHashKey(comp.Name));
                    }
                }

                await weaponstore_init.LoadModel(idObject);
                //ObjectStore = CreateObject(idObject, objectX, objectY, objectZ, false, true, true, true, true);
                //SetModelAsNoLongerNeeded(idObject);
                ObjectStore = Function.Call<int>((Hash)0x9888652B8BA77F73, GetHashKey(GetConfig.Config[list][index]["HashName"].ToString()), 0, objectX, objectY, objectZ, true, 1.0);
            }
         
        }

        public static async Task ExitBuyStore()
        {
            await Delay(100);
            if (!MenuController.IsAnyMenuOpen())
            {
                TriggerEvent("vorp:setInstancePlayer", false);
                //NetworkSetInSpectatorMode(false, PlayerPedId());
                FreezeEntityPosition(PlayerPedId(), false);
                SetEntityVisible(PlayerPedId(), true);
                SetCamActive(CamStore, false);
                RenderScriptCams(false, true, 1000, true, true, 0);
                DestroyCam(CamStore, true);

                DeleteObject(ref ObjectStore);
            }

        }


        public static async Task BuyWeaponStore(int index)
        {
            TriggerServerEvent("vorpweaponstore:BuyWeapon", index);
        }

        public static async Task RestockWeaponAmmo(int weaponId, double cost, string typeAmmo, int quantity)
        {
            TriggerServerEvent("vorpweaponstore:RestockAmmo", weaponId, cost, typeAmmo, quantity);
        }
    }
}
