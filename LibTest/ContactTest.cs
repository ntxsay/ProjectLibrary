using System;
using LibApi;
using LibApi.Services.Contacts;
using LibShared;
using LibShared.ViewModels.Contacts;

namespace LibTest
{
	public class ContactTest
	{
        [Fact]
        public async void NewContact()
        {
            Contact? contact = await Contact.CreateAsync(null, "Doe", "John", "Dmitri", null, new DateTime(1994, 09, 28));
            if (contact != null)
            {
                //...
                contact.Dispose();
            }

            Assert.NotNull(contact);
        }

        [Fact]
        public async void NewContact2()
        {
            Contact? contact = await Contact.CreateAsync("John Doe Dmitri", "", ContactType.Human);
            if (contact != null)
            {
                //...
                contact.Dispose();
            }

            Assert.NotNull(contact);
        }

        [Fact]
        public async void NewContact3()
        {
            Contact? contact = await Contact.CreateAsync("Ecole DeMaigarde-2", "", ContactType.Society);
            if (contact != null)
            {
                //...
                contact.Dispose();
            }

            Assert.NotNull(contact);
        }

        [Fact]
        public async void NewContact4()
        {
            ContactVM viewModel = new()
            {
                ContactType = ContactType.Human,
                TitreCivilite = "M.",
                NomNaissance = "Doe",
                Prenom = "Johnny",
                AdressePostal = "3 Rue les hiboux qui dansent",
                //...
            };

            Contact? contact = await Contact.CreateAsync(viewModel, false);
            if (contact != null)
            {
                //...
                contact.Dispose();
            }

            Assert.NotNull(contact);
        }

        [Fact]
        public async void SearchContact()
        {
            IEnumerable<Contact> contact = await Contact.SearchAsync("capesterre", Search.Terms.Contains, null);
            if (contact != null)
            {
                //...
                Assert.NotEmpty(contact);
            }
            else if (contact == null)
            {
                Assert.NotNull(contact);
            }

        }
    }
}

