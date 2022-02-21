using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Web;

namespace Model;

public class Grant
{
    public static List<Permissions> permissionsList;
    public static List<Document> documentList;

    // We only add documents since permissions will be added inside testcase
    public static void fillDocuments()
    {
        permissionsList = new List<Permissions>();
        documentList = new List<Document>();

        documentList.Add(new Document(1, "Document 1"));
        documentList.Add(new Document(2, "Document 2"));
        documentList.Add(new Document(3, "Document 3"));
        documentList.Add(new Document(4, "Document 4"));
        documentList.Add(new Document(5, "Document 5"));

    }

    public static string addPermission(int user_id, int doc_id)
    {
        string result = "";
        // If user is not logged and is not a manager, just exit
        bool islogged = Login.loggedEmployee != -1;
        bool ismanager = Login.isManager(Login.loggedEmployee);
        if (!islogged)
        {
            result += "not authenticated ";
        }
        if (!ismanager)
        {
            result += "not manager";
        }
        //System.Console.WriteLine($"Result logged: {islogged} - manager: {ismanager}");
        if (islogged && ismanager)
        {
            // First, we check if permission has been added so it's not duplicated
            if (!isPermissionForUserAdded(user_id, doc_id))
            {
                //System.Console.WriteLine("Added");
                permissionsList.Add(new Permissions(user_id, doc_id));
            }
            // Only return success if the permission has been added
            if (isPermissionForUserAdded(user_id, doc_id))
                result = "success";
        }
        return result;
    }

    public static string removePermission(int user_id, int doc_id)
    {
        string result = "";
        bool islogged = Login.loggedEmployee != -1;
        bool ismanager = Login.isManager(Login.loggedEmployee);

        if (!islogged)
        {
            result += "not authenticated ";
        }
        if (!ismanager)
        {
            result += "not manager";
        }
        if (islogged && ismanager)
        {
            // We have to iterate each one to remove the same object
            foreach (Permissions p in permissionsList)
            {
                if (p.user_id == user_id && p.doc_id == doc_id)
                {
                    permissionsList.Remove(p);
                    if (!Grant.isPermissionForUserAdded(p.user_id, p.doc_id))
                    {
                        result = "success";
                    }
                    return result;
                }
            }
            if (!result.Equals("success")) {
                result = "document doesn't exist";
            }
        }

        return result;
    }
    public static bool isPermissionForUserAdded(int user_id, int doc_id)
    {
        foreach (Permissions p in permissionsList)
        {
            if (p.user_id == user_id && p.doc_id == doc_id)
            {
                return true;
            }
        }
        return false;
    }

    public static string serializeListOfPermissions(List<Permissions> list)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(list, options);
        return jsonString;
    }

    public static Response GetDocumentById(Response response) {
        
        Console.WriteLine($"Downloading document {response.request_url}");

        var uri = new Uri(response.request_url);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var id = Int64.Parse(query.Get("id"));

        int user_id = Login.loggedEmployee;
        response.content = "";
        bool ismanager = Login.isManager(Login.loggedEmployee);
        bool islogged = Login.loggedEmployee != -1;

        // This is what we will return
        List<Permissions> newPermissionsList = new List<Permissions>();

        if (!islogged)
        {
            response.message += "not authenticated ";
        }
        if (!ismanager)
        {
            response.message += "not manager";
        }
        
        if (islogged) {
            
            if (ismanager) {
                foreach (Permissions p in permissionsList) {
                    if (id == p.doc_id) {
                        Console.WriteLine($"Downloading document {id} as manager");
                        response.content = $"Downloading document {id}";
                        return response;
                    }
                }
            } else {
                foreach (Permissions p in permissionsList) {
                    if (p.user_id == user_id && id == p.doc_id) {
                        Console.WriteLine($"Downloading document {id} as user");
                        response.content = $"Downloading document {id}";
                        return response;
                    }
                }
            }
        }
        if (response.content.Equals("")) {
            response.content = "Document doesn't exist";
        }
        Console.WriteLine(response);
        return response;
    }

    public static Response getDocumentsPerUser(Response response) {
        int user_id = Login.loggedEmployee;
        response.content = "";
        bool ismanager = Login.isManager(Login.loggedEmployee);
        bool islogged = Login.loggedEmployee != -1;

        // This is what we will return
        List<Permissions> newPermissionsList = new List<Permissions>();

        if (!islogged)
        {
            response.message += "not authenticated ";
        }
        if (!ismanager)
        {
            response.message += "not manager";
        }
        if (islogged) {
            
            if (ismanager) {
                newPermissionsList = Grant.permissionsList;
            } else {
                foreach (Permissions p in permissionsList) {
                    if (p.user_id == user_id) {
                        newPermissionsList.Add(p);
                    }
                }
            }
        }
        // Serializing the result
        response.content = serializeListOfPermissions(newPermissionsList);
        return response;
    }
}