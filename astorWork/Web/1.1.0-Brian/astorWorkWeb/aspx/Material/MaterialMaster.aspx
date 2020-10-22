<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/astorWork.Master" CodeBehind="MaterialMaster.aspx.cs" Inherits="astorWork.MaterialMaster" %>

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
                $get("<%= rbtnRefreshGrid.ClientID %>").click();
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

            function RowSelecting1(sender, args)
            {
                var url = args.get_commandName();
                var DisplayName = args.get_commandArgument();
                parent.AddNewTabs(DisplayName,url);
            }

            function RowSelecting(sender, eventArgs) {                
                var uxDisplayName = document.getElementById('<%= uxDisplayName.ClientID%>');
                var uxURL = document.getElementById('<%= uxURL.ClientID%>');
                var DisplayName = uxDisplayName.value + ": " + eventArgs.getDataKeyValue("MaterialNo");
                var URL = uxURL.value + "?MaterialNo=" + eventArgs.getDataKeyValue("MaterialNo");
                parent.AddNewTabs(DisplayName, URL);
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
    <asp:Panel ID="pnlViewJob" runat="server">
        <asp:HiddenField runat="server" ID="hdnOperation" />
        <telerik:RadButton runat="server" ID="rbtnRefreshGrid" OnClick="rbtnRefreshGrid_Click" Style="display: none;" />
      <%--  <telerik:RadWindow runat="server" ID="RadWin_AddJobPage" Width="610" Height="290" Behaviors="Close,Reload">
        </telerik:RadWindow>--%>
        <div class="layout">
            <telerik:RadGrid ID="uxMaterialMaster" runat="server" SkinID="RadGridSkin"
                OnNeedDataSource="uxMaterialMaster_NeedDataSource" OnItemDataBound="uxMaterialMaster_ItemDataBound" OnItemCommand="uxMaterialMaster_ItemCommand">
                    <MasterTableView Width="100%" ClientDataKeyNames="MaterialNo" DataKeyNames="MaterialNo" CommandItemDisplay="Top">
                    <CommandItemSettings ShowRefreshButton="true" ShowAddNewRecordButton="false" />
                    <ColumnGroups>
                        <telerik:GridColumnGroup Name="Drawing"  HeaderText="Drawing" HeaderStyle-HorizontalAlign="Center"></telerik:GridColumnGroup>
                        <telerik:GridColumnGroup Name="Location"  HeaderText="Location" HeaderStyle-HorizontalAlign="Center"></telerik:GridColumnGroup>
                    </ColumnGroups>
                    <Columns>
                        <telerik:GridBoundColumn DataField="MarkingNo" HeaderText="Marking No"  />    
                        <telerik:GridBoundColumn DataField="MaterialType" HeaderText="Component"  />    
                        <telerik:GridBoundColumn DataField="DrawingNo" HeaderText="Ref No" ColumnGroupName="Drawing"/>                        
                        <telerik:GridBoundColumn DataField="DrawingIssueDate" HeaderText="Issue Date" ColumnGroupName="Drawing" DataFormatString="{0:d}"/>                       
                        <telerik:GridBoundColumn DataField="Project" HeaderText="Project" />     
                        <telerik:GridBoundColumn DataField="Block" HeaderText="Block" ColumnGroupName="Location" ItemStyle-Width="10%"/>
                        <telerik:GridBoundColumn DataField="Level" HeaderText="Level" ColumnGroupName="Location" ItemStyle-Width="10%"/>
                        <telerik:GridBoundColumn DataField="Zone" HeaderText="Zone" ColumnGroupName="Location" ItemStyle-Width="10%"/>   
                        <telerik:GridTemplateColumn ReadOnly="true" ItemStyle-HorizontalAlign="Center" HeaderText="Progress" AllowFiltering="false" HeaderStyle-Width="12%">
                            <ItemTemplate>
                                <telerik:RadProgressBar ID="uxJobProgress" Label='<%# Eval("ProgressDescription").ToString() %>' runat="server"  Width="95%" MaxValue="100" value='<%# decimal.Parse(Eval("Progress").ToString()) %>' SkinID="RadProgressBarSkin"> 
                        </telerik:RadProgressBar>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>                                      
                    </Columns>                    
                </MasterTableView>
                <ClientSettings>     
                    <Selecting AllowRowSelect="true" />
                    <%--<ClientEvents OnRowSelecting="RowSelecting" />--%>
                </ClientSettings>
        </telerik:RadGrid>
            <asp:HiddenField ID="uxDisplayName" runat="server" />
            <asp:HiddenField ID="uxURL" runat="server" />
        </div>
    </asp:Panel>
</asp:Content>
