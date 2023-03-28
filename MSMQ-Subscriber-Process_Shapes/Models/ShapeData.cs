using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MSMQ_Subscriber_Process_Shapes.Models
{
    public class ShapeData
    {
        public Type? ShapeType { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush? Fill { get; set; }
        public Brush? Stroke { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
