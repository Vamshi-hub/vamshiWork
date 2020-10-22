<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TreeViewPage.aspx.cs" EnableSessionState="ReadOnly" Inherits="astorWork.aspx.Main.TreeViewPage" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .x-tree-node {
            color: #000 !important;
            font: 13px Segoe UI, Arial, Helvetica, sans-serif !important;
            margin-bottom: 5px;
        }

        .moduleStyle {
            white-space: nowrap;
            margin-bottom: 5px;
        }

        html, body, form {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }

        .RadPanelBar .rpLevel1 .rpLink .rpText {
            margin-left: 30px;
            white-space: nowrap;
        }

        .RadPanelBar .rpLevel2 .rpLink .rpText {
            margin-left: 30px;
            white-space: nowrap;
        }

        .RadTile_Metro .rtilePeekContent {
            background-color: rgba(217, 216, 217, 1) !important;
        }

        .RadButton_Metro.rbSkinnedButton, .RadButton_Metro.rbVerticalButton, .RadButton_Metro .rbDecorated, .RadButton_Metro.rbVerticalButton {
            background-color: rgba(37, 134, 218, 1) !important;
            border: rgba(37, 134, 218, 1) !important;
        }

        .rntNotification {
            /*padding-bottom: 2px !important;*/
            background-color: gray !important;
            /*opacity:0.5 !important;*/
            /*background:rgba(black ,0.5) !important;*/
        }

        .RadTile.rtileWide {
            width: 45% !important;
        }

        .lblCount {
            color: #FFFFFF;
            font-size: 12pt;
            white-space: normal;
            vertical-align: middle;
            text-align: center;
        }

        .lblTileHeader {
            font-family: "segoe ui, Arial,sans-serif";
            font-size: 25px;
            font-weight: normal;
            line-height: 38px;
            color: #008592;
            margin: 5px 0 20px;
        }

        .TileCell {
            vertical-align: middle;
            padding-left: 5%;
            padding-right: 0%;
        }

        .RadPanelBar .rpGroup .rpImage {
            padding: 8px 3px 3px 16px !important;
            width: 20px;
            height: 20px;
        }

        .rpOut img {
            padding: 15px 3px 3px !important;
            width: 20px;
        }

        .RadPanelBar .rpLevel2 .rpTemplate, .RadPanelBar .rpLevel2 .rpOut {
            padding-left: 20px !important;
        }

        .rtileHovered {
            cursor: pointer !important;
        }

        #rntfnFrequent_titlebar {
            display: none !important;
        }

        .RadButton_Default.rbSkinnedButton, .RadButton_Default .rbDecorated, .RadButton_Default.rbVerticalButton, .RadButton_Default.rbVerticalButton .rbDecorated, .RadButton_Default .rbSplitRight, .RadButton_Default .rbSplitLeft {
            background-image: none !important;
            background-color: rgba(37, 134, 218, 1) !important;
        }
    </style>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">

            var moduleArray;
            var pageArray;
            var interval = 1;


            function addTab(pageName, pageURL) {
                //parent.ContentFrame.addTab(pageName, pageURL);
                parent.ContentFrame.AddNewTabs(pageName, pageURL);
            }
            //Disabling Enter key in TreePanel Filter
            function SwallowEnter() {
                if (event.keyCode == 13) {
                    Ext.EventObject.stopEvent();
                }
            }

            function Collapse(iteration) {
                var radtabpanelbar = document.getElementById("rpbModuleItems");
                if (iteration >= 2) {
                    var size = iteration + '%,*';
                    parent.document.getElementById('Bottom').cols = size;
                    iteration = iteration - 1;
                    setTimeout(function () { Collapse(iteration) }, interval);
                }
                else {
                    radtabpanelbar.style.display = 'none';
                }
            }

            function Expand(iteration) {
                if (iteration <= 22) {
                    var size = iteration + '%,*';
                    parent.document.getElementById('Bottom').cols = size;
                    iteration = iteration + 1;
                    setTimeout(function () { Expand(iteration) }, interval);
                }
            }

            function CollapseExpandTreePanel(CollapseExpand) {
                var radtabpanelbar = document.getElementById("rpbModuleItems");

                var size;
                if (CollapseExpand == 'Collapse') {
                    size = parent.document.getElementById('Bottom').cols.split(',')[0].replace("%", "");
                    Collapse(2);
                    parent.document.getElementById('TreeViewFrame').noResize = true;
                    btnCollapse.hide();
                    btnExpand.show();
                }
                else if (CollapseExpand == 'Expand') {
                    Expand(22);
                    parent.document.getElementById('TreeViewFrame').noResize = false;
                    btnExpand.hide();
                    btnCollapse.show();
                    radtabpanelbar.style.display = 'block';
                }
            }

            function ShowHideRadTabPanel(value) {
                var radtabpanelbar = document.getElementById("rpbModuleItems");

                if (value == 0)
                    radtabpanelbar.style.display = 'none';
                else
                    radtabpanelbar.style.display = 'block';
            }


            function toggle(sender, args) {
                var panelBar = $find("<%= rpbModuleItems.ClientID %>");
                for (var i = 0; i < panelBar.get_allItems().length; i++) {
                    panelBar.get_allItems()[i].collapse();
                }

            }

            function OnClientHidingNotification() {
                var panelBar = $find("<%= rpbModuleItems.ClientID %>");
                panelBar.get_items().getItem(0).expand();
                $find("<%=rntfnFrequent.ClientID %>").set_enabled(false);
                document.getElementById("<%=hdnIsOpend.ClientID %>").value = "False";

            }

            function HideNotificationOnTabClick() {
                var notification = $find("<%=rntfnFrequent.ClientID %>");
                if (notification != null) {
                    notification.hide();
                    notification.set_enabled(false);
                    notification.set_text("SHOW FREQUENTLY USED PAGES");
                }
                document.getElementById("<%=hdnIsOpend.ClientID %>").value = "False";

            }
            function OpenNewTab(sender, args) {
                parent.ContentFrame.AddNewTabs(sender.innerText, sender.getAttribute("Url"));
                return false;
            }
            function OnClientShowingNotification() {
                document.getElementById("<%=hdnIsOpend.ClientID %>").value = "True";
            }
            function OnClientTileClicked(sender, args) {
                var tileId = sender._element.id;
                var lnkBtnID = sender._element.id + "_lnkbtnFrequent" + tileId.charAt(tileId.length - 1);
                var lnkElement = document.getElementById(lnkBtnID);
                var pageName = lnkElement.innerText.trim();
                var url = lnkElement.getAttribute("url");
                parent.ContentFrame.AddNewTabs(pageName, url);
                return false;
            }
            function PageResizeEvent() {
                $find("<%=rntfnFrequent.ClientID %>").hide();
                $find("<%=rntfnFrequent.ClientID %>").set_enabled(false);
                document.getElementById("<%=hdnIsOpend.ClientID %>").value = "False";

            }
        </script>

    </telerik:RadCodeBlock>

