using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.Windows.Forms
{
    /// <summary>
    /// How to handle messages when switching between target controls or no control is attached at all
    /// </summary>
    public enum RichTextBoxTargetMessageRetentionStrategy
    {
        /// <summary>
        /// Just skip logging events when no target control attached. Only new messages would be sent to rich text box after attachement.
        /// No additional resources spent on this.
        /// </summary>
        None,

        /// <summary>
        /// Store logging events only during periods when no target control attached. Only these messages would be sent to rich text box after attachment. Messages that were sent to previous textbox are not stored and would not be shown.
        /// Number of events stored is limited by <see cref="RichTextBoxTarget.MaxLines"/>.
        /// </summary>
        OnlyMissed,

        /// <summary>
        /// Store all events in internal queue. After attaching to a new control all the stored messages would be repeated in it, including messages that were sent to previous textbox.
        /// Number of messages stored is limited by <see cref="RichTextBoxTarget.MaxLines"/>.
        /// </summary>
        All,
    }
}
