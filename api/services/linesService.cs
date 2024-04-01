using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using utilities;
public interface ILinesService
{
    IList<String> GetLines();

}
public class LinesService : ILinesService
{
    public IConfiguration Configuration { get; }
    public LinesService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public IList<String> GetLines()
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var LinesofCoverage = new List<String>();
        string query = @"
    SELECT LineofCoverage from LineofCoverage order by LineOfCoverage
  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LinesofCoverage.Add(reader["LineofCoverage"].EmptyIfDBNullString());
                }
            }
        }
        return LinesofCoverage;
    }
}