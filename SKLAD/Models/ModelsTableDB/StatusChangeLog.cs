using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class StatusChangeLog
    {
        public int IdStatusChangeLog { get; set; }
        public DateTime DateTime { get; set; }
        public int IdOrder { get; set; }
        public int IdStatus { get; set; }

        public virtual Order IdOrderNavigation { get; set; }
        public virtual Status IdStatusNavigation { get; set; }
    }
}
