using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DeltaXMovies.Models
{
    public class ActorAgreement
    {
        public int AgreementId { get; set; }

        public int ActorId { get; set; }

        public int MovieId { get; set; }

        public List<Actor> GetActorsofMovie(int movieId)
        {
            List<Actor> actors = new List<Actor>();
            string connString = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("SELECT * FROM ActorAgreement WHERE MovieId=@movieId", connection);
                command.Parameters.AddWithValue("@movieId", movieId);

                try
                {
                    connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            actors.Add(new Actor().GetActorById(Convert.ToInt32(row["ActorId"])));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return actors;
        }

        public List<Movie> GetMoviesofActor(int actorId)
        {
            List<Movie> movies = new List<Movie>();
            string connString = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("SELECT * FROM ActorAgreement WHERE ActorId=@actorId", connection);
                command.Parameters.AddWithValue("@actorId", actorId);

                try
                {
                    connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            movies.Add(new Movie().GetMovieById(Convert.ToInt32(row["ActorId"])));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return movies;
        }

        public List<ActorAgreement> GetAgreementsofMovie(int movieId)
        {
            List<ActorAgreement> agreements = new List<ActorAgreement>();
            string connString = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("SELECT * FROM ActorAgreement WHERE MovieId=@movieId", connection);
                command.Parameters.AddWithValue("@movieId", movieId);

                try
                {
                    connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            agreements.Add(new ActorAgreement().GetAgreementByRow(row));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return agreements;
        }

        private ActorAgreement GetAgreementByRow(DataRow dr)
        {
            ActorAgreement a = new ActorAgreement();
            if (dr != null)
            {
                a.AgreementId = Convert.ToInt32(dr["AgreementId"]);
                a.ActorId = Convert.ToInt32(dr["ActorId"]);
                a.MovieId = Convert.ToInt32(dr["MovieId"]);
            }
            return a;
        }
        public int AddAgreement()
        {
            int newId = 0;
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = @"Insert into [dbo].[ActorAgreement] (ActorId,MovieId)
                output INSERTED.AgreementId
                VALUES (@actorId,@movieId) ";
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@actorId", this.ActorId);
                    cmd.Parameters.AddWithValue("@movieId", this.MovieId);
                    newId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return newId;
        }

        public void DeleteAgreement(List<int> agreementIds)
        {
            try
            {
                if (agreementIds.Count > 0)
                {
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                    string sqlQuery = @"Delete from [dbo].[ActorAgreement] WHERE AgreementId IN ( "+string.Join(",", agreementIds)+" ) ";
                    using (conn)
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}