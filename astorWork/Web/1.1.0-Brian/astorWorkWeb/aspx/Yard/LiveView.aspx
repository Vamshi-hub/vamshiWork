<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="LiveView.aspx.cs" Inherits="astorWork.aspx.Yard.LiveView" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <style>
            .rightCol  {
                padding-left: 55px;
                font-size: 14px;
                font-family:   "Segoe UI",Segoe,"Roboto","Droid Sans","Helvetica Neue",Helvetica,Arial,sans-serif;
                text-align: left;
                line-height: 19px;
            }

                   .rightCol  .country  {
                font-size: 24px;
                font-weight: normal;
                line-height: 18px;
                margin-bottom: 20px;
            }

                     .rightCol  .city  {
                font-weight: bold;
            }

                   .rightCol  .email  {
                color: #0394ae;
            }

                       .rightCol  .email a  {
                color: #0394ae;
                text-decoration: none;
            }

                           .rightCol  .email a:hover {
                text-decoration: underline;
            }

            .k-button k-zoom-in {
                display: none !important;
            }

            .RadMap {
                outline: 0;
            }

            .RadMap_Glow.k-widget.k-tooltip {
                border: 1px solid #2E3940;
            }

            div.RadMap_Glow {
                border: 1px solid #51636D;
                background: #A3CDD6;
            }

                div.RadMap_Glow .k-widget, .RadMap_Glow .k-header {
                    background: #29343b;
                }

            .markers {
                /*padding: 3px;*/
                color: #dddddd;
                text-align: center;
            }

            .RadMap.k-tooltip-closable {
                padding-right: 6px !important;
            }

            .markers hr {
                border: 0;
                border-bottom: 1px solid #49545A;
                color: #1A1F22;
                background-color: #1A1F22;
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
                background-color: #29343B;
            }

            .demo-container .mapTitle {
                background: #29343B;
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

            .right-pane {
                float: right;
                overflow: visible;
                position: relative;
            }

                .right-pane .button {
                    float: right;
                    background-color: #1c262c;
                }

                .right-pane .sliding-pane {
                    float: right;
                    visibility: hidden;
                }

            .RadMap_Glow .k-tooltip-button {
                display: none;
            }
        </style>
        <script language="javascript" type="text/javascript">

            function RequestStart(sender, eventArgs) {
                document.getElementById('<%= imgBtnReload.ClientID %>').disabled = true;
                PauseTimer();
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
                PauseTimer();
                GetReloadDuration();
                document.getElementById('<%= imgBtnReload.ClientID %>').disabled = false;
            }
             function PauseTimer() {
                var lblCounter = document.getElementById("<%= lblCounter.ClientID  %>");
                lblCounter.innerHTML = 'Loading Data...';
             
                clearInterval(timer);
            }
            function CloseLoadingPnl() {
                var currentLoadingPanel = $find("<%= ralpYardView.ClientID %>");
                if (currentLoadingPanel != null)
                    currentLoadingPanel.hide("");
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            }
            function OnMarkerCreated(e) {

                var marker = e.marker,
                    tooltip = marker.options.tooltip;

                // The following custom functionality is built due to design decision that tooltips with autoHide="false"
                // should hide previously opened tooltips.
                //setTimeout(function () {
                //    addShowHandler(marker);
                //}, 0);

            }
            function addShowHandler(marker) {
                var tooltip = marker.element.getKendoTooltip();
                //tooltip.bind("show", openAllToolTips);
                //tooltip.bind("close", showAllValidPopups);

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
            window.onload = function InitialpageLoad() {
                if (parent.name == "ContentFrame") {
                    parent.CloseLoadingPnl();
                }
                //GetReloadDuration();
            };
            function GetReloadDuration() {
               <%-- ReloadDuration = document.getElementById("<%= hdnRefreshDuration.ClientID %>").value * 1000;
                timer = setInterval(function () {
                    if (ReloadDuration > 0) {
                        ReloadDuration -= 1000;
                        SetCounterLabelMsg(ReloadDuration);
                    }
                    else if (ReloadDuration == 0) {
                        LoadData();
                    }
                }, 1000);--%>

            }
 function SetCounterLabelMsg(Counter) {
                var lblCounter = document.getElementById("<%= lblCounter.ClientID  %>");
                if (Counter == 0)
                    lblCounter.innerHTML = 'Loading Data...';
                else
                    lblCounter.innerHTML = Counter / 1000 + 's to next refresh.';
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
            var IsWindowOpened;
            var expanded;
            function LoadData() {
                if (!IsWindowOpened && !expanded) {
                    document.getElementById('<%= btnReload.ClientID %>').click();
                }
                else if (!IsWindowOpened) {
                    openAllToolTips();
                }

            }

        </script>
        <%-- Filter Methods--%>
        <script language="javascript">
            function addTab(pageName, pageURL) {
                parent.parent.ContentFrame.AddNewTabs(pageName, pageURL);
            }
            function fireServerButtonEvent(EmpKey) {
                closeAllToolTips();
                var Ids = EmpKey.split('&');
                window.location.href = 'Pivot.aspx?YardID=' + Ids[0];

            }
            //<![CDATA[
            var RadTimePicker2;
            function onLoadRadTimePicker2(sender, args) {
                RadTimePicker2 = sender;
            }
            function validate(sender, args) {
                var d2 = RadTimePicker2.get_selectedDate();
                if ((d2 == null)) {
                    args.IsValid = false;
                }
                else {
                    args.IsValid = true;
                }

            }
            //]]>

            function FileterClicked(source, args) {
                if (typeof (Page_ClientValidate) == 'function') {
                    var isPageValid = Page_ClientValidate('Filter');

                    if (isPageValid) {
                        document.getElementById("spnValidation").innerHTML = "";
                        expanded = false;
                        args.IsValid = true;
                    }
                    else {
                        document.getElementById("spnValidation").innerHTML = "Please select Past Date and Time";

                        args.IsValid = false;
                    }
                }
            }

            function map_load(sender, args) {
                var $ = $telerik.$;
                var kendoMap = sender.get_kendoWidget();
                var Extent = kendo.dataviz.map.Extent;
                var Extent1 = kendoMap.extent();
                // Get the Markers collection
                var markers = kendoMap.markers.items;

                for (var i = 0; i < markers.length; i++) {
                    if (Extent1.contains(markers[i].location())) {
                        markers[i].tooltip.show();
                    }
                }
                kendoMap.center([1.3521, 103.8198]);
                kendoMap.zoom(12); 
            }
            function logZoomEnd(eventArgs) {
                closeAllToolTips();
                var map = eventArgs.sender;
                var Extent1 = map.extent();
                var markers = map.markers.items;
                document.getElementById('<%=hdnZoom.ClientID%>').value = map.zoom();
                for (var i = 0; i < markers.length; i++) {
                    if (Extent1.contains(markers[i].location())) {
                        markers[i].tooltip.show();
                    }

                }
                //  alert(map.zoom());
            }


        </script>
        <style>
            .RadCalendarPopup {
                z-index: 12345 !important;
            }

            .RadCalendarFastNavPopup.RadCalendarPopupShadows {
                z-index: 99999 !important;
            }
        </style>

        <style>
            .switch-field {
                font-family: "Lucida Grande", Tahoma, Verdana, sans-serif;
                overflow: hidden;
            }

                .switch-field input {
                    position: absolute !important;
                    clip: rect(0, 0, 0, 0);
                    height: 1px;
                    width: 1px;
                    border: 0;
                    overflow: hidden;
                }

                .switch-field label {
                    float: left;
                }

                .switch-field label {
                    display: inline-block;
                    width: 60px;
                    background-color: #e4e4e4;
                    color: rgba(0, 0, 0, 0.6);
                    font-size: 14px;
                    font-weight: normal;
                    text-align: center;
                    text-shadow: none;
                    padding: 6px 14px;
                    border: 1px solid rgba(0, 0, 0, 0.2);
                    -webkit-box-shadow: inset 0 1px 3px rgba(0, 0, 0, 0.3), 0 1px rgba(255, 255, 255, 0.1);
                    box-shadow: inset 0 1px 3px rgba(0, 0, 0, 0.3), 0 1px rgba(255, 255, 255, 0.1);
                    -webkit-transition: all 0.1s ease-in-out;
                    -moz-transition: all 0.1s ease-in-out;
                    -ms-transition: all 0.1s ease-in-out;
                    -o-transition: all 0.1s ease-in-out;
                    transition: all 0.1s ease-in-out;
                }

                    .switch-field label:hover {
                        cursor: pointer;
                    }

                .switch-field input:checked + label {
                    /*background-color: #A5DC86;*/
                    background-color: #25a0da;
                    color: white;
                    -webkit-box-shadow: none;
                    box-shadow: none;
                }

                .switch-field label:first-of-type {
                    border-radius: 4px 0 0 4px;
                    margin-right: -2px;
                }

                .switch-field label:last-of-type {
                    border-radius: 0 4px 4px 0;
                }
        </style>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManager ID="ramYardView" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rpbGo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" LoadingPanelID="ralpYardView" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnReload">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" LoadingPanelID="ralpYardView" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="RequestEnd" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralpYardView" runat="server"></telerik:RadAjaxLoadingPanel>
    <asp:Panel runat="server" ID="pnlMain">
        <div class="demo-container size-auto">
            <telerik:RadMap RenderMode="Lightweight" runat="server" ID="rmapLiveView" 
                MinZoom="2" CssClass="MyMap" BackColor="#E6F5F8" Skin="Glow" OnItemDataBound="RadMap_ItemDataBound">
                <DataBindings>
                    <MarkerBinding DataLocationLatitudeField="Latitude" DataLocationLongitudeField="Longitude" />
                </DataBindings>
                <ClientEvents OnLoad="map_load" OnZoomEnd="logZoomEnd" />
                <LayersCollection>
                    <telerik:MapLayer Attribution="" Type="Bing" Shape="pinTarget" Key='<asp:Literal runat="server" Text="<%$ ConfigurationSettings.AppSettings["BingMapKey"] %>" />' ImagerySet="aerialWithLabels">
                        <TooltipSettings Width="300" AutoHide="false">
                            <AnimationSettings>
                                <OpenSettings Duration="300" Effects="fade:in" />
                                <CloseSettings Duration="200" Effects="fade:out" />
                            </AnimationSettings>
                        </TooltipSettings>
                    </telerik:MapLayer>
                </LayersCollection>
            </telerik:RadMap>
        </div>

        <div id="divFooter" runat="server" style="width: 100%; bottom: 5px; position: fixed; right: 10px;"
            align="right">
            <table width="100%" border="0">
                <tr>
                    <td align="right">
                        <asp:Label runat="server" ID="lblCounter" Font-Bold="true" ForeColor="White" />
                        &nbsp;
                            <asp:ImageButton ImageUrl="~/Images/Reload.png" BorderStyle="None" OnClientClick="LoadData();" runat="server" ToolTip="Reload"
                                ID="imgBtnReload" />
                        <asp:Button runat="server" ID="btnReload" Text="clickme"
                            Style="display: none;" />
                    </td>
                </tr>
            </table>
        </div>

        <asp:HiddenField ID="hdnKey" runat="server" />
        <asp:HiddenField ID="hdnDateValue" runat="server" />
        <asp:HiddenField ID="hdnDate" runat="server" />
        <asp:HiddenField ID="hdnRefreshDuration" runat="server" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" Style="display: none" />

        <asp:HiddenField ID="hdnZoom" runat="server" />
    </asp:Panel>

</asp:Content>

