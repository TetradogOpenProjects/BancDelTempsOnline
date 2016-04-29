/*
 * Creado por SharpDevelop.
 * Usuario: Pingu
 * Fecha: 04/15/2016
 * Hora: 18:02
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
	/// Description of Usuari.
	/// </summary>
	public class Usuari:ObjecteSql,IClauUnicaPerObjecte
	{
        enum CampsUsuari
        {
            NumSoci, Nom, NIE, Telefon, Municipi, Email, Actiu, UriImatgePerfil, DataRegistre, DataInscripcioFormal
        }
		const int SOCIPENDENT = -1;
		public const string TAULA="usuaris";
        //atributs clase
		int numSoci;
		string nom;
		string uriImatgePerfil;
		string municipi;
		string nie;
		string telefon;
		string email;
		bool actiu;
		//si té el valor per defecte es que s'ha de validar encara
		DateTime dataInscripcioFormal;
		DateTime dataRegistre;
        ListaUnica<CertificatUsuari> certificats = new ListaUnica<CertificatUsuari>();
        ListaUnica<MunicipiQueVolAnar> municipisQueVolAnar;//es una llista de municipis on l'usuari pot anar
        ListaUnica<ServeiUsuari> serveisSenseCertificat;
		//usuari donat d'alta
		public Usuari(int numSoci,string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email,bool actiu,DateTime dataInscripcioFormal,DateTime dataRegistre)
			:base(TAULA,nie,"NIE")
		{
			base.AltaCanvi("NumSoci");
			base.AltaCanvi("Nom");
			base.AltaCanvi("UriImatgePerfil");
			base.AltaCanvi("Municipi");
			base.AltaCanvi("NIE");
			base.AltaCanvi("Telefon");
			base.AltaCanvi("Email");
			base.AltaCanvi("Actiu");
			base.AltaCanvi("DataInscripcioFormal");
			base.AltaCanvi("DataRegistre");
			this.numSoci=numSoci;
			this.nom=nom;
			this.uriImatgePerfil=uriImatgePerfil;
			this.municipi=municipi;
			this.nie=nie;
			this.telefon=telefon;
			this.email=email;
			this.actiu=actiu;
			this.dataInscripcioFormal=dataInscripcioFormal;
			this.dataRegistre=dataRegistre;
            municipisQueVolAnar = new ListaUnica<MunicipiQueVolAnar>();
            certificats = new ListaUnica<CertificatUsuari>();
            serveisSenseCertificat = new ListaUnica<ServeiUsuari>();
		}
		//usuari registrat sense donar d'alta: per tant no esta activat ni te una data d'inscripcio formal!
		public Usuari(string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email,DateTime dataRegistre)
			:this(SOCIPENDENT,nom,uriImatgePerfil,municipi,nie,telefon,email,false,default(DateTime),dataRegistre){} 
		//nou registre:no esta activat,ni te data d'inscripcio i l'hora del registre es el moment quan ho fa
				public Usuari(string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email)
					:this(SOCIPENDENT,nom,uriImatgePerfil,municipi,nie,telefon,email,false,default(DateTime),DateTime.Now){}
		#region Propietats
        public ListaUnica<CertificatUsuari> Certificats
        {
            get { return certificats; }
        }
        public ListaUnica<ServeiUsuari> ServeisSenseCertificat
        {
            get { return serveisSenseCertificat; }
        }
        public int NumSoci {
			get{ return numSoci; }
			set{ numSoci = value;
				CanviNumero("NumSoci",NumSoci+"");
			}
		}
		public string Nom {
			get{ return nom; }
			set{ nom = value; 
				CanviString("Nom",nom);
			}
		}
		public string UriImatgePerfil {
			get{ return uriImatgePerfil; }
			set{ uriImatgePerfil = value;
				CanviString("UriImatgePerfil",UriImatgePerfil);
			}
		}
		public string Municipi {
			get{ return municipi; }
			set{
                municipi = value;
				CanviString("Municipi",Municipi);   
			}
		}

		public string NIE {
			get{ return nie; }
			set{ nie = value; 
				CanviString("NIE",nie);
			}
		}
		public string Telefon {
			get{ return telefon; }
			set{ telefon = value; 
				CanviString("Telefon",telefon);
			}
		}
		public string Email {
			get{ return email; }
			set{ email = value; 
				CanviString("Email",email);
			}
		}
		public bool Actiu {
			get{ return actiu; }
			set{ actiu = value;
				CanviString("Actiu",actiu+"");
			}
		}

		public DateTime DataInscripcioFormal {
			get {
				return dataInscripcioFormal;
			}
			set {
				dataInscripcioFormal = value;
				CanviData("DataInscripcioFormal",dataInscripcioFormal);
			}
		}

		public DateTime DataRegistre {
			get {
				return dataRegistre;
			}
			set {
				dataRegistre = value;
				CanviData("DataRegistre",dataRegistre);
			}
		}
		public bool Validat {
			get{ return !dataInscripcioFormal.Equals(default(DateTime)); }
		}

		#region implemented abstract members of ObjecteSql


		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			string sentencia="insert into "+Taula+"(NumSoci,Nom,NIE,Telefon,Municipi,Email,Actiu,UriImatgePerfil,DataRegistre,DataInscripcioFormal) values(";
			sentencia+=NumSoci+",";
			sentencia+="'"+Nom+"',";
			sentencia+="'"+NIE+"',";
			sentencia+="'"+Telefon+"',";
			sentencia+="'"+Municipi+"',";
			sentencia+="'"+Email+"',";
			sentencia+="'"+Actiu+"',";
			sentencia+="'"+UriImatgePerfil+"',";
			sentencia+=ObjecteSql.DateTimeToStringSQL(tipusBD,DataRegistre)+",";
			sentencia+=ObjecteSql.DateTimeToStringSQL(tipusBD,DataInscripcioFormal)+");";
			return sentencia;
		}


		#endregion

		#region IClauUnicaPerObjecte implementation


		public IComparable Clau()
		{
			return NIE;
		}


		#endregion

		#endregion
        public bool PotAnarAlMunicipi(string municipi)
        {
            return this.Municipi == municipi || this.municipisQueVolAnar.ExisteClave(municipi);
        }
        public void TreuMunicipiQuePotAnar(string municipi)
        {
            if (municipisQueVolAnar.ExisteClave(municipi))
            {
                municipisQueVolAnar.EliminaClave(municipi);
            }
        }
        public MunicipiQueVolAnar AfegirMunicipiQueVolAnar(string municipi)
        {
            if (!municipisQueVolAnar.ExisteClave(municipi))
            {
                municipisQueVolAnar.Añadir(new MunicipiQueVolAnar(municipi, this));
                municipisQueVolAnar[municipi].Baixa += (municipiATreure) => { TreuMunicipiQuePotAnar(municipiATreure.PrimaryKey); };
                municipisQueVolAnar[municipi].Alta += (municipiPosar) => { AfegirMunicipiQueVolAnar(municipiPosar.PrimaryKey); };
            }
            return municipisQueVolAnar[municipi];
        }
        public void AfegirMunicipiQueVolAnar(MunicipiQueVolAnar municipiQueVolAnar)
        {
            if (!municipisQueVolAnar.ExisteObjeto(municipiQueVolAnar))
                municipisQueVolAnar.Añadir(municipiQueVolAnar);
        }
        public Servei[] Serveis()
        {
            Certificat[] certificats = new Certificat[Certificats.Count];
            for (int i = 0; i < certificats.Length; i++)
                certificats[i] = Certificats[i].Certificat;
            return Certificat.ServeisCertificats(certificats);
        }
        public bool ConteServei(Servei servei)
        {
            return Serveis().Contains(servei);
        }
        public bool ConteServei(IEnumerable<Servei> serveis)
        {
            return Serveis().Contains(serveis);
        }
        #region Metodes de Clase
        public static string StringCreateTable()
        {
            string sentencia = "create table " + TAULA + "(";
            sentencia += "NumSoci int,";//fer trigger i secuencia per quan possin la data d'inscripció formal llavors es possi :D el numero que toqui
            sentencia += "Nom varchar(25) NOT NULL,";
            sentencia += "NIE varchar(10) primarykey,";
            sentencia += "Telefon varchar(9),";
            sentencia += "Municipi varchar(25) NOT NULL,";
            sentencia += "Email varchar(30) unique,";
            sentencia += "Actiu varchar(5) NOT NULL,";
            sentencia += "UriImatgePerfil varchar(300),";
            sentencia += "DataRegistre date Not NULL,";
            sentencia += "DataInscripcioFormal date Not NULL);"; 
            return sentencia;
        }
        public static Usuari[] TaulaToUsuariArray(string[,] taulaUsuaris)
        {
            Usuari[] usuaris = new Usuari[taulaUsuaris.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < usuaris.Length; i++)
                usuaris[i] = new Usuari(Convert.ToInt32(taulaUsuaris[(int)CampsUsuari.NumSoci, i]), taulaUsuaris[(int)CampsUsuari.Nom, i], taulaUsuaris[(int)CampsUsuari.UriImatgePerfil, i], taulaUsuaris[(int)CampsUsuari.Municipi, i], taulaUsuaris[(int)CampsUsuari.NIE, i], taulaUsuaris[(int)CampsUsuari.Telefon, i], taulaUsuaris[(int)CampsUsuari.Email, i], Convert.ToBoolean(taulaUsuaris[(int)CampsUsuari.Actiu, i]), ObjecteSql.StringToDateTime(taulaUsuaris[(int)CampsUsuari.DataRegistre, i]), ObjecteSql.StringToDateTime(taulaUsuaris[(int)CampsUsuari.DataInscripcioFormal, i]));
            return usuaris;
        }
        /// <summary>
        /// Filtra els usuaris que viuen al municipi
        /// </summary>
        /// <param name="usuaris"></param>
        /// <param name="municipi"></param>
        /// <returns></returns>
        public static Usuari[] FiltraPerMunicipiEstricte(Usuari[] usuaris,string municipi)
		{
			return usuaris.Filtra((usuari)=>{return usuari.Municipi==municipi;}).ToTaula();
		}
        /// <summary>
        /// Filtra els usuaris que poden anar al municipi
        /// </summary>
        /// <param name="usuaris"></param>
        /// <param name="municipi"></param>
        /// <returns></returns>
        public static Usuari[] FiltraPerMunicipi(Usuari[] usuaris, string municipi)
        {
            return usuaris.Filtra((usuari) => { return usuari.PotAnarAlMunicipi(municipi); }).ToTaula();
        }
        public static Usuari[] FiltraActius(Usuari[] usuaris)
		{
			return usuaris.Filtra((usuari)=>{ return usuari.Actiu; }).ToTaula();
		}

        public static Usuari[] FiltraServei(IEnumerable<Usuari> usuaris, Servei serveis)
        {
            return FiltraServei(usuaris, new Servei[] { serveis });
        }

        public static Usuari[] FiltraServei(IEnumerable<Usuari> usuaris,IEnumerable<Servei> serveis)
        {
            List<Usuari> usuarisQueFanElServei = new List<Usuari>();
            foreach (Usuari usuari in usuaris)
                if (usuari.ConteServei(serveis))
                    usuarisQueFanElServei.Add(usuari);
            return usuarisQueFanElServei.ToTaula();
        }


        #endregion

    }
    public class MunicipiQueVolAnar : ObjecteSqlIdAuto,IClauUnicaPerObjecte
    {
        enum CampsMunicipiQueVolAnar
        {
          Id,UsuariNIE, NomMunicipi
        }
        public const string TAULA = "MunicipisQueVolenAnar";
        string municipi;
        Usuari usuari;

        public MunicipiQueVolAnar(string municipi, Usuari usuari):base(TAULA,"","Id")
        {
            AltaCanvi("Municipi");
            AltaCanvi("Usuari");
            this.municipi = municipi;
            this.usuari = usuari;
        }

        public string Municipi
        {
            get
            {
                return municipi;
            }

            set
            {
                municipi = value;
                CanviString("Municipi", municipi);
            }
        }

        public Usuari Usuari
        {
            get
            {
                return usuari;
            }

            set
            {
                usuari = value;
                CanviString("UsuariId", usuari.PrimaryKey);
            }
        }

        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {
            return "insert into " + TAULA + " values('" + Usuari.PrimaryKey + "','" + Municipi + "');";
        }
        public static string StringCreateTable()
        {
            string createString = "create table " + TAULA + " (";
            createString+= "Id int NOT NULL AUTO_INCREMENT,";//como ObjetoSql no puede tener campos primaryKey compuestos pues tiene que tener este campo
            createString += "UsuariId varchar(10) not null references Usuaris(NIE),";
            createString += "Municipi varchar(25) not null, CONSTRAINT contraintUnique UNIQUE(UsuariId,Municipi) );";
            return createString;
        }

        public IComparable Clau()
        {
            return municipi;
        }
        /// <summary>
        /// Carrega i linka els municipis als usuaris
        /// </summary>
        /// <param name="taulaMunicipisQueVolAnar"></param>
        /// <param name="usuarisList"></param>
        /// <returns></returns>
        public static MunicipiQueVolAnar[] TaulaToMunicipisQueVolAnar(string[,] taulaMunicipisQueVolAnar, LlistaOrdenada<string, Usuari> usuarisList)
        {
            MunicipiQueVolAnar[] municipisQueVolenAnar = new MunicipiQueVolAnar[taulaMunicipisQueVolAnar.GetLength(DimensionMatriz.Fila)];
           for(int i=0;i<municipisQueVolenAnar.Length;i++)
            {
                municipisQueVolenAnar[i] = new MunicipiQueVolAnar(taulaMunicipisQueVolAnar[(int)CampsMunicipiQueVolAnar.NomMunicipi, i], usuarisList[taulaMunicipisQueVolAnar[(int)CampsMunicipiQueVolAnar.UsuariNIE, i]]) { PrimaryKey = taulaMunicipisQueVolAnar[(int)CampsMunicipiQueVolAnar.Id, i] };
                municipisQueVolenAnar[i].Usuari.AfegirMunicipiQueVolAnar(municipisQueVolenAnar[i]);
            }
            return municipisQueVolenAnar;
        }
    }
}
