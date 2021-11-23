using System;

namespace BlenderCS.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AddonInfoAttribute : Attribute
    {
        public enum SupportEnum
        {
            /// <summary>
            /// Officially supported
            /// </summary>
            OFFICIAL,
            /// <summary>
            /// Maintained by community developers
            /// </summary>
            COMMUNITY,
            /// <summary>
            /// Newly contributed scripts (excluded from release builds)
            /// </summary>
            TESTING
        }

        public enum CategoryEnum
        {
            VIEW_3D,
            ADD_MESH,
            ADD_CURVE,
            ANIMATION,
            COMPOSITING,
            DEVELOPMENT,
            GAME_ENGINE,
            IMPORT_EXPORT,
            LIGHTING,
            MATERIAL,
            MESH,
            NODE,
            OBJECT,
            PAINT,
            PHYSICS,
            RENDER,
            RIGGING,
            SCENE,
            SEQUENCER,
            SYSTEM,
            TEXT_EDITOR,
            UV,
            USER_INTERFACE,
            CUSTOM
        }

        private const string DICT_FIELD_NAME = "name";
        private const string DICT_FIELD_DESCRIPTION = "description";
        private const string DICT_FIELD_AUTHOR = "author";
        private const string DICT_FIELD_VERSION = "version";
        private const string DICT_FIELD_BLENDER_VERSION = "blender";
        private const string DICT_FIELD_LOCATION = "location";
        private const string DICT_FIELD_WARNING = "warning";
        private const string DICT_FIELD_DOC_URL = "doc_url";
        private const string DICT_FIELD_TRACKER_URL = "tracker_url";
        private const string DICT_FIELD_SUPPORT = "support";
        private const string DICT_FIELD_CATEGORY = "category";

        private string _name;
        private string _description;
        private string _author;
        private int _versionMajor;
        private int _versionMinor;
        private int _blenderVersionMajor;
        private int _blenderVersionMinor;
        private string _location;
        private string _warning;
        private SupportEnum _support;
        private string _docURL;
        private string _trackerURL;
        private CategoryEnum _category;
        private string _categoryCustomText;

        /// <summary>
        /// Name of the script. This will be displayed in the add-ons menu as the main entry.
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// This short text helps the user to decide if he needs the addon when he reads the addons list. Any further help text should be put in the help page (see doc_url).
        /// </summary>
        public string Description { get => _description; set => _description = value; }
        /// <summary>
        /// Author name(s).
        /// </summary>
        public string Author { get => _author; set => _author = value; }
        /// <summary>
        /// Script version (Major).
        /// </summary>
        public int VersionMajor { get => _versionMajor; set => _versionMajor = value; }
        /// <summary>
        /// Script version. (Minor)
        /// </summary>
        public int VersionMinor { get => _versionMinor; set => _versionMinor = value; }
        /// <summary>
        /// Blender version. (Major)
        /// The minimum Blender version required to run the script. In the add-ons panel this is used to check if the user has a Blender version that is new enough to enable the script.
        /// </summary>
        public int BlenderVersionMajor { get => _blenderVersionMajor; set => _blenderVersionMajor = value; }
        /// <summary>
        /// Blender version. (Minor)
        /// The minimum Blender version required to run the script. In the add-ons panel this is used to check if the user has a Blender version that is new enough to enable the script.
        /// When the version number ends in zero, you need to supply that 0 too, as in 2.50 and not 2.5.
        /// </summary>
        public int BlenderVersionMinor { get => _blenderVersionMinor; set => _blenderVersionMinor = value; }
        /// <summary>
        /// Explains where the new functionality can be found. For example: "View3D > Properties > Measure"
        /// </summary>
        public string Location { get => _location; set => _location = value; }
        /// <summary>
        /// Used for warning icon and text in addons panel. If this is empty you will see nothing; if it is non-empty, you will see a warning icon and the text you put here alerts the user about a bug or a problem to be aware of.
        /// </summary>
        public string Warning { get => _warning; set => _warning = value; }
        /// <summary>
        /// Display support level (COMMUNITY by default)
        /// </summary>
        public SupportEnum Support { get => _support; set => _support = value; }
        /// <summary>
        /// Link to the documentation page of the script: here you must put the script manual.
        /// This url is mandatory for the add-on to be accepted in the Blender Addon repositories
        /// </summary>
        public string DocURL { get => _docURL; set => _docURL = value; }
        /// <summary>
        /// Optional field to specify a bug tracker other than the default https://developer.blender.org
        /// </summary>
        public string TrackerURL { get => _trackerURL; set => _trackerURL = value; }
        /// <summary>
        /// Defines the group to which the script belongs. Only one value is allowed in this field, multiple categories are not supported. The category is used for filtering in the add-ons panel.
        /// </summary>
        public CategoryEnum Category { get => _category; set => _category = value; }
        /// <summary>
        /// When Category has CUSTOM as value you can specify the desired text
        /// </summary>
        public string CategoryCustomText { get => _categoryCustomText; set => _categoryCustomText = value; }

        public AddonInfoAttribute(string name, string description, string author, int versionMajor, int versionMinor, int blenderVersionMajor, int blenderVersionMinor, CategoryEnum category)
        {
            Name = name;
            Description = description;
            Author = author;
            VersionMajor = versionMajor;
            VersionMinor = versionMinor;
            BlenderVersionMajor = blenderVersionMajor;
            BlenderVersionMinor = blenderVersionMinor;
            Support = SupportEnum.COMMUNITY;
            Category = category;
        }

        private string _supportToSourceCode(SupportEnum support)
        {
            switch(support)
            {
                case SupportEnum.OFFICIAL:
                    return "OFFICIAL";
                case SupportEnum.COMMUNITY:
                    return "COMMUNITY";
                case SupportEnum.TESTING:
                    return "TESTING";
                default:
                    throw new ArgumentException();
            }
        }

        private string _categoryToSourceCode(CategoryEnum category)
        {
            switch(category)
            {
                case CategoryEnum.VIEW_3D:
                    return "3D View";
                case CategoryEnum.ADD_MESH:
                    return "Add Mesh";
                case CategoryEnum.ADD_CURVE:
                    return "Add Curve";
                case CategoryEnum.ANIMATION:
                    return "Animation";
                case CategoryEnum.COMPOSITING:
                    return "Compositing";
                case CategoryEnum.DEVELOPMENT:
                    return "Development";
                case CategoryEnum.GAME_ENGINE:
                    return "Game Engine";
                case CategoryEnum.IMPORT_EXPORT:
                    return "Import-Export";
                case CategoryEnum.LIGHTING:
                    return "Lighting";
                case CategoryEnum.MATERIAL:
                    return "Material";
                case CategoryEnum.MESH:
                    return "Mesh";
                case CategoryEnum.NODE:
                    return "Node";
                case CategoryEnum.OBJECT:
                    return "Object";
                case CategoryEnum.PAINT:
                    return "Paint";
                case CategoryEnum.PHYSICS:
                    return "Physics";
                case CategoryEnum.RENDER:
                    return "Render";
                case CategoryEnum.RIGGING:
                    return "Rigging";
                case CategoryEnum.SCENE:
                    return "Scene";
                case CategoryEnum.SEQUENCER:
                    return "Sequencer";
                case CategoryEnum.SYSTEM:
                    return "System";
                case CategoryEnum.TEXT_EDITOR:
                    return "Text Editor";
                case CategoryEnum.UV:
                    return "UV";
                case CategoryEnum.USER_INTERFACE:
                    return "User Interface";
                case CategoryEnum.CUSTOM:
                    return CategoryCustomText;
                default:
                    throw new ArgumentException();
            }
        }

        public string BuildSourceCode()
        {
            string sourceCode = "bl_info = {\n";
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_NAME, Name);
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_DESCRIPTION, Description);
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_AUTHOR, Author);
            sourceCode += string.Format("\t\"{0}\": ({1}, {2}, 0),\n", DICT_FIELD_VERSION, VersionMajor, VersionMinor);
            sourceCode += string.Format("\t\"{0}\": ({1}, {2}, 0),\n", DICT_FIELD_BLENDER_VERSION, BlenderVersionMajor, BlenderVersionMinor);
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_LOCATION, Location);
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_WARNING, Warning);
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_DOC_URL, DocURL);
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_TRACKER_URL, TrackerURL);
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_SUPPORT, _supportToSourceCode(Support));
            sourceCode += string.Format("\t\"{0}\": \"{1}\",\n", DICT_FIELD_CATEGORY, _categoryToSourceCode(Category));
            sourceCode += "}";

            return sourceCode;
        }
    }
}
