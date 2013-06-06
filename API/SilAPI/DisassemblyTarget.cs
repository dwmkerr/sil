using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilAPI
{
    /// <summary>
    /// A Disassembly Target is an object that describes an element that is being
    /// targetted in an assembly, such as a method, class or field.
    /// </summary>
    public class DisassemblyTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisassemblyTarget"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="fullName">The full name.</param>
        public DisassemblyTarget(DisassemblyTargetType targetType, string fullName)
        {
            TargetType = targetType;
            FullName = fullName;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Target: {0} - {1}", TargetType, FullName);
        }

        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public DisassemblyTargetType TargetType { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; }
    }

    public enum DisassemblyTargetType
    {
        Class,

        Enumeration,

        Method,

        Property,

        Field,

        Structure,

        Delegate,

        Event,

        Interface
    }
}
