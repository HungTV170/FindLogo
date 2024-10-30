using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using _24_10.Log;

namespace _23_10.Website
{
    static class SqlHelper
    {
        private static readonly string CONNECTIONSTRING = "Server=hung311223;Database=CompaniesInfoLog;Trusted_Connection=True;";

        public static Int32 ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTIONSTRING))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public static SqlDataReader ExcuteReader(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(CONNECTIONSTRING); // Không dùng 'using' ở đây
            SqlCommand cmd = new SqlCommand(commandText, conn);

            cmd.CommandType = commandType;
            cmd.Parameters.AddRange(parameters);

            conn.Open(); // Mở kết nối
            return cmd.ExecuteReader(CommandBehavior.CloseConnection); // 'CloseConnection' sẽ đóng khi SqlDataReader đóng
        }
    }
    public class SQL : ISource
    {
        private ILogger logger;
        public SQL(ILogger logger) 
        {
            this.logger = logger;
        }

        public async Task<Data> GetData(string name, string id)
        {
            try
            {
                string commandText = "SELECT INFO FROM [Companies] WHERE Id = @Id";
                SqlParameter sqlParameterId = new SqlParameter("@Id", SqlDbType.NVarChar)
                {
                    Value = id
                };

                JObject json = new JObject();
                using (SqlDataReader reader = SqlHelper.ExcuteReader(commandText, CommandType.Text, sqlParameterId))
                {
                    while (await reader.ReadAsync())
                    {
                        json = JObject.Parse(reader.GetString(0));
                    }
                }

                return await Task.FromResult(new Data()
                {
                    Id = id,
                    Name = name,
                    Json = json.ToString(),
                    Logo = "",
                });
            }
            catch (SqlException sqlEx)
            {
                logger.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("Database error occurred.");
            }
            catch (Exception ex)
            {
                logger.WriteLine($"Unknown Error in SQL Service: {ex.Message}");
                throw new Exception("Data not found.");
            }
        }

    }
}


