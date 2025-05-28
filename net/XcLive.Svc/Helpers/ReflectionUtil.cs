#nullable disable
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using ServiceStack;

namespace SchemaBuilder.Svc.Helpers
{
    /// <summary>反射实用工具集。</summary>
    public static class ReflectionUtil
    {
        public static bool ImplementsInterface(this Type objType, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(objType);
        }

        public static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetDescription(this MemberInfo info)
        {
            var att = info.FirstAttribute<DescriptionAttribute>();
            if (att != null)
            {
                return att.Description;
            }

            return "";
        }

        private static readonly ConcurrentDictionary<string, object[]> Caches =
            new ConcurrentDictionary<string, object[]>();

        /// <summary>生成指定类型的实例。</summary>
        /// <remarks>
        ///     <paramref name="args" />的数量决定构造方法的选择。
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>(this Type type, params object[] args)
        {
            return (T)type.CreateInstanceEx(args);
        }

        public static object CreateInstanceEx(this Type type, params object[] args)
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            ConstructorInfo[] source = !type.IsArray
                ? type.GetConstructors()
                : throw new NotImplementedException("请使用Array.CreateInstance创建数组对象");
            return args == null ||
                   ((IEnumerable<ConstructorInfo>)source).All<ConstructorInfo>(
                       (Func<ConstructorInfo, bool>)(ctor => ctor.GetParameters().Length != args.Length))
                ? Activator.CreateInstance(type)
                : Activator.CreateInstance(type, args);
        }

        /// <summary>检测泛型类是否实现了某一泛型接口。</summary>
        /// <param name="genericInterfaceType">泛型接口。</param>
        /// <param name="genericImplType">泛型实现。</param>
        /// <returns>实现了给定的泛型接口则返回"true"。</returns>
        public static bool IsAssignableFromGeneric(this Type genericInterfaceType, Type genericImplType)
        {
            if (((IEnumerable<Type>)genericImplType.GetInterfaces())
                .Where<Type>((Func<Type, bool>)(it => it.IsGenericType))
                .Any<Type>((Func<Type, bool>)(it => it.GetGenericTypeDefinition() == genericInterfaceType)))
                return true;
            Type baseType = genericImplType.BaseType;
            return !(baseType == (Type)null) &&
                   (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericInterfaceType ||
                    genericInterfaceType.IsAssignableFromGeneric(baseType));
        }

