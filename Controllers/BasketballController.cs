using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BasketballInfo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace BasketballInfo.Controllers
{
    public class BasketballController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;


        public BasketballController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;

        }

        public async Task<List<Game>> GetNbaGames(DateTime? selectedDate)
        {
            try
            {
                var apiKey = _configuration["ApiSettings:BasketballApiKey"];
                var currentDate = selectedDate ?? DateTime.Now;
                var formattedDate = currentDate.ToString("yyyy-MM-dd");

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("Chave de API não encontrada. Verifique a configuração.");
                }

                _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);

                var apiUrl = $"https://free-nba.p.rapidapi.com/games?dates[]={formattedDate}";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);

                    // Use o JsonSerializer para desserializar a resposta JSON em objetos C#
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Para corresponder ao estilo do JSON
                    };

                    var responseData = JsonSerializer.Deserialize<Root>(content, options);

                    Console.WriteLine("Número de jogos obtidos: " + responseData?.data?.Count);

                    if (responseData != null && responseData.data != null)
                    {
                        // Atualiza estatísticas após obter os resultados dos jogos
                        return responseData.data;
                    }
                    else
                    {
                        // Se não houver dados válidos ou valores nulos, retorna uma lista vazia
                        Console.WriteLine("Os dados ou a lista de jogos estão nulos.");
                        return new List<Game>();
                    }
                }
                else
                {
                    // Registra os detalhes da exceção
                    Console.WriteLine($"Erro na requisição: {response.StatusCode}");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                    // Lançar uma exceção personalizada ou retornar uma lista vazia
                    throw new HttpRequestException($"Erro na requisição: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exceção capturada: " + ex);
                throw ex;
            }
        }

    }
}
