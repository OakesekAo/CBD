using Npgsql;

namespace CBD.Helpers
{
    public class ConnectionHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("pgSettings")["pgConnection"];
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        //build a connection string if there is a var available from railway
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new Npgsql.NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
            };

            return builder.ToString();
        }
    }
}
