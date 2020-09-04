using System.ComponentModel;
using System.Windows.Forms.Design;

namespace LUI.controls.designer
{
    class ObjectCommandPanelDesigner : ParentControlDesigner
    {
        public override void Initialize(IComponent Component)
        {
            base.Initialize(Component);
            if (Control is ObjectCommandPanel) EnableDesignMode(((ObjectCommandPanel)Control).Flow, "Flow");
        }
    }
}