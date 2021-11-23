using BlenderCS;
using BlenderCS.Attributes;
using Python.Runtime;

namespace BlenderCSSample.Operators
{
    [Operator("cstest.test_operator", "test operator", "Test operator", new[] { OperatorAttribute.OptionsEnum.REGISTER, OperatorAttribute.OptionsEnum.UNDO })]
    public class TestOperator : BlenderOperator
    {
        public override ResultEnum ExecuteLogic(Py.GILState python, dynamic self, dynamic context)
        {
            Report("Aqui estoy de nuevo!!!");

            return ResultEnum.FINISHED;
        }
    }
}
