using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using SilAPI;

namespace SilUI
{
    /// <summary>
    /// The Sil View Host is a simple WinForms control that
    /// hosts the main WPF Sil view. Most commonly, this will
    /// be used from the Visual Studio addins.
    /// </summary>
    public partial class SilViewHost : UserControl
    {
        public SilViewHost()
        {
            InitializeComponent();

            //  Create the Sil View.
            SilView = new SilView();
            SilView.OnEntitySelected += SilView_OnEntitySelected;
            SilView.ShowBreadcrumbsBar = true;

            //  Add it to the WPF element host.
            elementHost.Child = SilView;
        }

        public event DisassemblyEntityEvent OnEntitySelected;

        void SilView_OnEntitySelected(object sender, DisassembledEntityEventArgs eventArgs)
        {
            var theEvent = OnEntitySelected;
            if (theEvent != null)
                theEvent(sender, eventArgs);
        }

        public void InitialiseView(IDisassemblyProvider disassemblyProvider, DisassemblyTarget disassemblyTarget)
        {
            SilView.InitialiseView(disassemblyProvider, disassemblyTarget);
        }

        /// <summary>
        /// The sil view.
        /// </summary>
        public SilView SilView { get; private set; }
    }
}
