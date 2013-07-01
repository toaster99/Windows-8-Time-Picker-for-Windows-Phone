using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;

namespace win8TimePicker
{
    public partial class timeWheel : UserControl
    {
        enum wheels { minutes, hours };

        //Private values
        private wheels selectedWheel = wheels.hours;

        //Public values
        public int selectedMinute { get; set; }
        public int selectedHour { get; set; }

        //Events
        public delegate void TimeChangedHandler(object sender, TimeChangedEventArgs e);
        public event TimeChangedHandler TimeChangedEvent;

        protected void OnTimeChangedEvent(TimeChangedEventArgs e)
        {
            TimeChangedEvent(this, e);
        }

        public timeWheel()
        {
            InitializeComponent();
            Touch.FrameReported += Touch_FrameReported;

            selectedMinute = DateTime.Now.Minute;
            selectedHour = DateTime.Now.Hour;
            setTime(selectedHour, selectedMinute);
        }

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            //Calculate anlge based on last touched wheel

            if (selectedWheel == wheels.hours)
            {
                //Calculate hour

                double angle = calcFingerAngle(e.GetPrimaryTouchPoint(this.hour_slider).Position, wheels.hours);
                selectedHour = (int)Math.Round((angle / 360) * 23);
                this.hour_slider.EndAngle = angle;
            }
            else
            {
                //Calculate minute

                double angle = calcFingerAngle(e.GetPrimaryTouchPoint(this.min_slider).Position, wheels.minutes);
                selectedMinute = (int)Math.Round((angle / 360) * 59);
                this.min_slider.EndAngle = angle;
            }

            //Set inner text
            this.timeText.Text = (selectedHour < 10 ? '0' + selectedHour.ToString() : selectedHour.ToString()) + ":"
                + (selectedMinute < 10 ? '0' + selectedMinute.ToString() : selectedMinute.ToString());

            //Call event
            OnTimeChangedEvent(new TimeChangedEventArgs(selectedHour, selectedMinute));
        }

        public void setTime(int hour, int minute)
        {
            //Allow the wheels to manually be set

            selectedHour = hour;
            selectedMinute = minute;

            this.hour_slider.EndAngle = (hour / 23.0) * 360.0;
            this.min_slider.EndAngle = (minute / 60.0) * 360.0;

            this.timeText.Text = (selectedHour < 10 ? '0' + selectedHour.ToString() : selectedHour.ToString()) + ":"
                + (selectedMinute < 10 ? '0' + selectedMinute.ToString() : selectedMinute.ToString());
        }

        private double calcFingerAngle(Point point3, wheels whichWheel)
        {
            //Point 1 is the center, point 2 is the top, point 3 is the finger

            //Set the points
            Point point1, point2;

            if (whichWheel == wheels.hours)
            {
                point1 = new Point(this.hour_slider.ActualWidth / 2, this.hour_slider.ActualHeight / 2);
                point2 = new Point(point1.X, 0);
            }
            else
            {
                point1 = new Point(this.min_slider.ActualWidth / 2, this.min_slider.ActualHeight / 2);
                point2 = new Point(point1.X, 0);
            }

            //Calculate the distances
            double dist12 = Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
            double dist13 = Math.Sqrt(Math.Pow((point1.X - point3.X), 2) + Math.Pow((point1.Y - point3.Y), 2));
            double dist23 = Math.Sqrt(Math.Pow((point2.X - point3.X), 2) + Math.Pow((point2.Y - point3.Y), 2));

            //Calculate the angle
            double angle = Math.Acos((Math.Pow(dist12, 2) + Math.Pow(dist13, 2) - Math.Pow(dist23, 2)) / (2 * dist12 * dist13));

            //Convert to degrees and return
            double degrees = (angle * (180 / Math.PI));
            if (point3.X < point1.X)
                degrees = 360 - degrees;

            return degrees;
        }

        private void min_slider_under_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //Set last selected wheel to minute wheel

            selectedWheel = wheels.minutes;
        }

        private void hour_slider_under_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //Set last selected wheel to hour wheel

            selectedWheel = wheels.hours;
        }

        private void hour_slider_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //Set last selected wheel to hour wheel

            selectedWheel = wheels.hours;
        }

        private void min_slider_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //Set last selected wheel to minute wheel

            selectedWheel = wheels.minutes;
        }
    }

    public class TimeChangedEventArgs : EventArgs
    {
        private int hour, minute;

        public TimeChangedEventArgs(int NewHour, int NewMinute)
        {
            hour = NewHour;
            minute = NewMinute;
        }

        public int Hour
        {
            get { return this.hour; }
        }

        public int Minute
        {
            get { return this.minute; }
        }
    }
}
