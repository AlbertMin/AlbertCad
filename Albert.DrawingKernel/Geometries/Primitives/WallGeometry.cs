﻿using Albert.Geometry.External;
using Albert.Geometry.Primitives;
using Albert.DrawingKernel.Selector;
using Albert.DrawingKernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Albert.Geometry.Transform;
using System.Xml;

namespace Albert.DrawingKernel.Geometries.Primitives
{
    /// <summary>
    /// 定义当前墙体的绘制类
    /// </summary>
    public class WallGeometry : Geometry2D
    {

        private Vector2D start;

        /// <summary>
        /// 墙的起点
        /// </summary>
        public Vector2D Start
        {

            get { return start; }
            set { start = value; }
        }
        private Vector2D end;

        /// <summary>
        /// 墙的终端
        /// </summary>
        public Vector2D End
        {

            get { return end; }
            set { end = value; }
        }

        /// <summary>
        /// 当前墙体的隔壁墙体
        /// </summary>
        public WallGeometry Next {

            get;
            set;
        }

        /// <summary>
        /// 当前墙体的前一个墙体
        /// </summary>
        public WallGeometry Previous
        {
            get;
            set;
        }
        /// <summary>
        /// 初始化一个多边形
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public WallGeometry(Vector2D start, Vector2D end, double thickness = 89)
        {
            this.start = start;
            this.end = end;
            this.Thickness = thickness;
        }
        /// <summary>
        /// 构造函数，初始化一个直线
        /// </summary>
        public WallGeometry()
        {

        }
        /// <summary>
        /// 获取当前所有的顶点信息
        /// </summary>
        public override List<Vector2D> Points
        {
            get
            {
                List<Vector2D> points = new List<Vector2D>();

                if (Lines != null) {
                    Lines.ForEach(x => {
                        points.Add(x.Start);
                        points.Add(x.End);
                    });
                }

                //四边的交点
                return points;
            }
        }

