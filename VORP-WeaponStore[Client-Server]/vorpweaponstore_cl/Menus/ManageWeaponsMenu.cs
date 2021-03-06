﻿using CitizenFX.Core;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_cl.Menus
{
    class ManageWeaponsMenu
    {
        private static Menu manageMenu = new Menu(GetConfig.Langs["MenuMainButtonManageWeapon"], "");
        private static bool setupDone = false;
        public static int indexItem;

        private static void SetupMenu()
        {
            if (setupDone) return;
            setupDone = true;
            MenuController.AddMenu(manageMenu);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)0;

            //SubMenu 
            MenuController.AddSubmenu(manageMenu, ManageMyWeapMenu.GetMenu());
            //end


            manageMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                indexItem = _index;
                var myWp = GetConfig.PlayerWeapons.ElementAt(_index);
                var wpc = GetConfig.Config["Weapons"].FirstOrDefault(x => x["HashName"].ToString().Contains(myWp["name"].ToString()));
                ManageMyWeapMenu.GetMenu().MenuTitle = wpc["Name"].ToString();
                //double totalPrice = double.Parse(GetConfig.Config["Weapons"][_index]["Price"].ToString());
                //buyMenuConfirm.MenuTitle = GetConfig.Config["Weapons"][_index]["Name"].ToString();
                //subMenuConfirmBuyBtnYes.Label = string.Format(GetConfig.Langs["MenuBuyWeaponsButtonYes"], totalPrice.ToString());
            };

            manageMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                indexItem = _newIndex;
                ActionStore.CreateObjectOnTable(_newIndex, "Manage", ActionStore.ObjectStore);
            };

            manageMenu.OnMenuOpen += (_menu) =>
            {
                foreach (var weapon in GetConfig.PlayerWeapons)
                {
                    var wp = GetConfig.Config["Weapons"].FirstOrDefault(x => x["HashName"].ToString().Contains(weapon["name"].ToString()));
                    MenuItem _weaponToManage = new MenuItem(wp["Name"].ToString(), "")
                    {
                        LeftIcon = MenuItem.Icon.ARROW_RIGHT
                    };

                    manageMenu.AddMenuItem(_weaponToManage);
                    MenuController.BindMenuItem(manageMenu, ManageMyWeapMenu.GetMenu(), _weaponToManage);
                }
                //ActionStore.CreateObjectOnTable(_menu.CurrentIndex, "Manage", ActionStore.ObjectStore);
            };

            manageMenu.OnMenuClose += (_menu) =>
            {
                manageMenu.ClearMenuItems();
            };


        }

        public static Menu GetMenu()
        {
            SetupMenu();
            return manageMenu;
        }
    }
}
