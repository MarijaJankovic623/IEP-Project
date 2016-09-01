using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using IEP_Project.Models;

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

                Clients.Caller.refresh(a.ID, a.productName, a.currentPriceRaise, a.status.ToString(), a.duration, (a.lastBidder != null ? a.lastBidder.UserName : null));
            }
                

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