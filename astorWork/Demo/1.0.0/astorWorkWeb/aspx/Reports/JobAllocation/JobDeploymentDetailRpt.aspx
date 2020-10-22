<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobDeploymentDetailRpt.aspx.cs"
    MasterPageFile="~/astorWork.Master" Inherits="astorWork.aspx.Reports.JobAllocation.JobDeploymentDetailRpt" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <script language="javascript" type="text/javascript">
        var objFrmDate = '';
        var objToDate = '';

        function ValidateFromDate(sender, args) {

            var isEmpty = IsDateValueEmpty(objFrmDate);
            if (isEmpty) {
                args.IsValid = false;
                return;
            }
        }

        function ValidateToDate(sender, args) {
            var isEmpty = IsDateValueEmpty(objToDate);
            if (isEmpty) {
                args.IsValid = false;
                return;
            }
        }
        function IsDateValueEmpty(objDateControl) {
            var txtboxID = objDateControl.id + '_textBox';
            var objTextBox = '';
            var count = objDateControl.children.length;
            var i = 0;

            for (i = 0; i < count; i++) {
                if (objDateControl.childNodes[i].type != null && objDateControl.childNodes[i].type == 'text' && objDateControl.childNodes[i].id == txtboxID) {
                    objTextBox = document.getElementById(objDateControl.childNodes[i].id);
                    if (objTextBox.value == '')
                        return true;
                }
            }
            return false;
        }
    </script>
    </asp:Content>
    <asp:Content ID="RawClockTimeReport" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="pnlMainReport">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="pnlMainReport" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" />
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server"></telerik:RadAjaxLoadingPanel>
        <asp:Panel ID="pnlMainReport" runat="server" CssClass="searchFilterPosition">
            <div class="pageLayout">
                <table width="100%">
                    <!-- Date Range Row -->
                    <tr>
                        <td class="tdLabel">
                            <asp:Label ID="lblFromDate" runat="server" Text="From Date" />
                        </td>
                        <td class="tdField" colspan="2">
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td>
                                        <telerik:RadDatePicker ID="clrFromDate" runat="server" Calendar-ShowRowHeaders="false" Width="100px">
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblFromDateFormat" runat="server" ForeColor="Gray" />
                                        <asp:CompareValidator ID="cmpFromDate" runat="server" ErrorMessage="Invalid Date Format"
                                            ControlToValidate="clrFromDate" Operator="DataTypeCheck" Type="Date" Display="Dynamic" />
                                        <asp:RequiredFieldValidator ID="reqFrmDate" runat="server" Display="Dynamic" ErrorMessage="*"
                                            ControlToValidate="clrFromDate" />
                                        <asp:CustomValidator ID="custFromDate" runat="server" ErrorMessage="*" ControlToValidate="clrFromDate"
                                            Display="Dynamic" ClientValidationFunction="ValidateFromDate" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="tdLabel" style="text-align: right; padding-right: 5px;">
                            <asp:Label ID="lblToDate" runat="server" Text="To Date" />
                        </td>
                        <td class="tdField" colspan="2">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <telerik:RadDatePicker ID="clrToDate" runat="server" Calendar-ShowRowHeaders="false" Width="100px">
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblToDateFormat" runat="server" ForeColor="Gray" />
                                        <asp:CompareValidator ID="cmpToDate" runat="server" ErrorMessage="Invalid Date Format"
                                            ControlToValidate="clrToDate" Operator="DataTypeCheck" Type="Date" Display="Dynamic" />
                                        <asp:CompareValidator ID="cvFromDate" runat="server" ControlToCompare="clrToDate"
                                            ControlToValidate="clrFromDate" ErrorMessage="Invalid date range" Operator="LessThanEqual"
                                            Type="Date" Display="Dynamic" SetFocusOnError="False" />
                                        <asp:RequiredFieldValidator ID="reqToDate" runat="server" Display="Dynamic" ErrorMessage="*"
                                            ControlToValidate="clrToDate" />
                                        <asp:CustomValidator ID="custToDate" runat="server" ErrorMessage="*" ControlToValidate="clrToDate"
                                            Display="Dynamic" ClientValidationFunction="ValidateToDate" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                    <td class="tdLabel" width="12%">
                        <asp:Label ID="lblGroupType" runat="server" Text="Report Type"></asp:Label>
                    </td>
                    <td class="tdField" colspan="4">
                        <telerik:RadComboBox ID="rcbReportType" Width="100px" runat="server">
                            <Items>
                                <telerik:RadComboBoxItem Value="1" Text="Detail" />
                                <telerik:RadComboBoxItem Value="2" Text="Summary" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>

                </tr>
                </table>
                <center>
                    <table width="100%" class="table">
                <tr>
                    <td class="tdControl" align="center">
                        <telerik:RadPushButton ID="rpbGenerate" Text="Generate" runat="server" OnClick="rpbGenerate_Click" SkinID="rpButton" />
                    </td>
                </tr>
            </table>
                </center>
                
            </div>
           
            
        </asp:Panel>
        <asp:UpdatePanel ID="updateReport" runat="server">
            <contenttemplate>
            <table id="reportData" style="display: none" width="100%" runat="server">
                <tr>
                    <td>
                        <rsweb:ReportViewer ID="rptvwrJobDeployment" SizeToReportContent="true"  AsyncRendering="true" CssClass="reportheight" runat="server" ShowRefreshButton="false">
                        </rsweb:ReportViewer>
                    </td>
                </tr>
            </table>
        </contenttemplate>
        </asp:UpdatePanel>
        <telerik:RadCodeBlock runat="server">
            <script language="javascript" type="text/javascript">
                function RequestStart() {
                    document.getElementById("<%= reportData.ClientID %>").style.display = 'none';
                var loadingPanel = document.getElementById("<%= RadAjaxLoadingPanel1.ClientID %>");
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
                loadingPanel.style.zIndex = 9000;
                loadingPanel.style.backgroundPosition = (parseInt(scrollLeftOffset) + parseInt(viewportWidth / 2) - parseInt(loadingImageWidth / 2)) + "px " + (parseInt(scrollTopOffset) + parseInt(viewportHeight / 2) - parseInt(loadingImageHeight / 2)) + "px";
            }
            window.onload = function resize() {
                var viewer = document.getElementById("<%=rptvwrJobDeployment.ClientID %>");
                var htmlheight = document.documentElement.clientHeight;
                if (viewer != null)
                    viewer.style.height = (htmlheight - 20) + "px";
            }
            </script>
    </telerik:RadCodeBlock>
    <asp:HiddenField ID="hdnDB" runat="server" />

        <style type="text/css">
        .reportheight {
            margin-bottom: 30px;
           
        }
    </style>
</asp:Content>
