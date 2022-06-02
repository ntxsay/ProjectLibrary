----------------------------------------------------------------------------------------------------
-- SQLITE --
--Bibliotheque -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibrary";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibrary" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cree
    "Name" TEXT NOT NULL UNIQUE,
    "Description" TEXT NULL,
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);

----------------------------------------------------------------------------------------------------
-- Categorie -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TLibraryCategorie";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TLibraryCategorie" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdLibrary" INTEGER NOT NULL,
	"IdParentCategorie" INTEGER NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdParentCategorie") REFERENCES "TLibraryCategorie"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Collection -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TCollection";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TCollection" (
	"Id" INTEGER NOT NULL UNIQUE,
    "IdLibrary" INTEGER NOT NULL,
    "CollectionType" INTEGER NOT NULL DEFAULT 0, -- Collection, liste de lecture
    "Name" TEXT NOT NULL UNIQUE,
    "Description" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- role de contact -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TContactRole";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TContactRole" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL, --Adhérant, Auteur, maison d'édition, Illustrateur, tracducteur, etx
	PRIMARY KEY("Id" AUTOINCREMENT)
);
--Insère les nouvelles valeurs
INSERT OR REPLACE INTO "TContactRole" ("Name", "Description")
VALUES 
        ('Adhérant', NULL),
        ('Auteur', NULL),
        ('Maison d''édition/Editeur', NULL),
        ('Traducteur', NULL),
        ('Illustrateur', NULL);


----------------------------------------------------------------------------------------------------
-- type de contact -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TContactType";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TContactType" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL, --Adhérant, Auteur, maison d'édition, Illustrateur, tracducteur, etx
	PRIMARY KEY("Id" AUTOINCREMENT)
);
--Insère les nouvelles valeurs
INSERT OR REPLACE INTO "TContactType" ("Name", "Description")
VALUES 
        ('Personne', NULL),
        ('Société', NULL);


