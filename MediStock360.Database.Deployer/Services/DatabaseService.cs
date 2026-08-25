using Microsoft.Data.SqlClient;

namespace MediStock360.Database.Deployer.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<SqlConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        return connection;
    }

    public async Task EnsureDatabaseVersionTableAsync()
    {
        const string sql = """
            IF NOT EXISTS
            (
                SELECT 1
                FROM sys.tables
                WHERE name = 'DatabaseVersion'
                  AND schema_id = SCHEMA_ID('dbo')
            )
            BEGIN
                CREATE TABLE dbo.DatabaseVersion
                (
                    DatabaseVersionId BIGINT IDENTITY(1,1)
                        NOT NULL,

                    VersionNumber INT NOT NULL,

                    Description NVARCHAR(500) NULL,

                    AppliedAt DATETIME2 NOT NULL
                        CONSTRAINT DF_DatabaseVersion_AppliedAt
                        DEFAULT SYSUTCDATETIME(),

                    CONSTRAINT PK_DatabaseVersion
                        PRIMARY KEY (DatabaseVersionId),

                    CONSTRAINT UQ_DatabaseVersion_VersionNumber
                        UNIQUE (VersionNumber)
                );
            END
            """;

        await using var connection =
            await CreateConnectionAsync();

        await using var command =
            new SqlCommand(sql, connection);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> GetCurrentVersionAsync()
    {
        const string sql = """
            SELECT ISNULL(MAX(VersionNumber), 0)
            FROM dbo.DatabaseVersion;
            """;

        await using var connection =
            await CreateConnectionAsync();

        await using var command =
            new SqlCommand(sql, connection);

        var result =
            await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task ExecuteScriptAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        string script)
    {
        await using var command =
            new SqlCommand(
                script,
                connection,
                transaction);

        command.CommandTimeout = 300;

        await command.ExecuteNonQueryAsync();
    }

    public async Task SaveVersionAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int versionNumber,
        string description)
    {
        const string sql = """
            INSERT INTO dbo.DatabaseVersion
            (
                VersionNumber,
                Description
            )
            VALUES
            (
                @VersionNumber,
                @Description
            );
            """;

        await using var command =
            new SqlCommand(
                sql,
                connection,
                transaction);

        command.Parameters.AddWithValue(
            "@VersionNumber",
            versionNumber);

        command.Parameters.AddWithValue(
            "@Description",
            description);

        await command.ExecuteNonQueryAsync();
    }
}