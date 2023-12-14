создать volume:
docker volume create app_volume

запустить базу данных в docker: 
sudo docker run -d --name postgresCont --mount source=app_volume,target=/db -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres
sudo docker run -d --name postgresCont -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres

Настроить базу данных:
sudo docker exec -it postgresCont bash
psql -h localhost -U postgres
create database dormitory_db;
quit
exit

развернуть: 
sudo docker run -d --network=host --name=app --mount source=app_volume,target=/app danst79/intelligenthostel:latest
sudo docker run --network=host --name=app danst79/intelligenthostel:latest

###
postgres=# \c dormitory_db
You are now connected to database "dormitory_db" as user "postgres".
dormitory_db=# \dt
              List of relations
 Schema |       Name       | Type  |  Owner   
--------+------------------+-------+----------
 public | dormitory        | table | postgres
 public | dormitory_access | table | postgres
 public | implements       | table | postgres
 public | rooms            | table | postgres
 public | students         | table | postgres
 public | users            | table | postgres
(6 rows)
https://docs.docker.com/storage/volumes/