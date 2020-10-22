using Xamarin.Forms;

namespace astorWorkMobile.Shared.Views
{
    public class CustomEditor : Editor
    {
        public CustomEditor()
        {
           // this.AutoSize = EditorAutoSizeOption.TextChanges;
            this.TextChanged += (sender, e) =>
            {
                this.InvalidateMeasure();
            };
        }

        private void CustomEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.InvalidateMeasure();
        }
    }
}
