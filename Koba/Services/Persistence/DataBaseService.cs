using Microsoft.Data.SqlClient;

namespace Koba.Services.Persistence {
    public class DataBaseService : IDataBaseService{
        private readonly SqlConnection conn;
        public DataBaseService(string connectionString) { 
            conn = new SqlConnection(connectionString);

            TestConnection();
        }
        private void TestConnection() {
            try {
                conn.Open();
                conn.Close();
            }catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
