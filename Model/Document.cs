namespace Model;

public class Document
{
    public int id { get; set; }
    public string name { get; set; }
    public Document(int id, string name) {
        this.id = id;
        this.name = name;
    }
}