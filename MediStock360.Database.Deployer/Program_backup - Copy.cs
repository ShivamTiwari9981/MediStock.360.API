//using Microsoft.Extensions.Configuration;
//using MediStock360.Database.Deployer.Services;

//try
//{
//    Console.WriteLine("==============================================");
//    Console.WriteLine("     MediStock360 Database Deployer");
//    Console.WriteLine("==============================================");
//    Console.WriteLine();

//    // -------------------------------------------------
//    // Load configuration
//    // -------------------------------------------------

//    IConfiguration configuration = new ConfigurationBuilder()
//        .SetBasePath(AppContext.BaseDirectory)
//        .AddJsonFile(
//            "appsettings.json",
//            optional: false,
//            reloadOnChange: false)
//        .Build();

//    string? connectionString =
//        configuration["DatabaseSettings:ConnectionString"];

//    if (string.IsNullOrWhiteSpace(connectionString))
//    {
//        throw new Exception(
//            "Database connection string is not configured.");
//    }

//    // -------------------------------------------------
//    // Get Scripts path
//    // -------------------------------------------------

//    string scriptsPath = Path.Combine(
//        AppContext.BaseDirectory,
//        "Scripts");

//    if (!Directory.Exists(scriptsPath))
//    {
//        throw new DirectoryNotFoundException(
//            $"Scripts directory not found: {scriptsPath}");
//    }

//    Console.WriteLine($"Scripts Path : {scriptsPath}");
//    Console.WriteLine();

//    // -------------------------------------------------
//    // Initialize Services
//    // -------------------------------------------------

//    var databaseService =
//        new DatabaseService(connectionString);

//    var migrationService =
//        new MigrationService(
//            databaseService,
//            scriptsPath);

//    // -------------------------------------------------
//    // Execute database migration
//    // -------------------------------------------------

//    await migrationService.UpdateDatabaseAsync();

//    Console.WriteLine();
//    Console.WriteLine("==============================================");
//    Console.WriteLine(" Database deployment completed successfully.");
//    Console.WriteLine("==============================================");
//}
//catch (Exception ex)
//{
//    Console.WriteLine();
//    Console.WriteLine("==============================================");
//    Console.WriteLine(" DATABASE DEPLOYMENT FAILED");
//    Console.WriteLine("==============================================");

//    Console.WriteLine();
//    Console.WriteLine(ex.Message);

//    Console.WriteLine();
//    Console.WriteLine("Press any key to exit...");
//    Console.ReadKey();

//    Environment.ExitCode = 1;
//}