</head>
<body onresize="PageResizeEvent()">
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" SkinID="treeView" />
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="180"
            EnableScriptGlobalization="true" EnablePageMethods="true">
        </asp:ScriptManager>

        <div>
            <asp:HiddenField runat="server" ID="hdnIsOpend" Value="False" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Inline">
                <ContentTemplate>
                    <div style="position: absolute; top: 1px; display: none; z-index: 1000000;">
                        <ext:Button ID="btnCollapse" runat="server" Icon="ControlRewind" ToolTip="Collapse"
                            StandOut="true" OnClientClick="CollapseExpandTreePanel('Collapse');" />

                        <ext:Button ID="btnExpand" runat="server" Icon="ControlFastforward" ToolTip="Expand"
                            Hidden="true" OnClientClick="CollapseExpandTreePanel('Expand');" StandOut="true" />
                    </div>
                    <telerik:RadPanelBar runat="server" ID="rpbModuleItems" Height="100%" Width="100%" ExpandMode="SingleExpandedItem" Skin="MetroTouch">
                        <Items>
                            <telerik:RadPanelItem Text="Configuration" ImageUrl="../../images/config.png" Expanded="True"
                                ID="menuConfig" Visible="false">
                                <Items>
                                    <telerik:RadPanelItem ImageUrl="../../images/535106-user_512x512.png" Text="User Master" onclick="addTab('User Master','../Configuration/ViewUsers.aspx');" />
                                    <telerik:RadPanelItem ImageUrl="../../images/yardmaster.png" Text="Site Master" onclick="addTab('Site Master','../Configuration/AddViewYardMaster.aspx');" />
                                    <telerik:RadPanelItem ImageUrl="../../images/Zone.png" Text="Zone Master" onclick="addTab('Zone Master','../Configuration/ViewZoneMaster.aspx');" />
                                </Items>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="Resource Management" ImageUrl="../../images/yard.png">
                                <Items>
                                    <telerik:RadPanelItem ImageUrl="../../images/liveview.png" Text="Live View" onclick="addTab('Live View (Resource)','../Yard/LiveView.aspx');" />
                                    <telerik:RadPanelItem Text="Workforce" ImageUrl="../../images/Page_Icon.png">
                                <Items>
                                    <telerik:RadPanelItem ImageUrl="../../images/dashboard.png" Text="Dashboard" onclick="addTab('Dashboard (Workforce)','../Yard/EmployeeDashboard.aspx');" />
                                    <telerik:RadPanelItem ImageUrl="../../images/siteview.png" Text="Site View" onclick="addTab('Site View (Workforce)','../Yard/Pivot.aspx?type=0');" />
                                    <telerik:RadPanelItem ImageUrl="../../images/material_management.png" Text="Active Site Employees" onclick="addTab('Active Site Employees','../Yard/ActiveYardEmployees.aspx');" />
                                        </Items>
                                    </telerik:RadPanelItem>
                                    <telerik:RadPanelItem Text="Material" ImageUrl="../../images/Page_Icon.png">
                                <Items>
                                    <telerik:RadPanelItem ImageUrl="../../images/dashboard.png" Text="Dashboard" onclick="addTab('Dashboard (Material)','../Material/Dashboard.aspx');" />
                                    <telerik:RadPanelItem ImageUrl="../../images/siteview.png" Text="Site View" onclick="addTab('Site View (Material)','../Yard/Pivot.aspx?type=1');" />
                                    <telerik:RadPanelItem ImageUrl="../../images/material.png" Text="Material Master" onclick="addTab('Material Master','../Material/MaterialMaster.aspx?type=1');" />
                                    <telerik:RadPanelItem ImageUrl="../../images/materialrequest.png" Text="Material Request Form" onclick="addTab('Material Request Form','../Material/MRFMaster.aspx?type=1');" />
                                        </Items>
                                    </telerik:RadPanelItem>
                                </Items>
                            </telerik:RadPanelItem>


                            <telerik:RadPanelItem Text="Incident Management" ImageUrl="../../Images/danger.png">
                                <Items>
                                    <telerik:RadPanelItem ImageUrl="../../Images/liveview.png" Text="Live View" onclick="addTab('Live View (Incident)','../Incident/LiveView.aspx');" />
                                    <telerik:RadPanelItem ImageUrl="../../Images/dashboard.png" Text="Dashboard" onclick="addTab('Dashboard (Incident)','../Incident/Dashboard.aspx');" />

                                </Items>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="Time Attendance" ImageUrl="../../Images/Time_Management.png">
                                <Items>
                                    <telerik:RadPanelItem ImageUrl="../../Images/import.png" Text="Import Raw ClockTime" onclick="addTab('Import Raw ClockTime','../TimeAttendance/ImportRawClockTime.aspx');" />

                                </Items>
                            </telerik:RadPanelItem>
                            <telerik:RadPanelItem Text="Job Allocation" ImageUrl="../../Images/Project_Costing.png">
                                <Items>
                                    <telerik:RadPanelItem ImageUrl="../../Images/dashboard.png" Text="Dashboard" onclick="addTab('Dashboard (Job)','../Reports/JobAllocation/Dashboard.aspx');" />
                                    <telerik:RadPanelItem ImageUrl="../../Images/report.png" Text="Job Deployment Detail" onclick="addTab('Job Deployment Detail','../Reports/JobAllocation/JobDeploymentDetailRpt.aspx');" />

                                </Items>
                            </telerik:RadPanelItem>

                        </Items>
                    </telerik:RadPanelBar>
                    <div id="divNotification" style="width: 100%;">
                        <telerik:RadNotification Skin="MetroTouch" ID="rntfnFrequent" runat="server" ShowTitleMenu="false" Width="100%" TitleIcon="none"
                            Animation="Slide" EnableRoundedCorners="false" EnableShadow="false" CssClass="rntNotification"
                            Position="BottomCenter" ShowCloseButton="true" AutoCloseDelay="100000" OnClientHiding="OnClientHidingNotification" OnClientShowing="OnClientShowingNotification">
                            <ContentTemplate>
                                <div id="divFrequent" runat="server" style="padding-top: 6%; padding-bottom: 0%">
                                    <asp:Label ID="lblNoLinks" runat="server" Visible="false" align="center" />
                                    <div class="TileCell">
                                        <telerik:RadContentTemplateTile runat="server" ID="rtlFrequent1" Visible="false" OnClientClicked="OnClientTileClicked"
                                            Target="_blank" Shape="Wide" Skin="MetroTouch" Height="87px">
                                            <ContentTemplate>
                                                <div style="align-content: center; padding-left: 2px;">
                                                    <br />
                                                    <asp:Label runat="server" ForeColor="White" ID="lblFrequent1" Font-Bold="true" Style="padding-left: 2px"
                                                        Font-Names="segoe ui, Arial,sans-serif"></asp:Label>
                                                </div>
                                            </ContentTemplate>
                                            <PeekTemplate>
                                                <div id="divPeekTemp1" runat="server" style="padding-left: 2px; width: 100%">

                                                    <asp:LinkButton runat="server" ID="lnkbtnFrequent1" CssClass="lblCount" Font-Underline="false"
                                                        Font-Names="segoe ui, Arial,sans-serif" ForeColor="White" OnClientClick="OpenNewTab(this);return false"></asp:LinkButton>

                                                </div>
                                            </PeekTemplate>

                                            <PeekTemplateSettings Animation="Slide" AnimationDuration="800" Easing="easeInOutBack"
                                                ShowInterval="2000" CloseDelay="2000" ShowPeekTemplateOnMouseOver="true" HidePeekTemplateOnMouseOut="true" />
                                        </telerik:RadContentTemplateTile>
                                    </div>
                                    <div class="TileCell">
                                        <telerik:RadContentTemplateTile runat="server" ID="rtlFrequent2" Visible="false" OnClientClicked="OnClientTileClicked"
                                            Target="_blank" Shape="Wide" Skin="MetroTouch" Height="87px">
                                            <ContentTemplate>
                                                <div style="align-content: center; padding-left: 2px">
                                                    <br />
                                                    <asp:Label runat="server" Text="Page1" ForeColor="White" ID="lblFrequent2" Font-Bold="true"
                                                        Font-Names="segoe ui, Arial,sans-serif"></asp:Label>
                                                </div>
                                            </ContentTemplate>
                                            <PeekTemplate>
                                                <div id="divPeekTemp2" runat="server" style="padding-left: 2px; width: 100%">

                                                    <asp:LinkButton runat="server" ID="lnkbtnFrequent2" Font-Underline="false" CssClass="lblCount" Font-Names="segoe ui, Arial,sans-serif" ForeColor="White" OnClientClick="OpenNewTab(this);return false"></asp:LinkButton>

                                                </div>
                                            </PeekTemplate>

                                            <PeekTemplateSettings Animation="Slide" AnimationDuration="800" Easing="easeInOutBack"
                                                ShowInterval="2000" CloseDelay="2000" ShowPeekTemplateOnMouseOver="true" HidePeekTemplateOnMouseOut="true" />
                                        </telerik:RadContentTemplateTile>
                                    </div>
                                    <div class="TileCell">
                                        <telerik:RadContentTemplateTile runat="server" ID="rtlFrequent3" Visible="false" OnClientClicked="OnClientTileClicked"
                                            Target="_blank" Shape="Wide" Skin="MetroTouch" Height="87px">
                                            <ContentTemplate>
                                                <div style="align-content: center; padding-left: 2px">
                                                    <br />
                                                    <asp:Label runat="server" ForeColor="White" ID="lblFrequent3" Font-Bold="true"
                                                        Font-Names="segoe ui, Arial,sans-serif"></asp:Label>
                                                </div>
                                            </ContentTemplate>
                                            <PeekTemplate>
                                                <div id="divPeekTemp3" runat="server" style="padding-left: 2px; width: 100%">

                                                    <asp:LinkButton runat="server" ID="lnkbtnFrequent3" Font-Underline="false" CssClass="lblCount"
                                                        Font-Names="segoe ui, Arial,sans-serif" ForeColor="White" OnClientClick="OpenNewTab(this);return false"></asp:LinkButton>

                                                </div>
                                            </PeekTemplate>

                                            <PeekTemplateSettings Animation="Slide" AnimationDuration="1000" Easing="easeInOutBack"
                                                ShowInterval="2000" CloseDelay="2000" ShowPeekTemplateOnMouseOver="true" HidePeekTemplateOnMouseOut="true" />
                                        </telerik:RadContentTemplateTile>
                                    </div>
                                    <div class="TileCell">
                                        <telerik:RadContentTemplateTile runat="server" ID="rtlFrequent4" Visible="false" OnClientClicked="OnClientTileClicked"
                                            Target="_blank" Shape="Wide" Skin="MetroTouch" Height="87px">
                                            <ContentTemplate>
                                                <div style="align-content: center; padding-left: 2px">
                                                    <br />
                                                    <asp:Label runat="server" Text="Page1" ForeColor="White" ID="lblFrequent4" Font-Bold="true"
                                                        Font-Names="segoe ui, Arial,sans-serif"></asp:Label>
                                                </div>
                                            </ContentTemplate>
                                            <PeekTemplate>
                                                <div id="divPeekTemp4" runat="server" style="padding-left: 2px; width: 100%">

                                                    <asp:LinkButton runat="server" ID="lnkbtnFrequent4" Font-Underline="false" CssClass="lblCount"
                                                        Font-Names="segoe ui, Arial,sans-serif" ForeColor="White" OnClientClick="OpenNewTab(this);return false"></asp:LinkButton>

                                                </div>
                                            </PeekTemplate>

                                            <PeekTemplateSettings Animation="Slide" AnimationDuration="800" Easing="easeInOutBack"
                                                ShowInterval="2000" CloseDelay="2000" ShowPeekTemplateOnMouseOver="true" HidePeekTemplateOnMouseOut="true" />
                                        </telerik:RadContentTemplateTile>
                                    </div>
                                    <asp:Button Height="20px" Width="100%" runat="server" CssClass="rntNotification" Text="   " BorderStyle="None"></asp:Button>
                                </div>
                            </ContentTemplate>
                        </telerik:RadNotification>
                    </div>

                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
