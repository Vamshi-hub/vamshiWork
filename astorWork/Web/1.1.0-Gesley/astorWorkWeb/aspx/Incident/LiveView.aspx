<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true"
    CodeBehind="LiveView.aspx.cs" Inherits="astorWork.LiveView" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Close Button */

        [class*='close-'] {
            color: #777;
            font: 14px/100% arial, sans-serif;
            position: absolute;
            right: 5px;
            text-decoration: none;
            text-shadow: 0 1px 0 #fff;
            top: 5px;
        }

        .close-classic:after {
            content: 'X'; /* ANSI X letter */
        }
        /* Files box */

            .sliding-pane {
                float: right;
                overflow: visible;
                position: relative;
                font-size: 16px;
                color: #eee;
            }

                .sliding-pane .button {
                    float: right;
                    background-color: #1c262c;
                }

                .sliding-pane.sliding-pane {
                    float: right;
                    visibility: hidden;
                }

        .divider-header {
            font-size: 16px;
            margin-bottom: 10px;
        }

        .files-box{
            width: 100%;
        }
        .files-box .files-content p {
            font-size: 16px;
            margin: 6px 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <style>
            .myBtn {
                display: none; /* Hidden by default */
                position: fixed; /* Fixed/sticky position */
                bottom: 20px; /* Place the button at the bottom of the page */
                right: 30px; /* Place the button 30px from the right */
                z-index: 99; /* Make sure it does not overlap */
                border: none; /* Remove borders */
                outline: none; /* Remove outline */
                background-color: red; /* Set a background color */
                color: white; /* Text color */
                cursor: pointer; /* Add a mouse pointer on hover */
                padding: 15px; /* Some padding */
                border-radius: 10px; /* Rounded corners */
            }

            .k-button k-zoom-in {
                display: none !important;
            }

            .RadMap_Glow .k-callout-s {
                border-top-color: white;
            }

            .RadMap {
                outline: 0;
            }

            .RadMap_Glow.k-widget.k-tooltip {
                border: 1px solid #2E3940;
                /*border: none;*/
                background: white !important;
            }


            /*.RadMap_Glow.k-widget.k-tooltip {
                background: none;
            }*/

            div.RadMap_Glow .k-widget, .RadMap_Glow .k-header {
                background: #29343b;
            }
            /*#page-content > .container{
                margin-top:-14px;
                margin-left:-25px;
                margin-right:0px !important;
            }*/

            .markers {
                padding: 5px;
                width: 250px;
                /*color: #dddddd;*/
                background: white;
                color: black;
                text-align: center;
            }

            .RadMap.k-tooltip-closable {
                padding-right: 6px !important;
            }

            .RadMap_Glow .k-marker.k-marker-yellowshape:before {
                color: yellow;
                font-size: 25px;
            }

            .RadMap_Glow .k-marker.k-marker-redshape:before {
                color: red;
                font-size: 25px;
            }

            .RadMap_Glow .k-marker.k-marker-greenshape:before {
                color: lawngreen;
                font-size: 25px;
            }

            .markers hr {
                border: 0;
                border-bottom: 1px solid #49545A;
                color: #1A1F22;
                /*background-color: #1A1F22;*/
                background-color: white;
                height: 1px;
                width: 200px;
            }

            .markers .title {
                /*padding-top: 3px;*/
                text-align: center;
                text-transform: uppercase;
            }

            .markers .details {
                font-size: 11px;
            }

            .markers img {
                /*border: 1px solid #51636D;*/
                border: none;
                border-bottom: none;
            }

            .radMapWrapper {
                padding: 0 35px 35px 35px;
                /*background-color: #29343B;*/
                background-color: white;
            }

            .demo-container .mapTitle {
                /*background: #29343B;*/
                background: white;
                color: #ffffff;
                font-family: 'Segoe UI',Segoe,'Roboto','Droid Sans','Helvetica Neue',Helvetica,Arial,sans-serif;
                font-size: 20px;
                font-weight: lighter;
                text-align: center;
                text-transform: uppercase;
                line-height: 59px;
            }

            #ctl00_ContentPlaceHolder1_rmapLiveView {
                display: block; /* iframes are inline by default */
                background: #000;
                border: none; /* Reset default border */
                height: 100vh; /* Viewport-relative units */
                width: 100vw;
            }

            .k-attribution {
                display: none !important;
            }

            .RadMap_Glow .k-tooltip-button {
                display: none;
            }
        </style>
        <script language="javascript" type="text/javascript">
            function ChangeColor(color) {

                $('.k-marker-greenshape').css('color', color);

            }
            function CAllToggle() {
                document.getElementById("<%=lbRight.ClientID%>").click();
            }
            function fireServerButtonEvent(EmpKey, color) {
                closeAllToolTips();

                // var Ids = EmpKey.split('&');
                document.getElementById("<%=hdnKey.ClientID%>").value = EmpKey;
                document.getElementById("<%=hdnColor.ClientID%>").value = color;
                document.getElementById("<%=btnSubmit.ClientID%>").click();

            }

            function PauseTimer() {
                var lblCounter = document.getElementById("<%= lblCounter.ClientID  %>");
                lblCounter.innerHTML = 'Loading Data...';
                // var $ = $telerik.$;
                var RadWinManager = $find("<%= rwmRptVwr.ClientID %>");
                var IsWindowOpened = false;
                if (RadWinManager.getActiveWindow() != null)
                    IsWindowOpened = true;
                //if (!IsWindowOpened) {
                //    openAllToolTips();
                //}

                clearInterval(timer);
            }

            function RequestStart(sender, eventArgs) {
                document.getElementById('<%= imgBtnReload.ClientID %>').disabled = true;
                // PauseTimer();
                var loadingPanel = document.getElementById("<%= ralpYardView.ClientID %>");
                loadingPanel.style.zIndex = "10000000";
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
            function RequestEnd(sender, eventArgs) {
                //   PauseTimer();
                //  GetReloadDuration();
                document.getElementById('<%= imgBtnReload.ClientID %>').disabled = false;
            }
            function CloseLoadingPnl() {
                var currentLoadingPanel = $find("<%= ralpYardView.ClientID %>");
                if (currentLoadingPanel != null)
                    currentLoadingPanel.hide("");
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            }
            function OnMarkerCreated(e) {

                var marker = e.marker;

                // The following custom functionality is built due to design decision that tooltips with autoHide="false"
                // should hide previously opened tooltips.
                setTimeout(function () {
                    addShowHandler(marker);
                }, 0);

            }
            function addShowHandler(marker) {
                var tooltip = marker.element.getKendoTooltip();
                //tooltip.bind("show", openAllToolTips);
                tooltip.bind("show", hideAllNonValidPopups);

            }

            function hideAllNonValidPopups(e) {
                var shownPopup = e.sender.popup.element[0],
                    $ = $telerik.$,
                    tooltipCollection = $(".k-tooltip");
                tooltipCollection.each(function () {
                    var that = this;
                    if (that != shownPopup) {
                        $(that).getKendoPopup().close();
                    }
                })
            }
            function closeAllToolTips() {
                var $ = $telerik.$,
                    tooltipCollection = $(".k-tooltip");
                tooltipCollection.each(function () {
                    var that = this;
                    $(that).getKendoPopup().close();
                });
            }

            function openAllToolTips() {
                var $ = $telerik.$;
                $(".k-tooltip").css("display", "block");
                var tooltipCollection = $(".k-tooltip");
                tooltipCollection.each(function () {
                    var that = this;
                    $(that).getKendoPopup().open();
                });

            }

            var ReloadDuration = 0;
            var timer = null;
            //window.onload = function InitialpageLoad() {
            //    if (parent.name == "ContentFrame") {
            //        parent.CloseLoadingPnl();
            //    }
            //    GetReloadDuration();
            //};
            function GetReloadDuration() {
                ReloadDuration = document.getElementById("<%= hdnRefreshDuration.ClientID %>").value * 1000;
                timer = setInterval(function () {
                    if (ReloadDuration > 0) {
                        ReloadDuration -= 1000;
                        SetCounterLabelMsg(ReloadDuration);
                    }
                    else if (ReloadDuration == 0) {
                        LoadData();
                    }
                }, 1000);

            }
            function SetCounterLabelMsg(Counter) {

                <%--var RadWinManager = $find('<%= rwmRptVwr.ClientID %>');
                                    var IsWindowOpened = false;
                                    if (RadWinManager.getActiveWindow() != null)
                                        IsWindowOpened = true;
                                    if (!IsWindowOpened) {
                                        openAllToolTips();
                                    }--%>
                var lblCounter = document.getElementById("<%= lblCounter.ClientID  %>");
                if (Counter == 0)
                    lblCounter.innerHTML = 'Loading Data...';
                else
                    lblCounter.innerHTML = Counter / 1000 + 's to next refresh.';
            }
            function LoadData() {
                var RadWinManager = $find('<%= rwmRptVwr.ClientID %>');
                var IsWindowOpened = false;
                if (RadWinManager.getActiveWindow() != null)
                    IsWindowOpened = true;

                if (!IsWindowOpened && !expanded) {
                    document.getElementById('<%= btnReload.ClientID %>').click();
                }
                //else if (!IsWindowOpened) {
                //    //  openAllToolTips();
                //}

            }
        </script>
        <%-- Filter Methods--%>
        <script language="javascript">
            var expanded = false;
            function openRadpanelBar() {
                var element = $get('<%=Div2.ClientID %>');
                if (!expanded) {
                    element.style.visibility = "visible";
                    element.style.display = "block";
                }
                else {
                    element.style.visibility = "hidden";
                    element.style.display = "none";
                }
                expanded = !expanded;
                return false;
            }
            function openRadpanelBarCheck() {
                var element = $get('<%=panelbar.ClientID %>');

                element.style.visibility = "visible";
                element.style.display = "block";

                expanded = true;
            }

            function closeRadPanelBar() {
                var element = $get('<%=panelbar.ClientID %>');
                element.style.visibility = "hidden";
                element.style.display = "none";
                expanded = false;
            }

            function resetRadPanelBar() {
                var element = $get('<%=panelbar.ClientID %>');
                element.style.visibility = "visible";
                element.style.display = "block";
                expanded = true;
            }
            function ClearSelection(source, args) {
              <%--  var rcbLoc = $find('<%= rcbLocation.ClientID %>')
                if (rcbLoc != null) {
                    for (var i = 0; i < rcbLoc.get_items().get_count() ; i++) {
                        if (rcbLoc.get_items().getItem(i).get_checked())
                            rcbLoc.get_items().getItem(i).set_checked(false);
                    }
                    rcbLoc.clearSelection();
                    rcbLoc.Text = " ";
                    rcbLoc.set_emptyMessage("-All Locations-");
                }--%>
                //  return false;
            }
            function ValidateforLocation(source, args) {
                var combo = $find(source.controltovalidate);
                var emptyMessage = combo.get_emptyMessage();
                var comboText = combo.get_text();
                if (combo.get_checkedItems().length >= 1 || comboText === emptyMessage) {
                    args.IsValid = true;
                }
                else {
                    args.IsValid = false;
                }
            }
            function FileterClicked(source, args) {
                if (Page_ClientValidate("Filter")) {
                    expanded = false;
                    args.IsValid = true;
                }
                else {
                    args.IsValid = false;
                }
            }

            function map_load(sender, args) {
                var $ = $telerik.$;
                var kendoMap = sender.get_kendoWidget();
                var Extent = kendo.dataviz.map.Extent;

                // Get the Markers collection
                var markers = kendoMap.markers.items;

                if (document.getElementById("<%=hdnZoom.ClientID%>").value == "") {
                    var markerLocations = [];
                    // Extract the markers' locations.
                    for (var i = 0; i < markers.length; i++) {
                        markerLocations.push(markers[i].location());
                    }
                    // Create an extent based on the first marker
                    var myExtent = Extent.create(markerLocations[0], markerLocations[0]);
                    // Extend the extent with all markers
                    myExtent.includeAll(markerLocations);
                    // Center RadMap based on the created extent.
                    kendoMap.extent(myExtent);
                    // You may need to zoom out to show all markers properly. 
                    // This can be furtehr adjusted based on your own preferences.

                    // kendoMap.center([parseFloat(document.getElementById('<%=hdnCenterLat.ClientID%>').value), parseFloat(document.getElementById('<%=hdnCenterLong.ClientID%>').value)]);
                    kendoMap.zoom(kendoMap.zoom() - 1);
                    //map = sender.get_kendoWidget();
                    ////If we have items, we'll show our item tooltip
                }


            }
            function logZoomEnd(eventArgs) {
                closeAllToolTips();
                var map = eventArgs.sender;
                var Extent1 = map.extent();
                var markers = map.markers.items;
                document.getElementById('<%=hdnZoom.ClientID%>').value = map.zoom();
                //for (var i = 0; i < markers.length; i++) {
                //    if (Extent1.contains(markers[i].location())) {
                //        markers[i].tooltip.show();
                //    }

                //}
                //  alert(map.zoom());
            }
            function OpenRadWindow(UserManualURL) {
                closeRadPanelBar();
                var UMWindow = $find("<%= rwEmpDetails.ClientID%>");
                var url;
                if (UserManualURL == 'pdf')
                    url = document.getElementById('<%=hdnPDF.ClientID%>').value;
                else
                    url = document.getElementById('<%=hdnVideo.ClientID%>').value;

                //UMWindow.set_width(900);
                UMWindow.set_height(480);
                //UMWindow.setUrl("UserManual.aspx?UserManualURL=" + url);
                UMWindow.setUrl(url);
                UMWindow.show();
                return false;
            }

        </script>

        <link rel="stylesheet" type="text/css" href="assets/widgets/input-switch/inputswitch.css">
        <link rel="stylesheet" type="text/css" href="assets/widgets/input-switch/inputswitch-alt.css">

        <script type="text/javascript" src="assets/widgets/input-switch/inputswitch.js"></script>
        <script type="text/javascript">
            /* Input switch */

            $(function () {
                "use strict";
                $('.input-switch').bootstrapSwitch();
            });
            function SizeToFit(sender, eventArgs) {
                var oWnd = sender;
                oWnd.SetWidth(document.body.clientWidth / 2);
                sender.center();
                //oWnd.SetHeight(document.body.scrollHeight + 70);
            }
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }
        </script>

    </telerik:RadCodeBlock>
    <telerik:RadAjaxManager ID="ramYardView" runat="server" DefaultLoadingPanelID="ralpYardView">
        <AjaxSettings>

            <telerik:AjaxSetting AjaxControlID="lbIncident">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rwDetails" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbNearMiss">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rwDetails" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbLastInspection">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rwDetails" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="rpbtnFilter">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rmapLiveView" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="RequestEnd" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralpYardView" runat="server"></telerik:RadAjaxLoadingPanel>




    <asp:Panel runat="server" ID="pnlMain" CssClass="row">


        <div class="col-md-12">
            <div id="divFooter" runat="server" style="width: auto; top: 0px; position: relative; right: 10px; z-index: 10005; float: right"
                align="right">
                <table width="100%" border="0">
                    <tr>
                        <td align="right">

                            <asp:Label runat="server" ID="lblCounter" Font-Bold="true" ForeColor="White" />
                            &nbsp;
                            <asp:ImageButton ImageUrl="~/Images/Reload.png" BorderStyle="None" OnClientClick="LoadData();" runat="server" ToolTip="Reload"
                                ID="imgBtnReload" />
                            <asp:Button runat="server" ID="btnReload" Text="clickme" OnClick="btnReload_Click"
                                Style="display: none;" />
                        </td>
                    </tr>
                    <tr class="bg-blue">
                        <td><span>Total No. of Projects: </span>
                            <asp:Label runat="server" ID="lblProjectCount" Text="Total No. of Projects" Font-Bold="true" ForeColor="White" /></td>
                    </tr>
                </table>
            </div>
            <div style="width: auto; top: 5px; position: relative; right: -90px; z-index: 10005; float: right" id="div1" runat="server"
                align="right">
                <asp:ImageButton ImageUrl="~/Images/filter.png" BackColor="#29343B" Width="25px" ToolTip="Filter" CssClass="button" OnClientClick="return openRadpanelBar();" runat="server"
                    ID="ImageButton2" />
                <div id="Div2" style="right: 21px; display: none" runat="server" class="sliding-pane">
                    <table class="table" style="background-color: white; padding: 4px;">
                        <tr>
                            <td colspan="2" style="background-color: #25a0da; padding: 4px">
                                <span style="color: white; font-weight: bold; font-size: 10pt;">Filter:
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td>

                                <table>
                                    <tr>
                                        <td class="tdLabel">
                                            <span style="white-space: nowrap;">Division</span>
                                        </td>
                                        <td class="tdField">
                                            <telerik:RadComboBox EmptyMessage="-All Divisions-" AutoPostBack="true" OnSelectedIndexChanged="rcbDesignation_SelectedIndexChanged" ID="rcbDivisions" runat="server" CheckBoxes="true" Width="95%" Height="300px" ZIndex="1000000"
                                                ChangeTextOnKeyBoardNavigation="false" EnableCheckAllItemsCheckBox="true" Filter="Contains"
                                                Localization-AllItemsCheckedString="-All Divisions-" Localization-CheckAllString="-All Divisions-">
                                            </telerik:RadComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdLabel" style="width: 30%">
                                            <span>Project</span>
                                        </td>
                                        <td class="tdField" style="width: 70%">
                                            <telerik:RadComboBox EmptyMessage="-All Projects-" ID="rcbProjects" runat="server" CheckBoxes="true" Width="95%" Height="300px" ZIndex="1000000"
                                                ChangeTextOnKeyBoardNavigation="false" EnableCheckAllItemsCheckBox="true" Filter="Contains"
                                                Localization-AllItemsCheckedString="-All Projects-" Localization-CheckAllString="-All Projects-">
                                            </telerik:RadComboBox>

                                        </td>
                                    </tr>

                                </table>


                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <center>
                                    <table>
                                        <tr>
                                            <td colspan="2" align="center">
                                                <telerik:RadPushButton ID="rpbtnFilter" runat="server" Text="Apply" OnClick="rbtnFilter_Click" SkinID="rpButton" />
                                                &nbsp;&nbsp;
                                          
                                       <telerik:RadPushButton ID="rpbtnReset" runat="server" Text="Reset" OnClick="rbtnReset_Click" CausesValidation="false" SkinID="rpButton" />

                                            </td>
                                        </tr>
                                    </table>
                                </center>
                            </td>
                        </tr>
                    </table>




                </div>
            </div>
            <!--
            <div style="width: auto; top: 0px; position: relative; left: 10px; z-index: 10005; float: left">
                <div class="btn-group">
                    <asp:Button ID="btnMap" CommandArgument="false" OnClick="btnMap_Click" CssClass="btn btn-sm btn-default" runat="server" Text="Map" />
                    <asp:Button ID="btnSattilite" CommandArgument="true" OnClick="btnMap_Click" CssClass="btn btn-sm btn-default" runat="server" Text="Satellite" />

                </div>
            </div>
            -->
            <div class="demo-container size-auto" style="height: 100%">

                <telerik:RadMap RenderMode="Lightweight" Width="100%" runat="server" ID="rmapLiveView" 
                    CssClass="MyMap" BackColor="#E6F5F8" Skin="Glow" OnItemDataBound="RadMap_ItemDataBound">
                    <DataBindings>
                        <MarkerBinding DataLocationLatitudeField="Lattitude" DataLocationLongitudeField="Logitude" />
                    </DataBindings>
                    <%--OnLoad="map_load"OnMarkerCreated--%>
                    <ClientEvents OnLoad="map_load" OnZoomEnd="logZoomEnd" OnMarkerActivate="OnMarkerCreated" />
                    <LayersCollection>
                        <telerik:MapLayer Attribution="" Type="Bing" Shape="pinTarget" Key='<asp:Literal runat="server" Text="<%$ ConfigurationSettings.AppSettings["BingMapKey"] %>" />' ImagerySet="aerialWithLabels">
                            <TooltipSettings AutoHide="false">
                                <AnimationSettings>
                                    <OpenSettings Duration="300" Effects="fade:in" />
                                    <CloseSettings Duration="200" Effects="fade:out" />
                                </AnimationSettings>
                            </TooltipSettings>
                        </telerik:MapLayer>
                    </LayersCollection>
                </telerik:RadMap>


                <div style="width: auto; height: 100%; top: 10px; position: fixed; right: 10px; z-index: 10005;" id="divFilter" runat="server"
                    align="right">
                    <asp:ImageButton Visible="false" ImageUrl="~/Images/filter.png" BackColor="#29343B" Width="25px" ToolTip="Filter" CssClass="button" OnClientClick="return openRadpanelBar();" runat="server"
                        ID="ImageButton1" />
                    <div id="panelbar" style="right: 21px; display: none; height: 100%" runat="server" class="sliding-pane">
                        <table style="background-color: #333; width: 50%; padding: 4px; height: 100%">
                            <tr>
                                <td colspan="2" style="width: 20%; padding: 4px">
                                    <div style="height: 100%;" class="bg-black">
                                        <a style="float: right" href="javascript:closeRadPanelBar();">
                                            <img src="../../Images/RemoveTab.png" /></a>
                                        <div class="pad15A">
                                            <div class="divider-header" style="opacity: unset; font-size: 20px; margin-bottom: 10px;">Project details</div>
                                            <ul class="files-box">
                                                <li>
                                                    <div class="files-content">
                                                        <p>Project Name</p>
                                                        <b>
                                                            <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                                                        </b>
                                                    </div>


                                                </li>
                                                <li class="divider" style="background: rgba(255,255,255,.1); padding: 1px;"></li>
                                                <li>
                                                    <div class="files-content">
                                                        <p>Division</p>
                                                        <b>
                                                            <asp:Label ID="lblDivision" runat="server"></asp:Label>
                                                        </b>
                                                    </div>


                                                </li>
                                                <li class="divider" style="background: rgba(255,255,255,.1); padding: 1px;"></li>
                                                <li>
                                                    <div class="files-content">
                                                        <p>Project Type</p>
                                                        <b>
                                                            <asp:Label ID="lblProjType" runat="server"></asp:Label>
                                                        </b>
                                                    </div>


                                                </li>
                                                <li class="divider" style="background: rgba(255,255,255,.1); padding: 1px;"></li>
                                                <li>
                                                    <div class="files-content">
                                                        <p>Contractor</p>
                                                        <b>
                                                            <asp:Label ID="lblContractor" runat="server"></asp:Label>
                                                        </b>
                                                    </div>
                                                </li>
                                                <li class="divider" style="background: rgba(255,255,255,.1); padding: 1px;"></li>
                                                <li>
                                                    <div class="files-content" style="width: 100%">
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td>
                                                                    <span>AFR</span>
                                                                </td>
                                                                <td>
                                                                    <span>ASR</span>
                                                                </td>
                                                                <td>
                                                                    <span>FAR</span>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblAFR" Font-Size="14pt" runat="server"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblASR" Font-Size="14pt" runat="server"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblFAR" Font-Size="14pt" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>

                                                        </table>
                                                    </div>

                                                </li>
                                                <li class="divider" style="background: rgba(255,255,255,.1); padding: 1px;"></li>
                                                <li>
                                                    <div class="files-content" style="width: 100%">
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <th>
                                                                    <span>Incident</span>
                                                                </th>
                                                                <th>
                                                                    <span>Near Miss</span>
                                                                </th>

                                                            </tr>
                                                            <tr>
                                                                <td>

                                                                    <asp:LinkButton OnClick="lbIncident_Click" Font-Size="14pt" CommandArgument="Incident" ID="lbIncident" runat="server"></asp:LinkButton>


                                                                </td>
                                                                <td>
                                                                    <asp:LinkButton ID="lbNearMiss" Font-Size="14pt" CommandArgument="Incident" OnClick="lbIncident_Click" runat="server"></asp:LinkButton>

                                                                </td>

                                                            </tr>

                                                        </table>
                                                    </div>

                                                </li>
                                                <li class="divider" style="background: rgba(255,255,255,.1); padding: 1px;"></li>
                                                <li id="li4" runat="server">

                                                    <div class="files-content">
                                                        <table style="width: 100%;">
                                                            <tr>

                                                                <td>
                                                                    <p>Date of Last Inspection</p>
                                                                    <b>
                                                                        <asp:LinkButton ID="lbLastInspection" CommandArgument="Inspection" OnClick="lbIncident_Click" runat="server"></asp:LinkButton>

                                                                    </b></td>
                                                                <td>
                                                                    <p>Live Feed</p>
                                                                    <b>
                                                        <asp:LinkButton ID="lbLiveFeed" ForeColor="White" OnClientClick="return OpenRadWindow('video');" runat="server"><img width="50px" src="../../Images/CCTV1.png" /></asp:LinkButton>
                                                    
                                                                    </b></td>
                                                            </tr>
                                                        </table>
                                                    </div>


                                                </li>
                                                <li class="divider" style="background: rgba(255,255,255,.1); padding: 1px;"></li>
                                                <li>
                                                    <div class="files-content">
                                                        <p>
                                                            Project Description
                                                        </p>
                                                        <b>
                                                            <asp:Label ID="lblProject" runat="server" Style="word-wrap: normal; word-break: break-all;"></asp:Label>
                                                        </b>
                                                    </div>
                                                </li>

                                            </ul>
                                        </div>

                                    </div>
                                </td>
                            </tr>

                        </table>
                    </div>
                </div>


            </div>
            <asp:HiddenField ID="hdnKey" runat="server" />
            <asp:HiddenField ID="hdnPDF" runat="server" />
            <asp:HiddenField ID="hdnCenterLat" runat="server" />
            <asp:HiddenField ID="hdnCenterLong" runat="server" />
            <asp:HiddenField ID="HiddenField2" runat="server" />

            <asp:HiddenField ID="hdnRefreshDuration" runat="server" />
            <asp:HiddenField ID="hdnColor" runat="server" />
            <asp:LinkButton ID="lbRight" Style="display: none" runat="server" CssClass="btn "></asp:LinkButton>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" Style="display: none" OnClick="lbRight_Click" />


            <telerik:RadWindowManager ID="rwmRptVwr" runat="server" Behaviors="Close">

                <Windows>

                    <telerik:RadWindow ID="rwEmpDetails" Title="Live Feed" OnClientShow="SizeToFit" MinHeight="250px" Behaviors="Maximize,Close" runat="server">
                    </telerik:RadWindow>

                    <telerik:RadWindow ID="rwDetails" runat="server" Title="Incident Details" OnClientShow="SizeToFit" Behaviors="Maximize,Close">

                        <ContentTemplate>

                            <div class="row">
                                <div class="col-md-12">
                                    <asp:HiddenField ID="hdnVideo" runat="server" />

                                    <telerik:RadGrid Width="500px" RenderMode="Lightweight" ID="rdGrid" runat="server" AutoGenerateColumns="false">

                                        <MasterTableView>
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="Remarks" UniqueName="col1" HeaderText="Remarks"></telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="IncidentDate" ItemStyle-Width="100px" UniqueName="col2" HeaderText="Date of Incident"></telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="IncidentType" UniqueName="col3" HeaderText="Date of Incident"></telerik:GridBoundColumn>
                                                <telerik:GridTemplateColumn UniqueName="pdf">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbPDF" OnClientClick="return OpenRadWindow('pdf');" runat="server"><img width="50px" src="../../Images/pdf.png" /></asp:LinkButton>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn UniqueName="video">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbVideo" OnClientClick="return OpenRadWindow('video');" runat="server"><img width="50px" src="../../Images/CCTV.png" /></asp:LinkButton>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>
                            </div>
                        </ContentTemplate>
                    </telerik:RadWindow>



                </Windows>
            </telerik:RadWindowManager>
            <asp:HiddenField ID="hdnZoom" runat="server" />
        </div>



    </asp:Panel>



    <!-- Slidebars -->

    <script type="text/javascript" src="assets/widgets/slidebars/slidebars.js"></script>
    <script type="text/javascript" src="assets/widgets/slidebars/slidebars-demo.js"></script>
</asp:Content>
