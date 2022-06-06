using AppHelpers.Serialization;
using LibApi.Services.Collections;
using LibApi.Services.Libraries;

namespace LibTest
{
    public class LibraryTest
    {
        [Fact]
        public void NewLibrary()
        {
            Library library = new ("Library_"+ DateTime.Now.ToString("yyyyMMddHHmmss"));
            
        }

        [Fact]
        public async void NewLibraryAndAddCollection()
        {
            Library library = new("Library_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Collection? collection = await library.AddCollectionAsync("Le petit futé", "uu");
            if (collection != null)
            {
                bool _bool = await collection.UpdateAsync("Moi et les monstre");
                var jsonString = collection.GetJsonDataString();
                Console.WriteLine(jsonString);
            }
        }

        [Fact]
        public async void NewLibraryAndAddCollectionAndDeleteCollection()
        {
            Library library = new("Library_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Collection? collection = await library.AddCollectionAsync("Le petit futé", "uu");
            if (collection != null)
            {
                await collection.DeleteAsync();
                Assert.True(collection.IsDeleted);
            }

           Assert.True(collection != null);
        }

        [Fact]
        public async void GetSingleLibrary()
        {
            var library = await Library.GetSingleAsync(1);

        }

        [Fact]
        public async void GetAllLibrary()
        {
            var libraries = await Library.GetAllAsync();

        }
    }
}