﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEP_Project.Models;
using Microsoft.AspNet.Identity;
namespace IEP_Project.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
      
        public ActionResult Index(string searchString, stateAuction? AuctionStates, int? minPrice, int? maxPrice)
        {
            var auctions = from m in db.Auctions
                         select m;


            if (AuctionStates != null && AuctionStates != stateAuction.ALL)
            {

                auctions = auctions.Where(i => i.status == AuctionStates);

            }


            if (minPrice != null ) {

                auctions = auctions.Where(i=> i.initialPrice >minPrice);

            }

            if(maxPrice != null)
            {
                auctions = auctions.Where(i => i.initialPrice < maxPrice);

            }

            if (!String.IsNullOrEmpty(searchString))
            {
                auctions = auctions.Where(s => s.productName.Contains(searchString));
              
            }




            return View(auctions);
        }

        // GET: User/Details/5
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

            ApplicationUser user = db.Users.Find( User.Identity.GetUserId());

            if (user == null || user.tokenNumber == 0)
            {
                return HttpNotFound();
            }


            user.tokenNumber--;
            auction.currentPriceRaise++;
           

            Bid bid = new Bid();
            bid.auction = auction;
            bid.sentTime = DateTime.Now;
            if (auction.duration < 10) { auction.duration = 10; }
                 
            bid.user = user;

            

            db.Entry(user).State = EntityState.Modified;
            db.Entry(auction).State = EntityState.Modified;

            db.Bids.Add(bid);
            
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "USER")]
        public ActionResult CreateInvoice() {
            var centili = "http://stage.centili.com/widget/WidgetModule?api=4390916c1cade5d6cbb749e567530b5f&clientid=" + User.Identity.GetUserId();

            Invoice invoice = new Invoice();
            invoice.creatingDateTime = DateTime.Now;
            invoice.status = stateInvoice.CREATED;
            invoice.user = db.Users.Find(User.Identity.GetUserId());

            db.Invoices.Add(invoice);
            db.SaveChanges();

            

            return Redirect(centili);

        
        }

       
        public ActionResult InvoiceResponse(string status, string clientId, float price)
        {

            Invoice invoice = db.Invoices.Find(Int32.Parse(clientId));
            if (!status.Equals("ACCEPTED")) { invoice.status = stateInvoice.FULFILLED; }
            else { invoice.status = stateInvoice.ABORTED; }
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

       
            if ( invoices != null)
            {
                String userID = User.Identity.GetUserId();
                invoices = invoices.Where(i => i.user.Id.Equals(userID));

            }

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.currentTokens = user.tokenNumber;
            return View(invoices);
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