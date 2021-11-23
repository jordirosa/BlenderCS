using System;

namespace BlenderCS.Attributes
{
    public class StringPropertyAttribute : PropertyAttribute
    {
        private string _name;
        private string _description;
        private string _default;
        private int _maxLength;

        /// <summary>
        /// Name used in the user interface.
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// Text used for the tooltip and api documentation.
        /// </summary>
        public string Description { get => _description; set => _description = value; }
        /// <summary>
        /// initializer string.
        /// </summary>
        public string Default { get => _default; set => _default = value; }
        /// <summary>
        /// maximum length of the string.
        /// </summary>
        public int MaxLength { get => _maxLength; set => _maxLength = value; }
    }
}
