using AppHelpers.Strings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHelpers.Serialization
{
    public static class JsonSerializationExtensions
    {
        /// <summary>
        /// Retourne une chaîne de caractère représentant l'objet convertit au format JSON
        /// </summary>
        /// <typeparam name="T">Type de l'objet</typeparam>
        /// <param name="self">L'objet à convertir</param>
        /// <returns></returns>
        public static string? GetJsonDataString<T>(this T self) where T : class
        {
            return JsonSerialization.SerializeObjectToString(self);
        }

        /// <summary>
        /// Retourne une chaîne de caractère représentant un tableau d'objets convertit au format JSON
        /// </summary>
        /// <typeparam name="T">Type de l'objet</typeparam>
        /// <param name="self">tableau d'objets à convertir</param>
        /// <returns></returns>
        public static string? GetJsonDataString<T>(this IEnumerable<T> self) where T : class
        {
            return JsonSerialization.SerializeObjectToString(self);
        }
    }

    public class JsonSerialization
    {
        private const string className = nameof(JsonSerialization);


        public static bool SerializeClassTofile<T>(T ObjectToSerialize, string FilePath) where T : class
        {
            try
            {
                if (FilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Logs.Log(className, nameof(SerializeClassTofile), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return false;
                }

                FileInfo fileInfo = new (FilePath);
                if (!fileInfo.Exists)
                {
                    Logs.Log(className, nameof(SerializeClassTofile), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return false;
                }
                else if (fileInfo.IsReadOnly)
                {
                    Logs.Log(className, nameof(SerializeClassTofile), "Le fichier spécifié existe mais il est en lecture seule.");
                    return false;
                }

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(fileInfo.FullName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, ObjectToSerialize);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeClassTofile), ex);
                return false;
            }
        }

        public static bool SerializeClassTofile<T>(IEnumerable<T> ObjectsToSerialize, string FilePath) where T : class
        {
            try
            {
                if (StringHelpers.IsStringNullOrEmptyOrWhiteSpace(FilePath))
                {
                    Logs.Log(className, nameof(SerializeClassTofile), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return false;
                }

                FileInfo fileInfo = new FileInfo(FilePath);
                if (!fileInfo.Exists)
                {
                    Logs.Log(className, nameof(SerializeClassTofile), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return false;
                }
                else if (fileInfo.IsReadOnly)
                {
                    Logs.Log(className, nameof(SerializeClassTofile), "Le fichier spécifié existe mais il est en lecture seule.");
                    return false;
                }

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(fileInfo.FullName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, ObjectsToSerialize);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeClassTofile), ex);
                return false;
            }
        }


        public static Task<bool> SerializeClassToFileAsync<T>(IEnumerable<T> ObjectsToSerialize, string FilePath) where T : class
        {
            try
            {
                if (FilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Logs.Log(className, nameof(SerializeClassToFileAsync), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return Task.FromResult(false);
                }

                FileInfo fileInfo = new (FilePath);
                if (fileInfo.Exists && fileInfo.IsReadOnly)
                {
                    Console.WriteLine("Le fichier spécifié existe mais il est en lecture seule.");
                    return Task.FromResult(false);
                }

                // serialize JSON directly to a file
                using StreamWriter file = File.CreateText(fileInfo.FullName);
                JsonSerializer serializer = new ();
                serializer.Serialize(file, ObjectsToSerialize);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeClassToFileAsync), ex);
                return Task.FromResult(false);
            }
        }

        public static Task<bool> SerializeClassToFileAsync<T>(T ObjectToSerialize, string FilePath) where T : class
        {
            try
            {
                if (FilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Logs.Log(className, nameof(SerializeClassToFileAsync), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return Task.FromResult(false);
                }

                FileInfo fileInfo = new (FilePath);
                if (!fileInfo.Exists)
                {
                    Logs.Log(className, nameof(SerializeClassToFileAsync), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return Task.FromResult(false);
                }
                else if (fileInfo.IsReadOnly)
                {
                    Logs.Log(className, nameof(SerializeClassToFileAsync), "Le fichier spécifié existe mais il est en lecture seule.");
                    return Task.FromResult(false);
                }

                // serialize JSON directly to a file
                using StreamWriter file = File.CreateText(fileInfo.FullName);
                JsonSerializer serializer = new ();
                serializer.Serialize(file, ObjectToSerialize);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeClassToFileAsync), ex);
                return Task.FromResult(false);
            }
        }


        public static string? SerializeObjectToString<T>(T ObjectToSerialize) where T : class
        {
            try
            {
                using StringWriter stringWriter = new ();
                JsonSerializer serializer = new();
                serializer.Serialize(stringWriter, ObjectToSerialize);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeObjectToString), ex);
                return null;
            }
        }

        public static string? SerializeObjectToString<T>(IEnumerable<T> ObjectsToSerialize) where T : class
        {
            try
            {
                using StringWriter stringWriter = new();
                JsonSerializer serializer = new();
                serializer.Serialize(stringWriter, ObjectsToSerialize);

                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeObjectToString), ex);
                return null;
            }
        }


        public static async Task<bool> SerializeDataStringToFile(string dataString, string configFileName)
        {
            try
            {
                if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return false;
                }

                if (configFileName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Logs.Log(className, nameof(SerializeDataStringToFile), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return false;
                }

                FileInfo fileInfo = new (configFileName);
                if (!fileInfo.Exists)
                {
                    Logs.Log(className, nameof(SerializeDataStringToFile), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return false;
                }
                else if (fileInfo.IsReadOnly)
                {
                    Logs.Log(className, nameof(SerializeDataStringToFile), "Le fichier spécifié existe mais il est en lecture seule.");
                    return false;
                }

                await File.WriteAllTextAsync(dataString, configFileName);
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeDataStringToFile), ex);
                return false;
            }
        }

        public static Task<T?> DeSerializeFileToClassAsync<T>(string FilePath) where T : class, new()
        {
            try
            {
                if (FilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Logs.Log(className, nameof(DeSerializeFileToClassAsync), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return Task.FromResult<T?>(null);
                }

                FileInfo fileInfo = new (FilePath);
                if (!fileInfo.Exists)
                {
                    Logs.Log(className, nameof(DeSerializeFileToClassAsync), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return Task.FromResult<T?>(null);
                }

                using StreamReader file = File.OpenText(FilePath);
                JsonSerializer serializer = new ();
                T? Tvalue = serializer.Deserialize(file, typeof(T)) as T;
                return Task.FromResult(Tvalue);
            }
            catch (Exception ex)
            {
                Logs.Log(className: className, exception:ex);
                return Task.FromResult<T?>(null);
            }
        }

        public static T? DeSerializeFileToClass<T>(string FilePath) where T : class, new()
        {
            try
            {
                if (FilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Console.WriteLine("Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return null;
                }

                if (!File.Exists(FilePath))
                {
                    Console.WriteLine("Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return null;
                }

                using StreamReader file = File.OpenText(FilePath);
                JsonSerializer serializer = new();
                T? Tvalue = serializer.Deserialize(file, typeof(T)) as T;
                return Tvalue;
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(DeSerializeFileToClass), ex);
                return null;
            }
        }

        public static Task<IEnumerable<T>> DeSerializeFileToIEnumerableClassAsync<T>(string FilePath) where T : class, new()
        {
            try
            {
                if (FilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Console.WriteLine("Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return Task.FromResult(Enumerable.Empty<T>());
                }

                FileInfo fileInfo = new (FilePath);
                if (!fileInfo.Exists)
                {
                    Console.WriteLine("Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                    return Task.FromResult(Enumerable.Empty<T>());
                }

                using (StreamReader file = File.OpenText(FilePath))
                {
                    JsonSerializer serializer = new ();
                    IEnumerable<T> Tvalue = serializer.Deserialize(file, typeof(IEnumerable<T>)) as IEnumerable<T> ?? Enumerable.Empty<T>();
                    return Task.FromResult(Tvalue);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(DeSerializeFileToIEnumerableClassAsync), ex);
                return Task.FromResult(Enumerable.Empty<T>());
            }
        }


        public static bool SerializeAsync(object value, string filePath)
        {
            try
            {
                if (filePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(filePath), "Le chemin d'accès au fichier n'est pas valide : il ne peut pas être vide ou ne contenir que des espaces blancs.");
                }

                // serialize JSON directly to a file
                using StreamWriter file = File.CreateText(filePath);
                JsonSerializer serializer = new();
                serializer.Serialize(file, value);
                file.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(SerializeClassToFileAsync), ex);
                return false;
            }
        }

        public static bool SerializeAsync(object value, FileInfo fileInfo)
        {
            try
            {
                if (fileInfo == null)
                {
                    throw new ArgumentNullException(nameof(fileInfo), "Le paramètre ne peut pas être null.");
                }

                if (!fileInfo.Exists)
                {
                    // serialize JSON directly to a file
                    using StreamWriter file = fileInfo.CreateText();
                    JsonSerializer serializer = new();
                    serializer.Serialize(file, value);
                    file.Close();
                }
                else
                {
                    if (fileInfo.IsReadOnly)
                    {
                        throw new Exception("Le fichier spécifié existe mais il est en lecture seule.");
                    }

                    // serialize JSON directly to a file
                    using FileStream file = fileInfo.OpenWrite();
                    using StreamWriter sw  = new (file);
                    JsonSerializer serializer = new();
                    serializer.Serialize(sw, value);
                    sw.Close();
                    file.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: className, exception:ex);
                return false;
            }
        }


        public static async Task<T?> DeserializeSingleFromPathAsync<T>(string configFilePath)
        {
            try
            {
                if (configFilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(configFilePath), "Le chemin d'accès au fichier n'est pas valide.");
                }

                string dataString = await File.ReadAllTextAsync(configFilePath);
                if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new Exception("Le fichier n'a retourné aucune données.");
                }

                var settings = new JsonSerializerSettings();
                return JsonConvert.DeserializeObject<T>(dataString, settings);
            }
            catch (Exception ex)
            {
                Logs.Log(className: className, exception: ex);
                return default;
            }
        }

        public static async Task<IEnumerable<T>> DeserializeManyFromPathAsync<T>(string configFilePath)
        {
            try
            {
                if (configFilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(configFilePath), "Le chemin d'accès au fichier n'est pas valide.");
                }

                string dataString = await File.ReadAllTextAsync(configFilePath);
                if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new Exception("Le fichier n'a retourné aucune données.");
                }

                var settings = new JsonSerializerSettings();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(dataString, settings) ?? Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Logs.Log(className: className, exception: ex);
                return Enumerable.Empty<T>();
            }
        }

        public static async Task<IEnumerable<T>> DeserializeAnyFromPathAsync<T>(string configFilePath) where T : class
        {
            try
            {
                if (configFilePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(configFilePath), "Le chemin d'accès au fichier n'est pas valide.");
                }

                JToken? jsonType = await GetJsonType(configFilePath);
                if (jsonType == null)
                {
                    throw new Exception("Le type JSON n'est pas valide.");
                }
                else if (jsonType is JObject)
                {
                    var result = await DeserializeSingleFromPathAsync<T>(configFilePath);
                    if (result == null)
                    {
                        throw new Exception("La désérialisation n'a retourné aucune données.");
                    }
                    return new T[] { result };
                }
                else if (jsonType is JArray)
                {
                    var result = await DeserializeManyFromPathAsync<T>(configFilePath);
                    if (result == null)
                    {
                        throw new Exception("La désérialisation n'a retourné aucune données.");
                    }
                    return result;
                }

                return Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Logs.Log(className: className, exception: ex);
                return Enumerable.Empty<T>();
            }
        }


        public static async Task<string?> GetDataStringAsync(string configFileName)
        {
            try
            {
                if (configFileName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                string dataString = await File.ReadAllTextAsync(configFileName);
                return dataString;
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(GetDataStringAsync), ex);
                return null;
            }
        }

        public static async Task<JToken?> GetJsonType(string configFileName)
        {
            try
            {
                if (configFileName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                string? dataString = await GetDataStringAsync(configFileName);
                if (dataString.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                return GetJsonTypeFromDataString(dataString);
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(GetJsonType), ex);
                return null;
            }
        }

        public static JToken? GetJsonTypeFromDataString(string jsonStringData)
        {
            try
            {
                if (jsonStringData.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }
                var token = JToken.Parse(jsonStringData);

                return token;
            }
            catch (Exception ex)
            {
                Logs.Log(className, nameof(GetJsonTypeFromDataString), ex);
                return null;
            }
        }

    }
}
