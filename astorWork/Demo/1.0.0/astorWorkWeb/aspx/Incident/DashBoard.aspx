<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" 
    CodeBehind="DashBoard.aspx.cs" Inherits="astorWork.DashBoard" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="assets/elements/menus.css" rel="stylesheet" />
    <style>
        iframe {
            display: block;
            width: 100%;
            height: 96vh;
            margin: 0 auto;
            border: 0;
        }
        /*#page-content > .container{
                margin-top:-14px;
                margin-left:-25px;
                margin-right:0px !important;
            }*/
    </style>

    <%--    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.2.621/styles/kendo.common-material.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.2.621/styles/kendo.material.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.2.621/styles/kendo.material.mobile.min.css" />--%>

    <%--   <script src="https://kendo.cdn.telerik.com/2017.2.621/js/jquery.min.js"></script>
    <script src="https://kendo.cdn.telerik.com/2017.2.621/js/kendo.all.min.js"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">


        <div class="demo-section k-content col-md-12">

            <%--    <div id="tabstrip-left">--%>

            <%--   <ul>
                    <li class="k-state-active">Summary
                    </li>
                    <li>ARF/ASR Chart
                    </li>

                </ul>--%>

            <iframe style="padding-top: 10px;" src="https://app.powerbi.com/view?r=eyJrIjoiZmNlMzUxZjctOTgzNy00ODFmLWEzMjQtNTE1NjI2OWJiM2U0IiwidCI6IjMxNTZlOTkxLWE3NzMtNDI5ZS1hNTlhLWRmNmZhYTAyZTQ3NCIsImMiOjEwfQ%3D%3D"></iframe>

            <%--<div>
                    <telerik:RadHtmlChart runat="server" ID="scatterChart" Width="800px" Height="500px">
                        <PlotArea>

                            <CommonTooltipsAppearance Shared="true" Color="White" BackgroundColor="Gray">
                                <SharedTemplate>
                
                 # for (var i = 0; i < points.length; i++) { # 
                <div>#: points[i].series.name# : #: points[i].value #</div>
                # } #
                                </SharedTemplate>
                            </CommonTooltipsAppearance>
                            <Series>
                                <telerik:LineSeries DataFieldY="AFR" Name="AFR">
                                </telerik:LineSeries>
                                <telerik:LineSeries DataFieldY="ASR" AxisName="AdditionalAxis" Name="ASR">
                                    <Appearance>
                                        <FillStyle BackgroundColor="Red" />
                                    </Appearance>

                                </telerik:LineSeries>
                            </Series>
                            <YAxis Color="Orange" Width="3">

                                <TitleAppearance Text="AFR/ASR">
                                </TitleAppearance>
                                <LabelsAppearance Visible="false" DataFormatString="AFR={0}">
                                    <TextStyle Color="Black" />
                                </LabelsAppearance>
                            </YAxis>
                            <AdditionalYAxes>
                                <telerik:AxisY Visible="false" Name="AdditionalAxis" Color="Red" Width="3">

                                    <LabelsAppearance DataFormatString="ASR={0}">
                                        <TextStyle Color="Black" Padding="5px" />
                                    </LabelsAppearance>
                                </telerik:AxisY>
                            </AdditionalYAxes>
                            <XAxis DataLabelsField="Division">
                                <LabelsAppearance RotationAngle="45" DataFormatString="{0}" />

                                <AxisCrossingPoints>
                                    <telerik:AxisCrossingPoint Value="0" />
                                    <telerik:AxisCrossingPoint Value="4" />
                                </AxisCrossingPoints>
                            </XAxis>
                        </PlotArea>
                    </telerik:RadHtmlChart>
                </div>--%>

            <%--  </div>--%>
        </div>
      
        <%-- <script>
            $(document).ready(function () {

                $("#tabstrip-left").kendoTabStrip({
                    tabPosition: "left",
                    animation: { open: { effects: "fadeIn" } }
                });
            });
        </script>--%>
    </div>



</asp:Content>
