using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Core.Services
{
    //For your example:

    //newschool1.dev.classlift.ca

    //the tenant registry should contain:

    //Subdomain: newschool1
    //DatabaseName: classlift_newschool1

    //The connection factory then generates a connection string with:

    //Database=classlift_newschool1


    public sealed class TenantConnectionStringFactory
        : ITenantConnectionStringFactory
    {
        private readonly string _baseConnectionString;

        public TenantConnectionStringFactory(
            IConfiguration configuration)
        {
            _baseConnectionString =
                configuration.GetConnectionString("ServerConnection")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:ServerConnection is missing.");
        }

        public string BuildConnectionString(string databaseName)
        {
            var builder = new MySqlConnectionStringBuilder(
                _baseConnectionString);

            builder.Database = databaseName;

            return builder.ConnectionString;
        }

        public string BuildServerConnectionString()
        {
            return _baseConnectionString;
        }
    }
}