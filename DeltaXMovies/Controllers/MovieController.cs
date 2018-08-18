using DeltaXMovies.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeltaXMovies.Controllers
{
    public class MovieController : Controller
    {
        // GET: Movie
        public ActionResult Index()
        {
            List<Models.Movie> movies = new List<Models.Movie>();
            movies = new DeltaXMovies.Models.Movie().GetAll();
            return View(movies);
        }

        // GET: Movie/Details/5
        public ActionResult Details(int id)
        {
            Movie m = new Movie().GetMovieById(id);
            return View(m);
        }

        // GET: Movie/Create
        public ActionResult Create()
        {
            List<Producer> producers = new Producer().GetAllProducersForSelectList();
            List<SelectListItem> lstP = new List<SelectListItem>();
            if (producers.Count > 0)
                lstP = producers.Select(p => new SelectListItem { Value = p.ProducerId.ToString(), Text = p.ProducerName }).ToList();
            ViewBag.ProducersList = lstP;

            List<Actor> actors = new Actor().GetAllActorsForSelectList();
            List<SelectListItem> lstA = new List<SelectListItem>();
            if (producers.Count > 0)
                lstA = actors.Select(p => new SelectListItem { Value = p.ActorId.ToString(), Text = p.ActorName }).ToList();
            ViewBag.ActorsList = lstA;
            return View();
        }

        // POST: Movie/Create
        [HttpPost]
        public ActionResult Create(Movie m, HttpPostedFileBase image)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    SetMoviePoster(m, image);
                    new Movie().AddMovie(m);
                }
               
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Movie/Edit/5
        public ActionResult Edit(int id)
        {
            Movie m = new Movie().GetMovieById(id);
            List<Producer> producers = new Producer().GetAllProducersForSelectList();
            List<SelectListItem> lstP = new List<SelectListItem>();
            if (producers.Count > 0)
                lstP = producers.Select(p => new SelectListItem { Value = p.ProducerId.ToString(), Text = p.ProducerName }).ToList();
            ViewBag.ProducersList = lstP;

            List<Actor> actors = new Actor().GetAllActorsForSelectList();
            List<SelectListItem> lstA = new List<SelectListItem>();
            if (producers.Count > 0)
                lstA = actors.Select(p => new SelectListItem { Value = p.ActorId.ToString(), Text = p.ActorName }).ToList();
            ViewBag.ActorsList = lstA;
            return View(m);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Movie m, HttpPostedFileBase image)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    SetMoviePoster(m, image);
                    new Movie().EditMovie(m);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Movie/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Movie/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private void SetMoviePoster(Movie m,HttpPostedFileBase image)
        {
            if (image != null)
            {
                string imageFile = m.ValidateAndReturnMoviePoster(image);
                if (!string.IsNullOrEmpty(imageFile))
                {
                    using (var srcImage = Image.FromFile(imageFile))
                    using (var newImage = new Bitmap(100, 100))
                    using (var graphics = Graphics.FromImage(newImage))
                    using (var stream = new MemoryStream())
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, 100, 100));
                        newImage.Save(stream, ImageFormat.Png);
                        var thumbNew = File(stream.ToArray(), "image/png");
                        m.MoviePoster = thumbNew.FileContents;
                    }
                }
            }
        }
    }
}
