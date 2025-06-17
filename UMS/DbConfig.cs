using Microsoft.Data.SqlClient;

namespace UMS
{
    public class DbConfig
    {
        public const string connectionString = "Server=LEGION\\SQLEXPRESS;Database=UMSV1;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;";
        public static SqlConnection EstablishConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
