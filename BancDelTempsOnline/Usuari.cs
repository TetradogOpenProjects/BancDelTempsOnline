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
            NumSoci, Nom, NIE, Telefon, Municipi, Email, Actiu, ImatgePerfil, DataRegistre, DataInscripcioFormal,QuiHoVaFormalitzar
        }
		const int SOCIPENDENT = -1;
		public const string TAULA="usuaris";
        public const string CAMPPRIMARYKEY = "NIE";
        public const int TAMANYIMATGEPERFIL = 250 * 1024;//un 250KB
        private const int TAMANYNOM = 25;
        private const int TAMANYNUMTELEFON = 9;
        private const int TAMANYEMAIL = 30;
        //atributs clase
        int numSoci;
		string nom;
		string imatgePerfil;
		string municipi;
        string telefon;
		string email;
		bool actiu;
		//si té el valor per defecte es que s'ha de validar encara
		DateTime dataInscripcioFormal;
		DateTime dataRegistre;
        ListaUnica<CertificatUsuari> certificats = new ListaUnica<CertificatUsuari>();
        ListaUnica<MunicipiQueVolAnar> municipisQueVolAnar;//es una llista de municipis on l'usuari pot anar
        ListaUnica<ServeiUsuari> serveisSenseCertificat;
        Usuari quiHoVaFormalitzar;
		//usuari donat d'alta
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numSoci"></param>
        /// <param name="nom"></param>
        /// <param name="imatgePerfil">string formada dels bytes de la imatge en format JPG</param>
        /// <param name="municipi"></param>
        /// <param name="nie"></param>
        /// <param name="telefon"></param>
        /// <param name="email"></param>
        /// <param name="actiu"></param>
        /// <param name="dataInscripcioFormal"></param>
        /// <param name="dataRegistre"></param>
        /// <param name="quiHoVaFormalitzar"></param>
		public Usuari(int numSoci,string nom,string imatgePerfil,string municipi,string nie,string telefon,string email,bool actiu,DateTime dataInscripcioFormal,DateTime dataRegistre,Usuari quiHoVaFormalitzar)
			:base(TAULA,nie,CAMPPRIMARYKEY)
		{
            if (String.IsNullOrEmpty(nom) || String.IsNullOrEmpty(municipi) || String.IsNullOrEmpty(nie) || String.IsNullOrEmpty(email))
                throw new NullReferenceException();
			base.AltaCanvi(CampsUsuari.NumSoci.ToString());
			base.AltaCanvi(CampsUsuari.Nom.ToString());
			base.AltaCanvi(CampsUsuari.ImatgePerfil.ToString());
			base.AltaCanvi(CampsUsuari.Municipi.ToString());
			base.AltaCanvi(CampsUsuari.Telefon.ToString());
			base.AltaCanvi(CampsUsuari.Email.ToString());
			base.AltaCanvi(CampsUsuari.Actiu.ToString());
			base.AltaCanvi(CampsUsuari.DataInscripcioFormal.ToString());
			base.AltaCanvi(CampsUsuari.DataRegistre.ToString());
            base.AltaCanvi(CampsUsuari.QuiHoVaFormalitzar.ToString());
            this.quiHoVaFormalitzar = quiHoVaFormalitzar;
			this.numSoci=numSoci;
			this.nom=nom;
			this.imatgePerfil=imatgePerfil;
			this.municipi=municipi;
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
			:this(SOCIPENDENT,nom,uriImatgePerfil,municipi,nie,telefon,email,false,default(DateTime),dataRegistre,null){} 
		//nou registre:no esta activat,ni te data d'inscripcio i l'hora del registre es el moment quan ho fa
				public Usuari(string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email)
					:this(SOCIPENDENT,nom,uriImatgePerfil,municipi,nie,telefon,email,false,default(DateTime),DateTime.Now,null){}
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
				CanviNumero(CampsUsuari.NumSoci.ToString(), NumSoci+"");
			}
		}
		public string Nom {
			get{ return nom; }
			set{
                if (value.Length > TAMANYNOM)
                    throw new ArgumentException("S'ha superat el maxim de longitud");
                nom = value; 
				CanviString(CampsUsuari.Nom.ToString(), nom);
			}
		}
        /// <summary>
        /// string formada dels bytes de la imatge en format JPG
        /// </summary>
		public string ImatgePerfil {
			get{ return imatgePerfil; }
			set{
                if (value.Length > TAMANYIMATGEPERFIL)
                    throw new ArgumentException("S'ha superat el maxim de longitud");
                imatgePerfil = value;
				CanviString(CampsUsuari.ImatgePerfil.ToString(), ImatgePerfil);
			}
		}
		public string Municipi {
			get{ return municipi; }
			set{
                if (value.Length > MunicipiQueVolAnar.TAMANYNOMMUNICIPI)
                    throw new ArgumentException("S'ha superat el tamany maxim");
                municipi = value;
				CanviString(CampsUsuari.Municipi.ToString(), Municipi);   
			}
		}

		public string NIE {
			get{ return PrimaryKey; }
			set{ PrimaryKey = value; 
			}
		}
		public string Telefon {
			get{ return telefon; }
			set{
                if (value != null)
                {
                    if (value.Length != TAMANYNUMTELEFON)
                        throw new ArgumentNullException();
                }
                else value = "";
                telefon = value; 
				CanviString(CampsUsuari.Telefon.ToString(), telefon);
			}
		}
		public string Email {
			get{ return email; }
			set{
                if (value == null)
                    throw new ArgumentNullException("es imprescindible");
                email = value; 
				CanviString(CampsUsuari.Email.ToString(), email);
			}
		}
		public bool Actiu {
			get{ return actiu; }
			set{ actiu = value;
				CanviString(CampsUsuari.Actiu.ToString(), actiu+"");
			}
		}

		public DateTime DataInscripcioFormal {
			get {
				return dataInscripcioFormal;
			}
			set {
				dataInscripcioFormal = value;
				CanviData(CampsUsuari.DataInscripcioFormal.ToString(), dataInscripcioFormal);
			}
		}

		public DateTime DataRegistre {
			get {
				return dataRegistre;
			}
			set {
				dataRegistre = value;
				CanviData(CampsUsuari.DataRegistre.ToString(), dataRegistre);
			}
		}
		public bool Validat {
			get{ return !dataInscripcioFormal.Equals(default(DateTime)); }
		}

        public Usuari QuiHoVaFormalitzar
        {
            get
            {
                return quiHoVaFormalitzar;
            }

            set
            {
                quiHoVaFormalitzar = value;
                if (quiHoVaFormalitzar == null)
                    CanviString(CampsUsuari.QuiHoVaFormalitzar.ToString(), quiHoVaFormalitzar.PrimaryKey);
                else
                    CanviString(CampsUsuari.QuiHoVaFormalitzar.ToString(), "");
            }
        }

        #region implemented abstract members of ObjecteSql


        public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			string sentencia="insert into "+Taula+" values(";
			sentencia+=NumSoci+",";
			sentencia+="'"+Nom+"',";
			sentencia+="'"+NIE+"',";
			sentencia+="'"+Telefon+"',";
			sentencia+="'"+Municipi+"',";
			sentencia+="'"+Email+"',";
			sentencia+="'"+Actiu+"',";
			sentencia+="'"+ImatgePerfil+"',";
			sentencia+=ObjecteSql.DateTimeToStringSQL(tipusBD,DataRegistre)+",";
			sentencia+=ObjecteSql.DateTimeToStringSQL(tipusBD,DataInscripcioFormal)+",";
            sentencia += "'" + (quiHoVaFormalitzar != null ? quiHoVaFormalitzar.PrimaryKey : "") + "');";
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
            sentencia += CampsUsuari.NumSoci.ToString()+" int,";//fer trigger i secuencia per quan possin la data d'inscripció formal llavors es possi :D el numero que toqui
            sentencia += CampsUsuari.Nom.ToString() + " varchar("+TAMANYNOM+") NOT NULL,";
            sentencia += CampsUsuari.NIE.ToString() + " varchar(10) primarykey,";
            sentencia += CampsUsuari.Telefon.ToString() + " varchar("+ TAMANYNUMTELEFON + "),";
            sentencia += CampsUsuari.Municipi.ToString() + " varchar("+MunicipiQueVolAnar.TAMANYNOMMUNICIPI+") NOT NULL,";
            sentencia += CampsUsuari.Email.ToString() + " varchar("+TAMANYEMAIL+") unique,";
            sentencia += CampsUsuari.Actiu.ToString() + " varchar(5) NOT NULL,";
            sentencia +=CampsUsuari.ImatgePerfil.ToString()+" varchar("+TAMANYIMATGEPERFIL+"),";
            sentencia += CampsUsuari.DataRegistre.ToString() + " date Not NULL,";
            sentencia += CampsUsuari.DataInscripcioFormal.ToString() + " date Not NULL,";
            sentencia += CampsUsuari.QuiHoVaFormalitzar.ToString() + " varchar(10) "+Usuari.TAULA+"("+Usuari.CAMPPRIMARYKEY+")"+");";
            return sentencia;
        }
        public static Usuari[] TaulaToUsuariArray(string[,] taulaUsuaris)
        {
            Usuari[] usuaris = new Usuari[taulaUsuaris.GetLength(DimensionMatriz.Fila)];
            SortedList<string, Usuari> usuarisList = new SortedList<string, Usuari>();
            for (int i = 0; i < usuaris.Length; i++)
            {
                usuaris[i] = new Usuari(Convert.ToInt32(taulaUsuaris[(int)CampsUsuari.NumSoci, i]), taulaUsuaris[(int)CampsUsuari.Nom, i], taulaUsuaris[(int)CampsUsuari.ImatgePerfil, i], taulaUsuaris[(int)CampsUsuari.Municipi, i], taulaUsuaris[(int)CampsUsuari.NIE, i], taulaUsuaris[(int)CampsUsuari.Telefon, i], taulaUsuaris[(int)CampsUsuari.Email, i], Convert.ToBoolean(taulaUsuaris[(int)CampsUsuari.Actiu, i]), ObjecteSql.StringToDateTime(taulaUsuaris[(int)CampsUsuari.DataRegistre, i]), ObjecteSql.StringToDateTime(taulaUsuaris[(int)CampsUsuari.DataInscripcioFormal, i]),null);
                usuarisList.Add(usuaris[i].PrimaryKey, usuaris[i]);
            }
            //poso qui ho va formalitzar
            for (int i = 0; i < usuaris.Length; i++)
            {
                if (usuarisList.ContainsKey(taulaUsuaris[(int)CampsUsuari.QuiHoVaFormalitzar, i]))
                    usuaris[i].quiHoVaFormalitzar = usuarisList[taulaUsuaris[(int)CampsUsuari.QuiHoVaFormalitzar, i]];
            }
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
          Id,UsuariId, MunicipiId
        }
        public const int TAMANYNOMMUNICIPI = 20;
        public const string TAULA = "MunicipisQueVolenAnar";
        const string CAMPPRIMARYKEY = "Id";
        string municipi;
        Usuari usuari;
        public MunicipiQueVolAnar(string municipi, Usuari usuari) : this("", municipi, usuari) { }
        private MunicipiQueVolAnar(string id,string municipi, Usuari usuari):base(TAULA,id,CAMPPRIMARYKEY)
        {
            if (municipi.Length > MunicipiQueVolAnar.TAMANYNOMMUNICIPI)
                throw new ArgumentException("S'ha superat el tamany maxim");
            AltaCanvi(CampsMunicipiQueVolAnar.MunicipiId.ToString());
            AltaCanvi(CampsMunicipiQueVolAnar.UsuariId.ToString());
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
                if (value.Length > TAMANYNOMMUNICIPI)
                    throw new ArgumentException("S'ha superat el tamany maxim");
                municipi = value;
                CanviString(CampsMunicipiQueVolAnar.MunicipiId.ToString(), municipi);
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
                CanviString(CampsMunicipiQueVolAnar.UsuariId.ToString(), usuari.PrimaryKey);
            }
        }
        public IComparable Clau()
        {
            return municipi;
        }
        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {
            return "insert into " + TAULA + " values('" + Usuari.PrimaryKey + "','" + Municipi + "');";
        }
        public static string StringCreateTable()
        {
            string createString = "create table " + TAULA + " (";
            createString+= CampsMunicipiQueVolAnar.Id.ToString()+" int NOT NULL AUTO_INCREMENT,";//como ObjetoSql no puede tener campos primaryKey compuestos pues tiene que tener este campo
            createString += CampsMunicipiQueVolAnar.UsuariId.ToString() + " varchar(10) not null references "+Usuari.TAULA+"("+Usuari.CAMPPRIMARYKEY+"),";
            createString += CampsMunicipiQueVolAnar.MunicipiId.ToString() + " varchar("+TAMANYNOMMUNICIPI+") not null, CONSTRAINT contraintUnique UNIQUE("+CampsMunicipiQueVolAnar.UsuariId.ToString()+","+ CampsMunicipiQueVolAnar.MunicipiId.ToString() + ") );";
            return createString;
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
                municipisQueVolenAnar[i] = new MunicipiQueVolAnar(taulaMunicipisQueVolAnar[(int)CampsMunicipiQueVolAnar.Id, i], taulaMunicipisQueVolAnar[(int)CampsMunicipiQueVolAnar.MunicipiId, i], usuarisList[taulaMunicipisQueVolAnar[(int)CampsMunicipiQueVolAnar.UsuariId, i]]) { PrimaryKey = taulaMunicipisQueVolAnar[(int)CampsMunicipiQueVolAnar.Id, i] };
                municipisQueVolenAnar[i].Usuari.AfegirMunicipiQueVolAnar(municipisQueVolenAnar[i]);
            }
            return municipisQueVolenAnar;
        }
    }
}