        /// <summary>根据程序集名全称获取程序集。</summary>
        /// <param name="assemblyName">程序集名全称</param>
        /// <returns>程序集。</returns>
        public static System.Reflection.Assembly GetAssemblyNamed(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));
            try
            {
                string directoryName = Path.GetDirectoryName(assemblyName);
                return string.IsNullOrEmpty(directoryName) || directoryName == AppDomain.CurrentDomain.BaseDirectory
                    ? System.Reflection.Assembly.Load(assemblyName)
                    : System.Reflection.Assembly.LoadFile(assemblyName);
            }
            catch (FileNotFoundException ex)
            {
                throw;
            }
            catch (FileLoadException ex)
            {
                throw;
            }
            catch (BadImageFormatException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("无法装载名称为“{0}”的程序集。", (object)assemblyName), ex);
            }
        }

        /// <summary>读取方法上的标签。</summary>
        /// <typeparam name="TAttribute">标签类型。</typeparam>
        /// <param name="methodInfo">方法信息。</param>
        /// <param name="callbacker">回调方法。</param>
        public static void ReadMethodAttribute<TAttribute>(
            this MethodInfo methodInfo,
            Action<TAttribute> callbacker)
        {
            Debug.Assert(methodInfo.DeclaringType != (Type)null, "methodInfo.DeclaringType != null");
            string key = string.Format("{0}.{1}", (object)methodInfo.DeclaringType.FullName, (object)methodInfo.Name);
            foreach (TAttribute attribute in ReflectionUtil.Caches.GetOrAdd(key,
                         (Func<string, object[]>)(type =>
                             methodInfo.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>()
                                 .Cast<object>().ToArray<object>())).Cast<TAttribute>())
                callbacker(attribute);
        }

        /// <summary>读取枚举上的标签集。</summary>
        /// <typeparam name="TEnum">枚举类型。</typeparam>
        /// <typeparam name="TAttribute">标签类型。</typeparam>
        /// <returns></returns>
        public static IDictionary<TEnum, TAttribute> ReadEnumAttributes<TEnum, TAttribute>()
            where TAttribute : Attribute
        {
            return (IDictionary<TEnum, TAttribute>)
                ((IEnumerable<FieldInfo>)typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public |
                                                                 BindingFlags.GetField))
                .ToDictionary<FieldInfo, TEnum, TAttribute>(
                    (Func<FieldInfo, TEnum>)(field =>
                        (TEnum)Convert.ChangeType(field.GetValue((object)null), Enum.GetUnderlyingType(typeof(TEnum)))),
                    (Func<FieldInfo, TAttribute>)(field =>
                        field.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>()
                            .FirstOrDefault<TAttribute>(
                                (Func<TAttribute, bool>)(attribute => (object)attribute != null))));
        }

        /// <summary>
        ///     读取枚举上的标签集并对每个标签调用<paramref name="callbacker" />方法。
        /// </summary>
        /// <typeparam name="TEnum">枚举类型。</typeparam>
        /// <typeparam name="TAttribute">标签类型。</typeparam>
        /// <param name="callbacker">回调方法。</param>
        public static void ReadEnumAttributes<TEnum, TAttribute>(Action<TEnum, TAttribute> callbacker)
            where TAttribute : Attribute
        {
            if (callbacker == null)
                throw new ArgumentNullException(nameof(callbacker));
            foreach (KeyValuePair<TEnum, TAttribute> keyValuePair in ReflectionUtil
                         .ReadEnumAttributes<TEnum, TAttribute>().Where<KeyValuePair<TEnum, TAttribute>>(
                             (Func<KeyValuePair<TEnum, TAttribute>, bool>)(attributePair =>
                                 (object)attributePair.Value != null)))
                callbacker(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>读取枚举上的标签。</summary>
        /// <typeparam name="TAttribute">标签类型。</typeparam>
        /// <param name="enumValue">枚举值。</param>
        /// <returns>标签实例。</returns>
        public static TAttribute ReadEnumAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            return enumValue != null
                ? enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(typeof(TAttribute), true)
                    .OfType<TAttribute>()
                    .FirstOrDefault<TAttribute>((Func<TAttribute, bool>)(attribute => (object)attribute != null))
                : throw new ArgumentNullException(nameof(enumValue));
        }

        /// <summary>获取属性特性</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<TAttribute, PropertyInfo> ReadPropertyAttributes<TAttribute>(
            this Type type)
            where TAttribute : Attribute
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            Dictionary<TAttribute, PropertyInfo> re = new Dictionary<TAttribute, PropertyInfo>();
            type.GetProperties().ForEach<PropertyInfo>((Action<PropertyInfo>)(mem =>
            {
                List<TAttribute> source = mem.ReadAttributes<TAttribute>();
                if (!source.Has<TAttribute>())
                    return;
                source.ForEach((Action<TAttribute>)(att => re[att] = mem));
            }));
            return re;
        }

        /// <summary>获取属性特性</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<PropertyInfo, TAttribute> ReadPropertyAttribute<TAttribute>(
            this Type type)
            where TAttribute : Attribute
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            Dictionary<PropertyInfo, TAttribute> re = new Dictionary<PropertyInfo, TAttribute>();
            type.GetProperties().ForEach<PropertyInfo>((Action<PropertyInfo>)(mem =>
            {
                List<TAttribute> source = mem.ReadAttributes<TAttribute>();
                if (!source.Any<TAttribute>())
                    return;
                re[mem] = source.FirstOrDefault<TAttribute>();
            }));
            return re;
        }

        /// <summary>获取字段特性</summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<TAttribute, FieldInfo> ReadFieldAttributes<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            Dictionary<TAttribute, FieldInfo> re = new Dictionary<TAttribute, FieldInfo>();
            type.GetFields().ForEach<FieldInfo>((Action<FieldInfo>)(mem =>
            {
                List<TAttribute> source = mem.ReadAttributes<TAttribute>();
                if (!source.Has<TAttribute>())
                    return;
                source.ForEach((Action<TAttribute>)(att => re[att] = mem));
            }));
            return re;
        }

        public static Dictionary<FieldInfo, TAttribute> ReadFieldAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            Dictionary<FieldInfo, TAttribute> re = new Dictionary<FieldInfo, TAttribute>();
            type.GetFields().ForEach<FieldInfo>((Action<FieldInfo>)(mem =>
            {
                List<TAttribute> source = mem.ReadAttributes<TAttribute>();
                if (!source.Any<TAttribute>())
                    return;
                re[mem] = source.FirstOrDefault<TAttribute>();
            }));
            return re;
        }

        public static List<TAttribute> ReadAttributes<TAttribute>(this MemberInfo propertyInfo)
            where TAttribute : Attribute
        {
            return !(propertyInfo == (MemberInfo)null)
                ? propertyInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>()
                    .Where<TAttribute>((Func<TAttribute, bool>)(att => (object)att != null)).ToList<TAttribute>()
                : throw new ArgumentNullException(nameof(propertyInfo));
        }

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            return obj.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("source",
                    "Unable to convert object to a dictionary. The source object is null.");
            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj))
            {
                object obj1 = ReflectionUtil.GetValue(property.GetValue(obj), property.PropertyType);
                if (ReflectionUtil.IsOfType<T>(obj1))
                    dictionary.Add(property.Name, (T)obj1);
            }

            return (IDictionary<string, T>)dictionary;
        }

        private static object GetValue(object val, Type type)
        {
            if (type.IsEnum)
                return (object)(int)(val ?? (object)0);
            if (type == typeof(string) || type.GetInterface(typeof(IEnumerable).Name) != typeof(IEnumerable))
                return val;
            object[] array = ((IEnumerable)val).Cast<object>().ToArray<object>();
            Type itemType = ReflectionUtil.GetItemType((IEnumerable<object>)array);
            if (itemType == (Type)null)
                return val;
            if (itemType.IsEnum)
                array = ((IEnumerable<object>)array).Select<object, object>((Func<object, object>)(s => (object)(int)s))
                    .ToArray<object>();
            return (object)string.Join(",",
                ((IEnumerable<object>)array).Select<object, string>((Func<object, string>)(s =>
                    string.Format("'{0}'", (object)(s ?? (object)string.Empty).ToString().Replace("'", "''")))));
        }

        private static Type GetItemType(IEnumerable<object> array)
        {
            object obj = array.FirstOrDefault<object>();
            return obj == null ? (Type)null : obj.GetType();
        }

        private static bool IsOfType<T>(object value) => value is T;

        public static Type GetRealType(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type;
            }

            return (Type)type.GetGenericArguments().First();
        }
    }
}