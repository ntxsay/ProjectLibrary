using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.Code.WebApi
{
    internal class WebApiConfig
    {
        protected string baseAPIUrl = "https://localhost:5001/";

        protected HttpClient HttpClient()
        {
            try
            {
                //Désactive la validation du certificat SSL auto-signé
                using HttpClientHandler handler = new()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    }
                };

                using var client = new HttpClient(handler);
                client.BaseAddress = new Uri(baseAPIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return client;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
