using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    //  TODO have one collection of ilclasses as a field, then each property (e.g. interfaces) is isclasses.OfType?

    /// <summary>
    /// A DisassembledILClass is an IL .class entity. This is not the same
    /// as a C# class - an IL .class can be a struct, class, enum, delegate etc.
    /// IL .classes can in many cases contain other other elements - 
    /// </summary>
    public class DisassembledIlClass : DisassembledEntity
    {
        public override string ToString()
        {
            return "Disassembled IL Class: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            //  Read the first two lines.
            string firstLine = null;
            string secondLine = null;
            using (var reader = new StringReader(RawIL))
            {
                firstLine = reader.ReadLine();
                if (firstLine != null)
                    secondLine = reader.ReadLine();
            }

            //  From the class declaration, read the full name and tokens.
            try
            {
                string className, templateSpecification;
                List<string> classModifiers;
                ILParseHelper.GetClassDeclarationParts(firstLine, out classModifiers, out className, out templateSpecification);
                FullName = className;
                TemplateSpecification = templateSpecification;
                ShortName = className.Split('.').Last();
                modifiers.Clear();
                modifiers.AddRange(classModifiers);
            }
            catch (Exception)
            {
                //  todo
            }

            if (secondLine != null)
            {
                string baseType;
                if (ILParseHelper.ReadExtendsLine(secondLine, out baseType))
                    BaseType = baseType;
            }

            //  Read the fields and methods.
            ReadFields();
            ReadMethods();
            ReadProperties();
            ReadEvents();
        }

        private void ReadFields()
        {
            fields.Clear();

            //  Read all lines.
            using (var reader = new StringReader(RawIL))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (ILParseHelper.IsLineFieldDeclaration(line))
                    {
                        var field = new DisassembledField();
                        field.Parent = this;
                        field.RawIL = line;
                        field.InitialiseFromIL();
                        field.FullName = FullName + "." + field.ShortName;
                        fields.Add(field);
                    }
                }
            }
        }

        private void ReadMethods()
        {
            if(FullName.Contains("WithTemplate"))
                Console.WriteLine();

            var methodBuilders = new List<StringBuilder>();

            //  Now start parsing the raw IL, line by line.
            using (var reader = new StringReader(RawIL))
            {
                //  Read each line
                string line;
                StringBuilder currentBuilder = null;
                string currentName = null;
                while ((line = reader.ReadLine()) != null)
                {
                    //  Is it a class start token? If so, we can start a new builder.
                    if (ILParseHelper.IsLineMethodDeclaration(line))
                    {
                        currentBuilder = new StringBuilder();
                    }
                    else if (currentBuilder != null && currentName == null)
                    {
                        //  We must now be on the name line.
                        currentName = ILParseHelper.GetMethodNameFromDeclarationLine(line);
                    }

                    //  If don't have a current class, skip.
                    if (currentBuilder == null)
                        continue;

                    //  Add the line to the class builder.
                    currentBuilder.AppendLine(line);

                    //  Is it a class end token? If so, clear the current class identifier.
                    if (ILParseHelper.IsLineMethodEndDeclaration(line, ShortName, currentName))
                    {
                        methodBuilders.Add(currentBuilder);
                        currentName = null;
                        currentBuilder = null;
                    }
                }
            }

            methodBuilders.ForEach(mb =>
                {
                    var method = new DisassembledMethod();
                    method.Parent = this;
                    method.RawIL = mb.ToString();
                    method.InitialiseFromIL();
                    method.FullName = FullName + "." + method.ShortName;
                    methods.Add(method);
                });
        }

        private void ReadProperties()
        {
            var propertyBuilders = new List<StringBuilder>();

            //  Now start parsing the raw IL, line by line.
            using (var reader = new StringReader(RawIL))
            {
                //  Read each line
                string line;
                StringBuilder currentBuilder = null;
                string currentName = null;
                while ((line = reader.ReadLine()) != null)
                {
                    //  Is it a class start token? If so, we can start a new builder.
                    if (ILParseHelper.IsLinePropertyDeclaration(line))
                    {
                        currentBuilder = new StringBuilder();
                        currentName = ILParseHelper.GetPropertyNameFromDeclarationLine(line);
                    }

                    //  If don't have a current class, skip.
                    if (currentBuilder == null)
                        continue;

                    //  Add the line to the class builder.
                    currentBuilder.AppendLine(line);

                    //  Is it a class end token? If so, clear the current class identifier.
                    if (ILParseHelper.IsLinePropertyEndDeclaration(line, ShortName, currentName))
                    {
                        propertyBuilders.Add(currentBuilder);
                        currentName = null;
                        currentBuilder = null;
                    }
                }
            }

            propertyBuilders.ForEach(mb =>
            {
                var method = new DisassembledProperty();
                method.Parent = this;
                method.RawIL = mb.ToString();
                method.InitialiseFromIL();
                method.FullName = FullName + "." + method.ShortName;
                properties.Add(method);
            });
        }

        private void ReadEvents()
        {
            var eventBuilders = new List<StringBuilder>();

            //  Now start parsing the raw IL, line by line.
            using (var reader = new StringReader(RawIL))
            {
                //  Read each line
                string line;
                StringBuilder currentBuilder = null;
                string currentName = null;
                while ((line = reader.ReadLine()) != null)
                {
                    //  Is it a class start token? If so, we can start a new builder.
                    if (ILParseHelper.IsLineEventPropertyDeclaration(line))
                    {
                        currentBuilder = new StringBuilder();
                        currentName = ILParseHelper.GetEventNameFromDeclarationLine(line);
                    }

                    //  If don't have a current class, skip.
                    if (currentBuilder == null)
                        continue;

                    //  Add the line to the class builder.
                    currentBuilder.AppendLine(line);

                    //  Is it a class end token? If so, clear the current class identifier.
                    if (ILParseHelper.IsLineEventEndDeclaration(line, ShortName, currentName))
                    {
                        eventBuilders.Add(currentBuilder);
                        currentName = null;
                        currentBuilder = null;
                    }
                }
            }

            eventBuilders.ForEach(mb =>
            {
                var method = new DisassembledEvent();
                method.Parent = this;
                method.RawIL = mb.ToString();
                method.InitialiseFromIL();
                method.FullName = FullName + "." + method.ShortName;
                events.Add(method);
            });
        }

        internal void AddChild(DisassembledIlClass ilClass)
        {
            childIlClasses.Add(ilClass);
        }

        internal IEnumerable<T> GetIlClassesRecursiveOfType<T>() where T : DisassembledIlClass
        {
            //  Create a set of classes.
            var ilClasses = new List<T>();

            //  Go through each il class.
            foreach (var ilClass in childIlClasses)
            {
                //  Add it if it's the right type..
                var typedIlClass = ilClass as T;
                if (typedIlClass != null)
                    ilClasses.Add(typedIlClass);

                //  Recurse.
                ilClasses.AddRange(ilClass.GetIlClassesRecursiveOfType<T>());
            }

            return ilClasses;
        }

        private readonly List<string> modifiers = new List<string>();

        private readonly List<DisassembledField> fields = new List<DisassembledField>(); 

        private readonly List<DisassembledMethod> methods = new List<DisassembledMethod>();

        private readonly List<DisassembledProperty> properties = new List<DisassembledProperty>(); 

        private readonly List<DisassembledEvent> events = new List<DisassembledEvent>();

        private readonly List<DisassembledIlClass> childIlClasses = new List<DisassembledIlClass>(); 

        public IEnumerable<string> Modifiers { get { return modifiers; } }

        public IEnumerable<DisassembledField> Fields { get { return fields; } }

        public IEnumerable<DisassembledMethod> Methods { get { return methods; } }

        public IEnumerable<DisassembledProperty> Properties { get { return properties; } }

        public IEnumerable<DisassembledEvent> Events { get { return events; } }

        public IEnumerable<DisassembledClass> Classes { get { return childIlClasses.OfType<DisassembledClass>(); } }

        public IEnumerable<DisassembledStructure> Structures { get { return childIlClasses.OfType<DisassembledStructure>(); } }

        public IEnumerable<DisassembledEnumeration> Enumerations { get { return childIlClasses.OfType<DisassembledEnumeration>(); } }

        public IEnumerable<DisassembledInterface> Interfaces { get { return childIlClasses.OfType<DisassembledInterface>(); } }

        public IEnumerable<DisassembledDelegate> Delegates { get { return childIlClasses.OfType<DisassembledDelegate>(); } } 

        /// <summary>
        /// Gets all classes, this is deep and recursive.
        /// </summary>
        /// <value>
        /// All classes.
        /// </value>
        public IEnumerable<DisassembledClass> AllClasses { get { return GetIlClassesRecursiveOfType<DisassembledClass>(); } }

        /// <summary>
        /// Gets all classes, this is deep and recursive.
        /// </summary>
        /// <value>
        /// All classes.
        /// </value>
        public IEnumerable<DisassembledStructure> AllStructures { get { return GetIlClassesRecursiveOfType<DisassembledStructure>(); } }

        /// <summary>
        /// Gets all classes, this is deep and recursive.
        /// </summary>
        /// <value>
        /// All classes.
        /// </value>
        public IEnumerable<DisassembledEnumeration> AllEnumerations { get { return GetIlClassesRecursiveOfType<DisassembledEnumeration>(); } }

        /// <summary>
        /// Gets all classes, this is deep and recursive.
        /// </summary>
        /// <value>
        /// All classes.
        /// </value>
        public IEnumerable<DisassembledDelegate> AllDelegates { get { return GetIlClassesRecursiveOfType<DisassembledDelegate>(); } }

        /// <summary>
        /// Gets all classes, this is deep and recursive.
        /// </summary>
        /// <value>
        /// All classes.
        /// </value>
        public IEnumerable<DisassembledInterface> AllInterfaces { get { return GetIlClassesRecursiveOfType<DisassembledInterface>(); } }

        public string BaseType { get; private set; }

        public void UpdateFullNamesOfChildren()
        {
            foreach (var childIlClass in childIlClasses)
            {
                childIlClass.FullName = FullName + "." + childIlClass.FullName;
                childIlClass.UpdateFullNamesOfChildren();
                childIlClass.Parent = this;
            }
        }
    }

    public class DisassemblyException : Exception
    {
        public DisassemblyException(string message) : base(message)
        {
        }

        public DisassemblyException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}