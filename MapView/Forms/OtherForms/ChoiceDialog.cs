using System;


namespace MapView
{
	/// <summary>
	/// Options for the ChoiceDialog.
	/// </summary>
	internal enum Choice
	{
		Overwrite,
		UseExisting
	};


	/// <summary>
	/// This form is used only for adding a new Map with the PathsEditor.
	/// TODO: replace ChoiceDialog with a standard .NET dialog.
	/// </summary>
	internal sealed class ChoiceDialog
		:
			System.Windows.Forms.Form
	{
		private Choice _choice;


		internal ChoiceDialog(string file)
		{
			InitializeComponent();
			txt.Text = string.Format(
								System.Globalization.CultureInfo.InvariantCulture,
								"{0} already exists.{1}{1}Do you want to overwrite the file?",
								file,
								Environment.NewLine);
		}


		internal Choice Choice // hello DialogResult hello.
		{
			get { return _choice; }
		}

		private void overwrite_Click(object sender, System.EventArgs e)
		{
			_choice = Choice.Overwrite;
			Close();
		}

		private void exist_Click(object sender, System.EventArgs e)
		{
			_choice = Choice.UseExisting;
			Close();
		}


		#region Windows Form Designer generated code
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txt = new System.Windows.Forms.Label();
			this.overwrite = new System.Windows.Forms.Button();
			this.exist = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txt
			// 
			this.txt.Dock = System.Windows.Forms.DockStyle.Top;
			this.txt.Location = new System.Drawing.Point(0, 0);
			this.txt.Name = "txt";
			this.txt.Size = new System.Drawing.Size(312, 50);
			this.txt.TabIndex = 0;
			// 
			// overwrite
			// 
			this.overwrite.Location = new System.Drawing.Point(30, 60);
			this.overwrite.Name = "overwrite";
			this.overwrite.Size = new System.Drawing.Size(110, 30);
			this.overwrite.TabIndex = 1;
			this.overwrite.Text = "Clobber";
			this.overwrite.Click += new System.EventHandler(this.overwrite_Click);
			// 
			// exist
			// 
			this.exist.Location = new System.Drawing.Point(170, 60);
			this.exist.Name = "exist";
			this.exist.Size = new System.Drawing.Size(110, 30);
			this.exist.TabIndex = 3;
			this.exist.Text = "Use it as is";
			this.exist.Click += new System.EventHandler(this.exist_Click);
			// 
			// ChoiceDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(312, 99);
			this.Controls.Add(this.exist);
			this.Controls.Add(this.overwrite);
			this.Controls.Add(this.txt);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChoiceDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "decisions decisions decisions";
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label txt;
		private System.Windows.Forms.Button overwrite;
		private System.Windows.Forms.Button exist;
		private System.ComponentModel.Container components = null;
	}
}
