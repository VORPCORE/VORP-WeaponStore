using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace vorpweaponstore_cl
{
    class PickupAmmo : BaseScript
    {
        private int PickPrompt;
        public static bool UIActive = false;

        public PickupAmmo()
        {
            EventHandlers[$"vorp_weaponstore:useAmmoItem"] += new Action<string>(OnUseAmmo);
            Tick += OnView;
            SetupPickPrompt();
        }

        public void OnUseAmmo(string ammoName)
        {
            uint weaponHash = 0;
            if (GetCurrentPedWeapon(PlayerPedId(), ref weaponHash, false, 0, false))
            {
                string weaponName = Function.Call<string>((Hash)0x89CF5FF3D363311E, weaponHash);
                if (weaponName.Contains("UNARMED")) 
                {
                    TriggerEvent("vorp:Tip", GetConfig.Langs["NeedWeaponOnHand"], 3000);
                }
                else
                {

                    JToken wpc = GetConfig.Config["Weapons"].FirstOrDefault(x => x["HashName"].ToString().Contains(weaponName));
                    Dictionary<string, double> ammoType = new Dictionary<string, double>();

                    string ammoGives = GetConfig.Config["AmmoGivesOnUse"][0][ammoName].ToString();

                    foreach (JObject ammoc in wpc["AmmoHash"].Children<JObject>())
                    {
                        foreach (JProperty ammo in ammoc.Properties())
                        {
                            ammoType.Add(ammo.Name, ammo.Value.ToObject<double>());
                        }
                    }

                    if (ammoType.ContainsKey(ammoGives))
                    {
                        int ammoActually = GetPedAmmoByType(PlayerPedId(), GetHashKey(ammoGives));
                        int maxAmmo = GetConfig.Config["AmmoLimit"][0][ammoGives].ToObject<int>();
                        int ammoNeeded = (maxAmmo - ammoActually);
                        if (ammoNeeded >= 10)
                        {
                            int newAmmo = ammoActually + 10;
                            SetPedAmmoByType(PlayerPedId(), GetHashKey(ammoGives), newAmmo);
                            TriggerServerEvent("vorpweaponstore:DeleteAmmoBox", ammoName);
                        }
                        else
                        {
                            TriggerEvent("vorp:Tip", GetConfig.Langs["AmmoIsFull"], 3000);
                        }
                    }
                    else
                    {
                        TriggerEvent("vorp:Tip", GetConfig.Langs["IncorretWeapon"], 3000);
                    }


                }

            }
        } 

        [Tick]
        public async Task OnView()
        {
            int entity = 0;
            bool hit = false;
            Vector3 endCoord = new Vector3();
            Vector3 surfaceNormal = new Vector3();
            Vector3 camCoords = GetGameplayCamCoord();
            Vector3 sourceCoords = GetCoordsFromCam(10.0F);
            int rayHandle = StartShapeTestRay(camCoords.X, camCoords.Y, camCoords.Z, sourceCoords.X, sourceCoords.Y, sourceCoords.Z, -1, PlayerPedId(), 0);
            GetShapeTestResult(rayHandle, ref hit, ref endCoord, ref surfaceNormal, ref entity);
            //Function.Call((Hash)0x2A32FAA57B937173, -1795314153, endCoord.X, endCoord.Y, endCoord.Z, 0, 0, 0, 0, 0, 0, 0.2f, 0.2f, 10.0f, 93, 0, 0, 155, 0, 0, 2, 0, 0, 0, 0);


            if (weaponstore_init.StoreAmmoObjects.ContainsKey(entity))
            {
                if (!UIActive)
                {
                    double costUnit = weaponstore_init.StoreAmmoObjects[entity][6].ToObject<double>();
                    double costTotal = costUnit * 10;
                    string name = string.Format(GetConfig.Langs[weaponstore_init.StoreAmmoObjects[entity][5].ToString()], costTotal.ToString());
                    long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", name);
                    Function.Call((Hash)0x5DD02A8318420DD7, PickPrompt, str);
                    Function.Call((Hash)0x8A0FB4D03A630D21, PickPrompt, true);
                    Function.Call((Hash)0x71215ACCFDE075EE, PickPrompt, true);
                    UIActive = true;
                }

                if (Function.Call<bool>((Hash)0xE0F65F0640EF0617, PickPrompt))
                {
                    Debug.WriteLine("Presionado");
                    double costUnit = weaponstore_init.StoreAmmoObjects[entity][6].ToObject<double>();
                    double costTotal = costUnit * 10;
                    Function.Call((Hash)0x8A0FB4D03A630D21, PickPrompt, false);
                    Function.Call((Hash)0x71215ACCFDE075EE, PickPrompt, false);
                    await PlayAnims();
                    UIActive = false;
                    TriggerServerEvent("vorpweaponstore:BuyAmmoItem", weaponstore_init.StoreAmmoObjects[entity][5].ToString(), costTotal);
                    Function.Call((Hash)0x502EC17B1BED4BFA, API.PlayerPedId(), entity);
                    await Delay(5000); 
                }
            }
            else
            {
                Function.Call((Hash)0x8A0FB4D03A630D21, PickPrompt, false);
                Function.Call((Hash)0x71215ACCFDE075EE, PickPrompt, false);
                UIActive = false;
            }

            if (IsControlJustPressed(0, 0xCEE12B50))
            {
                Debug.WriteLine(endCoord.ToString());
            }
        }

        public static async Task PlayAnims()
        {
            Function.Call((Hash)0x67C540AA08E4A6F5, "CHECKPOINT_PERFECT", "HUD_MINI_GAME_SOUNDSET", true, 1);
        }

        public static Vector3 GetCoordsFromCam(float distance)
        {
            Vector3 rot = API.GetGameplayCamRot(2);
            Vector3 coord = API.GetGameplayCamCoord();

            float tZ = rot.Z * 0.0174532924F;
            float tX = rot.X * 0.0174532924F;

            float num = (float)Math.Abs(Math.Cos(tX));

            float newCoordX = coord.X + (float)(-Math.Sin(tZ)) * (num + distance);
            float newCoordY = coord.Y + (float)(Math.Cos(tZ)) * (num + distance);
            float newCoordZ = coord.Z + (float)(Math.Sin(tX)) * (num + distance);

            return new Vector3(newCoordX, newCoordY, newCoordZ);
        }

        public void SetupPickPrompt()
        {
            PickPrompt = Function.Call<int>((Hash)0x04F97DE45A519419);
            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", "Comprar");
            Function.Call((Hash)0x5DD02A8318420DD7, PickPrompt, str);
            Function.Call((Hash)0xB5352B7494A08258, PickPrompt, 0xF84FA74F);
            Function.Call((Hash)0x8A0FB4D03A630D21, PickPrompt, false);
            Function.Call((Hash)0x71215ACCFDE075EE, PickPrompt, false);
            Function.Call((Hash)0x94073D5CA3F16B7B, PickPrompt, true);
            Function.Call((Hash)0xF7AA2696A22AD8B9, PickPrompt);
        }
    }
}
