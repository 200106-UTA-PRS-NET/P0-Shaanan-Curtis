using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaBox.Domain.Models
{
    public partial class Inventory
    {
        public int StoreId { get; set; }
        public int Preset { get; set; }
        public int Custom { get; set; }

        public virtual Store Store { get; set; }
    }
}
