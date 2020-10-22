<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="Pivot.aspx.cs" Inherits="astorWork.aspx.Yard.Pivot" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnReload">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" LoadingPanelID="RadAjaxLoadingPanel1"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1"></telerik:AjaxUpdatedControl>

                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="RequestEnd" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="MetroTouch">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadToolTipManager ID="RadToolTipManager1" ContentScrolling="Default" OffsetY="-1" HideEvent="LeaveToolTip" Skin="Metro" OnClientBeforeShow="BeforeShow" OnClientHide="BeforeHide"
        runat="server" OnAjaxUpdate="OnAjaxUpdate" RelativeTo="Element" ToolTipZoneID="imgMapYardView"
        Position="MiddleRight">
    </telerik:RadToolTipManager>
    <asp:HiddenField ID="hdfWidth_SAm" runat="server" />
    <asp:Panel runat="server" ID="pnlMain">

        <div runat="server" id="divImageMapper" style="position: absolute; width: 100%; height: 100%; overflow: hidden;">
            <%--  <asp:ImageMap ID="imgMapYardView" runat="server" Width="100%" Height="100%" HotSpotMode="PostBack" OnClick="RegionMap_Clicked"    >
            </asp:ImageMap>--%>
            <asp:ImageMap ID="imgMapYardView" Width="100%" Height="100%"
                AlternateText="Zone regions"
                runat="Server">
            </asp:ImageMap>

            <br />
            <br />

            <asp:Label ID="Message1"
                runat="Server">
            </asp:Label>
        </div>
         <div style="position: fixed; right: 2px;">
            <table>
                <tr>
                    
                    <td align="right">
                        <telerik:RadDropDownList ID="rddlYardMaster" runat="server" Width="170px" AutoPostBack="true" ZIndex="1000000"
                            OnSelectedIndexChanged="rddlYardMaster_SelectedIndexChanged" >
                        </telerik:RadDropDownList>
                    </td>
                </tr>
            </table>

        </div>
        <div id="divFooter" runat="server" style="width: 100%; bottom: 5px; position: fixed; right: 10px;"
            align="right">
            <table width="100%" border="0">
                <tr>
                    <td align="right">
                       
                        <asp:Label runat="server" ID="lblCounter" />
                        &nbsp;
                            <asp:ImageButton ImageUrl="~/Images/Reload.png" OnClientClick="LoadData();" runat="server"
                                ID="imgBtnReload" />
                        <asp:Button runat="server" ID="btnReload" OnClick="btnReload_Click" Text="clickme"
                            Style="display: none;" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
        <telerik:RadWindowManager ID="rwmWorkersIncidents" runat="server">
                <Windows>
                    <telerik:RadWindow runat="server" ID="RadWin_WorkersIncidents" Style="z-index: 1000000;" Behaviors="Close,Reload">
                    </telerik:RadWindow>
                </Windows>
            </telerik:RadWindowManager>
    <asp:HiddenField ID="hdnIsOpen" runat="server" Value="false" />
    <telerik:RadCodeBlock ID="rd" runat="server">
        <script src="../../JScript/jquery1.8.3.min.js"></script>
        <script src="../../JScript/jquery.rwdImageMaps.min.js"></script>
        <script>
            $(document).ready(function (e) {
                $('img[usemap]').rwdImageMaps();
            });

        </script>
        <script language="javascript" type="text/javascript">
            var HiddenWidthField;
            var ReloadDuration = 0;
            var timer = null;
            window.onload = function InitialpageLoad() {
                HiddenWidthField = document.getElementById('<%=hdfWidth_SAm.ClientID%>');
                if (parent.name == "ContentFrame") {
                    parent.CloseLoadingPnl();
                }
                GetReloadDuration();

            };
            function GetReloadDuration() {
                ReloadDuration = 8 * 1000;
                timer = setInterval(function () {
                    if (ReloadDuration > 0) {
                        ReloadDuration -= 1000;
                        SetCounterLabelMsg(ReloadDuration);
                    }
                    else if (ReloadDuration == 0) {
                        LoadData();
                    }
                }, 1000);

            }

            function PauseTimer() {
                var lblCounter = document.getElementById("<%= lblCounter.ClientID  %>");
                lblCounter.innerHTML = 'Loading Data...';
                clearInterval(timer);
            }

            function SetCounterLabelMsg(Counter) {
                var lblCounter = document.getElementById("<%= lblCounter.ClientID  %>");
                if (Counter == 0)
                    lblCounter.innerHTML = 'Loading Data...';
                else
                    lblCounter.innerHTML = Counter / 1000 + 's to next refresh.';
            }


              function ShowWorkerPopup(ZoneId, PageName, ZoneName, YardID, CompanyIDs, DepartmentIDs, TradeIDs) {

                var PopupWindow = $find("<%= RadWin_WorkersIncidents.ClientID %>");
                PopupWindow.set_width(880);
                PopupWindow.set_height(470);
                PopupWindow.setUrl(PageName + "?ZoneID=" + ZoneId + "&ZoneName=" + ZoneName + "&YardID=" + YardID + "&CompanyIDs=" + CompanyIDs + "&DepartmentIDs=" + DepartmentIDs + "&TradeIDs=" + TradeIDs);
                //PopupWindow.maximize();
                PopupWindow.show();
            }
            function LoadData() {

                if (document.getElementById('<%= hdnIsOpen.ClientID %>').value == "false") {
                    document.getElementById('<%= btnReload.ClientID %>').click();

                    CloseLoadingPnl();
                }
            }

            function BeforeShow() {
                document.getElementById('<%= hdnIsOpen.ClientID %>').value = "true";
            }
            function BeforeHide() {
                document.getElementById('<%= hdnIsOpen.ClientID %>').value = "false";
            }
            function CloseLoadingPnl() {
                var currentLoadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                if (currentLoadingPanel != null)
                    currentLoadingPanel.hide("");
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            }
            function RequestStart(sender, eventArgs) {
                document.getElementById('<%= imgBtnReload.ClientID %>').disabled = true;
                PauseTimer();
            }
            function RequestEnd(sender, eventArgs) {
                PauseTimer();
                GetReloadDuration();
                $('img[usemap]').rwdImageMaps();
            }
            function yarGrid_OnClientClicking() {
                PauseTimer();
            }
        </script>
    </telerik:RadCodeBlock>

    <script type="text/javascript" language="javascript">

        function AlertMe(ZoneNAme) {

            alert(ZoneNAme);

        }

    </script>

</asp:Content>
