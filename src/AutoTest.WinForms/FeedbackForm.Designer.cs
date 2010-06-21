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
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // runFeedbackList
            // 
            this.runFeedbackList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.runFeedbackList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.runFeedbackList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.runFeedbackList.FullRowSelect = true;
            this.runFeedbackList.Location = new System.Drawing.Point(12, 25);
            this.runFeedbackList.Name = "runFeedbackList";
            this.runFeedbackList.Size = new System.Drawing.Size(853, 217);
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
            this.labelRunState.Location = new System.Drawing.Point(12, 9);
            this.labelRunState.Name = "labelRunState";
            this.labelRunState.Size = new System.Drawing.Size(0, 13);
            this.labelRunState.TabIndex = 1;
            // 
            // richTextBoxInfo
            // 
            this.richTextBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxInfo.Location = new System.Drawing.Point(12, 248);
            this.richTextBoxInfo.Name = "richTextBoxInfo";
            this.richTextBoxInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxInfo.Size = new System.Drawing.Size(853, 141);
            this.richTextBoxInfo.TabIndex = 2;
            this.richTextBoxInfo.Text = "";
            // 
            // FeedbackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 401);
            this.Controls.Add(this.richTextBoxInfo);
            this.Controls.Add(this.labelRunState);
            this.Controls.Add(this.runFeedbackList);
            this.Name = "FeedbackForm";
            this.Text = "FeedbackForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView runFeedbackList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label labelRunState;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
    }
}

