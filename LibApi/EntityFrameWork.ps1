dotnet ef dbcontext scaffold "Data Source=LibraryDB.db" Microsoft.EntityFrameworkCore.Sqlite -o Models/Local/SQLite -f -c LibrarySqLiteDbContext

Scaffold-DbContext "Data Source=LibraryDB.db" Microsoft.EntityFrameworkCore.Sqlite  -Context LibrarySqLiteDbContext -OutputDir Models/Local/SQLite -Force

<# StorageFolder localFolder = ApplicationData.Current.LocalFolder;
var file = localFolder.GetFileAsync("DMA.db").AsTask().GetAwaiter().GetResult();
if (file == null) return;
optionsBuilder.UseSqlite($"Data Source={file.Path}"); 
.ValueGeneratedOnAdd();
#>