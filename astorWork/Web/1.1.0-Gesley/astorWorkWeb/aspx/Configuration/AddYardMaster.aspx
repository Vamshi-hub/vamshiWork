<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="AddYardMaster.aspx.cs" Inherits="astorWork.aspx.Configuration.AddYardMaster" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ MasterType VirtualPath="~/astorWork.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">
        .ruErrorMessage {
            display: block;
            color: #ef0000;
            font-variant: small-caps;
            text-transform: lowercase;
        }

        .rbLinkButton .rbPrimary {
            padding-left: 11px !important;
            line-height: 20px !important;
        }

        div.RadToolTip .rtWrapper td.rtWrapperContent {
            padding: 0;
        }

        div.RadToolTip_Default table.rtWrapper td.rtWrapperContent {
            background-color: transparent !important;
        }
    </style>
    <telerik:RadCodeBlock runat="server">

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


            function OnClientBeforeShow() {
                document.documentElement.focus();
            }


            <%--function removeFile(radAsyncUpload, args) {
                var rbtnSave = document.getElementById("<%=rbtnSave.ClientID %>");
                rbtnSave.disabled = false;
                var rbtnPreview = $find("<%= rbtnPreview.ClientID %>");
                rbtnPreview._element.style.display = "None";
                $find("<%= rttPreview.ClientID%>").hide();
            }--%>
            var $ = $telerik.$;
            function validateUpload(sender, args) {
                var upload = $find("<%= rAsyncUYardLayout.ClientID %>");
                args.IsValid = upload.getUploadedFiles().length != 0;
                var cvZoneImage = document.getElementById("<%= cvYardLayout.ClientID  %>");
               cvZoneImage.innerText = "*";
           }
           function Confirmation(sender, args) {
               var res = confirm('Are you sure want to delete ?');
               if (!res) {
                   //Cancel the postback
                   args.set_cancel(true);
               }
           }
           function OnClientBeforeShow() {
               document.documentElement.focus();
           }
           function validateString(oSrc, arguments) {
               //            args.IsValid = (args.Value.length >= 8);
               if (arguments.Value.toLowerCase() == "unknown" || arguments.Value == "UNKNOWN") {
                   arguments.IsValid = false;
               }
               else {
                   arguments.IsValid = true;
               }
           }
       <%--     function CloseGangwayWindow() {
                var PopupWindow = $find("<%= RadWin_AddEditGangway.ClientID %>");
            PopupWindow.close();
        }--%>
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
 <%--       function Dropdownclose(sender, eventArgs) {
            var ddlCL = $find("<%= rddlConnectingYard.ClientID %>");
             var ddlDevice = $find("<%= rddlDevice.ClientID %>");
             ddlCL.closeDropDown();
             ddlDevice.closeDropDown();
         }--%>
            function RequestStart() {
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
            function ValidateComboforTimeZone(source, args) {
                var combo = $find(source.controltovalidate);
                var emptyMessage = combo.get_emptyMessage();
                var comboText = combo.get_text();

                var item = combo.findItemByText(comboText);
                // var itm = combo.findItemByValue("0");
                if (item != null || comboText === emptyMessage) {

                    args.IsValid = true;
                }
                else {
                    //itm.select();
                    args.IsValid = false;
                }
            }
            function validationFailed(radAsyncUpload, args) {

                var $row = $(args.get_row());
                var rbtnSave = document.getElementById("<%=rbtnSave.ClientID %>");
             rbtnSave.disabled = 'disabled';
             var erorMessage = getErrorMessage(radAsyncUpload, args);
             var span = createError(erorMessage);
             $row.addClass("ruError");
             $row.append(span);
         }
         function getErrorMessage(sender, args) {

             var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);

             if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct

                 if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                     return ("This file type is not supported.");
                 }

                 else {
                     return ("This file exceeds the maximum allowed size of 20 KB.");
                 }
             }
             else {
                 return ("not correct extension.");
             }
         }
         function createError(erorMessage) {
             var input = '<span class="ruErrorMessage">' + erorMessage + ' </span>';
             return input;
         }
         function OnClientBlurHandler(sender, eventArgs) {
             var sender = $find('<%= rcbUTCOffset.ClientID %>');
            var item = sender.findItemByText(sender.get_text());

            if (!item) {

                sender.clearSelection();
                sender.set_emptyMessage("-Select TimeZone-");
            }
        }
        //functions for Yard image preview
     <%--   function rAsyncUpload_Selected(sender, args) {
            $find("<%= rbtnSaveSession.ClientID %>").click();
                var cvZoneImage = document.getElementById("<%= cvYardLayout.ClientID  %>");
                cvZoneImage.innerText = "";
            }--%>

        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" />
    <telerik:RadAjaxLoadingPanel ID="rlpRadwin" runat="server" SkinID="RadWin" Style="height: 460px; width: 890px">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgGangway">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbtnSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlSaveUpdateYard" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbtnAddGangway">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlSaveUpdateGangway" LoadingPanelID="rlpRadwin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbtnIsYardTimeCapture">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbtnAddGangway">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbtnPreview">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbtnSaveSession">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnlMain" runat="server">
        <asp:Panel ID="pnlSaveUpdateYard" runat="server">
            <table id="tblAddYard" class="table" width="100%">
                <tr>
                    <td class="tdLabel" style="width: 30%">
                        <asp:Label ID="lblYardName" runat="server" Text="Yard Name"></asp:Label>
                    </td>
                    <td class="tdField">
                        <asp:TextBox ID="tbYardName" runat="server" Width="30%" MaxLength="100" SkinID="tbMandatory"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rvYardName" ValidationGroup="SaveUpdateYard"
                            runat="server" ErrorMessage="*" ControlToValidate="tbYardName"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="tdLabel" style="width: 30%">
                        <asp:Label ID="lblIsDefault" runat="server" Text="Is Default"></asp:Label>
                    </td>
                    <td class="tdField">
                        <asp:RadioButtonList ID="rbtnIsDefault" runat="server" RepeatDirection="Horizontal"
                            Width="100">
                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            <asp:ListItem Selected="True" Value="No">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="tdLabel" style="width: 30%">
                        <asp:Label ID="lblUTCOffset" runat="server" Text="Yard TimeZone"></asp:Label>
                    </td>
                    <td class="tdField">
                        <telerik:RadComboBox ID="rcbUTCOffset" runat="server" DropDownAutoWidth="Enabled" Filter="Contains" Width="50%" EmptyMessage="-Select-" AllowCustomText="true" />
                        <asp:RequiredFieldValidator ID="rvUTCOffset" ValidationGroup="SaveUpdateYard"
                            runat="server" ErrorMessage="*" ControlToValidate="rcbUTCOffset"></asp:RequiredFieldValidator>
                        <asp:CustomValidator runat="server" ID="CustomValidator2" ClientValidationFunction="ValidateComboforTimeZone"
                            ErrorMessage="Invalid Input" ForeColor="Red" ValidationGroup="SaveUpdateYard" ControlToValidate="rcbUTCOffset">*</asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td class="tdLabel" style="width: 30%">
                        <asp:Label ID="Label1" runat="server" Text="Enable ClockTime Capture"></asp:Label>
                    </td>
                    <td class="tdField">
                        <asp:RadioButtonList ID="rbtnIsYardTimeCapture" runat="server" RepeatDirection="Horizontal"
                            Width="100">
                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            <asp:ListItem Selected="True" Value="No">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="tdLabel" style="width: 30%">
                        <asp:Label ID="lblYardLayout" runat="server" Text="YardLayout"></asp:Label>
                    </td>
                    <td class="tdField" style="width: 99%">
                        <table>
                            <tr>
                                <td valign="middle" align="left">

                                             <telerik:RadAsyncUpload ID="rAsyncUYardLayout" runat="server" Width="100%" MaxFileInputsCount="1" MultipleFileSelection="Disabled"
                                        PostbackTriggers="rbtnSave"
                                        OnClientValidationFailed="validationFailed"
                                       
                                        AutoAddFileInputs="true" AllowedFileExtensions="jpg,png,bmp,jpeg,gif">
                                        <Localization Select="Browse" />
                                    </telerik:RadAsyncUpload>
                               <%--     <telerik:RadAsyncUpload ID="rAsyncUYardLayout" runat="server" Width="100%" MaxFileInputsCount="1" MultipleFileSelection="Disabled"
                                        PostbackTriggers="rbtnSave"
                                        AllowedFileExtensions="png" AutoAddFileInputs="true" OnClientValidationFailed="validationFailed" OnClientFileUploadRemoved="removeFile"
                                        OnClientFileUploaded="rAsyncUpload_Selected" HttpHandlerUrl="~/Controls/TelerikImagePreviewHandler.ashx">
                                        <Localization Select="Browse" />
                                    </telerik:RadAsyncUpload>--%>
                                    <asp:Label ID="lblNote" Class="NoteFormat" runat="server" Text="Note : Allowed format is .png."></asp:Label>
                                </td>
                                <td valign="middle" style="padding-left: 10px !important; line-height: 4px">
                    <%--                <telerik:RadButton ID="rbtnPreview" runat="server" Text="Preview" OnClick="rbtnPreview_Click" CausesValidation="false" ButtonType="LinkButton">
                                        <Icon PrimaryIconCssClass="rbSearch" PrimaryIconTop="5" PrimaryIconLeft="5" />
                                    </telerik:RadButton>
                                    <telerik:RadButton ID="rbtnSaveSession" runat="server" OnClick="rbtnSaveSession_Click" Style="display: none"></telerik:RadButton>
                                    <asp:Label ID="lblToolTip" runat="server"></asp:Label>
                                    <telerik:RadToolTip ID="rttPreview" runat="server" ShowEvent="OnClick" ValidationGroup="SaveUpdateYard1"
                                        Animation="Slide" HideEvent="ManualClose" RelativeTo="Element" Position="BottomRight">
                                        <telerik:RadBinaryImage runat="server" CssClass="cursor" ID="imgYardLayout" ResizeMode="Fit" Style="width: 250px !important; height: 250px !important" />
                                    </telerik:RadToolTip>
                                    <asp:CustomValidator runat="server" Display="Static" ID="cvYardLayout" ClientValidationFunction="validateUpload" ValidationGroup="SaveUpdateYard"
                                        ErrorMessage="*">
                                    </asp:CustomValidator>--%>

                                        <asp:CustomValidator runat="server" Display="Dynamic" ID="cvYardLayout" ForeColor="Red"
                                        ClientValidationFunction="validateUpload" ValidationGroup="SaveUpdateYard"
                                        ErrorMessage="*">
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <%--<div>
                <telerik:RadGrid ID="rgGangway" runat="server" OnNeedDataSource="rgGangway_NeedDataSource" OnItemCommand="rgGangway_ItemCommand" OnItemDataBound="rgGangway_ItemDataBound" AllowPaging="false">
                    <MasterTableView Width="100%" DataKeyNames="GangwayID,GangwayName" NoMasterRecordsText="No Gangway(s) to display"
                        AllowPaging="false" AllowFilteringByColumn="false" AllowSorting="false">
                        <Columns>
                            <telerik:GridBoundColumn DataField="GangwayName" HeaderText="Gangway Name" HeaderStyle-Width="20%" />
                            <telerik:GridBoundColumn DataField="GangwayDescription" HeaderText="Description"
                                HeaderStyle-Width="25%" />
                            <telerik:GridBoundColumn DataField="DeviceName" HeaderText="Device Name" HeaderStyle-Width="18%" />
                            <telerik:GridBoundColumn DataField="ConnectingYardName" HeaderText="Connecting Yard"
                                HeaderStyle-Width="20%" />
                            <telerik:GridTemplateColumn AllowFiltering="false" HeaderStyle-Width="18%" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderTemplate>
                                    <telerik:RadButton ID="rbAddGangway" runat="server" Text="Add" CommandName="RowInsert"
                                        AutoPostBack="true" CausesValidation="true" ButtonType="LinkButton" Skin="Outlook">
                                        <Icon PrimaryIconCssClass="rbAdd" PrimaryIconLeft="4" PrimaryIconTop="5" />
                                    </telerik:RadButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <telerik:RadButton ID="rbtnEdit" runat="server" Text="Edit" CommandName="RowEdit"
                                        CausesValidation="true" CommandArgument='<%# Eval("GangwayID") %>' AutoPostBack="true"
                                        ButtonType="LinkButton" Width="42px">
                                    </telerik:RadButton>
                                    <telerik:RadButton ID="rbtnDelete" runat="server" Text="Delete" Width="55px"
                                        CausesValidation="true" CommandArgument='<%# Eval("GangwayName") %>' OnClientClicking="Confirmation"
                                        AutoPostBack="true" CommandName="RowDelete"
                                        ButtonType="LinkButton">
                                    </telerik:RadButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>--%>
            <br />
            <table width="100%">
                <tr>
                    <td class="tdControl" align="center">
                        <asp:Label runat="server" ID="lblErrMsg" ForeColor="Red" />
                        <br />
                        <telerik:RadButton ID="rbtnSave" runat="server" ValidationGroup="SaveUpdateYard"
                            CommandName="Insert" Text="Save" OnClick="rbtnSave_Click" />
                        <telerik:RadButton ID="rbtnCancel" runat="server" OnClientClicking="CloseWindow" Text="Cancel" AutoPostBack="false" />
                        <asp:HiddenField ID="hdnYardID" runat="server" />
                        <asp:HiddenField ID="hdnYardName" runat="server" />
                        <asp:HiddenField ID="hdnOrginalIsTimeCapture" runat="server" />
                        <telerik:RadButton ID="rbtnHidden" runat="server" Style="display: none;" />
                        <br />

                    </td>
                </tr>
            </table>

        </asp:Panel>
        <%--<telerik:RadWindow ID="RadWin_AddEditGangway" runat="server" Style="z-index: 10000;" Modal="true" OnClientBeforeShow="OnClientBeforeShow" OnClientClose="Dropdownclose"
            Behaviors="Close,Reload" VisibleStatusbar="true" AutoSize="true">
            <ContentTemplate>
                <asp:Panel ID="pnlSaveUpdateGangway" runat="server" Height="270px" Width="475px">
                    <table id="tblAddGangway" width="100%" class="table">
                        <tr>
                            <td class="tdLabel" style="width: 25%;">
                                <asp:Label ID="lblGangwayName" runat="server" Text="Gangway Name"></asp:Label>
                            </td>
                            <td class="tdField" colspan="3">
                                <asp:TextBox ID="tbGangwayName" runat="server" MaxLength="100" Width="160px" SkinID="tbMandatory"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rvGangwayName" ValidationGroup="SaveUpdateGangway"
                                    runat="server" ErrorMessage="*" ControlToValidate="tbGangwayName"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="Customvalidator1" Display="Dynamic" runat="server" ErrorMessage="This name is reserved for use"
                                    ValidationGroup="SaveUpdateGangway" ControlToValidate="tbGangwayName" ClientValidationFunction="validateString">
                                </asp:CustomValidator>
                                <br />
                                <asp:Label runat="server" ID="lblGangWayNameVal" ForeColor="Red" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tdLabel">
                                <asp:Label ID="lblGangwayDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td class="tdField" colspan="3">
                                <telerik:RadTextBox ID="rtbGangwayDesc" runat="server" MaxLength="400" TextMode="MultiLine"
                                    SkinID="rtbMandatory" Height="40px" Width="80%">
                                </telerik:RadTextBox>
                                <asp:RequiredFieldValidator ID="rvGangwayDesc" ValidationGroup="SaveUpdateGangway"
                                    runat="server" ErrorMessage="*" ControlToValidate="rtbGangwayDesc"></asp:RequiredFieldValidator>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 31px;" class="tdLabel">
                                <asp:Label ID="lblDeviceName" runat="server" Text="Device"></asp:Label>
                            </td>
                            <td style="height: 31px" class="tdField" colspan="3">
                                <telerik:RadDropDownList ID="rddlDevice" ZIndex="10500" DropDownHeight="160" runat="server"></telerik:RadDropDownList>
                                <br />
                                <asp:Label runat="server" ID="lblDeviceVal" ForeColor="Red" />
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 31px;" class="tdLabel">
                                <asp:Label ID="lblConnectingYard" runat="server" Text="Connecting Yard"></asp:Label>
                            </td>
                            <td style="height: 31px" class="tdField" colspan="3">
                                <telerik:RadDropDownList ID="rddlConnectingYard" runat="server" ZIndex="10500" DropDownHeight="160"></telerik:RadDropDownList>
                                <br />
                                <asp:Label runat="server" ID="lblErrorMsgofConntYard" ForeColor="Red" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td class="tdControl" align="center">
                                <telerik:RadButton ID="rbtnAddGangway" runat="server" Width="50px" ValidationGroup="SaveUpdateGangway"
                                    CommandName="Add" Text="Add" OnClick="rbtnAddGangway_Click" />
                                &nbsp;&nbsp;
                                        <telerik:RadButton ID="rbtnCancelGangway" runat="server" OnClientClicking="CloseGangwayWindow" Width="50px"
                                            Text="Cancel" AutoPostBack="false" />
                                <asp:HiddenField ID="hdnGangwayID" runat="server" />
                                <asp:HiddenField ID="hdnGangwayName" runat="server" />
                                <asp:HiddenField ID="hdnLocationID" runat="server" />
                                <asp:HiddenField ID="hdnDeviceID" runat="server" />
                                <telerik:RadButton ID="rbtn1" runat="server" Style="display: none;" />
                                <br />

                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
        </telerik:RadWindow>--%>
    </asp:Panel>
</asp:Content>

