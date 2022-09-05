using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBHTranVanPhuong.Models;

namespace QLBHTranVanPhuong.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product

        NWDataDataContext da = new NWDataDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListProduct()
        {
            var ds = da.Products.Select(s => s).ToList();
            return View(ds);
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, Product p)
        {
            da.Products.InsertOnSubmit(p);
            da.SubmitChanges();
            return RedirectToAction("ListProduct");
        }

        public ActionResult Details(int id)
        {
            var p = da.Products.FirstOrDefault(s => s.ProductID == id);
            da.SubmitChanges();
            return View(p);
        }

        public ActionResult Edit(int id)
        {
            var p = da.Products.FirstOrDefault(s => s.ProductID == id);
            da.SubmitChanges();
            return View(p);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection collection, int id)
        {
            var p = da.Products.First(s => s.ProductID == id);
            p.ProductName = collection["ProductName"];
            p.SupplierID = int.Parse(collection["SupplierID"]);
            p.CategoryID = int.Parse(collection["CategoryID"]);
            p.QuantityPerUnit = collection["QuantityPerUnit"];
            p.UnitPrice = decimal.Parse(collection["UnitPrice"]);
            p.UnitsInStock = short.Parse(collection["UnitsInStock"]);
            p.UnitsOnOrder = short.Parse(collection["UnitsOnOrder"]);
            //if (collection["ReOrderLevel"] != null)
            //    p.ReorderLevel = short.Parse(collection["ReOrderLevel"]);
            //p.Discontinued = bool.Parse(collection["Discontinued"]);
            UpdateModel(p);
            da.SubmitChanges();
            return RedirectToAction("ListProduct");
        }
    }
}