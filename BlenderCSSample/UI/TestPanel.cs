using BlenderCS;
using BlenderCS.Attributes;
using Python.Runtime;

namespace BlenderCSSample.UI
{
    [Panel("cstest.test_panel", "Test", PanelAttribute.SpaceTypeEnum.VIEW_3D, PanelAttribute.RegionTypeEnum.UI, "C# Test")]
    public class TestPanel : BlenderPanel
    {
        public override void DrawLogic(Py.GILState python, dynamic self, dynamic context)
        {
            dynamic builtins = Py.Import("builtins");

            builtins.exec("self.layout.operator(operator=\"cstest.test_operator\", text=\"Testing\")");
        }
    }
}
