using AppHelpers;
using LibShared;
using LibShared.ViewModels.Books;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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

                HttpResponseMessage response = await client.PostAsync("api/v2/books/upload/jaquette", content);
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
                Logs.Log(nameof(BookWebApi), exception: ex);
                return false;
            }
        }

        internal async Task<BitmapImage> GetJaquetteBitmap(long id)
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

                using HttpResponseMessage response = await client.GetAsync($"api/v2/books/jaquette?idBook={id}");
                string httpResponseBody = "";
                if (response.IsSuccessStatusCode)
                {
                    httpResponseBody = await response.Content.ReadAsStringAsync();
                    byte[] result = JsonConvert.DeserializeObject<byte[]>(httpResponseBody);

                    if (result == null || result.Length == 0)
                    {
                        throw new Exception("Le tableau de bytes ne peut pas être null ou vide.");
                    }

                    BitmapImage image = new();

                    using MemoryStream memoryStream = new(result);
                    using (IRandomAccessStream stream = memoryStream.AsRandomAccessStream())
                    {
                        await image.SetSourceAsync(stream);
                    }
                    return image;
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
