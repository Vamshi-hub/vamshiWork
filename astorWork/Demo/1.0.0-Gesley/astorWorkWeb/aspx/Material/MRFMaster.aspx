<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/astorWork.Master" CodeBehind="MRFMaster.aspx.cs" Inherits="astorWork.MRFMaster" %>

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
        //var popUp;
        //function PopUpShowing(sender, eventArgs) {
        //    popUp = eventArgs.get_popUp();
        //    popUp.style.left = "20%";
        //    popUp.style.top = "20%";
        //}
    </script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            <%--function openRadWindow(sender, args) {
                if (sender.get_commandArgument() == '')
                    $find("<%= RadWin_AddJobPage.ClientID %>").setUrl("AddMaterialMaster.aspx");
                else
                    $find("<%= RadWin_AddJobPage.ClientID %>").setUrl("AddMaterialMaster.aspx?MaterialID=" + sender.get_commandArgument());
                $find("<%= RadWin_AddJobPage.ClientID %>").show();
            }--%>

            function refreshGrid(Operation) {
                var hdnOperation = document.getElementById("<%= hdnOperation.ClientID %>");
                hdnOperation.value = Operation;

            }

            function RequestStart() {
                var loadingPanel = document.getElementById("<%= ralpViewJob.ClientID %>");
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

            function RowSelecting1(sender, args) {
                var url = args.get_commandName();
                var DisplayName = args.get_commandArgument();
                parent.AddNewTabs(DisplayName, url);
            }
            function NewMRFCreation() {
                var uxDisplayNameEdit = document.getElementById('<%= uxDisplayNameEdit.ClientID%>');
                var uxURLEdit = document.getElementById('<%= uxURLEdit.ClientID%>');
                var DisplayName = uxDisplayNameEdit.value + ": " + "New MRF Creation";
                var URL = uxURLEdit.value + "?MRFNo=" + "";
                parent.AddNewTabs(DisplayName, URL);
            }
            function RowSelecting(sender, eventArgs) {
                var uxStatus = eventArgs.getDataKeyValue("Status");
                if (uxStatus == "Pending") {
                    var uxDisplayNameEdit = document.getElementById('<%= uxDisplayNameEdit.ClientID%>');
                    var uxURLEdit = document.getElementById('<%= uxURLEdit.ClientID%>');
                    var DisplayName = uxDisplayNameEdit.value + ": " + eventArgs.getDataKeyValue("MRFNo");
                    var URL = uxURLEdit.value + "?MRFNo=" + eventArgs.getDataKeyValue("MRFNo");
                    parent.AddNewTabs(DisplayName, URL);
                }
                else {
                    var uxDisplayName = document.getElementById('<%= uxDisplayName.ClientID%>');
                    var uxURL = document.getElementById('<%= uxURL.ClientID%>');
                    var DisplayName = uxDisplayName.value + ": " + eventArgs.getDataKeyValue("MRFNo");
                    var URL = uxURL.value + "?MRFNo=" + eventArgs.getDataKeyValue("MRFNo");
                    parent.AddNewTabs(DisplayName, URL);
                }
            }

        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManager ID="ramViewJob" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="pnlViewJob">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlViewJob" LoadingPanelID="ralpViewJob" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralpViewJob" runat="server"></telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxPanel runat="server" ID="rapMRFM">
        <asp:Panel ID="pnlViewJob" runat="server">
            <asp:HiddenField runat="server" ID="hdnOperation" />

            <%--  <telerik:RadWindow runat="server" ID="RadWin_AddJobPage" Width="610" Height="290" Behaviors="Close,Reload">
        </telerik:RadWindow>--%>
            <div class="layout" >
                <telerik:RadGrid ID="uxMRFMaster" runat="server" SkinID="RadGridSkin"
                    OnNeedDataSource="uxMRFMaster_NeedDataSource" OnItemDataBound="uxMRFMaster_ItemDataBound" OnItemCommand="uxMRFMaster_ItemCommand">
                    <MasterTableView Width="100%" ClientDataKeyNames="MRFNo,Status" DataKeyNames="MRFNo,Status" CommandItemDisplay="Top" EditMode="EditForms">
                        <%-- <CommandItemSettings ShowRefreshButton="true" z="true" AddNewRecordText="Create New MRF" />--%>
                        <ColumnGroups>
                        <telerik:GridColumnGroup Name="Location"  HeaderText="Location" HeaderStyle-HorizontalAlign="Center"></telerik:GridColumnGroup>
                    </ColumnGroups>
                        <CommandItemTemplate>
                            <div style="padding: 5px 5px;">
                                <asp:Button ID="uxCreateNewMRF" runat="server" class="rgAdd" OnClientClick="return NewMRFCreation()" CommandName="CreateNewMRF" Text="Create New Request" ToolTip="Create New Request"></asp:Button>
                                <asp:LinkButton ID="uxInsertNewMRF" runat="server" OnClientClick="return NewMRFCreation()" CommandName="CreateNewMRF" Text="Add">Create New Request</asp:LinkButton>
                                <div style="float: right;">
                                    <asp:Button ID="uxRefresh" runat="server" OnClick="rbtnRefreshGrid_Click" class="rgRefresh" Text="Refresh" ToolTip="Refresh"></asp:Button>
                                    <asp:LinkButton ID="uxRefreshgrd" runat="server" OnClick="rbtnRefreshGrid_Click" Text="Refresh">Refresh</asp:LinkButton>
                                </div>
                            </div>
                        </CommandItemTemplate>
                        <Columns>
                            <telerik:GridBoundColumn DataField="MRFNo" HeaderText="PP09" />
                            <telerik:GridBoundColumn DataField="Project" HeaderText="Project"  AllowFiltering="false"  />
                            <telerik:GridBoundColumn DataField="Vendor" HeaderText="Vendor"  AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="MaterialType" HeaderText="Component"  AllowFiltering="false"  />
                            <telerik:GridBoundColumn DataField="Block" HeaderText="Block"  AllowFiltering="false" ColumnGroupName="Location" />
                            <telerik:GridBoundColumn DataField="Level" HeaderText="Level" AllowFiltering="false" ColumnGroupName="Location"/>
                            <telerik:GridBoundColumn DataField="Zone" HeaderText="Zone" AllowFiltering="false" ColumnGroupName="Location"/>
                            <telerik:GridBoundColumn DataField="PlannedCastingDate" HeaderText="Planned Casting Date" AllowFiltering="false" />
                            <telerik:GridTemplateColumn ReadOnly="true" ItemStyle-HorizontalAlign="Center" HeaderText="Progress" AllowFiltering="false" HeaderStyle-Width="10%">
                            <ItemTemplate>
                                <telerik:RadProgressBar ID="uxProgress" Label='<%# decimal.Ceiling(decimal.Parse(Eval("Progress").ToString())).ToString() + "%"  %>' runat="server"  Width="90%" MaxValue="100" value='<%# decimal.Parse(Eval("Progress").ToString()) %>' SkinID="RadProgressBarSkin"> 
                            </telerik:RadProgressBar>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="true" />
                        <ClientEvents OnRowSelecting="RowSelecting" />
                    </ClientSettings>
                </telerik:RadGrid>
                <asp:HiddenField ID="uxDisplayName" runat="server" />
                <asp:HiddenField ID="uxURL" runat="server" />
                <asp:HiddenField ID="uxDisplayNameEdit" runat="server" />
                <asp:HiddenField ID="uxURLEdit" runat="server" />
            </div>
        </asp:Panel>
    </telerik:RadAjaxPanel>
</asp:Content>
