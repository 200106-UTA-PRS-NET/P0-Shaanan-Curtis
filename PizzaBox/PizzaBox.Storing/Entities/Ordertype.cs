using System;
using System.Collections.Generic;

namespace PizzaBox.Storing.Entities
{
    public partial class Ordertype
    {
        public int OrderId { get; set; }
        public string Preset { get; set; }
        public string Custom { get; set; }
        public string Dt { get; set; }
        public string Tm { get; set; }

        public virtual Orders Order { get; set; }
    }
}
