using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            StatusChangeLogs = new HashSet<StatusChangeLog>();
        }

        public int IdOrder { get; set; }
        public DateTime DateTime { get; set; }
        public int IdClient { get; set; }
        public int IdStatus { get; set; }

        public virtual Client IdClientNavigation { get; set; }
        public virtual Status IdStatusNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<StatusChangeLog> StatusChangeLogs { get; set; }
    }
}
