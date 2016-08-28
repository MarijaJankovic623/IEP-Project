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

        /*
         * Image is not required, 
         * When open button is pressed, entity changes his status and starting date
         * SaveChanges can go without errors
         */
        public ActionResult Open(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null || auction.status != stateAuction.READY)
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
        //When first rendering a create view there is no error for image
        // GET: Auctions/Create
        public ActionResult Create()
        {
            ViewBag.ErrorPicture = "";
            return View();
        }

        /*
         * Image is not required in the database, 
         * When create button is pressed, we check if there is a picture,
         * if there isnt any picture, we return create view with an error, 
         * if there is we continue  creating an auction
         * SaveChanges can go without errors
         */
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
            if (auction == null || auction.status != stateAuction.READY)
            {
                return HttpNotFound();
            }
            return View(auction);
        }
        /*
         * Because of null fields in entities when submiting an edit form
         * we need to find an entity from a database again, and rewrite fields that are not changed 
         * 
         * So if an picture is uploaded than we chage the picture, if not we rewrite it. 
         */

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,productName,initialPrice,duration")] Auction auction)
        {
         
            Auction auctionFromDatabase = db.Auctions.Find( auction.ID );

            if (auctionFromDatabase == null )
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                auctionFromDatabase.productName = auction.productName;
                auctionFromDatabase.initialPrice = auction.initialPrice;
                auctionFromDatabase.duration = auction.duration;

               // db.Entry(auctionFromDatabase).State = EntityState.Modified;
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
            if (auction == null || auction.status != stateAuction.READY)
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
