using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public static class ILParseHelper
    {
        public static bool IsLineTopLevelIlClassDeclaration(string line)
        {
            return line.StartsWith(Token_StartClass);
        }

        public static bool IsLineAnyLevelIlClassDeclaration(string line)
        {
            return line.TrimStart().StartsWith(Token_StartClass);
        }

        public static bool IsLineClassDeclaration(string line)
        {
            //  If the line doesn't start with the class token, it's not a class.
            if (!line.StartsWith(Token_StartClass))
                return false;

            //  Get the declaration parts.
            string className, templateSpecification;
            List<string> modifiers;
            GetClassDeclarationParts(line, out modifiers, out className, out templateSpecification);

            //  We're a class if we're not an interface or struct.
            return !modifiers.Contains(Token_Structure) && !modifiers.Contains(Token_Interface);
        }

        public static void GetClassDeclarationParts(string line, out List<string> modifiers, out string className, out string templateSpecification)
        {
            //  Get the class name and template spec.
            GetClassNameFromClassDeclarationLine(line, out className, out templateSpecification);

            //  Remove the parts we've handled.
            if (className != null)
                line = line.Replace(className, "");
            if (templateSpecification != null)
                line = line.Replace(templateSpecification, "");

            //  Break up the line.
            var parts = line.Split(' ').ToList();
            
            if(parts.Count < 2)
                throw new InvalidOperationException("Invalid class declaration line.");

            //  Remove the class part.
            parts.RemoveAt(0);

            //  Get the classname.
            parts.RemoveAt(parts.Count - 1);
            modifiers = parts;
        }

        public static bool IsLineStructDeclaration(string line)
        {
            line = line.TrimStart();

            //  If the line doesn't start with the class token, it's not a struct.
            if (!line.StartsWith(Token_StartClass))
                return false;

            //  Get the declaration parts.
            string className, templateSpecification;
            List<string> modifiers;
            GetClassDeclarationParts(line, out modifiers, out className, out templateSpecification);

            return modifiers.Contains(Token_Structure) && !modifiers.Contains(Token_Interface);
        }

        public static bool IsLineInterfaceDeclaration(string line)
        {
            line = line.TrimStart();

            //  If the line doesn't start with the class token, it's not an interface.
            if (!line.StartsWith(Token_StartClass))
                return false;

            //  Get the declaration parts.
            string className, templateSpecification;
            List<string> modifiers;
            GetClassDeclarationParts(line, out modifiers, out className, out templateSpecification);

            return !modifiers.Contains(Token_Structure) && modifiers.Contains(Token_Interface);
        }
        
        public static bool ReadExtendsLine(string line, out string baseType)
        {
            baseType = null;

            //  Trim the line.
            var trimmed = line.Trim();

            //  Split into tokens.
            var tokens = trimmed.Split(' ').ToList();

            //  We must have at least two tokens.
            if (tokens.Count < 2)
                return false;

            //  The first token must be 'extends'.
            if (tokens[0] != Token_Extends)
                return false;

            //  The last token is the base type.
            baseType = tokens.Last();
            return true;
        }

        public static bool IsLineStartClassEndToken(string line)
        {
            return line.StartsWith(Token_EndClass);
        }

        public static bool IsLineClassEndDeclaration(string line, string className)
        {
            var classEnd = Token_EndClass + " " + className;
            return line.TrimStart().StartsWith(classEnd);
        }

        public static bool IsLineFieldDeclaration(string line)
        {
            return line.TrimStart().StartsWith(Token_Field);
        }

        public static void GetClassNameFromClassDeclarationLine(string line, out string className, out string templateSpecification)
        {
            className = null;
            //  First, check for anonymous classes.
            if (line.Contains("'"))
            {
                var from = line.IndexOf('\'');
                var to = line.LastIndexOf('\'');
                className = line.Substring(from, (to + 1) - from);
                line = line.Replace(className, string.Empty);
            }

            //  Strip out the template specification, if there is on.
            templateSpecification = null;
            if (line.Contains("<"))
            {
                var from = line.IndexOf('<');
                var to = line.LastIndexOf('>');

                templateSpecification = line.Substring(from, (to + 1) - from);
                line = line.Replace(templateSpecification, string.Empty);
            }

            //  The class name is the last word, unless we've already got it.
            if (className == null)
            {
                var lastSpace = line.LastIndexOf(' ');
                className = line.Substring(lastSpace + 1);
            }
        }

        public static bool IsLineMethodDeclaration(string line)
        {
            return line.TrimStart().StartsWith(Token_Method);
        }

        private const string Token_StartClass = @".class";
        private const string Token_Field = @".field";
        private const string Token_Method = @".method";
        private const string Token_Property = @".property";
        private const string Token_Event = @".event";
        private const string Token_Structure = @"sequential";
        private const string Token_Interface = @"interface";
        private const string Token_EndClass = @"} // end of class";
        private const string Token_EndMethod = @"} // end of method";
        private const string Token_EndProperty = @"} // end of property";
        private const string Token_EndEvent = @"} // end of event";
        private const string Token_Extends = @"extends";
        private const string Token_SourceComment = @"//";

        public static string GetMethodNameFromDeclarationLine(string line)
        {
            //   e.g. get_Species() cil managed
            var methodPart = line.Trim().Split(' ').FirstOrDefault(mp => mp.Contains('('));
            if (methodPart == null)
                return null;
            return methodPart.Substring(0, methodPart.IndexOf('('));
        }

        public static bool IsLineMethodEndDeclaration(string line, string ilClassName, string methodName)
        {
            //  Quick check - if we don't have 'end of method', we don't need to check in detail.
            if (line.Contains(Token_EndMethod) == false)
                return false;

            //  If the method contains a template specification, remove it before we check the 
            //  end line, because the end line always is the name without the template spec.
            if (methodName != null && methodName.Contains('<'))
                methodName = methodName.Substring(0, methodName.IndexOf('<'));

            var classEnd = Token_EndMethod + " " + ilClassName + "::" + methodName;
            return line.Contains(classEnd);
        }

        public static bool IsLinePropertyDeclaration(string line)
        {
            return line.TrimStart().StartsWith(Token_Property);
        }
        public static bool IsLineEventPropertyDeclaration(string line)
        {
            return line.TrimStart().StartsWith(Token_Event);
        }
        public static bool IsLinePropertyEndDeclaration(string line, string ilClassName, string propertyName)
        {
            var classEnd = Token_EndProperty + " " + ilClassName + "::" + propertyName;
            return line.Contains(classEnd);
        }
        public static bool IsLineEventEndDeclaration(string line, string ilClassName, string propertyName)
        {
            var classEnd = Token_EndEvent + " " + ilClassName + "::" + propertyName;
            return line.Contains(classEnd);
        }


        public static string GetPropertyNameFromDeclarationLine(string line)
        {
            var propertyPart = line.Trim().Split(' ').FirstOrDefault(mp => mp.Contains('('));
            if (propertyPart == null)
                return null;
            return propertyPart.Substring(0, propertyPart.IndexOf('('));
        }

        public static string GetEventNameFromDeclarationLine(string line)
        {
            var propertyPart = line.Trim().Split(' ').Last();
            return propertyPart;
        }

        public static bool IsLineSourceComment(string line)
        {
            return line.StartsWith(Token_SourceComment);
        }
    }
}
