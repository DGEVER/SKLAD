using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class OrderDetail
    {
        public int IdOrderDetails { get; set; }
        public int IdOrder { get; set; }
        public int IdProduct { get; set; }
        public int Amount { get; set; }

        public virtual Order IdOrderNavigation { get; set; }
        public virtual Product IdProductNavigation { get; set; }
    }
}
