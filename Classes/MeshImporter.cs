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




            // Faces
            StringBuilder fSB = new StringBuilder(); // faces
            StringBuilder pSB = new StringBuilder(); // points
            int fErrors = 0;
            int points = 0;
            List<Vector3> newPoints = new List<Vector3>();
            // problem is that with vertices [0,1,2,3,4], faces don't always go in order
            // so f[0,1,4],[2,3,4] might be a thing
            // that doesn't work when creating 3 new points per face

            for (int i = 0; i < loaded.facesVertsIndxs.Count; i++)
            {
                for (int o = 0; o < 3; o++)
                {
                    pSB.Append("," + PtoString(loaded.vertices[loaded.facesVertsIndxs[i][o] - 1]));
                    newPoints.Add(loaded.vertices[loaded.facesVertsIndxs[i][o] - 1]);
                    points++;
                }
                fSB.Append($",[{points - 3},{points - 2},{points - 1}]");
            }

            compSB.Replace(" *points* ", pSB.ToString().Substring(1));
            compSB.Replace(" *fMap* ", fSB.ToString().Substring(1));


            // SharedPoints
            StringBuilder sp = new StringBuilder();
            /*for (int i = 0; i < points; i++)
            {
                sp.Append($",[{i}]");
            }
            */

            List<int> checkedPoints = new List<int>();

            for (int p1 = 0; p1 < newPoints.Count; p1++)
            {
                if(checkedPoints.Contains(p1))
                {
                    continue;
                }
                sp.Append($",[{p1}");
                for (int p2 = p1+1; p2 < newPoints.Count; p2++)
                {
                    if(newPoints[p1] == newPoints[p2])
                    {
                        sp.Append($",{p2}");
                        checkedPoints.Add(p2);
                    }
                }
                sp.Append("]");
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
