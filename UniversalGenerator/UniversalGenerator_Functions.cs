using System.Collections;
using System.Reflection;
using System.Linq;
using System;

namespace UniversalGenerator
{
    public static class UniversalGenerator_Functions
    {
        private static Random rand = new Random();

        #region random numbers
        public static object GenerateFloat(object[] customAttributes)
        {
            RangeAttribute range = customAttributes.FirstOrDefault(o => o is RangeAttribute) as RangeAttribute;
            float min = -7000;
            float max = 7000;

            if (range != null)
            {
                min = range.Min;
                max = range.Max;
            }

            float difference = max - min;

            return (float)(rand.NextDouble() * difference + min);
        }

        public static object GenerateInt(object[] customAttributes)
        {
            RangeAttribute range = customAttributes.FirstOrDefault(o => o is RangeAttribute) as RangeAttribute;
            int min = int.MinValue;
            int max = int.MaxValue;

            if (range != null)
            {
                if (range.Min < int.MinValue || range.Min > int.MaxValue)
                {
                    min = int.MinValue;
                }
                else
                {
                    min = (int)range.Min;
                }

                if (range.Max < int.MinValue || range.Max > int.MaxValue)
                {
                    max = int.MaxValue;
                }
                else
                {
                    max = (int)range.Max;
                }
            }

            return rand.Next(min, max);
        }

        public static object GenerateShortInt(object[] customAttributes)
        {
            short min = short.MinValue;
            short max = short.MaxValue;

            RangeAttribute range = customAttributes.FirstOrDefault(o => o is RangeAttribute) as RangeAttribute;

            if (range != null)
            {
                if (range.Min < short.MinValue || range.Min > short.MaxValue)
                {
                    min = short.MinValue;
                }
                else
                {
                    min = (short)range.Min;
                }

                if (range.Max < short.MinValue || range.Max > short.MaxValue)
                {
                    max = short.MaxValue;
                }
                else
                {
                    max = (short)range.Max;
                }
            }

            return (short)rand.Next(min, max);
        }

        public static object GenerateULong(object[] customAttributes)
        {
            RangeAttribute range = customAttributes.FirstOrDefault(o => o is RangeAttribute) as RangeAttribute;
            int min = 0;
            int max = int.MaxValue;

            if (range != null)
            {
                if (range.Min < 0 || range.Min > int.MaxValue)
                {
                    min = 0;
                }
                else
                {
                    min = (int)range.Min;
                }

                if (range.Max < 0 || range.Max > int.MaxValue)
                {
                    max = int.MaxValue;
                }
                else
                {
                    max = (int)range.Max;
                }
            }

            return (ulong)rand.Next(min, max);
        }

        public static object GenerateUInt(object[] customAttributes)
        {
            RangeAttribute range = customAttributes.FirstOrDefault(o => o is RangeAttribute) as RangeAttribute;
            int min = 0;
            int max = int.MaxValue;

            if (range != null)
            {
                if (range.Min < 0 || range.Min > int.MaxValue)
                {
                    min = 0;
                }
                else
                {
                    min = (int)range.Min;
                }

                if (range.Max < 0 || range.Max > int.MaxValue)
                {
                    max = int.MaxValue;
                }
                else
                {
                    max = (int)range.Max;
                }
            }

            return (uint)rand.Next(min, max);
        }

        public static object GenerateUShortInt(object[] customAttributes)
        {
            RangeAttribute range = customAttributes.FirstOrDefault(o => o is RangeAttribute) as RangeAttribute;
            short min = 0;
            short max = short.MaxValue;

            if (range != null)
            {
                if (range.Min < 0 || range.Min > short.MaxValue)
                {
                    min = short.MinValue;
                }
                else
                {
                    min = (short)range.Min;
                }

                if (range.Max < 0 || range.Max > short.MaxValue)
                {
                    max = short.MaxValue;
                }
                else
                {
                    max = (short)range.Max;
                }
            }

            return (ushort)rand.Next(min, max);
        }
        #endregion

        public static object GenerateBool(object[] customAttributes)
        {
            return rand.Next(0, 2) == 0 ? false : true;
        }

        public static object GenerateDateTime(object[] customAttributes)
        {
            DateTime start = new DateTime();
            int days = (DateTime.Now - start).Days;

            return start.AddDays(rand.Next(0, days + 1));
        }

        public static object GenerateTimeSpan(object[] customAttributes)
        {
            DateTime start = new DateTime();
            int days = (DateTime.Now - start).Days;

            return start.AddDays(rand.Next(0, days + 1)) - start;
        }

        public static object GenerateString(object[] customAttributes)
        {
            ValueFromArrayAttribute arrayAttr = customAttributes.FirstOrDefault(o => o is ValueFromArrayAttribute) as ValueFromArrayAttribute;
            LengthAttribute lengthAttr = customAttributes.FirstOrDefault(o => o is LengthAttribute) as LengthAttribute;

            if (arrayAttr != null)
            {
                return arrayAttr.Values[rand.Next(0, arrayAttr.Values.Length)];
            }

            int length = 5;
            if (lengthAttr != null)
            {
                length = lengthAttr.Length;
            }

            return RandomString(length);
        }

        public static object GenerateChar(object[] customAttributes)
        {
            return RandomString(1)[0];
        }

        public static object GenerateEnum(object[] customAttributes)
        {
            Type enumType = customAttributes.FirstOrDefault(o => o is Type) as Type;
            if (enumType == null)
            {
                return null;
            }

            string[] values = Enum.GetNames(enumType);

            return Enum.Parse(enumType, values[rand.Next(0, values.Length)]);
        }

        public static Type GetTypeOfMemberInfo(MemberInfo member)
        {
            Type memberType = null;
            if (member.MemberType == MemberTypes.Property)
            {
                memberType = (member as PropertyInfo).PropertyType;
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                memberType = (member as FieldInfo).FieldType;
            }

            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                memberType = memberType.GetGenericArguments()[0];
            }

            return memberType;
        }

        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[rand.Next(0, s.Length)]).ToArray());
        }
    }
}