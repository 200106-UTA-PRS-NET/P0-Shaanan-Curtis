using System;
using System.Collections.Generic;

namespace PizzaBox.Storing.Repositories
{
    public partial class Store
    {
        public Store()
        {
            Ordi = new HashSet<Ordi>();
        }

        public int StoreId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual ICollection<Ordi> Ordi { get; set; }
    }
}
