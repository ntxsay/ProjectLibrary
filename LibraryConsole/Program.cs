// See https://aka.ms/new-console-template for more information
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibApi.Extensions;
using LibApi.Services.Contacts;
using LibApi.Services.Libraries;
using LibShared.ViewModels.Contacts;

int maxitemPerPage = 20;

ContactRole contactRole = new ("Ak12" + DateTime.Now.ToString(), null);
var isRoleCreated = await contactRole.CreateAsync();
Console.WriteLine("Role Created : " + isRoleCreated);
Console.WriteLine("Role Name Updated : " + await contactRole.UpdateNameAsync("Alalal" + DateTime.Now.ToString()));
Console.WriteLine("Role Description Updated : " + await contactRole.UpdateDescriptionAsync("Marshall" + DateTime.Now.ToString()));
Console.WriteLine(contactRole.GetJsonDataString());
Console.ReadLine();

ContactType contactType = new("Ak12" + DateTime.Now.ToString(), null);
var isTypeCreated = await contactType.CreateAsync();
Console.WriteLine("Type Created : " + isTypeCreated);
Console.WriteLine("Type Name Updated : " + await contactType.UpdateNameAsync("Alalal" + DateTime.Now.ToString()));
Console.WriteLine("Type Description Updated : " + await contactType.UpdateDescriptionAsync("Marshall" + DateTime.Now.ToString()));
Console.WriteLine(contactType.GetJsonDataString());
Console.ReadLine();

Library? library = null;
CreateLibrary();
await ListLibraries();
await UpdateLibrary();
await DeleteLibrary();

Console.ReadLine();


void CreateLibrary()
{
    try
    {
        Console.WriteLine("Entrez le nom de la bibliothèque à créer");
        string? libraryName = Console.ReadLine();
        while (libraryName.IsStringNullOrEmptyOrWhiteSpace())
        {
            Console.WriteLine("Entrez un nom de bibliothèque valide");
            libraryName = Console.ReadLine();
        }


        library = new(libraryName, null);
        var isCreated = library.Id != 0;
        if (isCreated)
        {
            Console.WriteLine("Created with ID : " + library.Id);
            Console.WriteLine(library.GetJsonDataString());

        }
        else
        {
            Console.WriteLine("Not Created");
        }
        Console.ReadLine();
    }
    catch (Exception)
    {

        throw;
    }
}

async Task ListLibraries()
{
    try
    {
        library = null;
        if (library == null)
        {
            library = (await Library.GetSingleAsync(1))?.ConvertToObject();
            return;
        }

        Console.WriteLine("Liste des bibliothèques. Nombre maximum de page : " + maxitemPerPage);
        var countLibrary = await Library.CountAsync();
        Console.WriteLine("Compter nombre de bibliothèque : " + countLibrary);
        var orderedItems = (await Library.GetAllAsync()).OrderItemsBy(LibShared.OrderBy.Ascending, LibShared.SortBy.Name);
        int countPage = orderedItems.CountPages(maxitemPerPage);
        Console.WriteLine("Compter nombre de page : " + countPage);
        for (int i = 1; i < countPage + 1; i++)
        {
            Console.WriteLine("Items de page la page  : " + i);
            var displayyedItem = orderedItems.DisplayPage(maxitemPerPage, i).GetJsonDataString();
            var displayyedItemVM = orderedItems.DisplayPage(maxitemPerPage, i).ConvertToViewModel().GetJsonDataString();
            Console.WriteLine(displayyedItem);
            Console.WriteLine();
            Console.WriteLine(displayyedItemVM);
            Console.WriteLine();

        }
        Console.ReadLine();
    }
    catch (Exception)
    {

        throw;
    }
}

async Task UpdateLibrary()
{
    try
    {
        if (library == null)
        {
            return;
        }

        Console.WriteLine("Entrez le nouveau nom de la bibliothèque");
        string name = Console.ReadLine();
        while (name.IsStringNullOrEmptyOrWhiteSpace())
        {
            Console.WriteLine("Entrez un nom de bibliothèque valide");
            name = Console.ReadLine();
        }
        var isNameUpdated = await library.UpdateNameAsync(name);
        Console.WriteLine("Name Updated : " + isNameUpdated);

        Console.WriteLine("Entrez une description à la bibliothèque");
        string description = Console.ReadLine();

        var isDescUpdated = await library.UpdateDescriptionAsync(description);
        Console.WriteLine("Description Updated : " + isDescUpdated);
        Console.WriteLine(library.GetJsonDataString());

        Console.ReadLine();

    }
    catch (Exception)
    {

        throw;
    }
}

async Task DeleteLibrary()
{
    try
    {
        if (library == null)
        {
            return;
        }

        Console.WriteLine("Supprimer la bibliothèque ? [O/N]");
        var result = Console.ReadLine();
        while (result.IsStringNullOrEmptyOrWhiteSpace() || result != "O" && result != "N")
        {
            Console.WriteLine("Supprimer la bibliothèque ? [O/N]");
            result = Console.ReadLine();
        }

        if (result == "O")
        {
            var isDeleted = await library.DeleteAsync();
            Console.WriteLine("Deleted : " + isDeleted);
            Console.ReadLine();
        }

    }
    catch (Exception)
    {

        throw;
    }
}