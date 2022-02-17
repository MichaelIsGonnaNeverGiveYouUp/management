namespace Model;

public class Permissions
{
    public int user_id { get; set; }
    public int doc_id { get; set; }

    public Permissions(int user_id, int doc_id) {
        this.user_id = user_id;
        this.doc_id = doc_id;
    }
}