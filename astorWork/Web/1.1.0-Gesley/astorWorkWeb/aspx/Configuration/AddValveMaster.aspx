<%@ Page Title="Add Valve Master" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="AddValveMaster.aspx.cs" Inherits="astorWork.aspx.Configuration.AddValveMaster" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../JScript/CommonMethods.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="rcbValve" runat="server">
        <script>
            //Get Coordinates
            function GetCoOrdinates() {
                var rddlZone = $find("<%= rddlZone.ClientID %>");
                var selectedZone = rddlZone.get_selectedItem().get_value();
                if (selectedZone == "-1")
                    return false;


                var oWnd = GetRadWindow();
                oWnd.BrowserWindow.GetCoOrdinates(selectedZone, rddlZone.get_selectedItem().get_text(), document.getElementById("<%=hdnValveID.ClientID%>").value);
                return false;
            }
            function populateCoordinates(arg) {
                document.getElementById("<%=tbImageCoordinates.ClientID %>").value = arg;

            }
            //End Get Coordinates
            function CloseWindow() {
                var oWnd = GetRadWindow();
                oWnd.close();
            }
            function validationFailed(radAsyncUpload, args) {

                var $row = $(args.get_row());
                var rbtnSave = document.getElementById("<%=rbnSave.ClientID %>");
                rbtnSave.disabled = 'disabled';
                var erorMessage = getErrorMessage(radAsyncUpload, args);
                var span = createError(erorMessage);
                $row.addClass("ruError");
                $row.append(span);
            }
            function removeFile(radAsyncUpload, args) {
                var cvValveImage = document.getElementById("<%= cvValveImage.ClientID  %>");
                var rbtnSave = document.getElementById("<%=rbnSave.ClientID %>");
                rbtnSave.disabled = false;
                cvValveImage.innerText = "*";

            }
            function getErrorMessage(sender, args) {
                var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);
                if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct
                    if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                        return ("This file type is not supported.");
                    }
                    else {
                        return ("This file exceeds the maximum allowed size of 500 KB.");
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
            function validateUpload(sender, args) {
                var hdnFileCount = document.getElementById("<%=hdnFileCount.ClientID%>");
                var upload = $find("<%= rAsyncUValveImage.ClientID %>");
                args.IsValid = upload.getUploadedFiles().length != 0;
                if (hdnFileCount.value != "")
                    args.IsValid = true;
            }
            //ClearCoordinates
            function ClearCoordinates() {
                var txtCoordinates = document.getElementById("<%=tbImageCoordinates.ClientID%>");
                txtCoordinates.value = "";
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" />
    <telerik:RadAjaxLoadingPanel ID="rlpRadwin" runat="server" SkinID="RadWin" Style="height: 460px; width: 890px;">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgGangway">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbnCoordinates" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnlValve" runat="server" class="layout">
        <table class="formTable">
            <tr>
                <td class="tdLabel">Zone
                </td>
                <td class="tdField">
                    <telerik:RadLabel ID="rlblZone" Visible="false" runat="server"></telerik:RadLabel>
                    <telerik:RadDropDownList ID="rddlZone" runat="server" OnClientSelectedIndexChanged="ClearCoordinates">
                    </telerik:RadDropDownList>

                    <asp:RequiredFieldValidator ValidationGroup="valve" InitialValue="-1" ForeColor="Red"
                        Display="Dynamic" ID="rfvZone" ControlToValidate="rddlZone" runat="server" ErrorMessage="Select Zone">*</asp:RequiredFieldValidator>
                </td>
                <td class="tdLabel">Valve Type
                </td>
                <td class="tdField">
                    <telerik:RadDropDownList ID="rddlValveType" runat="server"></telerik:RadDropDownList>

                    <asp:RequiredFieldValidator ValidationGroup="valve" InitialValue="-1" ForeColor="Red"
                        Display="Dynamic" ID="rfvValveType" ControlToValidate="rddlValveType" runat="server" ErrorMessage="Select ValveType">*</asp:RequiredFieldValidator>

                </td>


            </tr>
            <tr>
                <td class="tdLabel">Valve Name
                </td>
                <td class="tdField">
                    <telerik:RadTextBox ID="rtbValveName" Width="200px" runat="server" MaxLength="50" SkinID="rtbMandatory"></telerik:RadTextBox>

                    <asp:RequiredFieldValidator ValidationGroup="valve" ForeColor="Red"
                        Display="Dynamic" ID="RequiredFieldValidator1" ControlToValidate="rtbValveName" runat="server" ErrorMessage="Name">*</asp:RequiredFieldValidator>
                </td>
                <td class="tdLabel">Description
                </td>
                <td class="tdField">
                    <telerik:RadTextBox ID="rtbDesc" Width="200px" runat="server" TextMode="MultiLine" SkinID="rtbMandatory"></telerik:RadTextBox>

                    <asp:RequiredFieldValidator ValidationGroup="valve" ForeColor="Red"
                        Display="Dynamic" ID="rfvDesc" ControlToValidate="rtbDesc" runat="server" ErrorMessage="Description">*</asp:RequiredFieldValidator>

                </td>
            </tr>
            <tr>
                <td class="tdLabel">Coordinates
                </td>
                <td class="tdField" style="padding: 0px !important;padding-left:5px !important;">

                    <table style="margin: 0px; padding: 0px; line-height: 5;">
                        <tr>
                            <td style="border-right:solid 2px white;">
                                <asp:TextBox ID="tbImageCoordinates" runat="server" MaxLength="10" SkinID="tbMandatory" Width="35%"></asp:TextBox>
                                (x,y) Pixels
                                            <asp:RequiredFieldValidator ID="rvImageCoOrdinates" Display="Dynamic" Enabled="false" ValidationGroup="valve"
                                                runat="server" ErrorMessage="*" ControlToValidate="tbImageCoordinates"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:ImageButton ImageUrl="~/Images/sample.png" Height="20px" Width="20px" Style="border: none" ValidationGroup="Coordinates" ToolTip="Get Coordinates" runat="server"
                                    ID="imbtnGetCoordinates" OnClientClick="return GetCoOrdinates();" />
                            </td>
                            
                        </tr>
                    </table>


                </td>
                 <td class="tdLabel" >Valve Image  </td>
                <td class="tdField" colspan="3">
                    <table>
                        <tr>
                            <td class="tdField">
                                <table>
                                    <tr>
                                        <td>
                                            <telerik:RadAsyncUpload ID="rAsyncUValveImage" runat="server" Width="100%" MaxFileInputsCount="1" MultipleFileSelection="Disabled"
                                                PostbackTriggers="rbnSave"
                                                OnClientValidationFailed="validationFailed"
                                                OnClientFileUploadRemoved="removeFile"
                                                AutoAddFileInputs="true" AllowedFileExtensions="jpg,png,bmp,jpeg,gif">
                                                <Localization Select="Browse" />
                                            </telerik:RadAsyncUpload>
                                        </td>
                                        <td>
                                            <asp:CustomValidator runat="server" Display="Dynamic" ID="cvValveImage" ForeColor="Red"
                                                ClientValidationFunction="validateUpload" ValidationGroup="valve"
                                                ErrorMessage="*">
                                            </asp:CustomValidator>
                                            <asp:HiddenField ID="hdnFileCount" runat="server" />
                                            <asp:HiddenField ID="hdnValveID" Value="0" runat="server" />
                                        </td>
                                        <td class="tdField" style="width: 10%" runat="server" id="tdImgValve" visible="false">
                                            <telerik:RadBinaryImage Visible="false" runat="server" ID="rbImgValve" Width="25px" ImageStorageLocation="Session" Height="30px" ResizeMode="Fill" />
                                        </td>
                                    </tr>
                                </table>


                            </td>

                        </tr>
                    </table>


                </td>
            </tr>
        </table>
        <table class="formTable" style="margin-top: -2px" width="100%">
          

            <tr>
                <td colspan="5" class="tdControl" align="center">
                    <table align="center">
                        <tr>
                            <td class="tdControl">
                                <telerik:RadButton ID="rbnCancel" SkinID="rbnCancel" OnClientClicking="CloseWindow" AutoPostBack="false" runat="server" Text="Cancel"></telerik:RadButton>
                            </td>
                            <td class="tdControl">
                                <telerik:RadButton ID="rbnSave" ValidationGroup="valve" OnClick="rbnSave_Click" SkinID="rbnSave" runat="server" Text="Save"></telerik:RadButton>
                            </td>
                        </tr>
                    </table>



                </td>
            </tr>
        </table>

    </asp:Panel>
</asp:Content>
