using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace se.swesim.flight.formcontrols.Instruments
{
    public enum AirspeedUnit
    {
        Knots,
        KilometersPerHour,
        MilesPerHour
    }

    public class AirSpeedIndicatorControl : Control
    {
        private double _indicatedSpeed; // stored in *display units* for simplicity
        private AirspeedUnit _unit = AirspeedUnit.Knots;

        // Dial geometry
        private const float StartAngleDeg = 30f;   // ~1 o'clock
        private const float EndAngleDeg = 330f;    // ~11 o'clock (sweep via wrap)
        private const float SweepDeg = 300f;       // clockwise sweep from 30 to 330

        [Category("ASI")]
        public AirspeedUnit Unit
        {
            get => _unit;
            set { _unit = value; Invalidate(); }
        }

        [Category("ASI")]
        [Description("Current indicated airspeed in the selected Unit.")]
        public double IndicatedSpeed
        {
            get => _indicatedSpeed;
            set { _indicatedSpeed = value; Invalidate(); }
        }

        [Category("ASI")]
        [Description("Speed where the scale starts (below this is considered unreliable).")]
        public double StartSpeed { get; set; } = 40;

        [Category("ASI V-Speeds")]
        [Description("Stall speed or minimum steady flight speed in the landing configuration.")]
        public double VSO { get; set; } = 40;

        [Category("ASI V-Speeds")]
        [Description("Maximum flap extended speed.")]
        public double VFE { get; set; } = 85;

        [Category("ASI V-Speeds")]
        [Description("Stall speed or minimum steady flight speed in a specific configuration (typically clean).")]
        public double VS1 { get; set; } = 50;

        [Category("ASI V-Speeds")]
        [Description("Maximum structural cruising speed.")]
        public double VNO { get; set; } = 130;

        [Category("ASI V-Speeds")]
        [Description("Never exceed speed (top of scale).")]
        public double VNE { get; set; } = 160;

        [Category("ASI Scale")]
        [Description("Tick step size (default 5 units).")]
        public int TickStep { get; set; } = 5;

        [Category("ASI Scale")]
        [Description("Label step size (default 20 units).")]
        public int LabelStep { get; set; } = 20;

        [Category("ASI Display")]
        public bool ShowUnitText { get; set; } = true;

        public AirSpeedIndicatorControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Size = new Size(220, 220);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int size = Math.Min(ClientSize.Width, ClientSize.Height);
            float radius = size / 2f;
            PointF center = new(ClientSize.Width / 2f, ClientSize.Height / 2f);

            DrawBackground(g, center, radius, size);
            DrawArcs(g, center, radius);
            DrawScale(g, center, radius);
            DrawNeedle(g, center, radius);

            DrawCenterHub(g, center, radius);
            DrawText(g, center, radius);
        }

        private void DrawBackground(Graphics g, PointF center, float radius, int size)
        {
            using var backBrush = new SolidBrush(Color.Black);
            g.FillEllipse(backBrush, center.X - radius, center.Y - radius, size, size);

            using var bezelPen = new Pen(Color.Gray, 4);
            g.DrawEllipse(bezelPen, center.X - radius + 2, center.Y - radius + 2, size - 4, size - 4);
        }

        private void DrawArcs(Graphics g, PointF center, float radius)
        {
            // Two concentric rings so arcs don't overlap visually.
            float outerArcRadius = radius * 0.86f;  // green/yellow live here
            float innerArcRadius = radius * 0.80f;  // white lives here (flap range)

            float thickness = Math.Max(6f, radius * 0.055f);

            RectangleF RectFor(float r) => new RectangleF(center.X - r, center.Y - r, r * 2, r * 2);

            // White arc (flaps) slightly inside the main arc ring
            DrawArcClamped(g, RectFor(innerArcRadius), thickness, Color.White, VSO, VFE);

            // Green + Yellow on the outer ring
            DrawArcClamped(g, RectFor(outerArcRadius), thickness, Color.LimeGreen, VS1, VNO);
            DrawArcClamped(g, RectFor(outerArcRadius), thickness, Color.Gold, VNO, VNE);

            // Red tick at VNE (use outer geometry so it lines up with the scale)
            DrawRedTick(g, center, radius, VNE);
        }

        private void DrawArcClamped(Graphics g, RectangleF rect, float thickness, Color color, double fromSpeed, double toSpeed)
        {
            // Clamp to [StartSpeed, VNE] so weird aircraft inputs don't explode the dial.
            double min = StartSpeed;
            double max = VNE;

            double a = Clamp(fromSpeed, min, max);
            double b = Clamp(toSpeed, min, max);

            if (b <= a) return;

            float startAngle = SpeedToAngle((float)a);
            float endAngle = SpeedToAngle((float)b);

            // Convert into System.Drawing arc arguments:
            // DrawArc uses degrees clockwise from +x axis, but with start/sweep.
            // Our SpeedToAngle returns "compass style" with 0 at 12 o'clock.
            // We'll convert that to GDI+ angle where 0 is at 3 o'clock.
            float gdiStart = CompassToGdiAngle(startAngle);
            float sweep = ComputeClockwiseSweep(startAngle, endAngle);

            using var pen = new Pen(color, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawArc(pen, rect, gdiStart, sweep);
        }

        private void DrawRedTick(Graphics g, PointF center, float radius, double speed)
        {
            float angle = SpeedToAngle((float)speed);
            double rad = DegToRad(angle);

            float outer = radius * 0.92f;
            float inner = radius * 0.80f; // closer to arc ring

            float xOuter = center.X + outer * (float)Math.Sin(rad);
            float yOuter = center.Y - outer * (float)Math.Cos(rad);
            float xInner = center.X + inner * (float)Math.Sin(rad);
            float yInner = center.Y - inner * (float)Math.Cos(rad);

            using var pen = new Pen(Color.Red, Math.Max(3f, radius * 0.03f));
            g.DrawLine(pen, xInner, yInner, xOuter, yOuter);
        }

        private void DrawScale(Graphics g, PointF center, float radius)
        {
            if (VNE <= StartSpeed) return;

            using var majorPen = new Pen(Color.White, 2);
            using var minorPen = new Pen(Color.White, 1);
            using var textBrush = new SolidBrush(Color.White);

            using var font = new Font(FontFamily.GenericSansSerif, radius * 0.12f, FontStyle.Bold, GraphicsUnit.Pixel);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Round the first tick up to a clean tick step
            int startTick = (int)(Math.Ceiling(StartSpeed / TickStep) * TickStep);
            int endTick = (int)(Math.Floor(VNE / TickStep) * TickStep);

            for (int spd = startTick; spd <= endTick; spd += TickStep)
            {
                float angle = SpeedToAngle(spd);
                double rad = DegToRad(angle);

                bool isLabel = (spd % LabelStep == 0);
                bool isMajor = (spd % 10 == 0); // labels imply major ticks

                float outer = radius * 0.88f;
                float inner = isMajor ? radius * 0.65f : radius * 0.81f;

                float xOuter = center.X + outer * (float)Math.Sin(rad);
                float yOuter = center.Y - outer * (float)Math.Cos(rad);
                float xInner = center.X + inner * (float)Math.Sin(rad);
                float yInner = center.Y - inner * (float)Math.Cos(rad);

                g.DrawLine(isMajor ? majorPen : minorPen, xInner, yInner, xOuter, yOuter);

                if (isLabel)
                {
                    float labelRadius = radius * 0.55f;
                    float xLabel = center.X + labelRadius * (float)Math.Sin(rad);
                    float yLabel = center.Y - labelRadius * (float)Math.Cos(rad);

                    g.DrawString(spd.ToString(), font, textBrush, xLabel, yLabel, sf);
                }
            }
        }

        private void DrawNeedle(Graphics g, PointF center, float radius)
        {
            // Clamp needle to dial range
            double v = Clamp(IndicatedSpeed, StartSpeed, VNE);

            float angle = SpeedToAngle((float)v);

            var old = g.Transform;

            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle); // our angle is "compass style": 0 is up

            float length = radius * 0.78f;
            float tail = radius * 0.12f;

            using var pen = new Pen(Color.White, Math.Max(2f, radius * 0.025f))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Triangle
            };

            // Needle points "up" in local coordinates
            g.DrawLine(pen, 0, tail, 0, -length);

            g.Transform = old;
        }

        private void DrawCenterHub(Graphics g, PointF center, float radius)
        {
            float hub = radius * 0.055f;
            using var hubBrush = new SolidBrush(Color.Gray);
            using var hubPen = new Pen(Color.White, 1);

            var r = new RectangleF(center.X - hub, center.Y - hub, hub * 2, hub * 2);
            g.FillEllipse(hubBrush, r);
            g.DrawEllipse(hubPen, r);
        }

        private void DrawText(Graphics g, PointF center, float radius)
        {
            using var brush = new SolidBrush(Color.White);
            using var big = new Font(FontFamily.GenericSansSerif, radius * 0.12f, FontStyle.Bold, GraphicsUnit.Pixel);
            using var small = new Font(FontFamily.GenericSansSerif, radius * 0.09f, FontStyle.Regular, GraphicsUnit.Pixel);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            string unitText = Unit switch
            {
                AirspeedUnit.Knots => "KNOTS",
                AirspeedUnit.KilometersPerHour => "KM/H",
                AirspeedUnit.MilesPerHour => "MPH",
                _ => ""
            };

            // Put it down low-ish like many ASIs
            if (ShowUnitText)
                g.DrawString(unitText, big, brush, center.X, center.Y + radius * 0.28f, sf);

            // Optional: numeric readout (tiny)
            g.DrawString($"{IndicatedSpeed:0}", small, brush, center.X, center.Y + radius * 0.40f, sf);
        }

        /// <summary>
        /// Maps a speed value to a compass-style angle in degrees where:
        /// 0° = 12 o'clock, 90° = 3 o'clock, 180° = 6 o'clock, 270° = 9 o'clock.
        /// StartSpeed sits near 1 o'clock (30°), VNE sits near 11 o'clock (330°).
        /// </summary>
        private float SpeedToAngle(float speed)
        {
            if (VNE <= StartSpeed) return StartAngleDeg;

            float s = (float)Clamp(speed, StartSpeed, VNE);
            float t = (s - (float)StartSpeed) / ((float)VNE - (float)StartSpeed); // 0..1

            // Sweep from 30° to 330° clockwise via wrap: 30° + t*300°
            float angle = StartAngleDeg + t * SweepDeg;
            if (angle >= 360f) angle -= 360f;

            return angle;
        }

        private static float ComputeClockwiseSweep(float fromCompassDeg, float toCompassDeg)
        {
            // clockwise sweep in compass-angle space
            float sweep = toCompassDeg - fromCompassDeg;
            if (sweep < 0) sweep += 360f;
            return sweep;
        }

        private static float CompassToGdiAngle(float compassDeg)
        {
            // compass 0 at 12 o'clock; GDI+ 0 at 3 o'clock, positive clockwise
            // Convert: compassDeg -> gdiDeg = compassDeg - 90
            float a = compassDeg - 90f;
            if (a < 0) a += 360f;
            return a;
        }

        private static double Clamp(double v, double min, double max)
            => v < min ? min : (v > max ? max : v);

        private static double DegToRad(double deg) => deg * (Math.PI / 180.0);
    }
}