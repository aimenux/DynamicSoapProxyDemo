using System.Collections.Generic;
using System.Reflection;

namespace Lib.Services
{
    public interface ISoapService
    {
        ICollection<MethodInfo> GetMethodInfos();

        object RunMethod(string methodName, params object[] passedParams);
    }
}