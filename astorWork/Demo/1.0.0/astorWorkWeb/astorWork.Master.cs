using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace astorWork
{
    public partial class astorWorkMaster : System.Web.UI.MasterPage
    {
        #region Declration
        string strMessage = string.Empty;
        #endregion

        #region PageMethods
        protected void Page_Load(object sender, EventArgs e)
        {
            int intAsyncTimeOut = 60;
            try
            {
                intAsyncTimeOut = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AsyncTimeOut"]);
            }
            catch (Exception)
            { }

            ScriptManager1.AsyncPostBackTimeout = intAsyncTimeOut;
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "onLoad", "<script>ResetSessionTimeOut();</script>", false);
        }
        #endregion

        #region AlertMessage
        public void DisplayAlertMessage(Enums.Operation eOperation, string operationName = "Record")
        {
            switch (eOperation)
            {
                case Enums.Operation.Delete:
                    strMessage = string.Format("{0} deleted successfully", operationName);
                    break;
                case Enums.Operation.Save:
                    strMessage = string.Format("{0} created successfully", operationName);
                    break;
                case Enums.Operation.Update:
                    strMessage = string.Format("{0} updated successfully", operationName);
                    break;
                default:
                    break;
            }
            rdnMessage.Title = strMessage;
            rdnMessage.TitleIcon = "info";
            rdnMessage.Show();
        }

        public void DisplayAlertMessage(Enums.Operation eOperation, Exception exception, string operationName = "Record")
        {
            switch (eOperation)
            {
                case Enums.Operation.Delete:
                    strMessage = string.Format("Error deleting {0}", operationName.ToLower());
                    break;
                case Enums.Operation.Save:
                    strMessage = string.Format("Error creating {0}", operationName.ToLower());
                    break;
                case Enums.Operation.Update:
                    strMessage = string.Format("Error updating {0}", operationName.ToLower());
                    break;
                default:
                    break;
            }
            rdnMessage.Title = strMessage;
            rdnMessage.TitleIcon = "delete";
            rdnMessage.Show();
        }

        public void DisplayAlertMessage(string message, bool IsException = false, Exception exception = null, bool ShowInPopup = false)
        {
            if (ShowInPopup)
            {
                Ext.Net.X.Msg.Show(new Ext.Net.MessageBoxConfig
                {
                    Title = IsException ? "Error" : "Message",
                    Message = message.Replace("\n", "<br>"),
                    Buttons = Ext.Net.MessageBox.Button.OK,
                    Icon = IsException ? Ext.Net.MessageBox.Icon.ERROR : Ext.Net.MessageBox.Icon.INFO
                });
                return;
            }
            rdnMessage.Title = message.Replace("\n", "<br>");
            rdnMessage.TitleIcon = IsException ? "delete" : "info";
            rdnMessage.Show();
        }

        public void DisplayNotificationMessage(string message, bool IsException = false, Exception exception = null)
        {
            if (exception != null)
            {

                message = string.Format("{0}{1}Error: {2}", message, Environment.NewLine, exception.Message);
            }

            if (IsException)
            {
                rnErrorMessage.Title = message;
                rnErrorMessage.TitleIcon = "delete";
                rnErrorMessage.Show();
            }
            else
            {
                rnSuccessMessage.Title = message;
                rnSuccessMessage.TitleIcon = "info";
                rnSuccessMessage.Show();
            }
        }


        #endregion

    }
}