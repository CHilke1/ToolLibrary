using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolLibrary.Models
{
    public class ToolViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "barcode")]
        public string Barcode { get; set; }

        [JsonProperty(PropertyName = "descroption")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "additionalDescription")]
        public string AdditionalDescription { get; set; }

        [JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "type")]
        public Type Type { get; set; }

        [JsonProperty(PropertyName = "ischeckedout")]
        public bool IsCheckedOut { get; set; }

        [JsonProperty(PropertyName = "category")]
        public virtual CategoryViewModel Category { get; set; }
    }
}