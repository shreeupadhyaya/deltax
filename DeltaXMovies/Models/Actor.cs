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
    public class Actor
    {
        public int ActorId { get; set; }

        [DisplayName("Actor Name")]
        [Required(ErrorMessage = "*Required")]

        public string ActorName { get; set; }

        [DisplayName("Sex")]
        [Required(ErrorMessage = "*Required")]
        public char ActorSex { get; set; }

        [DisplayName("Bio")]
        public string ActorBio { get; set; }

        [DisplayName("DOB")]
        [Required(ErrorMessage = "*Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime ActorDOB { get; set; }

        public List<Movie> ActedMovies { get; set; }

        public Actor GetActorById(int ActorId)
        {
            Actor actor = new Actor();
            string connString = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("SELECT * FROM Actor WHERE ActorId=@actorId", connection);
                command.Parameters.Add(new SqlParameter("@actorId", ActorId));

                try
                {
                    connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        actor = GetActorByRow(table.Rows[0]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return actor;
        }

        public Actor GetActorByRow(DataRow row)
        {
            Actor a = new Actor();
            a.ActorId = Convert.ToInt32(row["ActorId"]);
            a.ActorName = Convert.ToString(row["ActorName"]);
            if (row.Table.Columns.Contains("ActorSex") && row["ActorSex"] != null)
                a.ActorSex = Convert.ToChar(row["ActorSex"]);
            if (row.Table.Columns.Contains("ActorDOB") && row["ActorDOB"] != null)
                a.ActorDOB = Convert.ToDateTime(row["ActorDOB"]);
            if (row.Table.Columns.Contains("ActorBio") && row["ActorBio"] != null)
                a.ActorBio = Convert.ToString(row["ActorBio"]);
            return a;
        }

        public List<Actor> GetAllActors()
        {
            List<Actor> a = new List<Actor>();
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = "select * from [dbo].[Actor]";
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
                            a.Add(GetActorByRow(dr));
                    }
                }
            }
            catch (Exception e)
            {

            }
            return a;
        }

        public int AddActor(Actor a)
        {
            int newId = 0;
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = @"Insert into [dbo].[Actor] (ActorName,ActorSex,ActorBio,ActorDOB)
                output INSERTED.ActorId
                VALUES (@name,@sex,@bio,@DOB) ";
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@name", a.ActorName);
                    cmd.Parameters.AddWithValue("@sex", a.ActorSex);
                    cmd.Parameters.AddWithValue("@bio", string.IsNullOrEmpty(a.ActorBio) ? string.Empty : a.ActorBio);
                    cmd.Parameters.AddWithValue("@dob", a.ActorDOB);
                    newId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return newId;
        }

        public int EditActor(Actor a)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = @"Update [dbo].[Actor] SET 
                ActorName=@name,ActorSex=@sex,ActorBio=@bio,ActorDOB =@dob
                WHERE ActorId=@actorID";
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@actorID", a.ActorId);
                    cmd.Parameters.AddWithValue("@name", a.ActorName);
                    cmd.Parameters.AddWithValue("@sex", a.ActorSex);
                    cmd.Parameters.AddWithValue("@bio", string.IsNullOrEmpty(a.ActorBio) ? string.Empty : a.ActorBio);
                    cmd.Parameters.AddWithValue("@dob", a.ActorDOB);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return a.ActorId;
        }

        public List<Actor> GetAllActorsForSelectList()
        {
            List<Actor> a = new List<Actor>();
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = "select ActorId,ActorName from [dbo].[Actor]";
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
                            a.Add(GetActorByRow(dr));
                    }
                }
            }
            catch (Exception e)
            {

            }
            return a;
        }
    }
}