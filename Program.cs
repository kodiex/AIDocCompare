using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Replace with the actual file path and Azure Function URL
            string filePath = "C:\\Users\\kodie\\OneDrive\\Desktop\\ordini morganti test\\SCANSIONE_20250613_162800.pdf";
            string functionUrl = "http://localhost:7071/api/ComparePDF";

            try
            {
                // Read file content
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Convert file content to Base64
                string base64File = Convert.ToBase64String(fileBytes);

                Console.WriteLine("Sending request to Azure Function...");

                using (HttpClient client = new HttpClient())
                {
                    // Prepare the request content with 'file' parameter
                    var jsonPayload = $"{{\"file\": \"{base64File}\"}}";
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Send POST request to Azure Function
                    HttpResponseMessage response = await client.PostAsync(functionUrl, content);
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                       
                        
                    }
                    else
                    {
                        Console.WriteLine("Failed to invoke Azure Function. Status Code: " + response.StatusCode);
                    }
                    Console.WriteLine("Response from Azure Function: " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}