using NUnit.Framework;
using Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace test;

public class FuncSecTest
{
    public static Response ApiJson(string endpointName, string inputJson="")
    {
        Response response = new Response();
        response.headers["Strict-Transport-Security"] = "";
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
            response = Grant.getDocumentsPerUser();
            return response;
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
        Employee emp1 = new Employee(1, "example1@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        Response result = ApiJson("https://api.example.com/login", emp1.serialize());
        Assert.AreEqual("success", result.message);

        // Success - Login for employee
        Employee emp2 = new Employee(2, "example2@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        result = ApiJson("https://api.example.com/login", emp2.serialize());
        Assert.AreEqual("success", result.message);

        // Success - Login for manager
        Employee emp4 = new Employee(4, "example4@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        result = ApiJson("https://api.example.com/login", emp4.serialize());
        Assert.AreEqual("success", result.message);

        // Error - since user5 is not defined inside Login.validEmployes
        Employee emp5 = new Employee(5, "example5@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        result = ApiJson("https://api.example.com/login", emp5.serialize());
        Assert.AreEqual("error", result.message);
        
    }

    [Test]
    public void VerifyEmployeeCantAdd()
    {
        Response response;

        // Error - not manager
        Setup();
        Employee emp1 = new Employee(1, "example1@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        Permissions permissions11 = new Permissions(1, 1);
        ApiJson("https://api.example.com/login", emp1.serialize());
        response = ApiJson("https://api.example.com/accesses/add", permissions11.serialize());
        Assert.AreEqual("not manager", response.message);

        // Error - not manager
        Setup();
        Employee emp2 = new Employee(2, "example2@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        Permissions permissions22 = new Permissions(2, 2);
        ApiJson("https://api.example.com/login", emp2.serialize());
        response = ApiJson("https://api.example.com/accesses/add", permissions22.serialize());
        Assert.AreEqual("not manager", response.message);

        // Success
        Setup();
        Employee emp4 = new Employee(4, "example4@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        Permissions permissions41 = new Permissions(4, 1);
        ApiJson("https://api.example.com/login", emp4.serialize());
        response = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());
        Assert.AreEqual("success", response.message);

        // Error - not authenticated not manager. Because user doesn't exist
        Setup();
        Employee emp5 = new Employee(5, "example5@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
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
        Employee emp1 = new Employee(1, "example1@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        Permissions permissions11 = new Permissions(1, 1);
        ApiJson("https://api.example.com/login", emp1.serialize());
        response = ApiJson("https://api.example.com/accesses/remove", permissions11.serialize());
        Assert.AreEqual("not manager", response.message);

        // Error - not manager
        Setup();
        Employee emp2 = new Employee(2, "example2@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        Permissions permissions22 = new Permissions(2, 2);
        ApiJson("https://api.example.com/login", emp2.serialize());
        response = ApiJson("https://api.example.com/accesses/remove", permissions22.serialize());
        Assert.AreEqual("not manager", response.message);

        // Success
        Setup();
        Employee emp4 = new Employee(4, "example4@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        Permissions permissions41 = new Permissions(4, 1);
        ApiJson("https://api.example.com/login", emp4.serialize());
        // Adding a document to remove it later
        response = ApiJson("https://api.example.com/accesses/add", permissions41.serialize());
        response = ApiJson("https://api.example.com/accesses/remove", permissions41.serialize());
        Assert.AreEqual("success", response.message);

        // Document doesn't exist
        Setup();
        emp4 = new Employee(4, "example4@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        permissions41 = new Permissions(4, 1);
        ApiJson("https://api.example.com/login", emp4.serialize());
        // We just try to remove something that doesn't exist
        response = ApiJson("https://api.example.com/accesses/remove", permissions41.serialize());
        Assert.AreEqual("document doesn't exist", response.message);

        // Error - not authenticated not manager. Because user doesn't exist
        Setup();
        Employee emp5 = new Employee(5, "example5@test.com", "AVeryLongLongPasswordWith3Rules!", "employee");
        Permissions permissions55 = new Permissions(5, 5);
        response = ApiJson("https://api.example.com/accesses/remove", permissions55.serialize());
        Assert.AreEqual("not authenticated not manager", response.message);
    }

    [Test]
    public void VerifyListOfDocumentsForManagers()
    {
        Response response;

        // Success
        Setup();
        Employee emp4 = new Employee(4, "example4@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        Permissions permissions11 = new Permissions(1, 1);
        Permissions permissions12 = new Permissions(1, 2);
        ApiJson("https://api.example.com/login", emp4.serialize());
        // Adding a document to remove it later
        response = ApiJson("https://api.example.com/accesses/add", permissions11.serialize());
        // Getting list of elements, it should be 1
        response = ApiJson("https://api.example.com/docs");

        // We define a list of permissions, and add progressively
        List <Permissions> expected = new List<Permissions>();
        expected.Add(permissions11);
        Assert.AreEqual(Grant.serializeListOfPermissions(expected), response.content);

        // Let's add another permission
        ApiJson("https://api.example.com/accesses/add", permissions12.serialize());
        response = ApiJson("https://api.example.com/docs");
        // Before we add the new, let's check if are not equal
        Assert.AreNotEqual(Grant.serializeListOfPermissions(expected), response.content);
        // Now we are sure, let's see if they are equal after adding the new permission
        expected.Add(permissions12);
        // Get again list of elements
        response = ApiJson("https://api.example.com/docs");
        Assert.AreEqual(Grant.serializeListOfPermissions(expected), response.content);

        // What if we delete one element?
        ApiJson("https://api.example.com/accesses/remove", permissions11.serialize());
        response = ApiJson("https://api.example.com/docs");
        // Comparing not equal again
        Assert.AreNotEqual(Grant.serializeListOfPermissions(expected), response.content);
        // Comparing the right result
        expected = new List<Permissions>();
        expected.Add(permissions12);
        response = ApiJson("https://api.example.com/docs");
        Assert.AreEqual(Grant.serializeListOfPermissions(expected), response.content);
    }

    [Test]
    public void VerifyListOfDocumentsForEmployees()
    {
        Response response;

        // Success
        Setup();
        Employee emp1 = new Employee(1, "example1@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        Employee emp2 = new Employee(2, "example2@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        Employee emp4 = new Employee(4, "example4@test.com", "AVeryLongLongPasswordWith3Rules!", "manager");
        Permissions permissions11 = new Permissions(1, 1);
        Permissions permissions12 = new Permissions(1, 2);
        Permissions permissions13 = new Permissions(1, 3);

        ApiJson("https://api.example.com/login", emp4.serialize());
        // Adding a document to remove it later
        response = ApiJson("https://api.example.com/accesses/add", permissions11.serialize());
        // Login with another user
        ApiJson("https://api.example.com/login", emp1.serialize());
        // Getting list of elements, it should be 1
        response = ApiJson("https://api.example.com/docs");

        // We define a list of permissions, and add progressively
        List <Permissions> expected = new List<Permissions>();
        expected.Add(permissions11);
        Assert.AreEqual(Grant.serializeListOfPermissions(expected), response.content);

        // Now let's login with another user and check that is empty
        ApiJson("https://api.example.com/login", emp2.serialize());
        // Expected will be empty
        expected = new List<Permissions>();
        response = ApiJson("https://api.example.com/docs");
        Assert.AreEqual(Grant.serializeListOfPermissions(expected), response.content);
        // Let's change expected to have one element and compare not equal
        expected.Add(permissions11);
        response = ApiJson("https://api.example.com/docs");
        Assert.AreNotEqual(Grant.serializeListOfPermissions(expected), response.content);

        // Go back to employee 1, is the data still there?
        expected = new List<Permissions>();
        ApiJson("https://api.example.com/login", emp1.serialize());
        // Getting list of elements, it should be 1
        response = ApiJson("https://api.example.com/docs");
        expected.Add(permissions11);
        Assert.AreEqual(Grant.serializeListOfPermissions(expected), response.content);

        // Now let's remove and check that is empty.
        response = ApiJson("https://api.example.com/docs");
        response = ApiJson("https://api.example.com/accesses/remove", permissions11.serialize());
        Assert.AreEqual(null, response.content);
    }
}