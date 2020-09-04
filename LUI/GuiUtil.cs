using System.Windows.Forms;

namespace LUI
{
    public static class GuiUtil
    {
        public static string SimpleFileNameDialog(string Filter)
        {
            var ofd = new OpenFileDialog
            {
                Filter = Filter
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileName;
            return "";
        }

        public static string SimpleFolderNameDialog()
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
                return fbd.SelectedPath;
            return "";
        }
    }
}