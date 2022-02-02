# MercadoLibreSatelites
Operación Fuego de Quasar

Dada una investigacion inicial para determinar como se realiza la Triangulacion, se determino que teniendo tres posiciones X, Y, la distacia el ejercicio puede ser resuelto por Trilateracion.
En el caso del mensaje, se realiza un recorrido de los arreglos y se toma la palabra de uno, asociando que en cada posicion cada uno envia la misma palabra. Y para validar el mensaje
se contempla que menos de 3 es un mensaje incompleto.

El nivel 1 realizarlo en Go

El nivel 2 y 3 se realizo en Google, bajo .net Core

Nota: se intento validar el resultado del Nivel 2 en ciertas platadormas online de triangulacion ingresando los valores, no dando como lo indica el ejercio, segun esos valores nunca se logra 
  la triangulacion (interseccion de las 3 circunferencias). Al no dominar el tema de la triangulacion no se contemplo dicha validacion.

# NIVEL 1
Se realizo en GO, el mismo se puede correr con "run go topsecret.go" recordar tener el entorno de GO instalado

## Codigo Fuente
```
package main

import (
	"fmt"
	"math"
)

//Estructura para almacenar las coordenadas x, y, distancia
type Coordinate struct {
	x float64
	y float64
	d float64
}

//Estructura para almacenar la informacion de los satelites indicada para luego ser procesada
type Satellite struct {
	name     string
	distance float64
	message  []string
}

func main() {
	//Inicializamos la estructura de los satelites
	info := []Satellite{
		{
			name:     "Mark",
			distance: 100,
			message:  []string{"este", "", "", "mensaje", ""},
		},
		{
			name:     "Mark",
			distance: 115.5,
			message:  []string{"", "es", "", "", "secreto"},
		},
		{
			name:     "Mark",
			distance: 142.7,
			message:  []string{"este", "", "un", "", ""},
		},
	}

	//Satelite 1, para ser procesada por la funcion GetLocation
	c1 := Coordinate{
		x: -500,
		y: -200,
		d: info[0].distance,
	}
	//Satelite 2, para ser procesada por la funcion GetLocation
	c2 := Coordinate{
		x: 100,
		y: -100,
		d: info[1].distance,
	}
	//Satelite 3, para ser procesada por la funcion GetLocation
	c3 := Coordinate{
		x: 500,
		y: 100,
		d: info[2].distance,
	}

	//Invocamos la funcion getLocation para obtener la posicion de la nave que envia la se#al
	GetLocation(c1, c2, c3)
	//Invocamos la funcion GetMessage para desifrar el mensaje de las 3 Naves
	//Si el mensaje se logra obtener menos de 3 palabras se determina como mensaje incompleto
	GetMessage(info[0].message, info[1].message, info[2].message)

}

func GetLocation(c1, c2, c3 Coordinate) (x, y float64) {

	//Segun lo investigado se determino que se debe usar Trilaterate para obtener la ubicacion de la Nave

	d1, d2, d3, i1, i2, i3, j1, j2, j3 := &c1.d, &c2.d, &c3.d, &c1.x, &c2.x, &c3.x, &c1.y, &c2.y, &c3.y

	x = ((((math.Pow(*d1, 2)-math.Pow(*d2, 2))+(math.Pow(*i2, 2)-math.Pow(*i1, 2))+(math.Pow(*j2, 2)-math.Pow(*j1, 2)))*(2**j3-2**j2) - ((math.Pow(*d2, 2)-math.Pow(*d3, 2))+(math.Pow(*i3, 2)-math.Pow(*i2, 2))+(math.Pow(*j3, 2)-math.Pow(*j2, 2)))*(2**j2-2**j1)) / ((2**i2-2**i3)*(2**j2-2**j1) - (2**i1-2**i2)*(2**j3-2**j2)))

	y = ((math.Pow(*d1, 2) - math.Pow(*d2, 2)) + (math.Pow(*i2, 2) - math.Pow(*i1, 2)) + (math.Pow(*j2, 2) - math.Pow(*j1, 2)) + x*(2**i1-2**i2)) / (2**j2 - 2**j1)

	fmt.Println("********** Resultado de X")
	fmt.Println(x)
	fmt.Println("********** Resultado de Y")
	fmt.Println(y)

	return x, y
}

func GetMessage(messages1, messages2, messages3 []string) (msg string) {
	//Se realizo un ciclo For de longitud 5 el cual seria el maximo de palabras que enviaria la nave, se buscan las posiciones que sean distintas
	//de vacio para ir concatenando cada palabra y un contador para validar que menos de 3 el mensaje esta incompleto
	var cant_acept = 0
	for i := 0; i < 5; i++ {
		if messages1[i] != "" {
			msg += messages1[i] + " "
			cant_acept++
		} else {
			if messages2[i] != "" {
				msg += messages2[i] + " "
				cant_acept++
			} else {
				if messages3[i] != "" {
					msg += messages3[i] + " "
					cant_acept++
				} else {
					msg += ""
				}
			}
		}
	}
	fmt.Println("********** Mesange")
	if cant_acept < 3 {
		fmt.Println("!!!No se logro completar en su totalidad el mensaje.")
	} else {
		fmt.Println(msg)
	}
	return msg
}
```

# NIVEL 2
Se desarrolo en la paltaforma de Google, bajo .net core 3.1 con Metodo POST entrada JSON
Se valido que solo permita el metodo POST y la estrutura de entrada sea un JSON, al igual que en el Nivel 1 se contemplo que si el mensaje tiene menos de 3 palabras
se determina como mensaje incompleto.

