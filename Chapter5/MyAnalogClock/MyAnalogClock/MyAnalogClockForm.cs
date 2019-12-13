using myextensionmethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MyAnalogClock
{
    /// <summary>
    /// Displays an analog clock on the screen
    /// </summary>
    public partial class MyAnalogClockForm : Form
    {
        /// <summary>
        /// Instance of the context menu which allows us
        /// to exit the application.
        /// </summary>
        private ContextMenuStrip _contextMenu;

        /// <summary>
        /// Dictionary with the form's context menu items.
        /// The key is assigned to the Tag property of the ToolStripMenuItem
        /// </summary>
        private readonly Dictionary<string, string> _contextMenuItems = new Dictionary<string, string>
        {
            {"EXIT","Exit"}
        };

        /// <summary>
        /// Instance of the timer in charge of updating the clock.
        /// </summary>
        private Timer _clockTimer;

        /// <summary>
        /// Creates the instance of the form.
        /// </summary>
        public MyAnalogClockForm()
        {
            InitializeComponent();

            //Subscribes the Paint event of the form
            Paint += MyAnalogClockForm_Paint;

            //Subscribes the Load event of the form
            Load += MyAnalogClockForm_Load;

            //Subscribes the MouseDown event of the form
            //in order to allow dragging it
            MouseDown += MyAnalogClockForm_MouseDown;

        }

        /// <summary>
        /// Handles the Load event of the form.
        /// </summary>
        /// <param name="sender">Instance of the form that raises the event.</param>
        /// <param name="e">Instance of <see cref="EventArgs"/> which holds the data for managing the event.</param>
        private void MyAnalogClockForm_Load(object sender, EventArgs e)
        {
            //Removes title bar and border
            //from the form.
            Text = string.Empty;
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;

            //Creates the instance of the context menu
            _contextMenu = new ContextMenuStrip();

            //Builds the context menu using the items dictionary.
            //Subscribes the Click event for every item to the MenuItem_Click method.
            _contextMenu.BuildContextMenu(_contextMenuItems, MenuItem_Click);

            // Attach the context menu to the form.
            ContextMenuStrip = _contextMenu;

            // Set the size to a square of 300x300.
            SetSize(300, 300);

            //Use double buffering to avoid flickering.
            DoubleBuffered = true;

            //The form will not be shown in the task bar.
            ShowInTaskbar = false;

            // Set focus to the form. You should do this
            // in order to make Alt+F4 close the clock properly.
            Focus();

            //Creates an instance of a timer
            //and adjusts the time for firing the
            //Tick event (1000 milliseconds == 1 second)
            _clockTimer = new Timer
            {
                Interval = 1000
            };

            //Subscribes the Tick event for the timer instance
            _clockTimer.Tick += _clockTimer_Tick;
            _clockTimer.Enabled = true;
        }

        /// <summary>
        /// Handles the MouseDown event of the form, in order to
        /// allow dragging from anywhere on the clock's surface.
        /// </summary>
        /// <param name="sender">Instance of the form that raises the event.</param>
        /// <param name="e">Instance of <see cref="EventArgs"/> which holds the data for managing the event.</param>
        private void MyAnalogClockForm_MouseDown(object sender, MouseEventArgs e)
        {
            //If we hold the mouse left button we can drag the clock.
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture started by the mouse down.
                Capture = false;

                //Create the windows message
                var wMessage = this.CreateWmNclButtonDown();

                //Sends the message to the form's instance
                DefWndProc(ref wMessage);
            }
        }

        // Redraws the clock to show the current hand position.
        private void _clockTimer_Tick(object sender, EventArgs e) => Refresh();

        /// <summary>
        /// Handles the Paint event of the form, and draws the clock.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyAnalogClockForm_Paint(object sender, PaintEventArgs e)
        {
            //Clear the drawing surface of the form
            //and fills it with the form's BackColor
            e.Graphics.Clear(BackColor);

            //Sets the quality for graphics rendering
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //Sets the quality for text rendering
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            //Translates the graphics coordinates to
            //center the drawing.
            e.Graphics.TranslateTransform(ClientSize.Width / 2, ClientSize.Height / 2);

            // Draws the clock face including tick marks.
            e.Graphics.DrawClockFace(ClientSize, Color.Blue, 4);

            // Draws the center of the clock.
            // This code was implemented in myextensionmethdos

            // Draws the clock hands.
            e.Graphics.DrawClockHands(ClientSize, Color.Red, Color.OrangeRed);
        }

        /// <summary>
        /// Handles the click event for every item in the context menu.
        /// Since there is only one item (Exit), executes the Close method
        /// to close the form and exit the application.
        /// </summary>
        /// <param name="sender">Instance of the menu item which raises the event.</param>
        /// <param name="e">Instance of <see cref="EventArgs"/> which holds the data for managing the event.</param>
        private void MenuItem_Click(object sender, EventArgs e) => Close();

        /// <summary>
        /// Set the form's size.
        /// </summary>
        /// <param name="clientWidth">Form's width in pixels.</param>
        /// <param name="clientHeight">Form's height in pixels.</param>
        private void SetSize(int clientWidth, int clientHeight)
        {
            //Set the form's size.
            ClientSize = new Size(clientWidth, clientHeight);

            // Set the form's region to a circle.
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(ClientRectangle);
            Region = new Region(path);

            // Redraw the form.
            Refresh();
        }
    }
}
