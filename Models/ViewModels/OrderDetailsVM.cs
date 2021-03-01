using System.Collections.Generic;

namespace Models.ViewModels
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetailsList { get; set; }
    }
}