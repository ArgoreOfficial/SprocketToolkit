using System;
using System.IO;
using System.Linq;
using SprocketToolkit.Classes;

namespace SprocketToolkit
{
    class Program
    {
        static string version = "0.0.2 ALPHA";
        static DateTime timer;

        /*
         *  Stuff to make 
         *  
         * - new .obj loading - MAYBE DONE
         * 
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
            ResourceHandler.Load();
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



                string input = "";
                while (input == "")
                {
                    CE.Write("Enter file path: ");
                    input = OpenFile();
                }

                // get file type

                string filetype = new FileInfo(input).Extension;
                if (filetype == ".obj")
                {
                    string savePath = "";
                    if (Settings.Utility.Debug)
                    {
                        CE.Write("[DEBUG] Debug is enabled. Saving to application folder.\n");
                    }
                    else
                    {
                        savePath = ChooseFaction();
                    }

                    CE.Line();

                    timer = DateTime.Now;
                    CE.Write($"[*] Loading {new FileInfo(input).Length} bytes...\n");
                    if (MeshImporter.ImportJson(input, savePath, new FileInfo(input).Name.Replace(".obj", "")))
                    {
                        CE.Write($"[*] Done!\n");

                        if (Settings.Utility.Debug)
                        {
                            CE.Line();
                            CE.Write($"[DEBUG] Finished in {(DateTime.Now - timer).TotalSeconds} seconds.\n");
                        }
                    }
                    else
                    {
                        CE.Alert("[!] File corrupt.");
                    }
                }
                else if (filetype == ".blueprint") // blueprint file
                {
                    //text
                    CE.Write("OK! Choose Option\n", ConsoleColor.Yellow);
                    CE.Line();
                    CE.Write("[0] Export Model\n", ConsoleColor.Green);
                    CE.Write("[1] Merge all compartments (not finished yet)\n", ConsoleColor.DarkGreen);
                    CE.Line();

                    // option
                    int option = Option(2);

                    if (option == 0) // export to model
                    {
                        CE.Line();
                        BlueprintExporter.Export(input, "", new FileInfo(input).Name.Replace(".blueprint", ""));
                    }
                    else if(option == 1) // merge into one hull
                    {
                        CE.Line();
                        CompartmentMerger.MergeAll(input);
                    }
                    
                }
                else
                {
                    CE.Alert("[!] File type not supported.");
                }

                Console.ReadLine();
            }
            
        }

        static string OpenFile()
        {
            string path = Console.ReadLine().Replace(@"\", "/").Replace("\"", "");
            // file
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
