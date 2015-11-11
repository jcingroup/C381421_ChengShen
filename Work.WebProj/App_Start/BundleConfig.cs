using System.Web.Optimization;
using System.Web.Optimization.React;

namespace DotWeb.AppStart
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JScript

            string[] commFile = new string[] {
                "~/ScriptsTSDef/dynScript/defData.js",
                "~/Scripts/jquery/jquery-2.1.3.js",
                "~/Scripts/angular/angular.js",
                "~/Scripts/angular/angular-animate.js",
                "~/Scripts/angular/angular-route.js",
                "~/Scripts/angular/i18n/angular-locale_zh-tw.js",
                "~/Scripts/angular-plugging/signalr-hub.js",
                "~/Scripts/angular-plugging/toaster.js",
                "~/Scripts/angular-plugging/ui-bootstrap-tpls-0.11.2.js",
                "~/ScriptsTSDef/commfunc.js",
                "~/ScriptsTSDef/commangular.js",
                "~/Scripts/BrowerInfo.js",
            };

            string[] cusFileUpLoad = new string[] {
                "~/ScriptsTSDef/dynScript/defData.js",
                "~/Scripts/angular-plugging/angular-file-upload/angular-file-upload-html5-shim.js",
                "~/Scripts/jquery/jquery-2.1.1.js",
                "~/Scripts/angular/angular.js",
                "~/Scripts/angular-plugging/angular-file-upload/angular-file-upload.js",
                "~/Scripts/angular/angular-animate.js",
                "~/Scripts/angular/angular-route.js",
                "~/Scripts/angular/i18n/angular-locale_zh-tw.js",
                "~/Scripts/angular-plugging/signalr-hub.js",
                "~/Scripts/angular-plugging/toaster.js",
                "~/Scripts/angular-plugging/ui-bootstrap-tpls-0.11.2.js",
                "~/ScriptsTSDef/commfunc.js",
                "~/ScriptsTSDef/commangular.js",
                "~/Scripts/BrowerInfo.js"
            };

            string[] commReacWebWWW = new string[] {
                "~/ScriptsTSDef/dynScript/defData.js",
                "~/Scripts/moment.js",
                "~/Scripts/react/react-with-addons.js",
                "~/ScriptsTSDef/commfunc.js",
                "~/ScriptsJSX/comm.jsx",
                "~/Scripts/BrowerInfo.js"
            };


            #region Sys_Base
            bundles.Add(new ScriptBundle("~/login")
            .Include(commFile)
            .Include("~/ScriptsCtrl/login.js")
            );
            bundles.Add(new ScriptBundle("~/rolesCtr")
                .Include(commFile)
                .Include("~/ScriptsCtrl/rolesCtr")
                );
            bundles.Add(new ScriptBundle("~/usersCtr")
                .Include(commFile)
                .Include("~/ScriptsCtrl/usersCtr.js")
                );

            bundles.Add(new ScriptBundle("~/changepasswordCtr")
                .Include(commFile)
                .Include("~/ScriptsCtrl/users_changepassword.js")
                );
            bundles.Add(new ScriptBundle("~/DocCtr")
                .Include(commFile)
                .Include("~/ScriptsCtrl/DocCtr.js")
                );
            #endregion
            #endregion

            #region 後台
            bundles.Add(new ScriptBundle("~/ProductTypeCtr")
                .Include(commFile)
                .Include("~/ScriptsCtrl/ProductTypeCtr.js")
            );
            bundles.Add(new ScriptBundle("~/ProductDataCtr")
                .Include(commFile)
                .Include("~/ScriptsCtrl/ProductDataCtr.js")
                .Include("~/Scripts/CKEditor/ckeditor.js")
                .Include("~/Scripts/CKEditor/configNG.js")
                .Include("~/Scripts/angular-plugging/ng-ckeditor.js")
                );
            bundles.Add(new ScriptBundle("~/ParmCtr")
                .Include(commFile)
                .Include("~/ScriptsCtrl/ParmCtr.js")
                );
            bundles.Add(new ScriptBundle("~/upfile")
            .Include(
            "~/Scripts/angular-plugging/angular-file-upload/angular-file-upload-html5-shim.js",
            "~/Scripts/angular-plugging/angular-file-upload/angular-file-upload.js",
            "~/Scripts/shadowbox/shadowbox.js",
            "~/ScriptsTSDef/commupload.js"
            ));
            #endregion

            #region 前台
            bundles.Add(new ScriptBundle("~/ServiceMail")
               .Include(commFile)
               .Include("~/Scripts/ctrl/ServiceMailCtr.js")
               );
            #endregion

            #region Web WWW

            bundles.Add(new JsxBundle("~/homeWWW")
            .Include(commReacWebWWW)
            .Include("~/ScriptsJSX/homeW.jsx")
            );

            bundles.Add(new JsxBundle("~/news_listWWW")
            .Include(commReacWebWWW)
            .Include("~/ScriptsJSX/news_listW.jsx")
            );

            bundles.Add(new JsxBundle("~/news_contentWWW")
            .Include(commReacWebWWW)
            .Include("~/ScriptsJSX/news_contentW.jsx")
            );

            #endregion


            #region CSS
            bundles.Add(new StyleBundle("~/_Code/CSS/CSS")
            .Include(
            "~/_Code/CSS/css/page.css",
            "~/_Code/CSS/toaster.css"
            ));
            #endregion
            BundleTable.EnableOptimizations = false;
        }
    }
}
