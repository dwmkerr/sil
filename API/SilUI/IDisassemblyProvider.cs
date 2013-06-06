using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilAPI;

namespace SilUI
{
    /// <summary>
    /// A contract for an object that can provide a disassembled
    /// assembly to a SilView. This contract uses TPL as operations
    /// may take some time.
    /// </summary>
    public interface IDisassemblyProvider
    {
        /// <summary>
        /// Disassembles the assembly.
        /// </summary>
        /// <returns>A task to disassemble the assembly.</returns>
        Task<DisassembledAssembly> DisassembleAssembly();
    }
}
