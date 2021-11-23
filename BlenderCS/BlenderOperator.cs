using System;

using Python.Runtime;

namespace BlenderCS
{
    public abstract class BlenderOperator
    {
        public enum ResultEnum
        {
            RUNNING_MODAL,
            CANCELLED,
            FINISHED,
            PASS_THROUGH,
            INTERFACE
        }

        private dynamic _self;

        public string Execute(dynamic self, dynamic context)
        {
            Py.GILState python = Py.GIL();

            ResultEnum result = ResultEnum.CANCELLED;
            try
            {
                _self = self;
                result = ExecuteLogic(python, self, context);
            }
            finally
            {
                python.Dispose();
            }

            return _translateResultToPython(result);
        }

        public virtual ResultEnum ExecuteLogic(Py.GILState python, dynamic self, dynamic context)
        {
            return ResultEnum.FINISHED;
        }
        public void Report(string message)
        {
            dynamic builtins = Py.Import("builtins");
            builtins.exec(string.Format("self.report({0}, \"{1}\")", "{ \"INFO\" }", message));
        }

        private string _translateResultToPython(ResultEnum result)
        {
            switch (result)
            {
                case ResultEnum.RUNNING_MODAL:
                    return "RUNNING_MODAL";
                case ResultEnum.CANCELLED:
                    return "CANCELLED";
                case ResultEnum.FINISHED:
                    return "FINISHED";
                case ResultEnum.PASS_THROUGH:
                    return "PASS_THROUGH";
                case ResultEnum.INTERFACE:
                    return "INTERFACE";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
