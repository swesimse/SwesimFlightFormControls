using se.swesim.flight.formcontrols.Instruments;

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

            attitudeIndicatorControl1.PitchDegrees += 1;
            if(attitudeIndicatorControl1.PitchDegrees > 90)
                attitudeIndicatorControl1.PitchDegrees = -90;
            attitudeIndicatorControl1.RollDegrees += 2;
            if(attitudeIndicatorControl1.RollDegrees > 180)
                attitudeIndicatorControl1.RollDegrees = -180;


            gi106bCdiControl1.ObsDegrees += 1;
            if(gi106bCdiControl1.ObsDegrees >= 360)
                gi106bCdiControl1.ObsDegrees = 0;
            gi106bCdiControl1.ToFrom = gi106bCdiControl1.ToFrom == ToFromState.To ? ToFromState.From : ToFromState.To;
            gi106bCdiControl1.GlideSlopeDots += 0.5f;
            if(gi106bCdiControl1.GlideSlopeDots > 2)
                gi106bCdiControl1.GlideSlopeDots = -2;
            gi106bCdiControl1.CourseDeviationDots += 0.5f;
            if(gi106bCdiControl1.CourseDeviationDots > 2)
                gi106bCdiControl1.CourseDeviationDots = -2;

            ki208CdiControl1.ObsDegrees += 1;
            if(ki208CdiControl1.ObsDegrees >= 360)
                ki208CdiControl1.ObsDegrees = 0;
            ki208CdiControl1.CourseDeviationDots += 0.5f;
            if(ki208CdiControl1.CourseDeviationDots > 2)
                ki208CdiControl1.CourseDeviationDots = -2;

            axisCalibrationControl1.CurrentValue += 1;
            if (axisCalibrationControl1.CurrentValue > 100)
                axisCalibrationControl1.CurrentValue = -100;

            if (valueProgressBar1.Value >= valueProgressBar1.Maximum)
                valueProgressBar1.Value = valueProgressBar1.Minimum;
            else
                valueProgressBar1.Value += 1;
        }
    }
}
