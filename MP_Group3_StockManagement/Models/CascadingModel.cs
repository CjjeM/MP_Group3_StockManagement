using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MP_Group3_StockManagement.Models
{
    public class CascadingModel
    {
        public CascadingModel()
        {
            this.Suppliers = new List<SelectListItem>();
            this.Products = new List<SelectListItem>();
        }

        public List<SelectListItem> Suppliers { get; set; }
        public List<SelectListItem> Products { get; set; }

        public string SupplierName { get; set; }
        public string ProductName { get; set; }
    }
}