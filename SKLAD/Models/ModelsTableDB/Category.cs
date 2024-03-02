using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int IdCategories { get; set; }
        public string CategoriesDescription { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
