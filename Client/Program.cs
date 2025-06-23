using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AzureFunctionClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Replace with the actual file path and Azure Function URL
            string filePath = "C:\\Users\\kodie\\OneDrive\\Desktop\\ordini morganti test\\SCANSIONE_20250613_162800.pdf";
            //string functionUrl = "http://localhost:7071/api/AIFile";
            string functionUrl = "https://aifile.azurewebsites.net/api/AIFile";
            try
            {
                // Read file content
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Convert file content to Base64
                string base64File = Convert.ToBase64String(fileBytes);

                Console.WriteLine("Sending request to Azure Function...");

                using (HttpClient client = new HttpClient())
                {
                    // Add 'question' query parameter to the function URL
                    string question = "Extract the following information and return as JSON: item, quantity, price, net value and delivery date";
                    string sysPrompt = "You are a helpful assistant that extracts information from documents. " +
                                       "Please provide your response in valid JSON format with the following structure:"+
                                       " {\"items\": [{\"item\": string, \"quantity\": number, \"price\": number, \"netValue\": number, \"deliveryDate\": string}]}";
                    functionUrl += "?sysprompt=" + System.Web.HttpUtility.UrlEncode(sysPrompt);
                    functionUrl += "&question=" + System.Web.HttpUtility.UrlEncode(question);

                    // Prepare the request content with 'file' parameter
                    var jsonPayload = $"{{\"file\": \"{base64File}\"}}";
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Send POST request to Azure Function
                    HttpResponseMessage response = await client.PostAsync(functionUrl, content);
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        ItemsContainer itemsContainer = DeserializeJson(result);
                        // Process the deserialized itemsContainer as needed
                        Console.WriteLine("Successfully invoked Azure Function.");
                        foreach (var item in itemsContainer.Items)
                        {
                            Console.WriteLine($"Item: {item.ItemName}, Quantity: {item.Quantity}, Price: {item.Price}, Net Value: {item.NetValue}, Delivery Date: {item.DeliveryDate}");
                        }
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

        public class Item
        {
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public double NetValue { get; set; }
            public DateTime DeliveryDate { get; set; }
        }

        public class ItemsContainer
        {
            public List<Item> Items { get; set; }
        }

        static ItemsContainer DeserializeJson(string json)
        {
            //case insensitive deserialization
            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return System.Text.Json.JsonSerializer.Deserialize<ItemsContainer>(json, options);
        }
    }
}