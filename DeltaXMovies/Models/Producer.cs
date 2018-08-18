using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DeltaXMovies.Models
{
    public class Producer
    {
        public int ProducerId { get; set; }
        [DisplayName("Producer Name")]
        [Required(ErrorMessage ="*Required")]
        public string ProducerName { get; set; }

        [DisplayName("Sex")]
        [Required(ErrorMessage = "*Required")]
        public char ProducerSex { get; set; }

        [DisplayName("Bio")]
        public string ProducerBio { get; set; }

        [DisplayName("DOB")]
        [Required(ErrorMessage = "*Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime ProducerDOB { get; set; }

        public List<Movie> ProducedMovies { get; set; }

        private Producer GetProducerByRow(DataRow dr)
        {
            Producer p = new Producer();
            if (dr != null)
            {
                p.ProducerId = Convert.ToInt32(dr["ProducerId"]);
                p.ProducerName = Convert.ToString(dr["ProducerName"]);
                if (dr.Table.Columns.Contains("ProducerSex") && dr["ProducerSex"] != null)
                    p.ProducerSex = Convert.ToChar(dr["ProducerSex"]);
                if (dr.Table.Columns.Contains("ProducerBio") && dr["ProducerBio"] != null)
                    p.ProducerBio = Convert.ToString(dr["ProducerBio"]);
                if (dr.Table.Columns.Contains("ProducerDOB") && dr["ProducerDOB"] != null)
                    p.ProducerDOB = Convert.ToDateTime(dr["ProducerDOB"]);
            }
            return p;
        }

        public Producer GetProducerById(int ProducerId)
        {
            Producer p = new Producer();
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = "select * from [dbo].[Producer] where ProducerId=@producerID";
                using (conn)
                {
                    DataSet ds = new DataSet();
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@producerId", ProducerId);
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        p = GetProducerByRow(ds.Tables[0].Rows[0]);
                }
            }
            catch (Exception e)
            {

            }
            return p;
        }

        public List<Producer> GetAllProducers()
        {
            List<Producer> p = new List<Producer>();
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = "select * from [dbo].[Producer]";
                using (conn)
                {
                    conn.Open();
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                            p.Add(GetProducerByRow(dr));
                    }
                }
            }
            catch (Exception e)
            {

            }
            return p;
        }

        public List<Producer> GetAllProducersForSelectList()
        {
            List<Producer> p = new List<Producer>();
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = "select ProducerId,ProducerName from [dbo].[Producer]";
                using (conn)
                {
                    conn.Open();
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                            p.Add(GetProducerByRow(dr));
                    }
                }
            }
            catch (Exception e)
            {

            }
            return p;
        }

        public int AddProducer(Producer p)
        {
            int newId = 0;
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = @"Insert into [dbo].[Producer] (ProducerName,ProducerSex,ProducerBio,ProducerDOB)
                output INSERTED.ProducerId
                VALUES (@name,@sex,@bio,@DOB) ";
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@name", p.ProducerName);
                    cmd.Parameters.AddWithValue("@sex", p.ProducerSex);
                    cmd.Parameters.AddWithValue("@bio", string.IsNullOrEmpty(p.ProducerBio) ? string.Empty : p.ProducerBio);
                    cmd.Parameters.AddWithValue("@dob", p.ProducerDOB);
                    newId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return newId;
        }

        public int EditProducer(Producer p)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = @"Update [dbo].[Producer] SET 
                ProducerName=@name,ProducerSex=@sex,ProducerBio=@bio,ProducerDOB =@dob
                WHERE ProducerId=@producerID";
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@producerID", p.ProducerId);
                    cmd.Parameters.AddWithValue("@name", p.ProducerName);
                    cmd.Parameters.AddWithValue("@sex", p.ProducerSex);
                    cmd.Parameters.AddWithValue("@bio", string.IsNullOrEmpty(p.ProducerBio) ? string.Empty : p.ProducerBio);
                    cmd.Parameters.AddWithValue("@dob", p.ProducerDOB);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return p.ProducerId;
        }
    }
}