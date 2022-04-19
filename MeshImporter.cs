using OBJ3DWavefrontLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.Json;

namespace SprocketToolkit
{

    

    static class MeshImporter
    {
        public static void Import(string filePath)
        {
            SimpleMesh loaded;

            // Load file
            using (var reader = new StreamReader(filePath))
            {
                loaded = SimpleMesh.LoadFromObj(reader);
            }



            // Setup
            StringBuilder compSB = new StringBuilder();
            compSB.Append(File.ReadAllText("Resources/BaseCompartment.txt"));



            //Points
            //compSB.Replace(" *points* ",
            //    string.Join("", loaded.vertices)
            //    .Replace(",", ".")
            //    .Replace("<", ",")
            //    .Replace(">", "")
            //    .Replace("  ", ",")
            //    .Substring(1));



            // Faces
            StringBuilder fSB = new StringBuilder();
            StringBuilder pSB = new StringBuilder();
            int fErrors = 0;
            int points = 0;

            for (int i = 0; i < loaded.facesVertsIndxs.Count; i++)
            {
                if(loaded.facesVertsIndxs[i].Count == 3)
                {
                    pSB.Append("," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][0] - 1]) +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][1] - 1]) +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][2] - 1])
                        );
                    fSB.Append("," + JsonSerializer.Serialize(loaded.facesVertsIndxs[i]));
                    points += 3;
                }
                else if(loaded.facesVertsIndxs[i].Count == 4)
                {
                    pSB.Append("," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][0] - 1]) +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][1] - 1]) +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][2] - 1]) +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][3] - 1])
                        );

                    // idk why it has to be this way
                    fSB.Append(",[" 
                        + (loaded.facesVertsIndxs[i][3]-1) + ","
                        + (loaded.facesVertsIndxs[i][0]-1) + ","
                        + (loaded.facesVertsIndxs[i][1]-1) + ","
                        + (loaded.facesVertsIndxs[i][3]-1) + ","
                        + (loaded.facesVertsIndxs[i][1]-1) + ","
                        + (loaded.facesVertsIndxs[i][2]-1)
                        + "]");
                    points += 4;
                }
                else
                {
                    fErrors++;
                }
            }
            compSB.Replace(" *points* ", pSB.ToString().Substring(1));
            compSB.Replace(" *fMap* ", fSB.ToString().Substring(1));


            // SharedPoints
            StringBuilder sp = new StringBuilder();
            for (int i = 0; i < points; i++)
            {
                sp.Append($",[{i}]");
            }
            compSB.Replace(" *sPoints* ", sp.ToString().Substring(1));
            
            // ThicknessMap
            StringBuilder tSB = new StringBuilder();
            for (int i = 0; i < points; i++)
            {
                tSB.Append(",1");
            }
            compSB.Replace(" *tMap* ", tSB.ToString().Substring(1));


            //compSB.Replace(" *fMap* ", "");

            // Final File
            File.WriteAllText(
                "testFile.txt",
                compSB.ToString());
            Console.WriteLine($"{fErrors} failed faces.");
                
        }

        static string PtoString(Vector3 p)
        {
            return ($"{p.X.ToString().Replace(",", ".")},{p.Y.ToString().Replace(",", ".")},{p.Z.ToString().Replace(",", ".")}");
        }
    }
}
