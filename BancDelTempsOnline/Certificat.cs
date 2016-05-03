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
        { Nom,QuiHoVaAfegirId}   
		public const string TAULA="Certificats";
        public const string CAMPPRIMARYKEY = "Nom";
        string localIdUnic;
		string nom;
        Usuari quiHoVaAfegir;
        ListaUnica<ServeiCertificat> serveis;
        private const int MAXLONGITUDNOM=200;

        public Certificat(string nom,Usuari quiHoVaAfegir)
			: base(TAULA, nom,CampsCertificat.Nom.ToString())
		{
            if (quiHoVaAfegir == null)
                throw new NullReferenceException();
			AltaCanvi(CampsCertificat.QuiHoVaAfegirId.ToString());
			this.nom = nom;
            this.quiHoVaAfegir = quiHoVaAfegir;
            serveis = new ListaUnica<ServeiCertificat>();
            localIdUnic = DateTime.Now.Ticks+" " + MiRandom.Next();//es per us local no es desa a la BD
        }

		public string Nom {
			get {
				return this.PrimaryKey;
			}
			set {
                if (value == null)
                    throw new NullReferenceException("El nom no pot ser null!!");
                if (value.Length > MAXLONGITUDNOM)
                    throw new ArgumentException("S'ha superat el maxim de longitud");
                this.PrimaryKey = value;
			}
		}
        public ListaUnica<ServeiCertificat> Serveis
        {
            get { return serveis; }
        }

        public Usuari QuiHoVaAfegir
        {
            get
            {
                return quiHoVaAfegir;
            }

            set
            {
                if (value == null)
                    throw new NullReferenceException("No puede ser null!!");
                quiHoVaAfegir = value;
                CanviString(CampsCertificat.QuiHoVaAfegirId.ToString(), quiHoVaAfegir.PrimaryKey);
            }
        }

        public IComparable Clau()
        {
            return localIdUnic;
        }
		#region implemented abstract members of ObjecteSql


		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "Insert into " + Taula + "  values('" + Nom + "','"+QuiHoVaAfegir.PrimaryKey+"');";
		}


		#endregion
		public static string StringCreateTable()
		{
			return "create table "+TAULA+"("+CampsCertificat.Nom.ToString()+ " varchar("+MAXLONGITUDNOM+") PrimaryKey,"+CampsCertificat.QuiHoVaAfegirId.ToString()+" varchar(10) references "+Usuari.TAULA+"("+Usuari.CAMPPRIMARYKEY+"));";
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
       public static Certificat[] TaulaToCertificats(string[,] taulaCertificats, TwoKeysList<string, string, Usuari> usuaris)
        {
            Certificat[] certificats = new Certificat[taulaCertificats.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < certificats.Length; i++)
                certificats[i] = new Certificat(taulaCertificats[(int)CampsCertificat.Nom, i],usuaris.ObtainValueWithKey2(taulaCertificats[(int)CampsCertificat.QuiHoVaAfegirId, i]));
            return certificats;
        }

    }
	public class ServeiCertificat:ObjecteSqlIdAuto,IClauUnicaPerObjecte
	{
        enum CampsServeiCertificat
        {
           Id, CertificatId,ServeiId,QuiHoVaAfegir
        }
		public const string TAULA="ServeisCertificat";
        public const string CAMPPRYMARYKEY = "Id";
		Certificat certificat;
		Servei servei;
        Usuari quiHoVaAfegir;
        public ServeiCertificat(Certificat certificat,Servei servei,Usuari quiHoVaAfegir) : this("", certificat, servei,quiHoVaAfegir) { }
		private ServeiCertificat(string id,Certificat certificat, Servei servei,Usuari quiHoVaAfegir)
			: base(TAULA, id, CampsServeiCertificat.Id.ToString())
		{
			AltaCanvi(CampsServeiCertificat.CertificatId.ToString());
			AltaCanvi(CampsServeiCertificat.ServeiId.ToString());
            AltaCanvi(CampsServeiCertificat.QuiHoVaAfegir.ToString());
			this.certificat = certificat;
			this.servei = servei;
            this.quiHoVaAfegir = quiHoVaAfegir;
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

        public Usuari QuiHoVaAfegir
        {
            get
            {
                return quiHoVaAfegir;
            }

            set
            {
                if (value == null) throw new NullReferenceException();
                quiHoVaAfegir = value;
                CanviString(CampsServeiCertificat.QuiHoVaAfegir.ToString(), quiHoVaAfegir.PrimaryKey);
            }
        }

        public IComparable Clau()
        {
            return servei.Clau();
        }
		#region implemented abstract members of ObjecteSql
		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			string sentencia = "insert into " + Taula + "("+CampsServeiCertificat.CertificatId.ToString() + "," + CampsServeiCertificat.ServeiId.ToString() + ","+CampsServeiCertificat.QuiHoVaAfegir.ToString()+") values(";
			sentencia += "" + Certificat.PrimaryKey + ",";
			sentencia += "" + Servei.PrimaryKey + ",";
            sentencia += "'" + QuiHoVaAfegir.PrimaryKey + "');";
			return sentencia;
		}
        /// <summary>
        /// Carrega els serveis i linca els serveis als certificats
        /// </summary>
        /// <param name="taulaServeisCertificats"></param>
        /// <param name="serveisList"></param>
        /// <param name="certificatsList"></param>
        /// <returns></returns>
        public static ServeiCertificat[] TaulaToServeisCertificats(string[,] taulaServeisCertificats, TwoKeysList<string, string, Usuari> usuaris, LlistaOrdenada<string, Servei> serveisList, LlistaOrdenada<string, Certificat> certificatsList)
        {
            ServeiCertificat[] serveisCertificat = new ServeiCertificat[taulaServeisCertificats.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<serveisCertificat.Length;i++)
            {
                serveisCertificat[i] = new ServeiCertificat(certificatsList[taulaServeisCertificats[(int)CampsServeiCertificat.CertificatId, i]], serveisList[taulaServeisCertificats[(int)CampsServeiCertificat.ServeiId, i]], usuaris.ObtainValueWithKey2(taulaServeisCertificats[(int)CampsServeiCertificat.QuiHoVaAfegir, i]));
                //poso el servei a la llista
                certificatsList[serveisCertificat[i].Certificat.Nom].Serveis.Añadir(serveisCertificat[i]);
            }
            return serveisCertificat;
        }
        #endregion
        public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+"(";
			sentencia+= CampsServeiCertificat.Id.ToString()+" int AUTO_INCREMENT primaryKey,";
			sentencia+= CampsServeiCertificat.CertificatId.ToString() + " int NOT NULL references " + Certificat.TAULA + " (" + Certificat.CAMPPRIMARYKEY + "),";
			sentencia+= CampsServeiCertificat.ServeiId.ToString() + " int NOT NULL references " + Servei.TAULA + " (" + Servei.CAMPPRIMARYKEY + ")";
            sentencia += CampsServeiCertificat.QuiHoVaAfegir.ToString() + " varchar(10) NOT NULL references " + Usuari.TAULA + " (" + Usuari.CAMPPRIMARYKEY + "));";
			return sentencia;
		}

      
    }
	public class CertificatUsuari:ObjecteSqlIdAuto,IClauUnicaPerObjecte
	{
        enum CampsCertificatUsuari
        { Id,CertificatId, UsuariId,QuiHoVaComprobarId,Activat}
		public const string TAULA="CertificatsUsuari";
        public const string CAMPPRIMARYKEY = "Id";
		Certificat certificat;
		Usuari usuari;
        bool actiu;
        Usuari quiHoVaActivar;

        public CertificatUsuari(Certificat certificat, Usuari usuari) : this("", certificat, usuari,false,null) { }
        private CertificatUsuari(string id,Certificat certificat, Usuari usuari, bool activat, Usuari quiHoVaActivar)
            : base(TAULA,id, CAMPPRIMARYKEY)
		{
			AltaCanvi(CampsCertificatUsuari.CertificatId.ToString());
			AltaCanvi(CampsCertificatUsuari.UsuariId.ToString());
            AltaCanvi(CampsCertificatUsuari.Activat.ToString());
            AltaCanvi(CampsCertificatUsuari.QuiHoVaComprobarId.ToString());
			this.certificat = certificat;
			this.usuari = usuari;
            this.actiu = activat;
            this.quiHoVaActivar = quiHoVaActivar;
		}

        public Certificat Certificat {
			get {
				return certificat;
			}
			set {
				certificat = value;
				CanviString("CertificatId", certificat.PrimaryKey);
			}
		}

		public Usuari Usuari {
			get {
				return usuari;
			}
			set {
				usuari = value;
				CanviString("UsuariId", usuari.PrimaryKey);
			}
		}
        /// <summary>
        /// Si es false vol dir que l'usuari no vol rebre encarrecs provinents d'aquest certificat
        /// </summary>
        public bool Actiu
        {
            get
            {
                return actiu;
            }

            set
            {
                actiu = value;
                CanviString("Actiu", actiu.ToString());
            }
        }
        /// <summary>
        /// Els certificats s'han de comprobar per poder donar un certificat de qualitat de servei
        /// </summary>
        public Usuari QuiHoVaComprobar
        {
            get
            {
                return quiHoVaActivar;
            }

            set
            {
                quiHoVaActivar = value;
                if (quiHoVaActivar != null)
                {
                    CanviString("QuiHoVaComprobarId", quiHoVaActivar.PrimaryKey);
                }
                else
                {
                    CanviString("QuiHoVaComprobarId", "");
                    
                }
            }
        }

        public IComparable Clau()
        {
            return certificat.Clau();
        }
		#region implemented abstract members of ObjecteSql

		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "insert into " + Taula + "(" + CampsCertificatUsuari.CertificatId.ToString() + "," + CampsCertificatUsuari.UsuariId.ToString() + "," + CampsCertificatUsuari.QuiHoVaComprobarId.ToString() + "," + CampsCertificatUsuari.Activat.ToString()+") values(" + certificat.PrimaryKey + ",'" + usuari.PrimaryKey + "','"+quiHoVaActivar.PrimaryKey+"','"+Actiu.ToString()+"');";
		}

		#endregion
		public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+"(";
			sentencia+= CampsCertificatUsuari.Id.ToString()+" int  AUTO_INCREMENT primarykey,";
			sentencia+= CampsCertificatUsuari.CertificatId.ToString() + " int NOT NULL references "+Certificat.TAULA+"("+Certificat.CAMPPRIMARYKEY+"),";
			sentencia+= CampsCertificatUsuari.UsuariId.ToString() + " varchar(10) NOT NULL references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsCertificatUsuari.QuiHoVaComprobarId.ToString() + " varchar(10) references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsCertificatUsuari.Activat.ToString() + " varchar(5));";
			return sentencia;
		}
        public static CertificatUsuari[] TaulaToServeisUsuaris(string[,] taulaCertificatUsuari,TwoKeysList<string, string, Usuari> usuaris, LlistaOrdenada<string, Certificat> certificats)
        {
            CertificatUsuari[] certificatsUsuari = new CertificatUsuari[taulaCertificatUsuari.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < certificatsUsuari.Length; i++)
            {
                certificatsUsuari[i] = new CertificatUsuari(taulaCertificatUsuari[(int)CampsCertificatUsuari.Id, i], certificats[taulaCertificatUsuari[(int)CampsCertificatUsuari.CertificatId, i]], usuaris.ObtainValueWithKey2(taulaCertificatUsuari[(int)CampsCertificatUsuari.UsuariId, i]),Convert.ToBoolean(taulaCertificatUsuari[(int)CampsCertificatUsuari.Activat, i]), usuaris.ObtainValueWithKey2(taulaCertificatUsuari[(int)CampsCertificatUsuari.QuiHoVaComprobarId, i]));

            }
                return certificatsUsuari;
        }
        
    }
}
