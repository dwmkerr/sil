using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Apex.MVVM;
using SilAPI;

namespace Sil.Main
{
    public class ElementNodeViewModel : ViewModel
    {
        public ElementNodeViewModel(DisassembledEntity disassembledEntity)
        {
            Model = disassembledEntity;
        }

        public DisassembledEntity Model { get; private set; }

        /// <summary>
        /// The NotifyingProperty for the DisplayName property.
        /// </summary>
        private readonly NotifyingProperty DisplayNameProperty =
          new NotifyingProperty("DisplayName", typeof(string), default(string));

        /// <summary>
        /// Gets or sets DisplayName.
        /// </summary>
        /// <value>The value of DisplayName.</value>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        
        /// <summary>
        /// The NotifyingProperty for the Name property.
        /// </summary>
        private readonly NotifyingProperty NameProperty =
          new NotifyingProperty("Name", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// <value>The value of Name.</value>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        
        /// <summary>
        /// The NotifyingProperty for the Content property.
        /// </summary>
        private readonly NotifyingProperty ContentProperty =
          new NotifyingProperty("Content", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Content.
        /// </summary>
        /// <value>The value of Content.</value>
        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        
        /// <summary>
        /// The Nodes observable collection.
        /// </summary>
        private readonly ObservableCollection<ElementNodeViewModel> NodesProperty =
          new ObservableCollection<ElementNodeViewModel>();

        /// <summary>
        /// Gets the Nodes observable collection.
        /// </summary>
        /// <value>The Nodes observable collection.</value>
        public ObservableCollection<ElementNodeViewModel> Nodes
        {
            get { return NodesProperty; }
        }

        
        /// <summary>
        /// The NotifyingProperty for the ElementNodeType property.
        /// </summary>
        private readonly NotifyingProperty ElementNodeTypeProperty =
          new NotifyingProperty("ElementNodeType", typeof(ElementNodeType), default(ElementNodeType));

        /// <summary>
        /// Gets or sets ElementNodeType.
        /// </summary>
        /// <value>The value of ElementNodeType.</value>
        public ElementNodeType ElementNodeType
        {
            get { return (ElementNodeType)GetValue(ElementNodeTypeProperty); }
            set { SetValue(ElementNodeTypeProperty, value); }
        }
    }

    public enum ElementNodeType
    {
        Assembly,
        Class,
        Struct,
        Interface,
        Namespace,
        Enum,
        Method,
        Delegate,
        Field,
        Property,
        Event
    }
}
