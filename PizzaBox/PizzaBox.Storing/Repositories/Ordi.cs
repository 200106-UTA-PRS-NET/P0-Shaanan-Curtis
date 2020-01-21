using System;
using System.Collections.Generic;

namespace PizzaBox.Storing.Repositories
{
    public partial class Ordi
    {
        public long OrderId { get; set; }
        public int StoreId { get; set; }
        public string User { get; set; }
        public int Pizzas { get; set; }
        public string Od { get; set; }
        public string Ot { get; set; }
        public Guid Preset { get; set; }
        public Guid Custom { get; set; }
        public double Cost { get; set; }

        public virtual Store Store { get; set; }
        public virtual User UserNavigation { get; set; }
    }
}
