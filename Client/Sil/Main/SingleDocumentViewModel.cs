using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Apex.MVVM;

namespace Sil.Main
{
    [ViewModel]
    public class SingleDocumentViewModel : ViewModel
    {
        public SingleDocumentViewModel()
        {
        }

        public void DisassemblyPEFile(string path)
        {
            //  Create a new document view model.
            var documentViewModel = new DocumentViewModel();
            documentViewModel.Initialise(path);

            //  Add it to the set of documents.
            Document = documentViewModel;
        }

        
        /// <summary>
        /// The NotifyingProperty for the Document property.
        /// </summary>
        private readonly NotifyingProperty DocumentProperty =
          new NotifyingProperty("Document", typeof(DocumentViewModel), default(DocumentViewModel));

        /// <summary>
        /// Gets or sets Document.
        /// </summary>
        /// <value>The value of Document.</value>
        public DocumentViewModel Document
        {
            get { return (DocumentViewModel)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }
    }
}
