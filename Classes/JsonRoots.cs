using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprocketToolkit.Classes
{
    #region BASE CLASSES
    public class CompartmentBaseRoot
    {
        public string id { get; set; }
        public string data { get; set; }
        public string metaData { get; set; }
    }


    #endregion

    #region BLUEPRINT ROOT CLASSES

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Compartment
    {
        public Version version { get; set; }
        public List<double> points { get; set; }
        public List<List<int>> sharedPoints { get; set; }
        public List<double> thicknessMap { get; set; }
        public List<List<int>> faceMap { get; set; }
        public double smooth { get; set; }
    }

    public class CompartmentRoot
    {
        public Version version { get; set; }
        public string name { get; set; }
        public int ID { get; set; }
        public int parentID { get; set; }
        public int flags { get; set; }
        public double displacement { get; set; }
        public double armourVolume { get; set; }
        public List<double> pos { get; set; }
        public List<double> rot { get; set; }
        public object turret { get; set; }
        public object genID { get; set; }
        public object genData { get; set; }
        public Compartment compartment { get; set; }
    }

    public class Version
    {
        public int Major { get; set; }
        public int Minor { get; set; }
    }

    #endregion
}
