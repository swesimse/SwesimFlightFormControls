using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace se.swesim.flight.formcontrols.Instruments
{
    public class TachometerControl : Control
    {
        private float _rpm;

        [Category("Tachometer")]
        [Description("Current engine RPM.")]
        public float Rpm
        {
            get => _rpm;
            set
            {
                _rpm = value;
                Invalidate();
            }
        }

        [Category("Tachometer")]
        [Description("Lowest RPM shown on the scale.")]
        public float StartRpm { get; set; } = 0f;

        [Category("Tachometer")]
        [Description("Highest RPM shown on the scale.")]
        public float EndRpm { get; set; } = 3500f;

        [Category("Tachometer")]
        [Description("Start of the green operating arc.")]
        public float GreenArcStartRpm { get; set; } = 500f;

        [Category("Tachometer")]
        [Description("End of the green operating arc.")]
        public float GreenArcEndRpm { get; set; } = 2600f;

        [Category("Tachometer")]
        [Description("Maximum allowable RPM. Drawn as red line/arc to the end of the scale.")]
        public float MaxAllowableRpm { get; set; } = 2700f;

        [Category("Tachometer")]
        [Description("Scale step in RPM between minor ticks.")]
        public float TickStepRpm { get; set; } = 100f;

        [Category("Tachometer")]
        [Description("Scale step in RPM between numbered labels.")]
        public float LabelStepRpm { get; set; } = 500f;

        [Category("Tachometer")]
        [Description("Start angle of the scale in degrees. 0 = top, 90 = right.")]
        public float ScaleStartAngle { get; set; } = 225f;

        [Category("Tachometer")]
        [Description("End angle of the scale in degrees. 0 = top, 90 = right.")]
        public float ScaleEndAngle { get; set; } = 135f;

        [Category("Appearance")]
        public Color FaceColor { get; set; } = Color.Black;

        [Category("Appearance")]
        public Color BezelColor { get; set; } = Color.FromArgb(35, 35, 35);

        [Category("Appearance")]
        public Color TickColor { get; set; } = Color.White;

        [Category("Appearance")]
        public Color NeedleColor { get; set; } = Color.White;

        [Category("Appearance")]
        public Color GreenArcColor { get; set; } = Color.ForestGreen;

        [Category("Appearance")]
        public Color RedArcColor { get; set; } = Color.Red;

        [Category("Appearance")]
        public bool ShowRpmText { get; set; } = true;

        public TachometerControl()
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

            DrawBezel(g, center, radius);
            DrawColorArcs(g, center, radius);
            DrawScale(g, center, radius);
            DrawNeedle(g, center, radius);
            DrawCenterHub(g, center, radius);

            if (ShowRpmText)
                DrawText(g, center, radius);
        }

        private void DrawBezel(Graphics g, PointF center, float radius)
        {
            RectangleF outer = new(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            using SolidBrush faceBrush = new(FaceColor);
            using Pen bezelPen = new(BezelColor, 6f);
            using Pen innerPen = new(Color.FromArgb(70, 70, 70), 2f);

            g.FillEllipse(faceBrush, outer);
            g.DrawEllipse(bezelPen, outer);

            float innerR = radius * 0.94f;
            g.DrawEllipse(innerPen, center.X - innerR, center.Y - innerR, innerR * 2, innerR * 2);
        }

        private void DrawColorArcs(Graphics g, PointF center, float radius)
        {
            float arcRadius = radius * 0.86f;
            float thickness = Math.Max(6f, radius * 0.08f);

            RectangleF rect = new(
                center.X - arcRadius,
                center.Y - arcRadius,
                arcRadius * 2,
                arcRadius * 2);

            DrawArcForRange(g, rect, thickness, GreenArcColor, GreenArcStartRpm, GreenArcEndRpm);

            if (MaxAllowableRpm < EndRpm)
                DrawArcForRange(g, rect, thickness, RedArcColor, MaxAllowableRpm, EndRpm);
            else
                DrawArcForRange(g, rect, thickness, RedArcColor, MaxAllowableRpm, MaxAllowableRpm + 1f);
        }

        private void DrawArcForRange(Graphics g, RectangleF rect, float thickness, Color color, float rpmStart, float rpmEnd)
        {
            float a1 = RpmToAngle(rpmStart);
            float a2 = RpmToAngle(rpmEnd);

            float gdiStart = CompassToGdiAngle(a1);
            float sweep = ComputeClockwiseSweep(a1, a2);

            using Pen pen = new(color, thickness)
            {
                StartCap = LineCap.Flat,
                EndCap = LineCap.Flat
            };

            g.DrawArc(pen, rect, gdiStart, sweep);
        }

        private void DrawScale(Graphics g, PointF center, float radius)
        {
            using Pen majorPen = new(TickColor, 2f);
            using Pen minorPen = new(TickColor, 1.2f);
            using Brush textBrush = new SolidBrush(TickColor);
            using Font font = new(Font.FontFamily, radius * 0.15f, FontStyle.Bold, GraphicsUnit.Pixel);

            StringFormat sf = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            if (EndRpm <= StartRpm || TickStepRpm <= 0)
                return;

            float firstTick = (float)(Math.Ceiling(StartRpm / TickStepRpm) * TickStepRpm);

            for (float rpm = firstTick; rpm <= EndRpm + 0.01f; rpm += TickStepRpm)
            {
                float angle = RpmToAngle(rpm);
                double rad = Math.PI / 180.0 * angle;

                bool isMajor = Math.Abs((rpm / LabelStepRpm) - Math.Round(rpm / LabelStepRpm)) < 0.001;

                float outer = radius * 0.90f;
                float inner = isMajor ? radius * 0.72f : radius * 0.80f;

                float x1 = center.X + outer * (float)Math.Sin(rad);
                float y1 = center.Y - outer * (float)Math.Cos(rad);
                float x2 = center.X + inner * (float)Math.Sin(rad);
                float y2 = center.Y - inner * (float)Math.Cos(rad);

                g.DrawLine(isMajor ? majorPen : minorPen, x1, y1, x2, y2);

                if (isMajor)
                {
                    float labelR = radius * 0.58f;
                    float lx = center.X + labelR * (float)Math.Sin(rad);
                    float ly = center.Y - labelR * (float)Math.Cos(rad);

                    string label = ((int)Math.Round(rpm / 100f)).ToString();

                    g.DrawString(label, font, textBrush, lx, ly, sf);
                }
            }
        }

        private void DrawNeedle(Graphics g, PointF center, float radius)
        {
            float clamped = Clamp(Rpm, StartRpm, EndRpm);
            float angle = RpmToAngle(clamped);

            var old = g.Transform;

            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);

            float length = radius * 0.78f;
            float tail = radius * 0.12f;

            using Pen pen = new(NeedleColor, Math.Max(2f, radius * 0.035f))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Triangle
            };

            g.DrawLine(pen, 0, tail, 0, -length);

            g.Transform = old;
        }

        private void DrawCenterHub(Graphics g, PointF center, float radius)
        {
            float r = radius * 0.08f;

            using SolidBrush brush = new(Color.FromArgb(30, 30, 30));
            using Pen pen = new(Color.FromArgb(60, 60, 60), 2f);

            g.FillEllipse(brush, center.X - r, center.Y - r, r * 2, r * 2);
            g.DrawEllipse(pen, center.X - r, center.Y - r, r * 2, r * 2);
        }

        private void DrawText(Graphics g, PointF center, float radius)
        {
            using Brush brush = new SolidBrush(TickColor);
            using Font font1 = new(Font.FontFamily, radius * 0.11f, FontStyle.Bold, GraphicsUnit.Pixel);
            using Font font2 = new(Font.FontFamily, radius * 0.08f, FontStyle.Regular, GraphicsUnit.Pixel);

            StringFormat sf = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString("RPM", font1, brush,
                new PointF(center.X, center.Y - radius * 0.10f), sf);

            g.DrawString("HUNDREDS", font2, brush,
                new PointF(center.X, center.Y + radius * 0.00f), sf);
        }

        private float RpmToAngle(float rpm)
        {
            if (EndRpm <= StartRpm)
                return ScaleStartAngle;

            float t = (rpm - StartRpm) / (EndRpm - StartRpm);
            t = Clamp(t, 0f, 1f);

            float start = NormalizeAngle(ScaleStartAngle);
            float end = NormalizeAngle(ScaleEndAngle);

            float sweep = end - start;
            if (sweep <= 0f)
                sweep += 360f;

            float angle = start + t * sweep;
            if (angle >= 360f)
                angle -= 360f;

            return angle;
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }

        private static float CompassToGdiAngle(float compassDeg)
        {
            float a = compassDeg - 90f;
            if (a < 0f) a += 360f;
            return a;
        }

        private static float ComputeClockwiseSweep(float fromCompassDeg, float toCompassDeg)
        {
            float sweep = toCompassDeg - fromCompassDeg;
            if (sweep < 0f) sweep += 360f;
            return sweep;
        }

        private static float Clamp(float value, float min, float max)
            => value < min ? min : (value > max ? max : value);
    }
}