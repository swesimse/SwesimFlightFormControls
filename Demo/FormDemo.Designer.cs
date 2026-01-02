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
            axisCalibrationControl1.Location = new Point(825, 12);
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
            valueProgressBar1.Location = new Point(825, 137);
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
            // FormDemo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1223, 506);
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
    }
}
