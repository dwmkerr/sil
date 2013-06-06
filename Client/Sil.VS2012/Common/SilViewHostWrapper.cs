using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;

namespace Sil.VSCommon
{
    /// <summary>
    /// A host for the main Sil interface.
    /// </summary>
    public partial class SilViewHostWrapper : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SilViewHostWrapper"/> class.
        /// </summary>
        public SilViewHostWrapper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the host window.
        /// </summary>
        /// <value>
        /// The host window.
        /// </value>
        public Window2 HostWindow { get; set; }

        /// <summary>
        /// Gets the sil view host.
        /// </summary>
        /// <value>
        /// The sil view host.
        /// </value>
        public SilUI.SilViewHost SilViewHost { get { return silViewHost1; } }

        /// <summary>
        /// Initialises the parent window.
        /// </summary>
        /// <param name="window">The window.</param>
        public void InitialiseParentWindow(Window2 window)
        {
            HostWindow = window;
            SilViewHost.OnEntitySelected += SilViewHost_OnEntitySelected;
        }

        void SilViewHost_OnEntitySelected(object sender, SilUI.DisassembledEntityEventArgs eventArgs)
        {
            HostWindow.Caption = eventArgs.Entity != null ? "Sil - " + eventArgs.Entity.ShortName : @"Sil";
        }
    }
}
