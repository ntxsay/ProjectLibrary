# Fonctionnement de la LibAPI

LibAPI (de Library Application Programing Interface) est une bibliothèque de classe qui vous permet de créer et de gérer une ou des bibliothèques de livres dans une base de données SQLite.

## Gestion d'une bibliothèque 

### Créer une bibliothèque

Tapez l'espace de nom `using LibApi.Services.Libraries;` dans votre fichier ensuite déclarez une variable de type `Library?` nommée `library`  puis appelez la méthode asynchrone statique `await Library.CreateAsync("nom_de_votre_bibliotheque", ["description_de_votre_description"]);` dans une méthode asynchrone.

Si la méthode retourne la valeur `null` , alors il est fort probable que le nom de votre bibliothèque ne contient aucun caractère, que la base de données est introuvable   ou que son schéma de données ne correspond pas avec celle de l’application ou que la variable `library` ait été déclarée `null` avant de faire appel à la méthode `Library.CreateAsync` .

### Mettre à jour la bibliothèque

Appelez la méthode asynchrone `UpdateAsync(["nouveau_nom"], ["nouvelle_description"]);` de la variable `library` .

Pour mettre à jour uniquement le nom de la bibliothèque, vous devez renseigner le premier paramètre et garder le second `null` *(afin pour d'éviter la modification de la description à votre insu)*.

Pour mettre à jour uniquement la description, assurez-vous de garder le premier paramètre vide *(`""`)* ou `null` *(afin pour d'éviter la modification du nom à votre insu)* puis tapez la nouvelle description dans le second paramètre.

Attention : Si les deux paramètres sont vide ou `null` ou que la méthode asynchrone `DeleteAsync()` ait été appelée avant celle-ci et qu'elle ait retournée la valeur `true`, alors une `NotSupportedException` est levée et retourne la valeur `false`.

### Supprimer la bibliothèque 

Appelez la méthode asynchrone `DeleteAsync();` de la variable `library` puis déclarez-là comme `null`.
