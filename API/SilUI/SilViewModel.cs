using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Apex.MVVM;
using SilAPI;

namespace SilUI
{
    public class SilViewModel : ViewModel
    {
        public SilViewModel()
        {
            SelectBreadcrumbCommand = new Command(DoSelectBreadcrumbCommand);
        }

        public void UpdateBreadcrumbs(DisassembledEntity selectedEntity)
        {
            Breadcrumbs.Clear();

            var current = selectedEntity;
            while (current != null)
            {
                Breadcrumbs.Insert(0, current);
                current = current.Parent;
            }
        }

        /// <summary>
        /// The NotifyingProperty for the IsDisassembling property.
        /// </summary>
        private readonly NotifyingProperty IsDisassemblingProperty =
          new NotifyingProperty("IsDisassembling", typeof(bool), default(bool));

        /// <summary>
        /// Gets or sets IsDisassembling.
        /// </summary>
        /// <value>The value of IsDisassembling.</value>
        public bool IsDisassembling
        {
            get { return (bool)GetValue(IsDisassemblingProperty); }
            set { SetValue(IsDisassemblingProperty, value); }
        }

        
        /// <summary>
        /// The NotifyingProperty for the DisassembledAssembly property.
        /// </summary>
        private readonly NotifyingProperty DisassembledAssemblyProperty =
          new NotifyingProperty("DisassembledAssembly", typeof(DisassembledAssembly), default(DisassembledAssembly));

        /// <summary>
        /// Gets or sets DisassembledAssembly.
        /// </summary>
        /// <value>The value of DisassembledAssembly.</value>
        public DisassembledAssembly DisassembledAssembly
        {
            get { return (DisassembledAssembly)GetValue(DisassembledAssemblyProperty); }
            set { SetValue(DisassembledAssemblyProperty, value); }
        }

        
        /// <summary>
        /// The Breadcrumbs observable collection.
        /// </summary>
        private readonly ObservableCollection<DisassembledEntity> BreadcrumbsProperty =
          new ObservableCollection<DisassembledEntity>();

        /// <summary>
        /// Gets the Breadcrumbs observable collection.
        /// </summary>
        /// <value>The Breadcrumbs observable collection.</value>
        public ObservableCollection<DisassembledEntity> Breadcrumbs
        {
            get { return BreadcrumbsProperty; }
        }

        

        /// <summary>
        /// Performs the SelectBreadcrumb command.
        /// </summary>
        /// <param name="parameter">The SelectBreadcrumb command parameter.</param>
        private void DoSelectBreadcrumbCommand(object parameter)
        {
        }

        /// <summary>
        /// Gets the SelectBreadcrumb command.
        /// </summary>
        /// <value>The value of .</value>
        public Command SelectBreadcrumbCommand
        {
            get;
            private set;
        }
    }
}
