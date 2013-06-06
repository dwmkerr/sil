using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Apex.MVVM;

namespace Sil.Main
{
    [ViewModel]
    public class MultiDocumentViewModel : ViewModel
    {
        public MultiDocumentViewModel()
        {
            //  Create the NewDocument Command.
            NewDocumentCommand = new Command(DoNewDocumentCommand);
        }

        public void DisassemblyPEFile(string path)
        {
            //  Create a new document view model.
            var documentViewModel = new DocumentViewModel();
            documentViewModel.Initialise(path);

            //  Add it to the set of documents.
            Documents.Add(documentViewModel);
        }
        
        /// <summary>
        /// The Documents observable collection.
        /// </summary>
        private readonly ObservableCollection<DocumentViewModel> DocumentsProperty =
          new ObservableCollection<DocumentViewModel>();

        /// <summary>
        /// Gets the Documents observable collection.
        /// </summary>
        /// <value>The Documents observable collection.</value>
        public ObservableCollection<DocumentViewModel> Documents
        {
            get { return DocumentsProperty; }
        }


        /// <summary>
        /// Performs the NewDocument command.
        /// </summary>
        /// <param name="parameter">The NewDocument command parameter.</param>
        private void DoNewDocumentCommand(object parameter)
        {
            //  Add a new document.
            Documents.Add(new DocumentViewModel());
        }

        /// <summary>
        /// Gets the NewDocument command.
        /// </summary>
        /// <value>The value of .</value>
        public Command NewDocumentCommand
        {
            get;
            private set;
        }
    }
}
