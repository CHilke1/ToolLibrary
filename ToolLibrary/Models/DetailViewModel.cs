using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolLibrary.Models
{
    public class DetailViewModel
    {
        public Tool tool { get; set; }

        /*public bool Empty
        {
            get
            {
                return (Id == 0 &&
                        string.IsNullOrWhiteSpace(Name) &&
                        string.IsNullOrWhiteSpace(Description) &&
                        string.IsNullOrWhiteSpace(AdditionalDescription) &&
                        string.IsNullOrWhiteSpace(Manufacturer) &&
                        string.IsNullOrWhiteSpace(ImageUrl));
                }
        }*/
        public bool? TriggerOnLoad { get; set; }
        public string TriggerOnLoadMessage { get; set; }
        public string RedirectUrl { get; set; }
        public int CategoryId { get; set; }
        public List<DateTime> ReservedDates { get; set; }
        public List<Rental> Rentals { get; set; }
    }
}