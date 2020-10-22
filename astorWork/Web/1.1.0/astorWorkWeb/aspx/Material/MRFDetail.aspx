<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/astorWork.Master" ValidateRequest="false" CodeBehind="MRFDetail.aspx.cs" Inherits="astorWork.MRFDetail" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            function refreshGrid(Operation) {
                document.getElementById("<%= hdnOperation.ClientID %>").value = Operation;
                <%--$get("<%= rbtnRefreshGrid.ClientID %>").click();--%>
            }

            function RequestStart() {
                var loadingPanel = document.getElementById("<%= ralpMRFDetail.ClientID %>");
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
    <telerik:RadAjaxManager ID="ramMRFDetail" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rapMRFDetail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rapMRFDetail" LoadingPanelID="ralpMRFDetail" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel runat="server" ID="ralpMRFDetail"></telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxPanel runat="server" ID="rapMRFDetail">
        <asp:Panel ID="pnlMRFDetail" runat="server">
            <asp:HiddenField runat="server" ID="hdnOperation" />
            <div class="layout" id="decorationZone">
                <fieldset class="fieldsettemplate">
                    <legend style="font-size: medium; font-weight: bold">MASTER CABLE SHEET</legend>
                    <table width="100%" style="margin-left: 20px;" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <telerik:RadLabel ID="RadLabel1" runat="server" Text="MRF No" Font-Size="Small" />
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel4" runat="server" Font-Size="Small" Text="MRF Date"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel2" runat="server" Font-Size="Small" Text="Status"></telerik:RadLabel>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <telerik:RadLabel ID="ulMRFNo" runat="server" Font-Size="X-Large" SkinID="RadLabelSkinReadOnly" ForeColor="#3498DB"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="ulMRFDate" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                        <telerik:RadLabel ID="ulStatus" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadLabel ID="RadLabel7" runat="server" Font-Size="Small" Text="Project"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel8" runat="server" Font-Size="Small" Text="Contractor"></telerik:RadLabel>
                            </td>
                          
                            <td>
                                <telerik:RadLabel ID="RadLabel6" runat="server" Font-Size="Small" Text="Officer In-Charge"></telerik:RadLabel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadLabel ID="ulProject" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="ulVendor" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="ulOfficerInCharge" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                           
                        </tr>
                        <tr>
                            <td colspan="3">
                                <br />
                            </td>
                        </tr>
        
                    </table>
                </fieldset>
            </div>

            <div class="layout" style="width: 99%; float: left">
                <fieldset class="fieldsettemplate">
                    <legend style="font-size: medium; font-weight: bold">MRF DETAILS</legend>
                    <telerik:RadGrid ID="uxMRFDetail" runat="server" SkinID="RadGridSkin" PagerStyle-Visible="false"
                        OnNeedDataSource="uxMRFDetail_NeedDataSource">
                        <MasterTableView Width="100%" CommandItemDisplay="Top" AllowFilteringByColumn="false">
                            <CommandItemSettings ShowRefreshButton="true" ShowAddNewRecordButton="false" />
                            <ColumnGroups>
                                <telerik:GridColumnGroup Name="Length" HeaderText="Length (m)" HeaderStyle-HorizontalAlign="Center"></telerik:GridColumnGroup>
                                <telerik:GridColumnGroup Name="Specification" HeaderText="Specification" HeaderStyle-HorizontalAlign="Center"></telerik:GridColumnGroup>
                            </ColumnGroups>
                            <Columns>
                                <telerik:GridBoundColumn DataField="MarkingNo" HeaderText="Cable Tag No <br/> Junction Box" />
                                <telerik:GridBoundColumn DataField="DrawingNo" HeaderText="Drawing No" />
                                <telerik:GridBoundColumn DataField="MaterialType" HeaderText="Type" ColumnGroupName="Specification" />
                                <telerik:GridBoundColumn DataField="MaterialSize" HeaderText="Size" ColumnGroupName="Specification" />
                                <telerik:GridBoundColumn DataField="CableOD" HeaderText="OD" ColumnGroupName="Specification" />
                                <telerik:GridBoundColumn DataField="EstimatedLength" HeaderText="Estimated" ColumnGroupName="Length" />
                                <telerik:GridBoundColumn DataField="ActualLength" HeaderText="Actual" ColumnGroupName="Length" />
                                <telerik:GridBoundColumn DataField="LayingBufferTime" HeaderText="Cable Lay Buffer(days)" />
                                <telerik:GridBoundColumn DataField="Status" HeaderText="Current Stage" />
                                <telerik:GridImageColumn DataType="System.String" DataImageUrlFields="Enrol" DataImageUrlFormatString="~/Images/Circle/{0}.png" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" HeaderText="Enrol" ImageAlign="Middle" ImageHeight="40px" HeaderStyle-HorizontalAlign="Center"></telerik:GridImageColumn>
                                <telerik:GridImageColumn DataType="System.String" DataImageUrlFields="Issue" DataImageUrlFormatString="~/Images/Circle/{0}.png" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" HeaderText="Issue" ImageAlign="Middle" ImageHeight="40px" HeaderStyle-HorizontalAlign="Center"></telerik:GridImageColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings>
                            <Selecting AllowRowSelect="true" />
                            <ClientEvents OnRowSelecting="RowSelecting" />
                        </ClientSettings>
                    </telerik:RadGrid>
                </fieldset>
            </div>
        </asp:Panel>
    </telerik:RadAjaxPanel>















</asp:Content>
