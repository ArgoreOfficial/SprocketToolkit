using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Numerics;
using SprocketToolkit.Classes;
using System;

namespace SprocketToolkit.Classes
{

    static class MeshImporter
    {
        public static bool Import(string filePath, string savePath, string saveName)
        {
            Mesh loaded;

            // Load file
            loaded = MeshLoader.LoadOBJ(filePath);

            // make sure file isn't empty
            if(loaded.Faces.Count == 0 || loaded.Vertices.Count == 0)
            {
                return false;
            }

            // Setup
            StringBuilder compSB = new StringBuilder();
            compSB.Append(File.ReadAllText("Resources/BaseCompartment.txt"));




            // Faces & Points
            StringBuilder fSB = new StringBuilder(); // faces
            StringBuilder pSB = new StringBuilder(); // points
            int fErrors = 0;
            int points = 0;
            List<Vector3> newPoints = new List<Vector3>(); // new list of points

            for (int i = 0; i < loaded.Faces.Count; i++)
            {
                if (loaded.Faces[i].Count == 3)
                {
                    // Triangles
                    MakeFace(pSB, newPoints, loaded.Vertices, loaded.Faces[i], new int[] { 0, 1, 2 });
                    points += 3;
                    fSB.Append($",[{points - 3},{points - 2},{points - 1}]");
                }
                else if (loaded.Faces[i].Count == 4)
                {
                    // Quad
                    // from:
                    // 1 2 4 3
                    // to:
                    // 4 2 1
                    // 4 1 3
                    // with indexes:
                    // 2 1 0
                    // 2 0 3

                    MakeFace(pSB, newPoints, loaded.Vertices, loaded.Faces[i], new int[] { 0,1,2,2,3,0 });
                    points += 6;
                    fSB.Append($",[{points - 4},{points - 5},{points - 6},{points - 1},{points - 2},{points - 3}]");
                }
                else
                {
                    fErrors++;
                }
            }

            // makes everything into one face
            if(Settings.Fun.OneFace)
            {
                fSB.Replace("[", "").Replace("]", "");
            }
            
            compSB.Replace(" *points* ", pSB.ToString().Substring(1));
            compSB.Replace(" *fMap* ", fSB.ToString().Substring(1 * Convert.ToInt32(!Settings.Fun.OneFace)));




            // SharedPoints
            StringBuilder sp = new StringBuilder();
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




            // Final File
            if(!Directory.Exists($"{savePath}/Blueprints/Compartments/"))
            {
                Directory.CreateDirectory($"{savePath}/Blueprints/Compartments/");
            }
            savePath += "/Blueprints/Compartments/";

            // if debug, save in application folder as DEBUG.blueprint
            if (Settings.Utility.Debug)
            {
                savePath = "";
                saveName = "DEBUG";
            }

            File.WriteAllText(
                $"{savePath}{saveName}.blueprint",
                compSB.ToString());
            

            if(fErrors > 0)
            {
                CE.Alert($"{fErrors} failed faces. Only Triangles and Quads are supported for now\n");
            }

            return true;
        }

        /// <summary>
        ///  Vector3 to string
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        static string PtoString(Vector3 p)
        {
            return ($"{p.X.ToString().Replace(",", ".")},{p.Y.ToString().Replace(",", ".")},{p.Z.ToString().Replace(",", ".")}");
        }

        /// <summary>
        ///  Make N-Gon
        /// </summary>
        /// <param name="VertexSB">StringBuilder</param>
        /// <param name="NewVerts">Vertex Vector3 list to write to</param>
        /// <param name="LoadedVerts">Vertex Vector3 list to read from</param>
        /// <param name="FaceIndex">Face int list to read from</param>
        /// <param name="IndexOrder">What order to do face indexes in</param>
        static void MakeFace(StringBuilder VertexSB, List<Vector3> NewVerts, List<Vector3> LoadedVerts, List<int> FaceIndex, int[] IndexOrder)
        {
            
            for (int i = 0; i < IndexOrder.Length; i++)
            {
                Vector3 newVert = LoadedVerts[FaceIndex[IndexOrder[i]] - 1] * new Vector3(-1,1,1);
                //Console.WriteLine($"{newVert.X} -> {newVert.X * -1}");
                
                if(Settings.Fun.Ballify)
                {
                    newVert = Vector3.Normalize(newVert); // turns mesh into a ball
                }

                VertexSB.Append("," + PtoString(newVert));  
                NewVerts.Add(newVert);
            }
        }
    }
}
