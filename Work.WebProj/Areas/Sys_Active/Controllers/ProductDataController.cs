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
    public class ProductDataController : BaseController, IGetMasterNewId
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
                    options_true = db0.ProductType.Where(x => x.is_second == true).OrderByDescending(x => x.sort).Select(x => new { key = x.id, value=x.type_name }),
                    options_false = db0.ProductType.Where(x => x.is_second == false).OrderByDescending(x => x.sort).Select(x => new { key = x.id, value = x.type_name })
                });
            }
        }

        [HttpPost]
        public string ajax_GetMasterNewId()
        {
            return defJSON(GetNewId(CodeTable.ProductData));
        }

        public string ajax_GetDetailNewId()
        {
            return defJSON(GetNewId(CodeTable.ProductData));
        }

        #endregion

        #region ajax file section
        [HttpPost]
        public String aj_FUpload(Int32 id, String FilesKind, HttpPostedFileBase file)
        {
            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();

            #region
            String tpl_File = String.Empty;
            try
            {
                //代表圖片
                if (FilesKind == "SingleImg")
                    ImageFile(file, id, ImageFileUpParm.NewsBasicSingle, FilesKind);
                //多張圖片
                if (FilesKind == "DoubleImg")
                    ImageFile(file, id, ImageFileUpParm.NewsBasicDouble, FilesKind);

                //if (FilesKind == "TypeImg")
                //    ImageFile(file, id, ImageFileUpParm.NewsBasicSingle, FilesKind, "PT");

                rAjaxResult.result = true;
                rAjaxResult.success = true;
                rAjaxResult.FileName = file.FileName;

            }
            catch (LogicError ex)
            {
                rAjaxResult.result = false;
                rAjaxResult.success = false;
                rAjaxResult.error = getRecMessage(ex.Message);
            }
            catch (Exception ex)
            {
                rAjaxResult.result = false;
                rAjaxResult.success = false;
                rAjaxResult.error = ex.Message;
            }
            #endregion
            return defJSON(rAjaxResult);
        }

        [HttpPost]
        public String aj_FList(int id, String FileKind)
        {
            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();
            //if (FileKind == "TypeImg")
            //{
            //    rAjaxResult.filesObject = ListSysFiles(id, FileKind, "PT");

            //}
            //else
            //{
            //    rAjaxResult.filesObject = ListSysFiles(id, FileKind);
            //}
            rAjaxResult.filesObject = ListSysFiles(id, FileKind);
            rAjaxResult.result = true;
            return defJSON(rAjaxResult);
        }

        [HttpPost]
        public String aj_FDelete(int id, String FileKind, String FileName)
        {
            ReturnAjaxFiles rAjaxResult = new ReturnAjaxFiles();
            //if (FileKind == "TypeImg")
            //{
            //    DeleteSysFile(id, FileKind, FileName, ImageFileUpParm.NewsBasicSingle, "PT");
            //}
            //else
            //{
            //    DeleteSysFile(id, FileKind, FileName, ImageFileUpParm.NewsBasicSingle);
            //}
            DeleteSysFile(id, FileKind, FileName, ImageFileUpParm.NewsBasicSingle);
            rAjaxResult.result = true;
            return defJSON(rAjaxResult);
        }
        #endregion
    }
}