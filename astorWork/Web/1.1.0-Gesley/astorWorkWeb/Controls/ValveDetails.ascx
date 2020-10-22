<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ValveDetails.ascx.cs" Inherits="astorWork.Controls.ValveDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style>
    .tdcLabel {
        vertical-align: middle;
        font-size: 11px;
    }

    .tdcField {
        vertical-align: middle;
        font-size: 16px;
        align: left;
    }

    .tdEmpty {
        height: 4px !important;
    }
</style>

<div class="layout" style="padding: 3px">
    <table style="width: 100%; color: black">
        <tr>
            <td class="tdcField">
                <telerik:RadBinaryImage runat="server" ID="rbImgValve"
                    Width="100px" ImageStorageLocation="Session" Height="100px" ResizeMode="Fill" />
            </td>
            <td>
                <table>
                    <tr>
                        <td class="tdcLabel">Name</td>

                    </tr>
                    <tr>
                        <td class="tdcField">
                            <asp:Label ID="lblVName" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdEmpty"></td>
                    </tr>
                    <tr>
                        <td style="font-size: 11px;">Description</td>
                    </tr>
                    <tr>
                        <td style="font-size: 16px;" align="left">
                            <asp:Label ID="lblDesc" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                </table>


            </td>
        </tr>
        <tr>
            <td colspan="2" class="tdEmpty"></td>
        </tr>
        <tr>
            <td style="font-size: 11px;">Type
            </td>
            <td style="font-size: 11px;">Status
            </td>
        </tr>
        <tr>

            <td style="font-size: 16px;" align="left">
                <asp:Label ID="lblType" runat="server" Text="Label"></asp:Label>
            </td>
            <td style="font-size: 16px;" align="left">
                <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label>
            </td>




        </tr>
        <tr>
            <td colspan="2">
                <telerik:RadGrid ID="rgTransactions" runat="server" AllowSorting="true" AllowFilteringByColumn="false"
                    Width="250px" OnNeedDataSource="rgTransactions_NeedDataSource">

                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" ScrollHeight="150px" SaveScrollPosition="true"></Scrolling>
                    </ClientSettings>

                    <MasterTableView AllowFilteringByColumn="false" AllowPaging="false">

                        <Columns>
                            <telerik:GridBoundColumn HeaderStyle-Width="70%" DataField="Timestamp" AllowSorting="true" HeaderText="Captured Date"></telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderStyle-Width="30%" AllowSorting="true" HeaderText="Status">
                                <ItemTemplate>
                                    <%#(astorWork.Enums.ValveStatus)Enum.Parse(typeof(astorWork.Enums.ValveStatus), Eval("Status").ToString()) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </td>

        </tr>
    </table>

</div>

