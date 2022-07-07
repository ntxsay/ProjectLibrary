using AppHelpers;
using LibShared;
using LibShared.ViewModels.Books;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.Code.WebApi
{
    internal class BookWebApi : WebApiConfig
    {
        internal async Task<BookRequestVM> GetBooksAsync(long idLibrary, OrderBy orderBy, SortBy sortBy, int maxItemsPerPage = 20, int gotoPage = 1)
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

                using HttpClient client = new(handler);
                client.BaseAddress = new Uri(baseAPIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using HttpResponseMessage response = await client.GetAsync($"api/v2/books/all/ordered?idLibrary={idLibrary}&orderBy={orderBy}&sortBy={sortBy}&maxItemsPerPage={maxItemsPerPage}&gotoPage={gotoPage}");
                string httpResponseBody = "";
                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<BookRequestVM>(httpResponseBody);
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(BookWebApi), exception: ex);
                return null;
            }
        }

    }
}
