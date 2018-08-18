using DeltaXMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStore.Controllers
{
    public class ProducerController : Controller
    {
        // GET: Producer
        public ActionResult Index()
        {
            List<Producer> producers = new List<Producer>();
            producers = new Producer().GetAllProducers();
            return View(producers);
        }

        // GET: Producer/Details/5
        public ActionResult Details(int id)
        {
            Producer p = new Producer().GetProducerById(id);
            return View(p);
        }

        // GET: Producer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Producer/Create
        [HttpPost]
        public ActionResult Create(Producer collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                    new Producer().AddProducer(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Producer/Edit/5
        public ActionResult Edit(int id)
        {
            Producer p = new Producer().GetProducerById(id);
            return View(p);
        }

        // POST: Producer/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Producer collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                    new Producer().EditProducer(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Producer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Producer/Delete/5
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