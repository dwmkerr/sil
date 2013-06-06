using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SilAPI;

namespace SilAPI.Tests
{
    [Category("Disassembly Targetting Tests")]
    [TestFixture]
    public class TargettingTests
    {
        private string GetAcceptanceModelPath()
        {
            return Path.Combine(TestHelper.GetTestsPath(), @"AcceptanceModel.dll");
        }

        [SetUp]
        public void Setup()
        {
            //  Disassemble the ExampleAssemble.
            disassembledAssembly = Disassembler.DisassembleAssembly(GetAcceptanceModelPath());
        }

        private DisassembledAssembly disassembledAssembly;

        [Test]
        [Description("Ensures we can target a class.")]
        public void CanTargetClass()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Class, "AcceptanceModel.ExampleClass");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetClassWithNesting()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Class, "AcceptanceModel.ClassWithNestedClass");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetNestedClass()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Class, "AcceptanceModel.ClassWithNestedClass.ClassNestedInClass");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetDeepNestedClass()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Class, "AcceptanceModel.ClassWithNestedClass.ClassNestedInClass.DeepNestedClassInClassNestedInClass");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetVeryDeepNestedStruct()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Structure, "AcceptanceModel.ClassWithNestedClass.ClassNestedInClass.DeepNestedClassInClassNestedInClass.VeryDeepNestedStructInDeepNestedClassInClassNestedInClass");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetExtremelyDeepNestedDelegate()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Delegate, "AcceptanceModel.ClassWithNestedClass.ClassNestedInClass.DeepNestedClassInClassNestedInClass.VeryDeepNestedStructInDeepNestedClassInClassNestedInClass.DelegateInVeryDeepNestedStructInDeepNestedClassInClassNestedInClass");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetStructWithNesting()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Structure, "AcceptanceModel.StructWithNestedClass");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetClassNestedInStruct()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Class, "AcceptanceModel.StructWithNestedClass.ClassNestedInStruct");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetDelegateNestedInClassNestedInStruct()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Delegate, "AcceptanceModel.StructWithNestedClass.ClassNestedInStruct.DelegateInClassNestedInStruct");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        public void CanTargetStructNestedInStruct()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Structure, "AcceptanceModel.StructWithNestedStruct.StructNestedInStruct");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        [Description("Ensures we can target a method.")]
        public void CanTargetMethod()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Method, "AcceptanceModel.ExampleClass.ExampleFunction");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }

        [Test]
        [Description("Ensures we can target a class with template methods.")]
        public void CanTargetClassWithTemplateMethods()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Class, "AcceptanceModel.ClassWithTemplateMethods");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target);

            //  Make sure we have found the entity.
            Assert.IsNotNull(entity, string.Format("Failed to find target {0}.", target));
        }


        [Test]
        [Description("Ensures we can target a template method in a class with template methods.")]
        public void CanTargetTemplateMethodInClassWithTemplateMethods()
        {
            //  Create the target.
            var target = new DisassemblyTarget(DisassemblyTargetType.Class, "AcceptanceModel.ClassWithTemplateMethods");

            //  Try and find the entity.
            var entity = disassembledAssembly.FindDisassembledEntity(target) as DisassembledClass;

            Assert.IsNotNull(entity);

            //  Do we have the template method?
            var templateMethod = entity.Methods.FirstOrDefault(m => m.FullName == @"AcceptanceModel.ClassWithTemplateMethods.WriteObject<T>");

            //  Make sure we have found the entity.
            Assert.IsNotNull(templateMethod, string.Format("Failed to find target {0}.", target));
        }
    }
}
