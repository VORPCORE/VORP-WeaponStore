using CitizenFX.Core;
using MenuAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_cl.Menus
{
    class ManageAmmoMenu
    {
        private static Menu manageAmmoMenu = new Menu(GetConfig.Langs["MenuManageAmmo"], GetConfig.Langs["MenuManageAmmoDesc"]);
        private static bool setupDone = false;

        public static int indexAmmo;

        private static void SetupMenu()
        {
            if (setupDone) return;
            setupDone = true;
            MenuController.AddMenu(manageAmmoMenu);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)0;

            ////SubMenu Ammo Restock
            //MenuController.AddSubmenu(manageAmmoMenu, ManageAmmoMenu.GetMenu());
            //MenuItem restockAmmo = new MenuItem(GetConfig.Langs["MenuManageAmmo"], "")
            //{
            //    LeftIcon = MenuItem.Icon.ARROW_RIGHT
            //};

            //manageAmmoMenu.AddMenuItem(restockAmmo);
            //MenuController.BindMenuItem(manageAmmoMenu, ManageAmmoMenu.GetMenu(), restockAmmo);
            ////end

            manageAmmoMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                var myWp = GetConfig.PlayerWeapons.ElementAt(ManageWeaponsMenu.indexItem);
                var wpc = GetConfig.Config["Weapons"].FirstOrDefault(x => x["HashName"].ToString().Contains(myWp["name"].ToString()));
                Dictionary<string, double> ammoType = new Dictionary<string, double>();

                foreach (JObject ammoc in wpc["AmmoHash"].Children<JObject>())
                {
                    foreach (JProperty ammo in ammoc.Properties())
                    {
                        ammoType.Add(ammo.Name, ammo.Value.ToObject<double>());
                    }
                }

                string AmmoType = ammoType.ElementAt(_index).Key;

                if (myWp["ammo"].Any(x => x.ToString().Contains(ammoType.ElementAt(_index).Key)))
                {
                    int ammoQ = myWp["ammo"].FirstOrDefault(x => x.ToString().Contains(AmmoType)).ToObject<int>();
                    if (ammoQ < GetConfig.Config["AmmoLimit"][0][AmmoType].ToObject<int>())
                    {
                        int ammoActual = myWp["ammo"].FirstOrDefault(x => x.ToString().Contains(AmmoType)).ToObject<int>();
                        int ammoNeeded = GetConfig.Config["AmmoLimit"][0][AmmoType].ToObject<int>() - ammoActual;
                        double cost = (double)ammoNeeded * ammoType.ElementAt(_index).Value;
                        ActionStore.RestockWeaponAmmo(myWp["id"].ToObject<int>(), cost, AmmoType, ammoNeeded);
                    }
                    else
                    {
                        //Nada
                    }
                }
                else
                {
                    double cost = GetConfig.Config["AmmoLimit"][0][AmmoType].ToObject<int>() * ammoType.ElementAt(_index).Value;
                    ActionStore.RestockWeaponAmmo(myWp["id"].ToObject<int>(), cost, AmmoType, GetConfig.Config["AmmoLimit"][0][AmmoType].ToObject<int>());
                }

                _item.Label = "Comprado";
                _item.Enabled = false;
            };

            manageAmmoMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                //ActionStore.CreateObjectOnTable(_newIndex, "Manage", ActionStore.ObjectStore);
            };

            manageAmmoMenu.OnMenuOpen += (_menu) =>
            {
                var myWp = GetConfig.PlayerWeapons.ElementAt(ManageWeaponsMenu.indexItem);
                var wpc = GetConfig.Config["Weapons"].FirstOrDefault(x => x["HashName"].ToString().Contains(myWp["name"].ToString()));

                foreach (JObject ammoc in wpc["AmmoHash"].Children<JObject>())
                {
                    foreach (JProperty ammo in ammoc.Properties())
                    {
                        if (myWp["ammo"].Any(x => x.ToString().Contains(ammo.Name.ToString())))
                        {
                            int ammoQ = myWp["ammo"].FirstOrDefault(x => x.ToString().Contains(ammo.Name.ToString())).ToObject<int>();
                            Debug.WriteLine(ammo.Name.ToString() + " " + ammoQ.ToString());
                            if (ammoQ < GetConfig.Config["AmmoLimit"][0][ammo.Name].ToObject<int>())
                            {
                                int ammoNeeded = GetConfig.Config["AmmoLimit"][0][ammo.Name].ToObject<int>() - ammoQ;
                                double cost = (double)ammoNeeded * ammo.Value.ToObject<double>();
                                MenuItem _ammoToRestock = new MenuItem(GetConfig.Langs[ammo.Name] + " " + string.Format(GetConfig.Langs["ManageAmmoRestockBtnRestock"], ammoQ.ToString(), GetConfig.Config["AmmoLimit"][0][ammo.Name].ToObject<string>(), cost.ToString()), "")
                                {
                                    Enabled = true
                                };

                                manageAmmoMenu.AddMenuItem(_ammoToRestock);
                            }
                            else
                            {
                                MenuItem _ammoToRestock = new MenuItem(GetConfig.Langs[ammo.Name] + " " + string.Format(GetConfig.Langs["ManageAmmoRestockBtnRestock"], ammoQ.ToString(), GetConfig.Config["AmmoLimit"][0][ammo.Name].ToObject<string>(), "0"), "")
                                {
                                    Enabled = false
                                };

                                manageAmmoMenu.AddMenuItem(_ammoToRestock);

                            }
                         
                        }
                        else
                        {
                            double cost = GetConfig.Config["AmmoLimit"][0][ammo.Name].ToObject<double>() * ammo.Value.ToObject<double>();
                            MenuItem _ammoToRestock = new MenuItem(GetConfig.Langs[ammo.Name] + " " + string.Format(GetConfig.Langs["ManageAmmoRestockBtnRestock"], "0", GetConfig.Config["AmmoLimit"][0][ammo.Name].ToObject<string>(), cost.ToString()), "")
                            {
                                Enabled = true
                            };

                            manageAmmoMenu.AddMenuItem(_ammoToRestock);
                        }

                    }
                }
                //ActionStore.CreateObjectOnTable(_menu.CurrentIndex, "Manage", ActionStore.ObjectStore);
            };

            manageAmmoMenu.OnMenuClose += (_menu) =>
            {
                manageAmmoMenu.ClearMenuItems();
            };


        }

        public static Menu GetMenu()
        {
            SetupMenu();
            return manageAmmoMenu;
        }
    }
}
