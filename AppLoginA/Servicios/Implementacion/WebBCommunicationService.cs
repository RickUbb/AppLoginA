using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AppLoginA.Servicios.Implementacion
{
    public class WebBCommunicationService
    {
        private readonly HttpClient _httpClient;

        public WebBCommunicationService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetInfoFromWebB(string tokenJWT)
        {
            // Verifica que se haya proporcionado un token JWT
            if (string.IsNullOrWhiteSpace(tokenJWT))
            {
                throw new ArgumentException("Token JWT no válido", nameof(tokenJWT));
            }

            try
            {
                // Agregar el token JWT a la cabecera Authorization
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenJWT);

                // Realizar la solicitud HTTP a la Aplicación Web B
                var response = await _httpClient.GetAsync("https://localhost:7232/api/endpoint");

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer y devolver el contenido de la respuesta
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Si la solicitud no fue exitosa, lanzar una excepción o manejar el error según sea necesario
                    throw new HttpRequestException($"La solicitud HTTP falló con el código de estado: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la comunicación HTTP
                throw new HttpRequestException("Error al enviar la solicitud HTTP a la Aplicación Web B", ex);
            }
        }
    }
}
