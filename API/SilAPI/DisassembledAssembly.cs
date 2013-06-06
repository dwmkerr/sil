using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    /// <summary>
    /// Represents an assembly that has been disassembled.
    /// Note that internally, most properties on this object 
    /// are lazy, so won't be parsed until required.
    /// </summary>
    public class DisassembledAssembly : DisassembledEntity
    {
        public DisassembledAssembly()
        {
            lazyDisassembledIlClasses = new Lazy<List<DisassembledIlClass>>(CreateDisassembledIlClasses);
        }

        private List<DisassembledIlClass> CreateDisassembledIlClasses()
        {
            var disassembledEntities = new List<DisassembledIlClass>();

            //  Parse the disassembled entities.
            var disassembledIlClasses = ParseDisassembledIlClasses();

            //  Go through each entity.
            foreach (var disassembledIlClass in disassembledIlClasses)
            {
                //  Create the entity type from the entity IL.
                var ilClass = CreateIlClassFromRawIlClass(disassembledIlClass);

                if (ilClass != null)
                {
                    //  Initialise the entity.
                    disassembledEntities.Add(ilClass);

                    //  Set the parent
                    ilClass.Parent = this;

                    //  Recursively set the full names of child objects.
                    ilClass.UpdateFullNamesOfChildren();
                }
            }
            return disassembledEntities;
        }

        private DisassembledIlClass CreateIlClassFromRawIlClass(RawILClass rawIlClass)
        {
            //  Get the raw IL.
            var rawIL = rawIlClass.ILBuilder.ToString();

            //  Get the first and second line.
            string firstLine = null;
            string secondLine = null;
            using (var reader = new StringReader(rawIL))
            {
                firstLine = reader.ReadLine();
                if (firstLine != null)
                    secondLine = reader.ReadLine();
            }

            DisassembledIlClass ilClass = new DisassembledClass();

            //  We can immediately identify structures and interfaces.
            if (ILParseHelper.IsLineStructDeclaration(firstLine))
                ilClass = new DisassembledStructure();
            else if(ILParseHelper.IsLineInterfaceDeclaration(firstLine))
                ilClass = new DisassembledInterface();
            else
            {
                //  Now check the second line.
                if (secondLine == null)
                    ilClass = new DisassembledClass();
                else
                {
                    string baseType;
                    if (!ILParseHelper.ReadExtendsLine(secondLine, out baseType))
                        ilClass = new DisassembledClass();
                    if (baseType == @"[mscorlib]System.Enum")
                        ilClass = new DisassembledEnumeration();
                    else if (baseType == @"[mscorlib]System.MulticastDelegate")
                        ilClass = new DisassembledDelegate();
                }
            }

            //  Set the IL.
            ilClass.RawIL = rawIL;
            ilClass.InitialiseFromIL();

            //  Add any children.
            foreach (var rawChild in rawIlClass.Children)
            {
                //  Create the entity type from the entity IL.
                var childIlClass = CreateIlClassFromRawIlClass(rawChild);
                ilClass.AddChild(childIlClass);
            }

            return ilClass;
        }

        private List<RawILClass> ParseDisassembledIlClasses()
        {
            //  We'll return a list of raw IL classes.
            var rootRawILClasses = new List<RawILClass>();

            //  Now start parsing the raw IL, line by line.
            using (var reader = new StringReader(RawIL))
            {
                //  IL classes can be nested, so when parsing them we must maintain
                //  a stack (so we can nest too).
                var rawIlClassStack = new Stack<RawILClass>();

                //  Read each line
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("<GetVisualChildren>"))
                    {
                        Console.WriteLine();
                    }

                    //  Is it a class start token? If so, we can start a new builder.
                    if (ILParseHelper.IsLineAnyLevelIlClassDeclaration(line))
                    {
                        string fullName, templateSpecification;
                        ILParseHelper.GetClassNameFromClassDeclarationLine(line, out fullName, out templateSpecification);

                        //  Create a new raw IL class, parse its name, add it to the list if it's top level, add it to the stack.
                        var rawIlClass = new RawILClass {FullName = fullName, TemplateSpecification = templateSpecification};
                        if(!rawIlClassStack.Any())
                            rootRawILClasses.Add(rawIlClass);
                        else
                            rawIlClassStack.Peek().Children.Add(rawIlClass);
                        rawIlClassStack.Push(rawIlClass);
                    }

                    //  If don't have a current class, skip.
                    if (!rawIlClassStack.Any())
                        continue;

                    //  Add the line to the class builder.
                    rawIlClassStack.Peek().ILBuilder.AppendLine(line);
                    
                    //  Is it a class end token? If so, we've finished the class.
                    if (ILParseHelper.IsLineClassEndDeclaration(line, rawIlClassStack.Peek().FullName))
                    {
                        //  Pop the stack.
                        rawIlClassStack.Pop();
                    }
                }
            }

            //  Return the set of class builders.
            return rootRawILClasses;
        }

        public override void InitialiseFromIL()
        {

        }

        /// <summary>
        /// Tries to find a disassembled entity, given a disassembled target.
        /// </summary>
        /// <param name="disassemblyTarget">The disassembly target.</param>
        /// <returns></returns>
        public DisassembledEntity FindDisassembledEntity(DisassemblyTarget disassemblyTarget)
        {
            //  If there's no target, we can't find anything.
            if (disassemblyTarget == null)
                return null;

            switch (disassemblyTarget.TargetType)
            {
                case DisassemblyTargetType.Class:

                    //  Find the class with the given name.
                    return AllClasses.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);

                case DisassemblyTargetType.Enumeration:

                    //  Find the enumeration with the given name.
                    return AllEnumerations.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);
                    
                case DisassemblyTargetType.Method:

                    //  Find the entity with the given name.
                    return AllMethods.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);

                case DisassemblyTargetType.Property:

                    //  Find the entity with the given name.
                    return AllProperties.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);

                case DisassemblyTargetType.Field:

                    //  Find the entity with the given name.
                    return AllFields.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);

                case DisassemblyTargetType.Structure:

                    //  Find the structure with the given name.
                    return AllStructures.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);

                case DisassemblyTargetType.Interface:

                    //  Find the structure with the given name.
                    return AllInterfaces.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);

                case DisassemblyTargetType.Event:

                    //  Find the structure with the given name.
                    return AllEvents.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);

                case DisassemblyTargetType.Delegate:

                    //  Find the structure with the given name.
                    return AllDelegates.FirstOrDefault(c => c.FullName == disassemblyTarget.FullName);


                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private IEnumerable<T> GetIlClassesRecursiveOfType<T>() where T : DisassembledIlClass
        {
            //  Create a set of classes.
            var ilClasses = new List<T>();

            //  Go through each il class.
            foreach (var ilClass in lazyDisassembledIlClasses.Value)
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


        private readonly Lazy<List<DisassembledIlClass>> lazyDisassembledIlClasses;

        /// <summary>
        /// Gets or sets the assembly path.
        /// </summary>
        /// <value>
        /// The assembly path.
        /// </value>
        public string AssemblyPath { get; internal set; }

        /// <summary>
        /// Gets the classes.
        /// </summary>
        /// <value>
        /// The classes.
        /// </value>
        public IEnumerable<DisassembledClass> Classes { get { return lazyDisassembledIlClasses.Value.OfType<DisassembledClass>(); } }

        /// <summary>
        /// Gets the structures.
        /// </summary>
        /// <value>
        /// The structures.
        /// </value>
        public IEnumerable<DisassembledStructure> Structures { get { return lazyDisassembledIlClasses.Value.OfType<DisassembledStructure>(); } }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <value>
        /// The interfaces.
        /// </value>
        public IEnumerable<DisassembledInterface> Interfaces { get { return lazyDisassembledIlClasses.Value.OfType<DisassembledInterface>(); } }

        /// <summary>
        /// Gets the delegates.
        /// </summary>
        /// <value>
        /// The delegates.
        /// </value>
        public IEnumerable<DisassembledDelegate> Delegates { get { return lazyDisassembledIlClasses.Value.OfType<DisassembledDelegate>(); } }

        /// <summary>
        /// Gets the enumerations.
        /// </summary>
        /// <value>
        /// The enumerations.
        /// </value>
        public IEnumerable<DisassembledEnumeration> Enumerations { get { return lazyDisassembledIlClasses.Value.OfType<DisassembledEnumeration>(); } }
        
        /// <summary>
        /// Gets all methods, this is deep and recursive - it will get all methods in all classes, nested to any level.
        /// </summary>
        /// <value>
        /// All methods.
        /// </value>
        public IEnumerable<DisassembledMethod> AllMethods
        {
            get
            {
                var allMethods = new List<DisassembledMethod>();

                foreach(var @class in AllClasses)
                    allMethods.AddRange(@class.Methods);

                return allMethods;
            }
        }

        /// <summary>
        /// Gets all properties, this is deep and recursive - it will get all methods in all classes, nested to any level.
        /// </summary>
        /// <value>
        /// All properties.
        /// </value>
        public IEnumerable<DisassembledProperty> AllProperties
        {
            get
            {
                var allProperties = new List<DisassembledProperty>();

                foreach (var @class in AllClasses)
                    allProperties.AddRange(@class.Properties);

                return allProperties;
            }
        }

        /// <summary>
        /// Gets all fields, this is deep and recursive - it will get all methods in all classes, nested to any level.
        /// </summary>
        /// <value>
        /// All fields.
        /// </value>
        public IEnumerable<DisassembledField> AllFields
        {
            get
            {
                var allFields = new List<DisassembledField>();

                foreach (var @class in AllClasses)
                    allFields.AddRange(@class.Fields);

                return allFields;
            }
        }

        public IEnumerable<DisassembledEvent> AllEvents
        {
            get
            {
                var allFields = new List<DisassembledEvent>();

                foreach (var @class in AllClasses)
                    allFields.AddRange(@class.Events);

                return allFields;
            }
        }

        public IEnumerable<DisassembledClass> AllClasses { get { return GetIlClassesRecursiveOfType<DisassembledClass>(); } }

        public IEnumerable<DisassembledEnumeration> AllEnumerations { get { return GetIlClassesRecursiveOfType<DisassembledEnumeration>(); } }

        public IEnumerable<DisassembledStructure> AllStructures { get { return GetIlClassesRecursiveOfType<DisassembledStructure>(); } }

        public IEnumerable<DisassembledInterface> AllInterfaces { get { return GetIlClassesRecursiveOfType<DisassembledInterface>(); } }

        public IEnumerable<DisassembledDelegate> AllDelegates { get { return GetIlClassesRecursiveOfType<DisassembledDelegate>(); } }


        internal class RawILClass
        {
            public RawILClass()
            {
                ILBuilder = new StringBuilder();
                Children = new List<RawILClass>();
            }

            public string FullName { get; set; }
            public string TemplateSpecification { get; set; }
            public StringBuilder ILBuilder { get; private set; }
            public List<RawILClass> Children { get; private set; }
        }
    }
}
