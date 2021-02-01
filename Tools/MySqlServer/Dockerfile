FROM mysql:5.6

ENV MYSQL_DATABASE retailinmotion
ENV MYSQL_ROOT_PASSWORD=password1

## all scripst into   docker-entrypoint-initdb.d/ are executed automatically
COPY Database/ ./docker-entrypoint-initdb.d/