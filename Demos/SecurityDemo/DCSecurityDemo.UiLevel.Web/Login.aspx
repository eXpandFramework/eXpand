<%@ Page Language="C#" AutoEventWireup="true" Inherits="LoginPage" CodeBehind="Login.aspx.cs" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Logon</title>
</head>
<body class="Dialog">
   <div id="PageContent" class="PageContent DialogPageContent">
	<form id="form1" runat="server">
    	<cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
    	<div id="Content" runat="server" />
	</form>
        <div id="Spacer" class="Spacer">
        </div>
    <script type="text/javascript">
    <!--
	    function OnLoad() {
	        DXMoveFooter();
            DXattachEventToElement(document.getElementById('formTable'), "resize", DXWindowOnResize);
            DXattachEventToElement(window, "resize", DXWindowOnResize);
        }
    //-->	    
    </script>
    </div>
</body>
</html>
