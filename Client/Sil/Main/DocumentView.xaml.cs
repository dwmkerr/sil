using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Sil.Main
{
    /// <summary>
    /// Interaction logic for DocumentView.xaml
    /// </summary>
    public partial class DocumentView : UserControl
    {
        public DocumentView()
        {
            InitializeComponent();
        }

        public void Initialise(DocumentViewModel viewModel)
        {
            DataContext = viewModel;
            
            //  We can now initialise the view.
            silView.InitialiseView(ViewModel, null);
        }

        public DocumentViewModel ViewModel { get { return (DocumentViewModel) DataContext; } }
        
        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedElementNode = e.NewValue as ElementNodeViewModel;
            silView.DisassemblyEditor.Document = null;
            if (ViewModel.SelectedElementNode != null && ViewModel.SelectedElementNode.Content != null)
            {
                silView.SelectedEntity = ViewModel.SelectedElementNode.Model;
            }
        }
    }
}
