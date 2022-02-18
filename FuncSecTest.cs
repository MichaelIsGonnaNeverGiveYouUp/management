using NUnit.Framework;
using Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace test;

public class FuncSecTest
{
    public static Response ApiJson(string endpointName, string inputJson)
    {
        Response response = new Response();
        string message = "error";
        if (endpointName.Equals("https://api.example.com/login"))
        {
            Employee? employee = JsonSerializer.Deserialize<Employee>(inputJson);
            Login.login(employee.email, employee.password);
            if (Login.loggedEmployee == employee.id)
            {
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

        // Success - Login for employee
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Response result = ApiJson("https://api.example.com/login", emp1.serialize());
        Assert.AreEqual("success", result.message);

        // Success - Login for employee
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        result = ApiJson("https://api.example.com/login", emp2.serialize());
        Assert.AreEqual("success", result.message);

        // Success - Login for manager
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");
        result = ApiJson("https://api.example.com/login", emp4.serialize());
        Assert.AreEqual("success", result.message);

        // Error - since user5 is not defined inside Login.validEmployes
        Employee emp5 = new Employee(5, "example5@test.com", "password", "employee");
        result = ApiJson("https://api.example.com/login", emp5.serialize());
        Assert.AreEqual("error", result.message);
        
    }

    [Test]
    public void VerifyEmployeeCantAdd()
    {
        Response response;

        // Error - not manager
        Setup();
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Permissions permissions11 = new Permissions(1, 1);
        ApiJson("https://api.example.com/login", emp1.serialize());
        response = ApiJson("https://api.example.com/accesses/add", permissions11.serialize());
        Assert.AreEqual("not manager", response.message);

        // Error - not manager
        Setup();
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        Permissions permissions22 = new Permissions(2, 2);
        ApiJson("https://api.example.com/login", emp2.serialize());
        response = ApiJson("https://api.example.com/accesses/add", permissions22.serialize());
        Assert.AreEqual("not manager", response.message);

        // Success
        Setup();
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");
        Permissions permissions41 = new Permissions(4, 1);
        ApiJson("https://api.example.com/login", emp4.serialize());
        response = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());
        Assert.AreEqual("success", response.message);

        // Error - not authenticated not manager. Because user doesn't exist
        Setup();
        Employee emp5 = new Employee(5, "example5@test.com", "password", "employee");
        Permissions permissions55 = new Permissions(5, 5);
        response = ApiJson("https://api.example.com/accesses/add", permissions55.serialize());
        Assert.AreEqual("not authenticated not manager", response.message);
    }

    [Test]
    public void VerifyEmployeeCantRemove()
    {
        Response response;

        // Error - not manager
        Setup();
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Permissions permissions11 = new Permissions(1, 1);
        ApiJson("https://api.example.com/login", emp1.serialize());
        response = ApiJson("https://api.example.com/accesses/remove", permissions11.serialize());
        Assert.AreEqual("not manager", response.message);

        // Error - not manager
        Setup();
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        Permissions permissions22 = new Permissions(2, 2);
        ApiJson("https://api.example.com/login", emp2.serialize());
        response = ApiJson("https://api.example.com/accesses/remove", permissions22.serialize());
        Assert.AreEqual("not manager", response.message);

        // Success
        Setup();
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");
        Permissions permissions41 = new Permissions(4, 1);
        ApiJson("https://api.example.com/login", emp4.serialize());
        // Adding a document to remove it later
        response = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());
        response = ApiJson("https://api.example.com/accesses/remove", permissions41.serialize());
        Assert.AreEqual("success", response.message);

        // Document doesn't exist
        Setup();
        emp4 = new Employee(4, "example4@test.com", "password", "manager");
        permissions41 = new Permissions(4, 1);
        ApiJson("https://api.example.com/login", emp4.serialize());
        // We just try to remove something that doesn't exist
        response = ApiJson("https://api.example.com/accesses/remove", permissions41.serialize());
        Assert.AreEqual("document doesn't exist", response.message);

        // Error - not authenticated not manager. Because user doesn't exist
        Setup();
        Employee emp5 = new Employee(5, "example5@test.com", "password", "employee");
        Permissions permissions55 = new Permissions(5, 5);
        response = ApiJson("https://api.example.com/accesses/remove", permissions55.serialize());
        Assert.AreEqual("not authenticated not manager", response.message);
    }

    /* [Test]
    public void VerifyListOfDocuments()
    {
        Setup();
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");

        // Login
        ApiJson("https://api.example.com/login", emp1.serialize());
        ApiJson("https://api.example.com/login", emp2.serialize());
        ApiJson("https://api.example.com/login", emp4.serialize());

        // Adding permissions for user 4
        Permissions permissions41 = new Permissions(4, 1);

        // Adding permission
        Response response = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());
    } */
}