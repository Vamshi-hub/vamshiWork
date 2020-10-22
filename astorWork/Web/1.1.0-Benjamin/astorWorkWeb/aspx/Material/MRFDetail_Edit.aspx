<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/astorWork.Master" ValidateRequest="false" CodeBehind="MRFDetail_Edit.aspx.cs" Inherits="astorWork.MRFDetail_Edit" %>

<%@ MasterType VirtualPath="~/astorWork.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function OnClientEntryAdding(sender, args) {
                var path = args.get_entry().get_fullPath();
                var s = path.split("/");
                if (s.length == 1) {
                    // if the selected entry is parent then it will not allow to select
                    args.set_cancel(true);
                }
                else {
                    // close the dropdown explicitly
                    sender.closeDropDown();
                }
            }
            function openConfirmationWindow() {
                uxEditForm.set_title();
                uxEditForm.show();
            }

            function refreshGrid(Operation) {
                document.getElementById("<%= hdnOperation.ClientID %>").value = Operation;
                <%--$get("<%= rbtnRefreshGrid.ClientID %>").click();--%>
            }

            function RequestStart() {
                var loadingPanel = document.getElementById("<%= ralpMRFDetail.ClientID %>");
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

            function searchBox_ClientLoad(sender) {
                sender._requestDelay = 800;
            }

            function urltadclose(closeurl, refreshurl) {
                //parent.RefreshTab(refreshurl);
                parent.RemoveTab(closeurl);
                //var tab = tabstrip.findTabByValue(closeurl);
                //open(location, '_self').close();
                
            }
            function RowSelecting(sender, eventArgs) {                
                var uxDisplayName = document.getElementById('<%= uxDisplayName.ClientID%>');
                var uxURL = document.getElementById('<%= uxURL.ClientID%>');
                var DisplayName = uxDisplayName.value + ": " + eventArgs.getDataKeyValue("MaterialNo");
                var URL = uxURL.value + "?MaterialNo=" + eventArgs.getDataKeyValue("MaterialNo");
                parent.AddNewTabs(DisplayName, URL);
            }
        </script>

        
    </telerik:RadCodeBlock>

    <telerik:RadAjaxManager ID="ramMRFDetail" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rapMRFDetail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="uxRadWizard" LoadingPanelID="ralpMRFDetail" />
                    <telerik:AjaxUpdatedControl ControlID="uxAddWindow" LoadingPanelID="ralpMRFDetail" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnRequestStart="RequestStart" />
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel runat="server" ID="ralpMRFDetail"></telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxPanel runat="server" ID="rapMRFDetail">
        <asp:Panel ID="pnlMRFDetail" runat="server">
            <asp:HiddenField runat="server" ID="hdnOperation" />
            <div class="layout1">
                <telerik:RadWizard ID="uxRadWizard" runat="server" SkinID="RadWizardSkin" OnNextButtonClick="uxRadWizard_NextButtonClick" OnFinishButtonClick="uxRadWizard_FinishButtonClick" OnNavigationBarButtonClick="uxRadWizard_NavigationBarButtonClick" CausesValidation="true">
                    <WizardSteps>
                        <telerik:RadWizardStep ID="uxHeader" Title="New Request" StepType="Start" ValidationGroup="vgMRFMaster" runat="server" CausesValidation="true" SpriteCssClass="vgMRFMaster">
                            <fieldset class="fieldsettemplate" style="height: 550px">
                                <table style="width: 95%; margin-left: 20px;">
                                    <tr>
                                        <td>
                                            <telerik:RadLabel runat="server" Text="PP09 No" Font-Size="Small" />
                                        </td>
                                        <td>
                                            <telerik:RadLabel runat="server" Font-Size="Small" Text="Date of Order"></telerik:RadLabel>
                                        </td>
                                        <td>
                                            <telerik:RadLabel runat="server" Font-Size="Small" Text="Status"></telerik:RadLabel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <telerik:RadTextBox ID="uxMRFNo" runat="server" Font-Size="Medium" Enabled="false" EmptyMessage="Auto Generate No" />

                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="uxMRFDate" ValidationGroup="vgMRFMaster" runat="server" Skin="MetroTouch"></telerik:RadDatePicker>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxMRFDate"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <telerik:RadLabel ID="uxStatus" runat="server" Font-Size="Medium" />
                                        </td>
                                    </tr>
                                    <tr>
                                       
                                        <td style="padding-top: 50px">
                                            <telerik:RadLabel runat="server" Text="Vendor" Font-Size="Small" />
                                        </td>
                                        <td style="padding-top: 50px">
                                            <telerik:RadLabel runat="server" Font-Size="Small" Text="Attention"></telerik:RadLabel>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td>
                                            <telerik:RadComboBox ID="uxVendor" runat="server" SkinID="RadComboBoxSkin" DataValueField="Code" ValidationGroup="vgMRFMaster" EmptyMessage="Select Vendor" Width="250px" AutoPostBack="true"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxVendor"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="uxAttention" CheckBoxes="true" EnableCheckAllItemsCheckBox="true"  Width="350px"  ValidationGroup="vgMRFMaster" runat="server" SkinID="RadComboBoxSkin">
                                            </telerik:RadComboBox>
                                           <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxAttention"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                         <td style="padding-top: 50px">
                                            <telerik:RadLabel runat="server" Text="Project" Font-Size="Small" />
                                        </td>
                                         <td style="padding-top: 50px">
                                            <telerik:RadLabel runat="server" Text="Component" Font-Size="Small" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <telerik:RadComboBox ID="uxProject" runat="server" SkinID="RadComboBoxSkin" DataValueField="Code" ValidationGroup="vgMRFMaster" EmptyMessage="Select Project" Width="250px" AutoPostBack="true"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxProject"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="uxMaterialType" CheckBoxes="true" EnableCheckAllItemsCheckBox="true"  Width="350px"  ValidationGroup="vgMRFMaster" runat="server" SkinID="RadComboBoxSkin">
                                            </telerik:RadComboBox>
                                           <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxMaterialType"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 50px">
                                            <telerik:RadLabel runat="server" Text="Block" Font-Size="Small" />
                                        </td>
                                        <td style="padding-top: 50px">
                                            <telerik:RadLabel runat="server" Text="Level" Font-Size="Small" />
                                        </td>
                                        <td style="padding-top: 50px">
                                            <telerik:RadLabel ID="RadLabel6" runat="server" Font-Size="Small" Text="Zone / Grid Line"></telerik:RadLabel>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td>
                                            <telerik:RadComboBox ID="uxBlock" runat="server" SkinID="RadComboBoxSkin" DataValueField="Code" ValidationGroup="vgMRFMaster" EmptyMessage="Select Block" Width="250px" AutoPostBack="true" OnSelectedIndexChanged="Location_SelectedIndexChanged" CausesValidation="false"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxBlock"></asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <telerik:RadComboBox ID="uxLevel" runat="server" SkinID="RadComboBoxSkin" DataValueField="Code" ValidationGroup="vgMRFMaster" EmptyMessage="Select Level" Width="250px" AutoPostBack="true" OnSelectedIndexChanged="Location_SelectedIndexChanged" CausesValidation="false"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxLevel"></asp:RequiredFieldValidator>
                                        </td>
                                        <td >
                                            <telerik:RadComboBox ID="uxZone" runat="server" SkinID="RadComboBoxSkin" DataValueField="Code" ValidationGroup="vgMRFMaster" EmptyMessage="Zone / Grid Line" Width="250px" AutoPostBack="true" OnSelectedIndexChanged="Location_SelectedIndexChanged" CausesValidation="false"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxZone"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td style="padding-top: 50px;" colspan="3">
                                            <telerik:RadLabel runat="server" Text="The delivery is planned based on projected slab casting on" SkinID ="RadLabelSkin" Font-Size="Small" />
                                        </td>  
                                    </tr>
                                    <tr>
                                         <td colspan="3">
                                            <telerik:RadDatePicker ID="uxPlannedCastingDate" ValidationGroup="vgMRFMaster" runat="server" Skin="MetroTouch"></telerik:RadDatePicker>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="*" ForeColor="Red" ValidationGroup="vgMRFMaster" Display="Dynamic" ControlToValidate="uxPlannedCastingDate"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </telerik:RadWizardStep>

                        <telerik:RadWizardStep ID="uxDetail" Title="Select Precast Component" runat="server" StepType="Finish" CssClass="rwzStep">
                            <div class="layout" style="width:99%">
                                <telerik:RadGrid ID="uxMRFDetail" runat="server" SkinID="RadGridSkin" AllowFilteringByColumn="true" OnItemCommand="uxMRFDetail_ItemCommand"
                                    OnNeedDataSource="uxMRFDetail_NeedDataSource" OnItemDataBound="uxMRFDetail_ItemDataBound">
                                    <MasterTableView Width="100%" CommandItemDisplay="Top" AllowFilteringByColumn="false" EditMode="PopUp" DataKeyNames="MaterialNo,Status,MRFNo" ClientDataKeyNames="MaterialNo">                                        
                                        <CommandItemSettings ShowRefreshButton="true" ShowAddNewRecordButton="false" />
                                        <ColumnGroups>
                                            <telerik:GridColumnGroup Name="Location"  HeaderText="Location" HeaderStyle-HorizontalAlign="Center"></telerik:GridColumnGroup>
                                        </ColumnGroups>                                       
                                        <Columns>
                                            <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" ButtonType="ImageButton"/>
                                           <telerik:GridBoundColumn DataField="MarkingNo" HeaderText="Marking No"  />                      
                                            <telerik:GridBoundColumn DataField="Project" HeaderText="Project"  />                      
                                            <telerik:GridBoundColumn DataField="MaterialType" HeaderText="Component"  />                        
                                            <telerik:GridBoundColumn DataField="Block" HeaderText="Block" ColumnGroupName="Location"/>
                                            <telerik:GridBoundColumn DataField="Level" HeaderText="Level" ColumnGroupName="Location" />
                                            <telerik:GridBoundColumn DataField="Zone" HeaderText="Zone" ColumnGroupName="Location" />
                                            <telerik:GridDateTimeColumn DataField="DeliveryDate" HeaderText="Expected Delivery" DataFormatString="{0:d}" ItemStyle-Width="10%"/>
                                            <telerik:GridBoundColumn DataField="DeliveryRemarks" HeaderText="Remarks"/>
                                            <telerik:GridImageColumn DataType="System.String" DataImageUrlFields="Produced" DataImageUrlFormatString="~/Images/Circle/{0}.png" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" HeaderText="Produced" ImageAlign="Middle" ImageHeight="40px" HeaderStyle-HorizontalAlign="Center"></telerik:GridImageColumn>
                                            <telerik:GridImageColumn DataType="System.String" DataImageUrlFields="Delivered" DataImageUrlFormatString="~/Images/Circle/{0}.png" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" HeaderText="Delivered" ImageAlign="Middle" ImageHeight="40px" HeaderStyle-HorizontalAlign="Center"></telerik:GridImageColumn>
                                            <telerik:GridImageColumn DataType="System.String" DataImageUrlFields="Installed" DataImageUrlFormatString="~/Images/Circle/{0}.png" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" HeaderText="Installed" ImageAlign="Middle" ImageHeight="40px" HeaderStyle-HorizontalAlign="Center"></telerik:GridImageColumn>
                                        </Columns>
                                        <EditFormSettings EditFormType="Template" PopUpSettings-Modal="true" PopUpSettings-Width="1110px">
                                            <FormTemplate>
                                                <asp:Panel runat="server" CssClass="paneltemplate" Width="1100px">
                                                <div class="divtemplate" style="line-height:75%"" >
                                                    <fieldset class="fieldsettemplate">
                                                        <table width="1000px">                                                        
                                                        <tr>
                                                        <td>
                                                        <div class="layout">
                                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                               <tr>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel1" runat="server" Text="Marking No" Font-Size="Small" />
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel7" runat="server" Font-Size="Small" Text="Component"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel4" runat="server" Font-Size="Small" Text="Drawing No"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel2" runat="server" Font-Size="Small" Text="Drawing Issue Date"></telerik:RadLabel>
                                                                </td>
                            
                                                                <td></td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <telerik:RadLabel ID="uxMarkingNo" Text='<%# Bind("MarkingNo") %>' runat="server" Font-Size="X-Large" SkinID="RadLabelSkinReadOnly" ForeColor="#3498DB"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="uxMaterialType" Text='<%# Bind("MaterialType") %>'  runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="uxDrawingNo" Text='<%# Bind("DrawingNo") %>' runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="uxDrawingIssueDate" Text='<%# DateTime.Parse(Eval("DrawingIssueDate").ToString()).ToShortDateString() %>'  runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td colspan="4">
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel3" runat="server" Font-Size="Small" Text="Project"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel10" runat="server" Font-Size="Small" Text="Block" />
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel5" runat="server" Font-Size="Small" Text="Level"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="RadLabel12" runat="server" Font-Size="Small" Text="Zone" />
                                                                </td>
                            
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <telerik:RadLabel ID="uxProject" Text='<%# Bind("Project") %>' runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="ulBlock" Text='<%# Bind("Block") %>' runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly" ></telerik:RadLabel>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadLabel ID="ulLevel" Text='<%# Bind("Level") %>' runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                </td>
                                                                <td valign="top">
                                                                    <telerik:RadLabel ID="ulZone" Text='<%# Bind("Zone") %>' runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="4">
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                            
                                                                    <td>
                                                                        <telerik:RadLabel ID="RadLabel8" runat="server" Font-Size="Small" Text="Component Size"></telerik:RadLabel>
                                                                    </td>
                                                                    <td>
                                                                        <telerik:RadLabel ID="RadLabel6" runat="server" Font-Size="Small" Text="Vendor"></telerik:RadLabel>
                                                                    </td>
                                                                    <td>
                                                                        <telerik:RadLabel ID="RadLabel11" runat="server" Font-Size="Small" Text="MRF No"></telerik:RadLabel>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                            
                                                                    <td>
                                                                        <telerik:RadLabel ID="uxMaterialSize" Text='<%# Bind("MaterialSize") %>'  runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                    </td>
                                                                    <td>
                                                                        <telerik:RadLabel ID="uxContractor" Text='<%# Bind("Contractor") %>'  runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                    </td>
                                                                     <td>
                                                                        <telerik:RadLabel ID="uxMRFNo" Text='<%# Bind("MRFNo") %>'  runat="server" Font-Size="Medium" SkinID="RadLabelSkinReadOnly"></telerik:RadLabel>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="4">
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                            
                                                                    <td valign="top">
                                                                        <telerik:RadLabel ID="RadLabel9" runat="server" Font-Size="Small" Text="Delivery Date"></telerik:RadLabel>
                                                                    </td>
                                                                    <td valign="top">
                                                                        <telerik:RadLabel ID="RadLabel13" runat="server" Font-Size="Small" Text="Delivery Remarks"></telerik:RadLabel>
                                                                    </td>                                                                    
                                                                </tr>                                                                
                                                                <tr>
                            
                                                                    <td valign="top">
                                                                        <telerik:RadDatePicker ID="uxDeliveryDate" SkinID="RadDatePickerSkin" SelectedDate='<%# Bind("DeliveryDate") %>' runat="server" Font-Size="Medium" ></telerik:RadDatePicker>
                                                                    </td>
                                                                    <td valign="top" colspan="3">
                                                                        <telerik:RadTextBox ID="uxDeliveryRemarks" SkinID="RadTextBoxSkin"  Text='<%# Bind("DeliveryRemarks") %>' runat="server" TextMode="MultiLine" Font-Size="Medium" Width="450px" Height="100px"></telerik:RadTextBox>
                                                                    </td>                                                                    
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="4">
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        </td>
                                                        </tr>
                                                        <tr>
                                                        <td colspan="2" align="center" style="padding-top: 20px">
                                                                <telerik:RadButton ID="uxInsertUpdate" SkinID="RadButtonSkin" Text='<%# (Container is GridEditFormInsertItem) ? "Insert" : "Update" %>'
                                                                    runat="server" CommandName='<%# (Container is GridEditFormInsertItem) ? "PerformInsert" : "Update" %>' ValidationGroup="ObjectValidation" CausesValidation="true">
                                                                    <Icon PrimaryIconCssClass="rbSave" />
                                                                </telerik:RadButton>
                                                                &nbsp;
                                                                <telerik:RadButton ID="uxCancel" Text="Cancel" runat="server" SkinID="RadButtonSkin" CausesValidation="False"
                                                                    CommandName="Cancel">
                                                                    <Icon PrimaryIconCssClass="rbCancel" />
                                                                </telerik:RadButton>
                                                        </td>
                                                        </tr>
                                                            <tr>
                                                                <td colspan="4">
                                                                    <br />
                                                                </td>
                                                            </tr>        
                                                    </table>                                
                                                    </fieldset>
                                                </div>
                                                </asp:Panel>
                                            </FormTemplate>
                                        </EditFormSettings>
                                    </MasterTableView>
                                    <ClientSettings>                
                                        <Selecting AllowRowSelect="true" />
                                        <ClientEvents OnRowSelecting="RowSelecting" />
                                    </ClientSettings>
                                </telerik:RadGrid>        
                                <asp:HiddenField ID="uxDisplayName" runat="server" />
                                <asp:HiddenField ID="uxURL" runat="server" />                        
                            </div>
                        </telerik:RadWizardStep>
                    </WizardSteps>
                </telerik:RadWizard>
                
            </div>
        </asp:Panel>   
    </telerik:RadAjaxPanel>
</asp:Content>
