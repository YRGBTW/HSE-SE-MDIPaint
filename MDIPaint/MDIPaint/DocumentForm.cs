using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class DocumentForm : Form
    {
        private MainForm parent;
        private int x, y;
        public Bitmap bitmap;
        private Graphics g;
        public static int starEndCount;
        public string Filename;
        public bool isOpenedFile;
        public DocumentForm(MainForm frm)
        {
            InitializeComponent();
            bitmap = new Bitmap(this.Width,this.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            parent = frm;
        }
        public DocumentForm(MainForm frm, Bitmap bmp)
        {
            InitializeComponent();
            parent = frm;
            bitmap = bmp;
        }

        private void DocumentForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                x = e.X;
                y = e.Y;
            } 
        }

        private void DocumentForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (MainForm.Tool)
                {
                    case "Pen":
                        {
                            g = Graphics.FromImage(bitmap);
                            g.DrawLine(new Pen(MainForm.Color, MainForm.Width), x, y, e.X, e.Y);
                            Invalidate();
                            x = e.X;
                            y = e.Y;
                            break;
                        }
                    case "Eraser":
                        {
                            g = Graphics.FromImage(bitmap);
                            g.DrawLine(new Pen(Color.White, MainForm.Width), x, y, e.X, e.Y);
                            Invalidate();
                            x = e.X;
                            y = e.Y;
                            break;
                        }
                    case "Line":
                        {
                            Refresh();
                            Graphics gr = CreateGraphics();
                            gr.DrawLine(new Pen(MainForm.Color, MainForm.Width),x,y,e.X,e.Y);
                            break;

                        }
                    case "Elypse":
                        {

                            Refresh();
                            Graphics gr = CreateGraphics();
                            gr.DrawEllipse(new Pen(MainForm.Color, MainForm.Width), x, y, e.X-x, e.Y-y);
                            Invalidate();
                            break;
                        }
                    case "Star":
                        {
                            Refresh();
                            Graphics gr = CreateGraphics();
                            PointF[] points = GetStarPoints(DocumentForm.starEndCount, new Rectangle(new Point(x, y), new Size(e.X - x, e.Y - y)));
                            gr.DrawPolygon(new Pen(MainForm.Color, MainForm.Width), points);
                            Invalidate();
                            break;

                        }
                }

            }

        }
        public void SizeChange(int height, int width)
        {
            Bitmap bmpClone = (Bitmap) bitmap.Clone();
            bitmap = new Bitmap(width, height);
            g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            for (int x = 0; x < bmpClone.Width && x < bitmap.Width; x++)
            {
                for (int y = 0; y < bmpClone.Height && y < bitmap.Height; y++)
                {
                    bitmap.SetPixel(x, y, bmpClone.GetPixel(x, y));
                }
            }
            Invalidate();
            this.Size = new Size(width, height);
        }
        public static void ScaleUp(DocumentForm form)
        {
            form.bitmap = new Bitmap(form.bitmap, new Size(form.bitmap.Width + 25, form.bitmap.Height + 25));
            form.Invalidate();
        }
        public static void ScaleDown(DocumentForm form)
        {
            form.bitmap = new Bitmap(form.bitmap, new Size(form.bitmap.Width - 25, form.bitmap.Height - 25));
            form.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(bitmap, 0, 0);
        }

        public Bitmap GetBitmap()
        {
            return this.bitmap;
        }

        private void DocumentForm_MouseUp(object sender, MouseEventArgs e)
        {
            switch(MainForm.Tool)
            {
                case "Line":
                    {
                        Graphics gr = Graphics.FromImage(bitmap);
                        gr.DrawLine(new Pen(MainForm.Color, MainForm.Width), x, y, e.X, e.Y);
                        x = e.X; y = e.Y;
                        Invalidate();
                        break;
                    }
                case "Elypse":
                    {
                        Graphics gr = Graphics.FromImage(bitmap);
                        gr.DrawEllipse(new Pen(MainForm.Color, MainForm.Width), x, y, e.X-x, e.Y-y);
                        x = e.X; y = e.Y;
                        Invalidate();
                        break;
                    }
                case "Star":
                    {
                        Graphics gr = Graphics.FromImage(bitmap);
                        PointF[] points = GetStarPoints(DocumentForm.starEndCount, new Rectangle(new Point(x, y), new Size(e.X - x, e.Y - y)));
                        gr.DrawPolygon(new Pen(MainForm.Color, MainForm.Width), points);
                        Invalidate();
                        break;
                        
                    }

            }

        }

        private void DocumentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.bitmap != null)
            {
                DialogResult result = MessageBox.Show($"Холст был изменен. Сохранить?", "Внимание", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop);
                if (result == DialogResult.Yes)
                {
                    parent.SaveBeforeClosing(sender,e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
        private PointF[] GetStarPoints(int numEnds, Rectangle rectangle)
        {
            PointF[] points = new PointF[numEnds];

            double rx = rectangle.Width / 2;
            double ry = rectangle.Height / 2;
            double cx = rectangle.X + rx;
            double cy = rectangle.Y + ry;

            double theta = -Math.PI / 2;
            double dtheta = 4 * Math.PI / numEnds;

            for (int i = 0; i < numEnds; i++)
            {
                points[i] = new PointF(
                    (float)(cx + rx * Math.Cos(theta)),
                    (float)(cy + ry * Math.Sin(theta)));
                theta += dtheta;
            }

            return points;
        }
    }
}
