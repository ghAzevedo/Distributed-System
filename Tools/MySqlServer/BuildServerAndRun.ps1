##Copy DB files
$source = "src/Database"
$destination = "Tools/MysqlServer"

Copy-Item -Path $source -Filter "*.sql" -Recurse -Destination $destination -Container -force

##Delete old image
docker kill $(docker container ls -aq --filter name=mysql)
docker rm $(docker container ls -aq --filter name=mysql)

##Build image
docker build -t mysql-server Tools\MysqlServer\.

##Run container
docker run -d --network retailimnetwork --hostname mysqlserver -p 4306:3306 --name mysql mysql-server