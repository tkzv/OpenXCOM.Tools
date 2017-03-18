using System;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace MapView
{
	public class ChangeMapSizeForm
		:
		Form
	{
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtC;
		private System.Windows.Forms.TextBox txtR;
		private System.Windows.Forms.TextBox txtH;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox oldC;
		private System.Windows.Forms.TextBox oldR;
		private System.Windows.Forms.TextBox oldH;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label2;

		private CheckBox CeilingCheckBox;
		private IMap_Base _baseMap;


		public ChangeMapSizeForm()
		{
			InitializeComponent();
			DialogResult = DialogResult.Cancel;
		}


		public IMap_Base Map
		{
			get { return _baseMap; }
			set
			{
				if ((_baseMap = value) != null)
				{
					oldR.Text =
					txtR.Text = _baseMap.MapSize.Rows.ToString(System.Globalization.CultureInfo.InvariantCulture);
					oldC.Text =
					txtC.Text = _baseMap.MapSize.Cols.ToString(System.Globalization.CultureInfo.InvariantCulture);
					oldH.Text =
					txtH.Text = _baseMap.MapSize.Height.ToString(System.Globalization.CultureInfo.InvariantCulture);
				}
			}
		}

		private int _rows;
		private int _cols;
		private int _height;

		public int NewRows
		{
			get { return _rows; }
			private set { _rows = value; }
		}

		public int NewCols
		{
			get { return _cols; }
			private set { _cols = value; }
		}

		public int NewHeight
		{
			get { return _height; }
			private set { _height = value; }
		}

		public bool AddHeightToCeiling
		{
			get { return CeilingCheckBox.Checked; }
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			try
			{
				var icon = MessageBoxIcon.None;
				string title = null;
				string message = null;

				_cols   = int.Parse(txtC.Text, System.Globalization.CultureInfo.InvariantCulture);
				_rows   = int.Parse(txtR.Text, System.Globalization.CultureInfo.InvariantCulture);
				_height = int.Parse(txtH.Text, System.Globalization.CultureInfo.InvariantCulture);

				if (_cols > 0 && _rows > 0 && _height > 0)
				{
					if (_cols % 10 == 0 && _rows % 10 == 0)
					{
						int colsOld   = int.Parse(oldC.Text, System.Globalization.CultureInfo.InvariantCulture);
						int rowsOld   = int.Parse(oldR.Text, System.Globalization.CultureInfo.InvariantCulture);
						int heightOld = int.Parse(oldH.Text, System.Globalization.CultureInfo.InvariantCulture);

						if (colsOld != _cols || rowsOld != _rows || heightOld != _height)
						{
							DialogResult = DialogResult.OK;
							Close();
						}
						else
						{
							icon = MessageBoxIcon.Information;
							title = "Notice";
							message = "The new size is the same as the old size.";
						}
					}
					else
					{
						icon = MessageBoxIcon.Exclamation;
						title = "Error";
						message = "Columns and Rows must be evenly divisible by 10.";
					}
				}
				else
				{
					icon = MessageBoxIcon.Exclamation;
					title = "Error";
					message = "Values must be positive integers greater than 0.";
				}

				if (icon != MessageBoxIcon.None)
				{
					MessageBox.Show(
								this,
								message,
								title,
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation,
								MessageBoxDefaultButton.Button1,
								0);
				}
			}
			catch
			{
				MessageBox.Show(
							this,
							"Columns and Rows must be evenly divisible by 10 and Height must be 1 or more.",
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation,
							MessageBoxDefaultButton.Button1,
							0);
				throw;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void txtH_TextChanged(object sender, EventArgs e)
		{
			int current = int.Parse(txtH.Text, System.Globalization.CultureInfo.InvariantCulture);
			CeilingCheckBox.Visible = (current != _baseMap.MapSize.Height);
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
			this.oldC = new System.Windows.Forms.TextBox();
			this.oldR = new System.Windows.Forms.TextBox();
			this.oldH = new System.Windows.Forms.TextBox();
			this.txtR = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtC = new System.Windows.Forms.TextBox();
			this.txtH = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.CeilingCheckBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// oldC
			// 
			this.oldC.Location = new System.Drawing.Point(70, 55);
			this.oldC.Name = "oldC";
			this.oldC.ReadOnly = true;
			this.oldC.Size = new System.Drawing.Size(45, 19);
			this.oldC.TabIndex = 7;
			// 
			// oldR
			// 
			this.oldR.Location = new System.Drawing.Point(20, 55);
			this.oldR.Name = "oldR";
			this.oldR.ReadOnly = true;
			this.oldR.Size = new System.Drawing.Size(45, 19);
			this.oldR.TabIndex = 6;
			// 
			// oldH
			// 
			this.oldH.Location = new System.Drawing.Point(120, 55);
			this.oldH.Name = "oldH";
			this.oldH.ReadOnly = true;
			this.oldH.Size = new System.Drawing.Size(45, 19);
			this.oldH.TabIndex = 8;
			// 
			// txtR
			// 
			this.txtR.Location = new System.Drawing.Point(20, 95);
			this.txtR.Name = "txtR";
			this.txtR.Size = new System.Drawing.Size(45, 19);
			this.txtR.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(70, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "c";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(20, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 15);
			this.label4.TabIndex = 7;
			this.label4.Text = "r";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(120, 40);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(45, 15);
			this.label5.TabIndex = 8;
			this.label5.Text = "h";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtC
			// 
			this.txtC.Location = new System.Drawing.Point(70, 95);
			this.txtC.Name = "txtC";
			this.txtC.Size = new System.Drawing.Size(45, 19);
			this.txtC.TabIndex = 2;
			// 
			// txtH
			// 
			this.txtH.Location = new System.Drawing.Point(120, 95);
			this.txtH.Name = "txtH";
			this.txtH.Size = new System.Drawing.Size(45, 19);
			this.txtH.TabIndex = 3;
			this.txtH.TextChanged += new System.EventHandler(this.txtH_TextChanged);
			// 
			// btnOk
			// 
			this.btnOk.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOk.Location = new System.Drawing.Point(125, 130);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(85, 40);
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.Location = new System.Drawing.Point(220, 130);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(85, 40);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// CeilingCheckBox
			// 
			this.CeilingCheckBox.AutoSize = true;
			this.CeilingCheckBox.Checked = true;
			this.CeilingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CeilingCheckBox.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CeilingCheckBox.Location = new System.Drawing.Point(190, 95);
			this.CeilingCheckBox.Name = "CeilingCheckBox";
			this.CeilingCheckBox.Size = new System.Drawing.Size(99, 16);
			this.CeilingCheckBox.TabIndex = 9;
			this.CeilingCheckBox.Text = "add to ceiling";
			this.CeilingCheckBox.UseVisualStyleBackColor = true;
			this.CeilingCheckBox.Visible = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(120, 80);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 15);
			this.label1.TabIndex = 12;
			this.label1.Text = "h";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(20, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(45, 15);
			this.label6.TabIndex = 11;
			this.label6.Text = "r";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(70, 80);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(45, 15);
			this.label7.TabIndex = 10;
			this.label7.Text = "c";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(305, 28);
			this.label2.TabIndex = 13;
			this.label2.Text = "Rows and Columns must be multiples of 10 (10, 20, 30, etc) and Height must be 1 o" +
	"r more.";
			// 
			// ChangeMapSize
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(314, 176);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.CeilingCheckBox);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.txtH);
			this.Controls.Add(this.txtC);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtR);
			this.Controls.Add(this.oldC);
			this.Controls.Add(this.oldH);
			this.Controls.Add(this.oldR);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(320, 200);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 200);
			this.Name = "ChangeMapSize";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Change Map Size";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}
