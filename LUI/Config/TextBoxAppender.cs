using log4net.Appender;
using log4net.Core;
using System.Windows.Forms;

namespace LUI.config
{
    class TextBoxAppender : AppenderSkeleton
    {
        public TextBox AppenderTextBox { get; set; }

        public string FormName { get; set; }
        public string TextBoxName { get; set; }

        Control FindControlRecursive(Control root, string textBoxName)
        {
            if (root.Name == textBoxName) return root;
            foreach (Control c in root.Controls)
            {
                var t = FindControlRecursive(c, textBoxName);
                if (t != null) return t;
            }

            return null;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (AppenderTextBox == null)
            {
                if (string.IsNullOrEmpty(FormName) ||
                    string.IsNullOrEmpty(TextBoxName))
                    return;

                var form = Application.OpenForms[FormName];
                if (form == null)
                    return;

                AppenderTextBox = (TextBox)FindControlRecursive(form, TextBoxName);
                if (AppenderTextBox == null)
                    return;

                form.FormClosing += (s, e) => AppenderTextBox = null;
            }

            AppenderTextBox.BeginInvoke((MethodInvoker)delegate
           {
               AppenderTextBox.AppendText(RenderLoggingEvent(loggingEvent));
           });
        }
    }
}