/*
 * Creado por SharpDevelop.
 * Usuario: Pingu
 * Fecha: 04/15/2016
 * Hora: 18:07
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
	/// Description of Servei.
	/// </summary>
	public class Servei:ObjecteSqlIdAuto,IClauUnicaPerObjecte,IComparable<Servei>,IComparable
	{
        enum CampsServei
        { Id,Nom, Imatge, Descripcio,QuiHoVaAfegirId }
        public const string TAULA="Serveis";
        public const string CAMPPRIMARYKEY = "Id";
		string nom;
		Fitxer imatge;
		string descripció;
        string localIdUnic;
        Usuari quiHoVaAfegir;
        private const int TAMANYIMATGE=64*1024;//64KB
        private const int TAMANYDESCRIPCIO = 200;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="imatge">string formada dels bytes de la imatge en format JPG</param>
        /// <param name="descripció"></param>
        /// <param name="quiHoVaAfegir"></param>
        public Servei(string nom, Fitxer imatge, string descripció, Usuari quiHoVaAfegir) : this("", nom, imatge, descripció, quiHoVaAfegir) { }
		private Servei(string id,string nom,Fitxer imatge,string descripció,Usuari quiHoVaAfegir):base(TAULA,id,CampsServei.Id.ToString())
		{
            if (descripció == null) descripció = "";
            if (imatge!=null&&imatge.Dades.Length > TAMANYIMATGE|| descripció.Length > TAMANYDESCRIPCIO)
                throw new ArgumentException("S'ha superat el tamany maxim");
            if (quiHoVaAfegir == null|nom==null)
                throw new NullReferenceException();
			AltaCanvi(CampsServei.Nom.ToString());
			AltaCanvi(CampsServei.Imatge.ToString());
			AltaCanvi(CampsServei.Descripcio.ToString());
            AltaCanvi(CampsServei.QuiHoVaAfegirId.ToString());
			this.nom=nom;
			this.imatge=imatge;
			this.descripció=descripció;
            this.quiHoVaAfegir = quiHoVaAfegir;
            localIdUnic = DateTime.Now.Ticks+" "+MiRandom.Next();//es per us local no es desa a la BD

        }
		public string Nom {
			get{ return nom; }
			set{ nom = value;
				CanviString(CampsServei.Nom.ToString(), nom);
			}
		}

		public Fitxer Imatge {
			get{ return imatge; }
			set{
                if (value!=null&&value.Dades.Length > TAMANYIMATGE)
                    throw new ArgumentException("S'ha superat el tamany maxim");
                if (imatge != null)
                    imatge.OnBaixa();
                imatge = value;
                if (imatge != null)
                    CanviString(CampsServei.Imatge.ToString(), imatge.PrimaryKey);
                else
                    CanviString(CampsServei.Imatge.ToString(), null);

            }
		}

		public string Descripció {
			get{ return descripció; }
			set{
                if (value.Length > TAMANYDESCRIPCIO)
                    throw new ArgumentException("S'ha superat el tamany maxim");
                descripció = value;
				CanviString(CampsServei.Descripcio.ToString(), descripció);
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
                if (value == null)
                    throw new NullReferenceException("Es requereix saber qui ho va afegir");
                quiHoVaAfegir = value;
                CanviString(CampsServei.QuiHoVaAfegirId.ToString(), quiHoVaAfegir.PrimaryKey);
             
            }
        }

        #region implemented abstract members of ObjecteSql


        public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "Insert into "+Taula+"("+CampsServei.Nom.ToString()+"," + CampsServei.Imatge.ToString() + "," + CampsServei.Descripcio.ToString() + "," + CampsServei.QuiHoVaAfegirId.ToString() + ") values('"+Nom+"',"+(Imatge!=null?"'"+Imatge.PrimaryKey+"'":"null")+",'"+Descripció+"','"+QuiHoVaAfegir.PrimaryKey+"');";
		}


        #endregion
        public override void OnBaixa()
        {
            base.OnBaixa();
            Imatge = null;//trec la imatge de la BD
        }
        #region IClauUnicaPerObjecte implementation

        public IComparable Clau()
		{
			return localIdUnic;
		}

		#endregion
        public int CompareTo(object obj)
        {
            return CompareTo(obj as Servei);
        }
		#region IComparable implementation

		public int CompareTo(Servei other)
		{
			return Clau().CompareTo(other.Clau());
		}

		#endregion
		public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+" (";
            sentencia += CampsServei.Id.ToString() + " int  AUTO_INCREMENT primarykey,";
            sentencia += CampsServei.Nom.ToString() + " varchar(25) NOT NULL,";
			sentencia+= CampsServei.Imatge.ToString() + " int references "+Fitxer.TAULA+"("+Fitxer.CAMPPRIMARYKEY+"),";
			sentencia+= CampsServei.Descripcio.ToString() + " varchar("+TAMANYDESCRIPCIO+"),";
            sentencia += CampsServei.QuiHoVaAfegirId.ToString() + " varchar(10) references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "));";
			return sentencia;
		}
        public static Servei[] TaulaToServeis(string[,] taulaServeis, TwoKeysList<string, string, Usuari> usuaris, TwoKeysList<string, string, Fitxer> fitxers)
        {
            Servei[] serveis = new Servei[taulaServeis.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<serveis.Length;i++)
                serveis[i]=new Servei(taulaServeis[(int)CampsServei.Id,i],taulaServeis[(int)CampsServei.Nom,i],fitxers.ObtainValueWithKey2(taulaServeis[(int)CampsServei.Imatge, i]),taulaServeis[(int)CampsServei.Descripcio, i],usuaris.ObtainValueWithKey2(taulaServeis[(int)CampsServei.QuiHoVaAfegirId, i]));
            return serveis;
        }

    }
    /// <summary>
    /// Serveix per comptar els serveis que no requereixen d'un certificat
    /// </summary>
	public class ServeiUsuari:ObjecteSqlIdAuto,IClauUnicaPerObjecte
	{
        enum CampsServeiUsuari
        { Id,ServeiId,UsuariId,QuiHoVaComprobarId,Actiu}
        public const string TAULA="ServeisUsuari";
        public const string CAMPPRIMARYKEY = "Id";
		Servei servei;
		Usuari usuari;
        Usuari quiHoVaComprobar;
        bool actiu;
        public ServeiUsuari( Servei servei, Usuari usuari) : this("", servei, usuari) { }
        private ServeiUsuari(string id,Servei servei,Usuari usuari):base(TAULA,id,CampsServeiUsuari.Id.ToString())
		{
			AltaCanvi(CampsServeiUsuari.ServeiId.ToString());
			AltaCanvi(CampsServeiUsuari.UsuariId.ToString());
            AltaCanvi(CampsServeiUsuari.QuiHoVaComprobarId.ToString());
            AltaCanvi(CampsServeiUsuari.Actiu.ToString());
			this.servei=servei;
			this.usuari=usuari;
		}

        public ServeiUsuari(Servei servei, Usuari usuari, Usuari quiHoVaComprobar, bool actiu):this(servei,usuari)
        {
            this.quiHoVaComprobar = quiHoVaComprobar;
            this.actiu = actiu;
        }

        public Servei Servei {
			get {
				return servei;
			}
			set {
				servei = value;
				CanviString(CampsServeiUsuari.ServeiId.ToString(), servei.PrimaryKey);
			}
		}

		public Usuari Usuari {
			get {
				return usuari;
			}
			set {
                if (value == null)
                    throw new NullReferenceException("Es necessita un usuari pel servei!");
				usuari = value;
				CanviString(CampsServeiUsuari.UsuariId.ToString(), usuari.PrimaryKey);
			}
		}

        public Usuari QuiHoVaComprobar
        {
            get
            {
                return quiHoVaComprobar;
            }

            set
            {
                quiHoVaComprobar = value;
                if(quiHoVaComprobar!=null)
                {
                    CanviString(CampsServeiUsuari.QuiHoVaComprobarId.ToString(), quiHoVaComprobar.PrimaryKey);
                }else
                {
                    CanviString(CampsServeiUsuari.QuiHoVaComprobarId.ToString(), "");
                }
            }
        }

        public bool Actiu
        {
            get
            {
                return actiu;
            }

            set
            {
                actiu = value;
                CanviString(CampsServeiUsuari.Actiu.ToString(), actiu.ToString());
            }
        }
        #region implemented abstract members of ObjecteSql
        public IComparable Clau()
        {
            return servei.Clau();
        }

        public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			string sentencia = "insert into " + Taula + "("+CampsServeiUsuari.ServeiId+"," + CampsServeiUsuari.UsuariId + "," + CampsServeiUsuari.QuiHoVaComprobarId + "," + CampsServeiUsuari.Actiu  + ")  values(";
			sentencia += "" + Servei.PrimaryKey + ",";
			sentencia += "'" + Usuari.PrimaryKey + "',";
            sentencia += "'" + (quiHoVaComprobar != null ? quiHoVaComprobar.PrimaryKey : "") + "',";
            sentencia += "'" + actiu.ToString() + "');";
			return sentencia;
		}
        #endregion
        /// <summary>
        /// Carrega i linka els usuaris als serveis 
        /// </summary>
        /// <param name="taulaServeisUsuaris"></param>
        /// <param name="serveis"></param>
        /// <param name="usuaris"></param>
        /// <returns></returns>
        public static ServeiUsuari[] TaulaToServeisUsuaris(string[,] taulaServeisUsuaris, LlistaOrdenada<string, Servei> serveis, TwoKeysList<string,string, Usuari> usuaris)
        {
            ServeiUsuari[] serveisUsuaris = new ServeiUsuari[taulaServeisUsuaris.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<serveisUsuaris.Length;i++)
            {
                serveisUsuaris[i] = new ServeiUsuari(serveis[taulaServeisUsuaris[(int)CampsServeiUsuari.ServeiId, i]], usuaris.ObtainValueWithKey2(taulaServeisUsuaris[(int)CampsServeiUsuari.UsuariId, i]));
                usuaris.ObtainValueWithKey2(serveisUsuaris[i].Usuari.PrimaryKey).ServeisSenseCertificat.Añadir(serveisUsuaris[i]);
            }
            return serveisUsuaris;
        }


      
        public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+"(";
			sentencia+=CampsServeiUsuari.Id.ToString()+" int  AUTO_INCREMENT primaryKey,";
			sentencia+= CampsServeiUsuari.ServeiId.ToString() + " int NOT NULL references " + Servei.TAULA + "(" + Servei.CAMPPRIMARYKEY + "),";
			sentencia+= CampsServeiUsuari.UsuariId.ToString() + " varchar(10) NOT NULL references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsServeiUsuari.QuiHoVaComprobarId.ToString() + " varchar(10)  references " + Usuari.TAULA+"("+Usuari.CAMPPRIMARYKEY+"),";
            sentencia += CampsServeiUsuari.Actiu.ToString() + " varchar(5));";
            return sentencia;
		}


    }

}
