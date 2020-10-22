using System.Linq;
using System.Web;
using Telerik.Web.UI;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

using System.IO;


namespace astorWork.Controls
{
    /// <summary>
    /// Summary description for TelerikImagePreviewHandler
    /// </summary>
    public class TelerikImagePreviewHandler : AsyncUploadHandler, System.Web.SessionState.IRequiresSessionState
    {

        protected override IAsyncUploadResult Process(UploadedFile file, HttpContext context, IAsyncUploadConfiguration configuration, string tempFileName)
            {
            SampleAsyncUploadResult result = CreateDefaultUploadResult<SampleAsyncUploadResult>(file);
            byte[] yardLayout = null;
            //UploadedFile file = e.File;
            Bitmap imgLayout = new Bitmap(file.InputStream);
            yardLayout = new byte[file.InputStream.Length];
            file.InputStream.Read(yardLayout, 0, (int)file.InputStream.Length);
            context.Session["Image"] = yardLayout;
            return result;

        }
    }
    public class SampleAsyncUploadResult : AsyncUploadResult
    {

        private int imageID;



        public int ImageID
        {

            get { return imageID; }

            set { imageID = value; }

        }

    }
}