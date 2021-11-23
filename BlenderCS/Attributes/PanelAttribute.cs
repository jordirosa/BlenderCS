using System;

namespace BlenderCS.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PanelAttribute : Attribute
    {
        public enum SpaceTypeEnum
        {
            VIEW_3D
        }

        public enum RegionTypeEnum
        {
            UI
        }

        private const string FIELD_ID_NAME = "bl_idname";
        private const string FIELD_LABEL = "bl_label";
        private const string FIELD_SPACE_TYPE = "bl_space_type";
        private const string FIELD_REGION_TYPE = "bl_region_type";
        private const string FIELD_CATEGORY = "bl_category";

        private string _idName;
        private string _label;
        private SpaceTypeEnum _spaceType;
        private RegionTypeEnum _regionType;
        private string _category;

        public string IDName { get => _idName; set => _idName = value; }
        public string Label { get => _label; set => _label = value; }
        public SpaceTypeEnum SpaceType { get => _spaceType; set => _spaceType = value; }
        public RegionTypeEnum RegionType { get => _regionType; set => _regionType = value; }
        public string Category { get => _category; set => _category = value; }

        public PanelAttribute(string idName, string label, SpaceTypeEnum spaceType, RegionTypeEnum regionType, string category)
        {
            IDName = idName;
            Label = label;
            SpaceType = spaceType;
            RegionType = regionType;
            Category = category;
        }

        private string _spaceTypeToSourceCode(SpaceTypeEnum spaceType)
        {
            switch(spaceType)
            {
                case SpaceTypeEnum.VIEW_3D:
                    return "VIEW_3D";
                default:
                    throw new ArgumentException();
            }
        }

        private string _regionTypeToSourceCode(RegionTypeEnum regionType)
        {
            switch (regionType)
            {
                case RegionTypeEnum.UI:
                    return "UI";
                default:
                    throw new ArgumentException();
            }
        }

        public string BuildSourceCode()
        {
            string sourceCode = "";
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_ID_NAME, IDName);
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_LABEL, Label);
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_SPACE_TYPE, _spaceTypeToSourceCode(SpaceType));
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_REGION_TYPE, _regionTypeToSourceCode(RegionType));
            sourceCode += string.Format("\t{0} = \"{1}\"\n", FIELD_CATEGORY, Category);

            return sourceCode;
        }
    }
}
