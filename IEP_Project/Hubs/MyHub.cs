using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using IEP_Project.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace IEP_Project.Hubs
{
    public class MyHub : Hub
    {
        public void refresh(int[] auctionIds)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            auctionTrigger(db);


            var auctions = from a in db.Auctions
                           select a;

            auctions = auctions.Where(a => auctionIds.Contains(a.ID));
            var auctions1 = auctions.ToList();


          
            foreach (Auction a in auctions1) {
                var auctionbids = a.auctionBidds;

                string bids = " <p> ";
                foreach (Bid bid in auctionbids.OrderByDescending(b => b.sentTime).Take(10)) {

                    bids += "<b>";
                    bids += "   Sent time:   ";
                    bids += "</b>";
                    bids += bid.sentTime;
                    bids += "</br>";
                    bids += "<b>";
                    bids += "   User:   ";
                    bids += bid.user.Email;
                    bids += "</b>";
                    bids += "</br>";


                }
                bids += " </p> ";
                Clients.Caller.refresh(a.ID, a.productName, a.currentPriceRaise, a.status.ToString(), a.duration, (a.lastBidder != null ? a.lastBidder.UserName : null), bids);
            }

            db.Dispose();
        }

        [Authorize(Roles = "USER")]
        public void bidNow(int id,int currentPriceRaise) {
            ApplicationDbContext db = new ApplicationDbContext();

            if (id == null)
            {
                //vrati poruku
                return ;
            }

            Auction auction = db.Auctions.Find(id);
            if (auction == null || auction.status != stateAuction.OPEN)
            {
                //vrati poruku
                return;
            }

            ApplicationUser user = db.Users.Find(Context.User.Identity.GetUserId());

            if (user == null)
            {
                //vrati poruku
                return;
            }

            if (user.tokenNumber == 0)
            {

                //vrati poruku
                return;
            }


            user.tokenNumber--;
            auction.currentPriceRaise++;
            auction.lastBidder = user;

            Bid bid = new Bid();
            bid.auction = auction;
            bid.sentTime = DateTime.Now;
            if (auction.duration < 10)
            {
                auction.duration = 10;
                auction.finishingDateTime = auction.finishingDateTime.AddSeconds(10);
            }

            bid.user = user;



            db.Entry(user).State = EntityState.Modified;
            db.Entry(auction).State = EntityState.Modified;

            db.Bids.Add(bid);

            db.SaveChanges();
            db.Dispose();
            return ;



        }
        public void auctionTrigger(ApplicationDbContext db)
        {

            var auctions = db.Auctions;
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



    }
}