using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apex.MVVM;
using SilAPI;
using SilUI;

namespace Sil.Main
{
    [ViewModel]
    public class DocumentViewModel : ViewModel, IDisassemblyProvider
    {
        public DocumentViewModel()
        {
        }

        public void Initialise(string path)
        {
            //  Set the path and title.
            Path = path;
            DocumentTitle = System.IO.Path.GetFileName(path);
        }
        
        /// <summary>
        /// The NotifyingProperty for the Path property.
        /// </summary>
        private readonly NotifyingProperty PathProperty =
          new NotifyingProperty("Path", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Path.
        /// </summary>
        /// <value>The value of Path.</value>
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }
        

        private void BuildViewModel(DisassembledAssembly disassembledAssembly)
        {
            ElementNodes.Clear();
            SelectedElementNode = null;
            
            //  Create a full document node.
            var assemblyNode = new ElementNodeViewModel(disassembledAssembly)
                {
                    Name = Path,
                    DisplayName = DocumentTitle,
                    ElementNodeType = ElementNodeType.Assembly,
                    Content = disassembledAssembly.RawIL
                };
            
            //  Add the child node.
            foreach (var className in disassembledAssembly.Classes.OrderBy(c => c.ShortName))
                assemblyNode.Nodes.Add(BuildElementNode(className));
            foreach (var className in disassembledAssembly.Structures.OrderBy(c => c.ShortName))
                assemblyNode.Nodes.Add(BuildElementNode(className));
            foreach (var className in disassembledAssembly.Interfaces.OrderBy(c => c.ShortName))
                assemblyNode.Nodes.Add(BuildElementNode(className));
            foreach (var className in disassembledAssembly.Enumerations.OrderBy(c => c.ShortName))
                assemblyNode.Nodes.Add(BuildElementNode(className));
            foreach (var className in disassembledAssembly.Delegates.OrderBy(c => c.ShortName))
                assemblyNode.Nodes.Add(BuildElementNode(className));
            
            ElementNodes.Add(assemblyNode);
        }

        private ElementNodeViewModel BuildElementNode(DisassembledEntity entity)
        {
            //  TODO sort out the orders

            var node = new ElementNodeViewModel(entity);
            node.Name = entity.FullName;
            node.DisplayName = entity.ShortName;
            node.Content = entity.RawIL;
            if(entity is DisassembledClass)
                node.ElementNodeType = ElementNodeType.Class;
            else if(entity is DisassembledAssembly)
                node.ElementNodeType = ElementNodeType.Assembly;
            else if (entity is DisassembledDelegate)
                node.ElementNodeType = ElementNodeType.Delegate;
            else if (entity is DisassembledEnumeration)
                node.ElementNodeType = ElementNodeType.Enum;
            else if (entity is DisassembledField)
                node.ElementNodeType = ElementNodeType.Field;
            else if (entity is DisassembledInterface)
                node.ElementNodeType = ElementNodeType.Interface;
            else if (entity is DisassembledMethod)
                node.ElementNodeType = ElementNodeType.Method;
            else if (entity is DisassembledStructure)
                node.ElementNodeType = ElementNodeType.Struct;
            else if (entity is DisassembledProperty)
                node.ElementNodeType = ElementNodeType.Property;
            else if (entity is DisassembledEvent)
                node.ElementNodeType = ElementNodeType.Event;


            var ilClass = entity as DisassembledIlClass;
            if (ilClass != null)
            {
                ilClass.Fields.OrderBy(c => c.ShortName).ToList().ForEach(f => node.Nodes.Add(BuildElementNode(f)));
                ilClass.Methods.OrderBy(c => c.ShortName).ToList().ForEach(f => node.Nodes.Add(BuildElementNode(f)));
                ilClass.Properties.OrderBy(c => c.ShortName).ToList().ForEach(p => node.Nodes.Add(BuildElementNode(p)));
                ilClass.Events.OrderBy(c => c.ShortName).ToList().ForEach(p => node.Nodes.Add(BuildElementNode(p)));
                ilClass.Classes.OrderBy(c => c.ShortName).ToList().ForEach(f => node.Nodes.Add(BuildElementNode(f)));
                ilClass.Structures.OrderBy(c => c.ShortName).ToList().ForEach(f => node.Nodes.Add(BuildElementNode(f)));
                ilClass.Enumerations.OrderBy(c => c.ShortName).ToList().ForEach(f => node.Nodes.Add(BuildElementNode(f)));
                ilClass.Interfaces.OrderBy(c => c.ShortName).ToList().ForEach(f => node.Nodes.Add(BuildElementNode(f)));
                ilClass.Delegates.OrderBy(c => c.ShortName).ToList().ForEach(f => node.Nodes.Add(BuildElementNode(f)));
            }

            return node;
        }

        /// <summary>
        /// The NotifyingProperty for the DocumentTitle property.
        /// </summary>
        private readonly NotifyingProperty DocumentTitleProperty =
          new NotifyingProperty("DocumentTitle", typeof(string), default(string));

        /// <summary>
        /// Gets or sets DocumentTitle.
        /// </summary>
        /// <value>The value of DocumentTitle.</value>
        public string DocumentTitle
        {
            get { return (string)GetValue(DocumentTitleProperty); }
            set { SetValue(DocumentTitleProperty, value); }
        }

        
        /// <summary>
        /// The ElementNodes observable collection.
        /// </summary>
        private readonly ObservableCollection<ElementNodeViewModel> ElementNodesProperty =
          new ObservableCollection<ElementNodeViewModel>();

        /// <summary>
        /// Gets the ElementNodes observable collection.
        /// </summary>
        /// <value>The ElementNodes observable collection.</value>
        public ObservableCollection<ElementNodeViewModel> ElementNodes
        {
            get { return ElementNodesProperty; }
        }

        
        /// <summary>
        /// The NotifyingProperty for the SelectedElementNode property.
        /// </summary>
        private readonly NotifyingProperty SelectedElementNodeProperty =
          new NotifyingProperty("SelectedElementNode", typeof(ElementNodeViewModel), default(ElementNodeViewModel));

        /// <summary>
        /// Gets or sets SelectedElementNode.
        /// </summary>
        /// <value>The value of SelectedElementNode.</value>
        public ElementNodeViewModel SelectedElementNode
        {
            get { return (ElementNodeViewModel)GetValue(SelectedElementNodeProperty); }
            set { SetValue(SelectedElementNodeProperty, value); }
        }

        public Task<DisassembledAssembly> DisassembleAssembly()
        {
            var task = Task<DisassembledAssembly>.Factory.StartNew(() =>
                {
                    //  Disassemble the assembly.
                    var disassembledAssembly = Disassembler.DisassembleAssembly(Path);

                    return disassembledAssembly;
                });
            task.ContinueWith(t => BuildViewModel(t.Result), TaskScheduler.FromCurrentSynchronizationContext());

            return task;
        }
    }
}
