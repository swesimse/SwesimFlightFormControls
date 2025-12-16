using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace se.swesim.flight.formcontrols.Instruments
{
    public class HeadingIndicatorControl : Control
    {
        private float _headingDegrees;

        [Category("Heading")]
        [Description("Current heading in degrees (0-360). 0/360 = North, 90 = East.")]
        public float HeadingDegrees
        {
            get => _headingDegrees;
            set
            {
                float normalized = value % 360f;
                if (normalized < 0) normalized += 360f;

                if (Math.Abs(_headingDegrees - normalized) > float.Epsilon)
                {
                    _headingDegrees = normalized;
                    Invalidate();
                }
            }
        }

        [Category("Heading")]
        [Description("Show numeric heading readout in the center.")]
        public bool ShowDigitalReadout { get; set; } = true;

        public HeadingIndicatorControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Size = new Size(220, 220);
        }

        private float _bugHeadingDegrees;

        [Category("Heading Bug")]
        [Description("Selected heading bug in degrees (0-360).")]
        public float BugHeadingDegrees
        {
            get => _bugHeadingDegrees;
            set
            {
                float normalized = value % 360f;
                if (normalized < 0) normalized += 360f;

                if (Math.Abs(_bugHeadingDegrees - normalized) > float.Epsilon)
                {
                    _bugHeadingDegrees = normalized;
                    Invalidate();
                }
            }
        }

        [Category("Heading Bug")]
        public bool ShowHeadingBug { get; set; } = true;

        [Category("Heading Bug")]
        public Color HeadingBugColor { get; set; } = Color.Orange;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int size = Math.Min(ClientSize.Width, ClientSize.Height);
            float radius = size / 2f;
            PointF center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

            DrawBackground(g, center, radius, size);
            DrawCompassCard(g, center, radius);
            DrawFixedIndex(g, center, radius);
            DrawAircraftSilhouette(g, center, radius);

            if (ShowDigitalReadout)
                DrawReadout(g, center, radius);
        }

        private void DrawBackground(Graphics g, PointF center, float radius, int size)
        {
            using var backBrush = new SolidBrush(Color.Black);
            g.FillEllipse(backBrush, center.X - radius, center.Y - radius, size, size);

            using var bezelPen = new Pen(Color.Gray, 4);
            g.DrawEllipse(bezelPen, center.X - radius + 2, center.Y - radius + 2, size - 4, size - 4);
        }

        private void DrawCompassCard(Graphics g, PointF center, float radius)
        {
            // The compass card rotates opposite the heading so that the current heading appears at the top.
            // Example: heading 090 -> rotate card -90 so "E/90" moves to top.
            var old = g.Transform;

            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(-HeadingDegrees);

            float cardRadius = radius * 0.82f;

            // Outer card ring
            using (var ringPen = new Pen(Color.White, 2))
            {
                g.DrawEllipse(ringPen, -cardRadius, -cardRadius, cardRadius * 2, cardRadius * 2);
            }

            // Tick marks and labels
            DrawCardTicksAndLabels(g, cardRadius);

            if (ShowHeadingBug)
                DrawHeadingBug(g, cardRadius);

            g.Transform = old;
        }

        private void DrawCardTicksAndLabels(Graphics g, float cardRadius)
        {
            using var majorPen = new Pen(Color.White, 2);
            using var minorPen = new Pen(Color.White, 1);
            using var textBrush = new SolidBrush(Color.White);

            using var font = new Font(FontFamily.GenericSansSerif, cardRadius * 0.14f, FontStyle.Bold, GraphicsUnit.Pixel);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Ticks every 5°, major every 30°
            for (int deg = 0; deg < 360; deg += 5)
            {
                double rad = Math.PI / 180.0 * deg;

                bool isMajor = (deg % 30 == 0);

                float outer = cardRadius;
                float inner = isMajor ? cardRadius * 0.86f : cardRadius * 0.91f;

                float xOuter = outer * (float)Math.Sin(rad);
                float yOuter = -outer * (float)Math.Cos(rad);
                float xInner = inner * (float)Math.Sin(rad);
                float yInner = -inner * (float)Math.Cos(rad);

                g.DrawLine(isMajor ? majorPen : minorPen, xInner, yInner, xOuter, yOuter);

                // Labels at 0/90/180/270 as N/E/S/W, otherwise 30/60/120 etc.
                if (isMajor)
                {
                    string label = deg switch
                    {
                        0 => "N",
                        90 => "E",
                        180 => "S",
                        270 => "W",
                        _ => (deg / 10).ToString() // classic DG style: 3,6,12,15,21,24,27,33
                    };

                    float labelRadius = cardRadius * 0.72f;
                    float xLabel = labelRadius * (float)Math.Sin(rad);
                    float yLabel = -labelRadius * (float)Math.Cos(rad);

                    var old = g.Transform;

                    g.TranslateTransform(xLabel, yLabel);
                    g.RotateTransform(deg); // key: label is rotated by its compass angle
                    g.DrawString(label, font, textBrush, 0, 0, sf);

                    g.Transform = old;
                }
            }
        }

        private void DrawHeadingBug(Graphics g, float cardRadius)
        {
            // We are already in the rotated card coordinate system (RotateTransform(-HeadingDegrees)).
            // So drawing the bug at BugHeadingDegrees places it correctly relative to the compass card.
            float bugAngleDeg = BugHeadingDegrees;

            double rad = Math.PI / 180.0 * bugAngleDeg;

            float rOuter = cardRadius * 1.02f; // slightly outside the card ring
            float rInner = cardRadius * 0.90f;

            // Point on circle (compass style: 0° is up)
            float xOuter = rOuter * (float)Math.Sin(rad);
            float yOuter = -rOuter * (float)Math.Cos(rad);

            // Make a small triangular "bug" that points inward
            // Build it in local coordinates around the outer point
            var old = g.Transform;

            g.TranslateTransform(xOuter, yOuter);
            g.RotateTransform(bugAngleDeg); // align bug to point inward

            float w = cardRadius * 0.07f;
            float h = cardRadius * 0.10f;

            PointF[] tri =
            {
                new PointF(0, 0),        // tip (on the rim)
                new PointF(-w, h),       // left
                new PointF(+w, h),       // right
            };

            using var brush = new SolidBrush(HeadingBugColor);
            g.FillPolygon(brush, tri);

            // Optional small line extending inward
            using var pen = new Pen(HeadingBugColor, Math.Max(2f, cardRadius * 0.02f));
            g.DrawLine(pen, 0, h, 0, rInner - rOuter + h);

            g.Transform = old;
        }

        private void DrawFixedIndex(Graphics g, PointF center, float radius)
        {
            // Small fixed triangle index at 12 o'clock (non-rotating)
            float r = radius * 0.90f;
            PointF p1 = new(center.X, center.Y - r);
            PointF p2 = new(center.X - radius * 0.04f, center.Y - r + radius * 0.07f);
            PointF p3 = new(center.X + radius * 0.04f, center.Y - r + radius * 0.07f);

            using var brush = new SolidBrush(Color.White);
            g.FillPolygon(brush, new[] { p1, p2, p3 });
        }

        private void DrawAircraftSilhouette(Graphics g, PointF center, float radius)
        {
            // Simple aircraft symbol (static)
            // You can replace this with a fancier silhouette later.
            float bodyLength = radius * 0.32f;
            float wingSpan = radius * 0.45f;
            float tailSpan = radius * 0.18f;

            using var pen = new Pen(Color.White, Math.Max(2f, radius * 0.025f))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Fuselage (nose points up)
            g.DrawLine(pen, center.X, center.Y + bodyLength * 0.35f, center.X, center.Y - bodyLength);

            // Wings
            g.DrawLine(pen, center.X - wingSpan / 2, center.Y, center.X + wingSpan / 2, center.Y);

            // Tailplane
            g.DrawLine(pen, center.X - tailSpan / 2, center.Y + bodyLength * 0.28f,
                            center.X + tailSpan / 2, center.Y + bodyLength * 0.28f);

            // Small nose dot
            using var brush = new SolidBrush(Color.Gray);
            float hub = radius * 0.05f;
            g.FillEllipse(brush, center.X - hub, center.Y - hub, hub * 2, hub * 2);
        }

        private void DrawReadout(Graphics g, PointF center, float radius)
        {
            using var brush = new SolidBrush(Color.White);
            using var font = new Font(FontFamily.GenericSansSerif, radius * 0.12f, FontStyle.Bold, GraphicsUnit.Pixel);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            int hdg = (int)Math.Round(HeadingDegrees);
            if (hdg == 0) hdg = 360;

            g.DrawString(hdg.ToString("000"), font, brush, center.X, center.Y + radius * 0.42f, sf);
        }
    }
}
