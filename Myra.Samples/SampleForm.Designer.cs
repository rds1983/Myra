namespace Myra.Samples
{
	partial class SampleForm
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

		#region Windows Window Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._listBoxSamples = new System.Windows.Forms.ListBox();
			this._buttonRun = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _listBoxSamples
			// 
			this._listBoxSamples.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._listBoxSamples.FormattingEnabled = true;
			this._listBoxSamples.Location = new System.Drawing.Point(12, 12);
			this._listBoxSamples.Name = "_listBoxSamples";
			this._listBoxSamples.Size = new System.Drawing.Size(199, 407);
			this._listBoxSamples.TabIndex = 0;
			this._listBoxSamples.SelectedIndexChanged += new System.EventHandler(this._listBoxSamples_SelectedIndexChanged);
			// 
			// _buttonRun
			// 
			this._buttonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonRun.Location = new System.Drawing.Point(136, 432);
			this._buttonRun.Name = "_buttonRun";
			this._buttonRun.Size = new System.Drawing.Size(75, 23);
			this._buttonRun.TabIndex = 1;
			this._buttonRun.Text = "&Run...";
			this._buttonRun.UseVisualStyleBackColor = true;
			this._buttonRun.Click += new System.EventHandler(this._buttonRun_Click);
			// 
			// SampleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(223, 467);
			this.Controls.Add(this._buttonRun);
			this.Controls.Add(this._listBoxSamples);
			this.Name = "SampleForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SampleForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox _listBoxSamples;
		private System.Windows.Forms.Button _buttonRun;
	}
}