using System;
using System.Collections.Generic;

namespace PizzaBox.Storing.Entities
{
    public partial class Inventory
    {
        public int StoreId { get; set; }
        public int Preset { get; set; }
        public int Custom { get; set; }

        public virtual Store Store { get; set; }
    }
}
