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

namespace BancDelTempsOnline
{
	/// <summary>
	/// Description of Servei.
	/// </summary>
	public class Servei:ObjecteSqlIdAuto,IClauUnicaPerObjecte,IComparable<Servei>
	{
		const string NOMTAULA="Serveis";
		string nom;
		string uriImatge;
		string descripció;
		public Servei(string nom,string uriImatge,string descripció):base(NOMTAULA,"","Id")
		{
			AltaCanvi("Nom");
			AltaCanvi("UriImatge");
			AltaCanvi("Descripcio");
			Nom=nom;
			UriImatge=uriImatge;
			Descripció=descripció;
			
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
			return nom;
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
			string sentencia="create table "+NOMTAULA+" (";
			sentencia+="Nom varchar(25) NOT NULL,";
			sentencia+="UriImatge varchar(250),";
			sentencia+="Descripcio varchar(200));";
			return sentencia;
		}

	}
	public class ServeiUsuari:ObjecteSqlIdAuto
	{
		const string NOMTAULA="ServeisUsuari";
		Servei servei;
		Usuari usuari;
		public ServeiUsuari(Servei servei,Usuari usuari):base(NOMTAULA,"","Id")
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


		#endregion
		public static string StringCreateTable()
		{
			string sentencia="create table "+NOMTAULA+"(";
			sentencia+="Id int NOT NULL AUTO_INCREMENT,";
			sentencia+="ServeiId int NOT NULL references Serveis(Id),";
			sentencia+="UsuariId varchar(10) NOT NULL references Usuaris(NIE));";
			return sentencia;
		}
		
	}
}
