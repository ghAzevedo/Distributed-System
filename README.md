# Distributed-System

The application is built using `NetCore 3.1` and `Docker` is needed in order to run it correclty.

## Architecture  of the aplication

The architecture consists in an API (`RetailInMotion.WebApi`) which generates events for the queues.

Each API endpoint contains a different pair of queues. 

The Queues are hosted/Managed in RabbitMQ.


A background worker per endpoint is located in the order microservice (`RetailInMotion.Order`) which will trigger the services and persist the database.
A response to `{queue-name}-{response}` is made at this point.


## Test
To run the tests go to the `root` folder execute the next script: `.\Tools\Tests\testAll.ps1` you need to have `Docker` installed for the execution of the Integration test.

In case that you don't have docker, you can install `MySQL` in your own machine and update the `connectionstring`.
`RabbitMQ` is also necessary for local debug.


## Development
To be able to debug correctly, `RabbitMQ` and `MySql` should be installed in the system. So both of them are being containerized using `Docker` approach.

Execute the script `.\Tools\local-development\up.ps1` to build Containers with `RabbitMQ` and `MySQL` locally:
* MySqlServer running in localhost:4306
* RabbitMQ running in http://localhost:15672/

You will need to start both the `RetailInMotion.WebApi` and the microservice `RetailInMotion.Order` in your VS.


## Production-ready
To simulate a production environment execute in the root of the repositroy the next command `docker-compose up`

The endpoints available are: 
* WebAPI: http://localhost:8080
* Order-Microservice: http://localhost:8180
* RabbitMQ: http://localhost:15672
* MySql: localhost:4306

The script located at `.\src\Database\DatabaseScript.sql` creates the table and also populate the `product` table with two results:
- `2a6b0daf-5bb7-4905-be03-3886b8b9d91a` 
- `38a50377-3780-486f-a83c-51f5add4d394`

The execution of the script is handled by the docker file `.\Tools\MySqlServer\Dockerfile` 

## API Endpoints
The api contains the next endpoints

### Create order
#### Request
POST http://localhost:8080/order/create

Body:
````
{
	"Delivery" : {
		"Country" : "ie",
		"City": "dublin",
		"Street" : "street"
	},
	"Products" : [
			{
				"ProductId" : "2a6b0daf-5bb7-4905-be03-3886b8b9d91a",
				"Quantity" : 2
			},
			{
				"ProductId" : "38a50377-3780-486f-a83c-51f5add4d394",
				"Quantity" : 2
			}
		]
}
````
#### Response
````
{
    "error": null,
    "result": {
        "orderId": "{new_Order_Guid}"
    }
}
````

### Update order
#### Request
#### Replace the {new_Order_Guid} with the new order Id generated on the Order Creation Api
PUT http://localhost:8080/order/updateproducts/{new_Order_Guid}

Body:
````
[{
	"ProductId" : "2a6b0daf-5bb7-4905-be03-3886b8b9d91a",
	"Quantity" : 3
},
{
	"ProductId" : "38a50377-3780-486f-a83c-51f5add4d394",
	"Quantity" : 3
}]
````

#### Response:
````
{
    "error": null,
    "result": {
        "success": true
    }
}
````

### Update Delivery
#### Request
#### Replace the {new_Order_Guid} with the new order Id generated on the Order Creation Api
PUT http://localhost:8080/order/updatedelivery/{new_Order_Guid}
Body:
````
{
	"Country": "ie",
	"City" : "Dublin",
	"Street" : "street "
}
````
#### Response:
````
{
    "error": null,
    "result": {
        "success": true
    }
}
````

### Get order
#### Request
#### Replace the {new_Order_Guid} with the new order Id generated on the Order Creation Api
Get http://localhost:8080/order/{new_Order_Guid}
#### Response:
````
{
    "error": null,
    "result": {
        "order": {},
            "products": [
                {},
            ]
        }
    }
}
````

### Get List of order paginated
#### Request
GET http://localhost:8080/order/page/1

#### Response
````
{
    "error": null,
    "result": {
        "order": {},
            "products": [
                {},
            ]
        }
    }
}
````
### Cancel order
#### Request
#### Replace the {new_Order_Guid} with the new order Id generated on the Order Creation Api
Delete http://localhost:8080/order/{new_Order_Guid}

#### Response
````
{
    "error": null,
    "result": {
        "success": true
    }
}
````
