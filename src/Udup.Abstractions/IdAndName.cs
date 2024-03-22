namespace Udup.Abstractions;

public class IdAndName
{
    public IdAndName()
    {
    }

    public IdAndName(string id)
    {
        Id = id;
        Name = id;
    }

    public IdAndName(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}