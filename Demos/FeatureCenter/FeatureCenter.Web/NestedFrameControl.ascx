<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="NestedFrameControl"
    CodeBehind="NestedFrameControl.ascx.cs" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v12.1" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>
<div class="NestedFrame">
    <cc3:XafUpdatePanel ID="UPToolBar" CssClass="ToolBarUpdatePanel" runat="server">
        <cc2:ActionContainerHolder runat="server" ID="ToolBar" CssClass="ToolBar" ContainerStyle="ToolBar"
            ImageTextStyle="CaptionAndImage" Orientation="Horizontal" Categories="ObjectsCreation;Link;Edit;RecordEdit;View;Reports;Export;Diagnostic;Filters"
            Menu-Width="100%" Menu-ItemAutoWidth="False" />
    </cc3:XafUpdatePanel>
    <cc4:ViewSiteControl ID="viewSiteControl" runat="server" Control-CssClass="NestedFrameViewSite" />
</div>
