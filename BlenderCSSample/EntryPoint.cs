using BlenderCS;

using Python.Runtime;

namespace BlenderCSSample
{
    public class EntryPoint : BlenderAddonEntryPoint
    {
        public override void RegisterPostLogic(Py.GILState python)
        {
            dynamic builtins = Py.Import("builtins");

            builtins.exec("bpy.types.Scene.blender_cs = bpy.props.PointerProperty(type = TestData)");
        }

        public override void UnregisterPostLogic(Py.GILState python)
        {
            dynamic builtins = Py.Import("builtins");

            builtins.exec("del bpy.types.Scene.blender_cs");
        }
    }
}
