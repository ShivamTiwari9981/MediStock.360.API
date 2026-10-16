using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediStock360.Application.Common;
using MediStock360.Application.DTOs.RequestDto;
using MediStock360.Application.DTOs.ResponseDto;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure;

namespace MediStock360.Application.Services
{
    public class ProductOwnerService : BaseService, IProductOwnerService
    {
        private readonly IPaymentService _paymentService;

        public ProductOwnerService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IPaymentService paymentService) : base(unitOfWork, currentUserService)
        {
            _paymentService = paymentService;
        }

        public async Task<ApiResponse<ProductOwnerDashboardDto>> GetDashboardStatsAsync()
        {
            try
            {
                var clients = await _unitOfWork.ClientRepository.GetAllAsync();
                var stores = await _unitOfWork.StoreRepository.GetAllAsync();
                var payments = await _unitOfWork.ClientPaymentRepository.GetAllAsync();
                var subscriptions = await _unitOfWork.ClientSubscriptionRepository.GetAllAsync();
                var plans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();

                var clientList = clients.ToList();
                var storeList = stores.ToList();
                var paymentList = payments.ToList();
                var subscriptionList = subscriptions.ToList();
                var planList = plans.ToList();

                var approvedPayments = paymentList.Where(p => p.PaymentStatus == 2).ToList();
                var pendingPayments = paymentList.Where(p => p.PaymentStatus == 1).ToList();

                var dashboardDto = new ProductOwnerDashboardDto
                {
                    TotalClients = clientList.Count,
                    ActiveClients = clientList.Count(c => c.IsActive == true),
                    PendingClients = clientList.Count(c => c.IsActive != true),
                    InactiveClients = clientList.Count(c => c.IsActive == false && c.IsOnboardingCompleted),
                    TotalStores = storeList.Count,
                    ActiveStores = storeList.Count(s => s.IsActive),
                    PendingApprovals = pendingPayments.Count,
                    ApprovedPayments = approvedPayments.Count,
                    TotalRevenue = approvedPayments.Sum(p => p.Amount),
                    RecentClients = clientList
                        .OrderByDescending(c => c.CreatedAt)
                        .Take(5)
                        .Select(c =>
                        {
                            var sub = subscriptionList.Where(s => s.ClientId == c.ClientId).OrderByDescending(s => s.CreatedAt).FirstOrDefault();
                            var plan = sub != null ? planList.FirstOrDefault(p => p.SubscriptionPlanId == sub.SubscriptionPlanId) : null;
                            return new ProductOwnerClientDto
                            {
                                ClientId = c.ClientId,
                                ClientKey = c.ClientKey,
                                ClientCode = c.ClientCode,
                                ClientName = c.ClientName,
                                CompanyName = c.CompanyName,
                                OwnerName = c.OwnerName,
                                Email = c.Email,
                                Phone = c.Phone,
                                IsActive = c.IsActive == true,
                                IsOnboardingCompleted = c.IsOnboardingCompleted,
                                OnboardingStep = c.OnboardingStep,
                                CreatedAt = c.CreatedAt,
                                StoreCount = storeList.Count(s => s.ClientId == c.ClientId),
                                CurrentPlanName = plan?.PlanName ?? "Free Trial",
                                SubscriptionStatusText = sub?.Status == 2 ? "Active" : (sub?.Status == 1 ? "Pending" : "Inactive")
                            };
                        }).ToList(),
                    RecentApprovals = pendingPayments
                        .OrderByDescending(p => p.SubmittedAt)
                        .Take(5)
                        .Select(p =>
                        {
                            var client = clientList.FirstOrDefault(c => c.ClientId == p.ClientId);
                            var sub = subscriptionList.FirstOrDefault(s => s.ClientSubscriptionId == p.ClientSubscriptionId);
                            var plan = sub != null ? planList.FirstOrDefault(pl => pl.SubscriptionPlanId == sub.SubscriptionPlanId) : null;
                            return new PendingApprovalDto
                            {
                                ClientPaymentId = p.ClientPaymentId,
                                ClientId = p.ClientId,
                                ClientKey = client?.ClientKey ?? Guid.Empty,
                                ClientName = client?.ClientName ?? "N/A",
                                CompanyName = client?.CompanyName ?? "N/A",
                                ClientEmail = client?.Email,
                                ClientPhone = client?.Phone,
                                ClientSubscriptionId = p.ClientSubscriptionId,
                                PlanName = plan?.PlanName ?? "Subscription",
                                BillingCycle = sub?.BillingCycle == 2 ? "Yearly" : "Monthly",
                                Amount = p.Amount,
                                CurrencyCode = p.CurrencyCode,
                                TransactionReference = p.TransactionReference,
                                SenderNotes = p.SenderNotes,
                                PaymentStatus = p.PaymentStatus,
                                PaymentStatusText = "Pending Approval",
                                SubmittedAt = p.SubmittedAt
                            };
                        }).ToList()
                };

                return ApiResponse<ProductOwnerDashboardDto>.Success(dashboardDto, "Product Owner dashboard statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductOwnerDashboardDto>.Fail(500, $"Error retrieving Product Owner dashboard stats: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductOwnerClientDto>>> GetAllClientsAsync(string? searchTerm = null, bool? isActive = null)
        {
            try
            {
                var clients = await _unitOfWork.ClientRepository.GetAllAsync();
                var stores = await _unitOfWork.StoreRepository.GetAllAsync();
                var subscriptions = await _unitOfWork.ClientSubscriptionRepository.GetAllAsync();
                var plans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();
                var cities = await _unitOfWork.CityRepository.GetAllAsync();
                var states = await _unitOfWork.StateRepository.GetAllAsync();
                var businessTypes = await _unitOfWork.BusinessTypeRepository.GetAllAsync();

                var clientQuery = clients.AsQueryable();

                if (isActive.HasValue)
                {
                    clientQuery = clientQuery.Where(c => (c.IsActive ?? false) == isActive.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    clientQuery = clientQuery.Where(c =>
                        (c.CompanyName != null && c.CompanyName.ToLower().Contains(term)) ||
                        (c.ClientName != null && c.ClientName.ToLower().Contains(term)) ||
                        (c.ClientCode != null && c.ClientCode.ToLower().Contains(term)) ||
                        (c.Email != null && c.Email.ToLower().Contains(term)) ||
                        (c.Phone != null && c.Phone.ToLower().Contains(term))
                    );
                }

                var clientList = clientQuery.OrderByDescending(c => c.CreatedAt).ToList();
                var storeList = stores.ToList();
                var subscriptionList = subscriptions.ToList();
                var planList = plans.ToList();
                var cityList = cities.ToList();
                var stateList = states.ToList();
                var bTypeList = businessTypes.ToList();

                var result = clientList.Select(c =>
                {
                    var sub = subscriptionList.Where(s => s.ClientId == c.ClientId).OrderByDescending(s => s.CreatedAt).FirstOrDefault();
                    var plan = sub != null ? planList.FirstOrDefault(p => p.SubscriptionPlanId == sub.SubscriptionPlanId) : null;
                    var city = c.CityId.HasValue ? cityList.FirstOrDefault(ci => ci.CityId == c.CityId.Value) : null;
                    var state = c.StateId.HasValue ? stateList.FirstOrDefault(st => st.StateId == c.StateId.Value) : null;
                    var bType = c.BusinessTypeId.HasValue ? bTypeList.FirstOrDefault(bt => bt.BusinessTypeId == c.BusinessTypeId.Value) : null;

                    string subStatusText = "No Subscription";
                    if (sub != null)
                    {
                        subStatusText = sub.Status switch
                        {
                            1 => "Pending",
                            2 => "Active",
                            3 => "Expired",
                            _ => "Unknown"
                        };
                    }

                    string paymentStatusText = "N/A";
                    if (sub != null)
                    {
                        paymentStatusText = sub.PaymentStatus switch
                        {
                            1 => "Pending Approval",
                            2 => "Paid",
                            3 => "Rejected",
                            _ => "Pending"
                        };
                    }

                    return new ProductOwnerClientDto
                    {
                        ClientId = c.ClientId,
                        ClientKey = c.ClientKey,
                        ClientCode = c.ClientCode,
                        ClientName = c.ClientName,
                        CompanyName = c.CompanyName,
                        OwnerName = c.OwnerName,
                        BusinessTypeId = c.BusinessTypeId,
                        BusinessTypeName = bType?.BusinessTypeName,
                        Email = c.Email,
                        Phone = c.Phone,
                        Gstnumber = c.Gstnumber,
                        DrugLicenseNumber = c.DrugLicenseNumber,
                        Address = c.Address,
                        CityId = c.CityId,
                        CityName = city?.CityName,
                        StateId = c.StateId,
                        StateName = state?.StateName,
                        CountryId = c.CountryId,
                        PostalCode = c.PostalCode,
                        IsOnboardingCompleted = c.IsOnboardingCompleted,
                        OnboardingStep = c.OnboardingStep,
                        IsActive = c.IsActive == true,
                        CreatedAt = c.CreatedAt,
                        StoreCount = storeList.Count(s => s.ClientId == c.ClientId),
                        CurrentPlanName = plan?.PlanName ?? "None",
                        SubscriptionStatus = sub?.Status,
                        SubscriptionStatusText = subStatusText,
                        SubscriptionEndDate = sub?.EndDate,
                        PaymentStatus = sub?.PaymentStatus,
                        PaymentStatusText = paymentStatusText
                    };
                }).ToList();

                return ApiResponse<List<ProductOwnerClientDto>>.Success(result, $"Fetched {result.Count} clients successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductOwnerClientDto>>.Fail(500, $"Error retrieving clients: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductOwnerClientDto>> GetClientDetailsAsync(long clientId)
        {
            try
            {
                var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == clientId);
                if (client == null)
                {
                    return ApiResponse<ProductOwnerClientDto>.Fail(404, $"Client with ID {clientId} not found.");
                }

                var stores = await _unitOfWork.StoreRepository.WhereAsync(s => s.ClientId == clientId);
                var subscriptions = await _unitOfWork.ClientSubscriptionRepository.WhereAsync(s => s.ClientId == clientId);
                var latestSub = subscriptions.OrderByDescending(s => s.CreatedAt).FirstOrDefault();

                SubscriptionPlan? plan = null;
                if (latestSub != null)
                {
                    plan = await _unitOfWork.SubscriptionPlanRepository.FirstOrDefaultAsync(p => p.SubscriptionPlanId == latestSub.SubscriptionPlanId);
                }

                City? city = null;
                if (client.CityId.HasValue)
                {
                    city = await _unitOfWork.CityRepository.FirstOrDefaultAsync(ci => ci.CityId == client.CityId.Value);
                }

                State? state = null;
                if (client.StateId.HasValue)
                {
                    state = await _unitOfWork.StateRepository.FirstOrDefaultAsync(st => st.StateId == client.StateId.Value);
                }

                BusinessType? bType = null;
                if (client.BusinessTypeId.HasValue)
                {
                    bType = await _unitOfWork.BusinessTypeRepository.FirstOrDefaultAsync(bt => bt.BusinessTypeId == client.BusinessTypeId.Value);
                }

                var dto = new ProductOwnerClientDto
                {
                    ClientId = client.ClientId,
                    ClientKey = client.ClientKey,
                    ClientCode = client.ClientCode,
                    ClientName = client.ClientName,
                    CompanyName = client.CompanyName,
                    OwnerName = client.OwnerName,
                    BusinessTypeId = client.BusinessTypeId,
                    BusinessTypeName = bType?.BusinessTypeName,
                    Email = client.Email,
                    Phone = client.Phone,
                    Gstnumber = client.Gstnumber,
                    DrugLicenseNumber = client.DrugLicenseNumber,
                    Address = client.Address,
                    CityId = client.CityId,
                    CityName = city?.CityName,
                    StateId = client.StateId,
                    StateName = state?.StateName,
                    CountryId = client.CountryId,
                    PostalCode = client.PostalCode,
                    IsOnboardingCompleted = client.IsOnboardingCompleted,
                    OnboardingStep = client.OnboardingStep,
                    IsActive = client.IsActive == true,
                    CreatedAt = client.CreatedAt,
                    StoreCount = stores.Count(),
                    CurrentPlanName = plan?.PlanName ?? "None",
                    SubscriptionStatus = latestSub?.Status,
                    SubscriptionStatusText = latestSub?.Status == 2 ? "Active" : "Inactive",
                    SubscriptionEndDate = latestSub?.EndDate,
                    PaymentStatus = latestSub?.PaymentStatus,
                    PaymentStatusText = latestSub?.PaymentStatus == 2 ? "Paid" : "Pending Approval"
                };

                return ApiResponse<ProductOwnerClientDto>.Success(dto, "Client details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductOwnerClientDto>.Fail(500, $"Error retrieving client details: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductOwnerClientDto>> CreateClientAsync(CreateClientByPoDto dto)
        {
            try
            {
                // 1. Validation - uniqueness
                var existingCompany = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.CompanyName.ToLower() == dto.CompanyName.Trim().ToLower());
                if (existingCompany != null)
                {
                    return ApiResponse<ProductOwnerClientDto>.Fail(400, "A client with this company name already exists.");
                }

                var existingUser = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.Trim().ToLower());
                if (existingUser != null)
                {
                    return ApiResponse<ProductOwnerClientDto>.Fail(400, "A user with this email address already exists.");
                }

                // 2. Generate Client Code
                var allClients = await _unitOfWork.ClientRepository.GetAllAsync();
                var totalClientsCount = allClients.Count;
                string clientCode = $"CLI{(totalClientsCount + 101):D4}";

                // 3. Create Client Record
                var newClient = new Client
                {
                    ClientKey = Guid.NewGuid(),
                    ClientCode = clientCode,
                    ClientName = dto.ClientName.Trim(),
                    CompanyName = dto.CompanyName.Trim(),
                    OwnerName = dto.OwnerName.Trim(),
                    Email = dto.Email.Trim(),
                    Phone = dto.Phone.Trim(),
                    BusinessTypeId = dto.BusinessTypeId,
                    Gstnumber = dto.Gstnumber?.Trim(),
                    DrugLicenseNumber = dto.DrugLicenseNumber?.Trim(),
                    Address = dto.Address?.Trim(),
                    CityId = dto.CityId,
                    StateId = dto.StateId,
                    CountryId = dto.CountryId,
                    PostalCode = dto.PostalCode?.Trim(),
                    IsOnboardingCompleted = true,
                    OnboardingStep = 4,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = UserId != 0 ? UserId : null
                };

                await _unitOfWork.ClientRepository.AddAsync(newClient);
                await _unitOfWork.SaveChangesAsync();

                // 4. Create Initial Store for this client
                var store = new Store
                {
                    StoreKey = Guid.NewGuid(),
                    ClientId = newClient.ClientId,
                    StoreCode = "STR-001",
                    StoreName = !string.IsNullOrWhiteSpace(dto.InitialStoreName)
                        ? dto.InitialStoreName.Trim()
                        : $"{dto.CompanyName} - Main Store",
                    StoreType = 1,
                    PhoneNumber = dto.Phone.Trim(),
                    DrugLicenseNumber = dto.DrugLicenseNumber?.Trim(),
                    AddressLine1 = dto.Address?.Trim(),
                    CityId = dto.CityId,
                    StateId = dto.StateId,
                    CountryId = dto.CountryId,
                    PostalCode = dto.PostalCode?.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = UserId != 0 ? UserId : null
                };
                await _unitOfWork.StoreRepository.AddAsync(store);

                // 5. Create Client Administrator User
                string rawPassword = !string.IsNullOrWhiteSpace(dto.Password) ? dto.Password : "Admin@123";
                var pwd = PasswordHelper.HashPassword(rawPassword);

                var user = new User
                {
                    UserKey = Guid.NewGuid(),
                    ClientId = newClient.ClientId,
                    UserName = dto.Email.Split('@')[0].Trim(),
                    Email = dto.Email.Trim(),
                    PasswordHash = pwd.hash,
                    UserSalt = pwd.salt,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = UserId != 0 ? UserId : null
                };
                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // 6. Map User to Store
                var storeUserMap = new StoreUserMap
                {
                    UserId = user.UserId,
                    StoreId = store.StoreId,
                    ClientId = newClient.ClientId,
                    IsDefaultStore = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.StoreUserMapRepository.AddAsync(storeUserMap);

                // 7. Assign CLIENT_OWNER role
                var existingRole = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.RoleCode == "CLIENT_OWNER" && (r.ClientId == newClient.ClientId || r.ClientId == 0));
                if (existingRole == null)
                {
                    // Create default role for client
                    existingRole = new Role
                    {
                        ClientId = newClient.ClientId,
                        RoleCode = "CLIENT_OWNER",
                        RoleName = "Client Owner",
                        Description = "Primary owner of the client account",
                        IsSystemRole = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = UserId != 0 ? UserId : null
                    };
                    await _unitOfWork.RoleRepository.AddAsync(existingRole);
                    await _unitOfWork.SaveChangesAsync();
                }

                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = existingRole.RoleId,
                    ClientId = newClient.ClientId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = UserId != 0 ? UserId : null
                };
                await _unitOfWork.UserRoleRepository.AddAsync(userRole);

                // 8. Assign Subscription Plan
                int planId = dto.SubscriptionPlanId.HasValue && dto.SubscriptionPlanId.Value > 0
                    ? dto.SubscriptionPlanId.Value
                    : 1;

                var plan = await _unitOfWork.SubscriptionPlanRepository.FirstOrDefaultAsync(p => p.SubscriptionPlanId == planId);

                var subscription = new ClientSubscription
                {
                    ClientId = newClient.ClientId,
                    SubscriptionPlanId = planId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1),
                    Status = 2, // Active
                    BillingCycle = 2, // Yearly
                    Amount = plan?.Price ?? 0,
                    CurrencyCode = "INR",
                    AutoRenew = true,
                    PaymentStatus = 2, // Paid
                    TransactionReference = $"PO-GEN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    IsTrial = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = UserId != 0 ? UserId : null
                };
                await _unitOfWork.ClientSubscriptionRepository.AddAsync(subscription);

                // 9. Initialize Default Client App Settings configured by Product Owner
                await SaveOrUpdateSettingKeyAsync(newClient.ClientId, "MAX_STORES", dto.MaxStores.ToString());
                await SaveOrUpdateSettingKeyAsync(newClient.ClientId, "MAX_USERS", dto.MaxUsers.ToString());
                await SaveOrUpdateSettingKeyAsync(newClient.ClientId, "INVOICE_PREFIX", "INV-");
                await SaveOrUpdateSettingKeyAsync(newClient.ClientId, "EXPIRY_ALERT_DAYS", "60");
                await SaveOrUpdateSettingKeyAsync(newClient.ClientId, "MULTI_BRANCH_ENABLED", dto.MaxStores > 1 ? "true" : "false");

                await _unitOfWork.SaveChangesAsync();

                var resultDto = new ProductOwnerClientDto
                {
                    ClientId = newClient.ClientId,
                    ClientKey = newClient.ClientKey,
                    ClientCode = newClient.ClientCode,
                    ClientName = newClient.ClientName,
                    CompanyName = newClient.CompanyName,
                    OwnerName = newClient.OwnerName,
                    Email = newClient.Email,
                    Phone = newClient.Phone,
                    IsActive = newClient.IsActive == true,
                    IsOnboardingCompleted = true,
                    OnboardingStep = 4,
                    CreatedAt = newClient.CreatedAt,
                    StoreCount = 1,
                    CurrentPlanName = plan?.PlanName ?? "Default Plan",
                    SubscriptionStatusText = "Active",
                    PaymentStatusText = "Approved"
                };

                return ApiResponse<ProductOwnerClientDto>.Success(resultDto, $"Client '{dto.CompanyName}' created successfully by Product Owner.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductOwnerClientDto>.Fail(500, $"An error occurred while creating client: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateClientStatusAsync(long clientId, bool isActive)
        {
            try
            {
                var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == clientId);
                if (client == null)
                {
                    return ApiResponse<bool>.Fail(404, $"Client with ID {clientId} not found.");
                }

                client.IsActive = isActive;
                client.UpdatedAt = DateTime.UtcNow;
                client.UpdatedBy = UserId != 0 ? UserId : null;

                _unitOfWork.ClientRepository.Update(client);
                await _unitOfWork.SaveChangesAsync();

                string statusText = isActive ? "activated" : "deactivated";
                return ApiResponse<bool>.Success(true, $"Client '{client.CompanyName}' has been {statusText} successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error updating client status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ClientConfigurationDto>> GetClientConfigurationAsync(long clientId)
        {
            try
            {
                var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == clientId);
                if (client == null)
                {
                    return ApiResponse<ClientConfigurationDto>.Fail(404, $"Client with ID {clientId} not found.");
                }

                var clientAppSettings = await _unitOfWork.ClientAppSettingRepository.WhereAsync(cas => cas.ClientId == clientId);
                var appSettings = await _unitOfWork.AppSettingRepository.GetAllAsync();

                var config = new ClientConfigurationDto
                {
                    ClientId = clientId,
                    CompanyName = client.CompanyName,
                    MaxStores = 1,
                    MaxUsers = 5,
                    InvoicePrefix = "INV-",
                    ExpiryAlertDays = 60,
                    MultiBranchEnabled = false,
                    WhatsAppNotificationsEnabled = false,
                    CreditSalesEnabled = true,
                    AutoBackupEnabled = true
                };

                var appSettingDict = appSettings.ToDictionary(a => a.AppSettingId, a => a.SettingKey);

                foreach (var cas in clientAppSettings)
                {
                    if (appSettingDict.TryGetValue(cas.AppSettingId, out var key))
                    {
                        var val = cas.SettingValue ?? string.Empty;
                        switch (key.ToUpper())
                        {
                            case "MAX_STORES":
                                if (int.TryParse(val, out var ms)) config.MaxStores = ms;
                                break;
                            case "MAX_USERS":
                                if (int.TryParse(val, out var mu)) config.MaxUsers = mu;
                                break;
                            case "INVOICE_PREFIX":
                                config.InvoicePrefix = val;
                                break;
                            case "EXPIRY_ALERT_DAYS":
                                if (int.TryParse(val, out var ea)) config.ExpiryAlertDays = ea;
                                break;
                            case "MULTI_BRANCH_ENABLED":
                                config.MultiBranchEnabled = bool.TryParse(val, out var mb) && mb;
                                break;
                            case "WHATSAPP_NOTIFICATIONS_ENABLED":
                                config.WhatsAppNotificationsEnabled = bool.TryParse(val, out var wn) && wn;
                                break;
                            case "CREDIT_SALES_ENABLED":
                                config.CreditSalesEnabled = bool.TryParse(val, out var cs) && cs;
                                break;
                            case "AUTO_BACKUP_ENABLED":
                                config.AutoBackupEnabled = bool.TryParse(val, out var ab) && ab;
                                break;
                            default:
                                config.CustomSettings[key] = val;
                                break;
                        }
                    }
                }

                return ApiResponse<ClientConfigurationDto>.Success(config, "Client configurations loaded successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClientConfigurationDto>.Fail(500, $"Error retrieving client configurations: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ClientConfigurationDto>> SaveClientConfigurationAsync(ClientConfigurationDto dto)
        {
            try
            {
                var client = await _unitOfWork.ClientRepository.FirstOrDefaultAsync(c => c.ClientId == dto.ClientId);
                if (client == null)
                {
                    return ApiResponse<ClientConfigurationDto>.Fail(404, $"Client with ID {dto.ClientId} not found.");
                }

                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "MAX_STORES", dto.MaxStores.ToString());
                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "MAX_USERS", dto.MaxUsers.ToString());
                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "INVOICE_PREFIX", dto.InvoicePrefix ?? "INV-");
                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "EXPIRY_ALERT_DAYS", dto.ExpiryAlertDays.ToString());
                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "MULTI_BRANCH_ENABLED", dto.MultiBranchEnabled.ToString().ToLower());
                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "WHATSAPP_NOTIFICATIONS_ENABLED", dto.WhatsAppNotificationsEnabled.ToString().ToLower());
                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "CREDIT_SALES_ENABLED", dto.CreditSalesEnabled.ToString().ToLower());
                await SaveOrUpdateSettingKeyAsync(dto.ClientId, "AUTO_BACKUP_ENABLED", dto.AutoBackupEnabled.ToString().ToLower());

                if (dto.CustomSettings != null)
                {
                    foreach (var pair in dto.CustomSettings)
                    {
                        if (!string.IsNullOrWhiteSpace(pair.Key))
                        {
                            await SaveOrUpdateSettingKeyAsync(dto.ClientId, pair.Key.Trim().ToUpper(), pair.Value ?? string.Empty);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                dto.CompanyName = client.CompanyName;

                return ApiResponse<ClientConfigurationDto>.Success(dto, $"Configuration for client '{client.CompanyName}' updated successfully by Product Owner.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClientConfigurationDto>.Fail(500, $"Error saving client configuration: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductOwnerStoreDto>>> GetAllStoresAsync(long? clientId = null, string? searchTerm = null)
        {
            try
            {
                var stores = await _unitOfWork.StoreRepository.GetAllAsync();
                var clients = await _unitOfWork.ClientRepository.GetAllAsync();
                var cities = await _unitOfWork.CityRepository.GetAllAsync();
                var states = await _unitOfWork.StateRepository.GetAllAsync();

                var storeQuery = stores.AsQueryable();
                if (clientId.HasValue && clientId.Value > 0)
                {
                    storeQuery = storeQuery.Where(s => s.ClientId == clientId.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    storeQuery = storeQuery.Where(s =>
                        (s.StoreName != null && s.StoreName.ToLower().Contains(term)) ||
                        (s.StoreCode != null && s.StoreCode.ToLower().Contains(term)) ||
                        (s.PhoneNumber != null && s.PhoneNumber.ToLower().Contains(term)) ||
                        (s.DrugLicenseNumber != null && s.DrugLicenseNumber.ToLower().Contains(term))
                    );
                }

                var storeList = storeQuery.OrderByDescending(s => s.CreatedAt).ToList();
                var clientDict = clients.ToDictionary(c => c.ClientId, c => c);
                var cityDict = cities.ToDictionary(ci => ci.CityId, ci => ci.CityName);
                var stateDict = states.ToDictionary(st => st.StateId, st => st.StateName);

                var result = storeList.Select(s =>
                {
                    clientDict.TryGetValue(s.ClientId, out var client);
                    cityDict.TryGetValue(s.CityId ?? 0, out var cityName);
                    stateDict.TryGetValue(s.StateId ?? 0, out var stateName);

                    string storeTypeText = s.StoreType switch
                    {
                        1 => "Retail Pharmacy",
                        2 => "Wholesale Distributor",
                        3 => "Hospital Pharmacy",
                        _ => "General Medical Store"
                    };

                    return new ProductOwnerStoreDto
                    {
                        StoreId = s.StoreId,
                        StoreKey = s.StoreKey,
                        ClientId = s.ClientId,
                        ClientName = client?.ClientName ?? "N/A",
                        CompanyName = client?.CompanyName ?? "N/A",
                        StoreCode = s.StoreCode,
                        StoreName = s.StoreName,
                        StoreType = s.StoreType,
                        StoreTypeText = storeTypeText,
                        StoreEmail = s.StoreEmail,
                        PhoneNumber = s.PhoneNumber,
                        DrugLicenseNumber = s.DrugLicenseNumber,
                        AddressLine1 = s.AddressLine1,
                        CityId = s.CityId,
                        CityName = cityName,
                        StateName = stateName,
                        IsActive = s.IsActive,
                        CreatedAt = s.CreatedAt
                    };
                }).ToList();

                return ApiResponse<List<ProductOwnerStoreDto>>.Success(result, $"Retrieved {result.Count} stores.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductOwnerStoreDto>>.Fail(500, $"Error retrieving stores: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<PendingApprovalDto>>> GetApprovalsAsync(byte? status = null)
        {
            try
            {
                var payments = await _unitOfWork.ClientPaymentRepository.GetAllAsync();
                var clients = await _unitOfWork.ClientRepository.GetAllAsync();
                var subscriptions = await _unitOfWork.ClientSubscriptionRepository.GetAllAsync();
                var plans = await _unitOfWork.SubscriptionPlanRepository.GetAllAsync();
                var configs = await _unitOfWork.PaymentConfigurationRepository.GetAllAsync();

                var paymentQuery = payments.AsQueryable();
                if (status.HasValue)
                {
                    paymentQuery = paymentQuery.Where(p => p.PaymentStatus == status.Value);
                }

                var paymentList = paymentQuery.OrderByDescending(p => p.SubmittedAt).ToList();
                var clientDict = clients.ToDictionary(c => c.ClientId, c => c);
                var subDict = subscriptions.ToDictionary(s => s.ClientSubscriptionId, s => s);
                var planDict = plans.ToDictionary(p => p.SubscriptionPlanId, p => p);
                var configDict = configs.ToDictionary(c => c.PaymentConfigurationId, c => c.ConfigurationName);

                var result = paymentList.Select(p =>
                {
                    clientDict.TryGetValue(p.ClientId, out var client);
                    subDict.TryGetValue(p.ClientSubscriptionId, out var sub);
                    
                    SubscriptionPlan? plan = null;
                    if (sub != null) planDict.TryGetValue(sub.SubscriptionPlanId, out plan);

                    configDict.TryGetValue(p.PaymentConfigurationId, out var methodName);

                    string statusText = p.PaymentStatus switch
                    {
                        1 => "Pending Approval",
                        2 => "Approved",
                        3 => "Rejected",
                        _ => "Unknown"
                    };

                    return new PendingApprovalDto
                    {
                        ClientPaymentId = p.ClientPaymentId,
                        ClientId = p.ClientId,
                        ClientKey = client?.ClientKey ?? Guid.Empty,
                        ClientName = client?.ClientName ?? "N/A",
                        CompanyName = client?.CompanyName ?? "N/A",
                        ClientEmail = client?.Email,
                        ClientPhone = client?.Phone,
                        ClientSubscriptionId = p.ClientSubscriptionId,
                        PlanName = plan?.PlanName ?? "Subscription",
                        BillingCycle = sub?.BillingCycle == 2 ? "Yearly" : "Monthly",
                        Amount = p.Amount,
                        CurrencyCode = p.CurrencyCode,
                        PaymentMethodName = !string.IsNullOrEmpty(methodName) ? methodName : "Manual Payment",
                        TransactionReference = p.TransactionReference,
                        SenderNotes = p.SenderNotes,
                        PaymentStatus = p.PaymentStatus,
                        PaymentStatusText = statusText,
                        SubmittedAt = p.SubmittedAt,
                        AdminRemarks = p.AdminRemarks,
                        ReviewedAt = p.ReviewedAt
                    };
                }).ToList();

                return ApiResponse<List<PendingApprovalDto>>.Success(result, $"Retrieved {result.Count} payment approval items.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<PendingApprovalDto>>.Fail(500, $"Error retrieving approvals: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ProcessApprovalActionAsync(ApprovePaymentActionDto dto)
        {
            try
            {
                var req = new ApprovePaymentRequestDto
                {
                    ClientPaymentId = dto.ClientPaymentId,
                    IsApproved = dto.IsApproved,
                    AdminRemarks = dto.AdminRemarks
                };

                var res = await _paymentService.ApproveClientPaymentAsync(req);
                if (!res.IsSuccess)
                {
                    return ApiResponse<bool>.Fail(res.ErrorNo, res.Message);
                }

                return ApiResponse<bool>.Success(true, res.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail(500, $"Error processing approval: {ex.Message}");
            }
        }

        private async Task SaveOrUpdateSettingKeyAsync(long clientId, string settingKey, string settingValue)
        {
            // 1. Ensure master AppSetting exists
            var appSetting = await _unitOfWork.AppSettingRepository.FirstOrDefaultAsync(a => a.SettingKey == settingKey);
            if (appSetting == null)
            {
                appSetting = new AppSetting
                {
                    SettingKey = settingKey,
                    SettingValue = settingValue,
                    DataType = "STRING",
                    Description = $"Client setting for {settingKey}",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = UserId != 0 ? UserId : null
                };
                await _unitOfWork.AppSettingRepository.AddAsync(appSetting);
                await _unitOfWork.SaveChangesAsync();
            }

            // 2. Ensure ClientAppSetting exists
            var clientAppSetting = await _unitOfWork.ClientAppSettingRepository.FirstOrDefaultAsync(
                cas => cas.ClientId == clientId && cas.AppSettingId == appSetting.AppSettingId);

            if (clientAppSetting == null)
            {
                clientAppSetting = new ClientAppSetting
                {
                    ClientId = clientId,
                    AppSettingId = appSetting.AppSettingId,
                    SettingValue = settingValue,
                    IsActive = true,
                    CreatedBy = UserId != 0 ? UserId : null
                };
                await _unitOfWork.ClientAppSettingRepository.AddAsync(clientAppSetting);
            }
            else
            {
                clientAppSetting.SettingValue = settingValue;
                clientAppSetting.IsActive = true;
                clientAppSetting.UpdatedAt = DateTime.UtcNow;
                clientAppSetting.UpdatedBy = UserId != 0 ? UserId : null;
                _unitOfWork.ClientAppSettingRepository.Update(clientAppSetting);
            }
        }
    }
}
