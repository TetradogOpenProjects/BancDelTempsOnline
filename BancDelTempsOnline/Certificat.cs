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
	public class Certificat:ObjecteSql,IClauUnicaPerObjecte
	{
        enum CampsCertificat
        { Nom}   
		public const string TAULA="Certificats";
        string localIdUnic;
		string nom;
        ListaUnica<ServeiCertificat> serveis;
		public Certificat(string nom)
			: base(TAULA, nom, "Nom")
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
        public IComparable Clau()
        {
            return localIdUnic;
        }
		#region implemented abstract members of ObjecteSql


		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "Insert into " + Taula + " (Nom) values('" + Nom + "');";
		}


		#endregion
		public static string StringCreateTable()
		{
			return "create table "+TAULA+"(Nom varchar(200) PrimaryKey);";
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
       public static Certificat[] TaulaToCertificatsArray(string[,] taulaCertificats)
        {
            Certificat[] certificats = new Certificat[taulaCertificats.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < certificats.Length; i++)
                certificats[i] = new Certificat(taulaCertificats[(int)CampsCertificat.Nom, i]);
            return certificats;
        }

    }
	public class ServeiCertificat:ObjecteSqlIdAuto,IClauUnicaPerObjecte
	{
        enum CampsServeiCertificat
        {
            CertificatNom,ServeiNom
        }
		public const string TAULA="ServeisCertificat";
		Certificat certificat;
		Servei servei;
		public ServeiCertificat(Certificat certificat, Servei servei)
			: base(TAULA, "", "Id")
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
        public IComparable Clau()
        {
            return servei.Clau();
        }
		#region implemented abstract members of ObjecteSql
		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			string sentencia = "insert into " + Taula + " (CertificatId,ServeiId) value(";
			sentencia += "" + Certificat.PrimaryKey + ",";
			sentencia += "" + Servei.PrimaryKey + ");";
			return sentencia;
		}
        /// <summary>
        /// Carrega els serveis i linca els serveis als certificats
        /// </summary>
        /// <param name="taulaServeisCertificats"></param>
        /// <param name="serveisList"></param>
        /// <param name="certificatsList"></param>
        /// <returns></returns>
        public static ServeiCertificat[] TaulaToServeisCertificatsArray(string[,] taulaServeisCertificats, LlistaOrdenada<string, Servei> serveisList, LlistaOrdenada<string, Certificat> certificatsList)
        {
            ServeiCertificat[] serveisCertificat = new ServeiCertificat[taulaServeisCertificats.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<serveisCertificat.Length;i++)
            {
                serveisCertificat[i] = new ServeiCertificat(certificatsList[taulaServeisCertificats[(int)CampsServeiCertificat.CertificatNom, i]], serveisList[taulaServeisCertificats[(int)CampsServeiCertificat.ServeiNom, i]]);
                //poso el servei a la llista
                certificatsList[serveisCertificat[i].Certificat.Nom].Serveis.Añadir(serveisCertificat[i]);
            }
            return serveisCertificat;
        }
        #endregion
        public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+"(";
			sentencia+="Id int NOT NULL AUTO_INCREMENT,";
			sentencia+="CertificatId int NOT NULL references Certificats(Id),";
			sentencia+="ServeiId int NOT NULL references Serveis(Id));";
			return sentencia;
		}

      
    }
	public class CertificatUsuari:ObjecteSqlIdAuto,IClauUnicaPerObjecte
	{
        enum CampsCertificatUsuari
        { CertificatNom, UsuariId }
		public const string TAULA="CertificatsUsuari";
		Certificat certificat;
		Usuari usuari;
		public CertificatUsuari(Certificat certificat, Usuari usuari)
			: base(TAULA, "", "Id")
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
        public IComparable Clau()
        {
            return certificat.Clau();
        }
		#region implemented abstract members of ObjecteSql

		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "insert into " + Taula + "(CertificatId,UsuariId) values(" + certificat.PrimaryKey + "," + usuari.PrimaryKey + ");";
		}

		#endregion
		public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+"(";
			sentencia+="Id int NOT NULL AUTO_INCREMENT,";
			sentencia+="CertificatId int NOT NULL references Certificats(Id),";
			sentencia+="UsuariId varchar(10) NOT NULL references Usuaris(NIE));";
			return sentencia;
		}
        public static CertificatUsuari[] TaulaToServeisUsuarisArray(string[,] taulaCertificatUsuari,LlistaOrdenada<string,Usuari> usuaris, LlistaOrdenada<string, Certificat> certificats)
        {
            CertificatUsuari[] certificatsUsuari = new CertificatUsuari[taulaCertificatUsuari.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < certificatsUsuari.Length; i++)
                certificatsUsuari[i] = new CertificatUsuari(certificats[taulaCertificatUsuari[(int)CampsCertificatUsuari.CertificatNom, i]], usuaris[taulaCertificatUsuari[(int)CampsCertificatUsuari.UsuariId, i]]);
            return certificatsUsuari;
        }
        
    }
}
