<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="ViewZoneValves.aspx.cs" Inherits="astorWork.ViewZoneValves" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script runat="server">

        void RegionMap_Clicked(object sender, ImageMapEventArgs e)
        {
            string hotSpotType;

            // When a user clicks a hot spot, display
            // the hot spot's type and name.
            ScriptManager.RegisterStartupScript(this, GetType(), "a1", "alert('" + e.PostBackValue + "');", true);
        }  

    </script>
    <style>
        div#ctl00_ContentPlaceHolder1_RadToolTipManager1RTMPanel {
            border: solid 1px white;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

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
        runat="server" OnAjaxUpdate="OnAjaxUpdate" RelativeTo="Mouse" ToolTipZoneID="imgMapYardView"
        Position="MiddleRight">
    </telerik:RadToolTipManager>

    <asp:Panel runat="server" ID="pnlMain">
        <div runat="server" id="divImageMapper" style="position: absolute; width: 100%; height: 100%; overflow: hidden;">
            <telerik:RadBinaryImage ID="rbZone" runat="server" Style="width: 100% !important; height: 100% !important;" />

        </div>
        <div id="divFooter" runat="server" style="width: 100%; bottom: 5px; position: fixed; right: 10px;"
            align="right">
            <table width="100%" border="0">
                <tr>
                    <td align="left">&nbsp;&nbsp;
                        <asp:ImageButton ImageUrl="~/Images/moreiconarrow.png" ToolTip="Back" PostBackUrl="~/aspx/Yard/Pivot.aspx"
                             runat="server" Width="35px" Height="34px"
                            Style="box-shadow:3px 3px 4px #2c5c8d !important;"
                            ID="imgbnBack" />
                    </td>
                    <td align="right">


                        <asp:Label runat="server" ID="lblCounter" />
                        &nbsp;
                            <asp:ImageButton ImageUrl="~/Images/Reload.png" ToolTip="Reload" OnClientClick="LoadData();" runat="server"
                                ID="imgBtnReload"  />
                        <asp:Button runat="server" ID="btnReload" OnClick="btnReload_Click" Text="clickme"
                            Style="display: none;" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:HiddenField ID="hdnIsOpen" runat="server" Value="false" />
    <telerik:RadCodeBlock ID="rd" runat="server">
        <script language="javascript" type="text/javascript">

            var ReloadDuration = 0;
            var timer = null;
            window.onload = function InitialpageLoad() {
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
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>


