using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBHTranVanPhuong.Models;
using System.Transactions;



namespace QLBHTranVanPhuong.Controllers
{



    public class CartController : Controller
    {
        NWDataDataContext da = new NWDataDataContext();
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
        public List<Cart> GetListCarts()
        {
            List<Cart> carts = Session["Cart"] as List<Cart>;
            if (carts == null)
            {
                carts = new List<Cart>();
                Session["Cart"] = carts;
            }
            return carts;
        }
        public ActionResult AddCart(int id)
        {
            List<Cart> carts = GetListCarts();
            Cart c = carts.Find(s => s.ProductID == id);
            if (c == null)
            {
                c = new Cart(id);
                carts.Add(c);
            }
            else
            {
                c.Quantity++;
            }
            return RedirectToAction("ListCarts");
        }

        public ActionResult ListCarts()
        {
            List<Cart> carts = GetListCarts();
            if (carts.Count == 0)
            {
                return RedirectToAction("ListProduct", "Product");
            }
            ViewBag.CountProduct = Count();
            ViewBag.Total = Total();
            return View(carts);
        }

        public ActionResult Delete(int id)
        {
            List<Cart> carts = GetListCarts();
            Cart c = carts.Find(s => s.ProductID == id);
            if (c != null)
            {
                carts.RemoveAll(s => s.ProductID == id);
                return RedirectToAction("ListCarts");
            }
            if (carts.Count == 0)
            {
                return RedirectToAction("ListProduct", "Product");
            }
            return RedirectToAction("ListCarts");
        }
        private int Count()
        {
            int n = 0;
            List<Cart> carts = Session["Cart"] as List<Cart>;
            if (carts != null)
            {
                n = carts.Sum(s => s.Quantity);
            }
            return n;
        }
        private decimal? Total()
        {
            decimal? total = 0;
            List<Cart> carts = Session["Cart"] as List<Cart>;
            if (carts != null)
            {
                total = carts.Sum(s => s.Total);

            }
            return total;
        }

        public ActionResult Index2()
        {
            return View();
        }

        // Order
        public ActionResult OrderProduct(FormCollection fCollection)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    Order order = new Order();
                    order.OrderDate = DateTime.Now;
                    da.Orders.InsertOnSubmit(order);
                    da.SubmitChanges();

                    order = da.Orders.OrderByDescending(s => s.OrderID).Take(1).SingleOrDefault();
                    List<Cart> carts = GetListCarts();
                    foreach (var item in carts)
                    {
                        Order_Detail d = new Order_Detail();
                        d.OrderID = order.OrderID;
                        d.ProductID = item.ProductID;
                        d.Quantity = short.Parse(item.Quantity.ToString());
                        d.UnitPrice = (decimal)item.UnitPrice;
                        d.Discount = 0;
                        da.Order_Details.InsertOnSubmit(d);
                    }

                    da.SubmitChanges();
                    tranScope.Complete();
                    Session["Cart"] = null;

                    return RedirectToAction("ListProduct", "Product");

                }
                catch (Exception e)
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListCart");
                }




            };
        }

       
        

    }
    
}