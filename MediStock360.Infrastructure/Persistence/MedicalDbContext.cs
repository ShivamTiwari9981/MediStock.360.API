using System;
using System.Collections.Generic;
using MediStock360.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MediStock360.Infrastructure.Persistence;

public partial class MedicalDbContext : DbContext
{
    public MedicalDbContext(DbContextOptions<MedicalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppSetting> AppSettings { get; set; }

    public virtual DbSet<BusinessType> BusinessTypes { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientSubscription> ClientSubscriptions { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<DatabaseVersion> DatabaseVersions { get; set; }

    public virtual DbSet<IsSyncDatum> IsSyncData { get; set; }

    public virtual DbSet<MasterCodeGeneration> MasterCodeGenerations { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserOtp> UserOtps { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(e => e.AppSettingId).HasName("PK__AppSetti__8CCD79967A975F1E");

            entity.ToTable("AppSetting");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DataType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SettingKey).HasMaxLength(100);
            entity.Property(e => e.SettingValue).HasMaxLength(500);
        });

        modelBuilder.Entity<BusinessType>(entity =>
        {
            entity.HasKey(e => e.BusinessTypeId).HasName("PK__Business__1D43DEC0B07C2F3D");

            entity.ToTable("BusinessType");

            entity.Property(e => e.BusinessTypeName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21B768B0751A0");

            entity.ToTable("City");

            entity.HasIndex(e => e.CityId, "IX_City_City").IsUnique();

            entity.Property(e => e.CityName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Country");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Client__E67E1A246ED3616F");

            entity.ToTable("Client");

            entity.HasIndex(e => e.ClientCode, "UQ__Client__96ADCE1B1E9935CD").IsUnique();

            entity.HasIndex(e => e.CompanyName, "UQ__Client__9BCE05DCAA08BF42").IsUnique();

            entity.HasIndex(e => e.ClientKey, "UQ__Client__E6AEDDB4AA85809D").IsUnique();

            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.ClientCode).HasMaxLength(50);
            entity.Property(e => e.ClientKey).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClientName).HasMaxLength(150);
            entity.Property(e => e.CompanyName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DrugLicenseNumber).HasMaxLength(150);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gstnumber)
                .HasMaxLength(50)
                .HasColumnName("GSTNumber");
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
            entity.Property(e => e.OnboardingStep).HasDefaultValue(1);
            entity.Property(e => e.OwnerName).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(10);

            entity.HasOne(d => d.BusinessType).WithMany(p => p.Clients)
                .HasForeignKey(d => d.BusinessTypeId)
                .HasConstraintName("FK__Client__Business__73BA3083");

            entity.HasOne(d => d.City).WithMany(p => p.Clients)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Client_City");

            entity.HasOne(d => d.Country).WithMany(p => p.Clients)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_Client_Country");

            entity.HasOne(d => d.State).WithMany(p => p.Clients)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK_Client_State");
        });

