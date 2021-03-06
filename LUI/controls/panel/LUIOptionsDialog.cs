﻿using Extensions;
using LUI.config;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LUI.controls
{
    /// <summary>
    ///     Base class for controls used as options dialogs.
    /// </summary>
    public abstract class LuiOptionsDialog : UserControl
    {
        LuiConfig _Config;

        public LuiOptionsDialog()
        {
            AutoScaleMode = AutoScaleMode.Inherit;
        }

        public LuiOptionsDialog(Size Size, bool Visibility) : this()
        {
            this.Size = Size;
            Visible = Visibility;
        }

        public LuiOptionsDialog(Size Size)
            : this(Size, true)
        {
        }

        /// <summary>
        ///     Gets or sets the LuiConfig object used to read and write configuration options.
        ///     Set triggers ConfigChanged.
        /// </summary>
        public virtual LuiConfig Config
        {
            get => _Config;
            set
            {
                _Config = value;
                ConfigChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Indicates backing config reference changed.
        /// </summary>
        public event EventHandler ConfigChanged;

        /// <summary>
        ///     Indicates entire dialog content updated to match a config. May lead to OptionsChanged.
        /// </summary>
        public event EventHandler ConfigMatched;

        /// <summary>
        ///     // Indicates piece of dialog content changed.
        /// </summary>
        public event EventHandler OptionsChanged;

        /// <summary>
        ///     Copies dialog content from a LuiConfig and triggers ConfigMatched.
        /// </summary>
        /// <param name="config"></param>
        public void MatchConfig(LuiConfig config)
        {
            CopyConfigState(config);
            ConfigMatched?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Safely triggers OptionsChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnOptionsChanged(object sender, EventArgs e)
        {
            OptionsChanged.Raise(this, e);
        }

        /// <summary>
        ///     Copy dialog content from the passed LuiConfig.
        /// </summary>
        /// <param name="config"></param>
        public abstract void CopyConfigState(LuiConfig config);

        /// <summary>
        ///     Copy dialog content from bound LuiConfig.
        /// </summary>
        public abstract void CopyConfigState();

        /// <summary>
        ///     Defines action taken when LuiConfig used to read/write options is set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void HandleConfigChanged(object sender, EventArgs e);

        /// <summary>
        ///     Defines action taken when dialog options are applied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void HandleApply(object sender, EventArgs e);
    }
}