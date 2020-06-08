using CitizenFX.Core;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_cl.Menus
{
    class BuyMenu
    {
        private static Menu buyMenu = new Menu(GetConfig.Langs["MenuMainButtonBuyWeapons"], "");
        private static Menu buyMenuConfirm = new Menu("", GetConfig.Langs["MenuBuyWeaponsDesc"]);

        private static int indexItem;
        private static int quantityItem;

        private static bool setupDone = false;

        private static void SetupMenu()
        {
            if (setupDone) return;
            setupDone = true;
            MenuController.AddMenu(buyMenu);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)0;

            MenuController.AddSubmenu(buyMenu, buyMenuConfirm);

            foreach (var weapon in GetConfig.Config["Weapons"])
            {
                MenuItem _weaponToBuy = new MenuItem(weapon["Name"].ToString() + " $" + weapon["Price"].ToString(), "")
                {

                };

                buyMenu.AddMenuItem(_weaponToBuy);
                MenuController.BindMenuItem(buyMenu, buyMenuConfirm, _weaponToBuy);
            }

            MenuItem subMenuConfirmBuyBtnYes = new MenuItem("", "")
            {
                RightIcon = MenuItem.Icon.TICK
            };
            MenuItem subMenuConfirmBuyBtnNo = new MenuItem(GetConfig.Langs["MenuBuyWeaponsButtonNo"], "")
            {
                RightIcon = MenuItem.Icon.ARROW_LEFT
            };

            buyMenuConfirm.AddMenuItem(subMenuConfirmBuyBtnYes);
            buyMenuConfirm.AddMenuItem(subMenuConfirmBuyBtnNo);

            buyMenu.OnItemSelect += (_menu, _item, _index) =>
            {
                indexItem = _index;
                double totalPrice = double.Parse(GetConfig.Config["Weapons"][_index]["Price"].ToString());
                buyMenuConfirm.MenuTitle = GetConfig.Config["Weapons"][_index]["Name"].ToString();
                subMenuConfirmBuyBtnYes.Label = string.Format(GetConfig.Langs["MenuBuyWeaponsButtonYes"], totalPrice.ToString());
            };

            buyMenu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                ActionStore.CreateObjectOnTable(_newIndex, "Weapons", ActionStore.ObjectStore);
            };

            buyMenu.OnMenuOpen += (_menu) =>
            {
                ActionStore.CreateObjectOnTable(_menu.CurrentIndex, "Weapons", ActionStore.ObjectStore);
            };

            buyMenuConfirm.OnItemSelect += (_menu, _item, _index) =>
            {
                if (_index == 0)
                {
                    ActionStore.BuyWeaponStore(indexItem);
                    buyMenu.OpenMenu();
                    buyMenuConfirm.CloseMenu();
                }
                else
                {
                    buyMenu.OpenMenu();
                    buyMenuConfirm.CloseMenu();
                }
            };

        }
        public static Menu GetMenu()
        {
            SetupMenu();
            return buyMenu;
        }
    }
}
