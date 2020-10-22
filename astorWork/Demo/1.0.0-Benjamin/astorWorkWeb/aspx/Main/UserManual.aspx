<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManual.aspx.cs" Inherits="astorWork.aspx.Main.UserManual" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>User Manual</title>



</head>

<body>

    <form id="form1" runat="server">
        <div id="divUserManual" runat="server">
            <table width="100%">
                <tr>
                    <td>
                        <asp:Label Text="Note: Adobe PDF Viewer is required to view the User Manual. You can download it from " runat="server" Font-Names="Segoe UI,Arial,Helvetica,sans-serif" Font-Size="Smaller" ID="lblAdobelink" />
                        <asp:HyperLink ID="hlnkPdfViewerDownload" runat="server" Font-Names="Segoe UI,Arial,Helvetica,sans-serif" Font-Size="Smaller" Text="here" Target="_blank" NavigateUrl="https://get.adobe.com/reader/" />
                    </td>
                </tr>
                <tr>
                    <td style="padding-left:35px" >
                        <asp:Label Text="Refer to the instructions in this link if manual is not opening in the browser window " runat="server" Font-Names="Segoe UI,Arial,Helvetica,sans-serif" Font-Size="Smaller" ID="lblAdobeAddon" />
                        <asp:HyperLink ID="hlnkPdfViewerDownloadProblem" runat="server" Font-Names="Segoe UI,Arial,Helvetica,sans-serif" Font-Size="Smaller" Text="here" Target="_blank" NavigateUrl="https://helpx.adobe.com/acrobat/using/display-pdf-in-browser.html" />
                    </td>
                </tr>
                <tr>
                    <td>
            <iframe runat="server" id="iframeUserManual" width="100%" height="480px" scrolling="no" />
                    </td>
                </tr>
            </table>
           
        </div>

    </form>
    <div runat="server" id="divPagenotFound" visible="false" style="padding-top: 15%;">
        <table width="100%">
            <tr>
                <td align="center">
                    <asp:Image runat="server" Width="80%" Height="70%" ImageAlign="Middle" ImageUrl="~/Images/Under_Construction.png" />
                </td>
            </tr>
        </table>

    </div>
</body>
</html>

