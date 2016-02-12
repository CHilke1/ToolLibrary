using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolLibrary.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int BasketID { get; set; }
        public int ToolID { get; set; }
        public int Quantity { get; set; }

        public virtual Basket Basket { get; set; }
        public virtual Tool Tool { get; set; }
    }
}
