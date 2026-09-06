using MediStock360.Application.DTOs.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.Common
{
    public static class SortMenuTree
    {
        public static List<MenuResponseDto> BuildMenuTree(List<MenuResponseDto> flatList)
        {

            var rootMenus = new List<MenuResponseDto>();
            rootMenus =flatList.Where(X => X.ParentMenuId == null).ToList();

            foreach (var menu in rootMenus)
            {
                    var submenu = flatList.Where(x => x.ParentMenuId == menu.MenuId).ToList();
                    menu.SubMenus=submenu;
            }
            // Sort recursively by DisplayOrder
            SortMenuTrees(rootMenus);

            return rootMenus;
        }

        //private static void SortMenuTree(List<MenuResponseDto> menus)
        //{
        //    menus.Sort((a, b) => a.DisplayOrder.CompareTo(b.DisplayOrder));
        //    foreach (var menu in menus)
        //    {
        //        if (menu.SubMenus.Any())
        //            SortMenuTree(menu.SubMenus);
        //    }
        //}

        private static void SortMenuTrees(List<MenuResponseDto> menus)
        {
            if (menus == null || menus.Count == 0)
                return;

            menus.Sort((a, b) =>
            {
                var orderA = a.DisplayOrder ?? 0;
                var orderB = b.DisplayOrder ?? 0;

                var result = orderA.CompareTo(orderB);

                // Optional: if same DisplayOrder, sort by name
                return result != 0
                    ? result
                    : string.Compare(
                        a.MenuName,
                        b.MenuName,
                        StringComparison.OrdinalIgnoreCase);
            });

            foreach (var menu in menus)
            {
                if (menu.SubMenus != null && menu.SubMenus.Count > 0)
                {
                    SortMenuTrees(menu.SubMenus);
                }
            }
        }
    }
}
