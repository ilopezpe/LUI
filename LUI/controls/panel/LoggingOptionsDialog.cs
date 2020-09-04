using log4net;
using log4net.Repository.Hierarchy;
using LUI.config;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LUI.controls
{
    public partial class LoggingOptionsDialog : LuiOptionsDialog
    {
        LabeledControl<ComboBox> LogLevel;

        public LoggingOptionsDialog(Size Size, bool Visibility)
        {
            InitializeComponent();
            this.Size = Size;
            Visible = Visibility;
            Init();
        }

        public LoggingOptionsDialog(Size Size) : this(Size, true)
        {
        }

        public LoggingOptionsDialog()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            SuspendLayout();

            LogLevel = new LabeledControl<ComboBox>(new ComboBox(), "Log Level");
            LogLevel.Control.DropDownStyle = ComboBoxStyle.DropDownList;

            var h = (Hierarchy)LogManager.GetRepository();
            foreach (var l in h.LevelMap.AllLevels) LogLevel.Control.Items.Add(l.DisplayName);

            LogLevel.Control.SelectedIndexChanged += (s, e) => OnOptionsChanged(s, e);

            Controls.Add(LogLevel);

            ConfigChanged += (s, e) => HandleConfigChanged(s, e);

            ResumeLayout(false);
        }

        public override void CopyConfigState(LuiConfig config)
        {
            LogLevel.Control.SelectedItem = config.LogLevel;
        }

        public override void CopyConfigState()
        {
            CopyConfigState(Config);
        }

        public override void HandleApply(object sender, EventArgs e)
        {
            var LevelName = (string)LogLevel.Control.SelectedItem;
            Config.SetLogLevel(LevelName);
        }

        public override void HandleConfigChanged(object sender, EventArgs e)
        {
            MatchConfig(Config);
        }
    }
}