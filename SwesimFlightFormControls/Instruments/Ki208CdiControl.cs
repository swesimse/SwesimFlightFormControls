using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace se.swesim.flight.formcontrols.Instruments
{
    public enum Ki208ToFromState
    {
        Off,
        To,
        From
    }

    public class Ki208CdiControl : Control
    {
        private float _obsDegrees;
        private float _courseDeviationDots;
        private Ki208ToFromState _toFrom = Ki208ToFromState.Off;
        private bool _cdiVisible = true;

        [Category("KI-208")]
        [Description("OBS / selected course in degrees.")]
        public float ObsDegrees
        {
            get => _obsDegrees;
            set
            {
                _obsDegrees = Normalize360(value);
                Invalidate();
            }
        }

        [Category("KI-208")]
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

        [Category("KI-208")]
        public Ki208ToFromState ToFrom
        {
            get => _toFrom;
            set
            {
                _toFrom = value;
                Invalidate();
            }
        }

        [Category("KI-208")]
        public bool CdiVisible
        {
            get => _cdiVisible;
            set
            {
                _cdiVisible = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color FaceColor { get; set; } = Color.Black;

        [Category("Appearance")]
        public Color BezelColor { get; set; } = Color.FromArgb(20, 20, 20);

        [Category("Appearance")]
        public Color TickColor { get; set; } = Color.White;

        [Category("Appearance")]
        public Color NeedleColor { get; set; } = Color.White;

        [Category("Appearance")]
        public Color PointerColor { get; set; } = Color.FromArgb(230, 210, 40);

        [Category("Appearance")]
        public bool ShowBrandText { get; set; } = true;

        [Category("Appearance")]
        public bool ShowObsKnobHint { get; set; } = true;

        public Ki208CdiControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Size = new Size(240, 240);
            Font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold);
            ForeColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            int size = Math.Min(ClientSize.Width, ClientSize.Height);
            float margin = 10f;
            float radius = size / 2f - margin;
            PointF center = new(ClientSize.Width / 2f, ClientSize.Height / 2f);

            DrawFaceplate(g, center, radius);
            DrawDial(g, center, radius);
            DrawObsCard(g, center, radius);
            DrawScaleDots(g, center, radius);
            DrawNeedle(g, center, radius);
            DrawCenterRing(g, center, radius);
            DrawCoursePointers(g, center, radius);
            DrawToFrom(g, center, radius);

            if (ShowBrandText)
                DrawBrandText(g, center, radius);

            if (ShowObsKnobHint)
                DrawObsKnobHint(g, center, radius);
        }

        private void DrawFaceplate(Graphics g, PointF center, float radius)
        {
            float plateSize = radius * 2.20f;
            RectangleF plate = new(center.X - plateSize / 2f, center.Y - plateSize / 2f, plateSize, plateSize);

            using GraphicsPath platePath = CreateChamferedRect(plate, plateSize * 0.10f);
            using SolidBrush plateBrush = new(BezelColor);
            using Pen platePen = new(Color.FromArgb(45, 45, 45), 3f);

            g.FillPath(plateBrush, platePath);
            g.DrawPath(platePen, platePath);

            // Corner screws
            float screwR = radius * 0.07f;
            float inset = radius * 0.86f;
            DrawScrew(g, center.X - inset, center.Y - inset, screwR);
            DrawScrew(g, center.X + inset, center.Y - inset, screwR);
            DrawScrew(g, center.X - inset, center.Y + inset, screwR);
            DrawScrew(g, center.X + inset, center.Y + inset, screwR);
        }

        private void DrawScrew(Graphics g, float x, float y, float r)
        {
            using SolidBrush b = new(Color.FromArgb(10, 10, 10));
            using Pen p = new(Color.FromArgb(30, 30, 30), 1.5f);
            g.FillEllipse(b, x - r, y - r, r * 2, r * 2);
            g.DrawEllipse(p, x - r, y - r, r * 2, r * 2);

            using Pen slotPen = new(Color.FromArgb(45, 45, 45), 1.2f);
            g.DrawLine(slotPen, x - r * 0.45f, y + r * 0.2f, x + r * 0.45f, y - r * 0.2f);
        }

        private void DrawDial(Graphics g, PointF center, float radius)
        {
            RectangleF outer = new(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            using SolidBrush faceBrush = new(FaceColor);
            using Pen outerPen = new(Color.FromArgb(210, 210, 210), 3f);
            using Pen innerPen = new(Color.FromArgb(55, 55, 80), 2f);

            g.FillEllipse(faceBrush, outer);
            g.DrawEllipse(outerPen, outer);

            float innerR = radius * 0.96f;
            g.DrawEllipse(innerPen, center.X - innerR, center.Y - innerR, innerR * 2, innerR * 2);
        }

        private void DrawObsCard(Graphics g, PointF center, float radius)
        {
            Matrix old = g.Transform;

            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(-ObsDegrees);

            float tickOuter = radius * 0.86f;
            float tickInnerMajor = radius * 0.73f;
            float tickInnerMinor = radius * 0.80f;
            float labelRadius = radius * 0.67f;

            using Pen tickPenMajor = new(TickColor, 2f);
            using Pen tickPenMinor = new(TickColor, 1.2f);
            using Brush textBrush = new SolidBrush(TickColor);
            using Font cardinalFont = new(Font.FontFamily, radius * 0.16f, FontStyle.Bold, GraphicsUnit.Pixel);
            using Font numberFont = new(Font.FontFamily, radius * 0.13f, FontStyle.Bold, GraphicsUnit.Pixel);

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

                float outer = tickOuter;
                float inner = majorTick ? tickInnerMajor : tickInnerMinor;

                float x1 = outer * (float)Math.Sin(rad);
                float y1 = -outer * (float)Math.Cos(rad);
                float x2 = inner * (float)Math.Sin(rad);
                float y2 = -inner * (float)Math.Cos(rad);

                g.DrawLine(majorTick ? tickPenMajor : tickPenMinor, x1, y1, x2, y2);

                if (labelTick)
                {
                    string label = deg switch
                    {
                        0 => "N",
                        90 => "E",
                        180 => "S",
                        270 => "W",
                        30 => "3",
                        60 => "6",
                        120 => "12",
                        150 => "15",
                        210 => "21",
                        240 => "24",
                        300 => "30",
                        330 => "33",
                        _ => (deg / 10).ToString()
                    };

                    float lx = labelRadius * (float)Math.Sin(rad);
                    float ly = -labelRadius * (float)Math.Cos(rad);

                    Matrix textOld = g.Transform;
                    g.TranslateTransform(lx, ly);
                    g.RotateTransform(deg);

                    Font useFont = (deg % 90 == 0) ? cardinalFont : numberFont;
                    g.DrawString(label, useFont, textBrush, 0, 0, sf);

                    g.Transform = textOld;
                }
            }

            g.Transform = old;
        }

        private void DrawScaleDots(Graphics g, PointF center, float radius)
        {
            using SolidBrush dotBrush = new(NeedleColor);

            float span = radius * 0.42f;
            float dotSpacing = span / 2.5f;
            float y = center.Y + radius * 0.28f;
            float r = radius * 0.018f;

            for (int i = -2; i <= 2; i++)
            {
                float x = center.X + i * dotSpacing;
                g.FillEllipse(dotBrush, x - r, y - r, r * 2, r * 2);
            }
        }

        private void DrawNeedle(Graphics g, PointF center, float radius)
        {
            if (!CdiVisible)
                return;

            float span = radius * 0.42f;
            float dotSpacing = span / 2.5f;
            float dx = CourseDeviationDots * dotSpacing;

            using Pen needlePen = new(NeedleColor, 4f);

            float topY = center.Y - radius * 0.48f;
            float bottomY = center.Y + radius * 0.30f;

            g.DrawLine(needlePen, center.X + dx, topY, center.X + dx, bottomY);
        }

        private void DrawCenterRing(Graphics g, PointF center, float radius)
        {
            float r = radius * 0.04f;
            using Pen pen = new(NeedleColor, 2f);
            g.DrawEllipse(pen, center.X - r, center.Y - r, r * 2, r * 2);
        }

        private void DrawCoursePointers(Graphics g, PointF center, float radius)
        {
            using Brush brush = new SolidBrush(Color.Yellow);

            float w = radius * 0.055f;
            float h = radius * 0.09f;

            float topY = center.Y - radius * 0.62f;
            float bottomY = center.Y + radius * 0.62f;

            g.FillPolygon(brush, new[]
            {
                new PointF(center.X, topY),
                new PointF(center.X - w, topY + h),
                new PointF(center.X + w, topY + h)
            });

            g.FillPolygon(brush, new[]
            {
                new PointF(center.X, bottomY),
                new PointF(center.X - w, bottomY - h),
                new PointF(center.X + w, bottomY - h)
            });
        }

        private void DrawToFrom(Graphics g, PointF center, float radius)
        {
            if (ToFrom == Ki208ToFromState.Off)
                return;

            using Brush brush = new SolidBrush(TickColor);
            using Font font = new(Font.FontFamily, radius * 0.11f, FontStyle.Bold, GraphicsUnit.Pixel);

            StringFormat sf = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            string text = ToFrom == Ki208ToFromState.To ? "TO" : "FROM";
            PointF pos = new(center.X, center.Y + radius * 0.42f);

            g.DrawString(text, font, brush, pos, sf);
        }

        private void DrawBrandText(Graphics g, PointF center, float radius)
        {
            using Brush brush = new SolidBrush(TickColor);
            using Font font = new(Font.FontFamily, radius * 0.08f, FontStyle.Bold, GraphicsUnit.Pixel);

            StringFormat sf = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            RectangleF rect = new(
                center.X - radius * 0.28f,
                center.Y + radius * 0.90f,
                radius * 0.6f,
                radius * 0.10f);

            g.DrawString("BENDIX/KING", font, brush, rect, sf);
        }

        private void DrawObsKnobHint(Graphics g, PointF center, float radius)
        {
            float knobR = radius * 0.18f;
            PointF knobCenter = new(center.X - radius * 0.78f, center.Y + radius * 0.82f);

            using SolidBrush knobBrush = new(Color.FromArgb(18, 18, 18));
            using Pen outerPen = new(Color.FromArgb(220, 220, 220), 2f);
            using Brush textBrush = new SolidBrush(PointerColor);
            using Font font = new(Font.FontFamily, radius * 0.10f, FontStyle.Bold, GraphicsUnit.Pixel);

            g.FillEllipse(knobBrush, knobCenter.X - knobR, knobCenter.Y - knobR, knobR * 2, knobR * 2);
            g.DrawEllipse(outerPen, knobCenter.X - knobR, knobCenter.Y - knobR, knobR * 2, knobR * 2);

            StringFormat sf = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString("OBS", font, textBrush,
                new RectangleF(knobCenter.X - knobR, knobCenter.Y - knobR, knobR * 2, knobR * 2), sf);
        }

        private static GraphicsPath CreateChamferedRect(RectangleF rect, float cut)
        {
            GraphicsPath path = new();
            path.StartFigure();

            path.AddLine(rect.Left + cut, rect.Top, rect.Right - cut, rect.Top);
            path.AddLine(rect.Right - cut, rect.Top, rect.Right, rect.Top + cut);
            path.AddLine(rect.Right, rect.Top + cut, rect.Right, rect.Bottom - cut);
            path.AddLine(rect.Right, rect.Bottom - cut, rect.Right - cut, rect.Bottom);
            path.AddLine(rect.Right - cut, rect.Bottom, rect.Left + cut, rect.Bottom);
            path.AddLine(rect.Left + cut, rect.Bottom, rect.Left, rect.Bottom - cut);
            path.AddLine(rect.Left, rect.Bottom - cut, rect.Left, rect.Top + cut);
            path.AddLine(rect.Left, rect.Top + cut, rect.Left + cut, rect.Top);

            path.CloseFigure();
            return path;
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