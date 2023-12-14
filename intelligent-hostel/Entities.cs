namespace intelligent_hostel;

public class Room
{
    public int room_id;
    public string room_number;
    public int floor;
    public int room_type;
    public int beds_number;
    public int image_x;
    public int image_y;
}

public class User
{
    public int user_id;
    public string login;
    public string password;
    public string username;
    public string email;
    public string phone;
}

public class Student
{
    public int student_id;
    public string name;
    public string surname;
    public string lastname;
    public int dormitory_id;
    public int floor;
    public int room_id;
    public DateTime birth_date;
    public string birth_place;
    public string photo_url;
    public string dormitory_provision_order;
    public string enrollment_order;
    public DateTime enrollment_date;
    public string note;
}

public class Implement
{
    public int implement_id;
    public int room_id;
    public int student_id;
    public string implement_name;
    public string implement_type;
    public int internal_id;
}
class RoomExternalData
{
    public int x;
    public int y;
    public int room;
}