﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using Newtonsoft.Json.Linq;

namespace vorpweaponstore_cl
{
    class weaponstore_init : BaseScript
    {
        public static List<int> StoreBlips = new List<int>();
        public static List<int> StorePeds = new List<int>();
        public static List<int> StoreObjects = new List<int>();
        public static Dictionary<int, JToken> StoreAmmoObjects = new Dictionary<int, JToken>();

        public weaponstore_init()
        {
            Tick += onStore;
        }

        public static async Task InitStores()
        {
            await Delay(10000);
            Menus.MainMenu.GetMenu();

            foreach (var store in GetConfig.Config["Stores"])
            {
                string ped = store["NPCModel"].ToString();
                uint HashPed = (uint)API.GetHashKey(ped);
                await LoadModel(HashPed);

                int blipIcon = int.Parse(store["BlipIcon"].ToString());
                float x = float.Parse(store["EnterStore"][0].ToString());
                float y = float.Parse(store["EnterStore"][1].ToString());
                float z = float.Parse(store["EnterStore"][2].ToString());
                float Pedx = float.Parse(store["NPCStore"][0].ToString());
                float Pedy = float.Parse(store["NPCStore"][1].ToString());
                float Pedz = float.Parse(store["NPCStore"][2].ToString());
                float Pedheading = float.Parse(store["NPCStore"][3].ToString());

                int _blip = Function.Call<int>((Hash)0x554D9D53F696D002, 1664425300, x, y, z);
                Function.Call((Hash)0x74F74D3207ED525C, _blip, blipIcon, 1);
                Function.Call((Hash)0x9CB1A1623062F402, _blip, store["name"].ToString());
                StoreBlips.Add(_blip);

                int _PedShop = API.CreatePed(HashPed, Pedx, Pedy, Pedz, Pedheading, false, true, true, true);
                Function.Call((Hash)0x283978A15512B2FE, _PedShop, true);
                StorePeds.Add(_PedShop);
                API.SetEntityNoCollisionEntity(API.PlayerPedId(), _PedShop, false);
                API.SetEntityCanBeDamaged(_PedShop, false);
                API.SetEntityInvincible(_PedShop, true);
                await Delay(1000);
                API.FreezeEntityPosition(_PedShop, true);
                API.SetBlockingOfNonTemporaryEvents(_PedShop, true);
                API.SetModelAsNoLongerNeeded(HashPed);
                await Delay(100);

                foreach (var ammoBox in store["SpawnAmmoInStores"]) 
                {
                    float _x = float.Parse(ammoBox[0].ToString());
                    float _y = float.Parse(ammoBox[1].ToString());
                    float _z = float.Parse(ammoBox[2].ToString());
                    float _h = float.Parse(ammoBox[3].ToString());
                    uint hashModel = (uint)API.GetHashKey(ammoBox[4].ToString());
                    await LoadModel(hashModel);
                    int _ammoObject = API.CreateObject(hashModel, _x, _y, _z, false, true, false, true, true);
                    API.SetEntityHeading(_ammoObject, _h);
                    Function.Call((Hash)0x7DFB49BCDB73089A, _ammoObject, true);
                    Function.Call((Hash)0x8A7391690F5AFD81, _ammoObject, true);
                    API.FreezeEntityPosition(_ammoObject, true);
                    StoreObjects.Add(_ammoObject);
                    StoreAmmoObjects.Add(_ammoObject, ammoBox);
                }

            }

            //Load Models (Prevent duplicated models in table of shop)
            foreach (JToken weapons in GetConfig.Config["Weapons"])
            {
                uint idObject = (uint)API.GetHashKey(weapons["WeaponModel"].ToString());

                foreach (JObject c in weapons["CompsHash"].Children<JObject>())
                {
                    foreach (JProperty comp in c.Properties())
                    {
                        await LoadModel((uint)API.GetHashKey(comp.Name));
                    }
                }

                await LoadModel(idObject);
            }

        }

        private async Task onStore()
        {
            if (StorePeds.Count() == 0) { return; }

            int pid = API.PlayerPedId();
            Vector3 pCoords = API.GetEntityCoords(pid, true, true);

            for (int i = 0; i < GetConfig.Config["Stores"].Count(); i++)
            {
                float x = float.Parse(GetConfig.Config["Stores"][i]["EnterStore"][0].ToString());
                float y = float.Parse(GetConfig.Config["Stores"][i]["EnterStore"][1].ToString());
                float z = float.Parse(GetConfig.Config["Stores"][i]["EnterStore"][2].ToString());
                float radius = float.Parse(GetConfig.Config["Stores"][i]["EnterStore"][3].ToString());

                if (API.GetDistanceBetweenCoords(pCoords.X, pCoords.Y, pCoords.Z, x, y, z, false) <= radius)
                {
                    await DrawTxt(GetConfig.Langs["PressToOpen"], 0.5f, 0.9f, 0.7f, 0.7f, 255, 255, 255, 255, true, true);
                    if (API.IsControlJustPressed(2, 0xD9D0E1C0))
                    {
                        await ActionStore.EnterBuyStore(i);
                    }
                }

            }

        }

        public async Task DrawTxt(string text, float x, float y, float fontscale, float fontsize, int r, int g, int b, int alpha, bool textcentred, bool shadow)
        {
            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", text);
            Function.Call(Hash.SET_TEXT_SCALE, fontscale, fontsize);
            Function.Call(Hash._SET_TEXT_COLOR, r, g, b, alpha);
            Function.Call(Hash.SET_TEXT_CENTRE, textcentred);
            if (shadow) { Function.Call(Hash.SET_TEXT_DROPSHADOW, 1, 0, 0, 255); }
            Function.Call(Hash.SET_TEXT_FONT_FOR_CURRENT_COMMAND, 1);
            Function.Call(Hash._DISPLAY_TEXT, str, x, y);
        }

        public static async Task<bool> LoadModel(uint hash)
        {
            if (Function.Call<bool>(Hash.IS_MODEL_VALID, hash))
            {
                Function.Call(Hash.REQUEST_MODEL, hash);
                while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, hash))
                {
                    await Delay(100);
                }
                return true;
            }
            else
            {
                Debug.WriteLine($"Model {hash} is not valid!");
                return false;
            }
        }
    }
}
