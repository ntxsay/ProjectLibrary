using AppHelpers;
using LibShared.ViewModels.Libraries;
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
    internal sealed class LibraryWebApi : WebApiConfig
    {
        internal async Task<IEnumerable<LibraryVM>> GetAllLibrariesAsync()
        {
            try
            {
                HttpResponseMessage response = await HttpClient().GetAsync("api/v2/libraries/all");
                string httpResponseBody = "";
                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LibraryVM[]>(httpResponseBody);
                    return result;
                }

                return Enumerable.Empty<LibraryVM>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryWebApi), exception: ex);
                return Enumerable.Empty<LibraryVM>();
            }
        }

    }
}
