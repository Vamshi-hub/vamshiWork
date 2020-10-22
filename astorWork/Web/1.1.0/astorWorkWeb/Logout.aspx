<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="astorSafeWeb.Logout" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AstorWork - Logout</title>
     <style type="text/css">
        body
        {
            color: #333333; 
            margin: 0;
            padding: 0;
            height: 100%;
            font-family: Segoe UI;
            font-size: 13pt;
        }
        #wrap
        {
            position: absolute;
            width: 100%;
            height: 100%;
            margin: 0 auto;
        }
        #content
        {
            text-align: center;
            position: absolute;
            border: solid;
            border-width: 1px;
            border-color: #333333;
            margin: 0px;
            top: 50%;
            left: 50%;
            height: 300px;
            margin-top: -150px;
            width: 380px;
            margin-left: -190px;
            -webkit-box-shadow: 9px 6px 56px 6px rgba(0,0,0,1);
            -moz-box-shadow: 9px 6px 56px 6px rgba(0,0,0,1);
            box-shadow: 9px 6px 56px 6px rgba(0,0,0,1);
            background-color: #FFFFFF;
        }
        #bottomDiv
        {
            position: absolute;
            bottom: 0px;
            right: 10px;
            width: 100%;
            height: 30px;
            font-family: Segoe UI;
            font-size: 9pt;
            padding: 3px;
            color:white;
        }
        #bottomDiv table
        {
            width: 100%;
            padding: 5px;
            text-align: right;
        }
        img.bg
        {
            /* Set rules to fill background */
            min-height: 100%;
            min-width: 1024px; /* Set up proportionate scaling */
            width: 100%;
            height: auto; /* Set up positioning */
            position: fixed;
            top: 0;
            left: 0;
        }
        @media screen and (max-width: 1024px)
        {
            /* Specific to this particular image */    img.bg
            {
                left: 50%;
                margin-left: -512px; /* 50% */
            }
        }
        .LoginButton
        {
            margin-top: 10px;
        }
        .TextBox
        {
            padding: 5px;
        }
    </style>
</head>
<body>
    
    <asp:Image runat="server" ID="imgLoginBaground" ImageUrl="~/images/Login_bg.png" CssClass="bg"  />
     <div id="wrap">
    <div id="content">
            <table width="100%" height="100%" cellspacing="0" cellpadding="20">
                <tr>
                    <td align="center" valign="middle">
                        <span style=" font-family: Segoe UI Light; font-size: 34pt;">Log out Successful</span>
                        <br />
                        <br />
                        Click
                        <asp:HyperLink ID="hplLogin" runat="server" NavigateUrl="~/Login.aspx" Target="_top">here</asp:HyperLink>
                        to login again.
                    </td>
                </tr>
            </table>
        </div>
</div>

    <div id="bottomDiv">
        <table>
            <tr>
                <td>
                     <span style="font-family: Segoe UI; font-size: 12px;"><a style="color: White;" href="http://www.astoriasolutions.com"
                            target="_blank">Astoria Solutions Pte Ltd</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;Support:
                            helpdesk@astoriasolutions.com&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;Supported in
                            Internet Explorer 8 & above and Google Chrome
                        </span>
                </td>
            </tr>
        </table>
    </div>
  
</body>
</html>
