using MediStock360.Application.Common;
using MediStock360.Application.Common.constaints;
using MediStock360.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MediStock360.Application.Common.constaints.ProductOwner;

namespace MediStock360.Infrastructure.Persistence
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly MedicalDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            MedicalDbContext dbContext,
            IConfiguration configuration,
            ILogger<DatabaseSeeder> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Verifying and seeding initial Product Owner, User, and Client data...");

                // 1. Read Product Owner user details from appsettings.json
                var userSection = _configuration.GetSection("ProductOwnerUser");
                if (!userSection.Exists())
                {
                    userSection = _configuration.GetSection("SeedData:ProductOwnerUser");
                }

                var userSettings = new ProductOwnerUserSeedSettings
                {
                    UserName = userSection["UserName"] ?? ProductOwner.ProductOwnerUserSeed.UserName,
                    Email = userSection["Email"] ?? ProductOwner.ProductOwnerUserSeed.Email,
                    Password = userSection["Password"] ?? ProductOwner.ProductOwnerUserSeed.Password,
                    FirstName = userSection["FirstName"] ?? ProductOwner.ProductOwnerUserSeed.FirstName,
                    LastName = userSection["LastName"] ?? ProductOwner.ProductOwnerUserSeed.LastName,
                    PhoneNumber = userSection["PhoneNumber"] ?? ProductOwner.ProductOwnerUserSeed.PhoneNumber
                };

                // 2. Read Internal Client details (derived from user details if not explicitly specified)
                var clientSection = _configuration.GetSection("InternalClient");
                if (!clientSection.Exists())
                {
                    clientSection = _configuration.GetSection("SeedData:InternalClient");
                }

                var clientSettings = new InternalClientSeedSettings
                {
                    ClientId = Convert.ToInt16(clientSection["ClintId"]) ,
                    ClientCode = clientSection["ClientCode"] ?? ProductOwner.InternalClientSeed.ClientCode,
                    ClientName = clientSection["ClientName"] ?? ProductOwner.InternalClientSeed.ClientName,
                    CompanyName = clientSection["CompanyName"] ?? ProductOwner.InternalClientSeed.CompanyName,
                    OwnerName = clientSection["OwnerName"],
                    Email = clientSection["Email"],
                    Phone = clientSection["Phone"],
                    Address = clientSection["Address"] ?? ProductOwner.InternalClientSeed.Address,
                    PostalCode = clientSection["PostalCode"] ?? ProductOwner.InternalClientSeed.PostalCode
                };
                var clientId = clientSettings.ClientId;
                var clientCode = string.IsNullOrWhiteSpace(clientSettings.ClientCode) ? ProductOwner.InternalClientSeed.ClientCode : clientSettings.ClientCode.Trim();
                var companyName = string.IsNullOrWhiteSpace(clientSettings.CompanyName) ? ProductOwner.InternalClientSeed.CompanyName : clientSettings.CompanyName.Trim();
                var clientName = string.IsNullOrWhiteSpace(clientSettings.ClientName) ? ProductOwner.InternalClientSeed.ClientName : clientSettings.ClientName.Trim();
                var ownerName = string.IsNullOrWhiteSpace(clientSettings.OwnerName) ? $"{userSettings.FirstName} {userSettings.LastName}".Trim() : clientSettings.OwnerName.Trim();
                var clientEmail = string.IsNullOrWhiteSpace(clientSettings.Email) ? userSettings.Email.Trim() : clientSettings.Email.Trim();
                var clientPhone = string.IsNullOrWhiteSpace(clientSettings.Phone) ? userSettings.PhoneNumber.Trim() : clientSettings.Phone.Trim();

                // 3. Find or Create Internal Client
                var internalClient = await _dbContext.Clients.FirstOrDefaultAsync(c =>
                    c.ClientCode == clientCode ||
                    c.CompanyName == companyName ||
                    (c.ClientName != null && c.ClientName == clientName));

                if (internalClient == null)
                {
                    _logger.LogInformation("Creating Internal Client '{CompanyName}' (Code: {ClientCode})...", companyName, clientCode);

                    internalClient = new Client
                    {
                        ClientId= clientId,
                        ClientKey = Guid.NewGuid(),
                        ClientCode = clientCode,
                        ClientName = clientName,
                        CompanyName = companyName,
                        OwnerName = ownerName,
                        Email = clientEmail,
                        Phone = clientPhone,
                        Address = clientSettings.Address ?? "Headquarters, Main Campus",
                        PostalCode = clientSettings.PostalCode ?? "10001",
                        IsOnboardingCompleted = true,
                        OnboardingStep = 1,
                        IsActive = true,
                        IsSynced = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Global.InternalUser
                    };

                    _dbContext.Clients.Add(internalClient);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Internal Client created with ID: {ClientId}", internalClient.ClientId);
                }
                else
                {
                    _logger.LogInformation("Existing Internal Client found with ID: {ClientId}", internalClient.ClientId);
                }

                // 4. Find or Create 'PRODUCT_OWNER' Role
                var productOwnerRole = await _dbContext.Roles.FirstOrDefaultAsync(r =>
                    r.ClientId == internalClient.ClientId &&
                    (r.RoleCode == "PRODUCT_OWNER" || r.RoleCode == "Product_Owner" || r.RoleName == "Product Owner" || r.RoleName == "Product_Owner"));

                if (productOwnerRole == null)
                {
                    _logger.LogInformation("Creating 'PRODUCT_OWNER' role for Internal Client ID: {ClientId}...", internalClient.ClientId);

                    productOwnerRole = new Role
                    {
                        ClientId = internalClient.ClientId,
                        RoleCode = ProductOwnerRole.RoleCode,
                        RoleName = ProductOwnerRole.RoleName,
                        Description = ProductOwnerRole.Description,
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Global.InternalUser
                    };

                    _dbContext.Roles.Add(productOwnerRole);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("'PRODUCT_OWNER' role created with ID: {RoleId}", productOwnerRole.RoleId);
                }
                else
                {
                    _logger.LogInformation("Existing 'PRODUCT_OWNER' role found with ID: {RoleId}", productOwnerRole.RoleId);
                }

                // 5. Find or Create Product Owner User from appsettings.json
                var userName = userSettings.UserName.Trim();
                var userEmail = userSettings.Email.Trim();

                var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                    u.UserName.ToLower() == userName.ToLower() ||
                    u.Email.ToLower() == userEmail.ToLower());

                if (user == null)
                {
                    _logger.LogInformation("Creating Product Owner user '{UserName}' ({Email})...", userName, userEmail);

                    var (pwdHash, pwdSalt) = PasswordHelper.HashPassword(userSettings.Password);

                    user = new User
                    {
                        ClientId = internalClient.ClientId,
                        UserKey = Guid.NewGuid(),
                        UserName = userName,
                        Email = userEmail,
                        PasswordHash = pwdHash,
                        UserSalt = pwdSalt,
                        IsEmailVerified = true,
                        IsLocked = false,
                        IsActive = true,
                        IsSynced = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Global.InternalUser
                    };

                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Product Owner user created with ID: {UserId}", user.UserId);
                }
                else
                {
                    _logger.LogInformation("Existing user found with ID: {UserId}. Ensuring active status...", user.UserId);
                    user.IsActive = true;
                    user.IsLocked = false;
                    user.IsEmailVerified = true;
                    await _dbContext.SaveChangesAsync();
                }

                // 6. Find or Create UserProfile for the User
                var userProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.UserId);
                if (userProfile == null)
                {
                    _logger.LogInformation("Creating UserProfile for User ID: {UserId}...", user.UserId);

                    var profilePhone = string.IsNullOrWhiteSpace(userSettings.PhoneNumber) ? "+919876543210" : userSettings.PhoneNumber.Trim();

                    // Check if phone number is already taken to ensure uniqueness constraint
                    var existingPhoneProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.PhoneNumber == profilePhone);
                    if (existingPhoneProfile != null && existingPhoneProfile.UserId != user.UserId)
                    {
                        profilePhone = $"+91987654{new Random().Next(1000, 9999)}";
                    }

                    userProfile = new UserProfile
                    {
                        ClientId = internalClient.ClientId,
                        UserId = user.UserId,
                        FirstName = string.IsNullOrWhiteSpace(userSettings.FirstName) ? "Product" : userSettings.FirstName.Trim(),
                        LastName = string.IsNullOrWhiteSpace(userSettings.LastName) ? "Owner" : userSettings.LastName.Trim(),
                        DisplayName = $"{userSettings.FirstName} {userSettings.LastName}".Trim(),
                        PhoneNumber = profilePhone,
                        IsPhoneVerified = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    _dbContext.UserProfiles.Add(userProfile);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("UserProfile created for User ID: {UserId}", user.UserId);
                }

                // 7. Find or Create UserRole mapping (User -> Role)
                var userRole = await _dbContext.UserRoles.FirstOrDefaultAsync(ur =>
                    ur.UserId == user.UserId && ur.RoleId == productOwnerRole.RoleId);

                if (userRole == null)
                {
                    _logger.LogInformation("Mapping User ID: {UserId} to Role ID: {RoleId}...", user.UserId, productOwnerRole.RoleId);

                    userRole = new UserRole
                    {
                        ClientId = internalClient.ClientId,
                        UserId = user.UserId,
                        RoleId = productOwnerRole.RoleId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Global.InternalUser
                    };

                    _dbContext.UserRoles.Add(userRole);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("UserRole mapped successfully.");
                }

                // 8. Find or Create Default Internal Store & StoreUserMap for Login procedure
                //var internalStore = await _dbContext.Stores.FirstOrDefaultAsync(s =>
                //    s.ClientId == internalClient.ClientId &&
                //    (s.StoreCode == "INTERNAL_HQ" || s.StoreName == "Headquarters Store"));

                //if (internalStore == null)
                //{
                //    _logger.LogInformation("Creating Default Internal Store for Client ID: {ClientId}...", internalClient.ClientId);

                //    internalStore = new Store
                //    {
                //        ClientId = internalClient.ClientId,
                //        StoreKey = Guid.NewGuid(),
                //        StoreCode = "INTERNAL_HQ",
                //        StoreName = "Headquarters Store",
                //        StoreType = 1,
                //        IsActive = true,
                //        CreatedAt = DateTime.UtcNow,
                //        CreatedBy = Global.InternalUser
                //    };

                //    _dbContext.Stores.Add(internalStore);
                //    await _dbContext.SaveChangesAsync();
                //    _logger.LogInformation("Internal Store created with ID: {StoreId}", internalStore.StoreId);
                //}

                //var storeUserMap = await _dbContext.StoreUserMaps.FirstOrDefaultAsync(m =>
                //    m.StoreId == internalStore.StoreId && m.UserId == user.UserId);

                //if (storeUserMap == null)
                //{
                //    _logger.LogInformation("Mapping User ID: {UserId} to Store ID: {StoreId}...", user.UserId, internalStore.StoreId);

                //    storeUserMap = new StoreUserMap
                //    {
                //        ClientId = internalClient.ClientId,
                //        StoreId = internalStore.StoreId,
                //        UserId = user.UserId,
                //        IsDefaultStore = true,
                //        IsActive = true,
                //        CreatedAt = DateTime.UtcNow
                //    };

                //    _dbContext.StoreUserMaps.Add(storeUserMap);
                //    await _dbContext.SaveChangesAsync();
                //    _logger.LogInformation("StoreUserMap created successfully.");
                //}

                // 9. Assign system permissions to Product Owner role
                var allPermissions = await _dbContext.Permissions.Where(p => p.IsActive == true).ToListAsync();
                if (allPermissions.Any())
                {
                    var existingRolePermissionIds = await _dbContext.RolePermissions
                        .Where(rp => rp.RoleId == productOwnerRole.RoleId)
                        .Select(rp => rp.PermissionId)
                        .ToListAsync();

                    var missingPermissions = allPermissions
                        .Where(p => !existingRolePermissionIds.Contains(p.PermissionId))
                        .Select(p => new RolePermission
                        {
                            RoleId = productOwnerRole.RoleId,
                            PermissionId = p.PermissionId,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = Global.InternalUser
                        })
                        .ToList();

                    if (missingPermissions.Any())
                    {
                        _logger.LogInformation("Assigning {Count} system permissions to 'PRODUCT_OWNER' role...", missingPermissions.Count);
                        await _dbContext.RolePermissions.AddRangeAsync(missingPermissions);
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation("Permissions assigned successfully.");
                    }
                }

                _logger.LogInformation("Product Owner seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database seeding: {Message}", ex.Message);
            }
        }
    }
}
