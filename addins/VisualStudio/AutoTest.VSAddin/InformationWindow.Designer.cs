namespace AutoTest.VSAddin
{
    partial class InformationWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewInformation = new System.Windows.Forms.ListView();
            this.columnDescription = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewInformation
            // 
            this.listViewInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewInformation.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDescription});
            this.listViewInformation.Location = new System.Drawing.Point(12, 12);
            this.listViewInformation.Name = "listViewInformation";
            this.listViewInformation.Size = new System.Drawing.Size(725, 164);
            this.listViewInformation.TabIndex = 0;
            this.listViewInformation.UseCompatibleStateImageBehavior = false;
            this.listViewInformation.View = System.Windows.Forms.View.Details;
            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 700;
            // 
            // InformationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 188);
            this.Controls.Add(this.listViewInformation);
            this.Name = "InformationWindow";
            this.Text = "InformationWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InformationWindow_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewInformation;
        private System.Windows.Forms.ColumnHeader columnDescription;
    }
}