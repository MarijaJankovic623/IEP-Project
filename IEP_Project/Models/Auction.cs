using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_Project.Models
{
    public enum stateAuction
    {
        DRAFT, READY, OPEN, SOLD, EXPIRED, DELETED
    }


    public class Auction
    {
        public int ID { get; set; }
        public string productName { get; set; }

        [NotMapped, Required]
        public HttpPostedFileBase ImageToUpload { get; set; }

        public byte[] Image { get; set; }

        public int initialPrice { get; set; }

        public int currentPriceRaise { get; set; }

        public stateAuction status { get; set; }

        public int duration { get; set; }

        public DateTime creatingDateTime { get; set; }
        public DateTime startingDateTime { get; set; }
        public DateTime finishingDateTime { get; set; }

        public virtual ICollection<Bid> auctionBidds { get; set; }

    }
}