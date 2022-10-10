namespace ContactList;

public class Contact
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"{Name} {PhoneNumber}";
    }
}