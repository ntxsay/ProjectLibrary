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