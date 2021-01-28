/****************************************************************************************
*	NextGen: The Next Sourcecode Generator using simple DSL's.							*
*	Copyright (C) Thierry Wiersma													    *
*****************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Generator.Utility;

namespace Generator
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }
        protected override void InitLayout()
        {
            base.InitLayout();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            InitializeForm();
        }
        private void InitializeForm()
        {

            OptionsSettings o = OptionsSettings.Instance();
            txtCommand.ForeColor = o.CommandColor;
            txtComment.ForeColor = o.CommentColor;
            txtClass.ForeColor = o.ClassColor;
            txtExpression.ForeColor = o.ExpressionColor;
            txtGeneratedCode.ForeColor = o.CodeColor;
            txtError.ForeColor = o.ErrorColor;

            txtTemplateTabstops.Value = o.Templatetabs;
            txtGeneratedTabstops.Value = o.Generatedtabs;

            btnApply.Enabled = false;
        }

        private void btnCommandColor_Click(object sender, EventArgs e)
        {
            OptionsSettings o = OptionsSettings.Instance();
            colorDialog1.Color = txtCommand.ForeColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK &&
                txtCommand.ForeColor != colorDialog1.Color)
            {
                txtCommand.ForeColor = colorDialog1.Color;
                btnApply.Enabled = true;
            }
        }

        private void btnCommentColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = txtComment.ForeColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK &&
                txtComment.ForeColor != colorDialog1.Color)
            {
                txtComment.ForeColor = colorDialog1.Color;
                btnApply.Enabled = true;
            }
        }

        private void btnClassColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = txtClass.ForeColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK &&
                txtClass.ForeColor != colorDialog1.Color)
            {
                txtClass.ForeColor = colorDialog1.Color;
                btnApply.Enabled = true;
            }
        }

        private void btnExpressionColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = txtExpression.ForeColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK &&
                txtExpression.ForeColor != colorDialog1.Color)
            {
                txtExpression.ForeColor = colorDialog1.Color;
                btnApply.Enabled = true;
            }
        }

        private void btnGeneratedCodeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = txtGeneratedCode.ForeColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK &&
                txtGeneratedCode.ForeColor != colorDialog1.Color)
            {
                txtGeneratedCode.ForeColor = colorDialog1.Color;
                btnApply.Enabled = true;
            }
        }
        private void btnErrorColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = txtError.ForeColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK &&
                txtError.ForeColor != colorDialog1.Color)
            {
                txtError.ForeColor = colorDialog1.Color;
                btnApply.Enabled = true;
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnApply_Click(sender, e);
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            OptionsSettings o = OptionsSettings.Instance();
            o.CommandColor = txtCommand.ForeColor;
            o.CommentColor = txtComment.ForeColor;
            o.ClassColor = txtClass.ForeColor;
            o.ExpressionColor = txtExpression.ForeColor;
            o.CodeColor = txtGeneratedCode.ForeColor;
            o.ErrorColor = txtError.ForeColor;

            o.Generatedtabs = Decimal.ToInt32(txtGeneratedTabstops.Value);
            o.Templatetabs = Decimal.ToInt32(txtTemplateTabstops.Value);
            o.SaveSettings();
            btnApply.Enabled = false;
        }

        private void txtTemplateTabstops_ValueChanged(object sender, EventArgs e)
        {
            btnApply.Enabled = true;
        }

        private void txtGeneratedTabstops_ValueChanged(object sender, EventArgs e)
        {
            btnApply.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}