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

                propertyInfo.SetValue(metadata.IsStatic ? null : obj, value, null);
            }
            else
            {
                var fieldInfo = obj.GetType().GetField(metadata.MemberName, flags);

                if (fieldInfo == null) return;

                fieldInfo.SetValue(metadata.IsStatic ? null : obj, value);
            }
        }

        public static T GetPrivateMember<T>(this object obj, MemberMetadata metadata)
        {
            var flags = BindingFlags.NonPublic;
            T retval;

            if (!metadata.IsStatic)
                flags |= BindingFlags.Instance;

            if (metadata.IsProperty)
            {
                var propertyInfo = obj.GetType().GetProperty(metadata.MemberName, flags);

                if (propertyInfo == null) return default(T);
                if (!propertyInfo.CanRead) return default(T);
                if (propertyInfo.GetIndexParameters().Any()) return default(T);

                retval = (T)propertyInfo.GetValue(metadata.IsStatic ? null : obj, null);
            }
            else
            {
                var fieldInfo = obj.GetType().GetField(metadata.MemberName, flags);

                if (fieldInfo == null) return default(T);

                retval = (T)fieldInfo.GetValue(metadata.IsStatic ? null : obj);
            }

            return retval;
        }
    }
}
