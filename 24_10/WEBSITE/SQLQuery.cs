using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _23_10.Website
{
    public class SQLQuery : ISQLQuery
    {
        public bool findById(string id)
        {
            string commandText = "SELECT 1 FROM Companies WHERE ID = @Id ";
            SqlParameter sqlParameter = new SqlParameter("@Id", SqlDbType.NVarChar)
            {
                Value = id
            };
            using (SqlDataReader reader = SqlHelper.ExcuteReader(commandText, CommandType.Text, sqlParameter))
            {
                if (reader.HasRows)
                {
                    return true;
                }
            }
            return false;

        }

        public void Insert(Data data)
        {
            string commandText = "IF NOT EXISTS (SELECT 1 FROM Companies WHERE ID = @Id)\r\nBEGIN\r\n    INSERT INTO Companies \r\n    VALUES (@Id, @Name, @Json, @Logo)\r\nEND";
            SqlParameter parameterId = new SqlParameter("@Id", System.Data.SqlDbType.NVarChar);
            parameterId.Value = data.Id;
            SqlParameter parameterName = new SqlParameter("@Name", System.Data.SqlDbType.NVarChar);
            parameterName.Value = data.Name;
            SqlParameter parameterJson = new SqlParameter("@Json", System.Data.SqlDbType.NVarChar);
            parameterJson.Value = data.Json;
            SqlParameter parameterLogo = new SqlParameter("@Logo", System.Data.SqlDbType.NVarChar)
            {
                Value = data.Logo,
            };
            SqlHelper.ExecuteNonQuery(commandText, System.Data.CommandType.Text, parameterId, parameterName, parameterJson, parameterLogo);
        }
    }
}
