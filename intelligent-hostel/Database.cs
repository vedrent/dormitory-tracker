using Newtonsoft.Json;
using System.Text.Json;
using Npgsql;
using System.Security.Cryptography;
using System.Text;

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
        await fillWithTestData();
    }
    public static async Task<Room[]> getRoomsByLevel(int level)
    {
        await using var connection = dataSource.CreateConnection();
        await connection.OpenAsync();
        await using var command = new Npgsql.NpgsqlCommand("select * from rooms where floor = $1::int", connection)
        {
            Parameters =
            {
                new() {Value = level},
            }
        };
        await using var reader = await command.ExecuteReaderAsync();
        List<Room> rooms = [];

        while (await reader.ReadAsync())
        {
            Room room = new Room();
            room.room_id = (int)reader[0];
            room.room_number = (string)reader[1];
            room.floor = (int)reader[2];
            room.room_type = (int)reader[3];
            room.beds_number = (int)reader[4];
            room.image_x = (int)reader[5];
            room.image_y = (int)reader[6];

            rooms.Add(room);
        }
        await connection.CloseAsync();
        return rooms.ToArray();

    }
    public static async Task<Room[]> getRoomById(int room_id)
    {
        await using var connection = dataSource.CreateConnection();
        await connection.OpenAsync();
        await using var command = new Npgsql.NpgsqlCommand("select * from rooms where room_id = $1::int", connection)
        {
            Parameters =
            {
                new() {Value = room_id},
            }
        };
        await using var reader = await command.ExecuteReaderAsync();
        List<Room> rooms = [];

        while (await reader.ReadAsync())
        {
            Room room = new Room();
            room.room_id = (int)reader[0];
            room.room_number = (string)reader[1];
            room.floor = (int)reader[2];
            room.room_type = (int)reader[3];
            room.beds_number = (int)reader[4];
            room.image_x = (int)reader[5];
            room.image_y= (int)reader[6];

            rooms.Add(room);
        }
        await connection.CloseAsync();
        return rooms.ToArray();
    }
    public static async Task<Student[]> getStudentsByRoom(int room_id)
    {
        await using var connection = dataSource.CreateConnection();
        await connection.OpenAsync();
        await using var command = new Npgsql.NpgsqlCommand("select * from students where room_id = $1::int", connection)
        {
            Parameters =
            {
                new() {Value = room_id},
            }
        };
        await using var reader = await command.ExecuteReaderAsync();
        List<Student> students = [];

        while (await reader.ReadAsync())
        {
            Student student = new Student();
            student.student_id = (int)reader[0];
            student.name = (string)reader[1];
            student.surname = (string)reader[2];
            student.lastname = (string)reader[3];
            student.dormitory_id = (int)reader[4];
            student.floor = (int)reader[5];
            student.room_id = (int)reader[6];
            student.birth_date = (DateTime)reader[7];
            student.birth_place = (string)reader[8];
            student.photo_url = (string)reader[9];
            student.dormitory_provision_order = (string)reader[10];
            student.enrollment_order = (string)(reader[11]);
            student.enrollment_date = (DateTime)reader[12];
            student.note = (string)reader[13];
            students.Add(student);
        }
        await connection.CloseAsync();
        return students.ToArray();

    }
    public static async Task<Implement[]> getImplementsByRoom(int room_id)
    {
        await using var connection = dataSource.CreateConnection();
        await connection.OpenAsync();
        await using var command = new Npgsql.NpgsqlCommand("select * from implements where room_id = $1::int", connection)
        {
            Parameters =
            {
                new() {Value = room_id},
            }
        };
        await using var reader = await command.ExecuteReaderAsync();
        List<Implement> implements = [];

        while (await reader.ReadAsync())
        {
            Implement imp = new Implement();
            imp.implement_id = (int)reader[0];
            imp.room_id = (int)reader[1];
            imp.student_id = (int)reader[2];
            imp.implement_name = (string)reader[3];
            imp.implement_type = (string)reader[4];
            imp.internal_id = (int)reader[5];
            implements.Add(imp);
        }
        await connection.CloseAsync();
        return implements.ToArray();
    }
    public static async Task<User?> getUserByLogin(string login)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new Npgsql.NpgsqlCommand(
            @"select * from users where login = $1::text", connection)
        {
            Parameters = { new() { Value = login } }
        };
        await using var reader = await command.ExecuteReaderAsync();
        List<User?> rows = [];
        while (await reader.ReadAsync())
        {
            User user = new User();
            user.user_id = (int)reader[0];
            user.login = (string)reader[1];
            user.password = (string)reader[2];
            user.username = (string)reader[3];
            user.email = (string)reader[4];
            user.phone = (string)reader[5];
            rows.Add(user);
        }
        await connection.CloseAsync();
        if (rows.Count > 1) {
            throw new Exception("getUserByLogin: more than 1 row found");
        };
        if (rows.Count == 1) return rows[0];
        if (rows.Count == 0) return null;
        return null;
    }
    private static async Task connect()
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
            password char(64),
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
            enrollment_date date,
            note varchar(500)
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
            floor int,
            room_type int,
            beds_number int,
            image_x int,
            image_y int
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
    private static async Task fillWithTestData()
    {
        string[] inventory_types = { "кровать", "стол", "стул", "тумбочка", "полка", "шкаф" };
        RoomExternalData[] roomExternalData = loadFloorCoords();
        loadFloorCoords();
        string hashTest = CreateSHA256("12345678");
        await using var command = dataSource.CreateCommand(
        @$"insert into users(login, password, username, email, phone)
            values('vlasova_login', '{hashTest}', 'vlasova_username', 'vlasova@mirea.ru', '81234567890');
          insert into users(login, password, username, email, phone)
            values('francuzova_login', '{hashTest}', 'francuzova_username', 'francuzova@mirea.ru', '81234567891');
          ");
        await using var reader = await command.ExecuteReaderAsync();

        await using var command_d = dataSource.CreateCommand(
        @"insert into dormitory(dormitory_id, name, address, floors_number, rooms_per_floor, floor_maps_url)
            values('1', 'Общежитие №1 РТУ МИРЭА', 'Проспект Вернадского, 86с1', '14', '39', '/static/images/floor.svg')
          ");
        await using var reader_d = await command_d.ExecuteReaderAsync();

        int currentRoom = 0;
        int currentStudent = 0;
        for (int i = 1; i <= 14; i++)
        {
            for (int r = 1; r < 32; r++)
            {
                await using var connection = dataSource.CreateConnection();
                await connection.OpenAsync();
                int beds_number = Random.Shared.Next(1, 5);
                await using var commandI = new Npgsql.NpgsqlCommand(
                @"insert into rooms(room_number, floor, room_type, beds_number, image_x, image_y)
                    values($1, $2, $3, $4, $5, $6)", connection)
                {
                    Parameters =
                    {
                        new() {Value = i * 100 + r},
                        new() {Value = i},
                        new() {Value = Random.Shared.Next(0, 3)},
                        new() {Value = beds_number },
                        new() {Value = roomExternalData.Where(d => d.room == r).First().x},
                        new() {Value = roomExternalData.Where(d => d.room == r).First().y}
                    }
                };
                
                currentRoom += 1;
                await using var readerI = await commandI.ExecuteReaderAsync();
                await connection.CloseAsync();
                // also add students to the room
                int students_count = Random.Shared.Next(0, 10) == 0 ? 0 : Random.Shared.Next(2, beds_number + 1);
                
                for (int st = 0; st < students_count; st++)
                {
                    await using var connection_s = dataSource.CreateConnection();
                    await connection_s.OpenAsync();
                    await using var insertStudentCommand = new Npgsql.NpgsqlCommand(
                        @"insert into students
                        (
                            name,
                            surname,
                            lastname,
                            dormitory_id,
                            floor,
                            room_id,
                            birth_date,
                            birth_place,
                            photo_url,
                            dormitory_provision_order,
                            enrollment_order,
                            enrollment_date,
                            note
                        )
                        values
                        (
                            $1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13
                        )", connection_s)
                    { 
                        Parameters =
                        {
                            new() {Value = "Имя" +  Random.Shared.Next(0, 320000).ToString()},
                            new() {Value = "Фамилия" +  Random.Shared.Next(0, 320000).ToString()},
                            new() {Value = "Отчество" +  Random.Shared.Next(0, 320000).ToString()},
                            new() {Value = 1},
                            new() {Value = i},
                            new() {Value = currentRoom},
                            new() {Value = new DateTime(Random.Shared.Next(1999, 2007), Random.Shared.Next(1, 13), Random.Shared.Next(1, 8)) },
                            new() {Value = "место_рождения" + Random.Shared.Next(0, 320000).ToString()},
                            new() {Value = "url:" + Random.Shared.Next(0, 320000).ToString()},
                            new() {Value = "приказ#" + Random.Shared.Next(0, 320000).ToString()},
                            new() {Value = "приказ#" + Random.Shared.Next(0, 320000).ToString()},
                            new() {Value = new DateTime(Random.Shared.Next(2019, 2023), Random.Shared.Next(1, 13), Random.Shared.Next(1, 8)) },
                            new() {Value = ""}

                        }
                    };
                    await using var readerS = await insertStudentCommand.ExecuteReaderAsync();
                    await connection_s.CloseAsync();
                    currentStudent += 1;

                    int inventoryCount_OfStudent = Random.Shared.Next(2, 5);
                    for (int inv_step = 0; inv_step < inventoryCount_OfStudent; inv_step++)
                    {
                        string type = inventory_types[Random.Shared.Next(0, 6)];
                        await using var connection_inv = dataSource.CreateConnection();
                        await connection_inv.OpenAsync();

                        await using var insertInventoryCommand = new Npgsql.NpgsqlCommand(
                        @"insert into implements
                        (
                            room_id, student_id, implement_name, implement_type, internal_id
                        ) values($1, $2, $3, $4, $5)", connection_inv)
                        {
                            Parameters =
                            {
                                new() {Value = currentRoom},
                                new() {Value = currentStudent},
                                new() {Value = type + " " +  new string[]{"дерево", "пластик", "металл"}[Random.Shared.Next(0, 3)] },
                                new() {Value = type},
                                new() {Value = Random.Shared.Next()}
                            }
                        };
                        await using var reader_inv = await insertInventoryCommand.ExecuteReaderAsync();
                        await connection_inv.CloseAsync();
                    }
                }
            }
        }
    }
    public static string CreateSHA256(string input)
    {
        using SHA256 hash = SHA256.Create();
        return Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(input)));
    }
    private static RoomExternalData[] loadFloorCoords()
    {
        string raw_data = File.ReadAllText("./floor_coords.json");
        RoomExternalData[] data = JsonConvert.DeserializeObject<RoomExternalData[]>(raw_data);
        return data;
    }
}

