using AppHelpers.Serialization;
using AppHelpers;
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

            Assert.NotNull(library);
        }

        [Fact]
        public async void NewLibraryAddUpdate()
        {
            Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
            if (library != null)
            {
                bool isUpdated = await library.UpdateAsync("Capucine 2", "Contient les livres de tante Suzie et tante Anne");
                Assert.True(isUpdated);
            }
            Assert.NotNull(library);
        }

        [Fact]
        public async void NewLibraryAndAddCollection()
        {
            Library? library = await Library.CreateAsync("Library_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (library != null)
            {
                Collection? collection = await library.AddCollectionAsync("Le petit futfut", "uu");
                Assert.NotNull(collection);
            }
            Assert.NotNull(library);
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
                    bool isUpdated = await collection.UpdateAsync("Moi et les monstre", null);
                    Assert.True(isUpdated);
                }
                Assert.NotNull(collection);
            }
            Assert.NotNull(library);
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
            Assert.NotNull(library);
        }

        [Fact]
        public async void NewLibraryAndGetAllCollections()
        {
            Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie", true);
            if (library != null)
            {
                IEnumerable<Collection> collections = await library.GetAllCollectionsAsync();
                if (collections != null)
                {
                    Assert.NotEmpty(collections);
                }
                Assert.NotNull(collections);
            }
            Assert.NotNull(library);
        }

        [Fact]
        public async void GetSingleLibrary()
        {
            Library? library = await Library.GetSingleAsync(1);
            if (library != null)
            {
                //...
            }
            Assert.NotNull(library);
        }

        [Fact]
        public async void GetAllLibrary()
        {
            IEnumerable<Library> libraries = await Library.GetAllAsync();
            if (libraries != null)
            {
                Assert.NotEmpty(libraries);
            }
            Assert.NotNull(libraries);
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
                bool isDeleted = await library.DeleteAsync();
                Assert.True(isDeleted);
            }
            Assert.NotNull(library);
        }
    }
}