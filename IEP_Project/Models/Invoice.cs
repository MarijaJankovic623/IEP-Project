using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEP_Project.Models
{
    public enum stateInvoice
    {
      CREATED  , ABORTED, FULFILLED 
    }
    public class Invoice
    {
        public int ID { get; set; }
        public int tokenNumber { get; set; }

        public int pricePerToken { get; set; }

        public stateInvoice status { get; set; }

        public DateTime creatingDateTime { get; set; }

        public virtual ApplicationUser user { get; set; }
    }
}