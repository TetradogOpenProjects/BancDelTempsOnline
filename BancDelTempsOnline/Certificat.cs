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
	public class Certificat:ObjecteSqlIdAuto,IClauUnicaPerObjecte
	{
		public const string NOMTAULA="Certificats";
        string localIdUnic;
		string nom;
        ListaUnica<ServeiCertificat> serveis;
		public Certificat(string nom)
			: base(NOMTAULA, "", "Id")
		{
			AltaCanvi("Nom");
			this.nom = nom;
            serveis = new ListaUnica<ServeiCertificat>();
            localIdUnic = DateTime.Now.Ticks+" " + MiRandom.Next();//es per us local no es desa a la BD
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
        public ListaUnica<ServeiCertificat> Serveis
        {
            get { return serveis; }
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
        public static Servei[] ServeisCertificats(IEnumerable<Certificat> certificats)
        {
            ListaUnica<Servei> serveis = new ListaUnica<Servei>();
            foreach(Certificat certificat in certificats)
            {
                for (int i = 0; i < certificat.Serveis.Count; i++)
                    if (!serveis.Existe(certificat.Serveis[i].Servei))
                        serveis.Añadir(certificat.Serveis[i].Servei);
            }
            return serveis.ToTaula();
        }

        public IComparable Clau()
        {
            return localIdUnic;
        }
    }
	public class ServeiCertificat:ObjecteSqlIdAuto,IClauUnicaPerObjecte
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

        public IComparable Clau()
        {
            return servei.Clau();
        }
    }
	public class CertificatUsuari:ObjecteSqlIdAuto,IClauUnicaPerObjecte
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

        public IComparable Clau()
        {
            return certificat.Clau();
        }
    }
}
