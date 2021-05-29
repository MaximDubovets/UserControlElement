using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2ControlElement
{
    public partial class Control : UserControl
    {
        Bitmap BM;
        Graphics GR;

        int CellX = 1;
        int CellY = 1;
        bool ChoiceCell = false; // отрисовка сетки
        Pen PenCell;

        bool ChoiceGraphic = false; //отрисовка графика
        double x, y, xxx; //функция
        int X1scr, Y1scr, X2scr, Y2scr; //экранные 
        double Kx, Ky; //масштабные коэффициенты
        double Xminbase = -1, Xmaxbase = 1, Yminbase = -1, Ymaxbase = 1; //прямоугольник
        double Xmin, Xmax, Ymin, Ymax; //рабочий прямоугольник
        int degree;
        double Yscr;

        public double ah = 1.1;
        public double av = 1.1;
        public double ak = 1.1;
        double k = 1;

        bool ChoicePoints = false;
        int Numbpoints = 0;
        double[] Xpoint;
        double[] Ypoint;
        SolidBrush br1;
        public int TypePoints = 0;
        public int SizePoints = 5;

        double[] Coeff;
        Pen PenGraph;
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ReDrawPicture();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            ReDrawPicture();
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ChoiceGraphic)
            {
                k *= Math.Pow(ak, e.Delta / 100);
                label1.Text = string.Format("( {0:f2}, {1:f2})", Xmin + e.X / Kx, Ymin + (BM.Height - e.Y) / Ky);
                ReDrawPicture();
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (ChoiceGraphic)
            {
                label1.Left = e.X + 10;
                label1.Top = e.Y + 10;
                label1.Text = string.Format("( {0:f2}, {1:f2})", Xmin + e.X / Kx, Ymin + (BM.Height - e.Y) / Ky);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            trackBar2.Value = 0;
            k = 1;
            ReDrawPicture();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            label1.Visible = ChoiceGraphic;
            
            
        }


        public Control()
        {
            InitializeComponent();
        }

        private void Control_Load(object sender, EventArgs e)
        {            
            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
            pictureBox1.Width = Width - 50;
            pictureBox1.Height = Height - 50;
            trackBar1.Top = pictureBox1.Height + 5;
            trackBar1.Left = 0;
            trackBar1.Width = pictureBox1.Width;
            trackBar2.Left = pictureBox1.Width + 5;
            trackBar2.Top = 0;
            trackBar2.Height = pictureBox1.Height;
            button1.Left = pictureBox1.Width + 5;
            button1.Top = pictureBox1.Height + 5;
            button1.Width = 40;
            button1.Height = 40;

        }      

        private void Control_Resize_1(object sender, EventArgs e)
        {
                                   
            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
            pictureBox1.Width = Width - 50;
            pictureBox1.Height = Height - 50;
            trackBar1.Top = pictureBox1.Height + 5;
            trackBar1.Left = 0;
            trackBar1.Width = pictureBox1.Width;
            trackBar2.Left = pictureBox1.Width + 5;
            trackBar2.Top = 0;
            trackBar2.Height = pictureBox1.Height;
            button1.Left = pictureBox1.Width + 5;
            button1.Top = pictureBox1.Height + 5;
            button1.Width = 40;
            button1.Height = 40;
        }

        private void ReDrawPicture()
        {
            GR.Clear(pictureBox1.BackColor);
            if (ChoiceCell)
            {
                for (int i = 0; i <= CellX; i++) GR.DrawLine(PenCell,(int)(BM.Width * i / CellX), 0, (int)(BM.Width * i / CellX), BM.Height);
                for (int i = 0; i <= CellY; i++) GR.DrawLine(PenCell, 0, (int)(BM.Height * i / CellY),BM.Width,(int)(BM.Height*i/CellY));
            }

            if (ChoiceGraphic)
            {
                Xmin = (Xminbase + Xmaxbase) / 2 - (Xmaxbase - Xminbase) / 2 * Math.Pow(ah, trackBar1.Value)*k;
                Xmax = (Xminbase + Xmaxbase) / 2 + (Xmaxbase - Xminbase) / 2 * Math.Pow(ah, trackBar1.Value)*k;
                Ymin = (Yminbase + Ymaxbase) / 2 - (Ymaxbase - Yminbase) / 2 * Math.Pow(av, trackBar2.Value)*k;
                Ymax = (Yminbase + Ymaxbase) / 2 + (Ymaxbase - Yminbase) / 2 * Math.Pow(av, trackBar2.Value)*k;
                Kx = BM.Width / (Xmax - Xmin);
                Ky = BM.Height / (Ymax - Ymin);

                // Xscr > x > f(x) > y > Yscr
                //Xscr = (int)(Kx(x - Xmin));
                //Yscr = (int)(BM.Height-Ky*(y-Ymin));

                x = Xmin;
                y = 0;
                xxx = 1;
                
                for (int i = degree; i >= 0; i--)                
                {
                    y += Coeff[i] * xxx;
                    xxx *= x;
                }
                X1scr = 0;
                Yscr = BM.Height - Ky * (y - Ymin);
                if (Yscr < 0) { Y1scr = -1; }
                else
                {
                    if (Yscr > BM.Height) { Y1scr = BM.Height + 1; }
                    else { Y1scr = (int)Yscr; }
                }
               

                //Y1scr = (int)(BM.Height - Ky * (y - Ymin));

                for (int i = 1; i <= BM.Width; i++)  
                {
                    X2scr = i;
                    x = Xmin + X2scr / Kx;
                    y = 0;
                    xxx = 1;
                    for (int j = degree; j >= 0; j--)
                    {
                        y += Coeff[j] * xxx;
                        xxx *= x;
                    }
                    // Y2scr = (int)(BM.Height - Ky * (y - Ymin));
                    Yscr = BM.Height - Ky * (y - Ymin);
                    if (Yscr < 0) { Y2scr = -1; }
                    else
                    {
                        if (Yscr > BM.Height) { Y2scr = BM.Height + 1; }
                        else { Y2scr = (int)Yscr; }
                    }
                    GR.DrawLine(PenGraph, X1scr, Y1scr, X2scr, Y2scr);
                    X1scr = X2scr;
                    Y1scr = Y2scr;
                }
            }

            if (ChoicePoints)
            {
                for (int i = 0; i < Numbpoints; i++)
                {
                    int Xscr =(int)(Kx * (Xpoint[i] - Xmin));
                    int Yscr = (int)(BM.Height - Ky * (Ypoint[i] - Ymin));
                    switch (TypePoints)
                    {
                        case 0:
                            GR.FillEllipse(br1, Xscr - SizePoints, Yscr - SizePoints, 2 * SizePoints, 2 * SizePoints);
                            break;

                        case 1:
                            GR.FillRectangle(br1, Xscr - SizePoints, Yscr - SizePoints, 2 * SizePoints, 2 * SizePoints);
                            break;
                    }
                }
            }



            pictureBox1.Image = BM;
        }


        public void InitGraphics()
        {
            BM = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            GR = Graphics.FromImage(BM);
        }

        public void SetBackColor(Color cl) //задний фон 
        {
            pictureBox1.BackColor = cl;
        }

        public void AddCell(int X, int Y, Color cl, int W) //сетка
        {
            ChoiceCell = true;
            PenCell = new Pen(cl, W);
            CellX = X;
            CellY = Y;
            ReDrawPicture();
        }

        public void DeleteCell()
        {
            if (ChoiceCell)
            {
                ChoiceCell = false;
                PenCell.Dispose();
                ReDrawPicture();
            }
        }

        public void AddPolynomGraphic(int n, double[] A, Color cl, int w)
        {
                Coeff = new double[n + 1];
                for (int i = 0; i <= n; i++) Coeff[i] = A[i];
                PenGraph = new Pen(cl, w);
                degree = n;
                ChoiceGraphic = true;
                ReDrawPicture();

        }

        public void DeletePolynomGraphic()
        {
            if (ChoiceGraphic)
            {
                ChoiceGraphic = false;
                PenGraph.Dispose();
                ReDrawPicture();
            }
        }

        public void AddPoints(int N, double[] X, double[] Y, Color cl)
        {
            ChoicePoints = true;
            Numbpoints = N;
            Xpoint = new double[N];
            Ypoint = new double[N];
            br1 = new SolidBrush(cl);
            for (int i = 0; i < N; i++)
            {
                Xpoint[i] = X[i];
                Ypoint[i] = Y[i];
            }
            ReDrawPicture();       
        }

        public void DeletePoints()
        {
            if (ChoicePoints)
            {
                ChoicePoints = false;
                br1.Dispose();
                Array.Clear(Xpoint, 0, Numbpoints);
                Array.Clear(Ypoint, 0, Numbpoints);
                ReDrawPicture();
            }
        }
    }
}
