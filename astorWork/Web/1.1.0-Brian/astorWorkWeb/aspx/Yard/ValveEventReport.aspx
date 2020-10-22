<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="ValveEventReport.aspx.cs" Inherits="astorWork.aspx.Yard.ValveEventReport" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="rcb1" runat="server">
        <script>
            function RequestStart() {
                var loadingPanel = document.getElementById("<%= rlpnl.ClientID %>");
                var pageHeight = document.documentElement.scrollHeight;
                var viewportHeight = document.documentElement.clientHeight;
                if (pageHeight > viewportHeight) {
                    loadingPanel.style.height = pageHeight + "px";
                }

                var pageWidth = document.documentElement.scrollWidth;
                var viewportWidth = document.documentElement.clientWidth;

                if (pageWidth > viewportWidth) {
                    loadingPanel.style.width = pageWidth + "px";
                }
                // the following Javascript code takes care of centering the RadAjaxLoadingPanel
                // background image, taking into consideration the scroll offset of the page content

                if ($telerik.isSafari) {
                    var scrollTopOffset = document.body.scrollTop;
                    var scrollLeftOffset = document.body.scrollLeft;
                }
                else {
                    var scrollTopOffset = document.documentElement.scrollTop;
                    var scrollLeftOffset = document.documentElement.scrollLeft;
                }
                var loadingImageWidth = 55;
                var loadingImageHeight = 55;
                loadingPanel.style.backgroundPosition = (parseInt(scrollLeftOffset) + parseInt(viewportWidth / 2) - parseInt(loadingImageWidth / 2)) + "px " + (parseInt(scrollTopOffset) + parseInt(viewportHeight / 2) - parseInt(loadingImageHeight / 2)) + "px";
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxLoadingPanel ID="rlpnl" runat="server" />
    <telerik:RadAjaxManager ID="ramZone" runat="server" DefaultLoadingPanelID="rlpnl">
        <AjaxSettings>

            <telerik:AjaxSetting AjaxControlID="pnlZone">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlZone" LoadingPanelID="rlpnl" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnlZone" runat="server">
        <div class="layout">

            <table class="formTable">
                <tr>
                    <td class="tdLabel">Select Zone
                    </td>
                    <td class="tdField">
                        <telerik:RadDropDownList AutoPostBack="true" DefaultMessage="--Select zone--"
                            OnSelectedIndexChanged="rddlZone_SelectedIndexChanged" ID="rddlZone" runat="server">
                        </telerik:RadDropDownList>

                        <asp:RequiredFieldValidator ValidationGroup="valve" ForeColor="Red"
                            Display="Dynamic" ID="rfvZone" ControlToValidate="rddlZone" runat="server" ErrorMessage="Select Zone">*</asp:RequiredFieldValidator>
                    </td>
                    <td class="tdLabel">Select Valve
                    </td>
                    <td class="tdField">
                        <telerik:RadDropDownList DefaultMessage="--Select Valve--"
                            ID="rddlValve" runat="server">
                        </telerik:RadDropDownList>

                        <asp:RequiredFieldValidator ValidationGroup="valve" ForeColor="Red"
                            Display="Dynamic" ID="rfvValve" ControlToValidate="rddlValve" runat="server" ErrorMessage="Select Valve">*</asp:RequiredFieldValidator>
                    </td>
                    <td class="tdLabel">Month and Year
                    </td>
                    <td class="tdField">
                        <table>
                            <tr>
                                <td>
                                    <telerik:RadMonthYearPicker ID="rmyPicker"  runat="server"></telerik:RadMonthYearPicker>
                                </td>
                                <td>
                                     <asp:RequiredFieldValidator ID="rfvMonth" ValidationGroup="valve" Display="Dynamic" ControlToValidate="rmyPicker"
                            runat="server" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                        
                       
                    </td>

                </tr>
                <tr>
                    <td class="tdControl" align="center" colspan="6">
                        <telerik:RadButton ID="rbnClear" runat="server" SkinID="rbnCancel" Text="Clear" OnClick="rbnClear_Click">
                        </telerik:RadButton>
                        <telerik:RadButton ID="rbnReport" runat="server" ValidationGroup="valve" OnClick="rbnReport_Click" SkinID="rbnHistory" Text="Search">
                        </telerik:RadButton>

                    </td>
                </tr>
            </table>
            <br />

            <asp:Panel ID="HtmlChartHolder" runat="server"></asp:Panel>
            <telerik:RadHtmlChart Legend-Appearance-Position="Left" runat="server" ID="rhcEvents" Width="100%" Height="480px" Visible="false">
                <PlotArea>

                    <Series>
                        <telerik:ColumnSeries DataFieldY="OpenStatus" Name="Open">
                            <Appearance>
                                <FillStyle BackgroundColor="Red"></FillStyle>
                            </Appearance>
                            <LabelsAppearance Visible="false" DataFormatString="StatusText" DataField="StatusText">
                            </LabelsAppearance>
                            <TooltipsAppearance Color="White" ClientTemplate="#=dataItem.StatusText# at #=dataItem.Timestamp#" />
                        </telerik:ColumnSeries>
                        <telerik:ColumnSeries DataFieldY="CloseStatus" Name="Close">
                            <Appearance>
                                <FillStyle BackgroundColor="Green"></FillStyle>
                            </Appearance>
                            <LabelsAppearance Visible="false" DataField="StatusText">
                            </LabelsAppearance>
                            <TooltipsAppearance Color="White" ClientTemplate="#=dataItem.StatusText# at #=dataItem.Timestamp#" />
                        </telerik:ColumnSeries>

                    </Series>
                    <XAxis DataLabelsField="Timestamp">
                        <TitleAppearance Text="Captured DateTime">
                        </TitleAppearance>
                        <LabelsAppearance RotationAngle="-90"></LabelsAppearance>
                    </XAxis>
                    <YAxis Step="0.5" Name="StatusText">
                        <LabelsAppearance Visible="false"></LabelsAppearance>
                        <TitleAppearance Text="Status">
                        </TitleAppearance>
                        <MajorGridLines Color="#EFEFEF" Width="1"></MajorGridLines>
                        <MinorGridLines Color="#F7F7F7" Width="1"></MinorGridLines>
                    </YAxis>
                </PlotArea>
                <ChartTitle>
                </ChartTitle>
                <Legend>
                    <Appearance BackgroundColor="White" Position="Top" Align="End" Orientation="Vertical" Width="0" Height="0" OffsetX="0" OffsetY="0">
                    </Appearance>
                </Legend>
            </telerik:RadHtmlChart>
        </div>
    </asp:Panel>
</asp:Content>
