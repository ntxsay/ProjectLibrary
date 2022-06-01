// See https://aka.ms/new-console-template for more information
using LibApi.ViewModels.Libraries;

Console.WriteLine("Hello, World!");

LibraryVM library = new()
{
    Name = "Ma bibliothèque 22313",
};

var isCreated = await library.CreateAsync();
Console.WriteLine("Created : " + isCreated);
Console.ReadLine();
Console.WriteLine("Id : " + library.Id);
Console.ReadLine();
var isDeleted = await library.DeleteAsync();
Console.WriteLine("Deleted : " + isDeleted);
Console.ReadLine();

