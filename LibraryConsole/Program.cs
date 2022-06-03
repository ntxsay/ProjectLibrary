// See https://aka.ms/new-console-template for more information
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibApi.Extensions;
using LibApi.Services.Libraries;

int maxitemPerPage = 20;
Library? library = null;
await CreateLibrary();
await ListLibraries();
await UpdateLibrary();
await DeleteLibrary();

Console.ReadLine();


 async Task CreateLibrary()
{
    try
    {
        Console.WriteLine("Entrez le nom de la bibliothèque à créer");
        string? LibraryName = Console.ReadLine();
        while (LibraryName.IsStringNullOrEmptyOrWhiteSpace())
        {
            Console.WriteLine("Entrez un nom de bibliothèque valide");
            LibraryName = Console.ReadLine();
        }

        library = new()
        {
            Name = LibraryName,
        };

        var isCreated = await library.CreateAsync();
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
        if (library == null)
        {
            return;
        }

        Console.WriteLine("Liste des bibliothèques. Nombre maximum de page : " + maxitemPerPage);

        var orderedItems = await Library.OrderByDescendingDateCreationAsync();
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
        library.Name = Console.ReadLine();
        while (library.Name.IsStringNullOrEmptyOrWhiteSpace())
        {
            Console.WriteLine("Entrez un nom de bibliothèque valide");
            library.Name = Console.ReadLine();
        }
        Console.WriteLine("Entrez une description à la bibliothèque");
        library.Description = Console.ReadLine();
        var isUpdated = await library.UpdateAsync();
        Console.WriteLine("Updated : " + isUpdated);
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