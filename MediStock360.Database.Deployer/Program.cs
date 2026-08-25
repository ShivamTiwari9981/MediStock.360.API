using Microsoft.Extensions.Configuration;
using MediStock360.Database.Deployer.Services;
namespace MediStock360.Database.Deployer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("     MediStock360 Database Deployer");
                Console.WriteLine("==============================================");
                Console.WriteLine();

                // -------------------------------------------------
                // Load configuration
                // -------------------------------------------------

                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile(
                        "appsettings.json",
                        optional: false,
                        reloadOnChange: false)
                    .Build();

                string? connectionString =
                    configuration["DatabaseSettings:ConnectionString"];

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception(
                        "Database connection string is not configured.");
                }

                // -------------------------------------------------
                // Initialize Services
                // -------------------------------------------------

                var databaseService =
                    new DatabaseService(connectionString);

                string scriptsPath = Path.Combine(
                    AppContext.BaseDirectory,
                    "Scripts");

                if (!Directory.Exists(scriptsPath))
                {
                    throw new DirectoryNotFoundException(
                        $"Scripts directory not found: {scriptsPath}");
                }

                var migrationService =
                    new MigrationService(
                        databaseService,
                        scriptsPath);

                var scaffoldService =
                    new ScaffoldService(configuration);

                // -------------------------------------------------
                // Main Menu
                // -------------------------------------------------

                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("==============================================");
                    Console.WriteLine("     MediStock360 Database Deployer");
                    Console.WriteLine("==============================================");
                    Console.WriteLine();

                    Console.WriteLine("1. Run Database Scripts");
                    Console.WriteLine("2. Scaffold Entities & DbContext");
                    Console.WriteLine("3. Exit");

                    Console.WriteLine();
                    Console.Write("Select Option: ");

                    string? option = Console.ReadLine();

                    Console.WriteLine();

                    switch (option)
                    {
                        // -----------------------------------------
                        // Database Migration
                        // -----------------------------------------

                        case "1":

                            Console.WriteLine("----------------------------------------------");
                            Console.WriteLine(" Running Database Migration");
                            Console.WriteLine("----------------------------------------------");
                            Console.WriteLine();

                            Console.WriteLine(
                                $"Scripts Path : {scriptsPath}");

                            Console.WriteLine();

                            await migrationService.UpdateDatabaseAsync();

                            Console.WriteLine();
                            Console.WriteLine("==============================================");
                            Console.WriteLine(
                                " Database deployment completed successfully.");
                            Console.WriteLine("==============================================");

                            Console.WriteLine();
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();

                            break;


                        // -----------------------------------------
                        // EF Core Scaffolding
                        // -----------------------------------------

                        case "2":

                            Console.WriteLine("----------------------------------------------");
                            Console.WriteLine(
                                " Scaffold Entities & DbContext");
                            Console.WriteLine("----------------------------------------------");
                            Console.WriteLine();

                            Console.Write(
                                "This will overwrite generated files. Continue? (Y/N): ");

                            string? confirmation = Console.ReadLine();

                            if (!string.Equals(
                                    confirmation,
                                    "Y",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine();
                                Console.WriteLine("Scaffolding cancelled.");

                                Console.WriteLine();
                                Console.WriteLine(
                                    "Press any key to continue...");

                                Console.ReadKey();

                                break;
                            }

                            Console.WriteLine();

                            await scaffoldService.ScaffoldAsync();

                            Console.WriteLine();
                            Console.WriteLine("==============================================");
                            Console.WriteLine(
                                " Scaffolding completed successfully.");
                            Console.WriteLine("==============================================");

                            Console.WriteLine();
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();

                            break;


                        // -----------------------------------------
                        // Exit
                        // -----------------------------------------

                        case "3":

                            Console.WriteLine("Exiting...");
                            return;


                        default:

                            Console.WriteLine(
                                "Invalid option. Please select 1, 2 or 3.");

                            Console.WriteLine();
                            Console.WriteLine(
                                "Press any key to continue...");

                            Console.ReadKey();

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("==============================================");
                Console.WriteLine(" DATABASE DEPLOYMENT FAILED");
                Console.WriteLine("==============================================");

                Console.WriteLine();
                Console.WriteLine(ex.Message);

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                Environment.ExitCode = 1;
            }
        }
    }
}




