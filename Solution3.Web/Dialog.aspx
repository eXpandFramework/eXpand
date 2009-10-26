<%@ Register Assembly="DevExpress.ExpressApp.Web.v9.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers" TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v9.2" Namespace="DevExpress.ExpressApp.Web.Templates.Controls" TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v9.2" Namespace="DevExpress.ExpressApp.Web.Controls" TagPrefix="cc4" %>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="DialogPage" EnableViewState="false" validateRequest="false" Codebehind="Dialog.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Dialog Page</title>
	<meta http-equiv="Expires" content="0" />
</head>
<body class="Dialog" style="background: none white">
   <div id="PageContent" class="PageContent DialogPageContent">
	<form id="form1" runat="server">
    	<cc4:ProgressControl ID="ProgressControl" runat="server" ImageName="~/Images/Progress.gif" CssClass="Progress" Text="" />
        <div class="Header">
            <table cellpadding="0" cellspacing="0" border="0">
                <tr><td class="ViewImage">
                        <asp:Image ID="ViewImage" runat="server" /></td>
                    <td class="ViewCaption">
                        <h1><asp:Label ID="ViewCaptionLabel" runat="server" Text="Contact list"></asp:Label></h1>
                    </td>
                </tr>
            </table>
        </div>

        <table class="DialogContent Content" border="0" cellpadding="0" cellspacing="0" width="100%"><tr><td class="ContentCell">
            <tc:ErrorInfoControl ID="ErrorInfo" style="margin: 10px 0px 10px 0px" runat="server">
            </tc:ErrorInfoControl>
            <asp:Table ID="Table1" runat="server" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="0">
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell runat="server" ID="TableCell1" HorizontalAlign="Center" style="padding-bottom:20px;">
			            <cc2:WebActionContainer ID="SearchActionContainer" runat="server" ContainerId="Search" CssClass="HContainer" Orientation="Horizontal">
			            </cc2:WebActionContainer>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell runat="server" ID="ViewSite" >view's control place holder</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow3" runat="server">
                    <asp:TableCell runat="server" ID="TableCell4" HorizontalAlign="Right" style="padding: 20px 0px 20px 0px" >
                        <cc2:ActionContainerHolder runat="server" ID="Holder1" ContainerStyle="Buttons" Orientation="Horizontal" ImageTextStyle="CaptionAndImage">
                            <cc2:WebActionContainer ID="ObjectsCreationActionContainer" runat="server" CssClass="HContainer" ContainerId="ObjectsCreation" Style="margin-right:0px;display:inline" Orientation="Horizontal"/>
                            <cc2:WebActionContainer ID="PopupActions" runat="server" ContainerId="PopupActions" CssClass="HContainer" Style="margin-left:10px;display:inline" Orientation="Horizontal"/>
                            <cc2:WebActionContainer ID="DiagnosticActionContainer" runat="server" ContainerId="Diagnostic" CssClass="HContainer" Style="margin-left:10px;display:inline" Orientation="Horizontal"/>
                        </cc2:ActionContainerHolder>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            </td></tr>
          </table>
	</form>
</div>
</body>
</html>
