using Microsoft.Data.SqlClient;

namespace MySQLDataBase {
    public class DataBase {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        public DataBase() {
            builder.ConnectionString = "Server=localhost\\\\sqlexpress; Initial Catalog=Discord; Intregrated Security=True; Encrypt=False;";
        }
    }
}
