namespace TriStateTreeView
{
    partial class TriStateTreeViewCtrl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriStateTreeViewCtrl));
            this.ctlStateImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // ctlStateImageList
            // 
            this.ctlStateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ctlStateImageList.ImageStream")));
            this.ctlStateImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ctlStateImageList.Images.SetKeyName(0, "StateNone16.ico");
            this.ctlStateImageList.Images.SetKeyName(1, "StateUnchecked16.ico");
            this.ctlStateImageList.Images.SetKeyName(2, "StateChecked16.ico");
            this.ctlStateImageList.Images.SetKeyName(3, "StateIndeterminate16.ico");
            // 
            // TriStateTreeViewCtrl
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "TriStateTreeViewCtrl";
            this.Size = new System.Drawing.Size(342, 276);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList ctlStateImageList;
    }
}
