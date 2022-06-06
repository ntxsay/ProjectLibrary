using AppHelpers.Serialization;
using LibApi.Models.Local.SQLite;
using LibApi.Services.Collections;
using LibApi.Services.Libraries;

namespace LibTest
{
    public class LibraryTest
    {
        [Fact]
        public async void NewLibrary()
        {
            Library? library = await Library.CreateAsync("Library_"+ DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (library != null)
            {
                //...
            }
        }

        [Fact]
        public async void NewLibraryAddUpdate()
        {
            Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
            if (library != null)
            {
                await library.UpdateAsync("Capucine 2", "Contient les livres de tante Suzie et tante Anne");
            }
        }

        [Fact]
        public async void NewLibraryAndAddCollection()
        {
            Library? library = await Library.CreateAsync("Library_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Collection? collection = await library.AddCollectionAsync("Le petit fut�", "uu");
            if (collection != null)
            {
                bool _bool = await collection.UpdateAsync("Moi et les monstre");
                var jsonString = collection.GetJsonDataString();
                Console.WriteLine(jsonString);
            }
        }

        [Fact]
        public async void NewLibraryAndAddCollectionAndUpdateCollection()
        {
            Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
            if (library != null)
            {
                Collection? collection = await library.AddCollectionAsync("Le petit fute", "Ma description");
                if (collection != null)
                {
                    await collection.UpdateAsync("Moi et les monstre", null);
                }
            }
        }

        [Fact]
        public async void NewLibraryAndAddCollectionAndDeleteCollection()
        {
            Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
            if (library != null)
            {
                Collection? collection = await library.AddCollectionAsync("Le petit futé", null);
                if (collection != null)
                {
                    await collection.DeleteAsync();
                    Assert.True(collection.IsDeleted);
                }
                Assert.True(collection != null);
            }

        }

        [Fact]
        public async void NewLibraryAndGetAllCollections()
        {
            Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
            if (library != null)
            {
                IEnumerable<Tcollection>? collections = await library.GetAllCollectionsAsync();
                if (collections != null && collections.Any())
                {
                    //...
                }
            }
        }

        [Fact]
        public async void GetSingleLibrary()
        {
            Tlibrary? library = await Library.GetSingleAsync(1);
            if (library != null)
            {
                //...
            }
        }

        [Fact]
        public async void GetAllLibrary()
        {
            IEnumerable<Tlibrary>? libraries = await Library.GetAllAsync();
            if (libraries != null && libraries.Any())
            {
                //...
            }
        }

        [Fact]
        public async void GetCountLibraries()
        {
            int count = await Library.CountAsync();
            Console.WriteLine("Nombre de bibliothèques : " + count);
        }

        [Fact]
        public async void Delete()
        {
            Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
            if (library != null)
            {
                await library.DeleteAsync();
            }
        }
    }
}