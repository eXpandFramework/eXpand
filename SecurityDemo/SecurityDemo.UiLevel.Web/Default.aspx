<%@ Page Language="C#" AutoEventWireup="true" Inherits="Default" EnableViewState="false"
    ValidateRequest="false" CodeBehind="Default.aspx.cs" %>
<%@ Register Assembly="DevExpress.Web.v12.1" Namespace="DevExpress.Web.ASPxRoundPanel"
    TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v12.1" Namespace="DevExpress.Web.ASPxEditors"
    TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v12.1" Namespace="DevExpress.Web.ASPxPanel"
    TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v12.1" Namespace="DevExpress.Web.ASPxSplitter"
    TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v12.1" Namespace="DevExpress.Web.ASPxGlobalEvents"
    TagPrefix="dxge" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Main Page</title>
    <meta http-equiv="Expires" content="0" />
</head>
<script type="text/javascript" src="MoveFooter.js"> </script>
<script type="text/javascript" src="TemplateScripts.js"> </script>
<body class="HorizontalTemplate BodyBackColor">
    <form id="form2" runat="server">
    <dxge:ASPxGlobalEvents ID="GE" ClientSideEvents-EndCallback="AdjustSize" runat="server" />
    <cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
    <div runat="server">
        <cc3:XafUpdatePanel ID="UPPopupWindowControl" runat="server">
            <cc4:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
        </cc3:XafUpdatePanel>
        <table id="MT" border="0" width="100%" cellpadding="0" cellspacing="0" class="dxsplControl_<%= CurrentTheme %>">
            <tbody>
                <tr>
                    <td style="vertical-align: top; height: 10px;" class="dxsplPane_<%= CurrentTheme %>">
                        <div id="HorizontalTemplateHeader" class="HorizontalTemplateHeader" style="width: 100%;">
                            <table cellpadding="0" cellspacing="0" border="0" class="Top" width="100%">
                                <tr>
                                    <td class="Logo">
                                        <asp:HyperLink runat="server" NavigateUrl="#" ID="LogoLink">
                                            <cc4:ThemedImageControl ID="TIC" ImageName="Logo.png" DefaultThemeImageLocation="App_Themes/{0}/Xaf"
                                                runat="server" BorderWidth="0px" />
                                        </asp:HyperLink>
                                    </td>
                                    <td class="Security">
                                        <cc3:XafUpdatePanel ID="UPSAC" runat="server">
                                        <cc2:ActionContainerHolder runat="server" ID="SAC" Categories="Security" ContainerStyle="Links"
                                            ImageTextStyle="CaptionAndImage" CssClass="Security" SeparatorHeight="23px" ShowSeparators="True" />
                                        </cc3:XafUpdatePanel>
                                    </td>
                                </tr>
                            </table>
                            <cc3:XafUpdatePanel ID="UPNTAC" runat="server">
                            <cc2:NavigationTabsActionContainer ID="NTAC" runat="server" ContainerId="ViewsNavigation"
                                CssClass="NavigationTabsActionContainer">
                                <SpaceAfterTabsTemplate>
                                    <cc2:ActionContainerHolder ID="VN" runat="server" Categories="RootObjectsCreation;Appearance;Search;FullTextSearch"
                                        ContainerStyle="Links" CssClass="TabsContainer" SeparatorHeight="15px" />
                                </SpaceAfterTabsTemplate>
                            </cc2:NavigationTabsActionContainer>
                            </cc3:XafUpdatePanel>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top">
                        <table id="MRC" style="width: 100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td id="LPcell" style="width: 200px; vertical-align: top;">
                                    <div id="LP" class="LeftPane" style="width: 200px;">
                                        <cc3:XafUpdatePanel ID="UPLP" runat="server">
                                        <cc2:ActionContainerHolder ID="VTC" runat="server" Orientation="Vertical" Categories="Tools"
                                            BorderWidth="0px" ContainerStyle="Links" ShowSeparators="False" />
                                        <cc2:ActionContainerHolder ID="DAC" runat="server" Orientation="Vertical" Categories="Diagnostic"
                                            BorderWidth="0px" ContainerStyle="Links" ShowSeparators="False" />
                                        <br />
                                        </cc3:XafUpdatePanel>
                                    </div>
                                </td>
                                <td id="separatorCell" style="width: 6px; border-bottom-style: none; border-top-style: none;
                                    display: none" class="dxsplVSeparator_<%= CurrentTheme %> dxsplPane_<%= CurrentTheme %>">
                                    <div id="separatorButton" class="dxsplVSeparatorButton_<%= CurrentTheme %>" onmouseenter="OnMouseEnter('separatorButton')"
                                        onmouseleave="OnMouseLeave('separatorButton')" onclick="OnClick('LPcell', 'separatorImage', true)">
                                        <div id="separatorImage" style="width: 6px;" class="dxWeb_splVCollapseBackwardButton_<%= CurrentTheme %>">
                                        </div>
                                    </div>
                                </td>
                                <td style="vertical-align: top;">
                                    <table style="width: 100%;" cellpadding="0" cellspacing="0">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <cc3:XafUpdatePanel ID="UPTB" runat="server">
                                                    <cc2:ActionContainerHolder CssClass="ACH MainToolbar" runat="server" ID="TB" ContainerStyle="ToolBar"
                                                        Orientation="Horizontal" Categories="ObjectsCreation;Edit;RecordEdit;View;Export;Reports;Filters">
                                                        <Menu Width="100%" ItemAutoWidth="False" ClientInstanceName="mainMenu">
                                                            <BorderTop BorderStyle="None" />
                                                            <BorderLeft BorderStyle="None" />
                                                            <BorderRight BorderStyle="None" />
                                                        </Menu>
                                                    </cc2:ActionContainerHolder>
                                                    </cc3:XafUpdatePanel>
                                                    <cc3:XafUpdatePanel ID="UPVH" runat="server">
                                                        <table id="VH" border="0" cellpadding="0" cellspacing="0" class="MainContent"
                                                            width="100%">
                                                        <tr>
                                                            <td class="ViewHeader">
                                                                <table cellpadding="0" cellspacing="0" border="0" width="100%" class="ViewHeader">
                                                                    <tr>
                                                                        <td class="ViewImage">
                                                                            <cc4:ViewImageControl ID="VIC" runat="server" />
                                                                        </td>
                                                                        <td class="ViewCaption">
                                                                            <h1>
                                                                                    <cc4:ViewCaptionControl ID="VCC" runat="server" />
                                                                            </h1>
                                                                            <cc2:NavigationHistoryActionContainer ID="VHC" runat="server" CssClass="NavigationHistoryLinks"
                                                                                ContainerId="ViewsHistoryNavigation" Delimiter=" / " />
                                                                        </td>
                                                                        <td align="right">
                                                                            <cc2:ActionContainerHolder runat="server" ID="RNC" ContainerStyle="Links" Orientation="Horizontal"
                                                                                Categories="RecordsNavigation" UseLargeImage="True" ImageTextStyle="Image" CssClass="RecordsNavigationContainer">
                                                                                <Menu Width="100%" ItemAutoWidth="False" HorizontalAlign="Right" />
                                                                            </cc2:ActionContainerHolder>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    </cc3:XafUpdatePanel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <cc3:XafUpdatePanel ID="UPEMA" runat="server">
                                                    <cc2:ActionContainerHolder runat="server" ID="EMA" ContainerStyle="Links" Orientation="Horizontal"
                                                        Categories="Save;UndoRedo" CssClass="EditModeActions">
                                                        <Menu Width="100%" ItemAutoWidth="False" HorizontalAlign="Right" />
                                                    </cc2:ActionContainerHolder>
                                                    </cc3:XafUpdatePanel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div id="CP" style="overflow: auto; width: 100%;">
                                                        <table border="0" cellpadding="0" cellspacing="0" class="MainContent" width="100%">
                                                            <tr class="Content">
                                                                <td class="Content">
                                                                    <cc3:XafUpdatePanel ID="UPEI" runat="server">
                                                                    <tc:ErrorInfoControl ID="ErrorInfo" Style="margin: 10px 0px 10px 0px" runat="server">
                                                                    </tc:ErrorInfoControl>
                                                                    </cc3:XafUpdatePanel>
                                                                    <cc3:XafUpdatePanel ID="UPVSC" runat="server">
                                                                    <cc4:ViewSiteControl ID="VSC" runat="server" />
                                                                    <cc2:ActionContainerHolder runat="server" ID="EditModeActions2" ContainerStyle="Links"
                                                                        Orientation="Horizontal" Categories="Save;UndoRedo" CssClass="EditModeActions">
                                                                        <Menu Width="100%" ItemAutoWidth="False" HorizontalAlign="Right" Paddings-PaddingTop="15px" />
                                                                    </cc2:ActionContainerHolder>
                                                                    </cc3:XafUpdatePanel>
                                                                    <div id="Spacer" class="Spacer">
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr class="Content">
                                                                <td class="Content Links" align="center" style="text-align: center">
																	<span style="display: inline-block">
                                                                    <cc3:XafUpdatePanel ID="UPQC" runat="server">
                                                                    <cc2:QuickAccessNavigationActionContainer CssClass="NavigationLinks" ID="QC" runat="server"
                                                                        ContainerId="ViewsNavigation" ImageTextStyle="Caption" ShowSeparators="True" />
                                                                    </cc3:XafUpdatePanel>
																	</span>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="height: 20px; vertical-align: bottom" class="BodyBackColor">
                        <cc3:XafUpdatePanel ID="UPIMP" runat="server">
                        <asp:Literal ID="InfoMessagesPanel" runat="server" Text="" Visible="False"></asp:Literal>
                        </cc3:XafUpdatePanel>
                        <div id="Footer" class="Footer">
                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                <tr>
                                    <td align="left">
                                        <div class="FooterCopyright">
                                            <cc4:AboutInfoControl ID="AIC" runat="server">Copyright text</cc4:AboutInfoControl>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
