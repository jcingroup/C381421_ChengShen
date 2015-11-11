using DotWeb.Helpers;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Reporting.WebForms;
using ProcCore.Business.DB0;
using ProcCore.Business.LogicConect;
using ProcCore.HandleResult;
using ProcCore.WebCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DotWeb.Areas.Sys_Base.Controllers
{
    public class ReportToPdfController : BaseController
    {
        private byte[] ToPdfReport(ReportData rpt, List<ReportParameter> param)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            ReportViewer rptvw = new ReportViewer();
            rptvw.ProcessingMode = ProcessingMode.Local;
            ReportDataSource rds = new ReportDataSource("ReportDataSet", rpt.Data);

            rptvw.LocalReport.DataSources.Clear();
            rptvw.LocalReport.ReportPath = @"_Code\RPTFile\" + rpt.ReportName;
            rptvw.LocalReport.DataSources.Add(rds);
            foreach (var pa in param)
            {
                rptvw.LocalReport.SetParameters(pa);
            }
            rptvw.LocalReport.Refresh();

            byte[] bytes = rptvw.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            return bytes;
        }
        public FileResult Apply(int Y, int user_id, int equipment_category_id)
        {
            string DBConnectionCode = CommSetup.CommWebSetup.DB0_CodeString;
            ReportCenter logicCenter = new ReportCenter(DBConnectionCode);
            db0 = ReportCenter.getDB0;

            var getUserIdCK = Request.Cookies["user_id"];
            string getUserId = string.Empty;

            Apply_User getUserData = null;
            if (getUserIdCK == null)
            {
                getUserData = db0.Apply_User.Find(user_id);
            }
            else
            {
                getUserId = getUserIdCK.Value;
                getUserData = db0.Apply_User.Where(x => x.USERID == getUserId).FirstOrDefault();
            }

            var getApplyData = db0.Apply.Where(x => x.USERID == getUserData.USERID && x.Y == Y).FirstOrDefault();
            var getEquipmentCategory = db0.Equipment_Category.Find(equipment_category_id);

            ReportData rpt = new ReportData() { ReportName = "Apply.rdlc" };

            List<ReportParameter> rparam = new List<ReportParameter>();
            rparam.Add(new ReportParameter("applyYear", Y.ToString()));
            rparam.Add(new ReportParameter("applyName", getUserData.USERNAME));

            rparam.Add(new ReportParameter("doc_name", getApplyData.doc_name));
            rparam.Add(new ReportParameter("doc_rank", getApplyData.doc_rank));
            rparam.Add(new ReportParameter("doc_tel", getApplyData.doc_tel));
            rparam.Add(new ReportParameter("mng_name", getApplyData.mng_name));
            rparam.Add(new ReportParameter("mng_rank", getApplyData.mng_rank));
            rparam.Add(new ReportParameter("mng_tel", getApplyData.mng_tel));
            rparam.Add(new ReportParameter("equipment_category_name", getEquipmentCategory.category_name));

            db0.Dispose();

            rpt.Data = logicCenter.reportAply_Detail(getApplyData.apply_id);

            ReportViewer rptvw = new ReportViewer();
            rptvw.ProcessingMode = ProcessingMode.Local;
            ReportDataSource rds = new ReportDataSource("MasterData", rpt.Data);

            rptvw.LocalReport.DataSources.Clear();
            rptvw.LocalReport.ReportPath = @"_Code\RPTFile\" + rpt.ReportName;
            rptvw.LocalReport.DataSources.Add(rds);
            foreach (var pa in rparam)
            {
                rptvw.LocalReport.SetParameters(pa);
            }
            rptvw.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
            rptvw.LocalReport.Refresh();

            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            byte[] bytes = rptvw.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            Stream outputStream = new MemoryStream(bytes);
            string setFileName = "申報表" + DateTime.Now.Ticks + ".pdf";
            return File(outputStream, "application/pdf", setFileName);
        }
        public string ApplyQuery(int Y, int user_id, int equipment_category_id)
        {

            string DBConnectionCode = CommSetup.CommWebSetup.DB0_CodeString;
            ReportCenter logicCenter = new ReportCenter(DBConnectionCode);
            db0 = ReportCenter.getDB0;

            var getUserIdCK = Request.Cookies["user_id"];
            string getUserId = string.Empty;

            Apply_User getUserData = null;
            if (getUserIdCK == null)
            {
                getUserData = db0.Apply_User.Find(user_id);
            }
            else
            {
                getUserId = getUserIdCK.Value;
                getUserData = db0.Apply_User.Where(x => x.USERID == getUserId).FirstOrDefault();
            }

            var getApplyData = db0.Apply.Where(x => x.USERID == getUserData.USERID && x.Y == Y).FirstOrDefault();
            var getEquipmentCategory = db0.Equipment_Category.Find(equipment_category_id);
            var getData = logicCenter.reportApply(Y, getUserData.USERID, equipment_category_id);
            return defJSON(getData);
        }
        void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var apply_detail_id = int.Parse(e.Parameters["apply_detail_id"].Values[0]);

            string DBConnectionCode = CommSetup.CommWebSetup.DB0_CodeString;
            ReportCenter reportLogic = new ReportCenter(DBConnectionCode);

            var getData = reportLogic.reportApply_Fuel(apply_detail_id);

            //繫結子報表
            e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("SubData", getData));
        }
    }
}