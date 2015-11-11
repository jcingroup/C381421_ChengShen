using System.Web.Mvc;
using ProcCore.Business.DB0;
using System.Collections.Generic;
using System.Linq;

namespace DotWeb.Controllers
{
    public class IndexController : WebFrontController
    {
        public ActionResult Index()
        {
            NewPorduct item = new NewPorduct();

            using (db0 = getDB0())
            {
                //linq沒有last(許最後一筆)的功能,只能利用orderby desc 再用first(取第一筆)
                item.SecondHand = db0.ProductData.Where(x => x.ProductType.is_second == true && x.i_Hide==false).OrderByDescending(x => x.i_InsertDateTime).First();
                item.BrandNew = db0.ProductData.Where(x => x.ProductType.is_second == false && x.i_Hide == false).OrderByDescending(x => x.i_InsertDateTime).First();

                item.SecondHand.img = ImgSrc("Sys_Active", "ProductData", item.SecondHand.id, "SingleImg", 2);
                item.BrandNew.img = ImgSrc("Sys_Active", "ProductData", item.BrandNew.id, "SingleImg", 2);
            }
            ViewBag.IsFirstPage = true;
            ViewBag.index = true;
            return View(item);
        }
    }
    public class NewPorduct
    {
        public ProductData SecondHand { get; set; }
        public ProductData BrandNew { get; set; }
    }
}
