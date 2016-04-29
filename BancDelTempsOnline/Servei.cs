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
        { Nom, UriImatge, Descripcio }
        public const string TAULA="Serveis";
		string nom;
		string uriImatge;
		string descripció;
        string localIdUnic;
		public Servei(string nom,string uriImatge,string descripció):base(TAULA,"","Id")
		{
			AltaCanvi("Nom");
			AltaCanvi("UriImatge");
			AltaCanvi("Descripcio");
			Nom=nom;
			UriImatge=uriImatge;
			Descripció=descripció;
            localIdUnic = DateTime.Now.Ticks+" "+MiRandom.Next();//es per us local no es desa a la BD

        }
		public string Nom {
			get{ return nom; }
			set{ nom = value;
				CanviString("Nom",nom);
			}
		}
		public string UriImatge {
			get{ return uriImatge; }
			set{ uriImatge = value;
				CanviString("UriImatge",uriImatge);
			}
		}
		public string Descripció {
			get{ return descripció; }
			set{ descripció = value;
				CanviString("Descripcio",descripció);
			}
		}

		#region implemented abstract members of ObjecteSql


		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			return "Insert into "+Taula+"(Nom,UriImatge,Descripcio) values('"+Nom+"','"+UriImatge+"','"+Descripció+"');";
		}


		#endregion

		#region IClauUnicaPerObjecte implementation

		public IComparable Clau()
		{
			return localIdUnic;
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(Servei other)
		{
			return Clau().CompareTo(other.Clau());
		}

		#endregion
		public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+" (";
			sentencia+="Nom varchar(25) NOT NULL,";
			sentencia+="UriImatge varchar(250),";
			sentencia+="Descripcio varchar(200));";
			return sentencia;
		}
        public static Servei[] TaulaToServeisArray(string[,] taulaServeis)
        {
            Servei[] serveis = new Servei[taulaServeis.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<serveis.Length;i++)
                serveis[i]=new Servei(taulaServeis[(int)CampsServei.Nom,i],taulaServeis[(int)CampsServei.UriImatge, i],taulaServeis[(int)CampsServei.Descripcio, i]);
            return serveis;
        }
        public int CompareTo(object obj)
        {
            return CompareTo(obj as Servei);
        }
    }
    /// <summary>
    /// Serveix per comptar els serveis que no requereixen d'un certificat
    /// </summary>
	public class ServeiUsuari:ObjecteSqlIdAuto,IClauUnicaPerObjecte
	{
        enum CampsServeiUsuari
        { ServeiNom,UsuariNIE}
        public const string TAULA="ServeisUsuari";
		Servei servei;
		Usuari usuari;
		public ServeiUsuari(Servei servei,Usuari usuari):base(TAULA,"","Id")
		{
			AltaCanvi("Servei");
			AltaCanvi("Usuari");
			Servei=servei;
			Usuari=usuari;
		}

		public Servei Servei {
			get {
				return servei;
			}
			set {
				servei = value;
				CanviString("Servei",servei.PrimaryKey);
			}
		}

		public Usuari Usuari {
			get {
				return usuari;
			}
			set {
				usuari = value;
				CanviString("Usuari",usuari.PrimaryKey);
			}
		}
		#region implemented abstract members of ObjecteSql


		public override string StringInsertSql(TipusBaseDeDades tipusBD)
		{
			string sentencia = "insert into " + Taula + " (ServeiId,UsuariId) value(";
			sentencia += "" + Servei.PrimaryKey + ",";
			sentencia += "'" + Usuari.PrimaryKey + "');";
			return sentencia;
		}
        /// <summary>
        /// Carrega i linka els usuaris als serveis 
        /// </summary>
        /// <param name="taulaServeisUsuaris"></param>
        /// <param name="serveis"></param>
        /// <param name="usuaris"></param>
        /// <returns></returns>
        public static ServeiUsuari[] TaulaToServeisUsuarisArray(string[,] taulaServeisUsuaris, LlistaOrdenada<string, Servei> serveis, LlistaOrdenada<string, Usuari> usuaris)
        {
            ServeiUsuari[] serveisUsuaris = new ServeiUsuari[taulaServeisUsuaris.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<serveisUsuaris.Length;i++)
            {
                serveisUsuaris[i] = new ServeiUsuari(serveis[taulaServeisUsuaris[(int)CampsServeiUsuari.ServeiNom, i]], usuaris[taulaServeisUsuaris[(int)CampsServeiUsuari.UsuariNIE, i]]);
                usuaris[serveisUsuaris[i].Usuari.NIE].ServeisSenseCertificat.Añadir(serveisUsuaris[i]);
            }
            return serveisUsuaris;
        }


        #endregion
        public static string StringCreateTable()
		{
			string sentencia="create table "+TAULA+"(";
			sentencia+="Id int NOT NULL AUTO_INCREMENT,";
			sentencia+="ServeiId int NOT NULL references Serveis(Id),";
			sentencia+="UsuariId varchar(10) NOT NULL references Usuaris(NIE));";
			return sentencia;
		}

        public IComparable Clau()
        {
            return servei.Clau();
        }
    }

}
