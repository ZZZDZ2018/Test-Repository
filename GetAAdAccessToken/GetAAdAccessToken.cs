using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

namespace GetAADaccesstoken
{
    public class GetAAdAccesstoken
    {
      
        //
        // The App ID is used by the application to uniquely identify itself to Azure AD.
        // The Cert Name is the subject name of the certificate used to authenticate this application to Azure AD.
        // The Tenant is the name of the Azure AD tenant in which this application is registered.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        //
        private static string aadInstance = "https://login.microsoftonline.com/{0}";
        private static string tenant = "microsoft.onmicrosoft.com";
        private static string AppId = "ded8589a-b1f5-47e1-8a35-f20ef74b3bc2";
        private static string certName = "C:\\v-zhzhu\\Porting2Linux\\CloudTestDailyRun.pfx";
        private static string certpassword = "2018Cert!!!";

        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        //
        // To authenticate to the To Do list service, the client needs to know the service's App ID URI
        //
        private static string ServiceAppIdURL = "https://microsoft.onmicrosoft.com/dc1b9302-3f14-415a-8ac2-136d2834978a";

        private static HttpClient httpClient = new HttpClient();
        private static AuthenticationContext authContext = null;
        private static ClientAssertionCertificate certCred = null;


        public static async Task<string> GetStrToken()
        {
            // Create the authentication context to be used to acquire tokens.
            authContext = new AuthenticationContext(authority);

            // Initialize the Certificate Credential to be used by ADAL.
            X509Certificate2 cert = ReadCertificateFromFile();
            if (cert == null)
            {
                throw new InvalidOperationException("Failed to obtain the Cert");
            }

            // Then create the certificate credential client assertion.
            certCred = new ClientAssertionCertificate(AppId, cert);

            AuthenticationResult result = await GetAccessToken(ServiceAppIdURL);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
          
            return result.AccessToken;
        }



        /// <summary>
        /// Reads the certificate
        /// </summary>
        private static X509Certificate2 ReadCertificateFromFile()
        {
            X509Certificate2 cert = new X509Certificate2();
            cert.Import(certName, certpassword, X509KeyStorageFlags.DefaultKeySet);
            return cert;
        }


        /// <summary>
        /// Get an access token from Azure AD using client credentials.
        /// If the attempt to get a token fails because the server is unavailable, retry twice after 3 seconds each
        /// </summary>
        private static async Task<AuthenticationResult> GetAccessToken(string ServiceAppIdURL)
        {
            //
            // Get an access token from Azure AD using client credentials.
            // If the attempt to get a token fails because the server is unavailable, retry twice after 3 seconds each.
            //
            AuthenticationResult result = null;
            int retryCount = 0;
            bool retry = false;

            do
            {
                retry = false;

                try
                {
                    // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
                    result = await authContext.AcquireTokenAsync(ServiceAppIdURL, certCred);
                }
                catch (AdalException ex)
                {
                    if (ex.ErrorCode == "temporarily_unavailable")
                    {
                        retry = true;
                        retryCount++;
                    }
                    throw new InvalidOperationException("An error occurred while acquiring a token");

                }

            } while ((retry == true) && (retryCount < 3));
            return result;
        }
    }
}
