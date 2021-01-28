using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace Generator.Utility
{
    public class Registry
    {
        private static Registry m_instance = null;
        public static Registry Instance()
        {
            if (m_instance == null)
                m_instance = new Registry();
            return m_instance;
        }
        protected Registry()
        {
        }
        /// <summary>
        /// Geef een true door als het lokale verhaspeling betreft.
        /// Bij false kunnen beide zijden dezelfde gebruiken.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private RijndaelManaged GeefRM(bool a)
        {
            byte[] y = { 160, 73, 40, 39, 201, 155, 19, 214, 46, 220, 83, 202, 7, 10, 19, 63 };
            string x;
            if (a)
            {
                string d = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                DateTime dt = System.IO.Directory.GetCreationTime(d);
                x = dt.ToString(new String('s', 2) + new String('M', 2) + new String('y', 4) + new String('s', 2) + new String('H', 2) + new String('s', 2) + new String('m', 2));
            }
            else
            {
                x = Encoding.ASCII.GetString(y);
            }
            Rfc2898DeriveBytes r = new Rfc2898DeriveBytes(x, y);
            RijndaelManaged rd = new RijndaelManaged();
            rd.Key = r.GetBytes(rd.KeySize / 8);
            rd.IV = r.GetBytes(rd.BlockSize / 8);
            return rd;
        }

        private string Verhaspel(string bron, bool a)
        {
            RijndaelManaged rd = GeefRM(a);
            ICryptoTransform i = rd.CreateEncryptor();
            System.IO.MemoryStream m = new System.IO.MemoryStream();
            byte[] bronbytes = Encoding.Unicode.GetBytes(bron);
            CryptoStream cs = new CryptoStream(m, i, CryptoStreamMode.Write);
            cs.Write(bronbytes, 0, bronbytes.Length);
            cs.FlushFinalBlock();
            m.Position = 0;
            byte[] haspelbytes = m.ToArray();
            m.Close();
            cs.Close();
            string verhaspeld = System.Convert.ToBase64String(haspelbytes);
            int lengte = verhaspeld.Length;
            return verhaspeld;
        }

        private string Onthaspel(string bron, bool a)
        {
            RijndaelManaged rd = GeefRM(a);
            ICryptoTransform i = rd.CreateDecryptor();
            byte[] to_onthaspel = System.Convert.FromBase64String(bron);
            System.IO.MemoryStream m = new System.IO.MemoryStream(to_onthaspel);
            CryptoStream cs = new CryptoStream(m, i, CryptoStreamMode.Read);

            byte[] haspelbytes = new byte[to_onthaspel.Length];
            int length = cs.Read(haspelbytes, 0, to_onthaspel.Length);
            cs.Close();
            m.Close();

            string onthaspeld = Encoding.Unicode.GetString(haspelbytes, 0, length);
            return onthaspeld;
        }
        private string PrepareHash(string tohash)
        {
            // remove all non-alfa-digits
            string result = "";
            foreach (char c in tohash)
            {
                if (Char.IsLetterOrDigit(c) ||
                    c == '@' || c == '.')
                    result += c;
            }
            return result.ToLowerInvariant();
        }
        private byte[] GetHash(string tohash)
        {
            byte[] key = { 214, 46, 220, 83, 160, 73, 40, 39, 201, 155, 19, 202, 7, 10, 19, 63 };
            HMACSHA1 hash = new HMACSHA1(key);
            byte[] tohashbytes = Encoding.Unicode.GetBytes(tohash);
            byte[] result = hash.ComputeHash(tohashbytes);
            return result;
        }

        private RegistrationInfo ri;
        private void BuildRegistrationInfo()
        {
            try
            {
                RegistryKey masterkey = System.Windows.Forms.Application.UserAppDataRegistry;
                // get registration
                string emailaddress = (string)masterkey.GetValue("Email", String.Empty, RegistryValueOptions.None);
                string registeredto = (string)masterkey.GetValue("Name", String.Empty, RegistryValueOptions.None);
                string organisation = (string)masterkey.GetValue("Organisation", String.Empty, RegistryValueOptions.None);
                string regcode1 = (string)masterkey.GetValue("Registration", String.Empty, RegistryValueOptions.None);
                bool valid = false;
                RegistrationType rt = RegistrationType.Fresh;
                String regnr = "";
                DateTime validuntil = DateTime.Today.AddDays(-1.0);

                if (regcode1 == String.Empty)
                {
                    string d = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                    DateTime dt = System.IO.Directory.GetCreationTime(d);
                    DateTime dt2 = new DateTime(dt.Year, dt.Month, dt.Day);
                    valid = (dt2.AddDays(2) >= DateTime.Today);
                    ri = new RegistrationInfo("?", "?", "?", "?", dt2.AddDays(2), RegistrationType.Fresh, valid);
                    return;
                }

                //*** begin DFZ Hack
                /*
                if (false)
                {
                    string regcode2 = Onthaspel(regcode1, true);

                    // nieuwe hash uitrekenen
                    string tohash = PrepareHash(registeredto) + '\n'
                            + PrepareHash(organisation) + '\n'
                            + PrepareHash(emailaddress);
                    byte[] hash = GetHash(tohash);
                    string newhash = System.Convert.ToBase64String(hash);
                    string[] parts = regcode2.Split(';');
                    string hashcheck = (parts.Length > 0) ? parts[0] : "?";
                    validuntil = (parts.Length > 1 && parts[1].Length == 8)
                        ? new DateTime(Int32.Parse(parts[1].Substring(0, 4)),
                                        Int32.Parse(parts[1].Substring(4, 2)),
                                        Int32.Parse(parts[1].Substring(6, 2)))
                        : DateTime.Today.AddDays(-1);
                    regnr = (parts.Length > 2) ? parts[2] : "?";
                    rt = RegistrationType.Fresh;
                    if (parts.Length > 3)
                    {
                        if (parts[3] == "1")
                            rt = RegistrationType.Trial;
                        else
                            rt = RegistrationType.Professional;
                    }
                    valid = (newhash == hashcheck && DateTime.Today <= validuntil);
                }
                
                 * */
                rt = RegistrationType.Professional;
                validuntil = new DateTime(2009, 11, 1);
                regnr = "1";
                valid = DateTime.Today <= validuntil;

                //*** end DFZ hack

                ri = new RegistrationInfo(registeredto, organisation, emailaddress,
                    regnr, validuntil, rt, valid);
            }
            catch (CryptographicException )
            {
                ri = new RegistrationInfo("?", "?", "?", "?", DateTime.Today.AddDays(-1), RegistrationType.Fresh, false);
            }
            catch (ApplicationException )
            {
                ri = new RegistrationInfo("?", "?", "?", "?", DateTime.Today.AddDays(-1), RegistrationType.Fresh, false);
            }
        }
        public RegistrationInfo RegistrationInfo()
        {
            if (ri == null)
                BuildRegistrationInfo();
            return ri;
        }
        public RegistrationResult TryRegistration(string registerto, string organisation, string email, string registrationcode)
        {
            try
            {
                // Onthaspelen van de registrationcode
                string decryptedmessage = Onthaspel(registrationcode, false);
                string[] parts = decryptedmessage.Split(';');
                if (parts.Length != 4)
                {
                    return RegistrationResult.CouldNotUnpack;
                }
                string hashreg = parts[0];
                DateTime validuntil = (parts[1].Length == 8)
                    ? new DateTime(Int32.Parse(parts[1].Substring(0, 4)),
                                    Int32.Parse(parts[1].Substring(4, 2)),
                                    Int32.Parse(parts[1].Substring(6, 2)))
                    : DateTime.Today.AddDays(-1);
                string regnr = parts[2];
                RegistrationType rt;
                if (parts[3] == "0")
                    rt = RegistrationType.Professional;
                else
                    rt = RegistrationType.Trial;

                // Check hashvalue
                string tohash = PrepareHash(registerto) + '\n'
                    + PrepareHash(organisation) + '\n'
                    + PrepareHash(email);
                byte[] hash = GetHash(tohash);
                string newhash = System.Convert.ToBase64String(hash);

                if (newhash != hashreg)
                {
                    return RegistrationResult.InvalidHash;
                }

                // We slaan het ontvangen bericht op met Rijndael gecodeerd.
                string regcodecode = Verhaspel(decryptedmessage, true);

                RegistryKey masterkey = System.Windows.Forms.Application.UserAppDataRegistry;
                if (masterkey == null)
                {
                    throw new ApplicationException("Unable to open the registry");
                }

                masterkey.SetValue("Email", email, RegistryValueKind.String);
                masterkey.SetValue("Name", registerto, RegistryValueKind.String);
                masterkey.SetValue("Organisation", organisation, RegistryValueKind.String);
                masterkey.SetValue("Registration", regcodecode, RegistryValueKind.String);
                masterkey.Flush();

                if (validuntil < DateTime.Today)
                    return RegistrationResult.NotValidAnymore;

                ri = new RegistrationInfo(registerto, organisation, email, regnr, validuntil, rt, true);
                return RegistrationResult.Ok;
            }
            catch (Exception)
            {
                return RegistrationResult.ExceptionOccured;
            }
        }
    }
    public enum RegistrationResult
    {
        CouldNotUnpack,
        InvalidHash,
        ExceptionOccured,
        NotValidAnymore, 
        Ok
    }
    public class RegistrationInfo
    {
        private string registeredto;
        private string organisation;
        private string emailaddress;
        private string regnr;
        private DateTime validuntil;
        private RegistrationType regtype;
        private bool valid;

        internal RegistrationInfo(
            string rt,
            string or,
            string em, 
            string rn,
            DateTime vu,
            RegistrationType ry,
            bool va)
        {
            registeredto = rt;
            organisation = or;
            emailaddress = em;
            regnr = rn;
            validuntil = vu;
            regtype = ry;
            valid = va;
        }

        public DateTime Validuntil
        {
            get { return validuntil; }
        }
        public string Emailaddress
        {
            get { return emailaddress; }
        }
        public string Registeredto
        {
            get { return registeredto; }
        }
        public RegistrationType Regtype
        {
            get { return regtype; }
        }
        public bool Valid
        {
            get { return valid; }
        }
        public string Organisation
        {
            get { return organisation; }
        }
        public string Regnr
        {
            get { return regnr; }
        }
    }

    public enum RegistrationType
    {
        Trial,
        Fresh,
        Professional,
    }
}
