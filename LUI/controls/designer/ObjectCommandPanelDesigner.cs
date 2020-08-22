using System.Windows.Forms.Design;

namespace LUI.controls.designer
{
    class ObjectCommandPanelDesigner : ParentControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent Component)
        {
            base.Initialize(Component);
            if (Control is ObjectCommandPanel)
            {
                EnableDesignMode(((ObjectCommandPanel)this.Control).Flow, "Flow");
            }
        }
    }
}
