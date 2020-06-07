using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using MenuAPI;
using CitizenFX.Core.Native;

namespace vorpweaponstore_cl
{
    public class ActionStore : BaseScript
    {
        private static int ObjectStore;
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
            NetworkSetInSpectatorMode(true, PlayerPedId());
            FreezeEntityPosition(PlayerPedId(), true);
            SetEntityVisible(PlayerPedId(), false);

            CamStore = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", Camerax, Cameray, Cameraz, CameraRotx, CameraRoty, CameraRotz, 50.00f, false, 0);
            SetCamActive(CamStore, true);
            RenderScriptCams(true, true, 500, true, true, 0);

            MenuController.MainMenu.MenuTitle = GetConfig.Config["Stores"][storeId]["name"].ToString();

            MenuController.MainMenu.OpenMenu();
        }

        static List<string> comps = new List<string>() {
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
            "wrap1",
            "mag1",
            "mag2",
            "mag3",
            "w_repeater_pumpaction01_barrel1",
            "w_repeater_pumpaction01_barrel2",
            "w_repeater_pumpaction01_barrel01",
            "w_repeater_pumpaction01_barrel02",
            "w_repeater_pumpaction01_grip1",
            "w_repeater_pumpaction01_grip2",
            "w_repeater_pumpaction01_sight1",
            "w_repeater_pumpaction01_clip",
            "w_repeater_pumpaction01_clip1",
            "w_repeater_pumpaction01_wrap1",
            "w_repeater_pumpaction01_mag1",
            "w_repeater_pumpaction01_mag2",
            "w_repeater_pumpaction01_rear01",
            "w_rifle_scopeinner01",
            "w_rifle_scope04",
            "w_rifle_scope03",
            "w_rifle_scope02",
            "w_rifle_cs_strap01",
            "w_sight_rear02",
            "w_sight_rear01",
            "w_repeater_cloth_strap01",
            "w_repeater_strap01",
            "w_rifle_boltaction03_grip1"
        };

        public static async Task CreateObjectOnTable(int index, string list)
        {

            DeleteObject(ref ObjectStore);
            float objectX = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][0].ToString());
            float objectY = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][1].ToString());
            float objectZ = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][2].ToString());
            float objectH = float.Parse(GetConfig.Config["Stores"][LaststoreId]["SpawnObjectStore"][3].ToString());
            uint idObject = (uint)GetHashKey(GetConfig.Config[list][index]["HashName"].ToString());
            foreach(string c in comps)
            {
                await weaponstore_init.LoadModel((uint)GetHashKey(c));
            }
            await weaponstore_init.LoadModel(idObject);
            //ObjectStore = CreateObject(idObject, objectX, objectY, objectZ, false, true, true, true, true);
            SetModelAsNoLongerNeeded(idObject);
            ObjectStore = Function.Call<int>((Hash)0x9888652B8BA77F73, idObject, 0, objectX, objectY, objectZ, false, 1.0);
        }

        public static async Task ExitBuyStore()
        {
            await Delay(100);
            if (!MenuController.IsAnyMenuOpen())
            {
                TriggerEvent("vorp:setInstancePlayer", false);
                NetworkSetInSpectatorMode(false, PlayerPedId());
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
