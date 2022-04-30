using System;
using Mapster;

namespace Lib.Helpers
{
    public class GenericMapper : IGenericMapper
    {
        public T Map<T>(object obj)
        {
            return obj == null ? default(T) : obj.Adapt<T>();
        }

        public object Map(object obj, Type outType)
        {
            return obj?.Adapt(obj.GetType(), outType);
        }
    }
}
