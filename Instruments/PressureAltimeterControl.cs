using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SwesimPiper
{
	public class PressureAltimeterControl : Control
	{
		private double _altitudeFeet;

		[Category("Altimeter")]
		[Description("Indicated altitude in feet.")]
		public double AltitudeFeet
		{
			get => _altitudeFeet;
			set
			{
				if (Math.Abs(_altitudeFeet - value) > double.Epsilon)
				{
					_altitudeFeet = value;
					Invalidate(); // redraw when altitude changes
				}
			}
		}

		public PressureAltimeterControl()
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
			DrawNeedles(g, center, radius);

			// Center hub
			using (var hubBrush = new SolidBrush(Color.Gray))
			using (var hubPen = new Pen(Color.White, 1))
			{
				float hubRadius = radius * 0.06f;
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

			using var font = new Font(FontFamily.GenericSansSerif,
									  radius * 0.25f,
									  FontStyle.Bold,
									  GraphicsUnit.Pixel);

			var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			//
			// Thousands labels (0–9), one every 1000 ft
			// These stay unchanged.
			//
			for (int i = 0; i < 10; i++)
			{
				float angleDeg = i * 36f; // 360 / 10
				double angleRad = Math.PI / 180.0 * angleDeg;

				float outer = radius * 0.9f;
				float inner = radius * 0.8f;

				float xOuter = center.X + outer * (float)Math.Sin(angleRad);
				float yOuter = center.Y - outer * (float)Math.Cos(angleRad);
				float xInner = center.X + inner * (float)Math.Sin(angleRad);
				float yInner = center.Y - inner * (float)Math.Cos(angleRad);

				g.DrawLine(majorPen, xInner, yInner, xOuter, yOuter);

				// Draw digit label
				string label = i.ToString();
				float labelRadius = radius * 0.65f;
				float xLabel = center.X + labelRadius * (float)Math.Sin(angleRad);
				float yLabel = center.Y - labelRadius * (float)Math.Cos(angleRad);

				g.DrawString(label, font, textBrush, xLabel, yLabel, sf);
			}

			//
			// New: 20-ft ticks
			// 0–49 ticks (50 positions), 360° / 50 = 7.2°
			//
			for (int tick = 0; tick < 50; tick++)
			{
				float angleDeg = tick * 7.2f; // each tick = 20 ft
				double angleRad = Math.PI / 180.0 * angleDeg;

				bool isMajor = (tick % 5 == 0); // every 5 ticks = 100 ft

				float outer = radius * 0.9f;
				float inner = isMajor ? radius * 0.8f : radius * 0.85f;

				float xOuter = center.X + outer * (float)Math.Sin(angleRad);
				float yOuter = center.Y - outer * (float)Math.Cos(angleRad);
				float xInner = center.X + inner * (float)Math.Sin(angleRad);
				float yInner = center.Y - inner * (float)Math.Cos(angleRad);

				g.DrawLine(isMajor ? majorPen : minorPen, xInner, yInner, xOuter, yOuter);
			}

			//
			// Optional “ALT” marking
			//
			using var altFont = new Font(FontFamily.GenericSansSerif,
										 radius * 0.12f,
										 FontStyle.Regular,
										 GraphicsUnit.Pixel);

			g.DrawString("ALT", altFont, textBrush,
				center.X,
				center.Y - radius * 0.35f,
				sf);
		}

		private void DrawNeedles(Graphics g, PointF center, float radius)
		{
			double alt = AltitudeFeet;
			if (alt < 0) alt = 0;

			//
			// Angle math stays the same as before
			//
			double hundredsPart = alt % 1000.0;
			float hundredsAngle = (float)(hundredsPart / 1000.0 * 360.0);

			double thousandsPart = alt % 10000.0;
			float thousandsAngle = (float)(thousandsPart / 10000.0 * 360.0);

			Matrix old = g.Transform;

			//
			// --- Thousands needle (short, thick) ---
			//
			g.TranslateTransform(center.X, center.Y);
			g.RotateTransform(thousandsAngle);

			float thousandsLength = radius * 0.50f;

			using (var thousandsPen = new Pen(Color.White, radius * 0.07f)  // << thicker
			{
				StartCap = LineCap.Round,
				EndCap = LineCap.Triangle   // nice pointed end
			})
			{
				g.DrawLine(thousandsPen, 0, 0, 0, -thousandsLength);
			}

			g.Transform = old;

			//
			// --- Hundreds needle (long, slimmer) ---
			//
			g.TranslateTransform(center.X, center.Y);
			g.RotateTransform(hundredsAngle);

			float hundredsLength = radius * 0.80f;

			using (var hundredsPen = new Pen(Color.White, radius * 0.03f)  // << thinner
			{
				StartCap = LineCap.Round,
				EndCap = LineCap.ArrowAnchor
			})
			{
				g.DrawLine(hundredsPen, 0, 0, 0, -hundredsLength);
			}

			g.Transform = old;
		}
	}
}
