using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace SilUI
{
    public class DisassemblyEditor : TextEditor
    {
        public DisassemblyEditor()
        {
            Loaded += DisassemblyEditor_Loaded;
        }

        void DisassemblyEditor_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadILSyntax();
        }

        /// <summary>
        /// Loads the IL syntax.
        /// </summary>
        private void LoadILSyntax()
        {
            using (var stream = typeof(DisassemblyEditor).Assembly.GetManifestResourceStream("SilUI.Resources.ILSyntax.xshd"))
            {
                if (stream == null)
                    return;
                using (var reader = new XmlTextReader(stream))
                {
                    SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
