using AppHelpers;
using LibShared;
using LibShared.Services;
using LibShared.ViewModels.Libraries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LibraryWinUI.Code.WebApi
{
    internal sealed class LibraryWebApi : WebApiConfig
    {
        internal async Task<IEnumerable<LibraryVM>> GetAllLibrariesAsync()
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


                using HttpResponseMessage response = await client.GetAsync("api/v2/libraries/all");
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

        internal async Task<LibraryRequestVM> GetLibrariesAsync(OrderBy orderBy, SortBy sortBy, int maxItemsPerPage = 20, int gotoPage = 1)
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

                using HttpResponseMessage response = await client.GetAsync($"api/v2/libraries/all/ordered?orderBy={orderBy}&sortBy={sortBy}&maxItemsPerPage={maxItemsPerPage}&gotoPage={gotoPage}");
                string httpResponseBody = "";
                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LibraryRequestVM>(httpResponseBody);
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryWebApi), exception: ex);
                return null;
            }
        }

        internal async Task<LibraryVM> CreateAsync(LibraryVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel), "Le modèle de vue ne peut pas être null.");
                }

                string json = JsonConvert.SerializeObject(viewModel);
                StringContent httpContent = new (json, Encoding.Default, "application/json");

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

                HttpResponseMessage response = await client.PostAsync("api/v2/libraries/create/view-model", httpContent);
                string httpResponseBody = "";

                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LibraryVM>(httpResponseBody);
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryWebApi), exception: ex);
                return null;
            }
        }

        internal async Task<LibraryVM> UpdateAsync(LibraryVM viewModel)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel), "Le modèle de vue ne peut pas être null.");
                }

                string json = JsonConvert.SerializeObject(viewModel);
                StringContent httpContent = new(json, Encoding.Default, "application/json");

                HttpResponseMessage response = await HttpClient().PostAsync("api/v2/libraries/create/view-model", httpContent);
                string httpResponseBody = "";

                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LibraryVM>(httpResponseBody);
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryWebApi), exception: ex);
                return null;
            }
        }

        internal async Task<bool> UpdloadJaquette(long id, string filePath)
        {
            try
            {
                if (filePath == null || !File.Exists(filePath))
                {
                    throw new ArgumentNullException(nameof(filePath), "Le modèle de vue ne peut pas être null.");
                }

                using MultipartFormDataContent content = new MultipartFormDataContent();

                using FileStream fileStream = File.OpenRead(filePath);
                using HttpContent fileStreamContent = new StreamContent(fileStream);
                content.Add(new StringContent(id.ToString()), "Id");
                content.Add(new StringContent(Path.GetFileName(filePath)), "Name");
                fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "FormFile",
                    FileName = Path.GetFileName(filePath),
                };

                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileStreamContent, "FormFile", Path.GetFileName(filePath));



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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

                HttpResponseMessage response = await client.PostAsync("api/v2/libraries/upload/jaquette", content);
                string httpResponseBody = "";

                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<bool>(httpResponseBody);
                    return result;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryWebApi), exception: ex);
                return false;
            }
        }

    }
}
