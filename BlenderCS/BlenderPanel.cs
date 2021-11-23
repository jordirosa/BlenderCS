using System;

using Python.Runtime;

namespace BlenderCS
{
    public abstract class BlenderPanel
    {
        public void Draw(dynamic self, dynamic context)
        {
            Py.GILState python = Py.GIL();

            try
            { 
                DrawLogic(python, self, context);
            }
            finally
            {
                python.Dispose();
            }
        }

        public virtual void DrawLogic(Py.GILState python, dynamic self, dynamic context) { }
    }
}
