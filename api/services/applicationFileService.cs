using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static System.Collections.Specialized.BitVector32;

namespace permaAPI.services
{
    public interface IApplicationFileService
    {
        public models.uploads.config GetConfig();
    }

    public class applicationFileService : IApplicationFileService
    {
        public IConfiguration Configuration { get; }
        public applicationFileService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string GetBlankConfig()
        {
            string blankConfigJSON;

            blankConfigJSON = System.Text.Json.JsonSerializer.Serialize(new models.uploads.config());
            return blankConfigJSON;
        }
        public models.uploads.config GetConfig()
        {
            string connectionString = this.Configuration.GetConnectionString("permaportal");
            models.uploads.config config = null;
            string query = @"
 if not exists (select * from configurationSettings where configurationKey=@key)
begin
  INSERT INTO configurationSettings (configurationkey, configurationValue) select @key, @newconfig;
end
 select * from configurationSettings where configurationkey=@key
";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@newconfig", this.GetBlankConfig());
                    cmd.Parameters.AddWithValue("@key", "azurefileconfig");

                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        config = System.Text.Json.JsonSerializer.Deserialize<models.uploads.config>(reader["configurationValue"].ToString());

                    }
                }
            }
            return config;
        }
    }
}

