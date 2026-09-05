using HRMS.Application.Services;
using MediStock360.Application.Common;
using MediStock360.Application.Common.constaints;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static MediStock360.Application.Common.GenericProcedureCall;

namespace MediStock360.Application.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IOTPService _otpService;
        private readonly IMenuService _menuService;
        private readonly ISettingService _settingService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IOTPService oTPService,
            IMenuService menuService,
            ICurrentUserService currentSession) : base(unitOfWork, currentSession)
        {
            _otpService = oTPService;
            _menuService = menuService;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        #region SignUp & OTP

        public async Task<ApiResponse<string>> SignUpAsync(SignupRequestDto dto)
        {
            int err_no = 0;
            string err_msg = string.Empty;

            try
            {
                var pwdResult = PasswordHelper.HashPassword(dto.Password);

                var param = new List<SqlParameter>
                {
                    new SqlParameter("@CompanyName", dto.CompanyName),
                    new SqlParameter("@UserName", dto.UserName),
                    new SqlParameter("@Email", dto.Email),
                    new SqlParameter("@HashPassword", pwdResult.hash),
                    new SqlParameter("@UserSalt", pwdResult.salt),
                    new SqlParameter("@CreatedBy", Global.InternalUser),
                    new SqlParameter("@ErrNumber", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    },
                    new SqlParameter("@ErrMsg", SqlDbType.VarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    }
                };
                var result = ExecuteStoredProcedure(StoredProcedure.sp_RegisterClientUser, param, _unitOfWork.GetConnection());
                err_no = param.First(p => p.ParameterName == "@ErrNumber").Value != DBNull.Value
                    ? Convert.ToInt32(param.First(p => p.ParameterName == "@ErrNumber").Value) : 0;
                err_msg = param.First(p => p.ParameterName == "@ErrMsg").Value?.ToString() ?? "";

                if (err_no != 0)
                    return ApiResponse<string>.Fail(err_no, err_msg);

                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive == false);

                if (user != null)
                {
                    try
                    {
                        var otpResult = await _otpService.SendOtpAsync(dto.Email);
                        if (!otpResult.IsSuccess)
                        {
                            return ApiResponse<string>.Success(null, "Signup successful. OTP sending failed, please try forget-password to request OTP");
                        }

                        return ApiResponse<string>.Success(null, "Signup successful. OTP sent to your email");
                    }
                    catch (Exception otpEx)
                    {
                        return ApiResponse<string>.Success(null, $"Signup successful. Note: {otpEx.Message}");
                    }
                }
                else
                {
                    return ApiResponse<string>.Success(null, "Signup successful. Unable to send OTP - user not found");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail(500, ex.Message);
            }
        }
        #endregion

        #region Login & Multi-Client / Multi-Store Dynamic Menu Resolution


        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            try
            {
                //var expiryMinutes = await _settingService.GetAppSettingValue(SettingKeys.OtpExpiryMinutes) ?? "10";
                var expiryMinutes = "10";
                if (dto == null || string.IsNullOrWhiteSpace(dto.EmailOrUsername) || string.IsNullOrWhiteSpace(dto.Password))
                {
                    return ApiResponse<LoginResponseDto>.Fail(1, "Email/Username and Password are required.");
                }

                var identifier = dto.EmailOrUsername.Trim();

                // 1. Find user by Email or UserName
                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(
                    u => u.Email.ToLower() == identifier.ToLower() || u.UserName.ToLower() == identifier.ToLower()
                );

                if (user == null)
                {
                    return ApiResponse<LoginResponseDto>.Fail(1, "Invalid username/email or password.");
                }

                // 2. Verify password
                bool isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.UserSalt);
                if (!isPasswordValid)
                {
                    return ApiResponse<LoginResponseDto>.Fail(1, "Invalid username/email or password.");
                }

                var param = new List<SqlParameter>
                {
                    new SqlParameter("@ClientId", user.ClientId),
                    new SqlParameter("@UserId", user.UserId),
                };
                var result = ExecuteStoredProcedure(StoredProcedure.Sp_Login, param, _unitOfWork.GetConnection());
                var userResponse = CommonMethod.ConvertToList<UserResponseDto>(result.Tables[0]).FirstOrDefault();
                var roles = CommonMethod.ConvertToList<RoleResponseDto>(result.Tables[1]);
                var permissions = CommonMethod.ConvertToList<PermissionResponseDto>(result.Tables[2]);
                var client = CommonMethod.ConvertToList<UserClientDto>(result.Tables[3]).FirstOrDefault();
                var stores = CommonMethod.ConvertToList<StoreResponseDto>(result.Tables[4]);
                var menus = CommonMethod.ConvertToList<MenuResponseDto>(result.Tables[5]);
                var activeStore = stores.FirstOrDefault(x => x.IsDefaultStore == true);
                var permissionCodes = permissions.Select(x => x.PermissionCode);

                var tokenString = GenerateJwtToken(userResponse, client, activeStore, expiryMinutes, roles, permissionCodes);

                var dynamicMenus = SortMenuTree.BuildMenuTree(menus);
                var responseDto = new LoginResponseDto
                {
                    Token = tokenString,
                    TokenType = "Bearer",
                    //ExpiresAt = expiresAt,
                    //IsActive = isActive,
                    Client = client,
                    OnboardingStep = client.OnboardingStep,
                    Stores = stores,
                    ActiveStore = activeStore,
                    Menus = dynamicMenus,
                    Role = roles.Select(r => r.RoleName).ToList(),
                    Permissions = permissionCodes.ToList(),
                    IsOnboardingCompleted = client.IsOnboardingCompleted,
                    User= userResponse
                };
                return ApiResponse<LoginResponseDto>.Success(responseDto, "Login successful");
                //return ApiResponse<LoginResponseDto>.Success(null, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.Fail(500, $"An error occurred during login: {ex.Message}");
            }
        }

        //public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        //{
        //    try
        //    {
        //        if (dto == null || string.IsNullOrWhiteSpace(dto.EmailOrUsername) || string.IsNullOrWhiteSpace(dto.Password))
        //        {
        //            return ApiResponse<LoginResponseDto>.Fail(1, "Email/Username and Password are required.");
        //        }

        //        var identifier = dto.EmailOrUsername.Trim();

        //        // 1. Find user by Email or UserName
        //        var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(
        //            u => u.Email.ToLower() == identifier.ToLower() || u.UserName.ToLower() == identifier.ToLower()
        //        );

        //        if (user == null)
        //        {
        //            return ApiResponse<LoginResponseDto>.Fail(1, "Invalid username/email or password.");
        //        }

        //        // 2. Verify password
        //        bool isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.UserSalt);
        //        if (!isPasswordValid)
        //        {
        //            return ApiResponse<LoginResponseDto>.Fail(1, "Invalid username/email or password.");
        //        }

        //        // 3. Fetch Roles
        //        var userRoles = await _unitOfWork.UserRoleRepository.WhereAsync(ur => ur.UserId == user.UserId);
        //        var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();

        //        var roles = new List<Role>();
        //        if (roleIds.Any())
        //        {
        //            roles = await _unitOfWork.RoleRepository.WhereAsync(r => roleIds.Contains(r.RoleId) && r.IsActive);
        //        }

        //        // 4. Determine User Type / Persona
        //        bool isProductOwner = roles.Any(r =>
        //            r.RoleCode.Equals("PRODUCT_OWNER", StringComparison.OrdinalIgnoreCase) ||
        //            r.RoleCode.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
        //            r.RoleCode.Equals("SUPERADMIN", StringComparison.OrdinalIgnoreCase) ||
        //            r.RoleName.Equals("Product Owner", StringComparison.OrdinalIgnoreCase) ||
        //            r.RoleName.Equals("Super Admin", StringComparison.OrdinalIgnoreCase));

        //        bool isClient = !isProductOwner && roles.Any(r =>
        //            r.RoleCode.Equals("CLIENT_OWNER", StringComparison.OrdinalIgnoreCase) ||
        //            r.RoleCode.Equals("CLIENT_ADMIN", StringComparison.OrdinalIgnoreCase) ||
        //            r.RoleName.Equals("Client Owner", StringComparison.OrdinalIgnoreCase) ||
        //            r.RoleName.Equals("Client Admin", StringComparison.OrdinalIgnoreCase));

        //        // 5. Resolve Client(s) & Store(s) based on Persona
        //        List<UserClientDto> clientsList;
        //        Client? client = null;
        //        List<Store> availableStores = new();

        //        if (isProductOwner)
        //        {
        //            // Product Owner: Show all Clients (both Active and Inactive)
        //            var allClients = await _unitOfWork.ClientRepository.GetAllAsync();
        //            clientsList = allClients.Select(MapToUserClientDto).OrderBy(c => c.CompanyName).ToList();

        //            if (dto.ClientId.HasValue && dto.ClientId.Value > 0)
        //            {
        //                client = allClients.FirstOrDefault(c => c.ClientId == dto.ClientId.Value);
        //            }
        //            client ??= allClients.FirstOrDefault(c => c.ClientId == user.ClientId) ?? allClients.FirstOrDefault();

        //            // Load all active stores for the selected client
        //            if (client != null)
        //            {
        //                availableStores = await _unitOfWork.StoreRepository.WhereAsync(s => s.ClientId == client.ClientId && s.IsActive);
        //            }
        //        }
        //        else if (isClient)
        //        {
        //            // Client (Owner / Admin): Show only that client and its stores
        //            long targetClientId = (long)((dto.ClientId.HasValue && dto.ClientId.Value > 0) ? dto.ClientId.Value : user.ClientId);
        //            client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == targetClientId);
        //            if (client == null)
        //            {
        //                return ApiResponse<LoginResponseDto>.Fail(1, "Associated client organization not found.");
        //            }

        //            clientsList = new List<UserClientDto> { MapToUserClientDto(client) };
        //            availableStores = await _unitOfWork.StoreRepository.WhereAsync(s => s.ClientId == client.ClientId && s.IsActive);
        //        }
        //        else
        //        {
        //            // Regular User: Show only that client and all mapped stores of that user
        //            long targetClientId = (long)((dto.ClientId.HasValue && dto.ClientId.Value > 0) ? dto.ClientId.Value : user.ClientId);
        //            client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == targetClientId);
        //            if (client == null)
        //            {
        //                return ApiResponse<LoginResponseDto>.Fail(1, "Associated client organization not found.");
        //            }

        //            clientsList = new List<UserClientDto> { MapToUserClientDto(client) };

        //            // Fetch all mapped stores of this user
        //            var userStoreMaps = await _unitOfWork.StoreUserMapRepository.WhereAsync(m => m.UserId == user.UserId && m.IsActive);
        //            var mappedStoreIds = userStoreMaps.Select(m => m.StoreId).Distinct().ToList();

        //            if (mappedStoreIds.Any())
        //            {
        //                availableStores = await _unitOfWork.StoreRepository.WhereAsync(s => mappedStoreIds.Contains(s.StoreId) && s.IsActive);
        //            }
        //            else
        //            {
        //                // Fallback: active stores for client if mapping not yet populated
        //                availableStores = await _unitOfWork.StoreRepository.WhereAsync(s => s.ClientId == client.ClientId && s.IsActive);
        //            }
        //        }

        //        var storeDtos = availableStores.Select(MapToStoreResponseDto).ToList();

        //        Store? activeStore = null;
        //        if (dto.StoreId.HasValue && dto.StoreId.Value > 0)
        //        {
        //            activeStore = availableStores.FirstOrDefault(s => s.StoreId == dto.StoreId.Value);
        //        }
        //        activeStore ??= availableStores.FirstOrDefault();

        //        var activeClientDto = client != null ? MapToUserClientDto(client) : null;
        //        var activeStoreDto = activeStore != null ? MapToStoreResponseDto(activeStore) : null;

        //        // 6. Fetch Permissions for the assigned roles
        //        var permissions = new List<Permission>();
        //        if (roleIds.Any())
        //        {
        //            var rolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(rp => roleIds.Contains(rp.RoleId));
        //            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

        //            if (permissionIds.Any())
        //            {
        //                permissions = await _unitOfWork.PermissionRepository.WhereAsync(p => permissionIds.Contains(p.PermissionId) && p.IsActive);
        //            }
        //        }

        //        var roleCodes = roles.Select(r => r.RoleCode).Concat(roles.Select(r => r.RoleName)).Distinct().ToList();
        //        var permissionCodes = permissions.Select(p => p.PermissionCode).Distinct().ToList();

        //        // 7. Update LastLoginAt timestamp
        //        user.LastLoginAt = DateTime.UtcNow;
        //        _unitOfWork.UserRepository.Update(user);
        //        await _unitOfWork.SaveChangesAsync();

        //        // 8. Onboarding & Active status
        //        bool isOnboardingCompleted = client?.IsOnboardingCompleted ?? false;
        //        int onboardingStep = client?.OnboardingStep ?? 0;
        //        bool isActive = user.IsActive;

        //        // 9. Generate Dynamic Menus & Submenus
        //        var dynamicMenus = await _menuService.GetAccessibleMenusForUserAsync(
        //            user.UserId, 
        //            roleCodes, 
        //            permissionCodes, 
        //            client?.ClientId, 
        //            activeStore?.StoreId
        //        );

        //        // 10. Generate JWT Token with Multi-Store and Multi-Client Context
        //        var tokenExpiryMinutes = 60;
        //        if (int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int exp) && exp > 0)
        //        {
        //            tokenExpiryMinutes = exp;
        //        }

        //        var expiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);
        //        var tokenString = GenerateJwtToken(user, client, activeStore, roles, permissionCodes, expiresAt, isOnboardingCompleted, onboardingStep);

        //        // 11. Construct Complete Response
        //        var responseDto = new LoginResponseDto
        //        {
        //            Token = tokenString,
        //            TokenType = "Bearer",
        //            ExpiresAt = expiresAt,
        //            IsActive = isActive,
        //            IsOnboardingCompleted = isOnboardingCompleted,
        //            OnboardingStep = onboardingStep,
        //            Clients = clientsList,
        //            ActiveClient = activeClientDto,
        //            Stores = storeDtos,
        //            ActiveStore = activeStoreDto,
        //            Menus = dynamicMenus,
        //            Roles = roles.Select(r => r.RoleName).ToList(),
        //            Permissions = permissionCodes,
        //            User = new LoginUserInfoDto
        //            {
        //                UserId = user.UserId,
        //                UserKey = user.UserKey,
        //                ClientId = (long)user.ClientId,
        //                ClientKey = client?.ClientKey ?? Guid.Empty,
        //                ClientCode = client?.ClientCode ?? string.Empty,
        //                CompanyName = client?.CompanyName ?? string.Empty,
        //                Email = user.Email,
        //                UserName = user.UserName,
        //                IsActive = isActive,
        //                IsEmailVerified = user.IsEmailVerified,
        //                //IsOnboardingCompleted = isOnboardingCompleted,
        //                //OnboardingStep = onboardingStep,

        //                //Clients = clientsList,
        //                //Stores = storeDtos,
        //                //Menus = dynamicMenus
        //            }
        //        };

        //        return ApiResponse<LoginResponseDto>.Success(responseDto, "Login successful");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResponse<LoginResponseDto>.Fail(500, $"An error occurred during login: {ex.Message}");
        //    }
        //}

        public async Task<ApiResponse<LoginResponseDto>> SwitchStoreAsync(SwitchStoreRequestDto dto)
        {
            try
            {
                //if (UserId <= 0)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(401, "User is not authenticated.");
                //}

                //if (dto == null || dto.StoreId <= 0)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(1, "A valid Store ID is required.");
                //}

                //var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == UserId);
                //if (user == null)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(1, "User not found.");
                //}

                //var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == user.ClientId);
                //if (client == null)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(1, "Client not found.");
                //}

                //var targetStore = await _unitOfWork.StoreRepository.FirstOrDefaultAsync(s => s.StoreId == dto.StoreId && s.ClientId == client.ClientId && s.IsActive);
                //if (targetStore == null)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(1, $"Store with ID {dto.StoreId} was not found or is not active for this client.");
                //}

                //var allStores = await _unitOfWork.StoreRepository.WhereAsync(s => s.ClientId == client.ClientId && s.IsActive);
                //var storeDtos = allStores.Select(MapToStoreResponseDto).ToList();
                //var activeStoreDto = MapToStoreResponseDto(targetStore);

                //var clientsList = new List<UserClientDto> { MapToUserClientDto(client) };
                //var activeClientDto = MapToUserClientDto(client);

                //// Fetch Roles and Permissions
                //var userRoles = await _unitOfWork.UserRoleRepository.WhereAsync(ur => ur.UserId == user.UserId);
                //var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();

                //var roles = new List<Role>();
                //if (roleIds.Any())
                //{
                //    roles = await _unitOfWork.RoleRepository.WhereAsync(r => roleIds.Contains(r.RoleId) && r.IsActive);
                //}

                //var permissions = new List<Permission>();
                //if (roleIds.Any())
                //{
                //    var rolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(rp => roleIds.Contains(rp.RoleId));
                //    var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

                //    if (permissionIds.Any())
                //    {
                //        permissions = await _unitOfWork.PermissionRepository.WhereAsync(p => permissionIds.Contains(p.PermissionId) && p.IsActive);
                //    }
                //}

                //var roleCodes = roles.Select(r => r.RoleCode).Concat(roles.Select(r => r.RoleName)).Distinct().ToList();
                //var permissionCodes = permissions.Select(p => p.PermissionCode).Distinct().ToList();

                //// Dynamic Menus for new Store
                //var dynamicMenus = await _menuService.GetAccessibleMenusForUserAsync(
                //    user.UserId, 
                //    roleCodes, 
                //    permissionCodes, 
                //    client.ClientId, 
                //    targetStore.StoreId
                //);

                //var tokenExpiryMinutes = 60;
                //if (int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int exp) && exp > 0)
                //{
                //    tokenExpiryMinutes = exp;
                //}

                //var expiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);
                //var tokenString = GenerateJwtToken(user, client, targetStore, roles, permissionCodes, expiresAt, client.IsOnboardingCompleted, client.OnboardingStep);

                //var responseDto = new LoginResponseDto
                //{
                //    Token = tokenString,
                //    TokenType = "Bearer",
                //    ExpiresAt = expiresAt,
                //    IsActive = user.IsActive,
                //    IsOnboardingCompleted = client.IsOnboardingCompleted,
                //    OnboardingStep = client.OnboardingStep,
                //    Clients = clientsList,
                //    ActiveClient = activeClientDto,
                //    Stores = storeDtos,
                //    ActiveStore = activeStoreDto,
                //    Menus = dynamicMenus,
                //    Roles = roles.Select(r => r.RoleName).ToList(),
                //    Permissions = permissionCodes,
                //    User = new LoginUserInfoDto
                //    {
                //        UserId = user.UserId,
                //        UserKey = user.UserKey,
                //        ClientId = (long)user.ClientId,
                //        ClientKey = client.ClientKey,
                //        ClientCode = client.ClientCode ?? string.Empty,
                //        CompanyName = client.CompanyName ?? string.Empty,
                //        Email = user.Email,
                //        UserName = user.UserName,
                //        IsActive = user.IsActive,
                //        IsEmailVerified = user.IsEmailVerified,
                //        //IsOnboardingCompleted = client.IsOnboardingCompleted,
                //        //OnboardingStep = client.OnboardingStep,
                //        //Roles = roles.Select(r => r.RoleName).ToList(),
                //        //Permissions = permissionCodes,
                //        //Clients = clientsList,
                //        //Stores = storeDtos,
                //        //Menus = dynamicMenus
                //    }
                //};

                //return ApiResponse<LoginResponseDto>.Success(responseDto, $"Switched to store '{targetStore.StoreName}' successfully.");
                return ApiResponse<LoginResponseDto>.Success(null);
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.Fail(500, $"Error switching store: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> SwitchClientAsync(SwitchClientRequestDto dto)
        {
            try
            {
                //if (UserId <= 0)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(401, "User is not authenticated.");
                //}

                //if (dto == null || dto.ClientId <= 0)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(1, "A valid Client ID is required.");
                //}

                //var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == UserId);
                //if (user == null)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(1, "User not found.");
                //}

                //var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == dto.ClientId && (c.IsActive == null || c.IsActive == true));
                //if (client == null)
                //{
                //    return ApiResponse<LoginResponseDto>.Fail(1, $"Client with ID {dto.ClientId} not found or inactive.");
                //}

                //var allStores = await _unitOfWork.StoreRepository.WhereAsync(s => s.ClientId == client.ClientId && s.IsActive);
                //var storeDtos = allStores.Select(MapToStoreResponseDto).ToList();

                //Store? activeStore = null;
                //if (dto.StoreId.HasValue && dto.StoreId.Value > 0)
                //{
                //    activeStore = allStores.FirstOrDefault(s => s.StoreId == dto.StoreId.Value);
                //}
                //activeStore ??= allStores.FirstOrDefault();

                //var activeStoreDto = activeStore != null ? MapToStoreResponseDto(activeStore) : null;
                //var clientsList = new List<UserClientDto> { MapToUserClientDto(client) };
                //var activeClientDto = MapToUserClientDto(client);

                //// Fetch Roles and Permissions
                //var userRoles = await _unitOfWork.UserRoleRepository.WhereAsync(ur => ur.UserId == user.UserId);
                //var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();

                //var roles = new List<Role>();
                //if (roleIds.Any())
                //{
                //    roles = await _unitOfWork.RoleRepository.WhereAsync(r => roleIds.Contains(r.RoleId) && r.IsActive);
                //}

                //var permissions = new List<Permission>();
                //if (roleIds.Any())
                //{
                //    var rolePermissions = await _unitOfWork.RolePermissionRepository.WhereAsync(rp => roleIds.Contains(rp.RoleId));
                //    var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

                //    if (permissionIds.Any())
                //    {
                //        permissions = await _unitOfWork.PermissionRepository.WhereAsync(p => permissionIds.Contains(p.PermissionId) && p.IsActive);
                //    }
                //}

                //var roleCodes = roles.Select(r => r.RoleCode).Concat(roles.Select(r => r.RoleName)).Distinct().ToList();
                //var permissionCodes = permissions.Select(p => p.PermissionCode).Distinct().ToList();

                //// Dynamic Menus
                //var dynamicMenus = await _menuService.GetAccessibleMenusForUserAsync(
                //    user.UserId, 
                //    roleCodes, 
                //    permissionCodes, 
                //    client.ClientId, 
                //    activeStore?.StoreId
                //);

                //var tokenExpiryMinutes = 60;
                //if (int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int exp) && exp > 0)
                //{
                //    tokenExpiryMinutes = exp;
                //}

                //var expiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);
                //var tokenString = GenerateJwtToken(user, client, activeStore, roles, permissionCodes, expiresAt, client.IsOnboardingCompleted, client.OnboardingStep);

                //var responseDto = new LoginResponseDto
                //{
                //    Token = tokenString,
                //    TokenType = "Bearer",
                //    ExpiresAt = expiresAt,
                //    IsActive = user.IsActive,
                //    IsOnboardingCompleted = client.IsOnboardingCompleted,
                //    OnboardingStep = client.OnboardingStep,
                //    Clients = clientsList,
                //    ActiveClient = activeClientDto,
                //    Stores = storeDtos,
                //    ActiveStore = activeStoreDto,
                //    Menus = dynamicMenus,
                //    Roles = roles.Select(r => r.RoleName).ToList(),
                //    Permissions = permissionCodes,
                //    User = new LoginUserInfoDto
                //    {
                //        UserId = user.UserId,
                //        UserKey = user.UserKey,
                //        ClientId = (long)user.ClientId,
                //        ClientKey = client.ClientKey,
                //        ClientCode = client.ClientCode ?? string.Empty,
                //        CompanyName = client.CompanyName ?? string.Empty,
                //        Email = user.Email,
                //        UserName = user.UserName,
                //        IsActive = user.IsActive,
                //        IsEmailVerified = user.IsEmailVerified,
                //        //IsOnboardingCompleted = client.IsOnboardingCompleted,
                //        //OnboardingStep = client.OnboardingStep,
                //        //Roles = roles.Select(r => r.RoleName).ToList(),
                //        //Permissions = permissionCodes,
                //        //Clients = clientsList,
                //        //Stores = storeDtos,
                //        //Menus = dynamicMenus
                //    }
                //};

                //return ApiResponse<LoginResponseDto>.Success(responseDto, $"Switched to client '{client.CompanyName}' successfully.");
                return ApiResponse<LoginResponseDto>.Success(null);
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.Fail(500, $"Error switching client: {ex.Message}");
            }
        }

        #endregion

        #region Helpers

        private string GenerateJwtToken(UserResponseDto user, UserClientDto? client, StoreResponseDto? activeStore, string expiryMinutes, List<RoleResponseDto> roles, IEnumerable<string> permissionCodes)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(Global.Claim_Types.UserId, user.UserId.ToString()),
                new Claim(Global.Claim_Types.ClientId, user.ClientId.ToString()),
                new Claim(Global.Claim_Types.ClientKey, client?.ClientKey.ToString() ?? Guid.Empty.ToString()),
                new Claim(Global.Claim_Types.UserName, user.UserName),
                new Claim(Global.Claim_Types.RoleIdKey, roles.FirstOrDefault()?.RoleId.ToString() ?? "0"),
                new Claim(Global.Claim_Types.StoreId, activeStore?.StoreId.ToString() ?? "0"),
                new Claim(Global.Claim_Types.StoreKey, activeStore?.StoreKey.ToString() ?? Guid.Empty.ToString()),
                new Claim(Global.Claim_Types.IsOnboardingCompleted, client.IsOnboardingCompleted.ToString()),
                new Claim(Global.Claim_Types.OnboardingStep, client.OnboardingStep.ToString()),
                new Claim(Global.Claim_Types.IsActive, client.IsActive.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                claims.Add(new Claim(Global.Claim_Types.RoleName, role.RoleName));
            }

            foreach (var permissionCode in permissionCodes)
            {
                claims.Add(new Claim(Global.Claim_Types.Permission, permissionCode));
            }
            var expiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiryMinutes));
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static UserClientDto MapToUserClientDto(Client c)
        {
            return new UserClientDto
            {
                ClientId = c.ClientId,
                ClientKey = c.ClientKey,
                ClientCode = c.ClientCode ?? string.Empty,
                CompanyName = c.CompanyName ?? string.Empty,
                ClientName = c.ClientName ?? string.Empty,
                Email = c.Email,
                //Phone = c.Phone,
                IsActive = c.IsActive ?? true,
                IsOnboardingCompleted = c.IsOnboardingCompleted,
                OnboardingStep = c.OnboardingStep
            };
        }

        private static StoreResponseDto MapToStoreResponseDto(Store s)
        {
            return new StoreResponseDto
            {
                StoreId = s.StoreId,
                StoreKey = s.StoreKey,
                ClientId = s.ClientId,
                StoreCode = s.StoreCode,
                StoreName = s.StoreName,
                StoreType = s.StoreType,
                OwnerName = s.OwnerName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                AlternatePhoneNumber = s.AlternatePhoneNumber,
                GSTNumber = s.Gstnumber,
                DrugLicenseNumber = s.DrugLicenseNumber,
                AddressLine1 = s.AddressLine1,
                AddressLine2 = s.AddressLine2,
                CityId = s.CityId,
                PostalCode = s.PostalCode,
                IsActive = s.IsActive
            };
        }

        #endregion

        #region Password Reset

        public async Task<ApiResponse<bool>> ResetPassword(string userEmail, string password)
        {
            try
            {
                var dbResult = await _unitOfWork
                    .UserRepository
                    .FirstOrDefaultAsync(x => x.Email == userEmail && x.IsActive == true);

                if (dbResult == null)
                {
                    return ApiResponse<bool>.Fail(1, "Invalid user");
                }
                var pwdResult = PasswordHelper.HashPassword(password);

                dbResult.PasswordHash = pwdResult.hash;
                dbResult.UserSalt = pwdResult.salt;

                _unitOfWork.UserRepository.Update(dbResult);

                bool save = await _unitOfWork.SaveChangesAsync();

                if (!save)
                {
                    return ApiResponse<bool>.Fail(1, "Password update failed");
                }
                return ApiResponse<bool>.Success(true, "Password updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(1, ex.Message);
            }
        }

        #endregion


        public async Task<ApiResponse<string>> PlateformUserAsync(PlateformRegister dto)
        {
            int err_no = 0;
            string err_msg = string.Empty;

            try
            {
                var pwdResult = PasswordHelper.HashPassword(dto.Password);

                var param = new List<SqlParameter>
                {
                    new SqlParameter("@UserName", dto.UserName),
                    new SqlParameter("@Email", dto.Email),
                    new SqlParameter("@HashPassword", pwdResult.hash),
                    new SqlParameter("@UserSalt", pwdResult.salt),
                    new SqlParameter("@CreatedBy", Global.InternalUser),
                    new SqlParameter("@ErrNumber", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    },
                    new SqlParameter("@ErrMsg", SqlDbType.VarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    }
                };
                var result = ExecuteStoredProcedure(StoredProcedure.sp_PlateformRegister, param, _unitOfWork.GetConnection());
                err_no = param.First(p => p.ParameterName == "@ErrNumber").Value != DBNull.Value
                    ? Convert.ToInt32(param.First(p => p.ParameterName == "@ErrNumber").Value) : 0;
                err_msg = param.First(p => p.ParameterName == "@ErrMsg").Value?.ToString() ?? "";

                if (err_no != 0)
                    return ApiResponse<string>.Fail(err_no, err_msg);

                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (user != null)
                {
                    try
                    {
                        var otpResult = await _otpService.SendOtpAsync(dto.Email);
                        if (!otpResult.IsSuccess)
                        {
                            return ApiResponse<string>.Success(null, "User successful. OTP sending failed, please try forget-password to request OTP");
                        }

                        return ApiResponse<string>.Success(null, "User successful. OTP sent to your email");
                    }
                    catch (Exception otpEx)
                    {
                        return ApiResponse<string>.Success(null, $"User successful. Note: {otpEx.Message}");
                    }
                }
                else
                {
                    return ApiResponse<string>.Success(null, "Signup successful. Unable to send OTP - user not found");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail(500, ex.Message);
            }
        }

       
    }
}
