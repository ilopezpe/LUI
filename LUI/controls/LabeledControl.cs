using System.Windows.Forms;

namespace LUI.controls
{
    public partial class LabeledControl<T> : FlowLayoutPanel where T:Control 
    {
        public T Control { get; set; }
        public DisabledRichTextBox Label { get; set; }

        public LabeledControl(T Control) : this (Control, "") {}

        public LabeledControl(T Control, string Text)
        {
            AutoSize = true;
            WrapContents = false;
            this.Control = Control;
            Label = new DisabledRichTextBox
            {
                ScrollBars = RichTextBoxScrollBars.None,
                Text = Text,
                BorderStyle = BorderStyle.None,
                BackColor = BackColor,
                Anchor = AnchorStyles.Left
            };
            Label.Size = TextRenderer.MeasureText(Text, Label.Font);

            Controls.Add(Label);
            Controls.Add(Control);
        }

        public LabeledControl() { }
    }
}
