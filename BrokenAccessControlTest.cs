using NUnit.Framework;
using Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace test;

public class BrokenAccessControlTest
{
    public static Response ApiJson(string endpointName, string inputJson)
    {
        Response response = new Response();
        string message = "error";
        if (endpointName.Equals("https://api.example.com/login"))
        {
            Employee? employee = JsonSerializer.Deserialize<Employee>(inputJson);
            Login.login(employee.email, employee.password);
            if (Login.isEmployeeLogged(employee.id)) {
                message = "success";
            }
        }
        else if (endpointName.Equals("https://api.example.com/accesses/add"))
        {
            Permissions? permissions = JsonSerializer.Deserialize<Permissions>(inputJson);
            message = Grant.addPermission(permissions.user_id, permissions.doc_id);
        }
        else if (endpointName.Equals("https://api.example.com/accesses/remove"))
        {
            Permissions? permissions = JsonSerializer.Deserialize<Permissions>(inputJson);

            message = Grant.removePermission(permissions.user_id, permissions.doc_id);
        }
        else if (endpointName.Equals("https://api.example.com/docs"))
        {

        }
        else if (endpointName.Equals("https://api.example.com/download?id=1"))
        {

        }
        response.message = message;
        return response;
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

        Response result = ApiJson("https://api.example.com/login", emp1.serialize());
        Assert.AreEqual("success", result.message);

        result = ApiJson("https://api.example.com/login", emp2.serialize());
        Assert.AreEqual("success", result.message);

        // This is error since user5 is not defined inside Login.validEmployes
        result = ApiJson("https://api.example.com/login", emp5.serialize());
        Assert.AreEqual("error", result.message);
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

        Response response = ApiJson("https://api.example.com/accesses/add", permissions11.serialize());
        Assert.AreEqual("not manager", response.message);

        response = ApiJson("https://api.example.com/accesses/add", permissions22.serialize());
        Assert.AreEqual("not manager", response.message);

        response = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());
        Assert.AreEqual("success", response.message);
    }

    [Test]
    public void VerifyEmployeeCantRemove()
    {
        Setup();

        // Login before removing
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");
        
        ApiJson("https://api.example.com/login", emp1.serialize());
        ApiJson("https://api.example.com/login", emp2.serialize());
        ApiJson("https://api.example.com/login", emp4.serialize());


        // Expecting error
        Permissions permissions11 = new Permissions(1,1);
        Permissions permissions33 = new Permissions(3,3);
        // This should work
        Permissions permissions41 = new Permissions(4,1);

        // Adding permission
        Response response = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());

        // Trying to remove permission with no manager user
        response = ApiJson("https://api.example.com/accesses/remove", permissions11.serialize());
        Assert.AreEqual("not manager", response.message);

        // Trying to remove permission with no authenticated and no manager user
        response = ApiJson("https://api.example.com/accesses/remove", permissions33.serialize());
        Assert.AreEqual("not authenticated not manager", response.message);

        // Removing permission
        response = ApiJson("https://api.example.com/accesses/remove", permissions41.serialize());
        Assert.AreEqual("success", response.message);
    }
}