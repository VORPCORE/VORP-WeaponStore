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

        static List<string> compsSuffix = new List<string>() {
            "barrel1",
            "barrel2",
            "barrel01",
            "barrel02",
            "grip1",
            "grip2",
            "grip3",
            "grip4",
            "grip5",
            "sight1",
            "sight2",
            "clip",
            "clip1",
            "clip2",
            "clip3",
            "wrap1",
            "wrap2",
            "wrap3",
            "mag1",
            "mag2",
            "mag3"
        };

        public static async Task CreateObjectOnTable(int index, string list, int LastObjectStore)
        {
            await Delay(10);
            DeleteObject(ref LastObjectStore);
            float objectX = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][0].ToString());
            float objectY = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][1].ToString());
            float objectZ = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][2].ToString());
            float objectH = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][3].ToString());
            uint idObject = (uint)GetHashKey(GetConfig.Config[list][index]["WeaponModel"].ToString());


            foreach (JObject c in GetConfig.Config[list][index]["CompsHash"].Children<JObject>())
            {
                foreach (JProperty comp in c.Properties())
                {
                    Debug.WriteLine(comp.Name);
                    weaponstore_init.LoadModel((uint)GetHashKey(comp.Name));
                }
            }

            await weaponstore_init.LoadModel(idObject);
            //ObjectStore = CreateObject(idObject, objectX, objectY, objectZ, false, true, true, true, true);
            //SetModelAsNoLongerNeeded(idObject);
            ObjectStore = Function.Call<int>((Hash)0x9888652B8BA77F73, GetHashKey(GetConfig.Config[list][index]["HashName"].ToString()), 0, objectX, objectY, objectZ, true, 1.0);
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
    }
}
