using Npgsql;

namespace intelligent_hostel;

public static class Database
{
    /* hardcoded */
    public static string connectionString;
    private static NpgsqlDataSource dataSource;

    public static async Task init()
    {
        /*connect();
        dropAllTables().Wait();
        Task.WaitAll(
            
            createUsersTable(),
            createStudentsTable(),
            createRoomsTable(),
            createDormitoryAccessTable(),
            createDormitoryTable(),
            createImplementsTable(),
            );
        createForeignKeys().Wait();*/
        connect();
        await dropAllTables();
        await createUsersTable();
        await createStudentsTable();
        await createRoomsTable();
        await createDormitoryAccessTable();
        await createDormitoryTable();
        await createImplementsTable();

        await createForeignKeys();
    }
    private static void connect()
    {
        connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=dormitory_db";
        dataSource = NpgsqlDataSource.Create(connectionString);
    }
    private static async Task createUsersTable()
    {
        await using var command = dataSource.CreateCommand(
        @"create table users
        (
            user_id serial primary key,
            login varchar(100),
            password varchar(100),
            username varchar(100),
            email varchar(100),
            phone varchar(100)
        )");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) 
        { 
            Console.WriteLine(reader.GetString(0)); 
        }
    }

    private static async Task createStudentsTable()
    {
        await using var command = dataSource.CreateCommand(
        @"create table students
        (
            student_id serial primary key,
            name varchar(50),
            surname varchar(50),
            lastname varchar(50),
            dormitory_id int,
            floor int,
            room_id int,
            birth_date date,
            birth_place varchar(200),
            photo_url varchar(300),
            dormitory_provision_order varchar(50),
            enrollment_order varchar(50),
            enrollment_date date
        )");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }

    private static async Task createRoomsTable()
    {
        await using var command = dataSource.CreateCommand(
        @"create table rooms
        (
            room_id serial primary key,
            room_number varchar(10),
            room_type int,
            beds_number int
        )");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
    private static async Task createDormitoryAccessTable()
    {
        await using var command = dataSource.CreateCommand(
        @"create table dormitory_access
        (
            user_id int,
            dormitory_id int
        )");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
    private static async Task createDormitoryTable()
    {
        await using var command = dataSource.CreateCommand(
        @"create table dormitory
        (
            dormitory_id serial primary key,
            name varchar(100),
            address varchar(200),
            floors_number int,
            rooms_per_floor int,
            floor_maps_url text
        )");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
    private static async Task createImplementsTable()
    {
        await using var command = dataSource.CreateCommand(
        @"create table implements
        (
            implement_id serial primary key,
            room_id int,
            student_id int,
            implement_name varchar(200),
            implement_type varchar(200),
            internal_id int
        )");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
    private static async Task createForeignKeys()
    {
        await using var command = dataSource.CreateCommand(
        @"alter table students
            add foreign key (dormitory_id) references dormitory (dormitory_id),
            add foreign key (room_id) references rooms (room_id);
          alter table dormitory_access
            add foreign key (user_id) references users (user_id),
            add foreign key (dormitory_id) references dormitory (dormitory_id);
          alter table implements 
            add foreign key (room_id) references rooms (room_id),
            add foreign key (student_id) references students (student_id)
            ");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
    private static async Task dropAllTables()
    {
        await using var command = dataSource.CreateCommand(
        @"drop table if exists users cascade; 
          drop table if exists students cascade;
          drop table if exists rooms cascade;
          drop table if exists dormitory_access cascade;
          drop table if exists dormitory cascade;
          drop table if exists implements cascade");
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
}
