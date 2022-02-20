using System.Text.Json;
using Ganss.XSS;

namespace Model;

public class Employee
{
    public int id { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string employee_type { get; set; }
    public Employee(int id, string email, string password, string employee_type)
    {
        this.id = id;
        this.email = email;
        this.password = password;
        this.employee_type = employee_type;
    }

    public string serialize()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(this, options);
        return jsonString;
    }
}