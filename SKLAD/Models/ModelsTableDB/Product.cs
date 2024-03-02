using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int IdProduct { get; set; }
        public string ProductName { get; set; }
        public int IdCategories { get; set; }
        public int IdColor { get; set; }
        public int IdSize { get; set; }
        public int Amount { get; set; }

        public virtual Category IdCategoriesNavigation { get; set; }
        public virtual Color IdColorNavigation { get; set; }
        public virtual Size IdSizeNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
