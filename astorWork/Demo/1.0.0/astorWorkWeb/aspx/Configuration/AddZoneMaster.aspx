<%@ Page Title="Zone Master" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="AddZoneMaster.aspx.cs" Inherits="astorWork.aspx.Configuration.AddZoneMaster" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        var END_CLICK_RADIUS = 15;
        //the max number of points of your polygon
        var MAX_POINTS = 8;

        var mouseX = 0;
        var mouseY = 0;
        var isStarted = false;
        var polygons = [];

        var canvas = null;
        var ctx;
        var image;
        var image1;
        var IsCompleted = false;
        var hdnIsEdit;
        window.onload = function () {
            hdnIsEdit = document.getElementById("<%=hdnIsEdit.ClientID%>");
            canvas = document.getElementById("canvas");
            ctx = canvas.getContext("2d");
            image = new Image();
            image.onload = function () {
                canvas.width = image.naturalWidth;
                canvas.height = image.naturalHeight;
                ctx.drawImage(image, 0, 0, canvas.width, canvas.height);
            };
            var ZoneID = document.getElementById("<%=hdnZoneID.ClientID %>").value;
            var YardID = document.getElementById("<%=hdnYardID.ClientID %>").value;
            var imageBytes = document.getElementById("<%=hdnImageByte.ClientID %>").value;
            if (YardID != 0)
                image.src = '../../Controls/ImageHandler.ashx?YardID=' + YardID + '&ID=' + Math.random();
            else
                image.src = "data:image/png;base64," + Convert.ToBase64String(imageBytes);;

            canvas.addEventListener("click", function (e) {
                var ContentDiv = document.getElementById("<%=ContentDiv.ClientID %>")
                var topPos = ContentDiv.scrollTop;
                var topLeft = ContentDiv.scrollLeft;
                var x = e.clientX - (canvas.offsetLeft - topLeft);
                var y = e.clientY - (canvas.offsetTop - topPos);
                if (isStarted) {
                    //drawing the next line, and closing the polygon if needed
                    if (Math.abs(x - polygons[polygons.length - 1][0].x) < END_CLICK_RADIUS && Math.abs(y - polygons[polygons.length - 1][0].y) < END_CLICK_RADIUS) {
                        isStarted = false;


                    } else {
                        polygons[polygons.length - 1].push(new Point(x, y));
                        if (polygons[polygons.length - 1].length >= MAX_POINTS) {
                            isStarted = false;
                        }
                    }
                    if (IsCompleted == false)
                        draw();
                } else {
                    //opening the polygon
                    polygons.push([new Point(x, y)]);
                    isStarted = true;
                }
            }, false);

            //we just save the location of the mouse
            canvas.addEventListener("mousemove", function (e) {
                var ContentDiv = document.getElementById("<%=ContentDiv.ClientID %>")
                var topPos = ContentDiv.scrollTop;
                var topLeft = ContentDiv.scrollLeft;
                mouseX = e.clientX - (canvas.offsetLeft - topLeft);
                mouseY = e.clientY - (canvas.offsetTop - topPos);
                if (IsCompleted == false)
                    draw();
            }, false);

        }


            function show(ender, args) {
                hdnIsEdit = document.getElementById("<%=hdnIsEdit.ClientID%>");
                canvas = document.getElementById("canvas");
                ctx = canvas.getContext("2d");
                image = new Image();
                image.onload = function () {
                    canvas.width = image.naturalWidth;
                    canvas.height = image.naturalHeight;
                    ctx.drawImage(image, 0, 0, canvas.width, canvas.height);
                };
                var ZoneID = document.getElementById("<%=hdnZoneID.ClientID %>").value;
            var imageBytes = document.getElementById("<%=hdnImageByte.ClientID %>").value;
                if (ZoneID == 0)
                    image.src = "data:image/png;base64," + imageBytes;
                else
                    image.src = '../../Controls/ImageHandler.ashx?ZoneID=' + ZoneID + '&ID=' + Math.random();

                canvas.addEventListener("click", function (e) {
                    var ContentDiv = document.getElementById("<%=ContentDiv.ClientID %>")
                var topPos = ContentDiv.scrollTop;
                var topLeft = ContentDiv.scrollLeft;
                var x = e.clientX - (canvas.offsetLeft - topLeft);
                var y = e.clientY - (canvas.offsetTop - topPos);
                if (isStarted) {
                    //drawing the next line, and closing the polygon if needed
                    if (Math.abs(x - polygons[polygons.length - 1][0].x) < END_CLICK_RADIUS && Math.abs(y - polygons[polygons.length - 1][0].y) < END_CLICK_RADIUS) {
                        isStarted = false;


                    } else {
                        polygons[polygons.length - 1].push(new Point(x, y));
                        if (polygons[polygons.length - 1].length >= MAX_POINTS) {
                            isStarted = false;
                        }
                    }
                    if (IsCompleted == false)
                        draw();
                } else {
                    //opening the polygon
                    polygons.push([new Point(x, y)]);
                    isStarted = true;
                }
            }, false);

                //we just save the location of the mouse
            canvas.addEventListener("mousemove", function (e) {
                var ContentDiv = document.getElementById("<%=ContentDiv.ClientID %>")
                         var topPos = ContentDiv.scrollTop;
                         var topLeft = ContentDiv.scrollLeft;
                         mouseX = e.clientX - (canvas.offsetLeft - topLeft);
                         mouseY = e.clientY - (canvas.offsetTop - topPos);
                         if (IsCompleted == false)
                             draw();
                     }, false);
                     }
                     function InitiateCanvas() {
                     }

                     //object representing a point
                     function Point(x, y) {
                         this.x = x;
                         this.y = y;
                     }

                     //resets the application
                     function reset(ender, args) {
                         hdnIsEdit.value = "0";
                         isStarted = false;
                         IsCompleted = false;
                         polygons = [];
                         points = null;
                         document.getElementById("<%=hdnCoord.ClientID%>").value = "";
                ctx.globalAlpha = 1;
                ctx.restore();
                draw();
            }
            function FillColor() {


                if (IsCompleted == false)
                    draw();
                else {
                    var colorPicker = $find("<%= rcpkrColor.ClientID %>");
                    if (colorPicker.get_selectedColor() != null)
                        ctx.fillStyle = colorPicker.get_selectedColor();
                    else {

                        ctx.fillStyle = "rgba(255, 255, 255, 0.1)";

                    }
                    ctx.fill();
                }
            }
            //draws the current shape
            function draw() {
                var colorPicker = $find("<%= rcpkrColor.ClientID %>");
                ctx.drawImage(image, 0, 0, canvas.width, canvas.height);
                polygons.forEach(function (points, i) {
                    ctx.beginPath();
                    points.forEach(function (p, j) {
                        if (j) {
                            ctx.lineTo(p.x, p.y);
                        } else {
                            ctx.moveTo(p.x, p.y);
                        }
                    });
                    if (i + 1 === polygons.length && isStarted) { // just the last one
                        ctx.lineTo(mouseX, mouseY);

                    } else {
                        ctx.lineTo(points[0].x, points[0].y);
                        if (colorPicker.get_selectedColor() != null)
                            ctx.fillStyle = colorPicker.get_selectedColor();
                        else
                            ctx.fillStyle = "rgba(0, 0, 0, 0.1)";
                        ctx.globalAlpha = 0.4;
                        ctx.fill();
                        IsCompleted = true;
                    }
                    ctx.lineWidth = 3;
                    ctx.strokeStyle = '#f1c40f';
                    ctx.stroke();


                });
            }

            function Save(sender, args) {
                var Result = false;
                if (polygons.length > 0) {
                    document.getElementById("<%=hdnCoord.ClientID%>").value = JSON.stringify(polygons[0]);
                    Result = true;
                }
                else if (document.getElementById("<%=hdnCoord.ClientID%>").value != "")
                    Result = true;
                else {
                    alert("Please Redraw the Layout");
                    Result = false;
                }

                args.set_cancel(!Result);
            }
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
            //Asynchronois Fileupload
            function validationFailed(radAsyncUpload, args) {

                var $row = $(args.get_row());
                var rbtnSave = document.getElementById("<%=rbnSaveZone.ClientID %>");
                rbtnSave.disabled = 'disabled';
                var erorMessage = getErrorMessage(radAsyncUpload, args);
                var span = createError(erorMessage);
                $row.addClass("ruError");
                $row.append(span);
            }
            function removeFile(radAsyncUpload, args) {

                var rbtnSave = document.getElementById("<%=rbnSaveZone.ClientID %>");
                rbtnSave.disabled = false;
                //     cvValveImage.innerText = "*";

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
      <%--      function validateUpload(sender, args) {
                var hdnFileCount = document.getElementById("<%=hdnFileCount.ClientID%>");
                var upload = $find("<%= rAsyncUZoneImage.ClientID %>");
                args.IsValid = upload.getUploadedFiles().length != 0;
                if (hdnFileCount.value != "")
                    args.IsValid = true;
            }--%>
        //End Asynchronois Fileupload
        //ToolTip
        function BeforeShow() {
            document.getElementById('<%= hdnIsOpen.ClientID %>').value = "true";
        }
        function BeforeHide() {
            document.getElementById('<%= hdnIsOpen.ClientID %>').value = "false";
            }
            //End ToolTip
    </script>
    <style>
        .rtWrapperContent img {
            width: 250px !important;
            height: 200px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hdnYardID" Value="0" />
    <asp:HiddenField runat="server" ID="hdnZoneID" />
    <asp:HiddenField runat="server" ID="hdnImageByte" />
    <asp:HiddenField ID="hdnIsOpen" runat="server" Value="false" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rbnPreview">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlZone" LoadingPanelID="RadAjaxLoadingPanel1"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>

        </AjaxSettings>

    </telerik:RadAjaxManager>

    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="MetroTouch">
    </telerik:RadAjaxLoadingPanel>
    <div class="layout">
        <asp:Panel ID="pnlZone" runat="server" Style="width: 100%">
            <table class="formTable" style="width: 100%">
                <tr>
                    <td style="width: 7%" class="tdLabel">Zone Name
                    </td>
                    <td style="width: 12%" class="tdField">
                        <telerik:RadTextBox SkinID="rtbMandatory" ID="rtbZoneName" Skin="MetroTouch" RenderMode="Lightweight" ValidationGroup="Zone" runat="server" MaxLength="100"></telerik:RadTextBox>

                        <asp:RequiredFieldValidator ID="rfvZoneName" runat="server" ValidationGroup="Zone" ControlToValidate="rtbZoneName" ForeColor="Red" Display="Dynamic">
                        *</asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 4%" class="tdLabel">Description
                    </td>
                    <td style="width: 15%" class="tdField">
                        <telerik:RadTextBox Width="95%" SkinID="rtbMandatory" RenderMode="Lightweight" ID="rtbDesc" ValidationGroup="Zone" runat="server" TextMode="MultiLine"></telerik:RadTextBox>

                        <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ValidationGroup="Zone" ControlToValidate="rtbDesc" ForeColor="Red" Display="Dynamic">
                        *</asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 7%" class="tdLabel">Zone Color
                    </td>
                    <td style="width: 5%" class="tdField">
                        <telerik:RadColorPicker SkinID="rcpkr" OnClientColorChange="FillColor" NoColorText="Transparent" SelectedColor="Red" runat="server" ID="rcpkrColor">
                        </telerik:RadColorPicker>
                        <asp:HiddenField ID="hdnFileCount" runat="server" />
                    </td>
                    <%--    <td style="width: 8%" class="tdLabel">Zone LayOut
                    </td>
                    <td style="width: 15%" class="tdField">

                        <table>
                            <tr>
                                <td>
                                    <telerik:RadAsyncUpload ID="rAsyncUZoneImage" runat="server" Width="100%" MaxFileInputsCount="1" MultipleFileSelection="Disabled"
                                        PostbackTriggers="rbnSaveZone"
                                        OnClientValidationFailed="validationFailed"
                                        OnClientFileUploadRemoved="removeFile"
                                        AutoAddFileInputs="true" AllowedFileExtensions="jpg,png,bmp,jpeg,gif">
                                        <Localization Select="Browse" />
                                    </telerik:RadAsyncUpload>
                                </td>
                                <td>
                                    <asp:CustomValidator runat="server" Display="Dynamic" ID="cvZoneIMage" ForeColor="Red"
                                        ClientValidationFunction="validateUpload" ValidationGroup="Zone"
                                        ErrorMessage="*">
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                        </table>




                    </td>
                    <td class="tdField">


                        <asp:Image ID="imgPreview" runat="server" ImageUrl="~/images/information.png" Visible="false" />

                        <telerik:RadToolTipManager ID="RadToolTipManager1" ContentScrolling="Default" HideEvent="LeaveToolTip" Skin="Metro"
                            OnClientBeforeShow="BeforeShow" OnClientHide="BeforeHide" ManualClose="true"
                            runat="server" OnAjaxUpdate="OnAjaxUpdate" RelativeTo="Element" ToolTipZoneID="imgMapYardView"
                            Position="BottomRight">
                        </telerik:RadToolTipManager>
                    </td>--%>
                </tr>
                <tr>
                    <td colspan="9" class="tdControl">
                        <telerik:RadButton ID="rbnCancel" SkinID="rbnCancel" NavigateUrl="ViewZoneMaster.aspx" ButtonType="LinkButton"
                            runat="server" Text="Cancel">
                        </telerik:RadButton>
                        <telerik:RadButton RenderMode="Lightweight" ID="rbnSaveZone" SkinID="rbnSave"
                            ValidationGroup="Zone" runat="server" OnClientClicking="Save" OnClick="btnSave_Click" Text="Save">
                        </telerik:RadButton>
                        <telerik:RadButton RenderMode="Lightweight" OnClientClicking="reset" AutoPostBack="false" Skin="MetroTouch" SkinID="rbnReset" ID="rbnReset" runat="server" Text="Reset">
                        </telerik:RadButton>

                    </td>
                </tr>

            </table>
        </asp:Panel>
        <div id="ContentDiv" runat="server" style="width: 100%; height: 83vh; overflow: scroll;">
            <canvas id="canvas"></canvas>
            <asp:HiddenField ID="hdnCoord" runat="server" />
            <asp:HiddenField ID="hdnIsEdit" runat="server" />
        </div>
    </div>


</asp:Content>