## Para consumir la API (POST)
* URL: https://us-east1-winter-field-339822.cloudfunctions.net/topsecret
```
JSON: {
    "satellites": [
        {
        "name": "kenobi",
        "distance": 100.0,
        "message": ["este", "", "", "mensaje", ""]
        },
        {
        "name": "skywalker",
        "distance": 115.5,
        "message": ["", "es", "", "", "secreto"]
        },
        {
        "name": "sato",
        "distance": 142.7,
        "message": ["este", "", "un", "", ""]
        }
    ]
}
```
## Resultado
```
{
    "position": {
        "x": -487.2859125,
        "y": 1557.014225
    },
    "message": "este es un mensaje secreto "
}
```
## Codigo Fuente
```
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

                    if (res.message == ""){
                        jsonString = "!!!No se logro completar en su totalidad el mensaje.";
                        await context.Response.WriteAsync(jsonString ?? "!!!Ocurrio algun Error.");
                        return;
                    }

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
```

# Nivel 3
Se desarrolo en la paltaforma de Google, bajo .net core 3.1 con Metodo POST o GET. En caso de ser POST la entrada sea un JSON

Se realizo una misma URL, pero dependiendo del Metodo se toma cierta variable de entrada para un resultado, ejemplo:
Para POST se toma la distancia para buscar cual es el satelite (Se contemplaron las distancias dada en el ejercio para elegir uno)
Par GET se toma el nombre en la URL para buscar cual es el satelite (Se contemplaron los nombres dados en el ejercio para elegir uno)

Se valido que solo permita el metodo POST o GET, para POST la estrutura de entrada sea un JSON, al igual que en el Nivel 1 se contemplo que si el mensaje tiene menos de 3 palabras
se determina como mensaje incompleto.

## Para consumir la API (POST)
* URL: https://us-east1-winter-field-339822.cloudfunctions.net/topsecret_split/
```
JSON: {
    "distance": 100.0,
    "message": ["este", "", "", "mensaje", ""]
}
```
## Resultado
```
{
    "position": {
        "x": -487.2859125,
        "y": 1557.014225
    },
    "message": "este es un mensaje secreto "
}
```
## Para consumir la API (GET)
* URL: https://us-east1-winter-field-339822.cloudfunctions.net/topsecret_split/?satellite_name=sato
## Resultado
```
{
    "position": {
        "x": -487.2859125,
        "y": 1557.014225
    },
    "message": "este es un mensaje secreto "
}
```
## Codigo Fuente
```
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
            public string[] message { get; set; }
        }
        //END / Objeto Retorna JSON resultado

        //Objeto para Leer la Informacion del JSON
        public class satellite
        { 
            public double distance { get; set; } 
            public string[] message { get; set; }
        } 
        //END / Objeto para Leer la Informacion del JSON

        private readonly ILogger _logger;

        public Function(ILogger<Function> logger) =>
            _logger = logger;

        public async Task HandleAsync(HttpContext context)
        {
            
            HttpRequest request = context.Request;
            // Check URL parameters for "message" field

            string message = request.Query["message"];

            string satellite_name = request.Query["satellite_name"];

            string jsonString = ""; 
            result res = new result();

            if ((context.Request.Method != "POST") && (context.Request.Method != "GET")){
		        //response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
                jsonString = "!!!Solo esta Habilitado para el Metodo POST o GET.";
                await context.Response.WriteAsync(jsonString ?? "!!!Ocurrio algun Error.");
                return;
            }

            if ((context.Request.Method == "POST") && (context.Request.ContentType != "application/json")){
                jsonString = "!!!Solo esta Habilitado para Recibir un JSON.";
                await context.Response.WriteAsync(jsonString ?? "!!!Ocurrio algun Error.");
                return;
            }
            
                        
            switch (context.Request.Method){
                case ("GET"):
                    jsonString = "GET";
                    switch (satellite_name){
                        case ("kenobi"):
                            coord c1 = new coord();
                            c1.x = -500;
                            c1.y = -200;
                            res.position = c1;
                            res.message = new string[] {"este", "", "", "mensaje", ""};
                            break;                         
                        case ("skywalker"):
                            coord c2 = new coord();
                            c2.x = 100;
                            c2.y = -100;
                            res.position = c2;
                            res.message = new string[] {"", "es", "", "", "secreto"};
                            break;                         
                        case ("sato"):
                            coord c3 = new coord();
                            c3.x = 500;
                            c3.y = 100;
                            res.position = c3;
                            res.message = new string[] {"este", "", "un", "", ""};
                            break;                
                    }
                    jsonString = JsonSerializer.Serialize(res);
                    break;    
                case ("POST"):
                    jsonString = "POST";
                    break;                                
            } 

            using TextReader reader = new StreamReader(request.Body);
            string text = await reader.ReadToEndAsync();
            if (text.Length > 0)
            {
                try
                {
                    satellite json = JsonSerializer.Deserialize<satellite>(text);
                
                    switch (json.distance)
                    {
                        case (100):
                            coord c4 = new coord();
                            c4.x = -500;
                            c4.y = -200;
                            res.position = c4;
                            res.message = json.message;
                            break;
                        case (115.5):
                            coord c5 = new coord();
                            c5.x = 100;
                            c5.y = -100;
                            res.position = c5;
                            res.message = json.message;
                            break;
                        case (142.7):
                            coord c6 = new coord();
                            c6.x = 500;
                            c6.y = 100;
                            res.position = c6;
                            res.message = json.message;
                            break;
                        // default:
                        //     //response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
                        //     //await response.WriteAsync("Something blew up!");
                        //     break;
                    }
                    
                    jsonString = JsonSerializer.Serialize(res);
                    
                }
                catch (JsonException parseException)
                {
                    _logger.LogError(parseException, "Error parsing JSON request");
                }
            }  
            
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(jsonString ?? "Hello World!");
        }
    }
}
```

