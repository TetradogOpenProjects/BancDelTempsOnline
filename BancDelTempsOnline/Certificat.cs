/*
 * Creado por SharpDevelop.
 * Usuario: Pingu
 * Fecha: 04/15/2016
 * Hora: 18:10
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using Gabriel.Cat;
using Gabriel.Cat.Extension;
namespace BancDelTempsOnline
{
	/// <summary>
	/// Description of Certificat.
	/// </summary>
	public class Certificat:ObjecteSqlIdAuto
	{
		const string NOMTAULA="Certificats";
		string nom;
		public Certificat(string nom)
			: base(NOMTAULA, "", "Id")
		{
			AltaCanvi("Nom");
			this.nom = nom;
		}

		public string Nom {
			get {
				return nom;
			}
			set {
				nom = value;
				CanviString("Nom", nom);
			}
		}

		#region implemented abstract members of ObjecteSql


		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "Insert into " + Taula + " (Nom) values('" + Nom + "');";
		}


		#endregion
		public static string StringCreateTable()
		{
			return "create table "+NOMTAULA+"(Id int NOT NULL AUTO_INCREMENT,Nom varchar(200) Not null);";
		}

	}
	public class ServeiCertificat:ObjecteSqlIdAuto
	{
		const string NOMTAULA="ServeisCertificat";
		Certificat certificat;
		Servei servei;
		public ServeiCertificat(Certificat certificat, Servei servei)
			: base(NOMTAULA, "", "Id")
		{
			AltaCanvi("Certificat");
			AltaCanvi("Servei");
			this.certificat = certificat;
			this.servei = servei;
		}
		public Certificat Certificat {
			get {
				return certificat;
			}
			set {
				certificat = value;
				base.CanviString("Certificat", certificat.PrimaryKey);
			}
		}

		public Servei Servei {
			get {
				return servei;
			}
			set {
				servei = value;
				base.CanviString("Servei", servei.PrimaryKey);
			}
		}
		#region implemented abstract members of ObjecteSql
		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			string sentencia = "insert into " + Taula + " (CertificatId,ServeiId) value(";
			sentencia += "" + Certificat.PrimaryKey + ",";
			sentencia += "" + Servei.PrimaryKey + ");";
			return sentencia;
		}
		#endregion
				public static string StringCreateTable()
		{
			string sentencia="create table "+NOMTAULA+"(";
			sentencia+="Id int NOT NULL AUTO_INCREMENT,";
			sentencia+="CertificatId int NOT NULL references Certificats(Id),";
			sentencia+="ServeiId int NOT NULL references Serveis(Id));";
			return sentencia;
		}
	}
	public class CertificatUsuari:ObjecteSqlIdAuto
	{
		const string NOMTAULA="CertificatsUsuari";
		Certificat certificat;
		Usuari usuari;
		public CertificatUsuari(Certificat certificat, Usuari usuari)
			: base(NOMTAULA, "", "Id")
		{
			AltaCanvi("Certificat");
			AltaCanvi("Usuari");
			this.certificat = certificat;
			this.usuari = usuari;
		}

		public Certificat Certificat {
			get {
				return certificat;
			}
			set {
				certificat = value;
				CanviString("Certificat", certificat.PrimaryKey);
			}
		}

		public Usuari Usuari {
			get {
				return usuari;
			}
			set {
				usuari = value;
				CanviString("Usuari", usuari.PrimaryKey);
			}
		}
		#region implemented abstract members of ObjecteSql

		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "insert into " + Taula + "(CertificatId,UsuariId) values(" + certificat.PrimaryKey + "," + usuari.PrimaryKey + ");";
		}

		#endregion
		public static string StringCreateTable()
		{
			string sentencia="create table "+NOMTAULA+"(";
			sentencia+="Id int NOT NULL AUTO_INCREMENT,";
			sentencia+="CertificatId int NOT NULL references Certificats(Id),";
			sentencia+="UsuariId varchar(10) NOT NULL references Usuaris(NIE));";
			return sentencia;
		}
	}
}
