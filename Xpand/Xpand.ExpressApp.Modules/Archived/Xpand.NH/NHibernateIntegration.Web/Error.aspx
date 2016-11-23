<%@ Page Language="c#" AutoEventWireup="false" Inherits="ErrorPage" EnableViewState="false"
    ValidateRequest="false" CodeBehind="Error.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Error</title>
    <meta http-equiv="Expires" content="0" />
</head>
<body class="Dialog Error">
    <div id="PageContent" class="PageContent DialogPageContent">
        <table id="formTable" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td>
                    <form id="form1" runat="server">
                    <div class="Header">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td class="ViewCaption">
                                    <h1>
                                        <asp:Literal ID="ApplicationTitle" runat="server" Text="Test Application" />
                                    </h1>
                                </td>
                                <td class="InfoMessagesPanel">
                                    <asp:Literal ID="InfoMessagesPanel" runat="server" Text=""></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <table class="DialogContent Content" border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td class="ContentCell">
                                <asp:Table ID="Table1" runat="server" Width="100%" BorderWidth="0px" CellPadding="0"
                                    CellSpacing="0">
                                    <asp:TableRow ID="TableRow2" runat="server">
                                        <asp:TableCell runat="server" ID="ViewSite">
                                            <h2 id="FormCaption">
                                                <asp:Literal ID="ErrorTitleLiteral" runat="server" Text="Application Error" /></h2>
                                            <asp:Panel runat="server" ID="ErrorPanel" Width="100%">
                                                <p class="StaticText">
                                                    <asp:PlaceHolder ID="ReportResult" runat="server">Your report was sent. Thank you.<br />
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="ApologizeMessage" runat="server">We are currently unable to serve
                                                        your request:&nbsp;<asp:HyperLink ID="RequestUrl" runat="server">RequestUrl</asp:HyperLink>.<br />
                                                        We apologize, but an error occurred and your request could not be completed.<br />
                                                    </asp:PlaceHolder>
                                                    You could go&nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="javascript:history.go(-1)">back</asp:HyperLink>&nbsp;and
                                                    try again
                                                    <asp:Literal ID="LiteralReturn" runat="server" Text=", or return to the "></asp:Literal>
                                                    <asp:HyperLink ID="HyperLinkReturn" runat="server">previous page</asp:HyperLink>
                                                    , or retry your request:&nbsp;<asp:HyperLink ID="RequestUrl2" runat="server">RequestUrl</asp:HyperLink>.</p>
                                                <asp:Panel ID="Details" runat="server" Width="100%">
                                                    <a style="text-decoration: underline; cursor: hand;" id="ShowErrorDetails" onclick="javascript:ShowDetails();">
                                                        Show Error details</a>
                                                    <div id="DetailsContent" style="display: none;">
                                                        <span class="StaticText" style="font-weight: bold;">Error details<hr width="100%" />
                                                        </span><a style="text-decoration: underline; cursor: hand;" id="HideErrorDetails"
                                                            onclick="javascript:HideDetails();">Hide Error details</a>
                                                        <pre class="ErrorDetails">
                                                            <asp:Literal ID="DetailsText" runat="server" />
                                                        </pre>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="ReportForm" runat="server" Width="100%">
                                                    <span class="StaticText" style="font-weight: bold;">Report error<hr width="100%" />
                                                    </span>
                                                    <p class="StaticText">
                                                        This error has been logged. If you have additional information that you believe
                                                        may have caused this error please report the problem.</p>
                                                    <table cellpadding="0" cellspacing="0" border="0">
                                                        <tr>
                                                            <td align="right" style="padding-bottom: 10px">
                                                                <asp:TextBox ID="DescriptionTextBox" runat="server" Columns="60" Rows="8" TextMode="MultiLine"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Button ID="ReportButton" runat="server" Text="Send Report" OnClick="ReportButton_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </asp:Panel>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </td>
                        </tr>
                    </table>
                    <div id="Spacer" class="Spacer">
                    </div>
                    </form>
                </td>
            </tr>
        </table>
        <script type="text/javascript">
	<!--
            function ShowDetails() {
                document.getElementById('DetailsContent').style.display = 'block';
                document.getElementById('ShowErrorDetails').style.display = 'none';
                return false;
            }
            function HideDetails() {
                document.getElementById('DetailsContent').style.display = 'none';
                document.getElementById('ShowErrorDetails').style.display = 'block';
                return false;
            }
    //-->	    
        </script>
    </div>
</body>
</html>
