using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LUI.controls
{
    public class DisabledRichTextBox : RichTextBox
    {
        const int WM_SETFOCUS = 0x07;
        const int WM_ENABLE = 0x0A;
        const int WM_SETCURSOR = 0x20;

        public DisabledRichTextBox()
        {
            SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
            TextChanged += HandleTextChanged;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        [Category("Appearance")]
        public override bool AutoSize { get; set; }

        protected override void WndProc(ref Message m)
        {
            // Originally included WM_ENABLE.
            // Removed to allow enabling/disabling to change text color.
            // WM_SETFOCUS and WM_SETCURSOR appear sufficient for use as label.
            if (!(m.Msg == WM_SETFOCUS || m.Msg == WM_SETCURSOR))
                base.WndProc(ref m);
        }

        Size GetAutoSize()
        {
            return TextRenderer.MeasureText(Text, Font);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            return GetAutoSize();
        }

        void ResizeForAutoSize()
        {
            if (AutoSize) SetBoundsCore(Left, Top, Width, Height, BoundsSpecified.Size);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            //  Only when the size is affected...
            if (AutoSize && (specified & BoundsSpecified.Size) != 0)
            {
                var size = GetAutoSize();

                width = size.Width;
                height = size.Height;
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        void HandleTextChanged(object sender, EventArgs e)
        {
            Size = GetAutoSize();
        }
    }
}