<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CRReport.aspx.cs" Inherits="DotWeb.WebApp.CRReport" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <CR:CrystalReportViewer ID="CYReportViewer" runat="server" AutoDataBind="true" ReportSourceID="CYReportSource" />

        <CR:CrystalReportSource ID="CYReportSource" runat="server">
        </CR:CrystalReportSource>

    </div>
    </form>
</body>
</html>
