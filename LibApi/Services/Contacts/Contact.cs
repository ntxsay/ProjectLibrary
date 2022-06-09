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
        public static async Task<Contact?> CreateAsync(string? titreCivilite, string nomNaissance, string prenom, string? autresPrenoms = null, string? nomUsage = null, DateTime? dateNaissance = null, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (nomNaissance == "" || prenom == "")
                {
                    throw new InvalidOperationException("Le nom de famille et le prénom doiven être renseignés ou gardés null.");
                }

                ContactVM contactVM = new()
                {
                    ContactType = ContactType.Human,
                    TitreCivilite = titreCivilite,
                    NomNaissance = nomNaissance,
                    NomUsage = nomUsage,
                    Prenom = prenom,
                    AutresPrenoms = autresPrenoms,
                    DateNaissance = dateNaissance,
                    Observation = description
                };

                return await CreateAsync(contactVM, openIfExist);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(CreateAsync), ex);
                return null;
            }
        }

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
                        for (int i = 2; i < split.Length; i++)
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

                using LibrarySqLiteDbContext context = new();

                Tcontact? existingItem = null;

                if (viewModel.ContactType == ContactType.Human)
                {
                    if (viewModel.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() || viewModel.Prenom.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        throw new ArgumentNullException(nameof(viewModel), "Le nom de naissance et le prénom du contact doivent au moins être renseignés.");
                    }

                    existingItem = await GetPersonIfExistAsync(context: context, _titreCivilite: viewModel.TitreCivilite, _nomNaissance: viewModel.NomNaissance, _nomUsage: viewModel.NomUsage, _prenom: viewModel.Prenom, _autresPrenoms: viewModel.AutresPrenoms);
                }
                else if (viewModel.ContactType == ContactType.Society)
                {
                    if (viewModel.SocietyName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        throw new ArgumentNullException(nameof(viewModel), "Le nom de la société ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                    }

                    existingItem = await GetSocietyIfExistAsync(context: context, _societyName: viewModel.SocietyName)!;
                }

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
                    Observation = viewModel.Description,
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

        public async Task<bool> UpdateCivilityAsync(string? titreCivilite = null, string? nomNaissance = null, string? nomUsage = null , string? prenom = null, string? autresPrenoms = null, DateTime? dateNaissance = null, string? newDescription = null)
        {
            try
            {
                if (ContactType != ContactType.Human)
                {
                    throw new InvalidOperationException($"Impossible de d'éditer une société dans cette méthode. Appelez la méthode {nameof(UpdateSocietyAsync)}.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (nomNaissance == "" || prenom == "")
                {
                    throw new InvalidOperationException("Le nom de famille et le prénom doiven être renseignés ou gardés null.");
                }

                Tcontact? tcontact = await context.Tcontacts.SingleOrDefaultAsync(s => s.Id == Id);
                if (tcontact == null)
                {
                    throw new ArgumentNullException(nameof(tcontact), $"Le contact (personne) n'existe pas avec l'id \"{Id}\".");
                }

                if (!titreCivilite.IsStringNullOrEmptyOrWhiteSpace() || !nomNaissance.IsStringNullOrEmptyOrWhiteSpace() || !nomUsage.IsStringNullOrEmptyOrWhiteSpace() || !prenom.IsStringNullOrEmptyOrWhiteSpace() || !autresPrenoms.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Tcontact? existingItem = await GetPersonIfExistAsync(context, titreCivilite, nomNaissance, nomUsage, prenom, autresPrenoms, true, Id);
                    if (existingItem != null)
                    {
                        Logs.Log(nameof(Contact), nameof(UpdateCivilityAsync), "Ce contact (personne) existe déjà");
                        return false;
                    }
                }

                if (titreCivilite != null)
                {
                    tcontact.TitreCivilite = titreCivilite.Trim();
                }

                if (!nomNaissance.IsStringNullOrEmptyOrWhiteSpace())
                {
                    tcontact.NomNaissance = nomNaissance.Trim();
                }

                if (nomUsage != null)
                {
                    tcontact.NomUsage = nomUsage.Trim();
                }

                if (!prenom.IsStringNullOrEmptyOrWhiteSpace())
                {
                    tcontact.Prenom = prenom.Trim();
                }

                if (autresPrenoms != null)
                {
                    tcontact.AutresPrenoms = autresPrenoms.Trim();
                }

                if (dateNaissance != null)
                {
                    tcontact.DateNaissance = dateNaissance.Value.ToString("d");
                }

                if (newDescription != null)
                {
                    tcontact.Observation = newDescription.Trim();
                }

                DateTime dateEdition = DateTime.Now;
                tcontact.DateEdition = dateEdition.ToString();

                context.Tcontacts.Update(tcontact);
                await context.SaveChangesAsync();

                DateEdition = dateEdition;

                if (titreCivilite != null)
                {
                    TitreCivilite = titreCivilite.Trim();
                }

                if (!nomNaissance.IsStringNullOrEmptyOrWhiteSpace())
                {
                    NomNaissance = nomNaissance.Trim();
                }

                if (nomUsage != null)
                {
                    NomUsage = nomUsage.Trim();
                }

                if (!prenom.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Prenom = prenom.Trim();
                }

                if (autresPrenoms != null)
                {
                    AutresPrenoms = autresPrenoms.Trim();
                }

                if (dateNaissance != null)
                {
                    DateNaissance = new DateTimeOffset(dateNaissance.Value);
                }

                if (newDescription != null)
                {
                    Observation = newDescription.Trim();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateCivilityAsync), ex);
                return false;
            }
        }

        public async Task<bool> UpdateSocietyAsync(string? newName, string? newDescription = null)
        {
            try
            {
                if (ContactType != ContactType.Society)
                {
                    throw new InvalidOperationException($"Impossible de d'éditer une personne dans cette méthode. Appelez la méthode {nameof(UpdateCivilityAsync)}.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
                {
                    throw new InvalidOperationException("Le nouveau nom de la société ou sa nouvelle description doit être renseignée.");
                }

                Tcontact? tcontact = await context.Tcontacts.SingleOrDefaultAsync(s => s.Id == Id);
                if (tcontact == null)
                {
                    throw new ArgumentNullException(nameof(tcontact), $"Le contact (société) n'existe pas avec l'id \"{Id}\".");
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    bool isExist = await context.Tcontacts.AnyAsync(c => c.Id != Id && c.SocietyName != null && c.SocietyName.ToLower() == newName.Trim().ToLower())!;
                    if (isExist)
                    {
                        Logs.Log(nameof(Contact), nameof(UpdateSocietyAsync), "Ce contact (société) existe déjà");
                        return false;
                    }
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    tcontact.SocietyName = newName.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    tcontact.Observation = newDescription.Trim();
                }

                DateTime dateEdition = DateTime.Now;
                tcontact.DateEdition = dateEdition.ToString();

                context.Tcontacts.Update(tcontact);
                await context.SaveChangesAsync();

                DateEdition = dateEdition;

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    SocietyName = tcontact.SocietyName;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    Description = tcontact.Observation;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(UpdateSocietyAsync), ex);
                return false;
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

                DateTime dateEdition = DateTime.Now;
                tcontact.DateEdition = dateEdition.ToString();

                context.Tcontacts.Update(tcontact);
                _ = await context.SaveChangesAsync();

                DateEdition = dateEdition;

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

        #region Méthodes statiques
        /// <summary>
            /// Obtient la liste de tous les contacts
            /// </summary>
            /// <param name="contactType"></param>
            /// <returns></returns>
        public static async Task<IEnumerable<Contact>> GetAllAsync(ContactType? contactType = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                List<Tcontact>? tcontacts = null;
                if (contactType == null)
                {
                    tcontacts = await context.Tcontacts.ToListAsync();
                }
                else
                {
                    tcontacts = await context.Tcontacts.Where(w => w.ContactType == (byte)contactType).ToListAsync();
                }

                if (tcontacts != null && tcontacts.Any())
                {
                    return tcontacts.Select(s => ConvertToViewModel(s)).Where(w => w != null) ?? Enumerable.Empty<Contact>()!;
                }

                return Enumerable.Empty<Contact>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(GetPersonIfExistAsync), ex);
                return Enumerable.Empty<Contact>();
            }
        }

        /// <summary>
        /// Obtient la liste de tous les contacts
        /// </summary>
        /// <param name="contactType"></param>
        /// <returns></returns>
        public static async Task<Contact?> GetSingleAsync(long idContact)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                Tcontact? tcontact = await context.Tcontacts.SingleOrDefaultAsync(s => s.Id == idContact);
                if (tcontact == null)
                {
                    return null;
                }

                return ConvertToViewModel(tcontact);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(GetPersonIfExistAsync), ex);
                return null;
            }
        }

        private static async Task<Tcontact?> GetPersonIfExistAsync(LibrarySqLiteDbContext context, string? _titreCivilite = null, string? _nomNaissance = null, string? _nomUsage = null, string? _prenom = null, string? _autresPrenoms = null, bool isEdit = false, long? editingId = null)
        {
            try
            {
                if (_nomNaissance == "" || _prenom == "")
                {
                    throw new InvalidOperationException("Le nom de famille et le prénom doiven être renseignés.");
                }

                List<Tcontact> existingItemList = new();

                if (!isEdit || editingId == null)
                {
                    existingItemList = await context.Tcontacts.Where(w => w.ContactType == (byte)ContactType.Human).ToListAsync();
                }
                else
                {
                    existingItemList = await context.Tcontacts.Where(c => c.ContactType == (byte)ContactType.Human && c.Id != (long)editingId).ToListAsync();
                }

                if (existingItemList != null && existingItemList.Any())
                {
                    string? titreCivilite = _titreCivilite?.Trim()?.ToLower();
                    string? nomNaissance = _nomNaissance?.Trim()?.ToLower();
                    string? prenom = _prenom?.Trim()?.ToLower();
                    string? autrePrenom = _autresPrenoms?.Trim()?.ToLower();
                    string? nomUsage = _nomUsage?.Trim()?.ToLower();

                    foreach (var item in existingItemList)
                    {
                        if (item.TitreCivilite?.ToLower() == titreCivilite && item.NomNaissance?.ToLower() == nomNaissance &&
                            item.Prenom?.ToLower() == prenom && item.AutresPrenoms?.ToLower() == autrePrenom &&
                            item.NomUsage?.ToLower() == nomUsage)
                        {
                            return item;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(GetPersonIfExistAsync), ex);
                return null;
            }
        }

        private static async Task<Tcontact?> GetSocietyIfExistAsync(LibrarySqLiteDbContext context, string _societyName, bool isEdit = false, long? editingId = null)
        {
            try
            {
                if (_societyName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new InvalidOperationException("Le nom de le société doit être renseigné.");
                }

                List<Tcontact> existingItemList = new();

                if (!isEdit || editingId == null)
                {
                    existingItemList = await context.Tcontacts.Where(w => w.ContactType == (byte)ContactType.Society).ToListAsync();
                }
                else
                {
                    existingItemList = await context.Tcontacts.Where(c => c.ContactType == (byte)ContactType.Society && c.Id != (long)editingId).ToListAsync();
                }

                if (existingItemList != null && existingItemList.Any())
                {
                    string? societyName = _societyName?.Trim()?.ToLower();

                    foreach (var item in existingItemList)
                    {
                        if (item.SocietyName?.ToLower() == _societyName)
                        {
                            return item;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(GetSocietyIfExistAsync), ex);
                return null;
            }
        }

        public static string? DisplayName(Tcontact model, DisplayCivility displayCivility = DisplayCivility.PrenomAutresprenomsNomnaissanceNomusage)
        {
            try
            {
                if (model == null)
                {
                    return null;
                }

                string? value = null;
                string titrecivility = !model.TitreCivilite.IsStringNullOrEmptyOrWhiteSpace() ? model.TitreCivilite + " " : "";
                string prenom = !model.Prenom.IsStringNullOrEmptyOrWhiteSpace() ? model.Prenom + " " : "";
                string autresPrenoms = !model.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace() ? model.AutresPrenoms + " " : "";
                string nomNaissance = !model.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() ? model.NomNaissance + " " : "";
                string nomUsage = !model.NomUsage.IsStringNullOrEmptyOrWhiteSpace() ? model.NomUsage + " " : "";

                var contactType = (ContactType)model.ContactType;
                if (contactType == ContactType.Human)
                {
                    switch (displayCivility)
                    {
                        case DisplayCivility.TitreNomnaissancePrenom:
                            value = $"{titrecivility}{nomNaissance}{prenom?.Trim()}";
                            break;
                        case DisplayCivility.PrenomAutresprenomsNomnaissanceNomusage:
                            value = $"{prenom}{autresPrenoms}{nomNaissance}{nomUsage?.Trim()}";
                            break;
                        case DisplayCivility.NomnaissanceNomusagePrenomAutresprenoms:
                            value = $"{nomNaissance}{nomUsage}{prenom}{autresPrenoms?.Trim()}";
                            break;
                        default:
                            break;
                    }
                }
                else if (contactType == ContactType.Society)
                {
                    value = model.SocietyName;

                }

                return value?.Trim();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(DisplayName), ex);
                return null;
            }
        }

        public static string? DisplayName(ContactVM model, DisplayCivility displayCivility = DisplayCivility.PrenomAutresprenomsNomnaissanceNomusage)
        {
            try
            {
                if (model == null)
                {
                    return null;
                }

                string? value = null;
                string titrecivility = !model.TitreCivilite.IsStringNullOrEmptyOrWhiteSpace() ? model.TitreCivilite + " " : "";
                string prenom = !model.Prenom.IsStringNullOrEmptyOrWhiteSpace() ? model.Prenom + " " : "";
                string autresPrenoms = !model.AutresPrenoms.IsStringNullOrEmptyOrWhiteSpace() ? model.AutresPrenoms + " " : "";
                string nomNaissance = !model.NomNaissance.IsStringNullOrEmptyOrWhiteSpace() ? model.NomNaissance + " " : "";
                string nomUsage = !model.NomUsage.IsStringNullOrEmptyOrWhiteSpace() ? model.NomUsage + " " : "";

                if (model.ContactType == ContactType.Human)
                {
                    switch (displayCivility)
                    {
                        case DisplayCivility.TitreNomnaissancePrenom:
                            value = $"{titrecivility}{nomNaissance}{prenom?.Trim()}";
                            break;
                        case DisplayCivility.PrenomAutresprenomsNomnaissanceNomusage:
                            value = $"{prenom}{autresPrenoms}{nomNaissance}{nomUsage?.Trim()}";
                            break;
                        case DisplayCivility.NomnaissanceNomusagePrenomAutresprenoms:
                            value = $"{nomNaissance}{nomUsage}{prenom}{autresPrenoms?.Trim()}";
                            break;
                        default:
                            break;
                    }
                }
                else if (model.ContactType == ContactType.Society)
                {
                    value = model.SocietyName;
                }

                return value?.Trim();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(DisplayName), ex);
                return null;
            }
        }

        public static async Task<IEnumerable<Contact>> SearchAsync(string searchTerm, Search.Terms termParameter = Search.Terms.Contains, ContactType? contactType = null)
        {
            try
            {
                if (searchTerm.IsStringNullOrEmptyOrWhiteSpace() || searchTerm.Length < 2)
                {
                    throw new ArgumentNullException(nameof(searchTerm), "Le terme de la recherche doit contenir au moins deux caractères.");
                }

                List<Tcontact> tcontacts = new ();

                using LibrarySqLiteDbContext context = new();
                if (contactType == null)
                {
                    tcontacts = await context.Tcontacts.ToListAsync();
                }
                else
                {
                    tcontacts = await context.Tcontacts.Where(w => w.ContactType == (byte)contactType).ToListAsync();
                }

                List<Tcontact> filteredTcontact = new();
                if (tcontacts != null && tcontacts.Any())
                {
                    List<Tcontact>? searchedInCivility = tcontacts.Select(s => SearchInCivility(s, searchTerm, termParameter)).Where(w => w != null).ToList();
                    if (searchedInCivility != null && searchedInCivility.Any())
                    {
                        filteredTcontact.AddRange(searchedInCivility);
                    }

                    List<Tcontact>? searchedInOtherLocation = tcontacts.Select(s => SearchInOtherLocation(s, searchTerm, termParameter)).Where(w => w != null).ToList();
                    if (searchedInOtherLocation != null && searchedInOtherLocation.Any())
                    {
                        filteredTcontact.AddRange(searchedInOtherLocation);
                    }
                }

                if (filteredTcontact != null && filteredTcontact.Any())
                {
                    filteredTcontact = filteredTcontact.Distinct().ToList();

                    return filteredTcontact.Select(s => ConvertToViewModel(s)) ?? Enumerable.Empty<Contact>();
                }

                return Enumerable.Empty<Contact>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(SearchInOtherLocation), ex);
                return Enumerable.Empty<Contact>();
            }
        }

        private static Tcontact? SearchInCivility(Tcontact tcontact, string searchTerm, Search.Terms termParameter = Search.Terms.Contains)
        {
            try
            {
                if (searchTerm.IsStringNullOrEmptyOrWhiteSpace() || searchTerm.Length < 2)
                {
                    throw new ArgumentNullException(nameof(searchTerm), "Le terme de la recherche doit contenir au moins deux caractères.");
                }

                string termToLower = searchTerm.ToLower();

                var enumDisplayCivlityList = Enum.GetValues(typeof(DisplayCivility)).Cast<DisplayCivility>();
                foreach (DisplayCivility displayStyle in enumDisplayCivlityList)
                {
                    string? contactDisplayStyle = DisplayName(tcontact, displayStyle);
                    switch (termParameter)
                    {
                        case Search.Terms.Equals:
                            if (contactDisplayStyle?.ToLower() == termToLower)
                            {
                                return tcontact;
                            }
                            break;
                        case Search.Terms.Contains:
                            if (contactDisplayStyle?.ToLower().Contains(termToLower) == true)
                            {
                                return tcontact;
                            }

                            break;
                        case Search.Terms.StartWith:
                            if (contactDisplayStyle?.ToLower().StartsWith(termToLower) == true)
                            {
                                return tcontact;
                            }
                            break;
                        case Search.Terms.EndWith:
                            if (contactDisplayStyle?.ToLower().EndsWith(termToLower) == true)
                            {
                                return tcontact;
                            }
                            break;
                        default:
                            break;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(SearchInCivility), ex);
                return null;
            }
        }


        private static Tcontact? SearchInOtherLocation(Tcontact tcontact, string searchTerm, Search.Terms termParameter = Search.Terms.Contains)
        {
            try
            {
                if (searchTerm.IsStringNullOrEmptyOrWhiteSpace() || searchTerm.Length < 2)
                {
                    throw new ArgumentNullException(nameof(searchTerm), "Le terme de la recherche doit contenir au moins deux caractères.");
                }

                string termToLower = searchTerm.ToLower();

                switch (termParameter)
                {
                    case Search.Terms.Equals:
                        if (tcontact.Observation?.ToLower() == termToLower || tcontact.Nationality?.ToLower() == termToLower || tcontact.AdressPostal?.ToLower() == termToLower ||
                            tcontact.Ville?.ToLower() == termToLower || tcontact.CodePostal?.ToLower() == termToLower || tcontact.MailAdress?.ToLower() == termToLower
                            || tcontact.NoTelephone?.ToLower() == termToLower || tcontact.NoMobile?.ToLower() == termToLower)
                        {
                            return tcontact;
                        }
                        break;
                    case Search.Terms.Contains:
                        if (tcontact.Observation?.ToLower().Contains(termToLower) == true || tcontact.Nationality?.ToLower().Contains(termToLower) == true || tcontact.AdressPostal?.ToLower().Contains(termToLower) == true ||
                            tcontact.Ville?.ToLower().Contains(termToLower) == true || tcontact.CodePostal?.ToLower().Contains(termToLower) == true || tcontact.MailAdress?.ToLower().Contains(termToLower) == true
                            || tcontact.NoTelephone?.ToLower().Contains(termToLower) == true || tcontact.NoMobile?.ToLower().Contains(termToLower) == true)
                        {
                            return tcontact;
                        }

                        break;
                    case Search.Terms.StartWith:
                        if (tcontact.Observation?.ToLower().StartsWith(termToLower) == true || tcontact.Nationality?.ToLower().StartsWith(termToLower) == true || tcontact.AdressPostal?.ToLower().StartsWith(termToLower) == true ||
                            tcontact.Ville?.ToLower().StartsWith(termToLower) == true || tcontact.CodePostal?.ToLower().StartsWith(termToLower) == true || tcontact.MailAdress?.ToLower().StartsWith(termToLower) == true
                            || tcontact.NoTelephone?.ToLower().StartsWith(termToLower) == true || tcontact.NoMobile?.ToLower().StartsWith(termToLower) == true)
                        {
                            return tcontact;
                        }

                        break;
                    case Search.Terms.EndWith:
                        if (tcontact.Observation?.ToLower().EndsWith(termToLower) == true || tcontact.Nationality?.ToLower().EndsWith(termToLower) == true || tcontact.AdressPostal?.ToLower().EndsWith(termToLower) == true ||
                            tcontact.Ville?.ToLower().EndsWith(termToLower) == true || tcontact.CodePostal?.ToLower().EndsWith(termToLower) == true || tcontact.MailAdress?.ToLower().EndsWith(termToLower) == true
                            || tcontact.NoTelephone?.ToLower().EndsWith(termToLower) == true || tcontact.NoMobile?.ToLower().EndsWith(termToLower) == true)
                        {
                            return tcontact;
                        }
                        break;
                    default:
                        return null;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(SearchInOtherLocation), ex);
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

                Contact viewModel = new()
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

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Contact), nameof(ConvertToViewModel), ex);
                return null;
            }
        }

        #endregion
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