        private double thickness = 89;
        /// <summary>
        /// 当前墙体的厚度
        /// </summary>
        public double Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = value;
            }
        }
        /// <summary>
        /// 更新当前的绘制
        /// </summary>
        public override void Update()
        {

            this.Draw(null);
        }


        
        /// <summary>
        /// 获取当前所有直线信息
        /// </summary>
        public override List<Line2D> Lines
        {
            get
            {
                List<Line2D> lines = new List<Line2D>();
                if (start != null && end != null)
                {
                    if (this.IsActioning)
                    {
                        Line2D l5 = Line2D.Create(start, end);
                        lines.Add(l5);
                    }
                    else
                    {
                        var halfThickness = thickness / 2;
                        var start3D = new Albert.Geometry.Primitives.Vector3D(Start.X, Start.Y, 0);
                        var end3D = new Albert.Geometry.Primitives.Vector3D(End.X, End.Y, 0);
                        Vector3D lineDir = end3D - start3D;
                        Vector3D offsetDir = lineDir.Cross(new Vector3D(0, 0, 1)).Normalize();
                        Vector3D offv1 = start3D + halfThickness * offsetDir;
                        Vector3D offv2 = start3D - halfThickness * offsetDir;
                        Vector3D offv3 = end3D - halfThickness * offsetDir;
                        Vector3D offv4 = end3D + halfThickness * offsetDir;
                        Line2D l1 = Line2D.Create(TransformUtil.Projection(offv1), TransformUtil.Projection(offv2));
                        Line2D l2 = Line2D.Create(TransformUtil.Projection(offv2), TransformUtil.Projection(offv3));
                        Line2D l3 = Line2D.Create(TransformUtil.Projection(offv3), TransformUtil.Projection(offv4));
                        Line2D l4 = Line2D.Create(TransformUtil.Projection(offv4), TransformUtil.Projection(offv1));
                        Line2D l5 = Line2D.Create(start, end);
                        lines.Add(l1);
                        lines.Add(l2);
                        lines.Add(l3);
                        lines.Add(l4);
                        lines.Add(l5);
                    }
                }
                return lines;
            }
        }

        /// <summary>
        /// 当前图形的着重线
        /// </summary>
        public override List<Line2D> Emphasize
        {

            get
            {
                List<Line2D> lines = new List<Line2D>();
                if (start != null && end != null)
                {
                    Line2D l5 = Line2D.Create(start, end);
                    lines.Add(l5);
                }
                return lines;
            }
        }
        /// <summary>
        /// 捕获当前图形信息
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override IntersectPoint IntersectPoint(Vector2D v)
        {

            for (int i = 0; i < Lines.Count; i++)
            {
                var len = Lines[i].Distance(v);
                if (len < KernelProperty.Tolerance)
                {
                    IntersectPoint ip = new IntersectPoint();
                    var nv = v.ProjectOn(Lines[i]);
                    ip.IntersectPointStyle = 1;
                    ip.Line = Lines[i];
                    ip.Point = nv;
                    return ip;
                }
            }

            return null;
        }

        /// <summary>
        /// 相交计算
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override IntersectPoint IntersectPoint2(Vector2D v)
        {
            return null;
        }

        /// <summary>
        /// 绘制当前的墙体
        /// </summary>
        /// <param name="points"></param>
        protected override void Draw(List<Vector2D> points)
        {
            if (start!=null&&end!=null)
            {
                var halfThickness = Thickness / 2;
                List<Vector2D> fillPoints = new List<Vector2D>();
                var start3D = new Albert.Geometry.Primitives.Vector3D(Start.X, Start.Y, 0);
                var end3D = new Albert.Geometry.Primitives.Vector3D(End.X, End.Y, 0);
                Vector3D lineDir = end3D - start3D;
                Vector3D offsetDir = lineDir.Cross(new Vector3D(0, 0, 1)).Normalize();
                Vector3D offv1 = start3D + halfThickness * offsetDir;
                Vector3D offv2 = start3D - halfThickness * offsetDir;
                Vector3D offv3 = end3D - halfThickness * offsetDir;
                Vector3D offv4 = end3D + halfThickness * offsetDir;
                fillPoints.Add(TransformUtil.Projection(offv1));
                fillPoints.Add(TransformUtil.Projection(offv2));
                fillPoints.Add(TransformUtil.Projection(offv3));
                fillPoints.Add(TransformUtil.Projection(offv4));
                base.DrawFill(fillPoints);
            }
      

        }

        /// <summary>
        /// 当前的线的移动
        /// </summary>
        /// <param name="v"></param>
        public override void Move(Vector2D v)
        {

            this.start.MoveTo(v);
            this.end.MoveTo(v);
            this.Update();
        }
        /// <summary>
        /// 当前线的缩放
        /// </summary>
        /// <param name="zm"></param>
        public override void Scale(double zm)
        {

            this.start = this.start * zm;
            this.end = this.end * zm;
            this.Update();
        }
        /// <summary>
        /// 当前线的旋转
        /// </summary>
        /// <param name="c"></param>
        /// <param name="angle"></param>
        public override void Rolate(Vector2D c, double angle)
        {

            this.start.Rotate(c, angle);
            this.end.Rotate(c, angle);
            this.Update();
        }
        /// <summary>
        /// 当前线的拷贝
        /// </summary>
        /// <param name="v"></param>
        public override Geometry2D Copy(bool isclone)
        {
            var nstart = Vector2D.Create(this.start.X, this.start.Y);
            var nend = Vector2D.Create(this.end.X, this.end.Y);
            WallGeometry wallGeometry = new WallGeometry(nstart, nend);
            if (isclone)
            {
 
            }
            else
            {
                wallGeometry.Element = this.Element;
            }
            wallGeometry.thickness = this.thickness;
            wallGeometry.PenColor = this.PenColor;
            wallGeometry.FillColor = this.FillColor;
            return wallGeometry;
        }

        internal override void Mirror(Geometry2D target, Line2D mirrorLine)
        {
            if (target is WallGeometry)
            {
                WallGeometry wall = (target as WallGeometry);

                this.start = TransformUtil.Mirror(wall.start, mirrorLine);
                this.end = TransformUtil.Mirror(wall.end, mirrorLine);
            }

        }
        /// <summary>
        /// 写入XML信息
        /// </summary>
        /// <param name="sw"></param>
        public override void WriteXML(XmlWriter sw)
        {
            sw.WriteStartElement("WallGeometry");
            sw.WriteAttributeString("Start", this.start.X + " " + this.start.Y);
            sw.WriteAttributeString("End", this.end.X + " " + this.end.Y);
            sw.WriteAttributeString("Thinkness", this.Thickness.ToString());
            sw.WriteEndElement();
        }

        /// <summary>
        /// 读取XML信息
        /// </summary>
        /// <param name="xmlReader"></param>
        public override void ReadXML(XmlReader xmlReader)
        {
            string start = xmlReader.GetAttribute("Start");
            var ns = start.Split(' ');
            this.start = new Vector2D(double.Parse(ns[0]), double.Parse(ns[1]));

            string end = xmlReader.GetAttribute("End");
            var ne = end.Split(' ');
            this.end = new Vector2D(double.Parse(ne[0]), double.Parse(ne[1]));

            string thinknesss = xmlReader.GetAttribute("Thinkness");
            this.thickness = double.Parse(thinknesss);
        }
    }
}
