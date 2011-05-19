<%@ Page Language="C#" AutoEventWireup="true" Inherits="Default" EnableViewState="false" validateRequest="false" CodeBehind="Default.aspx.cs" %>
<%@ Register Assembly="DevExpress.Web.v11.1" Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v11.1" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v11.1" Namespace="DevExpress.Web.ASPxSplitter" TagPrefix="dxsp" %>
<%@ Register Assembly="DevExpress.Web.v11.1" Namespace="DevExpress.Web.ASPxCallbackPanel" TagPrefix="dxcp" %>
<%@ Register Assembly="DevExpress.Web.v11.1" Namespace="DevExpress.Web.ASPxGlobalEvents" TagPrefix="dxge" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v11.1" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers" TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v11.1" Namespace="DevExpress.ExpressApp.Web.Controls" TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v11.1" Namespace="DevExpress.ExpressApp.Web.Templates.Controls" TagPrefix="tc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Main Page</title>
	<meta http-equiv="Expires" content="0" />
</head>
<body onload="OnLoad()" class="VerticalTemplate">
<link href="skin.css" rel="stylesheet" type="text/css" />
    <div id="PageContent" class="PageContent">
    <script src="MoveFooter.js" type="text/javascript"></script>
	<form id="form1" runat="server">
	<dxge:ASPxGlobalEvents ClientSideEvents-EndCallback="function(s, e) { DXUpdateSplitterSize();DXMoveFooter(); }" ID="GlobalEvents" runat="server" />
	<cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
        <table cellpadding="0" cellspacing="0" border="0" class="Top" width="100%">
            <tr>
                <td class="Logo">
                    <asp:HyperLink runat="server" NavigateUrl="~/" ID="LogoLink">
                        <cc4:ThemedImageControl ID="themedImageControl" DefaultThemeImageLocation="App_Themes/{0}/Xaf" ImageName="Logo.png" BorderWidth="0px" runat="server" />
                    </asp:HyperLink>
                </td>
                <td class="Security">
                    <cc2:ActionContainerHolder runat="server" ID="SecurityActionContainer" CssClass="Security" Categories="Security" ContainerStyle="Links" SeparatorHeight="23px" ShowSeparators="True"/>
                </td>
            </tr>
        </table>
        <table border="0" cellpadding="0" cellspacing="0" width="100%" class="ACPanel">
            <tr class="Content">
	            <td class="Content WithPaddings" align="right">
                    <cc2:ActionContainerHolder  ID="SearchActionContainer" runat="server" Categories="RootObjectsCreation;Appearance;Search;FullTextSearch" ContainerStyle="Links" CssClass="TabsContainer" SeparatorHeight="15px" />
                </td>
            </tr>
        </table>
         <dxsp:ASPxSplitter ID="Splitter" runat="server" Height="500px" Width="100%" SaveStateToCookiesID="SeparatorPos" SaveStateToCookies="True" ClientInstanceName="splitter">
             <Panes>
                 <dxsp:SplitterPane Name="Left" ShowCollapseBackwardButton="True" Size="300px" PaneStyle-Paddings-Padding="1px" PaneStyle-BorderLeft-BorderStyle="None">
                     <ContentCollection>
                         <dxsp:SplitterContentControl ID="SplitterContentControl1" runat="server">
                            <div id="LeftPane" class="LeftPane">
	                            <cc2:NavigationActionContainer ID="NavigationBarActionContainer" runat="server" CssClass="xafNavigationBarActionContainer" ContainerId="ViewsNavigation" AutoCollapse="True"/>
	                            <div id="ToolsPanel" class="ToolsActionContainerPanel">
                                    <dxrp:ASPxRoundPanel ID="ToolsRoundPanel" runat="server" Width="100%" HeaderText="Tools">
                                     <PanelCollection>
                                         <dxrp:PanelContent ID="PanelContent1" runat="server">
                                               <cc2:ActionContainerHolder id="VerticalToolsActionContainer" runat="server" Categories="Tools" Orientation="Vertical" ContainerStyle="Links" ShowSeparators="False"/>
                                         </dxrp:PanelContent>
                                     </PanelCollection>
                                    </dxrp:ASPxRoundPanel>
                                </div>
                                <cc2:ActionContainerHolder ID="DiagnosticActionContainer" runat="server" Orientation="Vertical" Categories="Diagnostic" BorderWidth="0px" ContainerStyle="Links" ShowSeparators="False" />
                            </div>
                         </dxsp:SplitterContentControl>
                     </ContentCollection>
                 </dxsp:SplitterPane>
                 <dxsp:SplitterPane Name="Content" PaneStyle-Paddings-Padding="0px">
                     <ContentCollection>
                         <dxsp:SplitterContentControl ID="SplitterContentControl3" runat="server">
                            <div id="ContentPane">
                                <cc2:ActionContainerHolder runat="server" ID="ToolBar" CssClass="ToolBar" ContainerStyle="ToolBar" Orientation="Horizontal"  Categories="ObjectsCreation;Edit;RecordEdit;View;Export;Reports;Filters">
                                    <Menu Width="100%" ItemAutoWidth="False">
                                        <BorderTop BorderStyle="None"/>
                                        <BorderLeft BorderStyle="None"/>
                                        <BorderRight BorderStyle="None"/>
                                    </Menu>
                                </cc2:ActionContainerHolder>
                                <table border="0" cellpadding="0" cellspacing="0" class="MainContent" width="100%">
			                        <tr>
			                            <td class="ViewHeader">
		                                    <table cellpadding="0" cellspacing="0" border="0" width="100%" class="ViewHeader">
			                                    <tr><td class="ViewImage">
					                                    <cc4:ViewImageControl ID="viewImageControl" runat="server" />
					                                </td>
				                                    <td class="ViewCaption">
					                                    <h1><cc4:ViewCaptionControl ID="viewCaptionControl" DetailViewCaptionMode="ViewCaption" runat="server"></cc4:ViewCaptionControl></h1>
					                                    <cc2:NavigationHistoryActionContainer ID="ViewsHistoryNavigationContainer" runat="server" CssClass="NavigationHistoryLinks" ContainerId="ViewsHistoryNavigation" Delimiter=" / "/>
				                                    </td>
		                                            <td align="right">
		                                                <cc2:ActionContainerHolder runat="server" ID="RecordsNavigationContainer" ContainerStyle="Links" Orientation="Horizontal"  Categories="RecordsNavigation" UseLargeImage="True" ImageTextStyle="Image">
		                                                    <Menu Width="100%" ItemAutoWidth="False" HorizontalAlign="Right"/>
		                                                </cc2:ActionContainerHolder>
		                                            </td>
			                                    </tr>
			                                    <tr>
			                                        <td class="EditModeActions" colspan="3">
		                                                <cc2:ActionContainerHolder runat="server" ID="EditModeActions" ContainerStyle="Links" Orientation="Horizontal"  Categories="Save;UndoRedo">
		                                                    <Menu Width="100%" ItemAutoWidth="False" HorizontalAlign="Right"/>
		                                                </cc2:ActionContainerHolder>
			                                        </td>
			                                    </tr>
		                                    </table>
			                            </td>
			                        </tr>
			                        <tr class="Content">
				                        <td class="Content">
					                        <asp:Table ID="viewSiteControlTable" runat="server" Width="100%">
                                            <asp:TableRow ID="TableRow1" runat=server>
                                                    <asp:TableCell ID="TableCell1" runat="server">
                                                        <dxcp:ASPxCallbackPanel id="TopCallBackPanel" runat="server" ClientInstanceName="TopCallBackPanel">
                                                        </dxcp:ASPxCallbackPanel>            
                                                    </asp:TableCell>
                                                </asp:TableRow>
                                                <asp:TableRow ID="row" runat=server>
                                                    <asp:TableCell ID="cell" runat="server">
                                                        <tc:ErrorInfoControl ID="ErrorInfo" style="margin: 10px 0px 10px 0px" runat="server"></tc:ErrorInfoControl>
                                                        <cc4:ViewSiteControl ID="viewSiteControl" runat="server"/>
                                                    </asp:TableCell>
                                                </asp:TableRow>
                                                <asp:TableRow ID="TableRow2" runat=server>
                                                    <asp:TableCell ID="TableCell2" runat="server">
                                                        <dxcp:ASPxCallbackPanel id="BottomCallBackPanel" runat="server" ClientInstanceName="BottomCallBackPanel">
                                                        </dxcp:ASPxCallbackPanel>            
                                                    </asp:TableCell>
                                                </asp:TableRow>
                                            </asp:Table>
                                            <cc2:ActionContainerHolder runat="server" ID="EditModeActions2" ContainerStyle="Links" Orientation="Horizontal"  Categories="Save;UndoRedo">
                                                <Menu Width="100%" ItemAutoWidth="False" HorizontalAlign="Right" Paddings-PaddingTop="15px"/>
                                            </cc2:ActionContainerHolder>
            	                            <div id="Spacer" class="Spacer"></div>
		                                </td>
	                                </tr>
			                        <tr class="Content">
				                        <td class="Content Links" align="center"><cc2:QuickAccessNavigationActionContainer CssClass="NavigationLinks" ID="QuickAccessNavigationActionContainer" runat="server" ContainerId="ViewsNavigation" ImageTextStyle="Caption" ShowSeparators ="True"/></td>
	                                </tr>
                                </table>
                            </div>
                         </dxsp:SplitterContentControl>
                     </ContentCollection>
                 </dxsp:SplitterPane>
                 <dxsp:SplitterPane Name="Right" ShowCollapseForwardButton="True" Size="300px"  PaneStyle-Paddings-Padding="1px" PaneStyle-BorderRight-BorderStyle="None">
                     <ContentCollection>
                         <dxsp:SplitterContentControl ID="SplitterContentControl2" runat="server"/>
                     </ContentCollection>
                 </dxsp:SplitterPane>
             </Panes>
         </dxsp:ASPxSplitter>
	    <asp:Literal ID="InfoMessagesPanel" runat="server" Text="" Visible="False"></asp:Literal>
		<div id="Footer" class="Footer">
		<table cellpadding="0" cellspacing="0" border="0" width="100%"><tr>
		<td align="left"><div class="FooterCopyright">
            <cc4:AboutInfoControl ID="aboutInfoControl" runat="server">Copyright text</cc4:AboutInfoControl></div></td>
		</tr></table></div>
	</form>
	<script type="text/javascript">
	<!--
	    function OnLoad() {
	        DXUpdateSplitterSize();
	        DXMoveFooter();
	        DXattachEventToElement(window, "resize", DXWindowOnResize);
	        DXattachEventToElement(window.LeftPane, "resize", DXUpdateSplitterSize);
	        DXattachEventToElement(window.ContentPane, "resize", DXUpdateSplitterSize);
	    }
    //-->	    
	</script>
	</div>
</body>
</html>
