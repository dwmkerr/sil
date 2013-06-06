using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.Document;
using SilAPI;

namespace SilUI
{
    /// <summary>
    /// Interaction logic for SilView.xaml
    /// </summary>
    public partial class SilView : UserControl
    {
        public SilView()
        {
            InitializeComponent();
        }

        public void InitialiseView(IDisassemblyProvider disassemblyProvider, DisassemblyTarget disassemblyTarget)
        {
            //  We can start disassembling.
            ViewModel.IsDisassembling = true;

            //  Execute the task to disassemble the assembly.
            disassemblyProvider
                .DisassembleAssembly()
                .ContinueWith(t =>
                    {
                        ViewModel.DisassembledAssembly = t.Result;
                        
                        //  Can we find the disassembled entity we're targetting?
                        var disassembledEntity = t.Result.FindDisassembledEntity(disassemblyTarget);

                        SelectedEntity = disassembledEntity ?? t.Result;
                        OnSelectedEntityChanged(this, new DependencyPropertyChangedEventArgs(SelectedEntityProperty, null, disassembledEntity));

                        //  Set the content of the disassembly editor.
                        ViewModel.IsDisassembling = false;

                    }, TaskScheduler.FromCurrentSynchronizationContext());

            ViewModel.SelectBreadcrumbCommand.Executed += SelectBreadcrumbCommand_Executed;
        }

        void SelectBreadcrumbCommand_Executed(object sender, Apex.MVVM.CommandEventArgs args)
        {
            var breadcrumbElement = args.Parameter as DisassembledEntity;
            if (breadcrumbElement != null)
                SelectedEntity = breadcrumbElement;
        }

        private void UpdateDocumentText()
        {
            var entity = SelectedEntity ?? ViewModel.DisassembledAssembly;

            //  Update the document text.
            disassemblyEditor.Document = new TextDocument(ShowComments ? entity.RawIL : entity.RawIlWithoutComments);
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public SilViewModel ViewModel { get { return (SilViewModel) DataContext; } }

        public DisassemblyEditor DisassemblyEditor { get { return disassemblyEditor; } }

        /// <summary>
        /// The DependencyProperty for the ShowBreadcrumbsBar property.
        /// </summary>
        public static readonly DependencyProperty ShowBreadcrumbsBarProperty =
          DependencyProperty.Register("ShowBreadcrumbsBar", typeof(bool), typeof(SilView),
          new PropertyMetadata(default(bool), new PropertyChangedCallback(OnShowBreadcrumbsBarChanged)));

        /// <summary>
        /// Gets or sets ShowBreadcrumbsBar.
        /// </summary>
        /// <value>The value of ShowBreadcrumbsBar.</value>
        public bool ShowBreadcrumbsBar
        {
            get { return (bool)GetValue(ShowBreadcrumbsBarProperty); }
            set { SetValue(ShowBreadcrumbsBarProperty, value); }
        }

        /// <summary>
        /// Called when ShowBreadcrumbsBar is changed.
        /// </summary>
        /// <param name="o">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowBreadcrumbsBarChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            SilView me = o as SilView;
        }

        
        /// <summary>
        /// The DependencyProperty for the SelectedEntity property.
        /// </summary>
        public static readonly DependencyProperty SelectedEntityProperty =
          DependencyProperty.Register("SelectedEntity", typeof(DisassembledEntity), typeof(SilView),
          new PropertyMetadata(default(DisassembledEntity), new PropertyChangedCallback(OnSelectedEntityChanged)));

        /// <summary>
        /// Gets or sets SelectedEntity.
        /// </summary>
        /// <value>The value of SelectedEntity.</value>
        public DisassembledEntity SelectedEntity
        {
            get { return (DisassembledEntity)GetValue(SelectedEntityProperty); }
            set { SetValue(SelectedEntityProperty, value); }
        }

        /// <summary>
        /// Called when SelectedEntity is changed.
        /// </summary>
        /// <param name="o">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedEntityChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            SilView me = o as SilView;
            me.UpdateDocumentText();

            me.ViewModel.UpdateBreadcrumbs(me.SelectedEntity);

            me.FireOnEntitySelected(me.SelectedEntity);
        }

        
        /// <summary>
        /// The DependencyProperty for the ShowComments property.
        /// </summary>
        public static readonly DependencyProperty ShowCommentsProperty =
          DependencyProperty.Register("ShowComments", typeof(bool), typeof(SilView),
          new PropertyMetadata(default(bool), new PropertyChangedCallback(OnShowCommentsChanged)));

        /// <summary>
        /// Gets or sets ShowComments.
        /// </summary>
        /// <value>The value of ShowComments.</value>
        public bool ShowComments
        {
            get { return (bool)GetValue(ShowCommentsProperty); }
            set { SetValue(ShowCommentsProperty, value); }
        }

        /// <summary>
        /// Called when ShowComments is changed.
        /// </summary>
        /// <param name="o">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowCommentsChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            SilView me = o as SilView;
            me.UpdateDocumentText();
        }

        
        /// <summary>
        /// The DependencyProperty for the ShowAnnotation property.
        /// </summary>
        public static readonly DependencyProperty ShowAnnotationProperty =
          DependencyProperty.Register("ShowAnnotation", typeof(bool), typeof(SilView),
          new PropertyMetadata(default(bool), new PropertyChangedCallback(OnShowAnnotationChanged)));

        /// <summary>
        /// Gets or sets ShowAnnotation.
        /// </summary>
        /// <value>The value of ShowAnnotation.</value>
        public bool ShowAnnotation
        {
            get { return (bool)GetValue(ShowAnnotationProperty); }
            set { SetValue(ShowAnnotationProperty, value); }
        }

        /// <summary>
        /// Called when ShowAnnotation is changed.
        /// </summary>
        /// <param name="o">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowAnnotationChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            SilView me = o as SilView;
        }

        
        /// <summary>
        /// The DependencyProperty for the ShowAssemblyTree property.
        /// </summary>
        public static readonly DependencyProperty ShowAssemblyTreeProperty =
          DependencyProperty.Register("ShowAssemblyTree", typeof(bool), typeof(SilView),
          new PropertyMetadata(default(bool), new PropertyChangedCallback(OnShowAssemblyTreeChanged)));

        /// <summary>
        /// Gets or sets ShowAssemblyTree.
        /// </summary>
        /// <value>The value of ShowAssemblyTree.</value>
        public bool ShowAssemblyTree
        {
            get { return (bool)GetValue(ShowAssemblyTreeProperty); }
            set { SetValue(ShowAssemblyTreeProperty, value); }
        }

        /// <summary>
        /// Called when ShowAssemblyTree is changed.
        /// </summary>
        /// <param name="o">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowAssemblyTreeChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            SilView me = o as SilView;
        }

        private void FireOnEntitySelected(DisassembledEntity disassembledEntity)
        {
            var theEvent = OnEntitySelected;
            if(theEvent != null)
                theEvent(this, new DisassembledEntityEventArgs(disassembledEntity));
        }

        public event DisassemblyEntityEvent OnEntitySelected;

        private void buttonGoogleSelection_Click(object sender, RoutedEventArgs e)
        {
            var selection = DisassemblyEditor.SelectedText;
            System.Diagnostics.Process.Start("http://www.google.com/search?&q=" + selection);
        }
    }

    public class DisassembledEntityEventArgs : EventArgs
    {
        public DisassembledEntityEventArgs(DisassembledEntity entity)
        {
            Entity = entity;
        }

        public DisassembledEntity Entity { get; private set; }
    }

    public delegate void DisassemblyEntityEvent(object sender, DisassembledEntityEventArgs eventArgs);
}
