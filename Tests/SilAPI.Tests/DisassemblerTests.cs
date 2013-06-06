using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SilAPI;

namespace SilAPI.Tests
{
    [TestFixture]
    [Category("Core Disassembler")]
    public class DisassemblerTests
    {
        private string GetAcceptanceModelPath()
        {
            return Path.Combine(TestHelper.GetTestsPath(), @"AcceptanceModel.dll");
        }

        [Test]
        [Description("Ensures that the Disassembler class can disassemble a valid assembly.")]
        public void CanDisassembleAssembly()
        {
            Assert.DoesNotThrow(() =>
            {
                var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());
                Assert.IsNotNull(disassembledAssembly);
            });
        }

        [Test]
        [Description("Ensures that the Disassembler class can provide full raw IL.")]
        public void CanGetRawIL()
        {
            var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());

            //  Make sure we have the IL.
            Assert.IsNotNullOrEmpty(disassembledAssembly.RawIL);
        }

        [Test]
        [Description("Ensures that the Disassembler class can get the class names.")]
        public void CanGetClasses()
        {
            var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());

            //  Make sure we have the expected file names.
            Assert.IsTrue(disassembledAssembly.Classes.Any(c => c.FullName == "AcceptanceModel.AnotherExampleClass"));
            Assert.IsTrue(disassembledAssembly.Classes.Any(c => c.FullName == "AcceptanceModel.ExampleClass"));
        }

        [Test]
        [Description("Ensures that the Disassembler class can get the structures.")]
        public void CanGetStructures()
        {
            var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());

            //  Make sure we have the expected file names.
            Assert.IsTrue(disassembledAssembly.Structures.Any(c => c.FullName == "AcceptanceModel.ExampleStructure"));
        }

        [Test]
        [Description("Ensures that the Disassembler class can get the interfaces.")]
        public void CanGetInterfaces()
        {
            var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());

            //  Make sure we have the expected file names.
            Assert.IsTrue(disassembledAssembly.Interfaces.Any(c => c.FullName == "AcceptanceModel.ExampleInterface"));
        }

        [Test]
        [Description("Ensures that the Disassembler class can get the emnumerations.")]
        public void CanGetEnumerations()
        {
            var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());

            //  Make sure we have the expected file names.
            Assert.IsTrue(disassembledAssembly.Enumerations.Any(c => c.FullName == "AcceptanceModel.ExampleEnumeration"));
        }

        [Test]
        [Description("Ensures that a known method can be discovered.")]
        public void CanGetMethod()
        {
            var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());
            var disassembledClass = disassembledAssembly.Classes.Single(c => c.ShortName == @"ExampleClass");
            var disassembledMethod = disassembledClass.Methods.Single(c => c.ShortName == @"CountLines");
            Assert.IsNotNull(disassembledMethod);
        }

        [Test]
        [Description("Ensures that a known event can be discovered.")]
        public void CanGetEvent()
        {
            var disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());
            var disassembledClass = disassembledAssembly.Classes.Single(c => c.ShortName == @"ClassWithEvents");
            var disassembledMethod = disassembledClass.Events.Single(c => c.ShortName == @"OnSomething");
            Assert.IsNotNull(disassembledMethod);
        }
    }
}
