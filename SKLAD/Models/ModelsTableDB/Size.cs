using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class Size
    {
        public Size()
        {
            Products = new HashSet<Product>();
        }

        public int IdSize { get; set; }
        public string SizeDescription { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
