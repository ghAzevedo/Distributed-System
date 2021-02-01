docker network create RetailInMotionNetwork

echo "Build MySQL Server"
.\Tools\MySqlServer\BuildServerAndRun.ps1

echo "build rabbitMQ"
.\Tools\RabbitMQ\BuildMessageBusAndRun.ps1

echo "MySqlServer running in localhost:4306"
echo "RabbitMQ running in http://localhost:15672/"