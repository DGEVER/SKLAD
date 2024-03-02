using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class Status
    {
        public Status()
        {
            Orders = new HashSet<Order>();
            StatusChangeLogs = new HashSet<StatusChangeLog>();
        }

        public int IdStatus { get; set; }
        public string StatusDescription { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<StatusChangeLog> StatusChangeLogs { get; set; }
    }
}
