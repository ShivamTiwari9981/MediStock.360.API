using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace MediStock360.Database.Deployer.Services;

public class MigrationService
{
    private readonly DatabaseService _databaseService;
    private readonly string _scriptsPath;

    public MigrationService(
        DatabaseService databaseService,
        string scriptsPath)
    {
        _databaseService = databaseService;
        _scriptsPath = scriptsPath;
    }

    public async Task UpdateDatabaseAsync()
    {
        Console.WriteLine("Checking database...");

        await _databaseService
            .EnsureDatabaseVersionTableAsync();

        int currentVersion =
            await _databaseService
                .GetCurrentVersionAsync();

        Console.WriteLine(
            $"Current Database Version : V{currentVersion:D3}");

        Console.WriteLine();

        var versions =
            GetMigrationVersions();

        if (!versions.Any())
        {
            Console.WriteLine(
                "No migration scripts found.");

            return;
        }

        bool migrationFound = false;

        foreach (var version in versions)
        {
            if (version.VersionNumber <= currentVersion)
            {
                Console.WriteLine(
                    $"[SKIP] V{version.VersionNumber:D3} - " +
                    $"{version.Description}");

                continue;
            }

            migrationFound = true;

            await ExecuteMigrationAsync(version);
        }

        if (!migrationFound)
        {
            Console.WriteLine();
            Console.WriteLine(
                "Database is already up to date.");
        }
    }

    private List<MigrationVersion> GetMigrationVersions()
    {
        var result = new List<MigrationVersion>();

        var directories =
            Directory.GetDirectories(
                _scriptsPath,
                "V*",
                SearchOption.TopDirectoryOnly);

        foreach (var directory in directories)
        {
            string directoryName =
                Path.GetFileName(directory);

            var match =
                Regex.Match(
                    directoryName,
                    @"^V(\d+)(?:_(.*))?$",
                    RegexOptions.IgnoreCase);

            if (!match.Success)
                continue;

            int versionNumber =
                int.Parse(match.Groups[1].Value);

            string description =
                match.Groups[2].Success
                    ? match.Groups[2].Value
                    : string.Empty;

            result.Add(
                new MigrationVersion
                {
                    VersionNumber = versionNumber,
                    Description = description,
                    DirectoryPath = directory
                });
        }

        return result
            .OrderBy(x => x.VersionNumber)
            .ToList();
    }

    private async Task ExecuteMigrationAsync(
        MigrationVersion migration)
    {
        Console.WriteLine();
        Console.WriteLine(
            $"[START] V{migration.VersionNumber:D3} - " +
            $"{migration.Description}");

        await using var connection =
            await _databaseService
                .CreateConnectionAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            var scripts =
                GetScriptsInExecutionOrder(
                    migration.DirectoryPath);

            if (!scripts.Any())
            {
                Console.WriteLine(
                    "  No SQL scripts found.");

                await transaction.CommitAsync();

                return;
            }

            foreach (var script in scripts)
            {
                Console.WriteLine(
                    $"  Executing: " +
                    $"{Path.GetFileName(script)}");

                string sql =
                    await File.ReadAllTextAsync(script);

                if (string.IsNullOrWhiteSpace(sql))
                    continue;

                await _databaseService.ExecuteScriptAsync(
                    connection,
                    (SqlTransaction)transaction,
                    sql);
            }

            await _databaseService.SaveVersionAsync(
                connection,
                (SqlTransaction)transaction,
                migration.VersionNumber,
                migration.Description);

            await transaction.CommitAsync();

            Console.WriteLine(
                $"[SUCCESS] V{migration.VersionNumber:D3}");
        }
        catch
        {
            await transaction.RollbackAsync();

            Console.WriteLine(
                $"[FAILED] V{migration.VersionNumber:D3}");

            throw;
        }
    }

    private List<string> GetScriptsInExecutionOrder(
        string migrationPath)
    {
        var orderedFolders = new[]
        {
            "Tables",
            "Functions",
            "Views",
            "StoredProcedures",
            "Seed"
        };

        var scripts = new List<string>();

        foreach (var folder in orderedFolders)
        {
            string folderPath =
                Path.Combine(
                    migrationPath,
                    folder);

            if (!Directory.Exists(folderPath))
                continue;

            var folderScripts =
                Directory.GetFiles(
                    folderPath,
                    "*.sql",
                    SearchOption.TopDirectoryOnly)
                .OrderBy(x => x)
                .ToList();

            scripts.AddRange(folderScripts);
        }

        return scripts;
    }

    private class MigrationVersion
    {
        public int VersionNumber { get; set; }

        public string Description { get; set; }
            = string.Empty;

        public string DirectoryPath { get; set; }
            = string.Empty;
    }
}