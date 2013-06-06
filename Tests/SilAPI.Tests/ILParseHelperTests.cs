using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SilAPI;

namespace SilAPI.Tests
{
	[TestFixture]
    [Category("IL Parsing Tests")]
    public class ILParseHelperTests
    {
		[Test]
		public void CanIdentifyClassDeclaration()
		{
		    var testLine = @".class public auto ansi beforefieldinit ExampleAssembly.AnotherExampleClass";
			Assert.IsTrue(ILParseHelper.IsLineClassDeclaration(testLine));
		}

        [Test]
        public void CanReadBaseType()
        {
            var testLine = @"       extends [mscorlib]System.Object";
            string baseType;
            Assert.IsTrue(ILParseHelper.ReadExtendsLine(testLine, out baseType));
            Assert.AreEqual(baseType, @"[mscorlib]System.Object");
        }
        
        [Test]
        public void CanIdentifyStructDeclaration()
        {
            var testLine = @".class public sequential ansi sealed beforefieldinit ExampleAssembly.ExampleStructure";
            Assert.IsTrue(ILParseHelper.IsLineStructDeclaration(testLine));
        }

        [Test]
        public void CanIdentifyInterfaceDeclaration()
        {
            var testLine = @".class interface public abstract auto ansi ExampleAssembly.ExampleInterface";
            Assert.IsTrue(ILParseHelper.IsLineInterfaceDeclaration(testLine));
        }

        [Test]
        public void CanIdentifyEndClassLine()
        {
            var testLine = @"} // end of class ExampleAssembly.AnotherExampleClass";
            Assert.IsTrue(ILParseHelper.IsLineStartClassEndToken(testLine));
        }

        [Test]
        public void CanGetClassNameFromClassDeclarationLine()
        {
            var testLine = @".class public auto ansi beforefieldinit ExampleAssembly.AnotherExampleClass";
            string className;
            string templateSpecifcation;
            ILParseHelper.GetClassNameFromClassDeclarationLine(testLine, out className, out templateSpecifcation);
            Assert.AreEqual(@"ExampleAssembly.AnotherExampleClass", className);
        }

        [Test]
        public void CanParseAnoymousTemplateClass()
        {
            var testLine = @"  .class auto ansi sealed nested private beforefieldinit '<GetVisualChildren>d__0`1'<([WindowsBase]System.Windows.DependencyObject) T>";
            string className;
            string templateSpecifcation;
            ILParseHelper.GetClassNameFromClassDeclarationLine(testLine, out className, out templateSpecifcation);
            Assert.AreEqual(className, @"'<GetVisualChildren>d__0`1'");
            Assert.AreEqual(templateSpecifcation, @"<([WindowsBase]System.Windows.DependencyObject) T>");
        }
    }
}
