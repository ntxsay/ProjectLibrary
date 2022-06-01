// See https://aka.ms/new-console-template for more information
using LibApi.ViewModels.Libraries;

Console.WriteLine("Entrez le nom de la bibliothèque à créer");
string? LibraryName = Console.ReadLine();

LibraryVM library = new()
{
    Name = LibraryName,
};

var isCreated = await library.CreateAsync();
Console.WriteLine("Created : " + isCreated);
Console.WriteLine("Id : " + library.Id);
Console.ReadLine();
Console.WriteLine(library.GetJsonDataStringAsync());
Console.WriteLine("Entrez le nouveau nom de la bibliothèque");
library.Name = Console.ReadLine();
Console.WriteLine("Entrez une description à la bibliothèque");
library.Description = Console.ReadLine();
var isUpdated = await library.UpdateAsync();
Console.WriteLine("Updated : " + isUpdated);
Console.ReadLine();
Console.WriteLine(library.GetJsonDataStringAsync());
Console.ReadLine();
var isDeleted = await library.DeleteAsync();
Console.WriteLine("Deleted : " + isDeleted);
Console.ReadLine();

