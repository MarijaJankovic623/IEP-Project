using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEP_Project.Models
{
    public class Bid
    {
        public int ID { get; set; }
        public DateTime sentTime { get; set; }

        public virtual ApplicationUser user { get; set; }
        public virtual Auction auction { get; set; }
    }
}