using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using utilities;
public interface ICategoriesService
{
    IList<permaAPI.models.NoteCategory> GetNoteCategories();
    IList<permaAPI.models.AttachmentCategory> GetAttachmentCategories();

}
public class CategoriesService : ICategoriesService
{
    public IConfiguration Configuration { get; }
    public CategoriesService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public IList<permaAPI.models.NoteCategory> GetNoteCategories()
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var categories = new List<permaAPI.models.NoteCategory>();
        string query = @"
    select * from NoteCategory order by categoryTitle
  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var category = new permaAPI.models.NoteCategory()
                    {
                        CategoryId = reader["categoryId"].asInt(),
                        CategoryTitle = reader["categoryTitle"].asString()
                    };
                    categories.Add(category);
                }
            }
        }
        return categories;
    }
    public IList<permaAPI.models.AttachmentCategory> GetAttachmentCategories()
    {
        string connectionString = this.Configuration.GetConnectionString("permaportal");
        var categories = new List<permaAPI.models.AttachmentCategory>();
        string query = @"
    select * from AttachmentCategory order by categoryTitle
  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var category = new permaAPI.models.AttachmentCategory()
                    {
                        CategoryId = reader["categoryId"].asInt(),
                        CategoryTitle = reader["categoryTitle"].asString()
                    };
                    categories.Add(category);
                }
            }
        }
        return categories;
    }
}