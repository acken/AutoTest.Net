namespace AutoTest.WinForms
{
    partial class FeedbackForm
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
            this.runFeedbackList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.labelRunState = new System.Windows.Forms.Label();
            this.linkLabelInfo = new System.Windows.Forms.LinkLabel();
            this.buttonInformation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // runFeedbackList
            // 
            this.runFeedbackList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.runFeedbackList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.runFeedbackList.FullRowSelect = true;
            this.runFeedbackList.Location = new System.Drawing.Point(12, 25);
            this.runFeedbackList.Name = "runFeedbackList";
            this.runFeedbackList.Size = new System.Drawing.Size(853, 334);
            this.runFeedbackList.TabIndex = 0;
            this.runFeedbackList.UseCompatibleStateImageBehavior = false;
            this.runFeedbackList.View = System.Windows.Forms.View.Details;
            this.runFeedbackList.SelectedIndexChanged += new System.EventHandler(this.runFeedbackList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 105;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Message";
            this.columnHeader2.Width = 720;
            // 
            // labelRunState
            // 
            this.labelRunState.AutoSize = true;
            this.labelRunState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRunState.Location = new System.Drawing.Point(12, 9);
            this.labelRunState.Name = "labelRunState";
            this.labelRunState.Size = new System.Drawing.Size(0, 13);
            this.labelRunState.TabIndex = 1;
            // 
            // linkLabelInfo
            // 
            this.linkLabelInfo.AutoSize = true;
            this.linkLabelInfo.Location = new System.Drawing.Point(12, 362);
            this.linkLabelInfo.Name = "linkLabelInfo";
            this.linkLabelInfo.Size = new System.Drawing.Size(0, 13);
            this.linkLabelInfo.TabIndex = 2;
            this.linkLabelInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelInfo_LinkClicked);
            // 
            // buttonInformation
            // 
            this.buttonInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInformation.Location = new System.Drawing.Point(839, 4);
            this.buttonInformation.Name = "buttonInformation";
            this.buttonInformation.Size = new System.Drawing.Size(25, 17);
            this.buttonInformation.TabIndex = 3;
            this.buttonInformation.UseVisualStyleBackColor = true;
            this.buttonInformation.Click += new System.EventHandler(this.buttonInformation_Click);
            // 
            // FeedbackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 384);
            this.Controls.Add(this.buttonInformation);
            this.Controls.Add(this.linkLabelInfo);
            this.Controls.Add(this.labelRunState);
            this.Controls.Add(this.runFeedbackList);
            this.Name = "FeedbackForm";
            this.Text = "AutoTest.Net - Build and test run feedback";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FeedbackForm_FormClosing);
            this.Resize += new System.EventHandler(this.FeedbackForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView runFeedbackList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label labelRunState;
        private System.Windows.Forms.LinkLabel linkLabelInfo;
        private System.Windows.Forms.Button buttonInformation;
    }
}

