namespace MediStock360.Application.Common
{
    public static class Global
    {
        public static long DefualtClient = 10000000;
        public static long InternalUser = 1000000;
        public static class Claim_Types
        {
            public static string User = "User";
            public static string ClientId = "ClientId";
            public static string UserId = "UserId";
            public static string UserName = "UserName";
            public static string RoleName = "RoleName";
            public static string ClientKey = "ClientKey";
            public static string RoleIdKey = "RoleIdKey";
            public static string Permission = "Permission";
            public static string StoreId = "StoreId";
            public static string StoreKey = "StoreKey";

            public static string IsOnboardingCompleted = "IsOnboardingCompleted";
            public static string OnboardingStep = "OnboardingStep";
            public static string IsActive = "IsActive";
        }
        public static class MasterTable
        {
            public static string Client = "Client";
            public static string User = "User";
            public static string Attendance = "Attendance";
            public static string Department = "Department";
            public static string Employee = "Employee";
            public static string Leave = "Leave";
            public static string Payroll = "Payroll";
            public static string Designation = "Designation";
        }

        public static class CodePrefix
        {
            public static string Client = "CLI";
            public static string User = "USR";
            public static string Attendance = "ATT";
            public static string Department = "DEP";
            public static string Employee = "EMP";
            public static string Leave = "LEA";
            public static string Payroll = "PAY";
        }
    }
}
