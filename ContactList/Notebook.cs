namespace ContactList;

public class Notebook
{
    private const string Filename = "contacts.txt";
    private static Notebook? _notebook;

    private Notebook()
    {
        if (!File.Exists(Filename)) File.Create(Filename);
    }
    
    public static Notebook GetInstance()
    {
        if (_notebook == null) _notebook = new Notebook();
        return _notebook;
    }
    
    public async Task<List<Contact>> GetContactsAsync()
    {
        var contacts = new List<Contact>();
        var lines = await File.ReadAllLinesAsync(Filename);
        foreach (var line in lines)
        {
            var data = line.Split(' ');
            contacts.Add(new Contact
            {
                Name = data[0],
                PhoneNumber = data[1]
            });
        }
        return contacts;
    }

    public async Task<Contact?> GetContactAsync(string name)
    {
        var contacts = await GetContactsAsync();
        return contacts.FirstOrDefault(c => c.Name == name);
    }

    public async void AddContact(Contact contact)
    {
        await File.AppendAllTextAsync(Filename, $"{contact}\n");
    }
    
    public async void UpdateContact(Contact contact)
    {
        var contacts = await GetContactsAsync();
        var index = contacts.FindIndex(c => c.Name == contact.Name);
        contacts[index] = contact;
        await File.WriteAllLinesAsync(Filename, contacts.Select(c => c.ToString()));
    }
    
    public async void DeleteContact(Contact contact)
    {
        var contacts = await GetContactsAsync();
        var index = contacts.FindIndex(c => c.Name == contact.Name);
        contacts.RemoveAt(index);
        await File.WriteAllLinesAsync(Filename, contacts.Select(c => c.ToString()));
    }
}