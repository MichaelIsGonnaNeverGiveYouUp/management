using NUnit.Framework;
using Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace test;

public class Tests
{
    public static string ApiJson(string endpointName, string inputJson)
    {
        string result = "error";
        if (endpointName.Equals("https://api.example.com/login"))
        {
            Employee? employee = JsonSerializer.Deserialize<Employee>(inputJson);
            Login.login(employee.email, employee.password);
            if (Login.isEmployeeLogged(employee.id)) {
                result = "success";
            }
        }
        else if (endpointName.Equals("https://api.example.com/accesses/add"))
        {
            Permissions? permissions = JsonSerializer.Deserialize<Permissions>(inputJson);
            Grant.addPermission(permissions.user_id, permissions.doc_id);
            if (Grant.isPermissionForUserAdded(permissions.user_id, permissions.doc_id)) {
                result = "success";
            }
        }
        else if (endpointName.Equals("https://api.example.com/accesses/remove"))
        {

        }
        else if (endpointName.Equals("https://api.example.com/docs"))
        {

        }
        else if (endpointName.Equals("https://api.example.com/download?id=1"))
        {

        }
        return result;
    }

    [SetUp]
    public void Setup()
    {
        // We add our data
        Login.fillValidEmployees();
        Grant.fillDocuments();
    }

    [Test]
    public void VerifyWrongCredentials()
    {
        Setup();
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");
        Employee emp5 = new Employee(5, "example5@test.com", "password", "employee");

        string result = ApiJson("https://api.example.com/login", emp1.serialize());
        Assert.AreEqual(result,"success");
        result = ApiJson("https://api.example.com/login", emp2.serialize());
        Assert.AreEqual(result,"success");
        
        result = ApiJson("https://api.example.com/login", emp4.serialize());

        // This is error since user5 is not defined inside Login.validEmployes
        result = ApiJson("https://api.example.com/login", emp5.serialize());
        Assert.AreEqual(result,"error");
    }

    [Test]
    public void VerifyEmployeeCantAdd()
    {
        Setup();

        // Login before adding
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");
        
        ApiJson("https://api.example.com/login", emp1.serialize());
        ApiJson("https://api.example.com/login", emp2.serialize());
        ApiJson("https://api.example.com/login", emp4.serialize());


        // This next 2 should fail
        Permissions permissions11 = new Permissions(1,1);
        Permissions permissions22 = new Permissions(2,2);

        // This should be ok
        Permissions permissions41 = new Permissions(4,1);

        string result = ApiJson("https://api.example.com/accesses/add", permissions11.serialize());
        Assert.AreEqual(result,"error");
        result = ApiJson("https://api.example.com/accesses/add", permissions22.serialize());
        Assert.AreEqual(result,"error");

        result = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());
        Assert.AreEqual(result,"success");
    }
}