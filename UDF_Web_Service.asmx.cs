using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace HTPT_UDF_Service
{
    /// <summary>
    /// Summary description for UDF_Web_Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UDF_Web_Service : System.Web.Services.WebService
    {
        [WebMethod]
        public void Upload(string filename, string extension, byte[] data)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            string filePath = Server.MapPath(string.Format("~/Upload/{0}", filename));
            File.WriteAllBytes(filePath, data);
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "insert into tblFiles values (@Name, @Data,@Extension)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Name", filename);
                    cmd.Parameters.AddWithValue("@Data", data);
                    cmd.Parameters.AddWithValue("@Extension", extension);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        [WebMethod]
        public DataSet DownloadFile(int id/*Vị trí trên UI*/)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    DataSet dtData = new DataSet();
                    cmd.CommandText = "select * from tblFiles where Id=@Id";
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Connection = con;
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtData);
                    con.Close();
                    return dtData;
                }
            }
        }

        [WebMethod]
        public DataSet getData()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                DataSet dtData = new DataSet();
                con.Open();
                SqlCommand command = new SqlCommand("Select ID,Name,Extension From tblFiles", con);
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dtData);
                con.Close();
                return dtData;
            }
        }
    }
}
