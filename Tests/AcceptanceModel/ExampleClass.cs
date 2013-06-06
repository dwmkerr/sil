using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AcceptanceModel
{
    public class ExampleClass
    {
        public void ExampleFunction()
        {
            //  This is the example function.
            Console.WriteLine("Example function");
        }

        public int CountLines(string text)
        {
            //  Return the count of the lines.
            int counter = 0;
            using (var reader = new StringReader(text))
            {
                while (reader.ReadLine() != null)
                    counter++;
            }

            //  Return the counter.
            return counter;
        }
    }
}
