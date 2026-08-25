using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;

namespace MediStock360.Application.Services
{
    public class MenuService : BaseService, IMenuService
    {
        public MenuService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentSession) : base(unitOfWork, currentSession)
        {
        }

        #region User & Role Dynamic Navigation Menus

        public async Task<List<MenuResponseDto>> GetAccessibleMenusForUserAsync(
            long userId, 
            List<string>? roleNames = null, 
            List<string>? permissionCodes = null,
            long? clientId = null,
            long? storeId = null)
        {
            try
            {
                // 1. Resolve roles and permissions if not already supplied
                if (roleNames == null || !roleNames.Any() || permissionCodes == null || !permissionCodes.Any())
                {
                    var userRoles = await _unitOfWork.UserRoleRepository.WhereAsync(ur => ur.UserId == userId);
                    var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();

                    if (roleIds.Any())
                    {
                        var roles = await _unitOfWork.RoleRepository.WhereAsync(r => roleIds.Contains(r.RoleId) && r.IsActive);
                        roleNames = roles.Select(r => r.RoleCode).Concat(roles.Select(r => r.RoleName)).Distinct().ToList();

                        var rolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(rp => roleIds.Contains(rp.RoleId));
                        var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

                        if (permissionIds.Any())
                        {
                            var permissions = await _unitOfWork.PermissionRepository.WhereAsync(p => permissionIds.Contains(p.PermissionId) && p.IsActive);
                            permissionCodes = permissions.Select(p => p.PermissionCode).Distinct().ToList();
                        }
                    }
                }

                roleNames ??= new List<string>();
                permissionCodes ??= new List<string>();

                // 2. Fetch all active and visible menus
                var allMenus = await _unitOfWork.MenuRepository.WhereAsync(m => m.IsActive == 1 && (m.IsVisible == null || m.IsVisible == true));
                if (!allMenus.Any())
                {
                    // Fallback to all active menus
                    allMenus = await _unitOfWork.MenuRepository.WhereAsync(m => m.IsActive == 1);
                }

                if (!allMenus.Any())
                {
                    return new List<MenuResponseDto>();
                }

                // 3. Check for Full Admin / Owner privileges
                bool isFullAdmin = roleNames.Any(r =>
                    r.Equals("CLIENT_OWNER", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("CLIENT_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("Client_Owner", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("Client_Admin", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("Super_Admin", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("Administrator", StringComparison.OrdinalIgnoreCase)
                );

                if (isFullAdmin)
                {
                    return BuildMenuTree(allMenus);
                }

                // 4. Determine permitted menus based on assigned permissions
                var permissionSet = new HashSet<string>(permissionCodes, StringComparer.OrdinalIgnoreCase);
                var permittedMenuIds = new HashSet<int>();

                foreach (var menu in allMenus)
                {
                    if (IsMenuPermitted(menu, permissionSet))
                    {
                        permittedMenuIds.Add(menu.MenuId);
                    }
                }

                return BuildMenuTree(allMenus, permittedMenuIds);
            }
            catch (Exception)
            {
                return new List<MenuResponseDto>();
            }
        }

        public async Task<ApiResponse<List<MenuResponseDto>>> GetNavMenusForCurrentUserAsync()
        {
            try
            {
                if (UserId <= 0)
                {
                    return ApiResponse<List<MenuResponseDto>>.Fail(401, "User is not authenticated.");
                }

                var menus = await GetAccessibleMenusForUserAsync(UserId, null, null, ClientId, StoreId);
                return ApiResponse<List<MenuResponseDto>>.Success(menus, "Navigation menus retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<MenuResponseDto>>.Fail(500, $"Error retrieving navigation menus: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<MenuResponseDto>>> GetAccessibleMenusForRoleAsync(int roleId)
        {
            try
            {
                var role = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.RoleId == roleId);
                if (role == null)
                {
                    return ApiResponse<List<MenuResponseDto>>.Fail(1, $"Role with ID {roleId} not found.");
                }

                var allMenus = await _unitOfWork.MenuRepository.WhereAsync(m => m.IsActive == 1 && (m.IsVisible == null || m.IsVisible == true));
                if (!allMenus.Any())
                {
                    allMenus = await _unitOfWork.MenuRepository.WhereAsync(m => m.IsActive == 1);
                }

                bool isFullAdmin = role.RoleCode.Equals("CLIENT_OWNER", StringComparison.OrdinalIgnoreCase) ||
                                   role.RoleCode.Equals("CLIENT_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                                   role.RoleCode.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                                   role.RoleCode.Equals("ADMIN", StringComparison.OrdinalIgnoreCase);

                if (isFullAdmin)
                {
                    var fullTree = BuildMenuTree(allMenus);
                    return ApiResponse<List<MenuResponseDto>>.Success(fullTree, "Role menus retrieved successfully.");
                }

                var rolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(rp => rp.RoleId == roleId);
                var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

                var permissionCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (permissionIds.Any())
                {
                    var permissions = await _unitOfWork.PermissionRepository.WhereAsync(p => permissionIds.Contains(p.PermissionId) && p.IsActive);
                    foreach (var p in permissions)
                    {
                        permissionCodes.Add(p.PermissionCode);
                    }
                }

                var permittedMenuIds = new HashSet<int>(allMenus.Where(m => IsMenuPermitted(m, permissionCodes)).Select(m => m.MenuId));
                var tree = BuildMenuTree(allMenus, permittedMenuIds);

                return ApiResponse<List<MenuResponseDto>>.Success(tree, "Role menus retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<MenuResponseDto>>.Fail(500, $"Error retrieving role menus: {ex.Message}");
            }
        }

        #endregion

        #region Tree Building & Permission Evaluation

        private bool IsMenuPermitted(Menu menu, HashSet<string> permissionCodes)
        {
            if (permissionCodes == null || !permissionCodes.Any())
                return false;

            // 1. Direct PermissionCode matching
            if (!string.IsNullOrWhiteSpace(menu.PermissionCode))
            {
                if (permissionCodes.Contains(menu.PermissionCode.Trim()))
                    return true;
            }

            // 2. Fallback matching on MenuName / RouterLink
            var menuName = menu.MenuName.Trim().ToLower();
            var routerLink = menu.RouterLink.Trim().ToLower().TrimStart('/');

            foreach (var code in permissionCodes)
            {
                var normalizedCode = code.ToLower();

                if (normalizedCode.Equals(menuName) || normalizedCode.Equals(routerLink))
                    return true;

                if (!string.IsNullOrEmpty(routerLink) && (normalizedCode.Contains(routerLink) || routerLink.Contains(normalizedCode)))
                    return true;

                var cleanCode = normalizedCode
                    .Replace("_view", "")
                    .Replace("_create", "")
                    .Replace("_edit", "")
                    .Replace("_delete", "")
                    .Replace("_manage", "")
                    .Replace("view", "")
                    .Replace("create", "")
                    .Replace("edit", "");

                if (!string.IsNullOrEmpty(cleanCode) && cleanCode.Length >= 3 && (menuName.Contains(cleanCode) || routerLink.Contains(cleanCode)))
                    return true;
            }

            return false;
        }

        private List<MenuResponseDto> BuildMenuTree(List<Menu> allMenus, HashSet<int>? permittedMenuIds = null)
        {
            var result = new List<MenuResponseDto>();
            var parents = allMenus
                .Where(m => m.ParentMenuId == null || m.ParentMenuId == 0)
                .OrderBy(m => m.DisplayOrder ?? 0)
                .ThenBy(m => m.MenuName)
                .ToList();

            var childrenByParent = allMenus
                .Where(m => m.ParentMenuId != null && m.ParentMenuId > 0)
                .GroupBy(m => m.ParentMenuId!.Value)
                .ToDictionary(
                    g => g.Key, 
                    g => g.OrderBy(m => m.DisplayOrder ?? 0).ThenBy(m => m.MenuName).ToList()
                );

            foreach (var parent in parents)
            {
                var parentDto = MapToDto(parent);

                if (childrenByParent.TryGetValue(parent.MenuId, out var children))
                {
                    var validChildren = permittedMenuIds == null
                        ? children
                        : children.Where(c => permittedMenuIds.Contains(c.MenuId)).ToList();

                    if (validChildren.Any())
                    {
                        parentDto.SubMenus = validChildren.Select(c => MapToDto(c, parent.MenuName)).ToList();
                        result.Add(parentDto);
                    }
                    else if (permittedMenuIds != null && permittedMenuIds.Contains(parent.MenuId))
                    {
                        // Parent is directly permitted even if submenus are not
                        result.Add(parentDto);
                    }
                }
                else
                {
                    // Leaf top-level menu
                    if (permittedMenuIds == null || permittedMenuIds.Contains(parent.MenuId))
                    {
                        result.Add(parentDto);
                    }
                }
            }

            return result;
        }

        private MenuResponseDto MapToDto(Menu menu, string? parentMenuName = null)
        {
            return new MenuResponseDto
            {
                MenuId = menu.MenuId,
                ParentMenuId = menu.ParentMenuId,
                ParentMenuName = parentMenuName ?? menu.ParentMenu?.MenuName,
                MenuName = menu.MenuName,
                MenuIcon = menu.MenuIcon,
                RouterLink = menu.RouterLink,
                PermissionCode = menu.PermissionCode,
                DisplayOrder = menu.DisplayOrder ?? 0,
                IsVisible = menu.IsVisible ?? true,
                IsActive = menu.IsActive == 1,
                SubMenus = new List<MenuResponseDto>()
            };
        }

        #endregion

        #region Menu Master Management (CRUD)

        public async Task<ApiResponse<List<MenuResponseDto>>> GetAllMenusTreeAsync()
        {
            try
            {
                var allMenus = await _unitOfWork.MenuRepository.GetAllAsync();
                var tree = BuildMenuTree(allMenus);
                return ApiResponse<List<MenuResponseDto>>.Success(tree, "Menu hierarchy retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<MenuResponseDto>>.Fail(500, $"Error retrieving menus: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<MenuResponseDto>>> GetAllMenusFlatAsync()
        {
            try
            {
                var allMenus = await _unitOfWork.MenuRepository.GetAllAsync();
                var parentNames = allMenus.ToDictionary(m => m.MenuId, m => m.MenuName);

                var list = allMenus
                    .OrderBy(m => m.ParentMenuId ?? 0)
                    .ThenBy(m => m.DisplayOrder ?? 0)
                    .Select(m => MapToDto(m, m.ParentMenuId.HasValue && parentNames.TryGetValue(m.ParentMenuId.Value, out var pName) ? pName : null))
                    .ToList();

                return ApiResponse<List<MenuResponseDto>>.Success(list, "Flat menus retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<MenuResponseDto>>.Fail(500, $"Error retrieving menus: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MenuResponseDto>> GetMenuByIdAsync(int id)
        {
            try
            {
                var menu = await _unitOfWork.MenuRepository.FirstOrDefaultAsync(m => m.MenuId == id);
                if (menu == null)
                {
                    return ApiResponse<MenuResponseDto>.Fail(1, $"Menu with ID {id} not found.");
                }

                string? parentName = null;
                if (menu.ParentMenuId.HasValue)
                {
                    var parent = await _unitOfWork.MenuRepository.FirstOrDefaultAsync(m => m.MenuId == menu.ParentMenuId.Value);
                    parentName = parent?.MenuName;
                }

                return ApiResponse<MenuResponseDto>.Success(MapToDto(menu, parentName), "Menu retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<MenuResponseDto>.Fail(500, $"Error retrieving menu: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MenuResponseDto>> CreateMenuAsync(CreateMenuRequestDto dto)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<MenuResponseDto>.Fail(1, "Invalid request payload.");

                var menuName = dto.MenuName.Trim();

                var exists = await _unitOfWork.MenuRepository.AnyAsync(m => m.MenuName.ToLower() == menuName.ToLower());
                if (exists)
                {
                    return ApiResponse<MenuResponseDto>.Fail(1, $"Menu with name '{menuName}' already exists.");
                }

                var entity = new Menu
                {
                    ParentMenuId = dto.ParentMenuId > 0 ? dto.ParentMenuId : null,
                    MenuName = menuName,
                    MenuIcon = dto.MenuIcon.Trim(),
                    RouterLink = dto.RouterLink.Trim(),
                    PermissionCode = string.IsNullOrWhiteSpace(dto.PermissionCode) ? null : dto.PermissionCode.Trim().ToUpper(),
                    DisplayOrder = dto.DisplayOrder ?? 0,
                    IsVisible = dto.IsVisible,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = UserId > 0 ? (long?)UserId : null
                };

                await _unitOfWork.MenuRepository.AddAsync(entity);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                {
                    return ApiResponse<MenuResponseDto>.Fail(1, "Failed to create menu.");
                }

                return ApiResponse<MenuResponseDto>.Success(MapToDto(entity), "Menu created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<MenuResponseDto>.Fail(500, $"Error creating menu: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateMenuAsync(int id, UpdateMenuRequestDto dto)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<bool>.Fail(1, "Invalid request payload.");

                var menu = await _unitOfWork.MenuRepository.FirstOrDefaultAsync(m => m.MenuId == id);
                if (menu == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Menu with ID {id} not found.");
                }

                var menuName = dto.MenuName.Trim();
                var duplicate = await _unitOfWork.MenuRepository.AnyAsync(m => m.MenuName.ToLower() == menuName.ToLower() && m.MenuId != id);
                if (duplicate)
                {
                    return ApiResponse<bool>.Fail(1, $"Another menu with name '{menuName}' already exists.");
                }

                menu.ParentMenuId = dto.ParentMenuId > 0 ? dto.ParentMenuId : null;
                menu.MenuName = menuName;
                menu.MenuIcon = dto.MenuIcon.Trim();
                menu.RouterLink = dto.RouterLink.Trim();
                menu.PermissionCode = string.IsNullOrWhiteSpace(dto.PermissionCode) ? null : dto.PermissionCode.Trim().ToUpper();
                menu.DisplayOrder = dto.DisplayOrder ?? menu.DisplayOrder;
                menu.IsVisible = dto.IsVisible;
                menu.IsActive = dto.IsActive;
                menu.UpdatedAt = DateTime.UtcNow;
                menu.UpdatedBy = UserId > 0 ? (long?)UserId : null;

                _unitOfWork.MenuRepository.Update(menu);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                {
                    return ApiResponse<bool>.Fail(1, "Failed to update menu.");
                }

                return ApiResponse<bool>.Success(true, "Menu updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error updating menu: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ToggleMenuStatusAsync(int id)
        {
            try
            {
                var menu = await _unitOfWork.MenuRepository.FirstOrDefaultAsync(m => m.MenuId == id);
                if (menu == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Menu with ID {id} not found.");
                }

                menu.IsActive = menu.IsActive == 1 ? 0 : 1;
                menu.UpdatedAt = DateTime.UtcNow;
                menu.UpdatedBy = UserId > 0 ? (long?)UserId : null;

                _unitOfWork.MenuRepository.Update(menu);
                await _unitOfWork.SaveChangesAsync();

                string statusText = menu.IsActive == 1 ? "activated" : "deactivated";
                return ApiResponse<bool>.Success(menu.IsActive == 1, $"Menu has been {statusText} successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error updating menu status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteMenuAsync(int id)
        {
            try
            {
                var menu = await _unitOfWork.MenuRepository.FirstOrDefaultAsync(m => m.MenuId == id);
                if (menu == null)
                {
                    return ApiResponse<bool>.Fail(1, $"Menu with ID {id} not found.");
                }

                // Delete child submenus if any
                var children = await _unitOfWork.MenuRepository.WhereAsync(m => m.ParentMenuId == id);
                if (children.Any())
                {
                    _unitOfWork.MenuRepository.RemoveRange(children);
                }

                _unitOfWork.MenuRepository.Remove(menu);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                {
                    return ApiResponse<bool>.Fail(1, "Failed to delete menu.");
                }

                return ApiResponse<bool>.Success(true, "Menu and its submenus deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error deleting menu: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> SeedDefaultMenusAsync()
        {
            try
            {
                var defaults = new List<(string Name, string? Parent, string Icon, string Link, string? PermCode, int Order)>
                {
                    // Top-level
                    ("Dashboard", null, "dashboard", "/dashboard", "DASHBOARD_VIEW", 1),
                    ("Administration", null, "admin_panel_settings", "/admin", "ADMIN_VIEW", 2),
                    ("Inventory & Stock", null, "inventory_2", "/inventory", "INVENTORY_VIEW", 3),
                    ("Purchases & Suppliers", null, "shopping_cart", "/purchases", "PURCHASE_VIEW", 4),
                    ("Sales & POS", null, "point_of_sale", "/sales", "SALES_VIEW", 5),
                    ("Customers & Prescriptions", null, "person_search", "/customers", "CUSTOMER_VIEW", 6),
                    ("Reports & Analytics", null, "assessment", "/reports", "REPORT_VIEW", 7),
                    ("Settings", null, "settings", "/settings", "SETTING_VIEW", 8),

                    // Administration Submenus
                    ("Clients", "Administration", "business", "/admin/clients", "CLIENT_VIEW", 1),
                    ("Stores & Branches", "Administration", "store", "/admin/stores", "STORE_VIEW", 2),
                    ("Users & Staff", "Administration", "people", "/admin/users", "USER_VIEW", 3),
                    ("Roles & Permissions", "Administration", "security", "/admin/roles-permissions", "ROLE_VIEW", 4),
                    ("Menu Management", "Administration", "menu_book", "/admin/menus", "MENU_VIEW", 5),

                    // Inventory Submenus
                    ("Product Catalog", "Inventory & Stock", "medication", "/inventory/products", "PRODUCT_VIEW", 1),
                    ("Stock Overview", "Inventory & Stock", "warehouse", "/inventory/stock", "STOCK_VIEW", 2),
                    ("Categories & Brands", "Inventory & Stock", "category", "/inventory/categories", "CATEGORY_VIEW", 3),
                    ("Batch & Expiry Tracker", "Inventory & Stock", "event_busy", "/inventory/batches", "BATCH_VIEW", 4),

                    // Purchases Submenus
                    ("Suppliers", "Purchases & Suppliers", "local_shipping", "/purchases/suppliers", "SUPPLIER_VIEW", 1),
                    ("Purchase Orders", "Purchases & Suppliers", "receipt_long", "/purchases/orders", "PURCHASE_ORDER_VIEW", 2),
                    ("Goods Received Notes (GRN)", "Purchases & Suppliers", "inventory", "/purchases/grn", "GRN_VIEW", 3),

                    // Sales Submenus
                    ("POS Billing Counter", "Sales & POS", "receipt", "/sales/pos", "POS_VIEW", 1),
                    ("Invoices & Bills", "Sales & POS", "history", "/sales/invoices", "INVOICE_VIEW", 2),
                    ("Sales Returns", "Sales & POS", "assignment_return", "/sales/returns", "SALES_RETURN_VIEW", 3),

                    // Customers Submenus
                    ("Customer Directory", "Customers & Prescriptions", "contacts", "/customers/list", "CUSTOMER_VIEW", 1),
                    ("Prescriptions", "Customers & Prescriptions", "description", "/customers/prescriptions", "PRESCRIPTION_VIEW", 2),

                    // Reports Submenus
                    ("Sales Reports", "Reports & Analytics", "trending_up", "/reports/sales", "REPORT_SALES_VIEW", 1),
                    ("Inventory Reports", "Reports & Analytics", "inventory", "/reports/inventory", "REPORT_INVENTORY_VIEW", 2),
                    ("Financial Reports", "Reports & Analytics", "account_balance_wallet", "/reports/financial", "REPORT_FINANCIAL_VIEW", 3),

                    // Settings Submenus
                    ("Store Settings", "Settings", "storefront", "/settings/store", "STORE_SETTING_VIEW", 1),
                    ("General Settings", "Settings", "tune", "/settings/general", "SETTING_VIEW", 2)
                };

                // Seed top level parents first
                foreach (var item in defaults.Where(d => d.Parent == null))
                {
                    var existing = await _unitOfWork.MenuRepository.FirstOrDefaultAsync(m => m.MenuName == item.Name);
                    if (existing == null)
                    {
                        await _unitOfWork.MenuRepository.AddAsync(new Menu
                        {
                            MenuName = item.Name,
                            ParentMenuId = null,
                            MenuIcon = item.Icon,
                            RouterLink = item.Link,
                            PermissionCode = item.PermCode,
                            DisplayOrder = item.Order,
                            IsVisible = true,
                            IsActive = 1,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        existing.MenuIcon = item.Icon;
                        existing.RouterLink = item.Link;
                        existing.PermissionCode = item.PermCode;
                        existing.DisplayOrder = item.Order;
                        existing.IsVisible = true;
                        existing.IsActive = 1;
                        _unitOfWork.MenuRepository.Update(existing);
                    }
                }
                await _unitOfWork.SaveChangesAsync();

                // Seed children
                var parents = await _unitOfWork.MenuRepository.WhereAsync(m => m.ParentMenuId == null);
                var parentMap = parents.ToDictionary(p => p.MenuName, p => p.MenuId);

                foreach (var item in defaults.Where(d => d.Parent != null))
                {
                    if (parentMap.TryGetValue(item.Parent!, out int parentId))
                    {
                        var existing = await _unitOfWork.MenuRepository.FirstOrDefaultAsync(m => m.MenuName == item.Name);
                        if (existing == null)
                        {
                            await _unitOfWork.MenuRepository.AddAsync(new Menu
                            {
                                MenuName = item.Name,
                                ParentMenuId = parentId,
                                MenuIcon = item.Icon,
                                RouterLink = item.Link,
                                PermissionCode = item.PermCode,
                                DisplayOrder = item.Order,
                                IsVisible = true,
                                IsActive = 1,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            existing.ParentMenuId = parentId;
                            existing.MenuIcon = item.Icon;
                            existing.RouterLink = item.Link;
                            existing.PermissionCode = item.PermCode;
                            existing.DisplayOrder = item.Order;
                            existing.IsVisible = true;
                            existing.IsActive = 1;
                            _unitOfWork.MenuRepository.Update(existing);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "Default dynamic menus seeded successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error seeding default menus: {ex.Message}");
            }
        }

        #endregion
    }
}

