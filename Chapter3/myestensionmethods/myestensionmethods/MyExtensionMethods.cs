using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace myestensionmethods
{
    public static class MyExtensionMethods
    {
        /// <summary>
        /// Message posted when the user presses the left mouse button while
        /// the cursor is within the nonclient area of a window. The window
        /// which has the cursor receives the message.
        /// </summary>
        private const int WM_NCLBUTTONDOWN = 0x00A1;

        /// <summary>
        /// Indicates that the screen coordinates belong to the
        /// title bar.
        /// </summary>
        private const int HTCAPTION = 2;

        /// <summary>
        /// Creates a WM_NCLBUTTONDOWN message for the specified form. This
        /// method provides a mechanism to drag the form from anywhere, including the
        /// title bar, if there is one.
        /// </summary>
        /// <param name="targetForm">The form instance which will receive the message</param>
        /// <returns></returns>
        public static Message CreateWmNclButtonDown(this Form targetForm) => Message.Create(targetForm.Handle, WM_NCLBUTTONDOWN, new IntPtr(HTCAPTION), IntPtr.Zero);

        /// <summary>
        /// Draw the hands for an analog clock face in a client area (usually a form's client area).
        /// </summary>
        /// <param name="graphicsInstance">Graphics object asociated to the client area.</param>
        /// <param name="clientSize">Width and Height of the client area.</param>
        /// <param name="handsColor">Color used to draw the hour and minute hands.</param>
        /// <param name="secondHandColor">Color used to draw the seconds hand.</param>
        public static void DrawClockHands(this Graphics graphicsInstance, Size clientSize,Color handsColor, Color secondHandColor)
        {
            using (var handsPen = new Pen(handsColor, 4))
            {
                // Get the hour and the minute including any fraction elapsed.
                DateTime currentDateTime = DateTime.Now;
                var currentHour = currentDateTime.Hour + currentDateTime.Minute / 60f + currentDateTime.Second / 3600f;
                var currentMinute = currentDateTime.Minute + currentDateTime.Second / 60f;

                // Gets the center point for the clock face
                var clockFaceCenter = new PointF(0, 0);

                //Set the scale factor for drawing the hour hand.
                var hourHandXFactor = 0.2f * clientSize.Width;
                var hourHandYFactor = 0.2f * clientSize.Height;

                //Gets the angle for the current hour.
                var hourHandAngle = -Math.PI / 2 + 2 * Math.PI * currentHour / 12.0;

                //Gets the hour hand end point
                var hourHandEndPoint = new PointF((float)(hourHandXFactor * Math.Cos(hourHandAngle)),(float)(hourHandYFactor * Math.Sin(hourHandAngle)));

                //Assing the hands color to the drawing pen
                handsPen.Color = handsColor;

                //Draws the hour hand
                graphicsInstance.DrawLine(handsPen, hourHandEndPoint, clockFaceCenter);

                //Set the scale factor for the minute hand
                float minuteHandXFactor = 0.3f * clientSize.Width;
                float minuteHandYFactor = 0.3f * clientSize.Height;

                //Gets the angle for the current minute
                double minuteHandAngle = -Math.PI / 2 + 2 * Math.PI * currentMinute / 60.0;

                //Gets the endpoint for the minute hand
                var minuteHandEndPoint = new PointF((float)(minuteHandXFactor * Math.Cos(minuteHandAngle)), (float)(minuteHandYFactor * Math.Sin(minuteHandAngle)));

                //Sets the pen width to 2 pixels for minute hand
                handsPen.Width = 2;

                //Draws the minute hand.
                graphicsInstance.DrawLine(handsPen, minuteHandEndPoint, clockFaceCenter);

                //Set the scale factor for second hand
                var secondHandXFactor = 0.4f * clientSize.Width;
                var secondHadYFactor = 0.4f * clientSize.Height;

                //Gets the angle for the current second
                var second_angle = -Math.PI / 2 + 2 * Math.PI * currentDateTime.Second / 60.0;

                //Gets the end point for the second hand
                var secondEndPoint = new PointF((float)(secondHandXFactor * Math.Cos(second_angle)),(float)(secondHadYFactor * Math.Sin(second_angle)));

                //Sets the color for the second hand
                handsPen.Color = secondHandColor;
                handsPen.Width = 1;

                //Draws the second hand.
                graphicsInstance.DrawLine(Pens.Red, secondEndPoint, clockFaceCenter);
            }
        }

        /// <summary>
        /// Draws an analog clock face in a client area (usually a form's client area).
        /// </summary>
        /// <param name="graphicsInstance">Graphics object asociated to the client area.</param>
        /// <param name="clientSize">Width and Height of the client area.</param>
        /// <param name="penColor">Color used to draw the analog clock face.</param>
        /// <param name="penWidth">Width of the pen used to draw the analog clock face.</param>
        public static void DrawClockFace(this Graphics graphicsInstance, Size clientSize, Color penColor, int penWidth)
        {
            using (var clockPen = new Pen(penColor, penWidth))
            {
                //Draws the clock face's outline
                graphicsInstance.DrawCircle(clientSize, penColor, penWidth);

                //Draws the tick marks around the clock's face outline

                //Defines a round cap for beginning and ending of every line
                clockPen.StartCap = LineCap.Round;
                clockPen.EndCap = LineCap.Round;

                //Defines the scale factors used to draw
                //the tick marks around the clock's face

                //Scale factors for the mark's outer point (x,y)
                var outXFactor = 0.45f * clientSize.Width;
                var outYFactor = 0.45f * clientSize.Height;

                //Scale factors for the mark's inner point (x,y)
                var innXFactor = 0.425f * clientSize.Width;
                var innYFactor = 0.425f * clientSize.Height;

                //Scale factors for the hour mark's inner point (x,y)
                var hourXFactor = 0.4f * clientSize.Width;
                var hourYFactor = 0.4f * clientSize.Height;

                //Loop for iterating the hour's minutes
                for (int minute = 1; minute <= 60; minute++)
                {
                    //Calculates the angle for every minute
                    var minuteAngle = Math.PI * minute / 30.0;
                    var cosineAngle = (float)Math.Cos(minuteAngle);
                    var sineAngle = (float)Math.Sin(minuteAngle);

                    //Calculates inner and outer points
                    //for the current tick mark
                    var innerPoint = (minute % 5 == 0) ? new PointF(hourXFactor * cosineAngle,hourYFactor * sineAngle) : new PointF(innXFactor * cosineAngle, innYFactor * sineAngle);
                    var outerPoint = new PointF(outXFactor * cosineAngle,outYFactor * sineAngle);

                    graphicsInstance.DrawLine((minute % 5 == 0) ? clockPen : Pens.Black,innerPoint,outerPoint);
                }
            }
        }

        /// <summary>
        /// Draws a circle within a client area (usually a form's client area).
        /// </summary>
        /// <param name="graphicsInstance">Graphics object asociated to the client area.</param>
        /// <param name="clientSize">Width and Height of the client area.</param>
        /// <param name="penColor">Color used to draw the circle.</param>
        /// <param name="penWidth">Width of the pen used to draw the circle.</param>
        public static void DrawCircle(this Graphics graphicsInstance,Size clientSize,Color penColor, int penWidth)
        {
            using (var circlePen = new Pen(penColor, penWidth))
            {
                graphicsInstance.DrawEllipse(circlePen, -clientSize.Width / 2, -clientSize.Height / 2, clientSize.Width, clientSize.Height);
            }
        }

        /// <summary>
        /// Builds a context menu based on the elements of a Dictionary.
        /// </summary>
        /// <param name="contextMenu">Instance of the context menu to build.</param>
        /// <param name="menuItems">Dictionary with all menu items. The key is used in the Tag property to identify the item</param>
        /// <param name="clickEventHandler">Handler for the Click event of every item</param>
        public static void BuildContextMenu(this ContextMenuStrip contextMenu, Dictionary<string,string> menuItems, EventHandler clickEventHandler)
        {
            menuItems.ToList().ForEach(item =>
            {
                var menuItem = new ToolStripMenuItem
                {
                    Text = item.Value,
                    Tag = item.Key
                };
                menuItem.Click += clickEventHandler;

                contextMenu.Items.Add(menuItem);
            });
        }

    }
}
