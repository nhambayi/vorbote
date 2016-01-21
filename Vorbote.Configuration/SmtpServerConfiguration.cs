using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote.Configuration
{
    public class SmtpServerConfiguration
    {

        private static X509Certificate _sslCertificate;

        public void Init()
        {
            
        }

        public static string CertificateThumbprint
        {
            get
            {
                return "989E9DB09FA84FEFE9A912E8A29C4D2A9F8A271A";
            }
        }

        public static X509Certificate SslCertificate
        {
            get
            {
                if(_sslCertificate == null)
                {
                    _sslCertificate = GetServerCertificate();
                }

                return _sslCertificate;
            }
        }

        private static X509Certificate GetServerCertificate()
        {
            string thumbprint = CertificateThumbprint;
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            if (certificateCollection.Count > 0)
            {
                var certificate = new X509Certificate2(certificateCollection[0]);
                return certificate;
            }
            else
            {
                string errorMessage = string.Format("Unable to load certificate with thumbprint {0}", thumbprint);
                throw new ApplicationException(errorMessage);
            }
        }
    }

    
}
