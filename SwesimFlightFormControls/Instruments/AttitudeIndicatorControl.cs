using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace se.swesim.flight.formcontrols.Instruments
{
    public class AttitudeIndicatorControl : Control
    {
        private float _pitchDeg;
        private float _rollDeg;

        [Category("Attitude")]
        [Description("Pitch in degrees. Positive = nose up.")]
        public float PitchDegrees
        {
            get => _pitchDeg;
            set { _pitchDeg = value; Invalidate(); }
        }

        [Category("Attitude")]
        [Description("Roll in degrees. Positive = right wing down.")]
        public float RollDegrees
        {
            get => _rollDeg;
            set { _rollDeg = NormalizeRoll(value); Invalidate(); }
        }

        [Category("Attitude")]
        [Description("How many pixels the horizon moves per 1 degree of pitch.")]
        public float PixelsPerPitchDegree { get; set; } = 2.2f;

        [Category("Attitude")]
        [Description("Clamp pitch display to avoid moving the horizon completely out of view.")]
        public float PitchClampDegrees { get; set; } = 30f;

        [Category("Attitude Display")]
        public Color SkyColor { get; set; } = Color.FromArgb(60, 120, 200);

        [Category("Attitude Display")]
        public Color GroundColor { get; set; } = Color.FromArgb(140, 90, 50);

        [Category("Attitude Display")]
        public bool ShowPitchLadder { get; set; } = true;

        public AttitudeIndicatorControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Size = new Size(240, 240);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int size = Math.Min(ClientSize.Width, ClientSize.Height);
            float radius = size / 2f;
            PointF center = new(ClientSize.Width / 2f, ClientSize.Height / 2f);

            DrawBezel(g, center, radius, size);

            // Clip all "inside instrument" drawing to a circular viewport
            float innerRadius = radius * 0.82f;
            using (var clipPath = new GraphicsPath())
            {
                clipPath.AddEllipse(center.X - innerRadius, center.Y - innerRadius, innerRadius * 2, innerRadius * 2);
                var oldClip = g.Clip;
                g.SetClip(clipPath);

                DrawWorld(g, center, innerRadius);      // moving sky/ground + pitch ladder
                g.Clip = oldClip;
            }

            DrawRollScale(g, center, radius);          // fixed outer roll marks
            DrawFixedAircraftSymbol(g, center, radius);// fixed airplane symbol
            DrawFixedRollPointer(g, center, radius);   // little triangle at top (optional but very “real”)
        }

        private void DrawBezel(Graphics g, PointF center, float radius, int size)
        {
            using var back = new SolidBrush(Color.Black);
            g.FillEllipse(back, center.X - radius, center.Y - radius, size, size);

            using var bezelPen = new Pen(Color.Gray, 4);
            g.DrawEllipse(bezelPen, center.X - radius + 2, center.Y - radius + 2, size - 4, size - 4);
        }

        private void DrawWorld(Graphics g, PointF center, float innerRadius)
        {
            // Clamp pitch for display sanity
            float pitch = Math.Max(-PitchClampDegrees, Math.Min(PitchClampDegrees, PitchDegrees));

            // Attitude indicator behavior:
            // - Roll: the world rotates opposite the aircraft
            // - Pitch up (positive): horizon line moves DOWN on the instrument
            float pitchPixels = pitch * PixelsPerPitchDegree;
            float worldRoll = -RollDegrees;

            var old = g.Transform;

            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(worldRoll);
            g.TranslateTransform(0, pitchPixels);

            // Draw giant sky and ground rectangles so rotation never shows gaps
            float big = innerRadius * 4f;

            using (var sky = new SolidBrush(SkyColor))
                g.FillRectangle(sky, -big, -big, big * 2, big);

            using (var ground = new SolidBrush(GroundColor))
                g.FillRectangle(ground, -big, 0, big * 2, big);

            // Horizon line
            using (var horizonPen = new Pen(Color.White, Math.Max(2f, innerRadius * 0.02f)))
                g.DrawLine(horizonPen, -big, 0, big, 0);

            if (ShowPitchLadder)
                DrawPitchLadder(g, innerRadius);

            g.Transform = old;
        }

        private void DrawPitchLadder(Graphics g, float innerRadius)
        {
            // Pitch ladder drawn in the moving "world" coordinate system.
            // We'll draw marks every 5°, label every 10°.
            using var penMajor = new Pen(Color.White, 2);
            using var penMinor = new Pen(Color.White, 1);
            using var brush = new SolidBrush(Color.White);
            using var font = new Font(FontFamily.GenericSansSerif, innerRadius * 0.10f, FontStyle.Bold, GraphicsUnit.Pixel);

            var sfLeft = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            var sfRight = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

            // Lengths (relative)
            float majorHalf = innerRadius * 0.35f;
            float minorHalf = innerRadius * 0.22f;

            // Vertical spacing based on same pitch scale
            for (int deg = -30; deg <= 30; deg += 5)
            {
                if (deg == 0) continue; // horizon already drawn

                float y = -deg * PixelsPerPitchDegree; // positive pitch ladder above horizon for positive deg

                bool major = (deg % 10 == 0);
                float half = major ? majorHalf : minorHalf;

                g.DrawLine(major ? penMajor : penMinor, -half, y, half, y);

                if (major)
                {
                    string label = Math.Abs(deg).ToString();
                    // Labels on both sides
                    g.DrawString(label, font, brush, -half - innerRadius * 0.06f, y, sfLeft);
                    g.DrawString(label, font, brush, half + innerRadius * 0.06f, y, sfRight);
                }
            }
        }

        private void DrawRollScale(Graphics g, PointF center, float radius)
        {
            float scaleRadius = radius * 0.90f;
            float tickOuter = scaleRadius;
            float tickInnerShort = scaleRadius * 0.94f;
            float tickInnerMed = scaleRadius * 0.92f;
            float tickInnerLong = scaleRadius * 0.89f;

            using var pen = new Pen(Color.White, 2);

            // Marks at 5, 10, 15, 45, 90 (on both sides)
            int[] marks = { 5, 10, 15, 45, 90 };

            foreach (int m in marks)
            {
                DrawRollTick(g, center, m, tickOuter, m switch
                {
                    5 => tickInnerShort,
                    10 => tickInnerMed,
                    15 => tickInnerMed,
                    45 => tickInnerLong,
                    90 => tickInnerLong,
                    _ => tickInnerShort
                }, pen);

                DrawRollTick(g, center, -m, tickOuter, m switch
                {
                    5 => tickInnerShort,
                    10 => tickInnerMed,
                    15 => tickInnerMed,
                    45 => tickInnerLong,
                    90 => tickInnerLong,
                    _ => tickInnerShort
                }, pen);
            }

            // Optional: small baseline ticks (every 30°) look nice
            for (int deg = -60; deg <= 60; deg += 30)
            {
                if (deg == 0) continue;
                DrawRollTick(g, center, deg, tickOuter, tickInnerMed, pen);
            }
        }

        private void DrawRollTick(Graphics g, PointF center, float rollDeg, float outerR, float innerR, Pen pen)
        {
            // Roll scale is fixed in the instrument frame.
            // RollDeg here means marker position around top, not aircraft roll.
            // 0° at top, positive to the right.
            double rad = DegToRad(rollDeg);

            float xOuter = center.X + outerR * (float)Math.Sin(rad);
            float yOuter = center.Y - outerR * (float)Math.Cos(rad);

            float xInner = center.X + innerR * (float)Math.Sin(rad);
            float yInner = center.Y - innerR * (float)Math.Cos(rad);

            g.DrawLine(pen, xInner, yInner, xOuter, yOuter);
        }

        private void DrawFixedRollPointer(Graphics g, PointF center, float radius)
        {
            // The little triangle at 12 o'clock (common on many AIs)
            float r = radius * 0.88f;
            float w = radius * 0.05f;
            float h = radius * 0.06f;

            PointF p1 = new(center.X, center.Y - r);
            PointF p2 = new(center.X - w, center.Y - r + h);
            PointF p3 = new(center.X + w, center.Y - r + h);

            using var brush = new SolidBrush(Color.White);
            g.FillPolygon(brush, new[] { p1, p2, p3 });
        }

        private void DrawFixedAircraftSymbol(Graphics g, PointF center, float radius)
        {
            // Simple "from behind" aircraft symbol (static)
            float wingSpan = radius * 0.52f;
            float wingY = center.Y + radius * 0.05f;

            float bodyTop = center.Y - radius * 0.12f;
            float bodyBottom = center.Y + radius * 0.22f;

            float tailY = center.Y + radius * 0.18f;
            float tailSpan = radius * 0.18f;

            using var pen = new Pen(Color.White, Math.Max(2f, radius * 0.03f))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Wings
            g.DrawLine(pen, center.X - wingSpan / 2, wingY, center.X + wingSpan / 2, wingY);

            // Fuselage
            g.DrawLine(pen, center.X, bodyTop, center.X, bodyBottom);

            // Tailplane
            g.DrawLine(pen, center.X - tailSpan / 2, tailY, center.X + tailSpan / 2, tailY);

            // Small center dot
            using var hub = new SolidBrush(Color.Gray);
            float hubR = radius * 0.05f;
            g.FillEllipse(hub, center.X - hubR, center.Y - hubR, hubR * 2, hubR * 2);
        }

        private static float NormalizeRoll(float deg)
        {
            float r = deg % 360f;
            if (r > 180f) r -= 360f;
            if (r < -180f) r += 360f;
            return r;
        }

        private static double DegToRad(double deg) => deg * (Math.PI / 180.0);
    }
}
