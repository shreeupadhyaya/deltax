using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DeltaXMovies.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        [DisplayName("Movie Name")]
        [Required(ErrorMessage = "*Required")]
        public string MovieName { get; set; }

        [DisplayName("Plot")]
        public string MoviePlot { get; set; }

        [DisplayName("Poster")]

        public byte[] MoviePoster { get; set; }

        [DisplayName("Year of Release")]
        [Required(ErrorMessage = "*Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime MovieYOR { get; set; }

        [DisplayName("Producer")]
        [Required(ErrorMessage = "*Required")]
        public int MovieProducerId { get; set; }
        public Producer MovieProducer { get; set; }

        [DisplayName("Actors")]
        [Required(ErrorMessage = "*Required")]
        public List<int> MovieActorIds { get; set; }
        public List<Actor> MovieActors { get; set; }

        public string PosterPath { get; set; }

        public int PosterImgContentLength { get; set; }

        public byte[] PosterImgContent { get; set; }

        public List<Movie> GetAll()
        {
            List<Movie> movies = new List<Movie>();
            string connString = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("SELECT * FROM Movie", connection);
                //command.Parameters.AddWithValue("@pricePoint", paramValue);

                try
                {
                    connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            movies.Add(GetMovieByRow(row));
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

        public Movie GetMovieById(int MovieId)
        {
            Movie movie = new Movie();
            string connString = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand("SELECT * FROM Movie WHERE MovieID=@movieId", connection);
                command.Parameters.Add(new SqlParameter("@movieId", MovieId));

                try
                {
                    connection.Open();
                    DataTable table = new DataTable();
                    table.Load(command.ExecuteReader());
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        movie = GetMovieByRow(table.Rows[0]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return movie;
        }

        private Movie GetMovieByRow(DataRow row)
        {
            Movie m = new Movie();
            m.MovieId = Convert.ToInt32(row["MovieId"]);
            m.MovieName = Convert.ToString(row["MovieName"]);
            m.MovieYOR = Convert.ToDateTime(row["MovieYOR"]);
            m.MoviePlot = Convert.ToString(row["MoviePlot"]);
            m.MovieProducer = new Producer().GetProducerById(Convert.ToInt32(row["MovieProducer"]));
            m.MovieActors = new ActorAgreement().GetActorsofMovie(Convert.ToInt32(row["MovieId"]));
            if (row["MoviePoster"] != DBNull.Value && row["MoviePoster"] != null && (byte[])row["MoviePoster"] != null && ((byte[])row["MoviePoster"]).Length > 0)
                m.MoviePoster = (byte[])(row["MoviePoster"]);
            m.MovieProducerId = m.MovieProducer.ProducerId;
            m.MovieActorIds = (m.MovieActors != null && m.MovieActors.Count > 0) ? m.MovieActors.Select(a => a.ActorId).ToList() : new List<int>();
            return m;
        }

        public int AddMovie(Movie m)
        {
            int newId = 0;
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = @"Insert into [dbo].[Movie] (MovieName,MovieYOR,MoviePlot,MoviePoster,MovieProducer)
                output INSERTED.MovieId
                VALUES (@name,@yor,@plot,@poster,@producer) ";
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@name", m.MovieName);
                    cmd.Parameters.AddWithValue("@yor", m.MovieYOR);
                    cmd.Parameters.AddWithValue("@plot", string.IsNullOrEmpty(m.MoviePlot) ? string.Empty : m.MoviePlot);
                    cmd.Parameters.AddWithValue("@poster", m.MoviePoster ?? (new byte[0]));
                    cmd.Parameters.AddWithValue("@producer", m.MovieProducerId);
                    newId = (int)cmd.ExecuteScalar();
                    m.MovieId = newId;
                    m.SetUpMovieActorAgreements();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return newId;
        }

        public int EditMovie(Movie m)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString);
                string sqlQuery = @"Update [dbo].[Movie] SET MovieName=@name,MovieYOR=@yor,MoviePlot=@plot,MoviePoster=@poster
                ,MovieProducer=@producer WHERE MovieId=@movieId";
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@movieId", m.MovieId);
                    cmd.Parameters.AddWithValue("@name", m.MovieName);
                    cmd.Parameters.AddWithValue("@yor", m.MovieYOR);
                    cmd.Parameters.AddWithValue("@plot", string.IsNullOrEmpty(m.MoviePlot) ? string.Empty : m.MoviePlot);
                    cmd.Parameters.AddWithValue("@poster", m.MoviePoster);
                    cmd.Parameters.AddWithValue("@producer", m.MovieProducerId);
                    m.SetUpMovieActorAgreements();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return m.MovieId;
        }

        public string ValidateAndReturnMoviePoster(HttpPostedFileBase image)
        {
            string poster = string.Empty;
            try
            {

                if (image.ContentType.ToLower() != "image/jpg" &&
                   image.ContentType.ToLower() != "image/jpeg" &&
                   image.ContentType.ToLower() != "image/pjpeg" &&
                   image.ContentType.ToLower() != "image/gif" &&
                   image.ContentType.ToLower() != "image/x-png" &&
                   image.ContentType.ToLower() != "image/png")
                {
                    throw new Exception("invalid file");
                }

                //-------------------------------------------
                //  Check the image extension
                //-------------------------------------------
                if (Path.GetExtension(image.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(image.FileName).ToLower() != ".png"
                    && Path.GetExtension(image.FileName).ToLower() != ".gif"
                    && Path.GetExtension(image.FileName).ToLower() != ".jpeg")
                {
                    throw new Exception("invalid file");
                }
                if (image.ContentLength > 524288000)
                {
                    throw new Exception("invalid file");
                }


                //attach the uploaded image to the object before saving to Database
                this.PosterImgContentLength = image.ContentLength;
                this.PosterImgContent = new byte[image.ContentLength];
                image.InputStream.Read(this.PosterImgContent, 0, image.ContentLength);

                //Save image to file
                var filename = image.FileName;
                var filePathOriginal = HttpContext.Current.Server.MapPath("/Content/Images/Originals");
                string savedFileName = Path.Combine(filePathOriginal, filename);
                image.SaveAs(savedFileName);

                //Read image back from file and create thumbnail from it
                var imageFile = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Originals"), filename);
                poster = imageFile;
            }
            catch (Exception e)
            {
                throw e;
            }
            return poster;
        }

        public void SetUpMovieActorAgreements()
        {
            if (this.MovieActorIds != null && this.MovieActorIds.Count > 0)
            {
                List<ActorAgreement> existingAgreements = new ActorAgreement().GetAgreementsofMovie(this.MovieId);
                if (existingAgreements != null && existingAgreements.Count > 0)
                {
                    List<int> existingAgreementIds = existingAgreements.Select(e => e.AgreementId).ToList();
                    new ActorAgreement().DeleteAgreement(existingAgreementIds);
                }
                foreach (int actorId in this.MovieActorIds)
                {
                    ActorAgreement newAgreement = new ActorAgreement { ActorId = actorId, MovieId = this.MovieId };
                    newAgreement.AddAgreement();
                }
            }
        }
    }
}