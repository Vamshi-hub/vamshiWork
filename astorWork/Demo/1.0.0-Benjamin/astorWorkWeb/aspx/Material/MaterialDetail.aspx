<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/astorWork.Master" CodeBehind="MaterialDetail.aspx.cs" Inherits="astorWork.MaterialDetail" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">

        var column = null;
        function MenuShowing(sender, args) {
            if (column == null) return;
            var menu = sender; var items = menu.get_items();
            if (column.get_dataType() == "System.String") {
                var i = 0;
                while (i < items.get_count()) {
                    if (!(items.getItem(i).get_value() in { 'NoFilter': '', 'Contains': '', 'EqualTo': '', 'StartsWith': '' })) {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(false);
                    }
                    else {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(true);
                    } i++;
                }
            }
            column = null;
        }

        function filterMenuShowing(sender, eventArgs) {
            column = eventArgs.get_column();
        }

        function CancelKeyPress(sender, eventArgs) {
            eventArgs.get_domEvent().keyCode = null;
            eventArgs.get_domEvent().stopPropagation();
            eventArgs.get_domEvent().preventDefault();
            eventArgs.get_domEvent().cancelBubble = true;
        }
        var popUp;
        function PopUpShowing(sender, eventArgs) {
            popUp = eventArgs.get_popUp();
            popUp.style.left = "20%";
            popUp.style.top = "20%";
        }
    </script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            
            function refreshGrid(Operation) {
                var hdnOperation = document.getElementById("<%= hdnOperation.ClientID %>");
                hdnOperation.value = Operation;
                $get("<%= rbtnRefreshGrid.ClientID %>").click();
            }

            function RequestStart() {
                var loadingPanel = document.getElementById("<%= loadingPanel.ClientID %>");
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
    <telerik:RadAjaxManager ID="ramViewJob" runat="server" DefaultLoadingPanelID="loadingPanel">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="uxMaterialDetail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="viewPanel" LoadingPanelID="loadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="uxPunchlist">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="windowPanel" LoadingPanelID="rlpRadWin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <%-- <telerik:AjaxSetting AjaxControlID="uxPunchlist">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="allPanel"  />
                </UpdatedControls>
            </telerik:AjaxSetting>--%>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="loadingPanel" runat="server" Style="z-index: 100000"></telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxLoadingPanel ID="rlpRadWin" runat="server" Style="height: 480px; width: 890px; z-index: 100000"></telerik:RadAjaxLoadingPanel>
    <telerik:RadFormDecorator ID="FormDecorator1" runat="server" DecoratedControls="All" DecorationZoneID="decorationZone" SkinID="RadFormDecoratorSkin"></telerik:RadFormDecorator>
    <asp:Panel ID="allPanel" runat="server">
        <asp:Panel ID="viewPanel" runat="server">
            <asp:HiddenField runat="server" ID="hdnOperation" />
            <telerik:RadButton runat="server" ID="rbtnRefreshGrid" OnClick="rbtnRefreshGrid_Click" Style="display: none;" />
            <div class="layout" id="decorationZone">
                <fieldset class="fieldsettemplate">
                    <legend style="font-size: medium; font-weight: bold">INFORMATION</legend>
                    <table width="100%" style="margin-left: 20px;" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <telerik:RadLabel ID="RadLabel1" runat="server" Text="Marking No" Font-Size="Small" />
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel7" runat="server" Font-Size="Small" Text="Component"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel4" runat="server" Font-Size="Small" Text="Drawing No"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel2" runat="server" Font-Size="Small" Text="Drawing Issue Date"></telerik:RadLabel>
                            </td>
                            
                            <td></td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <telerik:RadLabel ID="uxMarkingNo" runat="server" Font-Size="X-Large" SkinID="RadLabelSkinReadOnly" ForeColor="#3498DB"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="uxMaterialType" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="uxDrawingNo" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="uxDrawingIssueDate" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="4">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadLabel ID="RadLabel3" runat="server" Font-Size="Small" Text="Project"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel10" runat="server" Font-Size="Small" Text="Block" />
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel5" runat="server" Font-Size="Small" Text="Level"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel12" runat="server" Font-Size="Small" Text="Zone" />
                            </td>
                            
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadLabel ID="uxProject" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="uxBlock" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="uxLevel" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td valign="top">
                                <telerik:RadLabel ID="uxZone" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            
                            <td>
                                <telerik:RadLabel ID="RadLabel8" runat="server" Font-Size="Small" Text="Component Size"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel6" runat="server" Font-Size="Small" Text="Vendor"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="RadLabel11" runat="server" Font-Size="Small" Text="MRF No"></telerik:RadLabel>
                            </td>
                        </tr>
                        <tr>
                            
                            <td>
                                <telerik:RadLabel ID="uxMaterialSize" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                            <td>
                                <telerik:RadLabel ID="uxContractor" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                             <td>
                                <telerik:RadLabel ID="uxMRFNo" runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <br />
                            </td>
                        </tr>
                        
                    </table>
                </fieldset>
            </div>
            <div class="layout" style="width: 99%; float: left">
                <fieldset class="fieldsettemplate">
                    <legend style="font-size: medium; font-weight: bold">PROGRESS</legend>
                    <telerik:RadGrid ID="uxMaterialDetail" runat="server" SkinID="RadGridSkin" PagerStyle-Visible="false" OnSelectedIndexChanged="uxMaterialDetail_SelectedIndexChanged" OnItemDataBound="uxMaterialDetail_ItemDataBound"
                        OnNeedDataSource="uxMaterialDetail_NeedDataSource" OnItemCommand="uxMaterialDetail_ItemCommand">
                        <MasterTableView Width="100%" CommandItemDisplay="Top" AllowFilteringByColumn="false" ClientDataKeyNames="Stage, QCStatus" DataKeyNames="QCStatus" >
                            <SortExpressions>
                                <telerik:GridSortExpression FieldName="SeqNo" SortOrder="Ascending" />
                            </SortExpressions>
                            <CommandItemSettings ShowRefreshButton="true" ShowAddNewRecordButton="false" />
                            <Columns>
                                <telerik:GridBoundColumn DataField="Stage" HeaderText="Stage" ItemStyle-Width="15%" ItemStyle-Height="50px" ItemStyle-Font-Size="X-Large" ItemStyle-Font-Bold="true" />
                                <telerik:GridBoundColumn DataField="CreatedBy" HeaderText="Officer-In-Charge" ItemStyle-Width="20%" ColumnGroupName="GroupA" ItemStyle-Height="50px" ItemStyle-Font-Size="Medium" />
                                <telerik:GridBoundColumn DataField="CreatedDate" HeaderText="Date" ItemStyle-Width="20%" ColumnGroupName="GroupA" ItemStyle-Height="50px" ItemStyle-Font-Size="Medium" />
                                <telerik:GridBoundColumn DataField="Location" HeaderText="Location" ItemStyle-Width="20%" ItemStyle-Height="50px" ItemStyle-Font-Size="Medium" />
                                <telerik:GridImageColumn UniqueName="QCStatus" DataType="System.String" DataImageUrlFields="QCStatus" DataImageUrlFormatString="~/Images/{0}.png" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" HeaderText="QC" ImageAlign="Middle" ImageHeight="40px"></telerik:GridImageColumn>
                                <telerik:GridImageColumn DataType="System.String" DataImageUrlFields="Status" DataImageUrlFormatString="~/Images/Circle/{0}.png" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" HeaderText="Status" ImageAlign="Middle" ImageHeight="40px"></telerik:GridImageColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings>
                            <Selecting AllowRowSelect="true" />
                        </ClientSettings>
                    </telerik:RadGrid>
                </fieldset>
            </div>
            
        </asp:Panel>
    </asp:Panel>
    <asp:HiddenField ID="uxDisplayName" runat="server" />
    <asp:HiddenField ID="uxProgress" runat="server" />
    <asp:HiddenField ID="uxText" runat="server" />
    <asp:HiddenField ID="uxLocationID" runat="server" />
    <asp:HiddenField ID="uxURL" runat="server" />
</asp:Content>
