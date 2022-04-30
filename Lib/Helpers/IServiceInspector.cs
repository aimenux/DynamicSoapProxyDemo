using System.Collections.Generic;
using System.Reflection;

namespace Lib.Helpers
{
    public interface IServiceInspector
    {
        bool HasMethod(string methodName, out MethodInfo method);

        ICollection<ParameterInfo> GetMethodParameters(string methodName);

        object RunMethod(string methodName, params object[] passedParams);
    }
}
