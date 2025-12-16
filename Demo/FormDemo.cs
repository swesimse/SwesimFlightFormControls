namespace Demo
{
    public partial class FormDemo : Form
    {
        public FormDemo()
        {
            InitializeComponent();
        }

        private void timerDemo_Tick(object sender, EventArgs e)
        {
            axisCalibrationControl1.CurrentValue += 1;
            if(axisCalibrationControl1.CurrentValue > 100)
                axisCalibrationControl1.CurrentValue = -100;

            adfIndicatorControl1.BearingDegrees += 1;
            if(adfIndicatorControl1.BearingDegrees > 360)
                adfIndicatorControl1.BearingDegrees = 0;

            airSpeedIndicatorControl1.IndicatedSpeed += 1;
            if(airSpeedIndicatorControl1.IndicatedSpeed > airSpeedIndicatorControl1.VNE)
                airSpeedIndicatorControl1.IndicatedSpeed = 0;

            pressureAltimeterControl1.AltitudeFeet += 10;
            if(pressureAltimeterControl1.AltitudeFeet > 12000)
                pressureAltimeterControl1.AltitudeFeet = 0;

            vsiControl1.VerticalSpeedFpm += 100;
            if(vsiControl1.VerticalSpeedFpm > 2000)
                vsiControl1.VerticalSpeedFpm = -2000;

            headingIndicatorControl1.HeadingDegrees += 2;
            if(headingIndicatorControl1.HeadingDegrees >= 360)
                headingIndicatorControl1.HeadingDegrees = 0;
            headingIndicatorControl1.BugHeadingDegrees -= 1;
            if(headingIndicatorControl1.BugHeadingDegrees <= 0)
                headingIndicatorControl1.BugHeadingDegrees = 360;
        }
    }
}
