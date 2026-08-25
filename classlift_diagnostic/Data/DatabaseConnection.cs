namespace ClassLift.Diagnostic.Data;

public static class DatabaseConnection
{
    public static string? Build(IConfiguration configuration)
    {
        var direct = configuration.GetConnectionString("MySql") ?? configuration["MYSQL_URL"];
        if (!string.IsNullOrWhiteSpace(direct) && !direct.StartsWith("mysql://", StringComparison.OrdinalIgnoreCase))
            return direct;

        var host = configuration["MYSQLHOST"];
        if (string.IsNullOrWhiteSpace(host)) return null;

        var port = configuration["MYSQLPORT"] ?? "3306";
        var user = configuration["MYSQLUSER"];
        var password = configuration["MYSQLPASSWORD"];
        var database = configuration["MYSQLDATABASE"];
        return $"Server={host};Port={port};Database={database};User={user};Password={password};SslMode=Preferred;AllowPublicKeyRetrieval=True";
    }
}
