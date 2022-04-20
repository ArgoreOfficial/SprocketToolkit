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
            StringBuilder fSB = new StringBuilder(); // faces
            StringBuilder pSB = new StringBuilder(); // points
            int fErrors = 0;
            int points = 0;

            // problem is that with vertices [0,1,2,3,4], faces don't always go in order
            // so f[0,1,4],[2,3,4] might be a thing
            // that doesn't work when creating 3 new points per face

            for (int i = 0; i < loaded.facesVertsIndxs.Count; i++)
            {
                if(loaded.facesVertsIndxs[i].Count == 3)
                {
                    /*
                    pSB.Append("," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][0] - 1]) + "," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][1] - 1]) + "," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][2] - 1])
                        );*/

                    pSB.Append("," + PtoString(loaded.vertices[loaded.facesVertsIndxs[i].Min() - 1]));

                    for (int o = 0; o < loaded.facesVertsIndxs[i].Count; o++)
                    {
                        if (
                           loaded.facesVertsIndxs[i][o] !=
                           loaded.facesVertsIndxs[i].Min() &&
                           loaded.facesVertsIndxs[i][o] !=
                           loaded.facesVertsIndxs[i].Max()) 
                        {
                            pSB.Append("," + PtoString(loaded.vertices[loaded.facesVertsIndxs[i][o] - 1]));
                        }
                    }

                    pSB.Append("," + PtoString(loaded.vertices[loaded.facesVertsIndxs[i].Max() - 1]));


                    //fSB.Append("," + JsonSerializer.Serialize(loaded.facesVertsIndxs[i]));
                    fSB.Append(",["
                        + (loaded.facesVertsIndxs[i][0] - 1) + ","
                        + (loaded.facesVertsIndxs[i][1] - 1) + ","
                        + (loaded.facesVertsIndxs[i][2] - 1) 
                        + "]");


                    points += 3;
                }/*
                else if(loaded.facesVertsIndxs[i].Count == 4)
                {
                    pSB.Append("," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][0] - 1]) + "," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][1] - 1]) + "," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][2] - 1]) + "," +
                          PtoString(loaded.vertices[loaded.facesVertsIndxs[i][3] - 1])
                        );

                    // idk why it has to be this way

                    //  4/3/1 3/4/1
                    //  3/2/0 2/3/0
                    //   [0,1,3,2]
                    //      to
                    //  0 3 2 0 1 3 
                    // [0,2,3,0,1,2
                    // test 0,1,2,0,1,3,0,2,3
                    fSB.Append(",[" 
                        + (loaded.facesVertsIndxs[i][0] - 1) + ","
                        + (loaded.facesVertsIndxs[i][1] - 1) + ","
                        + (loaded.facesVertsIndxs[i][2] - 1) + ","
                        + (loaded.facesVertsIndxs[i][0] - 1) + ","
                        + (loaded.facesVertsIndxs[i][1] - 1) + ","
                        + (loaded.facesVertsIndxs[i][3] - 1) + ","
                        + (loaded.facesVertsIndxs[i][0] - 1) + ","
                        + (loaded.facesVertsIndxs[i][2] - 1) + ","
                        + (loaded.facesVertsIndxs[i][3] - 1)
                        + "]");
                    points += 4;
                }*/
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
