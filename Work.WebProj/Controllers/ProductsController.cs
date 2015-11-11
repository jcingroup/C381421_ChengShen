using ProcCore.Business.DB0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotWeb.WebApp.Controllers
{
    public class ProductsController : WebFrontController
    {
        // GET: Products
        public ActionResult Index()
        {
            Second(null);
            ViewBag.BodyClass = "Products";
            return View("Second");
        }
        public ActionResult Second(int? id)
        {
            m_ProductType item;

            using (db0 = getDB0())
            {
                Boolean Exist = db0.ProductType.Where(x => x.is_second == true && x.i_Hide == false).Any(x => x.id == id);
                if (id == null || !Exist)
                {
                    var haveData = db0.ProductType.Where(x => x.is_second == true && x.i_Hide == false).OrderByDescending(x => x.sort).FirstOrDefault();
                    if (haveData == null)
                    {
                        ViewBag.BodyClass = "Products Second";
                        return View("NoData");
                    }
                    else
                    {
                        id = haveData.id;
                    }
                }


                item = db0.ProductType
                    .Select(x => new m_ProductType
                    {
                        id = x.id,
                        type_name = x.type_name
                    }).Single(x => x.id == id);
                var getProductData = db0.ProductData
                    .Where(x => x.i_Hide == false)
                    .Select(x => new m_ProductData
                    {
                        id = x.id,
                        product_name = x.product_name,
                        supporting_capacity = x.supporting_capacity,
                        engine = x.engine,
                        sort = x.sort,
                        type_id = x.type_id,
                        img = ""
                    }).Where(x => x.type_id == id).OrderByDescending(x => x.sort).ToList();

                foreach (var key in getProductData)
                {
                    key.img = ImgSrc("Sys_Active", "ProductData", key.id, "SingleImg", 1);
                }
                item.ProductData = (List<m_ProductData>)getProductData;
            }
            ViewBag.A = id;
            ViewBag.BodyClass = "Products Second";
            return View(item);
        }
        public ActionResult Brand(int? id)
        {
            m_ProductType item;

            using (db0 = getDB0())
            {
                Boolean Exist = db0.ProductType.Where(x => x.is_second == false && x.i_Hide == false).Any(x => x.id == id);
                if (id == null || !Exist)
                {
                    var haveData = db0.ProductType.Where(x => x.is_second == false && x.i_Hide == false).OrderByDescending(x => x.sort).FirstOrDefault();
                    if (haveData == null)
                    {
                        ViewBag.Brand = "Brand";
                        ViewBag.BodyClass = "Products Brand";
                        return View("NoData");
                    }
                    else
                    {
                        id = haveData.id;
                    }
                }



                item = db0.ProductType
                    .Select(x => new m_ProductType
                    {
                        id = x.id,
                        type_name = x.type_name
                    }).Single(x => x.id == id);
                var getProductData = db0.ProductData
                    .Where(x=>x.i_Hide==false)
                    .Select(x => new m_ProductData
                    {
                        id = x.id,
                        product_name = x.product_name,
                        supporting_capacity = x.supporting_capacity,
                        engine = x.engine,
                        sort = x.sort,
                        type_id = x.type_id,
                        img = ""
                    }).Where(x => x.type_id == id).OrderByDescending(x => x.sort).ToList();

                foreach (var key in getProductData)
                {
                    key.img = ImgSrc("Sys_Active", "ProductData", key.id, "SingleImg", 1);
                }
                item.ProductData = (List<m_ProductData>)getProductData;
            }
            ViewBag.A = id;
            ViewBag.Brand = "Brand";
            ViewBag.BodyClass = "Products Brand";
            return View(item);
        }
        public ActionResult Detail(int id)
        {
            ProductData item = null;
            using (db0 = getDB0())
            {
                Boolean Exist = db0.ProductData.Any(x => x.id == id && x.i_Hide == false);
                if (!Exist)
                {
                    return Redirect("~/Products/Second");
                }
                else
                {
                    item = db0.ProductData.Single(x => x.id == id);
                    var product_type = db0.ProductType.Where(x => x.id == item.type_id && x.i_Hide == false).FirstOrDefault();
                    if (product_type == null)
                    {
                        return Redirect("~/Products/Second");
                    }
                    item.ProductType.type_name = product_type.type_name;
                    item.ProductType.is_second = product_type.is_second;

                    //圖片
                    item.imgs = TwoSizeAllImgSrc("Sys_Active", "ProductData", id, "DoubleImg", 1, 2);
                    if (item.ProductType.is_second == false)
                    {
                        ViewBag.Brand = "Brand";
                        ViewBag.BodyClass = "Products Brand";
                    }
                    else
                    {
                        ViewBag.BodyClass = "Products Second";
                    }
                }
            }
            return View(item);
        }
    }
}