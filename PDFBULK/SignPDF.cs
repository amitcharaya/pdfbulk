/*
    Use 'PDF BULK SIGN' to digitaly sign pdf documents 
    Copyright (C) <2019>  <Amit Kumar>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
	Contact Charayaamit@gmail.com

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;



using System.IO;

using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text;




namespace SIGNPDF
{
    public partial class SignPDF : Form
    {
        double increment=0;
        int progress;
        public SignPDF()
        {
            InitializeComponent();
            
        }
        public void sign(X509Certificate2 cert,String filename, String imageName, String output)
        {
            
                Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
                Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] {
                cp.ReadCertificate(cert.RawData)};

                IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA-1");

                PdfReader pdfReader = new PdfReader(filename);

                FileStream signedPdf = new FileStream(output, FileMode.Create);

                PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0');
                PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                Image img=  Image.GetInstance(imageName);
                img.Alignment = iTextSharp.text.Image.UNDERLYING;
                signatureAppearance.SignatureGraphic = img;
                
                signatureAppearance.Acro6Layers = true;    
                int x =Convert.ToInt32( xTextBox.Text);
                int y = Convert.ToInt32(yTextBox.Text);
                int w = Convert.ToInt32(widthTextBox.Text);
                int h = Convert.ToInt32(heightTextBox.Text);
                signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x,  y, w, h), pdfReader.NumberOfPages, "Signature");
                signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION ;
                    
                MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);
              
            
            }
           
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                string inputPath = inputText.Text;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                String image = path + "\\sign.jpg";
                string output = outputText.Text;
                string[] filePaths = Directory.GetFiles(inputPath, "*.pdf");
                X509Store store = new X509Store(StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection sel = X509Certificate2UI.SelectFromCollection(store.Certificates, null, null, X509SelectionFlag.SingleSelection);

                X509Certificate2 cert = sel[0];
            
                increment = filePaths.Length / 100.0;
                if (increment<1)
                {
                    progress = (int)(100/(increment * 100));

                }
                else
                {
                    progress =(int) increment;
                }



              
                for (int i = 0; i < filePaths.Length; i++)
                {
                    
                    
                    
                    string fileName = Path.GetFileName(filePaths[i]);
                    sign(cert, filePaths[i], image, output + "\\" + fileName);
                    if (i % progress == 0)
                    {
                        
                        progressBar1.Increment(1);
                        

                    }
                    label7.Text = i+1 + " of " + filePaths.Length +" Signed" ;
                    this.Refresh();
                    
                }
                progressBar1.Value = 100;
                
                MessageBox.Show("All Files Signed Successfully");
                progressBar1.Value = 0;
              
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if(dialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                inputText.Text = dialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputText.Text = dialog.SelectedPath;
            }
        }

        private void xTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void SignPDF_Load(object sender, EventArgs e)
        {

        }

        

        
    }
}
