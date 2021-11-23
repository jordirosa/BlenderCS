using BlenderCS.Attributes;

namespace BlenderCSSample.Data
{
    [PropertyGroup]
    public class TestData
    {
        private string _testStringProperty;

        [StringProperty(Name = "TestStringProperty")]
        public string TestStringProperty { get => _testStringProperty; set => _testStringProperty = value; }
    }
}
