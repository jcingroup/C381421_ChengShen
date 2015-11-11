using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ProcCore.Business;
using ProcCore.Business.DB0;
using ProcCore.Business.LogicConect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace DotWeb.Api
{
    public class BaseApiController : ApiController
    {
        protected int defPageSize = 10;
        protected string aspUserId;
        protected int departmentId;
        protected string UserId; //指的是廠商登錄帳號

        protected C38A0_ChengShenEntities db0;


        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            var getUserIdCookie = controllerContext.Request.Headers.GetCookies("user_id").SingleOrDefault();
            UserId = getUserIdCookie == null ? null : getUserIdCookie["user_id"].Value;

            var getUserNameCookie = controllerContext.Request.Headers.GetCookies("user_name").SingleOrDefault();
            var UserName = getUserNameCookie == null ? "" : getUserNameCookie["user_name"].Value;

        }

        protected virtual string getRecMessage(string MsgId)
        {
            String r = Resources.Res.ResourceManager.GetString(MsgId);
            return String.IsNullOrEmpty(r) ? MsgId : r;
        }
        protected virtual string getRecMessage(IList<i_Code> codeSheet, string code)
        {
            var c = codeSheet.Where(x => x.Code == code).FirstOrDefault();

            if (c == null)
                return code;
            else
            {
                string r = Resources.Res.ResourceManager.GetString(c.LangCode);
                return string.IsNullOrEmpty(r) ? c.Value : r;
            }
        }
        public static string RemoveScriptTag(string htmlSource)
        {
            //移除  javascript code.
            htmlSource = Regex.Replace(htmlSource, @"<script[\d\D]*?>[\d\D]*?</script>", String.Empty);

            //移除html tag.
            //htmlSource = Regex.Replace(htmlSource, @"<[^>]*>", String.Empty);
            return htmlSource;
        }
        protected virtual LogicCenter openLogic()
        {
            LogicCenter dbLogic = new LogicCenter(CommSetup.CommWebSetup.DB0_CodeString);
            dbLogic.IP = System.Web.HttpContext.Current.Request.UserHostAddress;

            return dbLogic;
        }
        protected string getNowLnag()
        {
            return System.Globalization.CultureInfo.CurrentCulture.Name;
        }
        protected static C38A0_ChengShenEntities getDB0()
        {
            LogicCenter.SetDB0EntityString(CommSetup.CommWebSetup.DB0_CodeString);
            return LogicCenter.getDB0;
        }
        protected string ModelStateErrorPack()
        {
            List<string> errMessage = new List<string>();
            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    errMessage.Add(error.ErrorMessage);

            return string.Join(":", errMessage);
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public RoleManager<IdentityRole> roleManager
        {
            get
            {
                ApplicationDbContext context = ApplicationDbContext.Create();
                return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            }
        }

    }

    #region 泛型控制器擴充

    [System.Web.Http.Authorize]
    public abstract class ajaxApi<M, Q> : BaseApiController
        where M : new()
        where Q : QueryBase
    {
        protected rAjaxGetData<M> r;
        protected rAjaxGetItems<M> rs;
        protected M item;
    }

    [System.Web.Http.Authorize]
    public abstract class ajaxBaseApi : BaseApiController
    {

    }
    #endregion
}