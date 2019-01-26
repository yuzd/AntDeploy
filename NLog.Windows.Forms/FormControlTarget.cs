using System;
using System.ComponentModel;
using System.Windows.Forms;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace NLog.Windows.Forms
{
    /// <summary>
    /// Logs text to Windows.Forms.Control.Text property control of specified Name.
    /// </summary>
    /// <example>
    /// <p>
    /// To set up the target in the <a href="config.html">configuration file</a>, 
    /// use the following syntax:
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/FormControl/NLog.config" />
    /// <p>
    /// The result is:
    /// </p>
    /// <img src="examples/targets/Screenshots/FormControl/FormControl.gif" />
    /// <p>
    /// To set up the log target programmatically similar to above use code like this:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/FormControl/Form1.cs" />,
    /// </example>
    [Target("FormControl")]
    public sealed class FormControlTarget : TargetWithLayout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormControlTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public FormControlTarget()
        {
            Append = true;
        }

        private delegate void DelSendTheMessageToFormControl(Control control, string logMessage);

        /// <summary>
        /// Gets or sets the name of control to which NLog will log write log text.
        /// </summary>
        /// <docgen category='Form Options' order='10' />
        [RequiredParameter]
        public string ControlName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether log text should be appended to the text of the control instead of overwriting it. </summary>
        /// <docgen category='Form Options' order='10' />
        [DefaultValue(true)]
        public bool Append { get; set; }

        /// <summary>
        /// Gets or sets the name of the Form on which the control is located.
        /// </summary>
        /// <docgen category='Form Options' order='10' />
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets whether new log entry are added to the start or the end of the control
        /// </summary>
        public bool ReverseOrder { get; set; }

        /// <summary>
        /// Log message to control.
        /// </summary>
        /// <param name="logEvent">
        /// The logging event.
        /// </param>
        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = Layout.Render(logEvent);

            FindControlAndSendTheMessage(logMessage);
        }

        private void FindControlAndSendTheMessage(string logMessage)
        {
            Form form = null;

            if (Form.ActiveForm != null)
            {
                form = Form.ActiveForm;
            }

            if (Application.OpenForms[FormName] != null)
            {
                form = Application.OpenForms[FormName];
            }

            if (form == null)
            {
                InternalLogger.Info("Form {0} not found", FormName);
                return;
            }

            Control control = FormHelper.FindControl(ControlName, form);

            if (control == null)
            {
                InternalLogger.Info("Control {0} on Form {1} not found", ControlName, FormName);
                return;
            }
            try
            {
                control.BeginInvoke(new DelSendTheMessageToFormControl(SendTheMessageToFormControl), control, logMessage);
            }
            catch (Exception ex)
            {
                InternalLogger.Warn(ex.ToString());

                if (LogManager.ThrowExceptions)
                {
                    throw;
                }
            }

        }

        private void SendTheMessageToFormControl(Control control, string logMessage)
        {
            //append of replace?
            if (Append)
            {
                //beginning or end?
                if (ReverseOrder)
                    control.Text = logMessage + control.Text;
                else
                    control.Text += logMessage;
            }
            else
            {
                control.Text = logMessage;
            }
        }
    }
}
