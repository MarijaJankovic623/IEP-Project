using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEP_Project.Models;

namespace IEP_Project.Controllers
{
    public class AuctionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        public ActionResult Open(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            auction.status = stateAuction.OPEN;
            auction.startingDateTime = DateTime.Now;
            auction.finishingDateTime = DateTime.Now;

            db.Entry(auction).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("Index");

        }


        // GET: Auctions
        public ActionResult Index()
        {
            return View(db.Auctions.ToList());
        }

        // GET: Auctions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // GET: Auctions/Create
        public ActionResult Create()
        {
            ViewBag.ErrorPicture = "";
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,productName,ImageToUpload,initialPrice,duration")] Auction auction)
        {

            if (auction.ImageToUpload == null) {
                ViewBag.ErrorPicture = "Please upload picture for auction";
                return View(auction);
            }


            if (ModelState.IsValid)
            {
                // Convert HttpPostedFileBase to byte array.

                auction.currentPriceRaise = 0;
                auction.status = stateAuction.READY;
                auction.creatingDateTime = DateTime.Now;
                auction.startingDateTime = DateTime.Now;
                auction.finishingDateTime = DateTime.Now;
                


                auction.Image = new byte[auction.ImageToUpload.ContentLength];
                auction.ImageToUpload.InputStream.Read(auction.Image, 0, auction.Image.Length);

                db.Auctions.Add(auction);
                db.SaveChanges();
                return RedirectToAction("Index");

              
            }

            return View(auction);
        }

        // GET: Auctions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,productName,initialPrice,duration")] Auction auction)
        {

            /*TODO kako da mi ne pregazi sve, da li moram kao debil da dohvatim iz baze ponovo */

            if (ModelState.IsValid)
            {
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auction);
        }

        // GET: Auctions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Auction auction = db.Auctions.Find(id);
            auction.status = stateAuction.DRAFT;
            db.Entry(auction).State = EntityState.Modified;
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
