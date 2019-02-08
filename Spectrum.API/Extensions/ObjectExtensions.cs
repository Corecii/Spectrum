using Spectrum.API.Reflection;
using System.Linq;
using System.Reflection;

namespace Spectrum.API.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetPrivateMember<T>(this object obj, MemberMetadata metadata, T value)
        {
            var flags = BindingFlags.NonPublic;

            if (!metadata.IsStatic)
                flags |= BindingFlags.Instance;

            if (metadata.IsProperty)
            {
                var propertyInfo = obj.GetType().GetProperty(metadata.MemberName, flags);

                if (propertyInfo == null) return;
                if (!propertyInfo.CanWrite) return;
                if (propertyInfo.GetIndexParameters().Any()) return;

                propertyInfo.SetValue(obj, value, null);
            }
            else
            {
                var fieldInfo = obj.GetType().GetField(metadata.MemberName, flags);

                if (fieldInfo == null) return;

                fieldInfo.SetValue(obj, value);
            }
        }
    }
}
