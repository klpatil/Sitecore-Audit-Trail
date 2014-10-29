<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ForSite.aspx.cs" Inherits="SC7.Web.Tools.AuditTrail.ForSite" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <title>Sitecore AuditTrail ForSite Viewer</title>
    <link rel="stylesheet" type="text/css" href="Resources/bootstrap.min.css" />

    <link rel="stylesheet" type="text/css" href="Resources/DT_bootstrap.css">
    <script type="text/javascript" charset="utf-8" language="javascript" src="Resources/jquery.js"></script>
    <script type="text/javascript" charset="utf-8" language="javascript" src="Resources/jquery.dataTables.js"></script>
    <script type="text/javascript" charset="utf-8" language="javascript" src="Resources/DT_bootstrap.js"></script>

    <script type="text/javascript" charset="utf-8" language="javascript" src="Resources/AuditTrail.js"></script>
</head>
<body>
    <form id="frmSCAuditTrailForSite" runat="server">
        <div class="container" style="margin-top: 30px">

            <center>
                <asp:Label ID="lblMessage" runat="server" Text="" 
                    role="alert"
                    CssClass="text-info" Font-Bold="false" Visible="false" />
            </center>
            

            <asp:Table ID="tblSCAuditTrailForSite"
                CellPadding="0" CellSpacing="0" runat="server"
                CssClass="table table-striped table-bordered" Visible="false">
                <asp:TableHeaderRow TableSection="TableHeader">
                    <asp:TableHeaderCell>What?</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Who?</asp:TableHeaderCell>
                    <asp:TableHeaderCell>When?</asp:TableHeaderCell>
                    <asp:TableHeaderCell>ID</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Language</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Version</asp:TableHeaderCell>

                </asp:TableHeaderRow>
            </asp:Table>
           
        </div>
    </form>
</body>
</html>
