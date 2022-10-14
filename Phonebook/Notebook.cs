using Phonebook.Models;

namespace Phonebook;

/// <summary>
/// Телефонная книга, реализующая CRUD функционал.
/// Singleton класс
/// </summary>
public class Notebook
{
    private const string Filename = "contacts.txt";
    private static Notebook? _notebook;

    private Notebook()
    {
        if (!File.Exists(Filename)) File.Create(Filename).Close();
    }
    
    /// <summary>
    /// Метод получения экземпляра класса
    /// </summary>
    /// <returns>Экземпляр класса Phonebook</returns>
    public static Notebook GetInstance()
    {
        if (_notebook == null) _notebook = new Notebook();
        return _notebook;
    }
    
    /// <summary>
    /// Функция получения списка всех контактов
    /// </summary>
    /// <returns>Список всех контактов</returns>
    public async Task<List<Contact>> GetContactsAsync()
    {
        var contacts = new List<Contact>();
        var lines = await File.ReadAllLinesAsync(Filename);
        foreach (var line in lines)
        {
            var data = line.Split('-');
            contacts.Add(new Contact
            {
                Name = data[0],
                PhoneNumber = data[1]
            });
        }
        return contacts;
    }

    /// <summary>
    /// Функция поиска контака по имени
    /// </summary>
    /// <param name="name">Имя контака, который необходимо найти</param>
    /// <returns>Найденный контакт либо Null, если контакт не найден</returns>
    public async Task<Contact?> FindContactAsync(string name)
    {
        var contacts = await GetContactsAsync();
        return contacts.FirstOrDefault(c => c.Name == name);
    }

    /// <summary>
    /// Функция добавления контакта
    /// </summary>
    /// <param name="contact">Контакт, который будет добавлен</param>
    public async void AddContact(Contact contact)
    {
        await File.AppendAllTextAsync(Filename, $"{contact}\n");
    }
    
    /// <summary>
    /// Функция обновления номера контакта
    /// </summary>
    /// <param name="contact">Контакт с обновленными данными</param>
    public async void UpdateContact(Contact contact)
    {
        var contacts = await GetContactsAsync();
        var index = contacts.FindIndex(c => c.Name == contact.Name);
        contacts[index] = contact;
        await File.WriteAllLinesAsync(Filename, contacts.Select(c => c.ToString()));
    }
    
    /// <summary>
    /// Функция удаления контакта
    /// </summary>
    /// <param name="contact">Контакт, который будет удален</param>
    public async void DeleteContact(Contact contact)
    {
        var contacts = await GetContactsAsync();
        var index = contacts.FindIndex(c => c.Name == contact.Name);
        contacts.RemoveAt(index);
        await File.WriteAllLinesAsync(Filename, contacts.Select(c => c.ToString()));
    }
}