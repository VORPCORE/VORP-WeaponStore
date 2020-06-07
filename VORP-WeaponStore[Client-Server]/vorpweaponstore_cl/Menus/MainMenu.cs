using CitizenFX.Core;
using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_cl.Menus
{
    public class MainMenu
    {
        private static Menu mainMenu = new Menu("", GetConfig.Langs["MenuMainDesc"]);
        private static bool setupDone = false;
        private static void SetupMenu()
        {
            if (setupDone) return;
            setupDone = true;
            MenuController.AddMenu(mainMenu);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)0;

            //Weapons Buy Menu
            MenuController.AddSubmenu(mainMenu, BuyMenu.GetMenu());

            MenuItem subMenuBuyBtn = new MenuItem(GetConfig.Langs["MenuMainButtonBuyWeapons"], " ")
            {
                RightIcon = MenuItem.Icon.ARROW_RIGHT
            };

            mainMenu.AddMenuItem(subMenuBuyBtn);
            MenuController.BindMenuItem(mainMenu, BuyMenu.GetMenu(), subMenuBuyBtn);


            mainMenu.OnMenuClose += (_menu) =>
            {
                ActionStore.ExitBuyStore();
            };

        }
        public static Menu GetMenu()
        {
            SetupMenu();
            return mainMenu;
        }
    }
}
