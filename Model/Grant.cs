using System;
using System.Collections.Generic;

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
        bool islogged = Login.isEmployeeLogged(user_id);
        bool ismanager = Login.isManager(user_id);
        if (!islogged) {
            result += "not authenticated ";
        }
        if (!ismanager) {
            result += "not manager";
        }
        System.Console.WriteLine($"Result logged: {islogged} - manager: {ismanager}");
        if (islogged && ismanager)
        {
            // First, we check if permission has been added so it's not duplicated
            if (!isPermissionForUserAdded(user_id, doc_id))
            {
                System.Console.WriteLine("Added");
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
        // If user is not logged, just exit
        bool islogged = Login.isEmployeeLogged(user_id);
        bool ismanager = Login.isManager(user_id);
        if (!islogged) {
            result += "not authenticated ";
        }
        if (!ismanager) {
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
                    if (!Grant.isPermissionForUserAdded(p.user_id, p.doc_id)) {
                        result = "success";
                    }
                    return result;
                }
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
}