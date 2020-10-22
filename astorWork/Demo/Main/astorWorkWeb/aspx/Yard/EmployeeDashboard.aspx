<%@ Page Title="" Language="C#" MasterPageFile="~/astorWork.Master" AutoEventWireup="true"
    CodeBehind="EmployeeDashboard.aspx.cs" Inherits="astorWork.EmployeeDashboard" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="assets/elements/menus.css" rel="stylesheet" />
    <style>
        iframe {
            display: block;
            width: 100%;
            height: 100%;
            margin: 0 auto;
            border: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
            <iframe src="https://app.powerbi.com/view?r=eyJrIjoiZmI3ODg3Y2MtODMxMi00Y2MxLWE1MDgtYmI4YzFhNTMwMTg2IiwidCI6IjMxNTZlOTkxLWE3NzMtNDI5ZS1hNTlhLWRmNmZhYTAyZTQ3NCIsImMiOjEwfQ%3D%3D"></iframe>



</asp:Content>
