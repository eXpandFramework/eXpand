<%@ Page Language="C#" AutoEventWireup="true" Inherits="LoginPage" Codebehind="Login.aspx.cs" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v9.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers" TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v9.2" Namespace="DevExpress.ExpressApp.Web.Templates.Controls" TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v9.2" Namespace="DevExpress.ExpressApp.Web.Controls" TagPrefix="cc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Logon</title>
</head>
<body class="Dialog" style="background: none white">
   <div id="PageContent" class="PageContent DialogPageContent">
    <script src="MoveFooter.js" type="text/javascript"></script>
	<form id="form1" runat="server">
    	<cc4:ProgressControl ID="ProgressControl" runat="server" ImageName="~/Images/Progress.gif" CssClass="Progress" Text="" />
        <div class="Header">
            <table cellpadding="0" cellspacing="0" border="0">
                <tr><td class="ViewImage">
                        <asp:Image ID="ViewImage" runat="server" /></td>
                    <td class="ViewCaption">
                        <h1><asp:Literal ID="ViewCaptionLabel" runat="server" Text="Log On"/></h1>
                    </td>
                </tr>
            </table>
        </div>

        <table class="DialogContent Content" border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="LogonContentCell" align="center">
                    <tc:ErrorInfoControl ID="ErrorInfo" style="margin: 10px 0px 10px 0px" runat="server">
                    </tc:ErrorInfoControl>
                    <asp:Table ID="Table1" CssClass="Logon" runat="server" BorderWidth="0px" CellPadding="0" CellSpacing="0">
                        <asp:TableRow ID="TableRow2" runat="server">
                            <asp:TableCell runat="server" ID="ViewSite" >view's control place holder</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow3" runat="server">
                            <asp:TableCell runat="server" ID="TableCell4" HorizontalAlign="Right" style="padding: 20px 0px 20px 0px" >
                                <cc2:WebActionContainer ID="PopupActions" runat="server" ContainerId="PopupActions" CssClass="HContainer" Style="margin-left:10px;display:inline" Orientation="Horizontal"/>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
          </table>
	</form>
        <div id="Spacer" class="Spacer"></div>
    <script type="text/javascript">
    <!--
	    function OnLoad() {
	        DXMoveFooter();
            DXattachEventToElement(document.getElementById('formTable'), "resize", DXWindowOnResize);
            DXattachEventToElement(window, "resize", DXWindowOnResize);
            detachElementEvent(document.body, "keypress", disableEnterKey);
        }
    //-->	    
    </script>
    </div>
</body>

</html>
