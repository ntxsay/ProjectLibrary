
# La LibAPI

LibAPI (Library Application Programing Interface) est une bibliothèque de classe développée avec C# et .NET 6.0 qui vous permet de créer et de gérer une ou des bibliothèques de livres dans une base de données SQLite.

## Introduction/Contexte

Physiquement parlant, une bibliothèque peut correspondre à un meuble qui permet de ranger et de classer des livres. De la même façon la LibAPI va vous permettre de créer l'objet meuble dans lequel il vous sera permis d'y ajouter des livres puis de les ranger et de les classer.

La collection est constituée par un ensemble de livres d’un même éditeur qui ont des points communs comme la présentation (format, couleurs, taille…), ou les types de lecteurs (âge, goûts…).  La collection porte un titre. Par exemple : _Chair de poule_, _Scripto_... ([pour plus d'infos](http://moncdivirtuel.free.fr/recherche_documentaire/cles_du_livre/cles_du_livre.html)).

## Les bibliothèques 

### Créer une bibliothèque

Ajoutez l'espace de nom  `using LibApi.Services.Libraries;`  dans votre fichier ensuite déclarez une variable de type  `Library?`  nommée  `library`  puis appelez la méthode asynchrone statique  `await Library.CreateAsync("nom_de_votre_bibliotheque", ["description_de_votre_description"], [Ouvrir_Si_Existe_Deja]);`  dans une méthode asynchrone.

    using LibApi.Services.Libraries;
    
    public async void NewLibrary()
    {
        Library? library = await Library.CreateAsync("Ma_bibliotheque", "Ma_description", false);
        if (library != null)
        {
            //...
        }
    }

### Récupérer des bibliothèques existantes

 1. Récupérer toutes les bibliothèques

Appelez la méthode statique asynchrone  `await Library.GetAllAsync();`.

    using LibApi.Services.Libraries;
    
    public async void GetAllLibrary()
    {
        IEnumerable<Library> libraries = await Library.GetAllAsync();
        if (libraries.Any())
        {
            //...
        }
    }

 2. Récupérer plusieurs bibliothèques


### Récupérer une bibliothèque existante

 1. Récupérer une bibliothèque par son *id*

Appelez la méthode statique asynchrone  `await Library.GetSingleAsync(id_bibliotheque_a_recuperer);`.

    using LibApi.Services.Libraries;
    
    public async void GetSingleLibraryById()
    {
        Library? library = await Library.GetSingleAsync(1);
        if (library != null)
        {
            //...
        }
    }

 2. Récupérer une bibliothèque par son *nom*

Appelez la méthode statique asynchrone  `await Library.GetSingleAsync("nom_de_la_bibliotheque_recuperer");`.

Remarque : le paramètre de la méthode n'est pas sensible à la casse.

    using LibApi.Services.Libraries;
    
    public async void GetSingleLibraryByName()
    {
        Library? library = await Library.GetSingleAsync("Ma_bibliotheque");
        if (library != null)
        {
            //...
        }
    }

### Mettre à jour la bibliothèque

Appelez la méthode asynchrone  `UpdateAsync(["nouveau_nom"], ["nouvelle_description"]);`  de la variable  `library`  .

    using LibApi.Services.Libraries;
    
    public async void NewLibraryAddUpdate()
    {
        Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
        if (library != null)
        {
            await library.UpdateAsync("Capucine 2", "Contient les livres de tante Suzie et tante Anne");
        }
    }

Pour mettre à jour uniquement le nom de la bibliothèque, vous devez renseigner le premier paramètre et garder le second  `null`  _(afin pour d'éviter la modification de la description à votre insu)_.
    
    using LibApi.Services.Libraries;
    
    public async void NewLibraryAddUpdate()
     {
         Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
         if (library != null)
         {
             await library.UpdateAsync("Capucine 2", null);
         }
     }

Pour mettre à jour uniquement sa description, assurez vous de garder le premier paramètre vide _`""`_  ou  `null`  _(afin pour d'éviter la modification du nom à votre insu)_  puis tapez la nouvelle description dans le second paramètre.

    using LibApi.Services.Libraries;
    
    public async void NewLibraryAddUpdate()
    {
        Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
        if (library != null)
        {
            await library.UpdateAsync(null, "Contient les livres de tante Suzie et de tante Anne");
        }
    }

Attention : Si les deux paramètres sont vide ou  `null`  ou que la méthode asynchrone  `DeleteAsync()`  ait été appelée avant celle-ci et qu'elle ait retournée la valeur  `true`, alors une  `NotSupportedException`  est levée et retourne la valeur  `false`.





### Compter le nombre de bibliothèque existant

Appelez la méthode statique asynchrone  `await Library.CountAsync();`.
Cette méthode retourne un objet de type `int`.

    using LibApi.Services.Libraries;
    
    public async void GetCountLibraries()
    {
        int count = await Library.CountAsync();
        Console.WriteLine("Nombre de bibliothèques : " + count);
    }

### Supprimer la bibliothèque

Appelez la méthode asynchrone  `DeleteAsync();`  de la variable  `library`  puis déclarez-là comme  `null`.

    using LibApi.Services.Libraries;
    
    public async void Delete()
    {
        Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
        if (library != null)
        {
            await library.DeleteAsync();
        }
    }

## Les collections

### Ajouter une collection

Ajoutez l'espace de nom  `using LibApi.Services.Collections;`  dans votre fichier ensuite, appelez ou déclarez une variable de type `Library` en créant ou en récupérant une bibliothèque existante avec l'une des méthodes citées ci-dessus ensuite déclarez une autre variable de type `Collection?` nommée `collection` puis appelez la méthode asynchrone  `AddCollectionAsync("nom_de_votre_collection", ["description_de_votre_collection"]);` 

    using LibApi.Services.Libraries;
    using LibApi.Services.Collections;
    
    public async void NewLibraryAndAddCollection()
    {
        Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
        if (library != null)
        {
	        Collection? collection = await library.AddCollectionAsync("Le petit futé", "Coté ");
	        if (collection != null)
	        {
	           //...
	        }
        }
        
    }

### Mettre à jour une collection

Appelez la méthode asynchrone  `UpdateAsync(["nouveau_nom"], ["nouvelle_description"]);`  de la variable  `collection`  .

    using LibApi.Services.Libraries;
    using LibApi.Services.Collections;
    
    public async void NewLibraryAndAddCollectionAndUpdateCollection()
     {
         Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
         if (library != null)
         {
             Collection? collection = await library.AddCollectionAsync("Le petit fute", "Ma description");
             if (collection != null)
             {
                 await collection.UpdateAsync("Moi et les monstre", "La collection des monstres");
             }
         }
     }

Pour mettre à jour uniquement le nom de la collection, vous devez renseigner le premier paramètre et garder le second  `null`  _(afin pour d'éviter la modification de la description à votre insu)_.

    using LibApi.Services.Libraries;
    using LibApi.Services.Collections;
    
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

Pour mettre à jour uniquement sa description, assurez vous de garder le premier paramètre vide  _(`""`)_  ou  `null`  _(afin pour d'éviter la modification du nom à votre insu)_  puis tapez la nouvelle description dans le second paramètre.

    using LibApi.Services.Libraries;
    using LibApi.Services.Collections;
    
    public async void NewLibraryAndAddCollectionAndUpdateCollection()
    {
        Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
        if (library != null)
        {
            Collection? collection = await library.AddCollectionAsync("Le petit fute", "Ma description");
            if (collection != null)
            {
                await collection.UpdateAsync(null, "La collection des monstres");
            }
        }
    }

Attention : Si les deux paramètres sont vide ou  `null`  ou que la méthode asynchrone  `DeleteAsync()`  ait été appelée avant celle-ci et qu'elle ait retournée la valeur  `true`, alors une  `NotSupportedException`  est levée et retourne la valeur  `false`.

### Récupérer le modèle de données de toutes les collections d'une bibliothèque

Appelez ou déclarez une variable de type `Library` en créant ou en récupérant une bibliothèque existante avec l'une des méthodes citées ci-dessus ensuite déclarez une autre variable de type `IEnumerable<Tcollection>?` nommée `collections` puis appelez la méthode asynchrone  `GetAllCollectionsAsync();` de la variable de type `Library`.

    using LibApi.Services.Libraries;
    using LibApi.Services.Collections;
    
    public async void NewLibraryAndGetAllCollections()
    {
        Library? library = await Library.CreateAsync("Capucine", "Contient les livres de tante Suzie");
        if (library != null)
        {
            IEnumerable<Collection> collections = await library.GetAllCollectionsAsync();
            if (collections.Any())
            {
                //...
            }
        }
    }

## Résolution de problèmes 

### Impossible de créer une bibliothèque

Si la méthode `Library.CreateAsync` retourne la valeur  `null`  , alors il est fort probable que le nom de votre bibliothèque ne contient aucun caractère, que la base de données est introuvable ou que son schéma de données ne correspond pas avec celle de l’application ou que la variable  `library`  ait été déclarée  `null`  avant de faire appel à la méthode  `Library.CreateAsync`  .
