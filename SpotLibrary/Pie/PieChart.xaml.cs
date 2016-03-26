using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotLibrary.Pie
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart : UserControl
    {
        public PieChart()
        {
            InitializeComponent();
        }

        

        //<Controls:Arc Center = "{Binding Path=PreviousMousePositionPixels}"
        // Stroke="White" 
        // StrokeDashArray="4 4"
        // SnapsToDevicePixels="True"
        // StartAngle="0" 
        // EndAngle="{Binding Path=DeltaAngle}" 
        // SmallAngle="True"
        // Radius="40"/>
    }
}
