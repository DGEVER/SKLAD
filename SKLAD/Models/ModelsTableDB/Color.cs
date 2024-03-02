using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class Color
    {
        public Color()
        {
            Products = new HashSet<Product>();
        }

        public int IdColor { get; set; }
        public string ColorDescription { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
