<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="astorWork.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AstorWork - Login</title>
     <link rel="icon" href="images/Astoria-Logo.ico" type="image/x-icon" sizes="16x16" />
    <style type="text/css" media="screen">
        body {
            color: #FFFFFF;
            /*background: url('images/login_bg.jpg');*/
            margin: 0;
            padding: 0;
            height: 100%;
        }

        #wrap {
            position: absolute;
            width: 100%;
            height: 100%;
            margin: 0 auto;
        }

        #content {
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
            width: 550px;
            margin-left: -275px;
            -webkit-box-shadow: 9px 6px 56px 6px rgba(0,0,0,1);
            -moz-box-shadow: 9px 6px 56px 6px rgba(0,0,0,1);
            box-shadow: 9px 6px 56px 6px rgba(0,0,0,1);
        }

        #bottomDiv {
            position: absolute;
            bottom: 0px;
            right: 10px;
            width: 100%;
            height: 30px;
            font-family: Segoe UI;
            font-size: 9pt;
            padding: 3px;
        }

            #bottomDiv table {
                width: 100%;
                padding: 5px;
                text-align: right;
            }

        #topDiv table {
            width: 100%;
            padding: 5px;
            text-align: left;
        }

        .button_disable {
            color: Gray;
        }

        img.bg {
            /* Set rules to fill background */
            min-height: 100%;
            min-width: 1024px; /* Set up proportionate scaling */
            width: 100%;
            height: auto; /* Set up positioning */
            position: fixed;
            top: 0;
            left: 0;
        }

        @media screen and (max-width: 1024px) {
            /* Specific to this particular image */ img.bg {
                left: 50%;
                margin-left: -512px; /* 50% */
            }
        }

        .LoginButton {
            margin-top: 10px;
        }

        .TextBox {
            padding: 5px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        var buttonClicked = false;

        function DisableButton(evt) {
            var srcButton = (window.event) ? (event.srcElement) : (evt.currentTarget);
            if (Page_ClientValidate()) {
                document.getElementById('divImage').style.display = "";
                if (!buttonClicked) {
                    srcButton.className = 'button_disable';
                    buttonClicked = true;
                }
                else {
                    srcButton.disabled = true;
                }
            }
        }
    </script>

</head>
<body onfocus="javascript:pageLoad();" onload="breakout_of_frame()">
    <script type="text/javascript">
        //function changeHashOnLoad() {
        //    window.location.href += '#';
        //    setTimeout('changeHashAgain()', '50');
        //}

        //function changeHashAgain() {
        //    window.location.href += '1';
        //}

        //var storedHash = window.location.hash;
        //window.setInterval(function () {
        //    if (window.location.hash != storedHash) {
        //        window.location.hash = storedHash;
        //    }
        //}, 50);
        //window.onload = changeHashOnLoad;
        function pageLoad() {
            
            document.getElementById('loginCtrl_UserName').focus();
        }

        function breakout_of_frame() {
            if (top.location != location) {
                top.location.href = document.location.href;
            }
        }
    </script>

    <div id="wrap">
       
        <asp:Image runat="server" ID="imgLoginBaground" ImageUrl="images/Login_bg.png" CssClass="bg"  />
    </div>
    <div id="wrap">
        <div id="topDiv">
            <table>
                <tr>
                    <td>
                        <asp:Image ID="imgTenantLogo"  runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="content">
            <table height="300px" width="600px" cellspacing="0" cellpadding="0">
                <tr>
                    
                    <td width="270px" align="center" valign="middle" style="background-color: #FFFFFF; border-right: solid 1px #666666;" valign="middle" align="center">
                        <img src="images/AstoriaLogo.jpg" height="65" width="243"><br />
                    </td>
               <td width="330px" style="background-color: #EFEFEF;" valign="middle" align="center">
                    <table valign="middle" height="100%">
                        <tr>
                            <td align="center">
                                <form id="Form1" method="Post" name="form1" runat="Server">
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td align="center">
                                                <asp:Login ID="loginCtrl" runat="server" OnAuthenticate="loginCtrl_Authenticate" TitleText="" Font-Names="Segoe UI,Tahoma" Font-Size="11" ForeColor="#333333"
                                                    TextLayout="TextOnTop" DisplayRememberMe="false">
                                                    <ValidatorTextStyle ForeColor="#FBB54D" />
                                                    <LayoutTemplate>
                                                        <table cellspacing="0" width="100%">
                                                            <tr>
                                                                <td>
                                                                    <table cellpadding="0" width="100%">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="Username"
                                                                                    Font-Size="11" Font-Names="Segoe UI,Tahoma" Wrap="false" HorizontalAlign="Left"
                                                                                    VerticalAlign="Top" ForeColor="#666666"></asp:Label>
                                                                                <br />
                                                                                <asp:TextBox ID="UserName" runat="server"
                                                                                    Width="220px" Font-Names="Segoe UI,Tahoma" Font-Size="16" ForeColor="#666666"
                                                                                    BackColor="#FFFFFF" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1"
                                                                                    Height="44" CssClass="TextBox"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ForeColor="Red"
                                                                                    ErrorMessage="*" Display="Static" ToolTip="User Name is required." ValidationGroup="Login1"></asp:RequiredFieldValidator>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="Password"
                                                                                    Font-Size="11" Font-Names="Segoe UI,Tahoma" Wrap="false" HorizontalAlign="Left"
                                                                                    VerticalAlign="Top" ForeColor="#666666"></asp:Label>
                                                                                <br />
                                                                                <asp:TextBox ID="Password" runat="server" TextMode="Password"
                                                                                    Width="220px" Font-Names="Segoe UI,Tahoma" Font-Size="16" ForeColor="#666666"
                                                                                    BackColor="#FFFFFF" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1"
                                                                                    Height="44" CssClass="TextBox"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ForeColor="Red"
                                                                                    ErrorMessage="*" Display="Static" ToolTip="Password is required." ValidationGroup="Login1"></asp:RequiredFieldValidator>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left" colspan="2" style="color: Red; padding-left: 18px; font-size: small">
                                                                                <br />
                                                                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                    <td style="text-align: right; padding-right: 9px;">
                                                                                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="Login1"
                                                                                    OnClientClick="DisableButton(event);" Font-Bold="true"
                                                                                    BackColor="#0a90ba" Font-Names="Segoe UI,Tahoma" Font-Size="12"
                                                                                    ForeColor="#FFFFFF" Width="80px" Height="40px" BorderColor="#000000" BorderStyle="Ridge"
                                                                                    BorderWidth="1" CssClass="LoginButton"></asp:Button>
                                                                    </td>
                                                                    <td>
                                                                         <div id="divImage" style="display:none">
                                                                        <img id="imgLogin"  alt="Login progress bar" src="images/ProgressBar.gif"/>
                                                                             </div>
                                                                    </td>
                                                                </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </LayoutTemplate>
                                                </asp:Login>
                                            </td>
                                        </tr>
                                       <tr>
                                            <td align="left" style="color: Red; padding-left: 18px; font-size: 11px">
                                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                            </td>
                                        </tr>
                                    </table>
                                </form>
                            </td>
                        </tr>
                    </table>
                </td>
                </tr>
            </table>
        </div>
        <div id="bottomDiv">
            <table>
                <tr>
                    <td>
                        <span style="font-family: Segoe UI; font-size: 12px;"><a style="color: White;" href="http://www.astoriasolutions.com"
                            target="_blank">Astoria Solutions Pte Ltd</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;Support:
                            helpdesk@astoriasolutions.com&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;Supported in Edge,
                            Internet Explorer 8 & above and Google Chrome
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</body>
</html>
