<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="ImportRawClockTime.aspx.cs" Inherits="astorWork.aspx.TimeAttendance.ImportRawClockTime" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxLoadingPanel runat="server" ID="rlpJobAllowance"></telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="ramJobAllowance">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="pnlImportRawClkTime">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlImportRawClkTime" LoadingPanelID="rlpJobAllowance" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>

    <div class="layout">
        <fieldset style="border: 1px solid #25A0DA; padding: 5px; width: 65%">
            <legend class="legend">Import Raw Clock Time</legend>
            <asp:Panel ID="pnlImportRawClkTime" runat="server">
                <table id="tblImport" style="width: 100%" class="table">
                    <tr>
                        <td class="tdLabel">
                            <span>Direction</span>
                        </td>
                        <td class="tdField">
                            <table>
                                <tr>
                                    <td>
                                        <telerik:RadDropDownList ID="ddlDirection" runat="server" DefaultMessage="Select direction">
                                        </telerik:RadDropDownList>
                                    </td>
                                    <td>

                                        <asp:RequiredFieldValidator ID="rfvddlDirection" runat="server" ValidationGroup="ImportFile"
                                            ErrorMessage="*" ControlToValidate="ddlDirection" Text="*" ForeColor="Red"
                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdLabel">
                            <span>Select File</span>
                        </td>
                        <td class="tdField">
                            <table>
                                <tr>

                                    <td valign="middle" align="center">
                                        <telerik:RadAsyncUpload ID="rfuImport" runat="server" Width="100%" MaxFileInputsCount="1"
                                            OnClientValidationFailed="validationFailed" OnClientFileUploadRemoved="removeFile" OnClientFilesSelected="ImportVal" PostbackTriggers="rbtnImport"
                                            AllowedFileExtensions="xls,xlsx">
                                            <Localization Select="Browse" />
                                        </telerik:RadAsyncUpload>

                                    </td>
                                    <td valign="middle" align="center">
                                        <asp:CustomValidator runat="server" Display="Dynamic" ID="cvImport" ClientValidationFunction="validateUpload" ValidationGroup="ImportFile"
                                            ErrorMessage="*" ForeColor="Red">
                                        </asp:CustomValidator>
                                        <asp:Label ID="lblErrorImport" runat="server" SkinID="lblError" meta:resourcekey="lblErrorImport"></asp:Label>
                                    </td>

                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td class="tdField" align="center" colspan="2">

                            <telerik:RadButton runat="server" ID="rbtnImport" Text="Import" CausesValidation="true" ButtonType="LinkButton"
                                OnClientClicking="postbackButtonClick" OnClick="rbtnImport_Click" ValidationGroup="ImportFile">
                                <Icon PrimaryIconUrl="~/Images/ImportExcel.png" PrimaryIconTop="1" />
                            </telerik:RadButton>
                        </td>


                    </tr>


                </table>
            </asp:Panel>
        </fieldset>
    </div>
    <asp:HiddenField ID="hdnDB" runat="server" />
    <style type="text/css">
        .ruErrorMessage {
            display: block;
            color: #ef0000;
            font-variant: small-caps;
            text-transform: lowercase;
        }
    </style>
    <telerik:RadCodeBlock ID="radCodeBlock" runat="server">
        <script language="javascript" type="text/javascript">
            function removeFile(radAsyncUpload, args) {
                var cvImport = document.getElementById("<%= cvImport.ClientID  %>");
                var btnImport = $find("<%= rbtnImport.ClientID %>");
                btnImport.set_enabled(true);
                cvImport.innerText = "*";
            }
            function validateUpload(sender, args) {
                var upload = $find("<%= rfuImport.ClientID %>");
                args.IsValid = upload.getUploadedFiles().length != 0;

            }
            function validationFailed(radAsyncUpload, args) {

                var $row = $(args.get_row());
                var btnImport = $find("<%= rbtnImport.ClientID %>");
                btnImport.set_enabled(false);
                var erorMessage = getErrorMessage(radAsyncUpload, args);
                var span = createError(erorMessage);
                $row.addClass("ruError");
                $row.append(span);
            }
            function ImportVal() {
                var cvImport = document.getElementById("<%= cvImport.ClientID  %>");
                cvImport.innerText = "";
            }
            function RequestStart(sender, args) {

                if (args._eventTargetElement != null)

                    var loadingPanel = document.getElementById("<%= rlpJobAllowance.ClientID %>");
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
            function getErrorMessage(sender, args) {

                var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);

                if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct

                    if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                        return ("Supports .csv Files only.");
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
             function postbackButtonClick(sender, args) {
                var upload = $find("<%= rfuImport.ClientID %>");
                if ((upload.getUploadedFiles().length == 0) )
                    args.set_cancel(true);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
