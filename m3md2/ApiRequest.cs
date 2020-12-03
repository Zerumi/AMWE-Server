// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace m3md2
{
    /// <summary>
    /// Предоставляет систему обращения к AMWE API
    /// </summary>
    public static class ApiRequest
    {
        /// <summary>
        /// Адрес API в сети Интернет
        /// </summary>
        public static string BaseAddress { get; set; }
        /// <summary>
        /// Отправляет POST запрос на API
        /// </summary>
        /// <typeparam name="T">Класс, объект которого отправляется</typeparam>
        /// <param name="product">Экземпляр клаасса T для отправки</param>
        /// <param name="apilist">Путь в формате BaseAddress/{apilist}</param>
        /// <returns>Асинхронную задачу с Uri этой операции</returns>
        public static async Task<Uri> CreateProductAsync<T>(T product, string apilist)
        {
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(BaseAddress)
                };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                string json = JsonConvert.SerializeObject(product);
                response = await client.PostAsync($"{apilist}", new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                OnRequestFailed?.Invoke(ex);
            }
            return response?.Headers?.Location;
        }
        /// <summary>
        /// Отправляет POST запрос на API и получает ответное значение
        /// </summary>
        /// <typeparam name="T">Класс, объект которого отправляется</typeparam>
        /// <typeparam name="T1">Класс, объект которого получается</typeparam>
        /// <param name="product">Экземпляр клаасса T для отправки</param>
        /// <param name="apilist">Путь в формате BaseAddress/{apilist}</param>
        /// <returns>Асинхронную задачу с возвращаемым объектом этой операции</returns>
        public static async Task<T1> CreateProductAsync<T, T1>(T product, string apilist)
        {
            T1 returnproduct = default;
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(BaseAddress)
                };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                string json = JsonConvert.SerializeObject(product);
                response = await client.PostAsync($"{apilist}", new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                returnproduct = await response.Content.ReadAsAsync<T1>();
            }
            catch (Exception ex)
            {
                OnRequestFailed?.Invoke(ex);
            }
            return returnproduct;
        }
        /// <summary>
        /// Отправляет GET запрос на API
        /// </summary>
        /// <typeparam name="T">Класс, объект которого отправляется</typeparam>
        /// <param name="path"></param>
        /// <returns>Асинхронную задачу с экземпляром класса T</returns>
        public static async Task<T> GetProductAsync<T>(string path)
        {
            T product = default;
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(BaseAddress)
                };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    product = await response.Content.ReadAsAsync<T>();
                }
                return product;
            }
            catch (Exception ex)
            {
                OnRequestFailed?.Invoke(ex);
            }
            return product;
        }

        /// <summary>
        /// Отправляет DELETE запрос на API по пути path без типизации
        /// </summary>
        /// <param name="path">Путь API</param>
        /// <returns>Асинхронную задачу со статусом операции</returns>
        public static async Task<HttpStatusCode> DeleteProductsAsync(string path)
        {
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(BaseAddress)
                };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                m3md2.StaticVariables.Diagnostics.ProgramInfo += $"{DateTime.Now.ToLongTimeString()}(ApiRequest) Отправляю DELETE запрос на {path}\r\n";
                response = await client.DeleteAsync(path);
                m3md2.StaticVariables.Diagnostics.ProgramInfo += $"{DateTime.Now.ToLongTimeString()}(ApiRequest) Ответ от API {(response.IsSuccessStatusCode ? "Обработано успешно" : $"Что-то пошло не так {await response.Content.ReadAsStringAsync()}")}\r\n";
            }
            catch (Exception ex)
            {
                OnRequestFailed?.Invoke(ex);
            }
            return response?.StatusCode ?? default;
        }
        
        public delegate void ApiExHandler(Exception ex);

        public static event ApiExHandler OnRequestFailed;
    }
}