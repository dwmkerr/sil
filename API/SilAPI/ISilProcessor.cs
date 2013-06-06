using System.IO;

namespace SilAPI
{
    /// <summary>
    /// A SilProcessor is an object ready to process IL from some input.
    /// </summary>
    public interface ISilProcessor
    {
        /// <summary>
        /// Processes the IL.
        /// </summary>
        /// <returns>
        /// False if processing the IL failed (i.e. we got nothing back).
        /// </returns>
        bool ProcessIL();

        /// <summary>
        /// Gets the selection IL.
        /// </summary>
        /// <returns>The IL for the selection.</returns>
        string GetSelectionIL();

        /// <summary>
        /// Gets the file IL.
        /// </summary>
        /// <returns>The IL for the file the selection is in.</returns>
        string GetFileIL();

        /// <summary>
        /// Gets the module IL.
        /// </summary>
        /// <returns>The IL for the module the selection is in.</returns>
        string GetModuleIL();
    }
}
