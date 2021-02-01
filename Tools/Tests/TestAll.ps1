#Build Sql server for Integration tests
echo "Build MySQL Server"
.\Tools\MySqlServer\BuildServerAndRun.ps1

echo "Build MySQL RabbitMQ"
.\Tools\RabbitMQ\BuildMessageBusAndRun.ps1

echo "Execute Unit test"
cd .\test\RetailInMotion.UnitTest\
dotnet test
cd ..\..

echo "Execute integration test"
cd .\test\retailInMotion.IntegrationTest\
dotnet test
cd ..\..