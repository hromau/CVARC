using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CVARC.V2
{
    [DataContract]
    public class LocatorItem
    {
        [DataMember]
        public string Id { get; set; }
        
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public double Angle { get; set; }

        public override string ToString()
        {
            return String.Format("X = {0:F}, Y = {1:F}, Angle = {2:F}", X, Y, Angle);
        }
    }
}
