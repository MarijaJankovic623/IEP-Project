using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEP_Project.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace IEP_Project.Controllers
{//ovde je auctionTriger u details-u i indexu 
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string searchString, stateAuction? AuctionStates, int? minPrice, int? maxPrice, int? page)
        {

            ViewBag.searchString = searchString;
            ViewBag.AuctionStates = AuctionStates;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;

            

            auctionTrigger();
            
            var auctions = from m in db.Auctions
                           select m;


            if (AuctionStates != null && AuctionStates != stateAuction.ALL)
            {

                auctions = auctions.Where(i => i.status == AuctionStates);

            }


            if (minPrice != null)
            {

                auctions = auctions.Where(i => i.initialPrice > minPrice);

            }

            if (maxPrice != null)
            {
                auctions = auctions.Where(i => i.initialPrice < maxPrice);

            }

            if (!String.IsNullOrEmpty(searchString))
            {
                auctions = auctions.Where(s => s.productName.Contains(searchString));

            }

       


            db.SaveChanges();

            int pageSize = 9;
            int pageNumber = (page ?? 1);
           
        

            return View(auctions.OrderByDescending(auction => auction.startingDateTime).ToPagedList(pageNumber, pageSize));
        }
       
        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            auctionTrigger();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            ViewBag.auction = auction;
            return View(auction.auctionBidds.OrderByDescending(bid => bid.sentTime).ToList());
        }

        [Authorize(Roles = "USER")]
        public ActionResult BidNow(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Auction auction = db.Auctions.Find(id);
            if (auction == null || auction.status != stateAuction.OPEN)
            {
                return HttpNotFound();
            }

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            if (user == null )
            {
                return HttpNotFound();
            }

            if (user.tokenNumber == 0) {
                
                return RedirectToAction("Index");
            }


            user.tokenNumber--;
            auction.currentPriceRaise++;
            auction.lastBidder = user;

            Bid bid = new Bid();
            bid.auction = auction;
            bid.sentTime = DateTime.Now;
            if (auction.duration < 10) {
                auction.duration = 10;
                auction.finishingDateTime = auction.finishingDateTime.AddSeconds(10);
            }

            bid.user = user;



            db.Entry(user).State = EntityState.Modified;
            db.Entry(auction).State = EntityState.Modified;

            db.Bids.Add(bid);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "USER")]
        public ActionResult CreateInvoice()
        {
            var centili = "https://stage.centili.com/widget/WidgetModule?api=09105f938dcd29be5efbb29fa67b88fd&clientid=" + User.Identity.GetUserId();

            Invoice invoice = new Invoice();
            invoice.creatingDateTime = DateTime.Now;
            invoice.status = stateInvoice.CREATED;
            invoice.user = db.Users.Find(User.Identity.GetUserId());

            db.Invoices.Add(invoice);
            db.SaveChanges();



            return Redirect(centili);


        }




        //http://localhost:48254/User/InvoiceSuccess?clientid=17&price=50
        public void InvoiceSuccess( int amount, string transactionid, string status, double enduserprice, string clientid)
        {

            Invoice invoice = db.Invoices.Find(Int32.Parse(clientid));
            if (invoice == null) { return ; }

            invoice.status = stateInvoice.FULFILLED;
            invoice.priceForPackage = amount * 50;
            invoice.tokenNumber = amount;
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            user.tokenNumber += invoice.tokenNumber;

            db.Entry(invoice).State = EntityState.Modified;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();


            //return RedirectToAction("InvoiceIndex");
        }




        //http://localhost:48254/User/InvoiceFailure?clientid=17&price=50
        public ActionResult InvoiceFailure(string status, string clientId, float price)
        {

            Invoice invoice = db.Invoices.Find(Int32.Parse(clientId));
            if (invoice == null) { return HttpNotFound(); }
          
            invoice.status = stateInvoice.ABORTED;
            invoice.priceForPackage = (int)(price);
            invoice.tokenNumber = invoice.priceForPackage / 50;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            user.tokenNumber += invoice.tokenNumber;

            db.Entry(invoice).State = EntityState.Modified;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("InvoiceIndex");
        }

        //http://localhost:48254/User/InvoiceResponse?status=%22ACCEPTED%22&clientid=1&price=50


        [Authorize(Roles = "USER")]
        public ActionResult InvoiceIndex()
        {
            var invoices = from m in db.Invoices
                           select m;


            if (invoices != null)
            {
                String userID = User.Identity.GetUserId();
                invoices = invoices.Where(i => i.user.Id.Equals(userID));

            }

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.currentTokens = user.tokenNumber;
            return View(invoices);
        }


        public ActionResult EditProfile()
        {
           
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/EditProfile/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "id,email,name,lastname")] ApplicationUser user)
        {
            string idUser = user.Id;
            ApplicationUser userFromDatabase = db.Users.Find(User.Identity.GetUserId());

            if (userFromDatabase == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                userFromDatabase.Email = user.Email;
                userFromDatabase.UserName = user.Email;
                userFromDatabase.name = user.name;
                userFromDatabase.lastname = user.lastname;

                db.Entry(userFromDatabase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(user);
        }

        public ActionResult soldAuctions() {

            auctionTrigger();

            var auctions = from m in db.Auctions
                           select m;


            String userID = User.Identity.GetUserId();


            if (auctions != null)
            {
                auctions = auctions.Where(a => a.status == stateAuction.SOLD );
               
            }


            if (auctions != null)
            {
              
                auctions = auctions.Where(a => a.lastBidder.Id.Equals(userID));
            }

            return View(auctions.ToList());
        }

    
        public void auctionTrigger()
        {

            var auctions = db.Auctions.ToList();
            foreach (Auction a in auctions)
            {

                if (a.status == stateAuction.OPEN)
                {
                    DateTime finishingTime = a.finishingDateTime;
                    TimeSpan timePassed = finishingTime.Subtract(DateTime.Now);

                    if ((int)timePassed.TotalSeconds > 0) a.duration = (int)timePassed.TotalSeconds;
                    else a.duration = 0;

                    ApplicationUser lastBidder = null;

                    if (a.duration == 0 && a.lastBidder == null)
                    {
                        a.status = stateAuction.EXPIRED;
                        
                    }
                    if (a.duration == 0 && a.lastBidder != null)
                    {
                        a.status = stateAuction.SOLD;

                    }

                }
            }


            db.SaveChanges();


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
