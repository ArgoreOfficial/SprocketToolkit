using OBJ3DWavefrontLoader;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Numerics;
using SprocketToolkit.Classes;

namespace SprocketToolkit
{



    static class MeshImporter
    {
        public static void Import(string filePath, string savePath, string saveName)
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




            // Faces & Points
            StringBuilder fSB = new StringBuilder(); // faces
            StringBuilder pSB = new StringBuilder(); // points
            int fErrors = 0;
            int points = 0;
            List<Vector3> newPoints = new List<Vector3>();

            for (int i = 0; i < loaded.facesVertsIndxs.Count; i++)
            {
                if (loaded.facesVertsIndxs[i].Count == 3)
                {
                    // Triangles
                    MakeFace(pSB, newPoints, loaded.vertices, loaded.facesVertsIndxs[i], new int[] { 0, 1, 2 });
                    points += 6;
                    fSB.Append($"[{points - 3},{points - 2},{points - 1}]");
                }
                else if (loaded.facesVertsIndxs[i].Count == 4)
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

                    MakeFace(pSB, newPoints, loaded.vertices, loaded.facesVertsIndxs[i], new int[] { 2,1,0,2,0,3 });
                    points += 6;
                    fSB.Append($",[{points - 4},{points - 5},{points - 6},{points - 1},{points - 2},{points - 3}]");
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
            if(!Directory.Exists($"{savePath}/Blueprints/Compartments/"))
            {
                Directory.CreateDirectory($"{savePath}/Blueprints/Compartments/");
            }

            File.WriteAllText(
                $"{savePath}/Blueprints/Compartments/{saveName}.blueprint",
                compSB.ToString());
            
            if(fErrors > 0)
            {
                CE.Alert($"{fErrors} failed faces. Only Triangles and Quads are supported for now\n");
            }
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
                VertexSB.Append("," + PtoString(LoadedVerts[FaceIndex[IndexOrder[i]] - 1]));
                NewVerts.Add(LoadedVerts[FaceIndex[IndexOrder[i]] - 1]);
            }
        }
    }
}
