using DotWeb.CommSetup;
using DotWeb.WebApp;
using ProcCore.Business;
using ProcCore.HandleResult;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotWeb.Areas.Sys_Active.Controllers
{
    public class ProductTypeController : BaseController, IGetMasterNewId
    {
        #region Action and function section
        public ActionResult Main()
        {
            ActionRun();
            return View();
        }

        public ActionResult Users()
        {
            ActionRun();
            return View();
        }

        #endregion

        #region ajax call section

        public string aj_Init()
        {
            using (var db0 = getDB0())
            {
                return defJSON(new
                {
                    //options_equipment_category = db0.Equipment_Category.OrderBy(x=>x.sort)
                });
            }
        }

        [HttpPost]
        public string ajax_GetMasterNewId()
        {
            return defJSON(GetNewId(CodeTable.ProductType));
        }

        public string ajax_GetDetailNewId()
        {
            return defJSON(GetNewId(CodeTable.ProductData));
        }

        #endregion

    }
}