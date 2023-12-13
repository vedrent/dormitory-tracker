запустить базу данных в docker: 
sudo docker run -d --name postgresCont -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres 

Настроить базу данных:
sudo docker exec -it postgresCont bash
psql -h localhost -U postgres
create database dormitory_db;
quit
exit

развернуть: 
sudo docker run --network=host danst79/intelligenthostel:latest

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
