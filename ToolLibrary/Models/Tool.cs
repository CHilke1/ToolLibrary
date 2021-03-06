﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolLibrary.Models
{
    public class Tool : Item
    {
        public string Manufacturer { get; set; }
        public string AdditionalDescription { get; set; }
        public string ImageUrl { get; set; }
        public Type Type { get; set; }

        [ForeignKey("Category")]
        public virtual int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}