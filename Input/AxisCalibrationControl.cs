using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Input
{
    /// <summary>
    /// Axis calibration control.
    /// </summary>
    public class AxisCalibrationControl : UserControl
    {
        private double _currentValue;

        // Observed range (auto-learns)
        public double ObservedMin { get; private set; } = double.NaN;
        public double ObservedMax { get; private set; } = double.NaN;

        // Calibration points
        public double? CalMin { get; private set; }
        public double? CalCenter { get; private set; }
        public double? CalMax { get; private set; }

        // UI
        private readonly Button _btnMin = new() { Text = "Set Min" };
        private readonly Button _btnCenter = new() { Text = "Set Center" };
        private readonly Button _btnMax = new() { Text = "Set Max" };
        private readonly Button _btnReset = new() { Text = "Reset Observed" };

        private readonly Label _lbl = new()
        {
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft
        };

        [Category("Axis")]
        public string AxisName
        {
            get => _lbl.Text;
            set { _lbl.Text = value; Invalidate(); }
        }

        [Category("Axis")]
        [Description("Current raw input value from hardware.")]
        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                UpdateObservedRange(value);
                Invalidate();
            }
        }

        public AxisCalibrationControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Height = 80;

            _lbl.Text = "Axis";
            _lbl.Dock = DockStyle.Top;
            _lbl.Height = 18;

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            btnPanel.Controls.AddRange(new Control[] { _btnMin, _btnCenter, _btnMax, _btnReset });

            Controls.Add(_lbl);
            Controls.Add(btnPanel);

            _btnMin.Click += (_, __) => { CalMin = CurrentValue; Invalidate(); CalibrationChanged?.Invoke(this, EventArgs.Empty); };
            _btnCenter.Click += (_, __) => { CalCenter = CurrentValue; Invalidate(); CalibrationChanged?.Invoke(this, EventArgs.Empty); };
            _btnMax.Click += (_, __) => { CalMax = CurrentValue; Invalidate(); CalibrationChanged?.Invoke(this, EventArgs.Empty); };
            _btnReset.Click += (_, __) => { ResetObserved(); Invalidate(); };
        }

        public event EventHandler? CalibrationChanged;

        public void ResetObserved()
        {
            ObservedMin = double.NaN;
            ObservedMax = double.NaN;
        }

        private void UpdateObservedRange(double v)
        {
            if (double.IsNaN(ObservedMin) || v < ObservedMin) ObservedMin = v;
            if (double.IsNaN(ObservedMax) || v > ObservedMax) ObservedMax = v;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Track area (between label and buttons)
            int top = _lbl.Bottom + 8;
            int bottom = Height - 40;
            var trackRect = new Rectangle(10, top, Width - 20, Math.Max(16, bottom - top));

            // Background
            using var bg = new SolidBrush(Color.FromArgb(25, 25, 25));
            using var border = new Pen(Color.Gray, 1);
            g.FillRectangle(bg, trackRect);
            g.DrawRectangle(border, trackRect);

            // If we don't have a usable range yet, just draw a centered marker
            bool hasRange = !double.IsNaN(ObservedMin) && !double.IsNaN(ObservedMax) && ObservedMax > ObservedMin;

            // Map value -> x
            float XOf(double value)
            {
                if (!hasRange) return trackRect.Left + trackRect.Width / 2f;
                double t = (value - ObservedMin) / (ObservedMax - ObservedMin);
                t = Math.Max(0, Math.Min(1, t));
                return (float)(trackRect.Left + t * trackRect.Width);
            }

            // Draw live value fill (like progress bar from left to current)
            float xCur = XOf(CurrentValue);
            var fillRect = RectangleF.FromLTRB(trackRect.Left, trackRect.Top, xCur, trackRect.Bottom);

            using (var fill = new SolidBrush(Color.FromArgb(60, 120, 255)))
            {
                g.FillRectangle(fill, fillRect);
            }

            // Draw markers: min/center/max calibration
            DrawMarker(g, trackRect, XOf, CalMin, Color.Orange, "MIN");
            DrawMarker(g, trackRect, XOf, CalCenter, Color.Yellow, "C");
            DrawMarker(g, trackRect, XOf, CalMax, Color.Orange, "MAX");

            // Draw current indicator line
            using (var curPen = new Pen(Color.White, 2))
            {
                g.DrawLine(curPen, xCur, trackRect.Top - 2, xCur, trackRect.Bottom + 2);
            }

            // Draw readout text
            string rangeText = hasRange
                ? $"Raw: {CurrentValue:0.##}   ObsMin: {ObservedMin:0.##}   ObsMax: {ObservedMax:0.##}"
                : $"Raw: {CurrentValue:0.##}   (move axis to discover range)";

            using var font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular);
            using var textBrush = new SolidBrush(Color.White);
            g.DrawString(rangeText, font, textBrush, 10, trackRect.Bottom + 4);
        }

        private static void DrawMarker(Graphics g, Rectangle trackRect, Func<double, float> xOf, double? calValue, Color color, string label)
        {
            if (!calValue.HasValue) return;

            float x = xOf(calValue.Value);

            using var pen = new Pen(color, 2);
            g.DrawLine(pen, x, trackRect.Top, x, trackRect.Bottom);

            using var font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
            using var brush = new SolidBrush(color);

            // Label above the track
            g.DrawString(label, font, brush, x - 10, trackRect.Top - 14);
        }

        /// <summary>
        /// Convert current raw value into normalized axis output using calibration.
        /// Returns -1..+1 if min/center/max are set. Otherwise returns null.
        /// </summary>
        public double? GetNormalizedMinus1ToPlus1(double raw)
        {
            if (!CalMin.HasValue || !CalCenter.HasValue || !CalMax.HasValue) return null;

            double min = CalMin.Value;
            double ctr = CalCenter.Value;
            double max = CalMax.Value;

            // Handle reversed axes (some pots run "backwards")
            bool reversed = max < min;
            if (reversed)
            {
                (min, max) = (max, min);
                // center stays as-is (it will be between them if calibration is sane)
            }

            // Protect against nonsense
            if (max - min < 1e-9) return null;

            // Piecewise mapping around center (gives symmetric feel)
            if (raw >= ctr)
            {
                double denom = (max - ctr);
                if (denom < 1e-9) return null;
                return Math.Min(1.0, (raw - ctr) / denom);
            }
            else
            {
                double denom = (ctr - min);
                if (denom < 1e-9) return null;
                return Math.Max(-1.0, -(ctr - raw) / denom);
            }
        }
    }
}
