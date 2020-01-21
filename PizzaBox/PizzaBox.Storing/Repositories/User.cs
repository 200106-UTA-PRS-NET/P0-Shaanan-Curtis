using System;
using System.Collections.Generic;

namespace PizzaBox.Storing.Repositories
{
    public partial class User
    {
        public User()
        {
            Ordi = new HashSet<Ordi>();
        }

        public string Username { get; set; }
        public string Pass { get; set; }
        public string FullName { get; set; }
        public sbyte SessionLive { get; set; }

        public virtual ICollection<Ordi> Ordi { get; set; }
    }
}
