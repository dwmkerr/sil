using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    /// <summary>
    /// The base class for all disassembled entities (which could be things like
    /// an assembly, interface, class, method etc).
    /// </summary>
    public abstract class DisassembledEntity
    {
        public override string ToString()
        {
            return "Disassembled Entity: " + ShortName ?? "Unknown";
        }
        
        /// <summary>
        /// Initialises this instance, reading whatever properties can be read from the IL.
        /// </summary>
        public abstract void InitialiseFromIL();

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; internal set; }

        public string TemplateSpecification { get; internal set; }

        /// <summary>
        /// Gets the short name.
        /// </summary>
        /// <value>
        /// The short name.
        /// </value>
        public string ShortName { get; internal set; }

        /// <summary>
        /// Gets the raw IL.
        /// </summary>
        /// <value>
        /// The raw IL.
        /// </value>
        public string RawIL { get; internal set; }

        public string RawIlWithoutComments
        {
            get
            {
                var builder = new StringBuilder();
                using(var reader = new StringReader(RawIL))
                {
                    string line;
                    while((line = reader.ReadLine()) != null)
                    {
                        if (!ILParseHelper.IsLineSourceComment(line))
                            builder.AppendLine(line);
                    }
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public DisassembledEntity Parent { get; internal set; }
    }
}
