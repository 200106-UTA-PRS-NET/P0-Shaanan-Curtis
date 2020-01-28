using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaBox.Domain.Models
{
    public partial class Orders
    {
        public int OrderId { get; set; }
        public int StoreId { get; set; }
        public string Username { get; set; }

        public virtual Store Store { get; set; }
        public virtual User UsernameNavigation { get; set; }
        public virtual Ordertype Ordertype { get; set; }
    }
}
