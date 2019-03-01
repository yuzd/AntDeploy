using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AltoControls
{
    public class SpinningCircles : Control
    {
        bool fullTransparency = true;
        float increment = 1f;
        float radius = 2.5f;
        int n = 8;
        int next = 0;
        System.Windows.Forms.Timer timer;
        public SpinningCircles()
        {
            Width = 90;
            Height = 100;
            timer = new System.Windows.Forms.Timer();
            timer.Tick += timer_Tick;
            timer.Enabled = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (fullTransparency)
            {
                Transparenter.MakeTransparent(this, e.Graphics);
            }
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            int length = Math.Min(Width, Height);
            PointF center = new PointF(length / 2, length / 2);
            float bigRadius = length / 2 - radius - (n - 1) * increment;
            float unitAngle = 360 / n;
            next++;
            next = next >= n ? 0 : next;
            int a = 0;
            for (int i = next; i < next + n; i++)
            {
                int factor = i % n;
                float c1X = center.X + (float)(bigRadius * Math.Cos(unitAngle * factor * Math.PI / 180));
                float c1Y = center.Y + (float)(bigRadius * Math.Sin(unitAngle * factor * Math.PI / 180));
                float currRad = radius + a * increment;
                PointF c1 = new PointF(c1X - currRad, c1Y - currRad);
                e.Graphics.FillEllipse(Brushes.Black, c1.X, c1.Y, 2 * currRad, 2 * currRad);
                using (Pen pen = new Pen(Color.White, 2))
                    e.Graphics.DrawEllipse(pen, c1.X, c1.Y, 2 * currRad, 2 * currRad);
                a++;
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            timer.Enabled = Visible;
            base.OnVisibleChanged(e);
        }
        public bool FullTransparent
        {
            get
            {
                return fullTransparency;
            }
            set
            {
                fullTransparency = value;
            }
        }
        public int N
        {
            get
            {
                return n;
            }
            set
            {
                n = value >= 2 ? value : 2;
                Invalidate();
            }
        }
        public float Increment
        {
            get
            {
                return increment;
            }
            set
            {
                increment = value >= 0 ? value : 0;
                Invalidate();
            }
        }
        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value >= 1 ? value : 1;
                Invalidate();
            }
        }
    }
}
