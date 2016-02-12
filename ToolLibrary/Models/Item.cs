using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolLibrary.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCheckedOut { get; set; }
    }
}