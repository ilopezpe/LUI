using System.Windows.Forms;

namespace LUI.controls
{
    public static class ControlExtensions
    {
        public static void AutoResize(this TextBox self)
        {
            var size = TextRenderer.MeasureText(self.Text, self.Font);
            if (self.MinimumSize.Height > size.Height) size.Height = self.MinimumSize.Height;
            if (self.MinimumSize.Width > size.Width) size.Width = self.MinimumSize.Width;
            self.Size = size;
        }
    }
}