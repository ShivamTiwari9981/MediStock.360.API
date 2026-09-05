using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace MediStock360.Database.Deployer.Services;

public class ScaffoldService
{
    private readonly IConfiguration _configuration;

    public ScaffoldService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ScaffoldAsync()
    {
        Console.WriteLine("Starting EF Core scaffolding...");
        Console.WriteLine();

        // -------------------------------------------------
        // Find Solution Directory
        // -------------------------------------------------

        string? solutionDirectory = FindSolutionDirectory();

        if (string.IsNullOrWhiteSpace(solutionDirectory))
        {
            throw new DirectoryNotFoundException(
                "MediStock360 solution directory could not be found.");
        }

        Console.WriteLine($"Solution Path : {solutionDirectory}");

        // -------------------------------------------------
        // Project Paths
        // -------------------------------------------------

        string domainProject = Path.Combine(
            solutionDirectory,
            "MediStock360.Domain");

        string infrastructureProject = Path.Combine(
            solutionDirectory,
            "MediStock360.Infrastructure");

        string apiProject = Path.Combine(
            solutionDirectory,
            "MediStock360.API");

        if (!Directory.Exists(domainProject))
        {
            throw new DirectoryNotFoundException(
                $"Domain project not found: {domainProject}");
        }

        if (!Directory.Exists(infrastructureProject))
        {
            throw new DirectoryNotFoundException(
                $"Infrastructure project not found: {infrastructureProject}");
        }

        if (!Directory.Exists(apiProject))
        {
            throw new DirectoryNotFoundException(
                $"API project not found: {apiProject}");
        }

        // -------------------------------------------------
        // Connection String
        // -------------------------------------------------

        string? connectionString =
            _configuration["DatabaseSettings:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception(
                "Database connection string is not configured.");
        }

        // -------------------------------------------------
        // Output Directories
        // -------------------------------------------------

        string entityDirectory = Path.Combine(
            domainProject,
            "Entities");

        string contextDirectory = Path.Combine(
            infrastructureProject,
            "Persistence");

        Directory.CreateDirectory(entityDirectory);
        Directory.CreateDirectory(contextDirectory);

        Console.WriteLine();
        Console.WriteLine($"Entity Path  : {entityDirectory}");
        Console.WriteLine($"Context Path : {contextDirectory}");
        Console.WriteLine();

        // -------------------------------------------------
        // Scaffold Arguments
        // -------------------------------------------------

        string arguments =
            $"ef dbcontext scaffold " +
            $"\"{connectionString}\" " +
            $"Microsoft.EntityFrameworkCore.SqlServer " +
            $"--project \"{infrastructureProject}\" " +
            $"--startup-project \"{apiProject}\" " +
            $"--output-dir \"{entityDirectory}\" " +
            $"--context-dir \"{contextDirectory}\" " +
            $"--context MedicalDbContext " +
            $"--force " +
            $"--no-onconfiguring";

        Console.WriteLine("Executing EF Core Scaffold...");
        Console.WriteLine();
        Console.WriteLine($"Command: dotnet {arguments}");
        Console.WriteLine();

        // -------------------------------------------------
        // Start Process
        // -------------------------------------------------

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,

                WorkingDirectory = solutionDirectory,

                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,

                CreateNoWindow = true
            }
        };

        process.Start();

        // -------------------------------------------------
        // Read stdout and stderr simultaneously
        // -------------------------------------------------

        Task<string> outputTask =
            process.StandardOutput.ReadToEndAsync();

        Task<string> errorTask =
            process.StandardError.ReadToEndAsync();

        // Wait for both streams
        await Task.WhenAll(outputTask, errorTask);

        // Wait for process
        await process.WaitForExitAsync();

        string output = outputTask.Result;
        string error = errorTask.Result;

        // -------------------------------------------------
        // Output
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine(output);
        }

        // -------------------------------------------------
        // Error
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.WriteLine();
            Console.WriteLine("EF Core Output/Error:");
            Console.WriteLine(error);
        }

        // -------------------------------------------------
        // Exit Code
        // -------------------------------------------------

        if (process.ExitCode != 0)
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" EF Core Scaffolding FAILED");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            throw new Exception(
                $"EF Core scaffolding failed with exit code {process.ExitCode}.");
        }

        // -------------------------------------------------
        // Success
        // -------------------------------------------------

        Console.WriteLine();
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine(" Scaffolding completed");
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine();

        Console.WriteLine("Entities generated at:");
        Console.WriteLine(entityDirectory);

        Console.WriteLine();

        Console.WriteLine("DbContext generated at:");
        Console.WriteLine(contextDirectory);
    }

    // -------------------------------------------------
    // Find .sln directory
    // -------------------------------------------------

    private static string? FindSolutionDirectory()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (directory.GetFiles("*.sln").Any())
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}