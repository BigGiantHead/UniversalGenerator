using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleIvan
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClass test = UniversalGenerator.UniversalGenerator.Instance.CreateInstance<TestClass>();
            Console.WriteLine(test.DoubleProperty);
            Console.WriteLine();

            Console.WriteLine(test.IntProperty);
            Console.WriteLine();

            Console.WriteLine(test.StringProperty);
            Console.WriteLine();

            for (int i = 0; i < test.StringArrayProperty.Length; ++i)
            {
                Console.WriteLine(test.StringArrayProperty[i]);
            }
            Console.WriteLine();

            for (int i = 0; i < test.ShortListProperty.Count; ++i)
            {
                Console.WriteLine(test.ShortListProperty[i]);
            }

            Console.ReadKey();
        }
    }
}
