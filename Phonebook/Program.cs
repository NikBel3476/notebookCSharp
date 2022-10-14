using Phonebook;
using Phonebook.Enums;
using Phonebook.Models;

var notebook = Notebook.GetInstance();
string? errorMessage = null;

while (true)
{
    Console.Clear();
    Console.WriteLine($"{(int)Choice.ShowAll + 1}. Список контактов");
    Console.WriteLine($"{(int)Choice.Add + 1}. Добавить контакт");
    Console.WriteLine($"{(int)Choice.Update + 1}. Обновить контакт");
    Console.WriteLine($"{(int)Choice.Delete + 1}. Удалить контакт");
    Console.WriteLine($"{(int)Choice.Exit + 1}. Выйти");
    if (errorMessage != null) Console.WriteLine(errorMessage);
    Console.Write("Введите номер команды: ");
    int choice;
    try
    {
        choice = int.Parse(Console.ReadLine());
        errorMessage = null;
    }
    catch (Exception e)
    {
        errorMessage = (e is ArgumentOutOfRangeException || e is IOException)
            ? "Введено неверное значение"
            : "Призошла неизвестная ошибка";
        continue;
    }

    switch ((Choice)(choice - 1))
    {
        case Choice.ShowAll:
        {
            var contactList = await notebook.GetContactsAsync();

            Console.Clear();
            Console.WriteLine("Список контактов");
            for (var i = 0; i < contactList.Count; i++)
                Console.WriteLine($"{i + 1}. {contactList[i]}");
            Console.WriteLine("Нажмите любую клавишу для продолжения");
            Console.ReadKey();
            break;
        }
        case Choice.Add:
        {
            Console.Clear();
            Console.WriteLine("Добавление контакта");
            Console.Write("Введите имя: ");
            string name = Console.ReadLine();
            Console.Write("Введите номер телефона: ");
            string phoneNumber = Console.ReadLine();

            notebook.AddContact(new Contact
            {
                Name = name,
                PhoneNumber = phoneNumber
            });
            break;
        }
        case Choice.Update:
        {
            errorMessage = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Обновление контакта");
                if (errorMessage != null) Console.WriteLine(errorMessage);
                Console.Write("Введите имя контакта: ");
                string name = Console.ReadLine();
                
                var contact = await notebook.FindContactAsync(name);
                if (contact == null)
                    errorMessage = "Контакт не найден";
                else
                {
                    Console.Write("Введите новый номер телефона: ");
                    string newPhoneNumber = Console.ReadLine();
                    contact.PhoneNumber = newPhoneNumber;
                    notebook.UpdateContact(contact);
                    errorMessage = null;
                    break;
                }
            }
            break;
        }
        case Choice.Delete:
        {
            errorMessage = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Удаление контакта");
                if (errorMessage != null) Console.WriteLine(errorMessage);
                Console.Write("Введите имя контакта: ");
                string name = Console.ReadLine();
                
                var contact = await notebook.FindContactAsync(name);
                if (contact == null)
                    errorMessage = "Контакт не найден";
                else
                {
                    notebook.DeleteContact(contact);
                    errorMessage = null;
                    break;
                }
            }
            break;
        }
        case Choice.Exit:
        {
            return;
        }
    }
}