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
		const int SOCIPENDENT = -1;
		const string NOMTAULA="usuaris";
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

		//usuari donat d'alta
		public Usuari(int numSoci,string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email,bool actiu,DateTime dataInscripcioFormal,DateTime dataRegistre)
			:base(NOMTAULA,nie,"NIE")
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
		}
		//usuari registrat sense donar d'alta
		public Usuari(string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email,bool actiu,DateTime dataInscripcioFormal,DateTime dataRegistre)
			:this(SOCIPENDENT,nom,uriImatgePerfil,municipi,nie,telefon,email,actiu,dataInscripcioFormal,dataRegistre){} 
		//nou registre
				public Usuari(string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email)
					:this(SOCIPENDENT,nom,uriImatgePerfil,municipi,nie,telefon,email,false,default(DateTime),DateTime.Now){}
		#region Propietats
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
			set{ municipi = value;
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
		public static string StringCreateTable()
		{
			string sentencia="create table "+NOMTAULA+"(";
			sentencia+="NumSoci int,";//fer trigger i secuencia per quan possin la data d'inscripció formal llavors es possi :D el numero que toqui
			sentencia+="Nom varchar(25) NOT NULL,";
			sentencia+="NIE varchar(10) primarykey,";
			sentencia+="Telefon varchar(9),";
			sentencia+="Municipi varchar(25) NOT NULL,";
			sentencia+="Email varchar(30) unique,";
			sentencia+="Actiu varchar(5) NOT NULL,";
			sentencia+="UriImatgePerfil varchar(300),";
			sentencia+="DataRegistre date Not NULL,";
			sentencia+="DataInscripcioFormal date Not NULL);";
			return sentencia;
		}
		public static Usuari[] FiltraPerMunicipi(Usuari[] usuaris,string municipi)
		{
			return usuaris.Filtra((usuari)=>{return usuari.Municipi==municipi;}).ToTaula();
		}
		public static Usuari[] FiltraActius(Usuari[] usuaris)
		{
			return usuaris.Filtra((usuari)=>{ return usuari.Actiu; }).ToTaula();
		}
	}
}
