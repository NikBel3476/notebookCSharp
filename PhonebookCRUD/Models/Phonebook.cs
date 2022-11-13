namespace PhonebookCRUD.Models;

/// <summary>
/// Телефонная книга
/// </summary>
public class Phonebook
{
    private const string Filename = "contacts.txt";
    private static Phonebook? _phonebook;
    private Action<string>? _onAddContact;
    private Action<string>? _onUpdateContact;
    private Action<string>? _onRemoveContact;
    private Action<string>? _onError;

    public event Action<string> OnAddContact
    {
        add => _onAddContact += value;
        remove
        {
            if (_onAddContact is not null)
                _onAddContact -= value;
        }
    }

    public event Action<string> OnUpdateContact
    {
        add => _onUpdateContact += value;
        remove
        {
            if (_onUpdateContact is not null)
                _onUpdateContact -= value;
        }
    }

    public event Action<string> OnRemoveContact
    {
        add => _onRemoveContact += value;
        remove
        {
            if (_onRemoveContact is not null)
                _onRemoveContact -= value;
        }
    }

    public event Action<string> OnError
    {
        add => _onError += value;
        remove
        {
            if (_onError is not null)
                _onError -= value;
        }
    }

    private Phonebook()
    {
        if (!File.Exists(Filename))
            File.Create(Filename).Close();
    }

    /// <summary>
    /// Метод получения экземпляра класса
    /// </summary>
    /// <returns>Экземпляр класса Phonebook</returns>
    public static Phonebook GetInstance()
    {
        if (_phonebook is null)
            _phonebook = new Phonebook();
        return _phonebook;
    }

    /// <summary>
    /// Получить список всех контактов
    /// </summary>
    /// <returns>Список всех контактов</returns>
    public async Task<List<Contact>> GetContactsAsync()
    {
        var contacts = new List<Contact>();
        var lines = await File.ReadAllLinesAsync(Filename);
        foreach (var line in lines)
        {
            var data = line.Split('-');
            contacts.Add(new Contact(data[0], data[1]));
        }
        return contacts;
    }

    /// <summary>
    /// Поиск контакта по имени
    /// </summary>
    /// <param name="name">Имя контака, который необходимо найти</param>
    /// <returns>Найденный контакт либо Null, если контакт не найден</returns>
    public async Task<Contact?> FindContactAsync(string name)
    {
        var contacts = await GetContactsAsync();
        return contacts.FirstOrDefault(c => c.Name == name);
    }

    /// <summary>
    /// Добавить контакт
    /// </summary>
    /// <param name="contact">Контакт, который будет добавлен</param>
    public async void AddContact(Contact contact)
    {
        var contacts = await GetContactsAsync();
        var index = contacts.FindIndex(c => c.Name == contact.Name);
        if (index == -1)
        {
            _onAddContact?.Invoke($"Контакт {contact.Name} добавлен");
            await File.AppendAllTextAsync(Filename, $"{contact}\n");
        }
        else
        {
            _onError?.Invoke($"Контакт {contact.Name} уже существует");
        }
    }

    /// <summary>
    /// Добавить контакт
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="phoneNumber">Номер телефона</param>
    public void AddContact(string name, string phoneNumber)
    {
        var contact = new Contact(name, phoneNumber);
        this.AddContact(contact);
    }

    /// <summary>
    /// Обновить номер контакта
    /// </summary>
    /// <param name="contact">Контакт с обновленными данными</param>
    public async void UpdateContact(Contact contact)
    {
        var contacts = await GetContactsAsync();
        var index = contacts.FindIndex(c => c.Name == contact.Name);
        if (index >= 0)
        {
            var oldContact = contacts[index];
            if (!oldContact.Equals(contact))
            {
                _onUpdateContact?.Invoke($"Контакт {contact.Name} обновлён");
                await File.WriteAllLinesAsync(Filename, contacts.Select(c => c.ToString()));
            }
        }
        else
        {
            _onError?.Invoke($"Контакт {contact.Name} не найден");
        }
    }

    /// <summary>
    /// Обновить номер контакта
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="phoneNumber">Номер телефона</param>
    public void UpdateContact(string name, string phoneNumber)
    {
        var contact = new Contact(name, phoneNumber);
        this.UpdateContact(contact);
    }

    /// <summary>
    /// Удалить контакт
    /// </summary>
    /// <param name="contact">Контакт, который будет удален</param>
    public async void DeleteContact(Contact contact)
    {
        var contacts = await GetContactsAsync();
        var index = contacts.FindIndex(c => c.Name == contact.Name);
        if (index >= 0)
        {
            contacts.RemoveAt(index);
            _onRemoveContact?.Invoke($"Контакт {contact.Name} удалён");
            await File.WriteAllLinesAsync(Filename, contacts.Select(c => c.ToString()));
        }
        else
        {
            _onError?.Invoke($"Контакт {contact.Name} не найден");
        }
    }

    /// <summary>
    /// Удалить контакт
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="phoneNumber">Номер телефона</param>
    public async void DeleteContact(string name)
    {
        var contact = await this.FindContactAsync(name);
        if (contact is not null)
        {
            this.DeleteContact(contact);
        }
        else
        {
            _onError?.Invoke($"Контакт {name} не найден");
        }
    }
}