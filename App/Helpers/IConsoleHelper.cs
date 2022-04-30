using System;
using System.Collections.Generic;
using System.Reflection;

namespace App.Helpers
{
    public interface IConsoleHelper
    {
        void RenderTitle(string text);

        void RenderInfos(ICollection<MethodInfo> methods);

        void RenderException(Exception exception);
    }
}