----------------------------------------------------------------------------------------------------
--Contact emprunteur -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TContact";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TContact" (
	"Id" INTEGER NOT NULL UNIQUE,
    "IdContactType" INTEGER NOT NULL,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cree
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "SocietyName" TEXT NULL,
    "TitreCivilite" TEXT NULL,
    "NomNaissance" TEXT NULL,
    "NomUsage" TEXT NULL,
    "Prenom" TEXT NULL,
    "AutresPrenoms" TEXT NULL,
    "DateNaissance" TEXT NULL,
    "Nationality" TEXT  NULL,
    "AdressPostal" TEXT NULL,
    "Ville" TEXT NULL,
    "CodePostal" TEXT NULL,
    "MailAdress" TEXT NULL,
    "NoTelephone" TEXT NULL,
    "NoMobile" TEXT NULL,
    "Observation" TEXT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdContactType") REFERENCES "TContactType"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
--Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBook";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBook" (
	"Id" INTEGER NOT NULL UNIQUE,
    "IdLibrary" INTEGER NOT NULL,
    "IdCategorie" INTEGER NULL,
    "Guid" TEXT NOT NULL UNIQUE,--Si le dossier ce cree
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "MainTitle" TEXT NOT NULL,
    "CountOpening" INTEGER NOT NULL DEFAULT 0,
    "DateParution" TEXT NULL,
    "Resume" TEXT NULL,
    "Notes" TEXT NULL,
	"Langue" TEXT NULL,
	"Pays" TEXT NULL,
    "PhysicalLocation" TEXT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdLibrary") REFERENCES "TLibrary"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdCategorie") REFERENCES "TLibraryCategorie"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Identification Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookIdentification";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookIdentification" (
	"Id" INTEGER NOT NULL UNIQUE,
    "ISBN" TEXT NULL,
    "ISBN10" TEXT NULL,
    "ISBN13" TEXT NULL,
    "ISSN" TEXT NULL,
    "ASIN" TEXT NULL, --Id Amazon
    "Cotation" TEXT NULL,
    "CodeBarre" TEXT NULL, -- voir le scan avec syncfusion si possible
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("Id") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Classification Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookClassification";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookClassification" (
	"Id" INTEGER NOT NULL UNIQUE,
    "TypeClassification" INTEGER  NOT NULL DEFAULT 0,
    "ApartirDe" INTEGER  NOT NULL DEFAULT 0,
    "Jusqua" INTEGER  NOT NULL DEFAULT 0,
    "DeTelAge" INTEGER  NOT NULL DEFAULT 0,
    "ATelAge" INTEGER  NOT NULL DEFAULT 0,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("Id") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Format Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookFormat";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookFormat" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Format" TEXT NULL, -- Relie, broche, cartonne, poche, audio, ebook
    "NbOfPages" INTEGER NULL,
    "Largeur" REAL NULL,
    "Hauteur" REAL NULL,
    "Epaisseur" REAL NULL,
    "Weight" REAL NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("Id") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- status -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookReading";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookReading" (
	"Id" INTEGER NOT NULL UNIQUE,
    "Status" INTEGER NULL, -- Terminé, En pause, En cours de lecture, Abandonné, Non lu, A lire
    "LastPageReaded" INTEGER NULL,
    "LastDateReaded" TEXT NULL,
    "Note10" REAL NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("Id") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Titres Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookOtherTitle";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookOtherTitle" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "Title" TEXT NOT NULL UNIQUE,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Collection Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookCollections";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookCollections" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
    "IdCollection" INTEGER NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdCollection") REFERENCES "TCollection"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
--Exemplaires Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookExemplary";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookExemplary" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
	"IdContactSource" INTEGER NULL,
    "NoGroup" TEXT NULL,
    "NoExemplary" INTEGER NOT NULL DEFAULT 0,
    --"Quantity" INTEGER NOT NULL DEFAULT 0,
    "DateAjout" TEXT NOT NULL,
    "DateEdition" TEXT NULL,
    "TypeAcquisition" TEXT NOT NULL, --Donné, Acheté, preté, Autre 
    "Price" REAL NULL,
	"DeviceName" TEXT NULL,
    "DateAcquisition" TEXT NULL, --Date emprunt, Achat, Donation
    "DateRemise" TEXT NULL, --Si emprunt
    "IsVisible" INTEGER NOT NULL DEFAULT 1,
    "Observations" TEXT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdContactSource") REFERENCES "TContact"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Connecteur Auteur/Livres -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookContactRoleConnector";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookContactRoleConnector" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBook" INTEGER NOT NULL,
	"IdContact" INTEGER NOT NULL,
	"IdRole" INTEGER NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBook") REFERENCES "TBook"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdContact") REFERENCES "TContact"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdRole") REFERENCES "TContactRole"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Livre Etat -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookEtat";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookEtat" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBookExemplary" INTEGER NOT NULL,
    "DateAjout" TEXT NOT NULL,
    "TypeVerification" INTEGER NOT NULL, -- Entrée, avant pret, apres pret, sortie
	"Etat" TEXT NOT NULL,
	"Observations" TEXT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdBookExemplary") REFERENCES "TBookExemplary"("Id") ON DELETE CASCADE
);

----------------------------------------------------------------------------------------------------
-- Pret Livre -- 
--Supprime la Table si existe
DROP TABLE IF EXISTS "TBookPret";
--Cree la Table si n'existe pas
CREATE TABLE IF NOT EXISTS "TBookPret" (
	"Id" INTEGER NOT NULL UNIQUE,
	"IdBookExemplary" INTEGER NOT NULL,
	"IdContact" INTEGER NOT NULL,
	"IdEtatBefore" INTEGER NOT NULL,
	"IdEtatAfter" INTEGER NULL,
    "DatePret" TEXT NOT NULL,
    "TimePret" TEXT NULL,
    "DateRemise" TEXT NULL,
    "TimeRemise" TEXT NULL,
    "DateRemiseUser" TEXT NULL, --new
    "Observation" TEXT NULL,

	PRIMARY KEY("Id" AUTOINCREMENT)
    FOREIGN KEY("IdContact") REFERENCES "TContact"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdEtatBefore") REFERENCES "TBookEtat"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdEtatAfter") REFERENCES "TBookEtat"("Id") ON DELETE CASCADE
    FOREIGN KEY("IdBookExemplary") REFERENCES "TBookExemplary"("Id") ON DELETE CASCADE
);