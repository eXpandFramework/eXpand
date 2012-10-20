<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="DialogPage" EnableViewState="false"
    ValidateRequest="false" CodeBehind="Dialog.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dialog Page</title>
    <meta http-equiv="Expires" content="0" />
</head>
<body class="Dialog">
    <div id="PageContent" class="PageContent DialogPageContent">
        <form id="form1" runat="server">
        <cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
        <cc3:XafUpdatePanel ID="UPVH" runat="server">
            <div style="display: none">
                <cc4:ViewImageControl ID="viewImageControl" runat="server" Control-UseLargeImage="false" />
                <cc4:ViewCaptionControl ID="viewCaptionControl" DetailViewCaptionMode="ViewCaption"
                    runat="server" />
            </div>
        </cc3:XafUpdatePanel>
        <table class="DialogContent Content" border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="ContentCell">
                    <cc3:XafUpdatePanel ID="UPEI" runat="server">
                        <tc:ErrorInfoControl ID="ErrorInfo" Style="margin: 10px 0px 10px 0px" runat="server" />
                    </cc3:XafUpdatePanel>
                    <asp:Table ID="Table1" runat="server" Width="100%" BorderWidth="0px" CellPadding="0"
                        CellSpacing="0">
                        <asp:TableRow ID="TableRow5" runat="server">
                            <asp:TableCell runat="server" ID="TableCell1" HorizontalAlign="Center">
                                <cc3:XafUpdatePanel ID="UPSAC" runat="server">
                                    <cc2:ActionContainerHolder ID="SearchActionContainer" runat="server" Categories="Search;FullTextSearch"
                                        CssClass="HContainer" Orientation="Horizontal" ContainerStyle="Buttons" />
                                </cc3:XafUpdatePanel>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow2" runat="server">
                            <asp:TableCell runat="server" ID="ViewSite">
                                <cc3:XafUpdatePanel ID="UPVSC" runat="server">
                                    <cc4:ViewSiteControl ID="viewSiteControl" runat="server" />
                                </cc3:XafUpdatePanel>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
            <asp:TableRow ID="TableRow3" runat="server">
                <asp:TableCell runat="server" ID="TableCell4" HorizontalAlign="Right" Style="padding-bottom: 10px">
                    <cc3:XafUpdatePanel ID="UPACH" runat="server">
                        <cc2:ActionContainerHolder runat="server" ID="actionContainerHolder" ContainerStyle="Buttons"
                            Orientation="Horizontal" ImageTextStyle="CaptionAndImage" Categories="ObjectsCreation;PopupActions;Diagnostic">
                            <Menu Width="100%" ItemAutoWidth="False" HorizontalAlign="Right" />
                        </cc2:ActionContainerHolder>
                    </cc3:XafUpdatePanel>
                </asp:TableCell>
            </asp:TableRow>
        </table>
        </form>
    </div>
</body>
</html>
