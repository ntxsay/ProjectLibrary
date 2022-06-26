using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LibUwp.Code.Api
{
    internal class LibraryApi : ApiConfig
    {
        internal async Task GetWeatherAsync()
        {
            try
            {
                #region En Developpement uniquement
                //Désactive la validation du certificat SSL auto-signé
                using (HttpClientHandler handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback =
                            (httpRequestMessage, cert, cetChain, policyErrors) =>
                            {
                                return true;
                            }
                })
                {
                    using (var client = new HttpClient(handler))
                    {
                        client.BaseAddress = new Uri(baseAPIUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("api/v2/libraries/all");
                        string httpResponseBody = "";
                        if (response.IsSuccessStatusCode)
                        {
                            httpResponseBody = await response.Content.ReadAsStringAsync();
                            //var result = JsonConvert.DeserializeObject<List<Account>>(httpResponseBody);
                            return;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
