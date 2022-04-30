using System;

namespace Lib.Helpers
{
    public interface IGenericMapper
    {
        T Map<T>(object obj);

        object Map(object obj, Type outType);
    }
}
