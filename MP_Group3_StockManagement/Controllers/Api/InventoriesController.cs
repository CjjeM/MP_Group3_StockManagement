using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MP_Group3_StockManagement.Models;

namespace MP_Group3_StockManagement.Controllers.Api
{
    public class InventoriesController : ApiController
    {
        private MP_StockManagementContext db = new MP_StockManagementContext();

        // GET: api/Inventories
        [Route("api/inventory")]
        public IQueryable<GetInventory> GetInventories()
        {
            return db.Inventories.ToList()
                    .Select(i => new GetInventory { 
                        InventoryID = i.InventoryID,
                        ProductName = i.ProductName,
                        SupplierName = i.SupplierName,
                        ReleaseQuantity = i.ReleaseQuantity,
                        TotalPrice = i.TotalPrice,
                        ExpirationDate = i.ExpirationDate
                    }).AsQueryable();
        }

        public class GetInventory
        {
            public int InventoryID { get; set; }
            public string ProductName { get; set; }
            public string SupplierName { get; set; }
            public int ReleaseQuantity { get; set; }
            public decimal TotalPrice { get; set; }
            public System.DateTime ExpirationDate { get; set; }
        }
    }
}