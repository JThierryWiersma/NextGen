namespace Generator
{
    partial class Options
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.tabOptions = new System.Windows.Forms.TabControl();
            this.tabEditor = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtGeneratedTabstops = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.txtTemplateTabstops = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnErrorColor = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtError = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnGeneratedCodeColor = new System.Windows.Forms.Button();
            this.btnExpressionColor = new System.Windows.Forms.Button();
            this.btnClassColor = new System.Windows.Forms.Button();
            this.btnCommentColor = new System.Windows.Forms.Button();
            this.btnCommandColor = new System.Windows.Forms.Button();
            this.txtGeneratedCode = new System.Windows.Forms.TextBox();
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.txtClass = new System.Windows.Forms.TextBox();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.tabOptions.SuspendLayout();
            this.tabEditor.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeneratedTabstops)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTemplateTabstops)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabOptions
            // 
            this.tabOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabOptions.Controls.Add(this.tabEditor);
            this.tabOptions.Location = new System.Drawing.Point(12, 8);
            this.tabOptions.Multiline = true;
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.SelectedIndex = 0;
            this.tabOptions.Size = new System.Drawing.Size(369, 304);
            this.tabOptions.TabIndex = 0;
            // 
            // tabEditor
            // 
            this.tabEditor.Controls.Add(this.groupBox2);
            this.tabEditor.Controls.Add(this.groupBox1);
            this.tabEditor.Location = new System.Drawing.Point(4, 22);
            this.tabEditor.Name = "tabEditor";
            this.tabEditor.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditor.Size = new System.Drawing.Size(361, 278);
            this.tabEditor.TabIndex = 0;
            this.tabEditor.Text = "Template editing";
            this.tabEditor.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtGeneratedTabstops);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtTemplateTabstops);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(15, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(329, 78);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tab settings";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(256, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "columns";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(256, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "columns";
            // 
            // txtGeneratedTabstops
            // 
            this.txtGeneratedTabstops.Location = new System.Drawing.Point(212, 46);
            this.txtGeneratedTabstops.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.txtGeneratedTabstops.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtGeneratedTabstops.Name = "txtGeneratedTabstops";
            this.txtGeneratedTabstops.Size = new System.Drawing.Size(38, 20);
            this.txtGeneratedTabstops.TabIndex = 3;
            this.txtGeneratedTabstops.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtGeneratedTabstops.ValueChanged += new System.EventHandler(this.txtGeneratedTabstops_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(181, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Generated code/text tabstops every:";
            // 
            // txtTemplateTabstops
            // 
            this.txtTemplateTabstops.Location = new System.Drawing.Point(212, 20);
            this.txtTemplateTabstops.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.txtTemplateTabstops.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtTemplateTabstops.Name = "txtTemplateTabstops";
            this.txtTemplateTabstops.Size = new System.Drawing.Size(38, 20);
            this.txtTemplateTabstops.TabIndex = 1;
            this.txtTemplateTabstops.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtTemplateTabstops.ValueChanged += new System.EventHandler(this.txtTemplateTabstops_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(173, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Template language tabstops every:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnErrorColor);
            this.groupBox1.Controls.Add(this.txtError);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.btnGeneratedCodeColor);
            this.groupBox1.Controls.Add(this.btnExpressionColor);
            this.groupBox1.Controls.Add(this.btnClassColor);
            this.groupBox1.Controls.Add(this.btnCommentColor);
            this.groupBox1.Controls.Add(this.btnCommandColor);
            this.groupBox1.Controls.Add(this.txtGeneratedCode);
            this.groupBox1.Controls.Add(this.txtExpression);
            this.groupBox1.Controls.Add(this.txtClass);
            this.groupBox1.Controls.Add(this.txtComment);
            this.groupBox1.Controls.Add(this.txtCommand);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(15, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(329, 173);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Colors";
            // 
            // btnErrorColor
            // 
            this.btnErrorColor.ImageIndex = 0;
            this.btnErrorColor.ImageList = this.imageList1;
            this.btnErrorColor.Location = new System.Drawing.Point(298, 141);
            this.btnErrorColor.Margin = new System.Windows.Forms.Padding(0);
            this.btnErrorColor.Name = "btnErrorColor";
            this.btnErrorColor.Size = new System.Drawing.Size(20, 20);
            this.btnErrorColor.TabIndex = 16;
            this.btnErrorColor.UseVisualStyleBackColor = true;
            this.btnErrorColor.Click += new System.EventHandler(this.btnErrorColor_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Black;
            this.imageList1.Images.SetKeyName(0, "mspaint_exe_Ico9_ico_Ico1.ico");
            // 
            // txtError
            // 
            this.txtError.BackColor = System.Drawing.Color.White;
            this.txtError.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtError.Location = new System.Drawing.Point(125, 142);
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(170, 18);
            this.txtError.TabIndex = 15;
            this.txtError.Text = "Unrecognized code";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 143);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Errors:";
            // 
            // btnGeneratedCodeColor
            // 
            this.btnGeneratedCodeColor.ImageIndex = 0;
            this.btnGeneratedCodeColor.ImageList = this.imageList1;
            this.btnGeneratedCodeColor.Location = new System.Drawing.Point(298, 117);
            this.btnGeneratedCodeColor.Margin = new System.Windows.Forms.Padding(0);
            this.btnGeneratedCodeColor.Name = "btnGeneratedCodeColor";
            this.btnGeneratedCodeColor.Size = new System.Drawing.Size(20, 20);
            this.btnGeneratedCodeColor.TabIndex = 13;
            this.btnGeneratedCodeColor.UseVisualStyleBackColor = true;
            this.btnGeneratedCodeColor.Click += new System.EventHandler(this.btnGeneratedCodeColor_Click);
            // 
            // btnExpressionColor
            // 
            this.btnExpressionColor.ImageIndex = 0;
            this.btnExpressionColor.ImageList = this.imageList1;
            this.btnExpressionColor.Location = new System.Drawing.Point(298, 93);
            this.btnExpressionColor.Margin = new System.Windows.Forms.Padding(0);
            this.btnExpressionColor.Name = "btnExpressionColor";
            this.btnExpressionColor.Size = new System.Drawing.Size(20, 20);
            this.btnExpressionColor.TabIndex = 12;
            this.btnExpressionColor.UseVisualStyleBackColor = true;
            this.btnExpressionColor.Click += new System.EventHandler(this.btnExpressionColor_Click);
            // 
            // btnClassColor
            // 
            this.btnClassColor.ImageIndex = 0;
            this.btnClassColor.ImageList = this.imageList1;
            this.btnClassColor.Location = new System.Drawing.Point(298, 69);
            this.btnClassColor.Margin = new System.Windows.Forms.Padding(0);
            this.btnClassColor.Name = "btnClassColor";
            this.btnClassColor.Size = new System.Drawing.Size(20, 20);
            this.btnClassColor.TabIndex = 11;
            this.btnClassColor.UseVisualStyleBackColor = true;
            this.btnClassColor.Click += new System.EventHandler(this.btnClassColor_Click);
            // 
            // btnCommentColor
            // 
            this.btnCommentColor.ImageIndex = 0;
            this.btnCommentColor.ImageList = this.imageList1;
            this.btnCommentColor.Location = new System.Drawing.Point(298, 45);
            this.btnCommentColor.Margin = new System.Windows.Forms.Padding(0);
            this.btnCommentColor.Name = "btnCommentColor";
            this.btnCommentColor.Size = new System.Drawing.Size(20, 20);
            this.btnCommentColor.TabIndex = 10;
            this.btnCommentColor.UseVisualStyleBackColor = true;
            this.btnCommentColor.Click += new System.EventHandler(this.btnCommentColor_Click);
            // 
            // btnCommandColor
            // 
            this.btnCommandColor.ImageIndex = 0;
            this.btnCommandColor.ImageList = this.imageList1;
            this.btnCommandColor.Location = new System.Drawing.Point(298, 21);
            this.btnCommandColor.Margin = new System.Windows.Forms.Padding(0);
            this.btnCommandColor.Name = "btnCommandColor";
            this.btnCommandColor.Size = new System.Drawing.Size(20, 20);
            this.btnCommandColor.TabIndex = 2;
            this.btnCommandColor.UseVisualStyleBackColor = true;
            this.btnCommandColor.Click += new System.EventHandler(this.btnCommandColor_Click);
            // 
            // txtGeneratedCode
            // 
            this.txtGeneratedCode.BackColor = System.Drawing.Color.White;
            this.txtGeneratedCode.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGeneratedCode.Location = new System.Drawing.Point(125, 118);
            this.txtGeneratedCode.Name = "txtGeneratedCode";
            this.txtGeneratedCode.ReadOnly = true;
            this.txtGeneratedCode.Size = new System.Drawing.Size(170, 18);
            this.txtGeneratedCode.TabIndex = 9;
            this.txtGeneratedCode.Text = "Your own example code";
            // 
            // txtExpression
            // 
            this.txtExpression.BackColor = System.Drawing.Color.White;
            this.txtExpression.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExpression.Location = new System.Drawing.Point(125, 94);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ReadOnly = true;
            this.txtExpression.Size = new System.Drawing.Size(170, 18);
            this.txtExpression.TabIndex = 8;
            this.txtExpression.Text = "text = \"Example\"";
            // 
            // txtClass
            // 
            this.txtClass.BackColor = System.Drawing.Color.White;
            this.txtClass.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClass.Location = new System.Drawing.Point(125, 70);
            this.txtClass.Name = "txtClass";
            this.txtClass.ReadOnly = true;
            this.txtClass.Size = new System.Drawing.Size(170, 18);
            this.txtClass.TabIndex = 7;
            this.txtClass.Text = "String";
            // 
            // txtComment
            // 
            this.txtComment.BackColor = System.Drawing.Color.White;
            this.txtComment.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtComment.Location = new System.Drawing.Point(125, 46);
            this.txtComment.Name = "txtComment";
            this.txtComment.ReadOnly = true;
            this.txtComment.Size = new System.Drawing.Size(170, 18);
            this.txtComment.TabIndex = 6;
            this.txtComment.Text = "-- Comment Example --";
            // 
            // txtCommand
            // 
            this.txtCommand.BackColor = System.Drawing.Color.White;
            this.txtCommand.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommand.Location = new System.Drawing.Point(125, 22);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ReadOnly = true;
            this.txtCommand.Size = new System.Drawing.Size(170, 18);
            this.txtCommand.TabIndex = 5;
            this.txtCommand.Text = "@Do";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Generated code/text:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Expressions:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Concept/Class names:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Comments: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Commands: ";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(292, 322);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(89, 27);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "&Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 322);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 27);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(102, 322);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 27);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // colorDialog1
            // 
            this.colorDialog1.AllowFullOpen = false;
            this.colorDialog1.SolidColorOnly = true;
            // 
            // Options
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(393, 361);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabOptions);
            this.Name = "Options";
            this.Text = "Options";
            this.tabOptions.ResumeLayout(false);
            this.tabEditor.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGeneratedTabstops)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTemplateTabstops)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabOptions;
        private System.Windows.Forms.TabPage tabEditor;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TextBox txtGeneratedCode;
        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.TextBox txtClass;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown txtGeneratedTabstops;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown txtTemplateTabstops;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCommandColor;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnGeneratedCodeColor;
        private System.Windows.Forms.Button btnExpressionColor;
        private System.Windows.Forms.Button btnClassColor;
        private System.Windows.Forms.Button btnCommentColor;
        private System.Windows.Forms.Button btnErrorColor;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.Label label10;
    }
}