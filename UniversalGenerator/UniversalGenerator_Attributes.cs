using System.Collections;
using System;

namespace UniversalGenerator
{
    /// <summary>
    /// Attribute that tells the generator Length of something, used for strings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class LengthAttribute : Attribute
    {
        private int length = 0;

        public int Length
        {
            get
            {
                return length;
            }
        }

        public LengthAttribute(int length)
        {
            this.length = length;
        }
    }

    /// <summary>
    /// Range of a value, minimum and maximum value. Used for number properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class RangeAttribute : Attribute
    {
        private float min = 0;

        private float max = 0;

        public float Min
        {
            get
            {
                return min;
            }
        }

        public float Max
        {
            get
            {
                return max;
            }
        }

        public RangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    /// <summary>
    /// Pick a string value from array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ValueFromArrayAttribute : Attribute
    {
        private string[] values = null;

        public string[] Values
        {
            get
            {
                return values;
            }
        }

        public ValueFromArrayAttribute(params string[] values)
        {
            this.values = values;
        }
    }
}