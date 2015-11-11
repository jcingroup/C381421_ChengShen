using ProcCore.HandleResult;
using System;
using System.Web.Mvc;
using ProcCore.Business.LogicConect;
using ProcCore.Business.DB0;

namespace DotWeb.Areas.Sys_Active.Controllers
{
    public class ParmDataController : BaseController
    {
        #region Action and function section
        public ActionResult Main()
        {
            ActionRun();
            return View();
        }
        #endregion

        public string aj_init()
        {
            parm r;
            var open = openLogic();
            using (db0 = getDB0())
            {
                ////產品規格
                //string layer = (string)open.getParmValue(ParmDefine.layer);
                //string surfacehandle = (string)open.getParmValue(ParmDefine.surfacehandle);
                //信件
                string receiveMails = (string)open.getParmValue(ParmDefine.receiveMails);
                //string BccMails = (string)open.getParmValue(ParmDefine.BccMails);

                r = new parm()
                {
                    //surfacehandle=surfacehandle,
                    //layer=layer,
                    receiveMails = receiveMails,
                    //BccMails = BccMails
                };
            }

            return defJSON(r);
        }
        //[ValidateInput(false)]
        public string aj_MasterUpdate(parm md)
        {

            ResultInfo rAjaxResult = new ResultInfo();
            try
            {
                var open = openLogic();

                using (db0 = getDB0())
                {
                    ////產品規格
                    //open.setParmValue(ParmDefine.surfacehandle, md.surfacehandle);
                    //open.setParmValue(ParmDefine.layer, md.layer);
                    //信件
                    open.setParmValue(ParmDefine.receiveMails, md.receiveMails);
                    //open.setParmValue(ParmDefine.BccMails, md.BccMails);
                }
                rAjaxResult.result = true;
            }
            catch (Exception ex)
            {
                rAjaxResult.result = false;
                rAjaxResult.message = ex.Message;
            }
            return defJSON(rAjaxResult);
        }

    }
}