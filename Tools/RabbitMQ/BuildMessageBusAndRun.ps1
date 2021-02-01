##Delete old image
docker kill $(docker container ls -aq --filter name=rabbitmq*)
docker rm $(docker container ls -aq --filter name=rabbitmq*)

##Build image
docker build -t rabbitmq Tools\RabbitMQ\.

##Run container
docker run -d  --name rabbitmq -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest -p 5672:5672 -p 15672:15672 rabbitmq:3-management
