namespace Demo
{
    partial class FormDemo
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            adfIndicatorControl1 = new se.swesim.flight.formcontrols.Instruments.AdfIndicatorControl();
            airSpeedIndicatorControl1 = new se.swesim.flight.formcontrols.Instruments.AirSpeedIndicatorControl();
            axisCalibrationControl1 = new se.swesim.flight.formcontrols.Input.AxisCalibrationControl();
            pressureAltimeterControl1 = new se.swesim.flight.formcontrols.Instruments.PressureAltimeterControl();
            vsiControl1 = new se.swesim.flight.formcontrols.Instruments.VSIControl();
            timerDemo = new System.Windows.Forms.Timer(components);
            headingIndicatorControl1 = new se.swesim.flight.formcontrols.Instruments.HeadingIndicatorControl();
            attitudeIndicatorControl1 = new se.swesim.flight.formcontrols.Instruments.AttitudeIndicatorControl();
            valueProgressBar1 = new se.swesim.flight.formcontrols.Controls.ValueProgressBar();
            gi106bCdiControl1 = new se.swesim.flight.formcontrols.Instruments.Gi106BCdiControl();
            ki208CdiControl1 = new se.swesim.flight.formcontrols.Instruments.Ki208CdiControl();
            tachometerControl1 = new se.swesim.flight.formcontrols.Instruments.TachometerControl();
            SuspendLayout();
            // 
            // adfIndicatorControl1
            // 
            adfIndicatorControl1.BearingDegrees = 0F;
            adfIndicatorControl1.Location = new Point(12, 249);
            adfIndicatorControl1.Name = "adfIndicatorControl1";
            adfIndicatorControl1.Size = new Size(220, 232);
            adfIndicatorControl1.TabIndex = 0;
            adfIndicatorControl1.Text = "adfIndicatorControl1";
            // 
            // airSpeedIndicatorControl1
            // 
            airSpeedIndicatorControl1.IndicatedSpeed = 0D;
            airSpeedIndicatorControl1.LabelStep = 20;
            airSpeedIndicatorControl1.Location = new Point(12, 12);
            airSpeedIndicatorControl1.Name = "airSpeedIndicatorControl1";
            airSpeedIndicatorControl1.ShowUnitText = true;
            airSpeedIndicatorControl1.Size = new Size(220, 220);
            airSpeedIndicatorControl1.StartSpeed = 40D;
            airSpeedIndicatorControl1.TabIndex = 1;
            airSpeedIndicatorControl1.Text = "airSpeedIndicatorControl1";
            airSpeedIndicatorControl1.TickStep = 5;
            airSpeedIndicatorControl1.Unit = se.swesim.flight.formcontrols.Instruments.AirspeedUnit.Knots;
            airSpeedIndicatorControl1.VFE = 102D;
            airSpeedIndicatorControl1.VNE = 160D;
            airSpeedIndicatorControl1.VNO = 130D;
            airSpeedIndicatorControl1.VS1 = 50D;
            airSpeedIndicatorControl1.VSO = 48D;
            // 
            // axisCalibrationControl1
            // 
            axisCalibrationControl1.AxisName = "Axis";
            axisCalibrationControl1.CurrentValue = -100D;
            axisCalibrationControl1.Location = new Point(12, 487);
            axisCalibrationControl1.Name = "axisCalibrationControl1";
            axisCalibrationControl1.Size = new Size(335, 80);
            axisCalibrationControl1.TabIndex = 2;
            // 
            // pressureAltimeterControl1
            // 
            pressureAltimeterControl1.AltitudeFeet = 0D;
            pressureAltimeterControl1.Location = new Point(482, 12);
            pressureAltimeterControl1.Name = "pressureAltimeterControl1";
            pressureAltimeterControl1.Size = new Size(227, 220);
            pressureAltimeterControl1.TabIndex = 3;
            pressureAltimeterControl1.Text = "pressureAltimeterControl1";
            // 
            // vsiControl1
            // 
            vsiControl1.Location = new Point(482, 249);
            vsiControl1.Name = "vsiControl1";
            vsiControl1.Size = new Size(227, 232);
            vsiControl1.TabIndex = 4;
            vsiControl1.Text = "vsiControl1";
            vsiControl1.VerticalSpeedFpm = 0D;
            // 
            // timerDemo
            // 
            timerDemo.Enabled = true;
            timerDemo.Tick += timerDemo_Tick;
            // 
            // headingIndicatorControl1
            // 
            headingIndicatorControl1.BugHeadingDegrees = 0F;
            headingIndicatorControl1.HeadingBugColor = Color.Orange;
            headingIndicatorControl1.HeadingDegrees = 0F;
            headingIndicatorControl1.Location = new Point(238, 249);
            headingIndicatorControl1.Name = "headingIndicatorControl1";
            headingIndicatorControl1.ShowDigitalReadout = true;
            headingIndicatorControl1.ShowHeadingBug = true;
            headingIndicatorControl1.Size = new Size(238, 232);
            headingIndicatorControl1.TabIndex = 5;
            headingIndicatorControl1.Text = "headingIndicatorControl1";
            // 
            // attitudeIndicatorControl1
            // 
            attitudeIndicatorControl1.GroundColor = Color.FromArgb(140, 90, 50);
            attitudeIndicatorControl1.Location = new Point(236, 12);
            attitudeIndicatorControl1.Name = "attitudeIndicatorControl1";
            attitudeIndicatorControl1.PitchClampDegrees = 30F;
            attitudeIndicatorControl1.PitchDegrees = 0F;
            attitudeIndicatorControl1.PixelsPerPitchDegree = 2.2F;
            attitudeIndicatorControl1.RollDegrees = 0F;
            attitudeIndicatorControl1.ShowPitchLadder = true;
            attitudeIndicatorControl1.Size = new Size(240, 220);
            attitudeIndicatorControl1.SkyColor = Color.FromArgb(60, 120, 200);
            attitudeIndicatorControl1.TabIndex = 6;
            attitudeIndicatorControl1.Text = "attitudeIndicatorControl1";
            // 
            // valueProgressBar1
            // 
            valueProgressBar1.BarColor = Color.FromArgb(60, 120, 255);
            valueProgressBar1.BorderColor = Color.Gray;
            valueProgressBar1.CornerRadius = 6;
            valueProgressBar1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            valueProgressBar1.ForeColor = Color.White;
            valueProgressBar1.Location = new Point(12, 612);
            valueProgressBar1.Maximum = 100D;
            valueProgressBar1.Minimum = 0D;
            valueProgressBar1.Name = "valueProgressBar1";
            valueProgressBar1.NegativeBarColor = Color.FromArgb(255, 120, 60);
            valueProgressBar1.ShowValueOnly = true;
            valueProgressBar1.ShowZeroLine = true;
            valueProgressBar1.Size = new Size(335, 24);
            valueProgressBar1.TabIndex = 7;
            valueProgressBar1.Text = "valueProgressBar1";
            valueProgressBar1.TrackColor = Color.FromArgb(30, 30, 30);
            valueProgressBar1.UnitSuffix = "%";
            valueProgressBar1.Value = 0D;
            valueProgressBar1.ValueFormat = "{0:0}";
            valueProgressBar1.ZeroLineColor = Color.FromArgb(180, 180, 180);
            // 
            // gi106bCdiControl1
            // 
            gi106bCdiControl1.AnnunciatorColor = Color.LimeGreen;
            gi106bCdiControl1.BezelColor = Color.FromArgb(40, 40, 40);
            gi106bCdiControl1.CdiVisible = true;
            gi106bCdiControl1.CourseDeviationDots = 0F;
            gi106bCdiControl1.FaceColor = Color.Black;
            gi106bCdiControl1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            gi106bCdiControl1.ForeColor = Color.White;
            gi106bCdiControl1.GlideSlopeDots = 0F;
            gi106bCdiControl1.GsVisible = true;
            gi106bCdiControl1.Location = new Point(715, 12);
            gi106bCdiControl1.Name = "gi106bCdiControl1";
            gi106bCdiControl1.NeedleColor = Color.White;
            gi106bCdiControl1.ObsDegrees = 0F;
            gi106bCdiControl1.ShowGlideSlope = true;
            gi106bCdiControl1.Size = new Size(220, 220);
            gi106bCdiControl1.Source = se.swesim.flight.formcontrols.Instruments.Gi106BSource.GPS;
            gi106bCdiControl1.TabIndex = 8;
            gi106bCdiControl1.Text = "gi106bCdiControl1";
            gi106bCdiControl1.TickColor = Color.White;
            gi106bCdiControl1.ToFrom = se.swesim.flight.formcontrols.Instruments.ToFromState.Off;
            // 
            // ki208CdiControl1
            // 
            ki208CdiControl1.BezelColor = Color.FromArgb(20, 20, 20);
            ki208CdiControl1.CdiVisible = true;
            ki208CdiControl1.CourseDeviationDots = 0F;
            ki208CdiControl1.FaceColor = Color.Black;
            ki208CdiControl1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            ki208CdiControl1.ForeColor = Color.White;
            ki208CdiControl1.Location = new Point(715, 249);
            ki208CdiControl1.Name = "ki208CdiControl1";
            ki208CdiControl1.NeedleColor = Color.White;
            ki208CdiControl1.ObsDegrees = 0F;
            ki208CdiControl1.PointerColor = Color.FromArgb(230, 210, 40);
            ki208CdiControl1.ShowBrandText = true;
            ki208CdiControl1.ShowObsKnobHint = true;
            ki208CdiControl1.Size = new Size(220, 232);
            ki208CdiControl1.TabIndex = 9;
            ki208CdiControl1.Text = "ki208CdiControl1";
            ki208CdiControl1.TickColor = Color.White;
            ki208CdiControl1.ToFrom = se.swesim.flight.formcontrols.Instruments.Ki208ToFromState.Off;
            // 
            // tachometerControl1
            // 
            tachometerControl1.BezelColor = Color.FromArgb(35, 35, 35);
            tachometerControl1.EndRpm = 3500F;
            tachometerControl1.FaceColor = Color.Black;
            tachometerControl1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            tachometerControl1.ForeColor = Color.White;
            tachometerControl1.GreenArcColor = Color.ForestGreen;
            tachometerControl1.GreenArcEndRpm = 2600F;
            tachometerControl1.GreenArcStartRpm = 500F;
            tachometerControl1.LabelStepRpm = 500F;
            tachometerControl1.Location = new Point(952, 249);
            tachometerControl1.MaxAllowableRpm = 2700F;
            tachometerControl1.Name = "tachometerControl1";
            tachometerControl1.NeedleColor = Color.White;
            tachometerControl1.RedArcColor = Color.Red;
            tachometerControl1.Rpm = 0F;
            tachometerControl1.ScaleEndAngle = 135F;
            tachometerControl1.ScaleStartAngle = 225F;
            tachometerControl1.ShowRpmText = true;
            tachometerControl1.Size = new Size(240, 232);
            tachometerControl1.StartRpm = 0F;
            tachometerControl1.TabIndex = 10;
            tachometerControl1.Text = "tachometerControl1";
            tachometerControl1.TickColor = Color.White;
            tachometerControl1.TickStepRpm = 100F;
            // 
            // FormDemo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1223, 664);
            Controls.Add(tachometerControl1);
            Controls.Add(ki208CdiControl1);
            Controls.Add(gi106bCdiControl1);
            Controls.Add(valueProgressBar1);
            Controls.Add(attitudeIndicatorControl1);
            Controls.Add(headingIndicatorControl1);
            Controls.Add(vsiControl1);
            Controls.Add(pressureAltimeterControl1);
            Controls.Add(axisCalibrationControl1);
            Controls.Add(airSpeedIndicatorControl1);
            Controls.Add(adfIndicatorControl1);
            Name = "FormDemo";
            Text = "SwesimFlightFormControls Demo";
            KeyDown += FormDemo_KeyDown;
            ResumeLayout(false);
        }

        #endregion

        private se.swesim.flight.formcontrols.Instruments.AdfIndicatorControl adfIndicatorControl1;
        private se.swesim.flight.formcontrols.Instruments.AirSpeedIndicatorControl airSpeedIndicatorControl1;
        private se.swesim.flight.formcontrols.Input.AxisCalibrationControl axisCalibrationControl1;
        private se.swesim.flight.formcontrols.Instruments.PressureAltimeterControl pressureAltimeterControl1;
        private se.swesim.flight.formcontrols.Instruments.VSIControl vsiControl1;
        private System.Windows.Forms.Timer timerDemo;
        private se.swesim.flight.formcontrols.Instruments.HeadingIndicatorControl headingIndicatorControl1;
        private se.swesim.flight.formcontrols.Instruments.AttitudeIndicatorControl attitudeIndicatorControl1;
        private se.swesim.flight.formcontrols.Controls.ValueProgressBar valueProgressBar1;
        private se.swesim.flight.formcontrols.Instruments.Gi106BCdiControl gi106bCdiControl1;
        private se.swesim.flight.formcontrols.Instruments.Ki208CdiControl ki208CdiControl1;
        private se.swesim.flight.formcontrols.Instruments.TachometerControl tachometerControl1;
    }
}
