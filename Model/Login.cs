using System;
using System.Collections.Generic;

namespace Model;

public class Login
{
    public static List<int> loggedEmployees;
    public static List<Employee> validEmployees;
    public static void fillValidEmployees()
    {
        loggedEmployees = new List<int>();
        validEmployees = new List<Employee>();
        Employee emp1 = new Employee(1, "example1@test.com", "password", "employee");
        Employee emp2 = new Employee(2, "example2@test.com", "password", "employee");
        Employee emp3 = new Employee(3, "example3@test.com", "password", "employee");
        Employee emp4 = new Employee(4, "example4@test.com", "password", "manager");
        validEmployees.Add(emp1);
        validEmployees.Add(emp2);
        validEmployees.Add(emp3);
        validEmployees.Add(emp4);
    }

    public static void login(string email, string password)
    {
        foreach (Employee e in validEmployees)
        {
            if (e.email.Equals(email) && e.password.Equals(password))
            {
                if (isEmployeeLogged(e.id)) {
                    System.Console.WriteLine("Already logged in!");
                    break;
                }
                System.Console.WriteLine("Login successfull");
                loggedEmployees.Add(e.id);
                break;
            }

        }
        loggedEmployees.ForEach(Console.WriteLine);
        System.Console.WriteLine("Oops, try again!");
    }
    public static bool isEmployeeLogged(int id)
    {
        foreach (int _id in loggedEmployees)
        {
            if (_id == id)
            {
                return true;
            }
        }
        return false;
    }
    public static bool isManager(int id)
    {
        foreach (Employee e in validEmployees)
        {
            if (e.id == id && e.employee_type.Equals("manager"))
            {
                return true;
            }
        }
        return false;
    }
}