using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace se.swesim.flight.formcontrols.Instruments
{
    public enum Gi106BSource
    {
        GPS,
        NAV,
        VLOC
    }

    public enum ToFromState
    {
        Off,
        To,
        From
    }

    public class Gi106BCdiControl : Control
    {
        private float _obsDegrees;
        private float _courseDeviationDots;
        private float _glideSlopeDots;
        private bool _showGlideSlope = true;
        private bool _cdiVisible = true;
        private bool _gsVisible = true;
        private Gi106BSource _source = Gi106BSource.GPS;
        private ToFromState _toFrom = ToFromState.Off;

        [Category("GI-106B")]
        public float ObsDegrees
        {
            get => _obsDegrees;
            set
            {
                _obsDegrees = Normalize360(value);
                Invalidate();
            }
        }

        [Category("GI-106B")]
        [Description("CDI deviation in dots. Negative = left, positive = right.")]
        public float CourseDeviationDots
        {
            get => _courseDeviationDots;
            set
            {
                _courseDeviationDots = Clamp(value, -2.5f, 2.5f);
                Invalidate();
            }
        }

        [Category("GI-106B")]
        [Description("Glideslope deviation in dots. Negative = below, positive = above.")]
        public float GlideSlopeDots
        {
            get => _glideSlopeDots;
            set
            {
                _glideSlopeDots = Clamp(value, -2.5f, 2.5f);
                Invalidate();
            }
        }

        [Category("GI-106B")]
        public bool ShowGlideSlope
        {
            get => _showGlideSlope;
            set
            {
                _showGlideSlope = value;
                Invalidate();
            }
        }

        [Category("GI-106B")]
        public bool CdiVisible
        {
            get => _cdiVisible;
            set
            {
                _cdiVisible = value;
                Invalidate();
            }
        }

        [Category("GI-106B")]
        public bool GsVisible
        {
            get => _gsVisible;
            set
            {
                _gsVisible = value;
                Invalidate();
            }
        }

        [Category("GI-106B")]
        public Gi106BSource Source
        {
            get => _source;
            set
            {
                _source = value;
                Invalidate();
            }
        }

        [Category("GI-106B")]
        public ToFromState ToFrom
        {
            get => _toFrom;
            set
            {
                _toFrom = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color NeedleColor { get; set; } = Color.White;

        [Category("Appearance")]
        public Color AnnunciatorColor { get; set; } = Color.LimeGreen;

        [Category("Appearance")]
        public Color FaceColor { get; set; } = Color.Black;

        [Category("Appearance")]
        public Color BezelColor { get; set; } = Color.FromArgb(40, 40, 40);

        [Category("Appearance")]
        public Color TickColor { get; set; } = Color.White;

        public Gi106BCdiControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Size = new Size(220, 220);
            Font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold);
            ForeColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int size = Math.Min(ClientSize.Width, ClientSize.Height);
            float margin = 6f; // or slightly larger than your thickest pen / marker
            float radius = size / 2f - margin;
            PointF center = new(ClientSize.Width / 2f, ClientSize.Height / 2f);

            DrawBezel(g, center, radius);
            DrawObsCard(g, center, radius);
            DrawNeedleScale(g, center, radius);
            DrawNeedles(g, center, radius);
            DrawCenterHub(g, center, radius);
            DrawAnnunciators(g, center, radius);
            DrawFixedPointers(g, center, radius);
        }

        private void DrawBezel(Graphics g, PointF center, float radius)
        {
            RectangleF outer = new(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            using SolidBrush faceBrush = new(FaceColor);
            using Pen bezelPen = new(BezelColor, 6);

            g.FillEllipse(faceBrush, outer);
            g.DrawEllipse(bezelPen, outer);

            // Inner face ring
            float innerR = radius * 0.86f;
            RectangleF inner = new(center.X - innerR, center.Y - innerR, innerR * 2, innerR * 2);
            using Pen innerPen = new(Color.FromArgb(90, 90, 90), 2);
            g.DrawEllipse(innerPen, inner);
        }

        private void DrawObsCard(Graphics g, PointF center, float radius)
        {
            Matrix old = g.Transform;

            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(-ObsDegrees);

            // Moved outward to free up space in the middle
            float ringOuter = radius * 0.90f;
            float ringInnerMajor = radius * 0.76f;
            float ringInnerMinor = radius * 0.84f;

            using Pen tickPenMajor = new(TickColor, 2);
            using Pen tickPenMinor = new(TickColor, 1.2f);
            using Brush textBrush = new SolidBrush(TickColor);
            using Font font = new(Font.FontFamily, radius * 0.11f, FontStyle.Bold, GraphicsUnit.Pixel);

            StringFormat sf = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            for (int deg = 0; deg < 360; deg += 5)
            {
                bool majorTick = deg % 10 == 0;
                bool labelTick = deg % 30 == 0;

                double rad = Math.PI / 180.0 * deg;

                float outer = ringOuter;
                float inner = majorTick ? ringInnerMajor : ringInnerMinor;

                float x1 = outer * (float)Math.Sin(rad);
                float y1 = -outer * (float)Math.Cos(rad);
                float x2 = inner * (float)Math.Sin(rad);
                float y2 = -inner * (float)Math.Cos(rad);

                g.DrawLine(majorTick ? tickPenMajor : tickPenMinor, x1, y1, x2, y2);

                if (labelTick)
                {
                    string label = deg switch
                    {
                        0 => "0",
                        30 => "3",
                        60 => "6",
                        90 => "9",
                        120 => "12",
                        150 => "15",
                        180 => "18",
                        210 => "21",
                        240 => "24",
                        270 => "27",
                        300 => "30",
                        330 => "33",
                        _ => (deg / 10).ToString()
                    };

                    // Also moved outward with the tick ring
                    float labelR = radius * 0.68f;
                    float lx = labelR * (float)Math.Sin(rad);
                    float ly = -labelR * (float)Math.Cos(rad);

                    Matrix textOld = g.Transform;
                    g.TranslateTransform(lx, ly);
                    g.RotateTransform(deg);
                    g.DrawString(label, font, textBrush, 0, 0, sf);
                    g.Transform = textOld;
                }
            }

            g.Transform = old;
        }

        private void DrawNeedleScale(Graphics g, PointF center, float radius)
        {
            using Pen pen = new(NeedleColor, 2);

            float halfSpan = radius * 0.42f;
            float dotSpacing = halfSpan / 2.5f;

            g.DrawLine(pen, center.X - halfSpan, center.Y, center.X + halfSpan, center.Y);
            g.DrawLine(pen, center.X, center.Y - halfSpan, center.X, center.Y + halfSpan);

            for (int i = -2; i <= 2; i++)
            {
                if (i == 0) continue;

                float x = center.X + i * dotSpacing;
                DrawSmallArrowPair(g, x, center.Y, true, radius);

                float y = center.Y + i * dotSpacing;
                DrawSmallArrowPair(g, center.X, y, false, radius);
            }
        }

        private void DrawSmallArrowPair(Graphics g, float x, float y, bool horizontalScale, float radius)
        {
            using Pen pen = new(NeedleColor, 1.6f);

            float s = radius * 0.03f;
            if (horizontalScale)
            {
                g.DrawLine(pen, x - s, y, x + s, y);
                g.DrawLine(pen, x - s, y, x - s * 0.4f, y - s * 0.6f);
                g.DrawLine(pen, x - s, y, x - s * 0.4f, y + s * 0.6f);
                g.DrawLine(pen, x + s, y, x + s * 0.4f, y - s * 0.6f);
                g.DrawLine(pen, x + s, y, x + s * 0.4f, y + s * 0.6f);
            }
            else
            {
                g.DrawLine(pen, x, y - s, x, y + s);
                g.DrawLine(pen, x, y - s, x - s * 0.6f, y - s * 0.4f);
                g.DrawLine(pen, x, y - s, x + s * 0.6f, y - s * 0.4f);
                g.DrawLine(pen, x, y + s, x - s * 0.6f, y + s * 0.4f);
                g.DrawLine(pen, x, y + s, x + s * 0.6f, y + s * 0.4f);
            }
        }

        private void DrawNeedles(Graphics g, PointF center, float radius)
        {
            float halfSpan = radius * 0.42f;
            float dotSpacing = halfSpan / 2.5f;

            if (CdiVisible)
            {
                float dx = CourseDeviationDots * dotSpacing;
                using Pen cdiPen = new(NeedleColor, 4);

                g.DrawLine(cdiPen,
                    center.X + dx,
                    center.Y - halfSpan,
                    center.X + dx,
                    center.Y + halfSpan);
            }

            if (ShowGlideSlope && GsVisible)
            {
                float dy = -GlideSlopeDots * dotSpacing;
                using Pen gsPen = new(NeedleColor, 4);

                g.DrawLine(gsPen,
                    center.X - halfSpan,
                    center.Y + dy,
                    center.X + halfSpan,
                    center.Y + dy);
            }
        }

        private void DrawCenterHub(Graphics g, PointF center, float radius)
        {
            float r = radius * 0.07f;
            using Pen pen = new(NeedleColor, 2);
            g.DrawEllipse(pen, center.X - r, center.Y - r, r * 2, r * 2);
        }

        private void DrawAnnunciators(Graphics g, PointF center, float radius)
        {
            using Brush activeBrush = new SolidBrush(AnnunciatorColor);
            using Brush inactiveBrush = new SolidBrush(Color.FromArgb(90, 90, 90));
            using Font font = new(Font.FontFamily, radius * 0.11f, FontStyle.Bold, GraphicsUnit.Pixel);

            StringFormat left = new()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            StringFormat centerFmt = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            //
            // Left-side source annunciators
            //
            float leftX = center.X - radius * 0.42f;

            g.DrawString("NAV", font,
                Source == Gi106BSource.NAV ? activeBrush : inactiveBrush,
                new PointF(leftX, center.Y - radius * 0.24f), left);

            g.DrawString("VLOC", font,
                Source == Gi106BSource.VLOC ? activeBrush : inactiveBrush,
                new PointF(leftX, center.Y - radius * 0.1f), left);

            g.DrawString("GPS", font,
                Source == Gi106BSource.GPS ? activeBrush : inactiveBrush,
                new PointF(leftX, center.Y + radius * 0.16f), left);

            //
            // TO / FROM in upper-right quadrant
            //
            float tfX = center.X + radius * 0.33f;
            float toY = center.Y - radius * 0.34f;
            float fromY = center.Y - radius * 0.22f;

            g.DrawString("TO", font,
                ToFrom == ToFromState.To ? activeBrush : inactiveBrush,
                new PointF(tfX, toY), centerFmt);

            g.DrawString("FROM", font,
                ToFrom == ToFromState.From ? activeBrush : inactiveBrush,
                new PointF(tfX, fromY), centerFmt);
        }

        private void DrawFixedPointers(Graphics g, PointF center, float radius)
        {
            float yTop = center.Y - radius * 0.6f;
            float yBottom = center.Y + radius * 0.6f;
            float w = radius * 0.05f;
            float h = radius * 0.07f;

            using Brush brush = new SolidBrush(Color.Yellow);

            g.FillPolygon(brush, new[]
            {
                new PointF(center.X, yTop),
                new PointF(center.X - w, yTop + h),
                new PointF(center.X + w, yTop + h),
            });

            g.FillPolygon(brush, new[]
            {
                new PointF(center.X, yBottom),
                new PointF(center.X - w, yBottom - h),
                new PointF(center.X + w, yBottom - h),
            });
        }

        private static float Normalize360(float value)
        {
            float v = value % 360f;
            if (v < 0) v += 360f;
            return v;
        }

        private static float Clamp(float value, float min, float max)
            => value < min ? min : (value > max ? max : value);
    }
}