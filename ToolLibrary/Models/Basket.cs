using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolLibrary.Models
{
    public class Basket
    {
        public Basket()
        {
            ICollection<BasketItem> BasketItems = new List<BasketItem>();
        }
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [StringLength(255)]
        public string SessionId { get; set; }
        public virtual ICollection<BasketItem> BasketItems { get; set; }
    }
}