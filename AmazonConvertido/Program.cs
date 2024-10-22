using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Threading;

class Program
{
    static string lastPrice = null;

    static async Task<string> GetAmazonPrice(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            // Establecer headers para la petición HTTP
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");

            try
            {
                // Hacer la solicitud
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string pageContents = await response.Content.ReadAsStringAsync();

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(pageContents);

                    // Verificar el selector y ajustarlo según sea necesario
                    var priceElements = doc.DocumentNode.SelectNodes("//span[contains(@class, 'a-price-whole')]"); // Ajusta el selector aquí
                    if (priceElements != null && priceElements.Count > 0)
                    {
                        string price = priceElements[0].InnerText.Trim();
                        lastPrice = price;  // Actualizar la variable global
                        return price;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el precio: " + ex.Message);
            }
        }
        return null;
    }

    static async Task Main(string[] args)
    {
        string amazonUrl = "https://www.amazon.com.mx/Acer-Laptop-N%C3%BAcleos-Graphics-Garantia/dp/B0D6C681Y9/?_encoding=UTF8&ref_=pd_hp_d_atf_unk";

        while (true)
        {
            string price = await GetAmazonPrice(amazonUrl);
            if (price != null)
            {
                Console.WriteLine("El precio actual en Amazon es: " + price);
            }
            else
            {
                Console.WriteLine("El precio actual en Amazon es: " + lastPrice);
            }

            // Esperar 3 segundos antes de volver a ejecutar
            Thread.Sleep(3000);
        }
    }
}
