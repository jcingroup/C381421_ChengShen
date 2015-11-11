using ProcCore.Business.LogicConect;
using ProcCore.HandleResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotWeb.WebApp.Controllers
{
    public class ServiceController : WebFrontController
    {
        // GET: Service
        public ActionResult Index()
        {
            ViewBag.BodyClass = "Service";
            return View("Service"); 
        }
        public ActionResult Email()
        {

            return View();
        }

        public string sendMail(MailContent md)
        {
            ResultInfo rAjaxResult = new ResultInfo();
            try
            {
                md.vaild = md.vaild.ToUpper();//轉大寫
                if (Session["mail"].ToString() != md.vaild)
                {
                    rAjaxResult.result = false;
                    rAjaxResult.message = "驗證碼不正確";
                    return defJSON(rAjaxResult);
                };
                var open = openLogic();
                using (db0 = getDB0())
                {
                    #region 信件發送
                    string Body = getMailBody("Email", md);//套用信件版面
                    string mailTitle = "承信推高機-線上諮詢";
                    string receiveMails = (string)open.getParmValue(ParmDefine.receiveMails);
                    string BccMails = (string)open.getParmValue(ParmDefine.BccMails);
                    Boolean mail;
                    mail = Mail_Send(md.email, //寄信人
                                    receiveMails.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), //收信人
                                    BccMails.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), //收信人(密件副本)
                                    mailTitle, //信件標題
                                    Body, //信件內容
                                    true); //是否為html格式
                    if (mail == false)
                    {
                        rAjaxResult.result = false;
                        rAjaxResult.message = "信箱號碼不正確或送信失敗";
                        return defJSON(rAjaxResult);
                    }
                    #endregion
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
    public class MailContent
    {        
        //聯絡資料
        public string name { get; set; }
        public string gender { get; set; }
        public string tel { get; set; }
        public string email { get; set; }
        public string opinion { get; set; }
        public string vaild { get; set; }
    }
}