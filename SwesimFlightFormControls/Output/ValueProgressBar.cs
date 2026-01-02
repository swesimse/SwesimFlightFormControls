using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace se.swesim.flight.formcontrols.Controls
{
    public class ValueProgressBar : Control
    {
        private double _minimum = 0;
        private double _maximum = 100;
        private double _value = 0;

        [Category("Behavior")]
        public double Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_maximum < _minimum) _maximum = _minimum;
                if (_value < _minimum) _value = _minimum;
                Invalidate();
            }
        }

        [Category("Behavior")]
        public double Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_maximum < _minimum) _minimum = _maximum;
                if (_value > _maximum) _value = _maximum;
                Invalidate();
            }
        }

        [Category("Behavior")]
        public double Value
        {
            get => _value;
            set
            {
                var clamped = Clamp(value, _minimum, _maximum);
                if (Math.Abs(_value - clamped) > double.Epsilon)
                {
                    _value = clamped;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        public Color BarColor { get; set; } = Color.FromArgb(60, 120, 255);

        [Category("Appearance")]
        public Color NegativeBarColor { get; set; } = Color.FromArgb(255, 120, 60);

        [Category("Appearance")]
        public Color BorderColor { get; set; } = Color.Gray;

        [Category("Appearance")]
        public Color TrackColor { get; set; } = Color.FromArgb(30, 30, 30);

        [Category("Appearance")]
        public bool ShowZeroLine { get; set; } = true;

        [Category("Appearance")]
        public Color ZeroLineColor { get; set; } = Color.FromArgb(180, 180, 180);

        [Category("Appearance")]
        [Description("Format string used when displaying value. Example: \"{0:0.0}\" or \"{0:0}%\"")]
        public string ValueFormat { get; set; } = "{0:0}";

        [Category("Appearance")]
        [Description("If true, shows \"Value\". If false, shows \"Value / Max\".")]
        public bool ShowValueOnly { get; set; } = true;

        [Category("Appearance")]
        public int CornerRadius { get; set; } = 6;

        [Category("Appearance")]
        [Description("Optional unit suffix appended to the displayed value, e.g. %, kt, °.")]
        public string UnitSuffix { get; set; } = string.Empty;

        public ValueProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Size = new Size(200, 24);
            Font = new Font(FontFamily.GenericSansSerif, 9f, FontStyle.Bold);

            ForeColor = Color.White; // 👈 important default for dark background
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            if (rect.Width <= 2 || rect.Height <= 2) return;

            rect.Inflate(-1, -1);

            using var trackPath = RoundedRect(rect, CornerRadius);
            using var trackBrush = new SolidBrush(TrackColor);
            using var borderPen = new Pen(BorderColor, 1);

            g.FillPath(trackBrush, trackPath);
            g.DrawPath(borderPen, trackPath);

            // Compute fill rectangle(s)
            if (_maximum <= _minimum)
                return;

            // Helper mapping value->x
            float XOf(double v)
            {
                double t = (v - _minimum) / (_maximum - _minimum);
                t = Math.Max(0, Math.Min(1, t));
                return rect.Left + (float)(t * rect.Width);
            }

            bool crossesZero = (_minimum < 0 && _maximum > 0);
            float xMin = rect.Left;
            float xMax = rect.Right;
            float xVal = XOf(_value);

            if (crossesZero)
            {
                float xZero = XOf(0);

                if (ShowZeroLine)
                {
                    using var zeroPen = new Pen(ZeroLineColor, 1);
                    g.DrawLine(zeroPen, xZero, rect.Top + 2, xZero, rect.Bottom - 2);
                }

                if (_value >= 0)
                {
                    // Fill from zero to value
                    var fill = RectangleF.FromLTRB(xZero, rect.Top, xVal, rect.Bottom);
                    DrawFill(g, rect, fill, BarColor);
                }
                else
                {
                    // Fill from value to zero (left side)
                    var fill = RectangleF.FromLTRB(xVal, rect.Top, xZero, rect.Bottom);
                    DrawFill(g, rect, fill, NegativeBarColor);
                }
            }
            else
            {
                // Normal fill from min to value
                var fill = RectangleF.FromLTRB(xMin, rect.Top, xVal, rect.Bottom);

                // Choose negative color if the entire range is negative (optional but intuitive)
                var color = (_maximum <= 0) ? NegativeBarColor : BarColor;
                DrawFill(g, rect, fill, color);
            }

            DrawCenteredText(g, rect);
        }

        private void DrawFill(Graphics g, Rectangle trackRect, RectangleF fillRect, Color color)
        {
            if (fillRect.Width <= 0.5f) return;

            // Clip fill to rounded rectangle so corners stay clean
            using var clipPath = RoundedRect(trackRect, CornerRadius);
            var oldClip = g.Clip;
            g.SetClip(clipPath);

            using var fillBrush = new SolidBrush(color);
            g.FillRectangle(fillBrush, fillRect);

            g.Clip = oldClip;
        }

        private void DrawCenteredText(Graphics g, Rectangle rect)
        {
            string valueText = string.Format(ValueFormat, _value);

            if (!string.IsNullOrWhiteSpace(UnitSuffix))
                valueText += UnitSuffix;

            string text = ShowValueOnly
                ? valueText
                : $"{valueText} / {string.Format(ValueFormat, _maximum)}{UnitSuffix}";

            using var brush = new SolidBrush(ForeColor);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(text, Font, brush, rect, sf);
        }

        private static double Clamp(double v, double min, double max)
            => v < min ? min : (v > max ? max : v);

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(r);
                return path;
            }

            int d = radius * 2;
            var arc = new Rectangle(r.Left, r.Top, d, d);

            // top-left
            path.AddArc(arc, 180, 90);

            // top-right
            arc.X = r.Right - d;
            path.AddArc(arc, 270, 90);

            // bottom-right
            arc.Y = r.Bottom - d;
            path.AddArc(arc, 0, 90);

            // bottom-left
            arc.X = r.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
