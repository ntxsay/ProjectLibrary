using AppHelpers;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared.ViewModels.Contacts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Services.Contacts
{
    public class Contact : ContactVM, IDisposable
    {
        private bool disposedValue;
        LibrarySqLiteDbContext context = new();

        /// <summary>
        /// Initialise une nouvelle instance de l'objet <see cref="Contact"/> afin créer un nouveau contact puis d'interagir avec lui.
        /// </summary>
        /// <remarks>Remarque : Pour ajouter le contact dans la base de données, appelez la méthode <see cref="CreateAsync"/></remarks>
        public Contact(string nomNaissance, string prenom, string? autresPrenoms = null)
        {
            NomNaissance = nomNaissance.Trim();
            Prenom = prenom.Trim();
            AutresPrenoms = autresPrenoms?.Trim();
            ContactType = LibShared.ContactType.Human;
        }

        /// <summary>
        /// Initialise une nouvelle instance de l'objet <see cref="Contact"/> afin créer un nouveau contact puis d'interagir avec lui.
        /// </summary>
        /// <remarks>Remarque : Pour ajouter le contact dans la base de données, appelez la méthode <see cref="CreateAsync"/></remarks>
        public Contact(string societyName)
        {
            SocietyName = societyName.Trim();
            ContactType = LibShared.ContactType.Society;
        }

        #region CRUD
        /// <summary>
        /// Ajoute un contact dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAsync()
        {
            try
            {
                if (ContactType == LibShared.ContactType.Human)
                {
                    if (NomNaissance.IsStringNullOrEmptyOrWhiteSpace() || Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        throw new ArgumentNullException(nameof(Name), "Les informations minimales obligatoires à renseigner sont : le nom de naissance et le prénom.");
                    }
                }
                else if (ContactType == LibShared.ContactType.Society)
                {
                    if (SocietyName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        throw new ArgumentNullException(nameof(Name), "Les informations minimales obligatoires à renseigner est : le nom de la société.");
                    }
                }

                bool isExist = await context.TcontactTypes.AnyAsync(c => c.Name.ToLower() == Name.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(Contact), nameof(CreateAsync), "Ce type existe déjà");
                    return true;
                }

                TcontactType TcontactType = new()
                {
                    Name = Name,
                    Description = Description,
                };

                await context.TcontactTypes.AddAsync(TcontactType);
                await context.SaveChangesAsync();

                Id = TcontactType.Id;

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Contact), nameof(CreateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Contact), nameof(CreateAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(CreateAsync), ex);
                return false;
            }
        }

        /// <summary>
        /// Met à jour le type dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateNameAsync(string value)
        {
            try
            {
                if (value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(value), "Le nom du type ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                TcontactType? TcontactType = await context.TcontactTypes.SingleOrDefaultAsync(s => s.Id == Id);
                if (TcontactType == null)
                {
                    throw new ArgumentNullException(nameof(TcontactType), $"La type n'existe pas avec l'id \"{Id}\".");
                }

                bool isExist = await context.TcontactTypes.AnyAsync(c => c.Id != Id && c.Name.ToLower() == value.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(Contact), nameof(UpdateNameAsync), "Ce type existe déjà");
                    return false;
                }

                TcontactType.Name = value.Trim();

                context.TcontactTypes.Update(TcontactType);
                _ = await context.SaveChangesAsync();

                Name = TcontactType.Name;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateNameAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateNameAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateNameAsync), ex);
                return false;
            }
        }

        public async Task<bool> UpdateDescriptionAsync(string? value = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                TcontactType? TcontactType = await context.TcontactTypes.SingleOrDefaultAsync(s => s.Id == Id);
                if (TcontactType == null)
                {
                    throw new ArgumentNullException(nameof(TcontactType), $"Le type n'existe pas avec l'id \"{Id}\".");
                }

                TcontactType.Description = value?.Trim();

                context.TcontactTypes.Update(TcontactType);
                _ = await context.SaveChangesAsync();

                Description = TcontactType.Description;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
        }


        /// <summary>
        /// Supprime la bibliothèque de la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                TcontactType? TcontactType = await context.TcontactTypes.SingleOrDefaultAsync(s => s.Id == Id);
                if (TcontactType == null)
                {
                    throw new ArgumentNullException(nameof(TcontactType), $"Le type n'existe pas avec l'id \"{Id}\".");
                }

                context.TcontactTypes.Remove(TcontactType);
                _ = await context.SaveChangesAsync();

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Contact), nameof(DeleteAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Contact), nameof(DeleteAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion

        public async Task<Tuple<bool, long>> IsContactExistAsync(bool isEdit = false, long? modelId = null)
        {
            try
            {
                List<Tcontact> existingItemList = new List<Tcontact>();

                if (!isEdit || modelId == null)
                {
                    existingItemList = await context.Tcontacts.ToListAsync();
                }
                else
                {
                    existingItemList = await context.Tcontacts.Where(c => c.Id != (long)modelId).ToListAsync();
                }

                if (existingItemList != null && existingItemList.Any())
                {
                    string? titreCivilite = TitreCivilite?.Trim()?.ToLower();
                    string? nomNaissance = NomNaissance?.Trim()?.ToLower();
                    string? prenom = Prenom?.Trim()?.ToLower();
                    string? autrePrenom = AutresPrenoms?.Trim()?.ToLower();
                    string? nomUsage = NomUsage?.Trim()?.ToLower();
                    string? societyName = SocietyName?.Trim()?.ToLower();

                    foreach (var item in existingItemList)
                    {
                        //Si personne Physique
                        if (ContactType == LibShared.ContactType.Human)
                        {
                            if (item.TitreCivilite?.ToLower() == titreCivilite && item.NomNaissance?.ToLower() == nomNaissance &&
                            item.Prenom?.ToLower() == prenom && item.AutresPrenoms?.ToLower() == autrePrenom &&
                            item.NomUsage?.ToLower() == nomUsage)
                            {
                                return new Tuple<bool, long>(true, item.Id);
                            }
                        }
                        //Si personne Morale
                        else if (ContactType == LibShared.ContactType.Society)
                        {
                            if (item.SocietyName?.ToLower() == societyName)
                            {
                                return new Tuple<bool, long>(true, item.Id);
                            }
                        }
                    }
                }

                return new Tuple<bool, long>(false, 0);

            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(DeleteAsync), ex);
                return new Tuple<bool, long>(false, 0);
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés)
                    context.Dispose();
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~Contact()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
