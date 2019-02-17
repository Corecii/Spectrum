using Spectrum.API.Reflection;
using System;
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
            else
                flags |= BindingFlags.Static;

            Type type = metadata.Type ?? obj.GetType();

            if (metadata.IsProperty)
            {
                var propertyInfo = type.GetProperty(metadata.MemberName, flags);

                if (propertyInfo == null) return;
                if (!propertyInfo.CanWrite) return;
                if (propertyInfo.GetIndexParameters().Any()) return;

                propertyInfo.SetValue(metadata.IsStatic ? null : obj, value, null);
            }
            else
            {
                var fieldInfo = type.GetField(metadata.MemberName, flags);

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
            else
                flags |= BindingFlags.Static;

            Type type = metadata.Type ?? obj.GetType();

            if (metadata.IsProperty)
            {
                var propertyInfo = type.GetProperty(metadata.MemberName, flags);

                if (propertyInfo == null) return default(T);
                if (!propertyInfo.CanRead) return default(T);
                if (propertyInfo.GetIndexParameters().Any()) return default(T);

                retval = (T)propertyInfo.GetValue(metadata.IsStatic ? null : obj, null);
            }
            else
            {
                var fieldInfo = type.GetField(metadata.MemberName, flags);

                if (fieldInfo == null) return default(T);

                retval = (T)fieldInfo.GetValue(metadata.IsStatic ? null : obj);
            }

            return retval;
        }

        public static void CallPrivateMethod(this object obj, MethodMetadata metadata, params object[] parameters)
        {
            var flags = BindingFlags.NonPublic;

            if (!metadata.IsStatic)
                flags |= BindingFlags.Instance;
            else
                flags |= BindingFlags.Static;

            MethodInfo methodInfo;
            
            if (metadata.Types != null)
                methodInfo = obj.GetType().GetMethod(metadata.Name, flags, null, metadata.Types, null);
            else
                methodInfo = obj.GetType().GetMethod(metadata.Name, flags);

            if (methodInfo == null) return;

            methodInfo.Invoke(metadata.IsStatic ? null : obj, parameters);

            Console.WriteLine($"Invoked method {metadata.Name} on {obj.GetType()} (non-generic).");
        }

        public static void CallPrivateGenericMethod<T>(this object obj, MethodMetadata metadata, params object[] parameters)
        {
            var flags = BindingFlags.NonPublic;

            if (!metadata.IsStatic)
                flags |= BindingFlags.Instance;
            else
                flags |= BindingFlags.Static;

            MethodInfo methodInfo;

            if (metadata.Types != null)
                methodInfo = obj.GetType().GetMethod(metadata.Name, flags, null, metadata.Types, null);
            else
                methodInfo = obj.GetType().GetMethod(metadata.Name, flags);

            if (methodInfo == null) return;

            var genericMethod = methodInfo.MakeGenericMethod(typeof(T));
            genericMethod.Invoke(metadata.IsStatic ? null : obj, parameters);

            Console.WriteLine($"Invoked method {metadata.Name} on {obj.GetType()} for generic type {typeof(T)}.");
        }
    }
}
