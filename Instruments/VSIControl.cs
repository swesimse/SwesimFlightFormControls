using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SwesimPiper
{
	public class VSIControl : Control
	{
		private double _verticalSpeedFpm; // feet per minute

		[Category("VSI")]
		[Description("Vertical speed in feet per minute (-2000 to +2000).")]
		public double VerticalSpeedFpm
		{
			get => _verticalSpeedFpm;
			set
			{
				if (Math.Abs(_verticalSpeedFpm - value) > double.Epsilon)
				{
					_verticalSpeedFpm = value;
					Invalidate();
				}
			}
		}

		public VSIControl()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.UserPaint |
					 ControlStyles.ResizeRedraw, true);

			DoubleBuffered = true;
			Size = new Size(200, 200);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			int size = Math.Min(ClientSize.Width, ClientSize.Height);
			float radius = size / 2f;
			PointF center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

			// Background
			using (var backBrush = new SolidBrush(Color.Black))
			{
				g.FillEllipse(backBrush, center.X - radius, center.Y - radius, size, size);
			}

			// Bezel
			using (var bezelPen = new Pen(Color.Gray, 4))
			{
				g.DrawEllipse(bezelPen, center.X - radius + 2, center.Y - radius + 2, size - 4, size - 4);
			}

			DrawScale(g, center, radius);
			DrawNeedle(g, center, radius);

			// Center hub
			using (var hubBrush = new SolidBrush(Color.Gray))
			using (var hubPen = new Pen(Color.White, 1))
			{
				float hubRadius = radius * 0.05f;
				var rect = new RectangleF(center.X - hubRadius, center.Y - hubRadius,
										  hubRadius * 2, hubRadius * 2);
				g.FillEllipse(hubBrush, rect);
				g.DrawEllipse(hubPen, rect);
			}
		}

		private void DrawScale(Graphics g, PointF center, float radius)
		{
			using var majorPen = new Pen(Color.White, 2);
			using var minorPen = new Pen(Color.White, 1);
			using var textBrush = new SolidBrush(Color.White);

			var sfCenter = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			// Tick marks: –2000 to +2000 in 100 ft/min steps
			// Major tick every 500 ft/min, minor in between.
			for (int vs = -2000; vs <= 2000; vs += 100)
			{
				float angleDeg = ValueToAngle(vs);
				double angleRad = Math.PI / 180.0 * angleDeg;

				bool isMajor = (vs % 500 == 0);

				float outer = radius * 0.9f;
				float inner = isMajor ? radius * 0.78f : radius * 0.84f;

				float xOuter = center.X + outer * (float)Math.Sin(angleRad);
				float yOuter = center.Y - outer * (float)Math.Cos(angleRad);
				float xInner = center.X + inner * (float)Math.Sin(angleRad);
				float yInner = center.Y - inner * (float)Math.Cos(angleRad);

				g.DrawLine(isMajor ? majorPen : minorPen, xInner, yInner, xOuter, yOuter);
			}

			// Number labels at every 500 ft/min (5, 10, 15, 20)
			using var labelFont = new Font(FontFamily.GenericSansSerif,
										   radius * 0.13f,
										   FontStyle.Bold,
										   GraphicsUnit.Pixel);

			for (int vs = -2000; vs <= 2000; vs += 500)
			{
				if (vs == 0) continue; // 0 is labelled with UP/DN text region

				float angleDeg = ValueToAngle(vs);
				double angleRad = Math.PI / 180.0 * angleDeg;

				float labelRadius = radius * 0.62f;

				float xLabel = center.X + labelRadius * (float)Math.Sin(angleRad);
				float yLabel = center.Y - labelRadius * (float)Math.Cos(angleRad);

				int magnitude = Math.Abs(vs / 100); // 500 -> 5, 2000 -> 20
				string label = magnitude.ToString();

				g.DrawString(label, labelFont, textBrush, xLabel, yLabel, sfCenter);
			}

			// "UP" and "DN" near 0
			using var smallFont = new Font(FontFamily.GenericSansSerif,
										   radius * 0.11f,
										   FontStyle.Regular,
										   GraphicsUnit.Pixel);

			// Slightly above and below the 0 tick (which is at 9 o'clock)
			float zeroAngle = ValueToAngle(0); // 9 o'clock
			double zeroRad = Math.PI / 180.0 * zeroAngle;
			float textRadius = radius * 0.55f;

			// UP (above 0)
			float xUp = center.X + textRadius * (float)Math.Sin(zeroRad);
			float yUp = center.Y - textRadius * (float)Math.Cos(zeroRad) - radius * 0.12f;
			g.DrawString("UP", smallFont, textBrush, xUp, yUp, sfCenter);

			// DN (below 0)
			float xDn = xUp;
			float yDn = center.Y - textRadius * (float)Math.Cos(zeroRad) + radius * 0.12f;
			g.DrawString("DN", smallFont, textBrush, xDn, yDn, sfCenter);

			// Center text: "VERTICAL SPEED" and "100 FEET PER MINUTE"
			using var centerFontBig = new Font(FontFamily.GenericSansSerif,
											   radius * 0.11f,
											   FontStyle.Bold,
											   GraphicsUnit.Pixel);
			using var centerFontSmall = new Font(FontFamily.GenericSansSerif,
												 radius * 0.09f,
												 FontStyle.Regular,
												 GraphicsUnit.Pixel);

			g.DrawString("VERTICAL", centerFontBig, textBrush,
						 center.X, center.Y - radius * 0.08f, sfCenter);
			g.DrawString("SPEED", centerFontBig, textBrush,
						 center.X, center.Y + radius * 0.02f, sfCenter);
			g.DrawString("100 FEET", centerFontSmall, textBrush,
						 center.X, center.Y + radius * 0.13f, sfCenter);
			g.DrawString("PER MINUTE", centerFontSmall, textBrush,
						 center.X, center.Y + radius * 0.22f, sfCenter);
		}

		private void DrawNeedle(Graphics g, PointF center, float radius)
		{
			double vs = VerticalSpeedFpm;

			// Clamp to –2000..+2000
			if (vs > 2000) vs = 2000;
			if (vs < -2000) vs = -2000;

			float angleDeg = ValueToAngle(vs);

			var oldTransform = g.Transform;

			g.TranslateTransform(center.X, center.Y);
			g.RotateTransform(angleDeg);

			float length = radius * 0.82f;
			float tail = radius * 0.15f;

			using (var pen = new Pen(Color.White, radius * 0.03f)
			{
				StartCap = LineCap.Round,
				EndCap = LineCap.Triangle
			})
			{
				// Draw a line from slightly behind center to the scale
				g.DrawLine(pen, 0, tail, 0, -length);
			}

			g.Transform = oldTransform;
		}

		/// <summary>
		/// Map vertical speed (ft/min) to a pointer angle in degrees.
		///
		/// -2000 -> 135°  (~4:30 o'clock, full down)
		/// 0     -> 270°  (9 o'clock, zero)
		/// +2000 -> 405°  (= 45°, ~1:30 o'clock, full up)
		///
		/// The 135°..405° span gives a nice, wide arc on the right side of the gauge,
		/// similar to a real VSI.
		/// </summary>
		private float ValueToAngle(double verticalSpeedFpm)
		{
			const double minVs = -2000.0;
			const double maxVs = 2000.0;

			// Clamp
			double v = verticalSpeedFpm;
			if (v < minVs) v = minVs;
			if (v > maxVs) v = maxVs;

			// These define where the scale starts/ends on the dial
			const double minAngleDeg = 115.0; // at -2000 ft/min
			const double maxAngleDeg = 425.0; // at +2000 ft/min (405 == 45 degrees)

			double t = (v - minVs) / (maxVs - minVs); // 0..1
			double angle = minAngleDeg + t * (maxAngleDeg - minAngleDeg);

			return (float)angle;
		}
	}
}
