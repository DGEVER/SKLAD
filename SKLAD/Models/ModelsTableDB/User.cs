using System;
using System.Collections.Generic;

#nullable disable

namespace SKLAD.Models.ModelsTableDB
{
    public partial class User
    {
        public User()
        {
            Clients = new HashSet<Client>();
        }

        public int IdUser { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string TypeOfAccount { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
