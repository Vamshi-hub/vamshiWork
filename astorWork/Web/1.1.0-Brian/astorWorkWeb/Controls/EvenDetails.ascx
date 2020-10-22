<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EvenDetails.ascx.cs" Inherits="astorWork.aspx.Configuration.EvenDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<div class="layout">
    <fieldset style="border: 1px solid white; padding: 5px; width: 98%;" runat="server" id="fsZoneStatus">
        <legend class="legend">
            <asp:Label ID="lbZoneName" runat="server" ForeColor="White" Font-Size="Large" Text="Zone Device Status"></asp:Label></legend>
        <table width="100%" id="tblGangwayDeviceStatus" runat="server">
            <tr>
                <td>
                    <telerik:RadGrid ID="rgEmployee" Width="450px" runat="server"
                        OnItemDataBound="rgEmployee_ItemDataBound" OnNeedDataSource="rgEmployee_NeedDataSource" PagerStyle-AlwaysVisible="false" PagerStyle-Visible="false"
                        EnableLinqExpressions="false">
                        <MasterTableView Width="100%" AllowSorting="false" DataKeyNames="Status" AllowFilteringByColumn="false" AllowPaging="false">
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="Image" AllowFiltering="false">
                                    <ItemTemplate>
                                        <telerik:RadBinaryImage runat="server" ID="rbImgEmployee" DataValue='<%#Eval("ValveImage") %>' ImageStorageLocation="Session" Width="30px" Height="30px" ResizeMode="Fill" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn UniqueName="ValveDescription" DataField="ValveDescription" AllowFiltering="false" HeaderText="Description" HeaderStyle-Width="40%" />
                                <telerik:GridBoundColumn UniqueName="ValveType" DataField="ValveType" AllowFiltering="false" HeaderText="Valve Type" HeaderStyle-Width="20%" />
                                <telerik:GridBoundColumn UniqueName="Capturedate" DataField="Capturedate" AllowFiltering="false" HeaderText="Capture Date" HeaderStyle-Width="30%" />

                            </Columns>

                        </MasterTableView>
                        <ClientSettings>
                            <Scrolling AllowScroll="True" UseStaticHeaders="True" ScrollHeight="180px" SaveScrollPosition="true"></Scrolling>
                        </ClientSettings>
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>
    </fieldset>
</div>
