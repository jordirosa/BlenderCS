using System;

namespace BlenderCS.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OperatorAttribute : Attribute
    {
        public enum OptionsEnum
        {
            REGISTER,
            UNDO,
            UNDO_GROUPED,
            BLOCKING,
            MACRO,
            GRAB_CURSOR,
            GRAB_CURSOR_X,
            GRAB_CURSOR_Y,
            PRESET,
            INTERNAL
        }

        private const string FIELD_ID_NAME = "bl_idname";
        private const string FIELD_DESCRIPTION = "bl_description";
        private const string FIELD_LABEL = "bl_label";
        private const string FIELD_OPTIONS = "bl_options";

        private string _idName;
        private string _description;
        private string _label;
        private OptionsEnum[] _options;

        public string IDName { get => _idName; set => _idName = value; }
        public string Description { get => _description; set => _description = value; }
        public string Label { get => _label; set => _label = value; }
        public OptionsEnum[] Options { get => _options; set => _options = value; }

        public OperatorAttribute(string idName, string description, string label, OptionsEnum[] options)
        {
            IDName = idName;
            Description = description;
            Label = label;
            Options = options;
        }

        private string _optionsToSourceCode(OptionsEnum[] options)
        {
            string sourceCode = "{";
            bool first = true;
            foreach (OptionsEnum option in options)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sourceCode += ", ";
                }

                switch (option)
                {
                    case OptionsEnum.REGISTER:
                        sourceCode += "\"REGISTER\"";
                        break;
                    case OptionsEnum.UNDO:
                        sourceCode += "\"UNDO\"";
                        break;
                    case OptionsEnum.UNDO_GROUPED:
                        sourceCode += "\"UNDO_GROUPED\"";
                        break;
                    case OptionsEnum.BLOCKING:
                        sourceCode += "\"BLOCKING\"";
                        break;
                    case OptionsEnum.MACRO:
                        sourceCode += "\"MACRO\"";
                        break;
                    case OptionsEnum.GRAB_CURSOR:
                        sourceCode += "\"GRAB_CURSOR\"";
                        break;
                    case OptionsEnum.GRAB_CURSOR_X:
                        sourceCode += "\"GRAB_CURSOR_X\"";
                        break;
                    case OptionsEnum.GRAB_CURSOR_Y:
                        sourceCode += "\"GRAB_CURSOR_Y\"";
                        break;
                    case OptionsEnum.PRESET:
                        sourceCode += "\"PRESET\"";
                        break;
                    case OptionsEnum.INTERNAL:
                        sourceCode += "\"INTERNAL\"";
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            sourceCode += "}";
            return sourceCode;
        }

        public string BuildSourceCode()
        {
            string sourceCode = "";
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_ID_NAME, IDName);
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_DESCRIPTION, Description);
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_LABEL, Label);
            sourceCode += string.Format("\t{0} = {1}\n", FIELD_OPTIONS, _optionsToSourceCode(Options));

            return sourceCode;
        }
    }
}
