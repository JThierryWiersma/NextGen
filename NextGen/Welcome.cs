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
    public partial class Welcome : Form
    {
        private bool isStartUp = true;
        public Welcome()
        {
            InitializeComponent();
        }
        public Welcome(bool isstartup) : this()
        {
            isStartUp = isstartup;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            RegistrationInfo ri = Registry.Instance().RegistrationInfo(); 
            txtRegisteredTo.Text = txtRegisteredTo2.Text = ri.Registeredto;
            txtOrganisation.Text = txtOrganisation2.Text = ri.Organisation;
            txtEmailAddress.Text = txtEmailAddress2.Text = ri.Emailaddress;
            txtRegnr.Text = ri.Regnr;

            btnBuy.Enabled = true;
            btnRegister.Enabled = true;
            btnContinue.Enabled = true;
            
            txtValidUntil.Visible = ri.Valid;
            if (ri.Valid)
            {
                txtValidUntil.Text = ri.Validuntil.ToShortDateString();
            }

            switch (ri.Regtype)
            {
                case RegistrationType.Fresh:
                    txtLicense.Text = "No license";
                    txtLicense.Visible = true;
                    txtMessage.Text = "Request a free 30-day trial license now!";
                    txtMessage.Visible = true;
                    this.AcceptButton = btnBuy;
                    break;

                case RegistrationType.Trial:
                    txtLicense.Text = "Trial license";
                    txtMessage.Text = "Order a professional license to skip this message";
                    txtMessage.Visible = true;
                    this.AcceptButton = btnBuy;
                    break;

                case RegistrationType.Professional:
                    txtLicense.Text = "Professional license";
                    if (!ri.Valid)
                    {
                        txtMessage.Text = "License is invalid. Please order a valid license.";
                        txtMessage.Visible = true;
                        this.AcceptButton = btnBuy;
                    }
                    else if (DateTime.Today < ri.Validuntil && DateTime.Today.AddDays(14) > ri.Validuntil)
                    {
                        txtMessage.Text = "License is about to end. Order a new license today!";
                        txtMessage.Visible = true;
                        this.AcceptButton = btnBuy;
                    }
                    else if (isStartUp)
                    {
                        tmrAutoClose.Start();
                        btnBuy.Enabled = false;
                        btnRegister.Enabled = false;
                        btnContinue.Enabled = false;
                    } 
                    break;
            }
            if (ri.Valid)
            {
                btnContinue.Text = "Continue";
            }
            else
            {
                btnContinue.Text = "Bye";
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            pnlRegistration.Height = 120;
            pnlRegistration.Dock = DockStyle.Bottom;
            grpRegistration.Height = 110;
            lblValidUntil2.Visible = false;
            txtValidUntil2.Visible = false;
            lblRegnr2.Visible = false;
            txtRegnr2.Visible = false;
            lblPasteRemark.Visible = false;
            lblRegistration.Visible = false;
            txtRegistrationcode.Visible = false;
            chkTrial.Enabled = (Registry.Instance().RegistrationInfo().Regtype == RegistrationType.Fresh);
            chkTrial.Checked = false;
            btnSendRegister.Text = "Send";
            this.AcceptButton = btnSendRegister;
            this.CancelButton = btnCancel2;

            pnlRegistration.Visible = true;
            pnlRegistrationInfo.Visible = false;
            txtRegisteredTo2.Focus();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            pnlRegistration.Height = 177;
            pnlRegistration.Dock = DockStyle.Bottom;
            grpRegistration.Height = 167;

            lblValidUntil2.Visible = false;
            txtValidUntil2.Visible = false;
            lblRegnr2.Visible = false;
            txtRegnr2.Visible = false;
            lblPasteRemark.Visible = true;
            lblRegistration.Visible = true;
            txtRegistrationcode.Visible = true;
            chkTrial.Visible = false;
            //chkTrial.Checked = false;
            btnSendRegister.Text = "Register";
            this.AcceptButton = btnSendRegister;
            this.CancelButton = btnCancel2;

            pnlRegistration.Visible = true;
            pnlRegistrationInfo.Visible = false;
        }

        private void btnTryOrBye_Click(object sender, EventArgs e)
        {
            Close();    
        }

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            pnlRegistration.Visible = false;
            pnlRegistrationInfo.Visible = true;
            this.AcceptButton = btnContinue;
            this.CancelButton = null;
        }

        private void btnSendRegister_Click(object sender, EventArgs e)
        {
            if (btnSendRegister.Text == "Register")
            {
                if (TryRegistration())
                    Close();
            }
            else
            {
                if (TrySending())
                    Close();
            }
        }
        private void Msg(string text)
        {
            MessageBox.Show(text, "Registration", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private bool TryRegistration()
        {
            if (txtRegisteredTo2.Text.Trim() == "")
            {
                Msg("'Register to' is mandatory");
                txtRegisteredTo2.Focus();
                return false;
            }
            if (txtOrganisation2.Text.Trim() == "")
            {
                Msg("'Organisation' is mandatory");
                txtOrganisation2.Focus();
                return false;
            }
            if (txtEmailAddress2.Text.Trim() == "")
            {
                Msg("'Email address' is mandatory");
                txtEmailAddress2.Focus();
                return false;
            }
            if (txtRegistrationcode.Text.Replace("\r\n", "").Trim() == "")
            {
                Msg("'Registration' is mandatory and has to contain the registation code received in an email (only paste the indicated part)");
                txtRegistrationcode.Focus();
                return false;
            }
            string registratie = txtRegistrationcode.Text;
            registratie = registratie.Trim('-', '\r', '\n', ' ');
            if (registratie.StartsWith("Cut Here"))
            {
                registratie = registratie.Substring("Cut Here".Length).TrimStart('-', '\r', '\n');
            }
            registratie = registratie.Replace("\r\n", "");

            RegistrationResult rr = Registry.Instance().TryRegistration(
                txtRegisteredTo2.Text.Trim(), 
                txtOrganisation2.Text.Trim(), 
                txtEmailAddress2.Text.Trim(), 
                registratie);
            if (rr == RegistrationResult.Ok)
            {
                RegistrationInfo ri = Registry.Instance().RegistrationInfo();
                lblValidUntil2.Visible = ri.Valid;
                txtValidUntil2.Visible = ri.Valid;
                txtValidUntil2.Text = ri.Validuntil.ToShortDateString();

                lblRegnr2.Visible = ri.Valid;
                txtRegnr2.Visible = ri.Valid;
                txtRegnr2.Text = ri.Regnr;
                if (!ri.Valid)
                {
                    Msg("License code was registered, but is not valid anymore");
                }
                return ri.Valid;
            }
            else
            {
                Msg("Something went wrong trying to register the license");
                return false;
            }
        }

        private bool TrySending()
        {
            Utility.AppStarter.BrowseTo("http://www.codemultiplier.com?xyz&abc");
            return true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utility.AppStarter.BrowseTo("http://www.codemultiplier.com");
        }

        private void tmrAutoClose_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            Close();    
        }

        private void txtRegisteredTo2_Enter(object sender, EventArgs e)
        {
            txtMessage2.Text = "Enter the name of the user to be registered";
        }

        private void txtOrganisation2_Enter(object sender, EventArgs e)
        {
            txtMessage2.Text = "";
        }

    }
}