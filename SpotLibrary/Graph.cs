using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SpotLibrary
{
    public class Graph
    {
        public Polyline GraphPolyline { get; private set; }
        public string GraphName { get; private set; }

        public Graph() : this(string.Empty, new Polyline()) { }

        public Graph(string name, Polyline polyline)
        {
            GraphName = name;
            GraphPolyline = polyline;
        }
    }
}
