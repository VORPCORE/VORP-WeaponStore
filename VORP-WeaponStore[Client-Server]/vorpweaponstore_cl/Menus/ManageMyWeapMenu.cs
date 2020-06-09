using CitizenFX.Core;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_cl.Menus
{
    class ManageMyWeapMenu
    {
        private static Menu manageMyWeapMenu = new Menu("", GetConfig.Langs["MenuManageWeaponDesc"]);
        private static bool setupDone = false;



        private static void SetupMenu()
        {
            if (setupDone) return;
            setupDone = true;
            MenuController.AddMenu(manageMyWeapMenu);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)0;

            //SubMenu Ammo Restock
            MenuController.AddSubmenu(manageMyWeapMenu, ManageAmmoMenu.GetMenu());
            MenuItem restockAmmo = new MenuItem(GetConfig.Langs["MenuManageAmmo"], "")
            {
                LeftIcon = MenuItem.Icon.ARROW_RIGHT
            };

            manageMyWeapMenu.AddMenuItem(restockAmmo);
            MenuController.BindMenuItem(manageMyWeapMenu, ManageAmmoMenu.GetMenu(), restockAmmo);
            //end

            manageMyWeapMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                //indexItem = _index; MenuManageAmmoDesc
                //double totalPrice = double.Parse(GetConfig.Config["Weapons"][_index]["Price"].ToString());
                //buyMenuConfirm.MenuTitle = GetConfig.Config["Weapons"][_index]["Name"].ToString();
                //subMenuConfirmBuyBtnYes.Label = string.Format(GetConfig.Langs["MenuBuyWeaponsButtonYes"], totalPrice.ToString());
            };

            manageMyWeapMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                //ActionStore.CreateObjectOnTable(_newIndex, "Manage", ActionStore.ObjectStore);
            };

            manageMyWeapMenu.OnMenuOpen += (_menu) =>
            {
                //foreach (var weapon in GetConfig.PlayerWeapons)
                //{
                //    var wp = GetConfig.Config["Weapons"].FirstOrDefault(x => x["HashName"].ToString().Contains(weapon["name"].ToString()));
                //    MenuItem _weaponToManage = new MenuItem(wp["Name"].ToString(), "")
                //    {
                //        LeftIcon = MenuItem.Icon.ARROW_RIGHT
                //    };

                //    manageMyWeapMenu.AddMenuItem(_weaponToManage);
                //}
                //ActionStore.CreateObjectOnTable(_menu.CurrentIndex, "Manage", ActionStore.ObjectStore);
            };

            manageMyWeapMenu.OnMenuClose += (_menu) =>
            {

            };


        }

        public static Menu GetMenu()
        {
            SetupMenu();
            return manageMyWeapMenu;
        }
    }
}
