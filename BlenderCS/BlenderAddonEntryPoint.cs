using Python.Runtime;

using BlenderCS.Attributes;

namespace BlenderCS
{
    [EntryPointClass]
    public abstract class BlenderAddonEntryPoint
    {
        public void RegisterPre()
        {
            Py.GILState python = Py.GIL();

            RegisterPreLogic(python);

            python.Dispose();
        }

        public void RegisterPost()
        {
            Py.GILState python = Py.GIL();

            RegisterPostLogic(python);

            python.Dispose();
        }

        public void UnregisterPre()
        {
            Py.GILState python = Py.GIL();

            UnregisterPreLogic(python);

            python.Dispose();
        }

        public void UnregisterPost()
        {
            Py.GILState python = Py.GIL();

            UnregisterPostLogic(python);

            python.Dispose();
        }

        public virtual void RegisterPreLogic(Py.GILState python) { }
        public virtual void RegisterPostLogic(Py.GILState python) { }
        public virtual void UnregisterPreLogic(Py.GILState python) { }
        public virtual void UnregisterPostLogic(Py.GILState python) { }
    }
}
