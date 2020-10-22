<%@ Page Title="Add User Master" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="astorWork.aspx.Configuration.AddUser" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock runat="server">
        <script type="text/javascript">

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseAndReload(Operation) {
                var oWnd = GetRadWindow();
                oWnd.BrowserWindow.refreshGrid(Operation);
                oWnd.close();
            }

            function CloseWindow() {
                var oWnd = GetRadWindow();
                oWnd.close();
            }



            function RequestStart() {
                var loadingPanel = document.getElementById("<%= ralpAddUser.ClientID %>");
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
                loadingPanel.style.backgroundPosition = (parseInt(scrollLeftOffset) + parseInt(viewportWidth / 2) - parseInt(loadingImageWidth / 2)) + "px " + (parseInt(scrollTopOffset) + parseInt(viewportHeight / 2) - parseInt(loadingImageHeight / 2)) + "px";
            }

        </script>
    </telerik:RadCodeBlock>

    <telerik:RadAjaxManager ID="ramAddUser" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="pnlAddUser">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAddUser" LoadingPanelID="ralpAddUser" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralpAddUser" runat="server"></telerik:RadAjaxLoadingPanel>
    <asp:Panel ID="pnlAddUser" runat="server">
        <table class="formTable">
            <tbody>
                <tr>
                    <td style="width: 15%" class="tdLabel">
                        <asp:Label ID="lblUserID" runat="server" Text="User ID"></asp:Label>
                    </td>
                    <td style="width: 35%" class="tdField" colspan="3">

                        <asp:TextBox ID="txtUserId" runat="server" Width="182px" MaxLength="20" SkinID="tbMandatory" autocomplete="off"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvUserId" ForeColor="Red" Display="Dynamic" ValidationGroup="user" runat="server" ErrorMessage="*" ControlToValidate="txtUserId"></asp:RequiredFieldValidator>
                    </td>
                    <cc1:FilteredTextBoxExtender ID="ftbUserID" runat="server" ValidChars="abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ."
                        TargetControlID="txtUserId" FilterMode="ValidChars">
                    </cc1:FilteredTextBoxExtender>
                </tr>
                <tr>
                    <td style="width: 15%" class="tdLabel">
                        <asp:Label ID="lblUserName" runat="server" Text="User Name"></asp:Label>
                    </td>
                    <td style="width: 35%" class="tdField">
                        <asp:TextBox ID="txtUserName" runat="server" MaxLength="100" Width="182px" SkinID="tbMandatory" autocomplete="off"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ForeColor="Red" Display="Dynamic" ValidationGroup="user"
                            ErrorMessage="*" ControlToValidate="txtUserName"></asp:RequiredFieldValidator>
                    </td>


                </tr>
                <tr>
                    <td style="width: 15%" class="tdLabel">
                        <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td class="tdField" style="width: 35%">
                        <asp:Label ID="lblPwd" runat="server" Visible="false" />
                        <asp:TextBox ID="txtPwd" runat="server" TextMode="Password" Width="182px" SkinID="tbMandatory" MaxLength="100"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPwd" runat="server" ControlToValidate="txtPwd" ForeColor="Red" Display="Dynamic" ValidationGroup="user"
                            ErrorMessage="*"></asp:RequiredFieldValidator>

                    </td>
                </tr>
                <tr>
                    <td style="width: 15%" class="tdLabel">
                        <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password"></asp:Label>
                    </td>

                    <td class="tdField" style="width: 35%">
                        <asp:TextBox ID="txtConfirmPwd" runat="server" TextMode="Password" MaxLength="100" Width="182px" SkinID="tbMandatory" autocomplete="off"></asp:TextBox>

                        <asp:RequiredFieldValidator ID="rfvCPwd" runat="server" ControlToValidate="txtConfirmPwd" ForeColor="Red" Display="Dynamic" ValidationGroup="user"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvCPwd" runat="server" ErrorMessage="Password Must Match" ForeColor="Red" Display="Dynamic" ValidationGroup="user"
                            Operator="Equal" ControlToValidate="txtConfirmPwd" ControlToCompare="txtPwd" />
                    </td>
                </tr>


                <tr>
                    <td class="tdControl" colspan="4">
                        <telerik:RadButton ID="closeMe" SkinID="rbnCancel" runat="server" OnClientClicking="CloseWindow" Text="Cancel" AutoPostBack="false" />
                        <telerik:RadButton ID="rbtnSubmit" ValidationGroup="user" SkinID="rbnSave" OnClick="rbtnSubmit_Click" runat="server" Text="Submit" />

                    </td>
                </tr>

            </tbody>
        </table>
        <asp:Label ID="lblResult" runat="server"></asp:Label>
    </asp:Panel>
</asp:Content>
