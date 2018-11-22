using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHierarchy;

namespace Test
{
    class Program
    {
        static int Main(string[] args)
        {

            Console.Write("Enter the directory path...");
            string path = Console.ReadLine();
            Console.Write("Enter the JSON-file destination directory path...");
            string pathDestination = Console.ReadLine();
            try
            {
                Folder.ToConvertToJson(path, pathDestination);
                Console.WriteLine("File hierarchy representation of the selected path\nis sucssesfully saved in JSON format.");
            }
            catch (ConvertToJsonMethodException e)
            {
                Console.WriteLine(e.Message);
            }

            return 1;
        }
    }
}
