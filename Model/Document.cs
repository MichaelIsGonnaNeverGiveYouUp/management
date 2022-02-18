using System.Text.Json;

namespace Model;

public class Document
{
    public int id { get; set; }
    public string name { get; set; }
    public Document(int id, string name) {
        this.id = id;
        this.name = name;
    }
    public string serialize()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(this, options);
        return jsonString;
    }
}