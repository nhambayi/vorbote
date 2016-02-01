namespace Vorbote.Configuration
{
    using System.Net;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class SmtpServerConfiguration
    {
        private string _storageAccountKey;

        private static X509Certificate _sslCertificate;

        public static string CertificateThumbprint
        {
            get
            {
                return "2A8582079BBD5326B87D54CC443A850F7656AF03";
            }
        }

        public static string HostName
        {
            get
            {
                return "localhost";
            }
        }

        public static string QueueName
        {
            get
            {
                return "myqueue";
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

        public static string EndPointName
        {
            get
            {
                return "smtpSsl";
            }
        }

        public static int SmtpPort
        {
            get
            {
                return PublicEndPoint.Port;
            }
        }

        public static IPAddress PublicAddress
        {
            get
            {
                return PublicEndPoint.Address;
            }
        }

        public static IPEndPoint PublicEndPoint
        {
            get
            {
                var endPoint = CurrentRoleIntsance.InstanceEndpoints[EndPointName].IPEndpoint;
                return endPoint;
            }
        }

        public static RoleInstance CurrentRoleIntsance
        {
            get
            {
                var currentRole = RoleEnvironment.CurrentRoleInstance;
                return currentRole;
            }
        }

        public static string StorageAccountName
        {
            get
            {
                return "vorbotedevstore";
            }
        }

        public static string StorageAccountKey
        {
            get
            {
                return "gzv1viKgSoKy8cdpCwk3p7eRadSgnuMk9rashX2OCvqInMMEFrtUs2rLvHWK4Hi2hkhv5CeZWLBjKxGGI82oqw==";
            }
        }
        
        public static bool RequireSsl
        {
            get
            {
                return true;
            }
        }

        private static X509Certificate GetServerCertificate()
        {
            string thumbprint = CertificateThumbprint;
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            if (certificateCollection.Count == 0)
            {
                store.Close();
                string errorMessage = string.Format("Unable to load certificate with thumbprint {0}", thumbprint);
                throw new ApplicationException(errorMessage);
            }
            else
            {
                var certificate = new X509Certificate2(certificateCollection[0]);
                store.Close();
                return certificate;
            }
        }
    }
}
