using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Configuration;

namespace UniversalGenerator
{
    /// <summary>
    /// Singleton class that creates an instance from a type and fills the object's properties, not fields.
    /// Currently it can handle the following types:
    /// short, int, long, ushort, uint, ulong, float, double, string, char, DateTime, TimeSpan, Color, Vector2, Vector3, Vector4, Quaternion.
    /// The class can be extended with new fill/generate/randomize functions.
    /// </summary>
    public class UniversalGenerator
    {
        private static UniversalGenerator instance = null;

        public static UniversalGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UniversalGenerator();
                }

                return instance;
            }
        }

        private Random rand = new Random();

        private Dictionary<Type, GetRandomValue> fillFunctions = new Dictionary<Type, GetRandomValue>(0);

        public delegate object GetRandomValue(object[] customAttributes);

        private UniversalGenerator()
        {
            //assign functions for each type
            fillFunctions.Add(typeof(short), UniversalGenerator_Functions.GenerateShortInt);
            fillFunctions.Add(typeof(int), UniversalGenerator_Functions.GenerateInt);
            fillFunctions.Add(typeof(long), UniversalGenerator_Functions.GenerateInt);
            fillFunctions.Add(typeof(ushort), UniversalGenerator_Functions.GenerateUShortInt);
            fillFunctions.Add(typeof(uint), UniversalGenerator_Functions.GenerateUInt);
            fillFunctions.Add(typeof(ulong), UniversalGenerator_Functions.GenerateULong);

            fillFunctions.Add(typeof(float), UniversalGenerator_Functions.GenerateFloat);
            fillFunctions.Add(typeof(double), UniversalGenerator_Functions.GenerateFloat);

            fillFunctions.Add(typeof(string), UniversalGenerator_Functions.GenerateString);
            fillFunctions.Add(typeof(char), UniversalGenerator_Functions.GenerateChar);

            fillFunctions.Add(typeof(bool), UniversalGenerator_Functions.GenerateBool);

            fillFunctions.Add(typeof(DateTime), UniversalGenerator_Functions.GenerateDateTime);
            fillFunctions.Add(typeof(TimeSpan), UniversalGenerator_Functions.GenerateTimeSpan);

            fillFunctions.Add(typeof(Enum), UniversalGenerator_Functions.GenerateEnum);

            //read from config
            string[] keys = ConfigurationManager.AppSettings.AllKeys;
            for (int i = 0; i < keys.Length; ++i)
            {
                try
                {
                    Type type = Type.GetType(keys[i]);

                    string stringValue = ConfigurationManager.AppSettings[keys[i]];
                    string[] splitValue = stringValue.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    string stringType = stringValue.Replace("." + splitValue[splitValue.Length - 1], "");
                    string stringMethod = splitValue[splitValue.Length - 1];

                    MethodInfo method = Type.GetType(stringType).GetMethod(stringMethod);
                    GetRandomValue value = Delegate.CreateDelegate(typeof(GetRandomValue), method) as GetRandomValue;

                    if (value != null && type != null)
                    {
                        if (fillFunctions.ContainsKey(type))
                        {
                            fillFunctions[type] = value;
                        }
                        else
                        {
                            fillFunctions.Add(type, value);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Create instance of type and fill it with random values.
        /// </summary>
        /// <param name="type">Type of the instance to be created.</param>
        /// <returns></returns>
        public object CreateInstance(Type type, int elementCount = 50)
        {
            if (type == null)
                return null;

            object instanceFromType = null;

            try
            {
                instanceFromType = Activator.CreateInstance(type);
            }
            catch
            {
            }

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();

                if (elementCount == 0)
                {
                    elementCount = rand.Next(50);
                }

                Array arr = Array.CreateInstance(elementType, elementCount);

                for (int i = 0; i < arr.Length; ++i)
                {
                    object value = GetRandomValueFromFunctionForType(elementType, new object[0]);
                    if (value == null)
                    {
                        value = CreateInstance(elementType);
                    }
                    arr.SetValue(value, i);
                }

                instanceFromType = arr;
            }
            else if (instanceFromType != null && typeof(IList).IsAssignableFrom(type))
            {
                Type elementType = type.GetGenericArguments()[0];
                IList list = instanceFromType as IList;

                if (elementCount == 0)
                {
                    elementCount = rand.Next(50);
                }

                for (int i = 0; i < elementCount; ++i)
                {
                    object value = GetRandomValueFromFunctionForType(elementType, new object[0]);
                    if (value == null)
                    {
                        value = CreateInstance(elementType);
                    }

                    list.Add(value);
                }
            }
            else if (instanceFromType != null)
            {
                FillMembers(instanceFromType);
            }

            return instanceFromType;
        }

        /// <summary>
        /// Create instance of type T and fill it with random values.
        /// </summary>
        /// <typeparam name="T">Type of the instance to be created.</typeparam>
        /// <returns></returns>
        public T CreateInstance<T>(int elementCount = 0)
        {
            Type theType = typeof(T);

            return (T)CreateInstance(theType, elementCount);
        }

        /// <summary>
        /// Assign a function that will generate a random value for a given type.
        /// </summary>
        /// <param name="type">Type for which the function will be used.</param>
        /// <param name="action">Function that will generate a random value for a given type.</param>
        public void AddFillFunction(Type type, GetRandomValue action)
        {
            if (fillFunctions.ContainsKey(type))
            {
                fillFunctions[type] = action;
            }
            else
            {
                fillFunctions.Add(type, action);
            }
        }

        /// <summary>
        /// Assign a function that generates a random value for a given type.
        /// </summary>
        /// <param name="type">Type for which the function is removed.</param>
        public void RemoveFillFunction(Type type)
        {
            if (fillFunctions.ContainsKey(type))
            {
                fillFunctions.Remove(type);
            }
        }

        private void FillMembers(object target)
        {
            Type theType = target.GetType();

            //fill properties
            PropertyInfo[] properties = theType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < properties.Length; ++i)
            {
                PropertyInfo p = properties[i];

                FillMember(target, p);
            }

            //fill fields
            FieldInfo[] fields = theType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo f = fields[i];

                FillMember(target, f);
            }
        }

        private void FillMember(object target, MemberInfo member)
        {
            Type memberType = UniversalGenerator_Functions.GetTypeOfMemberInfo(member);

            if (memberType == null)
                return;

            object[] customAttributes = member.GetCustomAttributes(false);

            if (fillFunctions.ContainsKey(memberType))
            {
                if (memberType != null)
                {
                    GetRandomValue piaction = fillFunctions[memberType];
                    FillMemberValue(target, member, GetRandomValueForType(memberType, customAttributes));
                }
            }
            else if (memberType.IsEnum)
            {
                GetRandomValue piaction = fillFunctions[typeof(Enum)];
                FillMemberValue(target, member, GetRandomValueForType(memberType, customAttributes));
            }
            else
            {
                object instance = CreateInstance(memberType);
                FillMemberValue(target, member, instance);
            }
        }

        private void FillMemberValue(object target, MemberInfo member, object value)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                (member as PropertyInfo).SetValue(target, value, null);
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                (member as FieldInfo).SetValue(target, value);
            }
        }

        private object GetRandomValueForType(Type type, object[] customAttributes)
        {
            object instance = GetRandomValueFromFunctionForType(type, customAttributes);

            if (instance == null)
            {
                instance = CreateInstance(type);
            }

            return instance;
        }

        private object GetRandomValueFromFunctionForType(Type type, object[] customAttributes)
        {
            if (fillFunctions.ContainsKey(type))
            {
                if (type != null)
                {
                    GetRandomValue piaction = fillFunctions[type];

                    return piaction(customAttributes);
                }
            }
            else if (type.IsEnum)
            {
                GetRandomValue piaction = fillFunctions[typeof(Enum)];

                List<object> attrs = new List<object>(0);
                attrs.AddRange(customAttributes);
                attrs.Add(type);

                return piaction(customAttributes);
            }

            return null;
        }
    }
}