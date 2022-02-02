using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SimpleHttpFunction
{
    
    public class Function : IHttpFunction
    {

        //Objeto Retorna JSON resultado
        public class coord
        {
            public double x { get; set; }
            public double y { get; set; }
        }

        public class result 
        {
            public coord position { get; set; }
            public string message { get; set; }
        }
        //END / Objeto Retorna JSON resultado

        //Objeto para Leer la Informacion del JSON
        public class satellite
        { 
            public string name { get; set; }
            public double distance { get; set; } 
            public string[] message { get; set; }
        } 
        public class info{
            public satellite[] satellites { get; set; }
        }
        //END / Objeto para Leer la Informacion del JSON
        private readonly ILogger _logger;

        public Function(ILogger<Function> logger) =>
            _logger = logger;

        public coord GetLocation(double d1, double d2, double d3 ) {
            //Trilaterate
            coord result = new coord();
            double x1 = -500;
            double x2 = 100;
            double x3 = 500;
            double y1 = -200;
            double y2 = -100;
            double y3 = 100;            
            result.x = (((((d1*d1) - (d2*d2)) + ((x2*x2) - (x1 * x1)) + ((y2*y2) - (y1*y1))) * (2 * y3 - 2 * y2) - (((d2*d2) - (d3*d3)) + ((x3*x3) - (x2*x2)) + ((y3*y3) - (y2*y2))) * (2 * y2 - 2 * y1)) / ((2 * x2 - 2 * x3) * (2 * y2 - 2 * y1) - (2 * x1 - 2 * x2) * (2 * y3 - 2 * y2)));
            result.y = (((d1*d1) - (d2*d2)) + ((x2*x2) - (x1*x1)) + ((y2*y2) - (y1*y1)) + result.x * (2 * x1 - 2 * x2)) / (2 * y2 - 2 * y1);
            return result;
        }

        public string GetMessage(string[] messages1, string[] messages2, string[] messages3)
        {
            string msg = "";
            int cant_acep = 0;
            for (int i = 0; i < 5; i++)
            {
                if (messages1[i] != ""){
                    msg += messages1[i] + " ";
                    cant_acep++;
                }else{ 
                    if (messages2[i] != ""){
                        msg += messages2[i] + " ";
                        cant_acep++;
                    }else{ 
                        if (messages3[i] != ""){
                            msg += messages3[i] + " ";
                            cant_acep++;
                        }else{
                            msg += "";
                        }
                    }
                }
            }

            if (cant_acep < 3)
                msg = "";

            return msg;
        }

        public async Task HandleAsync(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            string message = request.Query["message"];
            string jsonString = "";            

            result res = new result();

            if (context.Request.Method != "POST"){
                jsonString = "!!!Solo esta Habilitado para el Metodo POST.";
                await context.Response.WriteAsync(jsonString ?? "!!!Ocurrio algun Error.");
                return;
            }
            if (context.Request.ContentType != "application/json"){
                jsonString = "!!!Solo esta Habilitado para Recibir un JSON.";
                await context.Response.WriteAsync(jsonString ?? "!!!Ocurrio algun Error.");
                return;
            }
                
            // If there's a body, parse it as JSON and check for "message" field.
            using TextReader reader = new StreamReader(request.Body);
            string text = await reader.ReadToEndAsync();
            if (text.Length > 0)
            {
                try
                {
                    info json = JsonSerializer.Deserialize<info>(text);
                    res.position = GetLocation(json.satellites[0].distance, json.satellites[1].distance, json.satellites[2].distance);
                    res.message = GetMessage(json.satellites[0].message, json.satellites[1].message, json.satellites[2].message);
                    jsonString = JsonSerializer.Serialize(res);
                }
                catch (JsonException parseException)
                {
                    _logger.LogError(parseException, "Error parsing JSON request");
                }
            }                

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(jsonString ?? "!!!Ocurrio algun Error.");
        }
    }
}
