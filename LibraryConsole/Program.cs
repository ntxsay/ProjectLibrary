// See https://aka.ms/new-console-template for more information
using LibApi.ViewModels.Libraries;
using LibApi.Extensions;
Console.WriteLine("Entrez le nom de la bibliothèque à créer");
string? LibraryName = Console.ReadLine();

LibraryVM library = new()
{
    Name = LibraryName,
};

var orderedItems = await LibraryVM.OrderByNameAsync();
var countPage = orderedItems.CountPages(20);

for (int i = 1; i < countPage + 1; i++)
{
    var item = orderedItems[i];
}
var displayyedItem = orderedItems.DisplayPage(20, 1);
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

