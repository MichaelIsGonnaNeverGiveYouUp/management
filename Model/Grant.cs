using System;
using System.Collections.Generic;

namespace Model;

public class Grant {
    public static List<Permissions> permissionsList;
    public static List<Document> documentList;

    // We only add documents since permissions will be added inside testcase
    public static void fillDocuments()
    {
        permissionsList = new List<Permissions>();
        documentList = new List<Document>();

        documentList.Add(new Document(1,"Document 1"));
        documentList.Add(new Document(2,"Document 2"));
        documentList.Add(new Document(3,"Document 3"));
        documentList.Add(new Document(4,"Document 4"));
        documentList.Add(new Document(5,"Document 5"));

    }

    public static void addPermission(int user_id, int doc_id) {
        // First, we check if permission has been added so it's not duplicated
        if (!isPermissionForUserAdded(user_id, doc_id)) {
            bool islogged = Login.isEmployeeLogged(user_id);
            bool ismanager = Login.isManager(user_id);
            System.Console.WriteLine($"Not added yet {user_id} - {islogged} - {ismanager}");
            if (islogged && ismanager) {
                System.Console.WriteLine("Added");
                permissionsList.Add(new Permissions(user_id, doc_id));
            }
        }
    }
    public static bool isPermissionForUserAdded(int user_id, int doc_id) {
        foreach (Permissions p in permissionsList) {
            if (p.user_id == user_id && p.doc_id == doc_id) {
                return true;
            }
        }
        return false;
    }
}