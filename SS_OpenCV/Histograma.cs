using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace CG_OpenCV
{
    public partial class Histograma : Form
    {
        public Histograma(int[] array)
        {
            InitializeComponent();
            GraphPane myPane = zedGraphControl1.GraphPane;
            PointPairList greyList = new PointPairList();

            for (int i = 0; i < 256; i++)
            {
                greyList.Add(i, array[i]);
            }

            myPane.AddCurve("Grey", greyList, System.Drawing.Color.Black);
            myPane.AxisChange();
            zedGraphControl1.Refresh();
        }

        public Histograma(int[,] array)
        {
            InitializeComponent();
            GraphPane myPane = zedGraphControl1.GraphPane;
            PointPairList blueList = new PointPairList();
            PointPairList greenList = new PointPairList();
            PointPairList redList = new PointPairList();
            for (int i = 0; i < 256; i++)
            {
                blueList.Add(i, array[0, i]);
                greenList.Add(i, array[1, i]);
                redList.Add(i, array[2, i]);
            }

            myPane.AddCurve("Blue", blueList, System.Drawing.Color.Blue);
            myPane.AddCurve("Green", greenList, System.Drawing.Color.Green);
            myPane.AddCurve("Red", redList, System.Drawing.Color.Red);
            myPane.AxisChange();
            zedGraphControl1.Refresh();
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }
    }

    
}

