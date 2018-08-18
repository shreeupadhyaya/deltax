using DeltaXMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeltaXMovies.Controllers
{
    public class ActorController : Controller
    {
        // GET: Actor
        public ActionResult Index()
        {
            List<Actor> actors = new List<Actor>();
            actors = new Actor().GetAllActors();
            return View(actors);
        }

        // GET: Actor/Details/5
        public ActionResult Details(int id)
        {
            Actor a = new Actor().GetActorById(id);
            return View(a);
        }

        // GET: Actor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Actor/Create
        [HttpPost]
        public ActionResult Create(Actor collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                    new Actor().AddActor(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Actor/Edit/5
        public ActionResult Edit(int id)
        {
            Actor a = new Actor().GetActorById(id);
            return View(a);
        }

        // POST: Actor/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Actor collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                    new Actor().EditActor(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Actor/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Actor/Delete/5
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
    }
}
