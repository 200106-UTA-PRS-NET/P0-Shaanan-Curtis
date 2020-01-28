using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaBox.Domain.Models
{
    public partial class Store
    {
        public Store()
        {
            Orders = new HashSet<Orders>();
        }

        public int StoreId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
