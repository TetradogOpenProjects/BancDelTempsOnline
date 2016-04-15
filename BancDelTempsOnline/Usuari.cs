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
		const long SOCIPENDENT = -1;
		
		long numSoci;
		string nom;
		string uriImatgePerfil;
		string municipi;
		string nie;
		string telefon;
		string email;
		bool actiu;
		//si el per defecte es que s'ha de validar encara
		DateTime dataInscripcioFormal;


		
		public Usuari(long numSoci,string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email,bool actiu,DateTime dataInscripcioFormal,Servei[] serveis,Certificat[] certificats)
			:base("usuaris",nie,"NIE")
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
			NumSoci=numSoci;
			Nom=nom;
			UriImatgePerfil=uriImatgePerfil;
			Municipi=municipi;
			NIE=nie;
			Telefon=telefon;
			Email=email;
			Actiu=actiu;
			DataInscripcioFormal=dataInscripcioFormal;
		}
		public Usuari(string nom,string uriImatgePerfil,string municipi,string nie,string telefon,string email,bool actiu,DateTime dataInscripcioFormal)
			:this(SOCIPENDENT,nom,uriImatgePerfil,municipi,nie,telefon,email,actiu,dataInscripcioFormal,serveis,certificats){}
		#region Propietats
		public long NumSoci {
			get{ return numSoci; }
			set{ numSoci = value; }
		}
		public string Nom {
			get{ return nom; }
			set{ nom = value; }
		}
		public string UriImatgePerfil {
			get{ return uriImatgePerfil; }
			set{ uriImatgePerfil = value; }
		}
		public string Municipi {
			get{ return municipi; }
			set{ municipi = value; }
		}
		public string NIE {
			get{ return nie; }
			set{ nie = value; }
		}
		public string Telefon {
			get{ return telefon; }
			set{ telefon = value; }
		}
		public string Email {
			get{ return email; }
			set{ email = value; }
		}
		public bool Actiu {
			get{ return actiu; }
			set{ actiu = value; }
		}

		public DateTime DataInscripcioFormal {
			get {
				return dataInscripcioFormal;
			}
			set {
				dataInscripcioFormal = value;
			}
		}

		public bool Validat {
			get{ return !dataInscripcioFormal.Equals(default(DateTime)); }
		}
		#region IClauUnicaPerObjecte implementation


		public IComparable Clau()
		{
			return NIE;
		}


		#endregion

		#endregion
	
		public static Usuari[] FiltraPerMunicipi(Usuari[] usuaris,string municipi)
		{
			return usuaris.Filtra((usuari)=>{return usuari.Municipi==municipi;}).ToTaula();
		}
		public static Usuari[] FiltraActius(Usuari[] usuaris)
		{
			return usuaris.Filtra((usuari)=>{return usuari.Actiu}).ToTaula();
		}
	}
}
