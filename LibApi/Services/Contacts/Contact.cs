using AppHelpers;
using AppHelpers.Dates;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared;
using LibShared.ViewModels.Contacts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Services.Contacts
{
    public sealed class Contact : ContactVM, IDisposable
    {
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }

        LibrarySqLiteDbContext context = new ();

        private Contact()
        {

        }

        #region CRUD
        /// <summary>
        /// Ajoute un nouveau contact dans la base de données puis retourne un objet <see cref="Contact"/>
        /// </summary>
        /// <param name="fullName">Nom complet de la société ou de la personne. Les formats pour la personne : [titreCivilite] prenom nom autresPrenoms</param>
        /// <param name="description">Description du contact</param>
        /// <param name="contactType">Type de contact</param>
        /// <param name="openIfExist">Obtient le contact existant au lieu de générer une erreur</param>
        /// <returns></returns>
        public static async Task<Contact?> CreateAsync(string fullName, string? description = null, ContactType contactType = ContactType.Human, bool openIfExist = false)
        {
            try
            {
                if (fullName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(fullName), "Le nom du contact ou de la société ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                ContactVM contactVM = new()
                {
                    ContactType = contactType,
                };

                if (contactType == ContactType.Society)
                {
                    contactVM.SocietyName = fullName.Trim();
                }
                else if (contactType == ContactType.Human)
                {
                    foreach (string civility in CivilityHelpers.CiviliteListAll())
                    {
                        bool isMatch = false;
                        if (fullName.Contains(civility))
                        {
                            fullName = fullName.Replace(civility, "");
                            isMatch = true;
                        }
                        else if (fullName.ToLower().Contains(civility.ToLower()))
                        {
                            fullName = fullName.Replace(civility.ToLower(), "");
                            isMatch = true;
                        }

                        if (isMatch)
                        {
                            contactVM.TitreCivilite = civility;
                            break;
                        }
                    }

                    var split = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (split.Length == 1)
                    {
                        contactVM.Prenom = split[0].Trim();
                    }
                    else if (split.Length == 2)
                    {
                        contactVM.Prenom = split[0].Trim();
                        contactVM.NomNaissance = split[1].Trim();
                    }
                    else if (split.Length > 2)
                    {
                        contactVM.Prenom = split[0].Trim();
                        contactVM.NomNaissance = split[1].Trim();

                        StringBuilder stringBuilder = new ();
                        for (int i = 2; i < split.Length + 1; i++)
                            stringBuilder.Append($"{split[i]} ");
                        
                        contactVM.AutresPrenoms = stringBuilder.ToString().Trim();
                    }
                }

                return await CreateAsync(contactVM, openIfExist);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(CreateAsync), ex);
                return null;
            }
        }

        /// <summary>
        /// Ajoute un contact dans la base de données
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="openIfExist"></param>
        /// <returns></returns>
        public static async Task<Contact?> CreateAsync(ContactVM viewModel, bool openIfExist = false)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel), $"Le modèle de vue du contact ne peut être null.");
                }


                if (viewModel.ContactType == ContactType.Human)
                {
                    if (viewModel.TitreCivilite.IsStringNullOrEmptyOrWhiteSpace() ||
                    viewModel.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() ||
                    viewModel.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        throw new ArgumentNullException(nameof(viewModel), "Le nom de naissance et le prénom du contact doivent au moins être renseignés.");
                    }
                }
                else if (viewModel.ContactType == ContactType.Society)
                {
                    if (viewModel.SocietyName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        throw new ArgumentNullException(nameof(viewModel), "Le nom de la société ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                    }
                }

                Tcontact? existingItem = await GetContactIfExistAsync(viewModel);
                if (existingItem != null)
                {
                    Logs.Log(nameof(Contact), nameof(CreateAsync), $"Le contact : \"{viewModel.NomNaissance} {viewModel.Prenom}\" ou la société: \"{viewModel.SocietyName}\" existe déjà.");
                    if (openIfExist)
                    {
                        return ConvertToViewModel(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Le contact : \"{viewModel.NomNaissance} {viewModel.Prenom}\" ou la société: \"{viewModel.SocietyName}\" existe déjà.");
                    }
                }

                var _dateAjout = DateTime.UtcNow;
                var _guid = System.Guid.NewGuid();

                Tcontact record = new ()
                {
                    Guid = _guid.ToString(),
                    DateAjout = _dateAjout.ToString(),
                    DateEdition = null,
                    Observation = viewModel.Observation,
                    TitreCivilite = viewModel.TitreCivilite,
                    NomNaissance = viewModel.NomNaissance,
                    NomUsage = viewModel.NomUsage,
                    Prenom = viewModel.Prenom,
                    AutresPrenoms = viewModel.AutresPrenoms,
                    AdressPostal = viewModel.AdressePostal,
                    CodePostal = viewModel.CodePostal,
                    Ville = viewModel.Ville,
                    NoMobile = viewModel.NoMobile,
                    NoTelephone = viewModel.NoTelephone,
                    MailAdress = viewModel.AdresseMail,
                    DateNaissance = viewModel.DateNaissance?.ToString(),
                    Nationality = viewModel.Nationality,
                    SocietyName = viewModel.SocietyName,
                    ContactType = (long)viewModel.ContactType,
                };

                await context.Tcontacts.AddAsync(record);
                await context.SaveChangesAsync();

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(CreateAsync), ex);
                return null;
            }
        }

        public async Task<bool> AddOrUpdateAdressAsync(string? codePostal = "", string? ville = "", string adressPostal = "", string? noFixe = "", string? noMobile = "", string? email = "")
        {
            try
            {
                if (codePostal == null && ville == null && adressPostal == null && adressPostal == null && noFixe == null && noMobile == null && email == null)
                {
                    throw new InvalidOperationException("Tous les paramètres ne peuvent pas être null.");
                }

                Tcontact? tcontact = await context.Tcontacts.SingleOrDefaultAsync(s => s.Id == Id);
                if (tcontact == null)
                {
                    throw new ArgumentNullException(nameof(tcontact), $"Le contact n'existe pas avec l'id \"{Id}\".");
                }

                if (codePostal != null)
                {
                    tcontact.CodePostal = codePostal.Trim();
                }

                if (ville != null)
                {
                    tcontact.Ville = ville.Trim();
                }

                if (adressPostal != null)
                {
                    tcontact.AdressPostal = adressPostal.Trim();
                }

                if (noFixe != null)
                {
                    tcontact.NoTelephone = noFixe.Trim();
                }

                if (noMobile != null)
                {
                    tcontact.NoMobile = noMobile.Trim();
                }

                if (email != null)
                {
                    tcontact.MailAdress = email.Trim();
                }

                context.Tcontacts.Update(tcontact);
                _ = await context.SaveChangesAsync();

                if (codePostal != null)
                {
                    CodePostal = codePostal.Trim();
                }

                if (ville != null)
                {
                    Ville = ville.Trim();
                }

                if (adressPostal != null)
                {
                    AdressePostal = adressPostal.Trim();
                }

                if (noFixe != null)
                {
                    NoTelephone = noFixe.Trim();
                }

                if (noMobile != null)
                {
                    NoMobile = noMobile.Trim();
                }

                if (email != null)
                {
                    AdresseMail = email.Trim();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(AddOrUpdateAdressAsync), ex);
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
                if (IsDeleted)
                {
                    throw new NotSupportedException($"Le contact a déjà été supprimée.");
                }

                using LibrarySqLiteDbContext context = new();

                Tcontact? record = await context.Tcontacts.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(record), $"Le contact n'existe pas avec l'id \"{Id}\".");
                }

                List<TbookContactRoleConnector> tbookContactRoleConnector = await context.TbookContactRoleConnectors.Where(s => s.IdContact == Id).ToListAsync();
                if (tbookContactRoleConnector.Any())
                {
                    context.TbookContactRoleConnectors.RemoveRange(tbookContactRoleConnector);
                }

                context.Tcontacts.Remove(record);
                _ = await context.SaveChangesAsync();
                IsDeleted = true;

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion

        public static async Task<Tcontact?> GetContactIfExistAsync(ContactVM viewModel, bool isEdit = false, long? editingId = null)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel), $"Le modèle de vue du contact ne peut être null.");
                }

                using LibrarySqLiteDbContext context = new();
                List<Tcontact> existingItemList = new ();

                if (!isEdit || editingId == null)
                {
                    existingItemList = await context.Tcontacts.ToListAsync();
                }
                else
                {
                    existingItemList = await context.Tcontacts.Where(c => c.Id != (long)editingId).ToListAsync();
                }

                if (existingItemList != null && existingItemList.Any())
                {
                    string? titreCivilite = viewModel.TitreCivilite?.Trim()?.ToLower();
                    string? nomNaissance = viewModel.NomNaissance?.Trim()?.ToLower();
                    string? prenom = viewModel.Prenom?.Trim()?.ToLower();
                    string? autrePrenom = viewModel.AutresPrenoms?.Trim()?.ToLower();
                    string? nomUsage = viewModel.NomUsage?.Trim()?.ToLower();
                    string? societyName = viewModel.SocietyName?.Trim()?.ToLower();

                    foreach (var item in existingItemList)
                    {
                        //Si personne Physique
                        if (viewModel.ContactType == ContactType.Human)
                        {
                            if (item.TitreCivilite?.ToLower() == titreCivilite && item.NomNaissance?.ToLower() == nomNaissance &&
                            item.Prenom?.ToLower() == prenom && item.AutresPrenoms?.ToLower() == autrePrenom &&
                            item.NomUsage?.ToLower() == nomUsage)
                            {
                                return item;
                            }
                        }
                        //Si personne Morale
                        else if (viewModel.ContactType == ContactType.Society)
                        {
                            if (item.SocietyName?.ToLower() == societyName)
                            {
                                return item;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(GetContactIfExistAsync), ex);
                return null;
            }
        }

        public static Contact? ConvertToViewModel(Tcontact model)
        {
            try
            {
                if (model == null) return null;

                var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                if (isGuidCorrect == false) return null;

                Contact viewModel = new ()
                {
                    Id = model.Id,
                    Guid = isGuidCorrect ? guid : Guid.Empty,
                    ContactType = (ContactType)model.ContactType,
                    DateAjout = DateHelpers.Converter.GetDateFromString(model.DateAjout),
                    DateEdition = DateHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                    Observation = model.Observation,
                    TitreCivilite = model.TitreCivilite,
                    NomNaissance = model.NomNaissance,
                    NomUsage = model.NomUsage,
                    Prenom = model.Prenom,
                    AutresPrenoms = model.AutresPrenoms,
                    AdressePostal = model.AdressPostal,
                    CodePostal = model.CodePostal,
                    Ville = model.Ville,
                    NoMobile = model.NoMobile,
                    NoTelephone = model.NoTelephone,
                    AdresseMail = model.MailAdress,
                    DateNaissance = DateHelpers.Converter.GetNullableDateFromString(model.DateNaissance),
                    SocietyName = model.SocietyName,
                    Nationality = model.Nationality,
                };

                //if (model.TcontactRole == null || !model.TcontactRole.Any())
                //{
                //    await CompleteModelInfos(model);
                //}

                //if (model.TcontactRole != null && model.TcontactRole.Count > 0)
                //{
                //    viewModel.ContactRoles = new ObservableCollection<ContactRole>(model.TcontactRole.Select(s => (ContactRole)s.Role));
                //}

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(ConvertToViewModel), ex);
                return null;
            }
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
