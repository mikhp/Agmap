using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Agmap
{
    public partial class Form1 : Form
    {
        public GMapOverlay routes;
        public GMapOverlay MyMark;
        public GMapPolygon polygon;
        public GMapMarker MyShop;
        public GMapMarker Center;
        public PointLatLng lastPosition;
        public GeocodingProvider gp;
        private GMapOverlay polygons = new GMapOverlay("polygon"); //放置polygon的图层
        private GMapPolygon drawingPolygon = null; //正在画的polygon
        private List<PointLatLng> drawingPoints = new List<PointLatLng>(); //多边形的点集
        private GMapPolygon currentPolygon;

        public Form1()
        {
            InitializeComponent();



        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {


            
            this.gMapControl1.BackColor = Color.Red;
            //设置控件的管理模式
            this.gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
            //设置控件显示的地图来源
            this.gMapControl1.MapProvider = GMapProviders.GoogleChinaMap;
            //设置控件显示的当前中心位置

            gp = GMapProviders.OpenStreetMap as GeocodingProvider;

            //31.7543, 121.6281
            this.gMapControl1.Position = new PointLatLng(28.210422, 112.976478);
            //设置控件最大的缩放比例
            this.gMapControl1.MaxZoom = 18;
            //设置控件最小的缩放比例
            this.gMapControl1.MinZoom = 1;
            //设置控件当前的缩放比例
            this.gMapControl1.Zoom = 11;
            //创建一个新图层
            routes = new GMapOverlay("routes");
            MyMark = new GMapOverlay("MyMark");
            routes.IsVisibile = true;//可以显示
            MyMark.IsVisibile = true;
            this.gMapControl1.Overlays.Add(routes);//添加到图层列表中
            this.gMapControl1.Overlays.Add(MyMark);
            //创建一个图标
            GMapMarker gMapMarker = new GMarkerGoogle(gMapControl1.Position, GMarkerGoogleType.arrow);
            //添加图层routes中
            gMapMarker.ToolTipText = "我的商店";
            this.MyMark.Markers.Add(gMapMarker);
            // this.gMapControl1.Dock = DockStyle.Fill;//将控件全屏显示
            gMapControl1.Overlays.Add(polygons);
        }

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            polygons.Polygons.Add(drawingPolygon);
            drawingPolygon = null;
            drawingPoints.Clear();
        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            textBox1.Text = string.Format("x:{0}  y:{1}", point.Lat, point.Lng);
        }


        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                Bitmap bitmap = Bitmap.FromFile("F:\\晴.png") as Bitmap;
                GMapMarker gmm = new GMapMarkerImage(point, bitmap);
                gmm.ToolTipText = string.Format("x:{0}  y:{1}", point.Lat, point.Lng);
                gmm.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                this.MyMark.Markers.Add(gmm);
                //ddd
                drawingPoints.Add(point);
                if (drawingPolygon == null)
                {
                    drawingPolygon = new GMapPolygon(drawingPoints, "my polygon");
                    drawingPolygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
                    drawingPolygon.Stroke = new Pen(Color.Blue, 1);
                    drawingPolygon.IsHitTestVisible = true;
                    polygons.Polygons.Add(drawingPolygon);
                }
                else
                {
                    drawingPolygon.Points.Clear();
                    drawingPolygon.Points.AddRange(drawingPoints);
                    if (polygons.Polygons.Count == 0)
                    {
                        polygons.Polygons.Add(drawingPolygon);
                    }
                    else
                    {
                        gMapControl1.UpdatePolygonLocalPosition(drawingPolygon);
                    }
                }
            
            }          

        }

        private void gMapControl1_OnPolygonEnter(GMapPolygon item)
        {
            currentPolygon = item;
            item.Stroke.Color = Color.Red;
        }

        private void gMapControl1_OnPolygonLeave(GMapPolygon item)
        {
            currentPolygon = null;
            item.Stroke.Color = Color.MidnightBlue;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.MyMark.Clear();
            this.routes.Clear();
            this.polygons.Clear();
        }

    }
}