        modelBuilder.Entity<ClientSubscription>(entity =>
        {
            entity.ToTable("ClientSubscription");

            entity.HasIndex(e => e.ClientId, "IX_ClientSubscription_ClientId");

            entity.HasIndex(e => e.Status, "IX_ClientSubscription_Status");

            entity.HasIndex(e => e.SubscriptionPlanId, "IX_ClientSubscription_SubscriptionPlanId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("INR")
                .IsFixedLength();
            entity.Property(e => e.TransactionReference).HasMaxLength(150);

            entity.HasOne(d => d.Client).WithMany(p => p.ClientSubscriptions)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientSubscription_Client");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.ClientSubscriptions)
                .HasForeignKey(d => d.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientSubscription_SubscriptionPlan");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Country__10D1609F17DEA878");

            entity.ToTable("Country");

            entity.HasIndex(e => e.CountryId, "IX_Country_CountryGuid").IsUnique();

            entity.Property(e => e.CountryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
        });

        modelBuilder.Entity<DatabaseVersion>(entity =>
        {
            entity.HasKey(e => e.DatabaseVersionId).HasName("PK__Database__99E3E86C58056CEF");

            entity.ToTable("DatabaseVersion");

            entity.HasIndex(e => e.VersionNumber, "UQ__Database__A13F1708C7DAEFCB").IsUnique();

            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<IsSyncDatum>(entity =>
        {
            entity.HasKey(e => e.SyncId).HasName("PK__IsSyncDa__7E50DEC68E21F683");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
            entity.Property(e => e.JsonData).HasColumnType("text");
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MasterCodeGeneration>(entity =>
        {
            entity.HasKey(e => e.MasterCodeGenerationId).HasName("PK__MasterCo__E33D78EB9F15FBC3");

            entity.ToTable("MasterCodeGeneration");

            entity.Property(e => e.CodePrefix).HasMaxLength(20);
            entity.Property(e => e.CodeType).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NumberLength).HasDefaultValue(3);

            entity.HasOne(d => d.Client).WithMany(p => p.MasterCodeGenerations)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_MasterCodeGeneration_Client");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menu__C99ED2306567AE72");

            entity.ToTable("Menu");

            entity.HasIndex(e => e.MenuName, "UQ__Menu__B42383E4D2DA098F").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(1);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
            entity.Property(e => e.IsVisible).HasDefaultValue(false);
            entity.Property(e => e.MenuIcon).HasMaxLength(50);
            entity.Property(e => e.MenuName).HasMaxLength(200);
            entity.Property(e => e.ParentMenuId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.RouterLink).HasMaxLength(100);
            entity.Property(e => e.PermissionCode).HasMaxLength(100);

            entity.HasOne(d => d.ParentMenu).WithMany(p => p.InverseParentMenu)
                .HasForeignKey(d => d.ParentMenuId)
                .HasConstraintName("FK_Menu_Parent");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permission");

            entity.HasIndex(e => e.PermissionCode, "UQ_Permission_Code").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModuleName).HasMaxLength(100);
            entity.Property(e => e.PermissionCode).HasMaxLength(100);
            entity.Property(e => e.PermissionName).HasMaxLength(150);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasIndex(e => new { e.ClientId, e.RoleCode }, "UQ_Role_Client_RoleCode").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleCode).HasMaxLength(50);
            entity.Property(e => e.RoleName).HasMaxLength(100);

            entity.HasOne(d => d.Client).WithMany(p => p.Roles)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Role_Client");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermission");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "UQ_RolePermission_Role_Permission").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermission_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermission_Role");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__State__C3BA3B3AB4C60CDB");

            entity.ToTable("State");

            entity.HasIndex(e => e.StateId, "IX_State_State").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
            entity.Property(e => e.StateName).HasMaxLength(100);

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_State_Country");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Store");

            entity.HasIndex(e => new { e.ClientId, e.StoreCode }, "UQ_Store_Client_StoreCode").IsUnique();

            entity.HasIndex(e => e.StoreKey, "UQ_Store_StoreKey").IsUnique();

            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.AddressLine2).HasMaxLength(250);
            entity.Property(e => e.AlternatePhoneNumber).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DrugLicenseNumber).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Gstnumber)
                .HasMaxLength(50)
                .HasColumnName("GSTNumber");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.OwnerName).HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.StoreCode).HasMaxLength(50);
            entity.Property(e => e.StoreKey).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.StoreName).HasMaxLength(200);
            entity.Property(e => e.StoreType).HasDefaultValue((byte)1);

            entity.HasOne(d => d.City).WithMany(p => p.Stores)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Store_City");

            entity.HasOne(d => d.Client).WithMany(p => p.Stores)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Store_Client");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("SubscriptionPlan");

            entity.HasIndex(e => e.PlanCode, "UQ_SubscriptionPlan_PlanCode").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("INR")
                .IsFixedLength();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAienabled).HasColumnName("IsAIEnabled");
            entity.Property(e => e.IsInventoryEnabled).HasDefaultValue(true);
            entity.Property(e => e.IsPurchaseEnabled).HasDefaultValue(true);
            entity.Property(e => e.IsReportsEnabled).HasDefaultValue(true);
            entity.Property(e => e.IsSalesEnabled).HasDefaultValue(true);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
            entity.Property(e => e.PlanCode).HasMaxLength(50);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => new { e.ClientId, e.Email }, "UQ_User_Client_Email").IsUnique();

            entity.HasIndex(e => new { e.ClientId, e.UserName }, "UQ_User_Client_UserName").IsUnique();

            entity.HasIndex(e => e.UserKey, "UQ_User_UserKey").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E3806A46116").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D1053444DF1EE3").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__User__C9F284564D401B85").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsSynced).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UserKey).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Client).WithMany(p => p.Users)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Client");
        });

        modelBuilder.Entity<UserOtp>(entity =>
        {
            entity.HasKey(e => e.UserOtpId).HasName("PK__UserOtp__59A2C54F84B2F943");

            entity.ToTable("UserOtp");

            entity.HasIndex(e => new { e.UserId, e.OtpType, e.IsUsed, e.ExpiresAt }, "IX_UserOtp_Active");

            entity.HasIndex(e => new { e.UserId, e.OtpType }, "IX_UserOtp_UserId_OtpType");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.OtpHash).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.UserOtps)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserOtp_User");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ_UserRole_User_Role").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
