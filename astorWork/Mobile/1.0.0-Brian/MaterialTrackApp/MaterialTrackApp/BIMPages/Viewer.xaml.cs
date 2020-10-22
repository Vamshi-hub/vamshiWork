using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp.BIMPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Viewer : ContentPage
    {
        public Viewer()
        {
            InitializeComponent();
            var assembly = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(Viewer)).Assembly;

            using (var stream = assembly.GetManifestResourceStream("MaterialTrackApp.BIMPages.viewer.html"))
            {
                string text = "";
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Replace("$ACCESS_TOKEN$", "eyJhbGciOiJIUzI1NiIsImtpZCI6Imp3dF9zeW1tZXRyaWNfa2V5In0.eyJjbGllbnRfaWQiOiJZOVFhbEdnWW5ZRUFMNWljanI5dDh6VEhDd1JYcUdHYSIsImV4cCI6MTUyMDMyODY0Nywic2NvcGUiOlsiZGF0YTpyZWFkIiwiZGF0YTp3cml0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6cmVhZCIsInZpZXdhYmxlczpyZWFkIl0sImF1ZCI6Imh0dHBzOi8vYXV0b2Rlc2suY29tL2F1ZC9qd3RleHA2MCIsImp0aSI6IkRtRUhNQVdxS2xNZkpFY1QzZWsxZ3BqQWl5MENybjV5SDd0SzNSQ1lnQjkwYjYxTUszb2Q5WG41M2FIbzhkdGkifQ.9SFLnLih46zXMs4fRiOuQQL6LndB-OedHIFbzqrJ9zo");
                    text = text.Replace("$DOCUMENT_ID$", "dXJuOmFkc2sub2JqZWN0czpvcy5vYmplY3Q6YXN0b3J3b3JrLWNvbnZlcnQvc291cmNlXzIwMTcxMjI5LnJ2dA");
                    var html = new HtmlWebViewSource
                    {
                        Html = text
                    };
                    webBIM.Source = html;
                }
            }
        }
    }
}