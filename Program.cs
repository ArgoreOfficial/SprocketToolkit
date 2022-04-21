using System;
using System.IO;
using System.Linq;
using SprocketToolkit.Classes;

namespace SprocketToolkit
{
    class Program
    {
        static bool debug = true;
        static string version = "0.1 ALPHA";
        static DateTime timer;

        /*
         *  Stuff to make 
         *  
         * - Mesh Export
         * - Compartment Merging
         *      Merges two compartments with vertices and faces
         *      usefull for making decorations in sprocket with turrets, then merging to make one hull
         * - Terrain parsing
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
         

        static void Setup()
        {
            Console.Title = $"The Sprocket Toolkit - {version}";
        }

        static void Main(string[] args)
        {
            Setup();
            bool run = true;
            while (run)
            {
                Console.Clear();
                string header =
                   $"[+] The Sprocket Toolkit v{version}\n" +
                    "[+] By Argore\n";
                CE.Write(header, ConsoleColor.Yellow);
                CE.Line();



                string path = "";
                while (path == "")
                {
                    CE.Write("Enter file path: ");
                    path = OpenFile();
                }

                if (new FileInfo(path).Extension == ".obj")
                {
                    string faction = ChooseFaction();
                    CE.Line();


                    timer = DateTime.Now;
                    CE.Write($"[*] Loading {new FileInfo(path).Length} bytes...\n");
                    MeshImporter.Import(@"C:\Users\argor\Desktop\WEE.obj", faction, new FileInfo(path).Name.Replace(".obj", ""));
                    CE.Write($"[*] Done!\n");

                    if (debug)
                    {
                        CE.Line();
                        CE.Write($"[DEBUG] Finished in {(DateTime.Now - timer).TotalSeconds} seconds.\n");
                    }
                }
                else
                {
                    CE.Alert("File type not supported.");
                }
                Console.ReadLine();
            }
        }

        static string OpenFile()
        {
            string path = Console.ReadLine().Replace(@"\", "/").Replace("\"", "");
            if (File.Exists(path))
            {
                return path;
            }
            return "";
        }

        /// <summary>
        ///  Prints faction list and returns path to chosen faciton
        /// </summary>
        /// <returns></returns>
        static string ChooseFaction()
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string factionsFolder = $"{documents}/My Games/Sprocket/Factions/";
            string[] factions = Directory.GetDirectories(factionsFolder);

            CE.Write("OK! Choose faction.\n", ConsoleColor.Yellow);
            CE.Line();
            for (int i = 0; i < factions.Length; i++)
            {
                CE.Write($"[{i}] {factions[i].Split('/').Last()}\n", ConsoleColor.Green);
            }
            CE.Line();
            
            return factions[Option(factions.Length)];
        }

        /// <summary>
        ///  Gets Console.ReadLine and checks if it's within range
        /// </summary>
        /// <param name="range">exclusive upper bound</param>
        /// <returns></returns>
        static int Option(int range)
        {
            int option = -1;
            while (option > range-1 || option < 0)
            {
                CE.Write("Enter option number: ");
                int.TryParse(Console.ReadLine(), out option);
            }
            return option;

        }
    }
}
