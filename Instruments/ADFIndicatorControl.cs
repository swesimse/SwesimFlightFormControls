using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Instruments
{
    /// <summary>
	/// ADF Indicator control.
	/// </summary>
    public class AdfIndicatorControl : Control
	{
		private float _bearingDegrees;

		[Category("ADF")]
		[Description("Bearing to the NDB, in degrees (0-360). 0 = North, 90 = East, etc.")]
		public float BearingDegrees
		{
			get => _bearingDegrees;
			set
			{
				// Normalize to 0–360
				float normalized = value % 360f;
				if (normalized < 0) normalized += 360f;

				if (Math.Abs(_bearingDegrees - normalized) > float.Epsilon)
				{
					_bearingDegrees = normalized;
					Invalidate(); // trigger repaint
				}
			}
		}

		public AdfIndicatorControl()
		{
			// Good defaults for smooth instrument drawing
			SetStyle(ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.UserPaint |
					 ControlStyles.ResizeRedraw, true);

			DoubleBuffered = true;
			Size = new Size(200, 200); // default size
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			int size = Math.Min(ClientSize.Width, ClientSize.Height);
			float radius = size / 2f;
			PointF center = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);

			// Draw background (outer circle)
			using (var backBrush = new SolidBrush(Color.Black))
			{
				g.FillEllipse(backBrush, center.X - radius, center.Y - radius, size, size);
			}

			// Draw bezel
			using (var bezelPen = new Pen(Color.Gray, 4))
			{
				g.DrawEllipse(bezelPen, center.X - radius + 2, center.Y - radius + 2, size - 4, size - 4);
			}

			// Draw tick marks and labels
			DrawScale(g, center, radius);

			// Draw arrow
			DrawArrow(g, center, radius, BearingDegrees);

			// Optional: draw center hub
			using (var hubBrush = new SolidBrush(Color.Gray))
			{
				float hubRadius = radius * 0.06f;
				g.FillEllipse(hubBrush,
					center.X - hubRadius,
					center.Y - hubRadius,
					hubRadius * 2,
					hubRadius * 2);
			}
		}

		private void DrawScale(Graphics g, PointF center, float radius)
		{
			using var majorPen = new Pen(Color.White, 2);
			using var minorPen = new Pen(Color.White, 1);
			using var textBrush = new SolidBrush(Color.White);

			using var font = new Font(FontFamily.GenericSansSerif, radius * 0.15f, FontStyle.Bold, GraphicsUnit.Pixel);
			StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

			// 0 to 330 every 30 degrees
			for (int angle = 0; angle < 360; angle += 5)
			{
				double radians = (Math.PI / 180.0) * angle;

				float outer = radius * 0.9f;
				float inner = (angle % 30 == 0) ? radius * 0.78f : radius * 0.83f;

				float xOuter = center.X + outer * (float)Math.Sin(radians);
				float yOuter = center.Y - outer * (float)Math.Cos(radians);
				float xInner = center.X + inner * (float)Math.Sin(radians);
				float yInner = center.Y - inner * (float)Math.Cos(radians);

				g.DrawLine(angle % 30 == 0 ? majorPen : minorPen, xInner, yInner, xOuter, yOuter);

				// Labels for each 30°
				if (angle % 30 == 0)
				{
					string label;
					switch (angle)
					{
						case 0: label = "N"; break;
						case 90: label = "E"; break;
						case 180: label = "S"; break;
						case 270: label = "W"; break;
						default: label = angle.ToString(); break;
					}

					float labelRadius = radius * 0.65f;
					float xLabel = center.X + labelRadius * (float)Math.Sin(radians);
					float yLabel = center.Y - labelRadius * (float)Math.Cos(radians);

					g.DrawString(label, font, textBrush, xLabel, yLabel, sf);
				}
			}
		}

		private void DrawArrow(Graphics g, PointF center, float radius, float bearingDegrees)
		{
			// Convert bearing to radians. 0° = up (north), positive clockwise.
			float angleRad = (float)(Math.PI / 180.0 * bearingDegrees);

			// Save transform
			var oldTransform = g.Transform;

			// Move origin to center and rotate
			g.TranslateTransform(center.X, center.Y);
			g.RotateTransform(bearingDegrees);

			// Arrow geometry relative to origin, pointing "up"
			float arrowLength = radius * 0.75f;
			float arrowWidth = radius * 0.12f;
			float tailWidth = radius * 0.06f;

			// Arrow polygon (simple ADF pointer-style shape)
			PointF[] points =
			{
				new PointF(0, -arrowLength),               // tip
                new PointF(arrowWidth / 2, 0),             // right middle
                new PointF(tailWidth / 2, arrowLength*0.25f), // right tail
                new PointF(-tailWidth / 2, arrowLength*0.25f), // left tail
                new PointF(-arrowWidth / 2, 0),            // left middle
            };

			using (var arrowBrush = new SolidBrush(Color.Yellow))
			using (var arrowPen = new Pen(Color.Black, 1))
			{
				g.FillPolygon(arrowBrush, points);
				g.DrawPolygon(arrowPen, points);
			}

			// Restore transform
			g.Transform = oldTransform;
		}
	}
}
