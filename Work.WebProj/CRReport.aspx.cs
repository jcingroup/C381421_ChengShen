using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CrystalDecisions.Reporting;
using CrystalDecisions.Shared;
using System.Data;
namespace DotWeb.WebApp
{
    public partial class CRReport : System.Web.UI.Page
    {
        string Database = "EEIR_OIL";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { 
            
            }
        }

        private void ToPrintPage(CReportInfo CRInfo)
        {
            CYReportSource.Report.FileName = CRInfo.ReportFile;
            CYReportSource.ReportDocument.SetDataSource(CRInfo.ReportData);

            if (CRInfo.SubReportDataSource.Count > 0)
            {
                foreach (SubReportData srData in CRInfo.SubReportDataSource)
                {
                    CYReportSource.ReportDocument.Subreports[srData.SubReportName].SetDataSource(srData.DataSource);
                }
            }

            if (CRInfo.ReportParm.Count > 0)
            {
                foreach (var Parm in CRInfo.ReportParm)
                {
                    ParameterField ParmFieldSet = CYReportViewer.ParameterFieldInfo[Parm.Key];
                    ParameterDiscreteValue ParmValueSet = new ParameterDiscreteValue();
                    ParmValueSet.Value = Parm.Value;
                    
                    ParmFieldSet.CurrentValues.Clear();
                    ParmFieldSet.CurrentValues.Add(ParmValueSet);
                }
            }

            CYReportViewer.ReportSourceID = CYReportSource.ID;
            var configstring = CommSetup.CommWebSetup.DB0_CodeString;

            string[] DataConnectionInfo = configstring.Split(',');

            ConnectionInfo ci = new ConnectionInfo() {
                ServerName = DataConnectionInfo[0],
                UserID = DataConnectionInfo[1],
                Password = DataConnectionInfo[2] ,
                DatabaseName = Database
            };
            foreach (TableLogOnInfo cnInfo in CYReportViewer.LogOnInfo)
                cnInfo.ConnectionInfo = ci;
        }
    }
}