using System.Windows.Shapes;

namespace SpotLibrary
{
    public class Graph
    {
        public Polyline GraphPolyline { get; private set; }
        public string GraphName { get; private set; }

        /// <summary>
        /// Create new graph instance.
        /// </summary>
        public Graph() : this(string.Empty, new Polyline()) { }

        /// <summary>
        /// Create new graph instance.
        /// </summary>
        /// <param name="name">Graph name.</param>
        /// <param name="polyline">Graph polyline.</param>
        public Graph(string name, Polyline polyline)
        {
            GraphName = name;
            GraphPolyline = polyline;
        }
    }
}
