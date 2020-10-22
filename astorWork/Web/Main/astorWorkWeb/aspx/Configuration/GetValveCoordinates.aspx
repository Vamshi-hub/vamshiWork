<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="GetValveCoordinates.aspx.cs"
    Inherits="astorWork.aspx.Configuration.GetValveCoordinates" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock runat="server">
        <script type="text/javascript">
            function GetCoordinate(event) {
                var obj = document.getElementById("<%= rimgZone.ClientID %>");
                var p = {};
                p.x = obj.offsetLeft;
                p.y = obj.offsetTop;
                while (obj.offsetParent) {
                    p.x = p.x + obj.offsetParent.offsetLeft;
                    p.y = p.y + obj.offsetParent.offsetTop;
                    if (obj == document.getElementsByTagName("body")[0]) {
                        break;
                    }
                    else {
                        obj = obj.offsetParent;
                    }
                }
                var oWnd = GetRadWindow();
                oWnd.BrowserWindow.CloseCordinatesWindow(parseInt(p.x) + ',' + parseInt(p.y));
            }

            function DisplayCoordinate(event) {
                var obj = document.getElementById("<%= rimgZone.ClientID %>");
                var p = {};
                p.x = obj.offsetLeft;
                p.y = obj.offsetTop;
                while (obj.offsetParent) {
                    p.x = p.x + obj.offsetParent.offsetLeft;
                    p.y = p.y + obj.offsetParent.offsetTop;
                    if (obj == document.getElementsByTagName("body")[0]) {
                        break;
                    }
                    else {
                        obj = obj.offsetParent;
                    }
                }

                var t = document.getElementById("hCoordinate");
                t.innerHTML = parseInt(p.x) + ',' + parseInt(p.y);
            }

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }
        </script>

    </telerik:RadCodeBlock>
    <style type="text/css">
        div.RadToolTip .rtWrapper td.rtWrapperContent {
            padding: 0;
        }

        .cursor {
            cursor: move;
        }
       
        div.RadToolTip_Default table.rtWrapper td.rtWrapperContent {
            background-color: transparent !important;
        }
        #ContentPlaceHolder1_rimgZoneLayOut {
            margin-top:2px;
        }
    </style>
    <asp:HiddenField runat="server" ID="hdOrientation" />
    <telerik:RadToolTip runat="server" ID="rttYard" TargetControlID="rimgZoneLayOut" RelativeTo="Mouse"
        OffsetY="1" OffsetX="1" AutoCloseDelay="0" HideDelay="0" MouseTrailing="true">

        <telerik:RadBinaryImage Style="border: none" Width="50px" Height="40px" runat="server" ID="rimgZone" CssClass="cursor" />
    </telerik:RadToolTip>
    <div runat="server" id="divImageMapper" style="position: absolute; width: 100%; height: 100%;">
        <telerik:RadBinaryImage runat="server" ID="rimgZoneLayOut"  CssClass="cursor" onclick="GetCoordinate(event)" onmousemove="DisplayCoordinate(event);" />
        <div style="position: fixed; top: 8%; left: 2%;">
            <h1 id="hCoordinate" style="font-size: large">0,0</h1>
        </div>
    </div>


</asp:Content>

