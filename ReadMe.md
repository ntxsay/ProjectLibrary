
# La LibAPI

LibAPI (Library Application Programing Interface) est une bibliothèque de classe développée avec C# .NET 6.0 qui vous permet de créer et de gérer une ou des bibliothèques de livres dans une base de données SQLite.

## Introduction/Contexte

Physiquement parlant, une bibliothèque peut correspondre à un meuble qui permet de ranger et de classer des livres. De la même façon la LibAPI va vous permettre de créer l'objet meuble dans lequel il vous sera permis d'y ajouter des livres puis de les ranger et de les classer.

La collection est constituée par un ensemble de livres d’un même éditeur qui ont des points communs comme la présentation (format, couleurs, taille…), ou les types de lecteurs (âge, goûts…).  La collection porte un titre. Par exemple : _Chair de poule_, _Scripto_... ([pour plus d'infos](http://moncdivirtuel.free.fr/recherche_documentaire/cles_du_livre/cles_du_livre.html)).

## Les bibliothèques 

### Créer une bibliothèque

Tapez l'espace de nom  `using LibApi.Services.Libraries;`  dans votre fichier ensuite déclarez une variable de type  `Library?`  nommée  `library`  puis appelez la méthode asynchrone statique  `await Library.CreateAsync("nom_de_votre_bibliotheque", ["description_de_votre_description"]);`  dans une méthode asynchrone.

Si la méthode retourne la valeur  `null`  , alors il est fort probable que le nom de votre bibliothèque ne contient aucun caractère, que la base de données est introuvable ou que son schéma de données ne correspond pas avec celle de l’application ou que la variable  `library`  ait été déclarée  `null`  avant de faire appel à la méthode  `Library.CreateAsync`  .

### Mettre à jour la bibliothèque

Appelez la méthode asynchrone  `UpdateAsync(["nouveau_nom"], ["nouvelle_description"]);`  de la variable  `library`  .

Pour mettre à jour uniquement le nom de la bibliothèque, vous devez renseigner le premier paramètre et garder le second  `null`  _(afin pour d'éviter la modification de la description à votre insu)_.

Pour mettre à jour uniquement sa description, assurez-vous de garder le premier paramètre vide  _(`""`)_  ou  `null`  _(afin pour d'éviter la modification du nom à votre insu)_  puis tapez la nouvelle description dans le second paramètre.

Attention : Si les deux paramètres sont vide ou  `null`  ou que la méthode asynchrone  `DeleteAsync()`  ait été appelée avant celle-ci et qu'elle ait retournée la valeur  `true`, alors une  `NotSupportedException`  est levée et retourne la valeur  `false`.

### Récupérer le modèle de données de toutes les bibliothèques

Appelez la méthode statique asynchrone  `await Library.GetAllAsync();`.
Cette méthode retourne  un objet `IEnumerable<Tlibrary>`.

### Récupérer le modèle de données d'une bibliothèque existante

Appelez la méthode statique asynchrone  `await Library.GetSingleAsync(id_bibliotheque_a_recuperer);`.
Cette méthode retourne un objet `Tlibrary?`.

### Compter le nombre de bibliothèque existant

Appelez la méthode statique asynchrone  `await Library.CountAsync();`.
Cette méthode retourne un objet de type `int`.

### Supprimer la bibliothèque

Appelez la méthode asynchrone  `DeleteAsync();`  de la variable  `library`  puis déclarez-là comme  `null`.

## Les collections

### Ajouter une collection

Appelez ou déclarez une variable de type `Library` en créant ou en récupérant une bibliothèque existante avec l'une des méthodes citées ci-dessus ensuite déclarez une autre variable de type `Collection?` nommée `collection` puis appelez la méthode asynchrone  `AddCollectionAsync("nom_de_votre_collection", ["description_de_votre_collection"]);` 

### Mettre à jour une collection

Appelez la méthode asynchrone  `UpdateAsync(["nouveau_nom"], ["nouvelle_description"]);`  de la variable  `collection`  .

Pour mettre à jour uniquement le nom de la collection, vous devez renseigner le premier paramètre et garder le second  `null`  _(afin pour d'éviter la modification de la description à votre insu)_.

Pour mettre à jour uniquement sa description, assurez-vous de garder le premier paramètre vide  _(`""`)_  ou  `null`  _(afin pour d'éviter la modification du nom à votre insu)_  puis tapez la nouvelle description dans le second paramètre.

Attention : Si les deux paramètres sont vide ou  `null`  ou que la méthode asynchrone  `DeleteAsync()`  ait été appelée avant celle-ci et qu'elle ait retournée la valeur  `true`, alors une  `NotSupportedException`  est levée et retourne la valeur  `false`.

### Récupérer le modèle de données de toutes les collections d'une bibliothèque

Appelez ou déclarez une variable de type `Library` en créant ou en récupérant une bibliothèque existante avec l'une des méthodes citées ci-dessus ensuite déclarez une autre variable de type `IEnumerable<Tcollection>` nommée `collections` puis appelez la méthode asynchrone  `GetAllCollectionsAsync();` de la variable de type `Library`.
