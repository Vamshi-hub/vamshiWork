<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="astorWork.aspx.Reports.JobAllocation.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hdnEmbedToken" runat="server" />
    <asp:HiddenField ID="hdnEmbedUrl" runat="server" />
    <asp:HiddenField ID="hdnReportId" runat="server" />
    <asp:Label ID="lblError" Font-Size="Large" ForeColor="Red" runat="server" />
    <script type="text/javascript" src="../../../scripts/jquery-3.2.1.js"></script>
    <script type="text/javascript" src="../../../scripts/powerbi.js"></script>
    <div id="divPowerBIWrapper" runat="server" style="width: 100%; height: 100%">
        <div id="divPowerBI" style="width: 100%; height: 100%; overflow: hidden;">
            <script type="text/javascript">
                // Read embed application token from Model
                var accessToken = '<%= hdnEmbedToken.Value %>';

                // Read embed URL from Model
                var embedUrl = '<%= hdnEmbedUrl.Value %>';

                // Read report Id from Model
                var embedReportId = '<%= hdnReportId.Value %>';

                // Get models. models contains enums that can be used.
                var models = window['powerbi-client'].models;

                // Embed configuration used to describe the what and how to embed.
                // This object is used when calling powerbi.embed.
                // This also includes settings and options such as filters.
                // You can find more information at https://github.com/Microsoft/PowerBI-JavaScript/wiki/Embed-Configuration-Details.
                var config = {
                    type: 'report',
                    tokenType: models.TokenType.Embed,
                    accessToken: accessToken,
                    embedUrl: embedUrl,
                    id: embedReportId,
                    permissions: models.Permissions.All,
                    settings: {
                        filterPaneEnabled: true,
                        navContentPaneEnabled: true
                    }
                };

                // Get a reference to the embedded report HTML element
                var reportContainer = $('#divPowerBI')[0];

                // Embed the report and display it within the div container.
                var report = powerbi.embed(reportContainer, config);

                function printReport() {

                    report.print()
                        .catch(error => {
                            alert(error);
                        });
                }
            </script>
        </div>
    </div>

    <button style="display: inline; float: right; width: 100px; height: 40px; font-size: 24px;" onclick="printReport();">Print</button>
</asp:Content>
