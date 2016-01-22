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
        private static X509Certificate _sslCertificate;

        public static string CertificateThumbprint
        {
            get
            {
                return "989E9DB09FA84FEFE9A912E8A29C4D2A9F8A271A";
            }
        }

        public static string HostName
        {
            get
            {
                return "localhost";
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
