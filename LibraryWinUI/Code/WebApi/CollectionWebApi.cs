using AppHelpers;
using LibraryWinUI.ViewModels.Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.Code.WebApi
{
    internal class CollectionWebApi : WebApiConfig
    {
        internal async Task<IEnumerable<CollectionVM>> GetAllAsync()
        {
            try
            {
                using HttpResponseMessage response = await HttpClient().GetAsync("api/v2/collections/all");
                string httpResponseBody = "";
                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CollectionVM[]>(httpResponseBody);
                    return result;
                }

                return Enumerable.Empty<CollectionVM>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryWebApi), exception: ex);
                return Enumerable.Empty<CollectionVM>();
            }
        }
    }
}
