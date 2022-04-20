using OBJ3DWavefrontLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
namespace SprocketToolkit
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime timer = DateTime.Now;

            MeshImporter.Import(@"C:\Users\argor\Desktop\Models\Sprocket\TriCube.obj");

            Console.WriteLine((DateTime.Now - timer).TotalSeconds);
            Console.ReadLine();
        }
    }
}
