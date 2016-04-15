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
	public class Usuari:IClauUnicaPerObjecte
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
		DateTime dataInscripcioFormal;
		//si el per defecte es que s'ha de validar encara
		Llista<Servei> serveis;
		//serveis que vol fer
		Llista<Certificat> certificats;
		//per poder fer un servei el certificat ha de contenir-lo
		
		public Usuari()
		{
			certificats = new Llista<Certificat>();
			serveis = new Llista<Servei>();
			numSoci = SOCIPENDENT;
		}
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

		public Llista<Certificat> Certificats {
			get {
				return certificats;
			}
		}

		public Llista<Servei> Serveis {
			get {
				return serveis;
			}
		}

		#region IClauUnicaPerObjecte implementation


		public IComparable Clau()
		{
			return NIE;
		}


		#endregion

		#endregion
		public bool PotRealitzarServei(Servei servei)
		{
			if (servei == null)
				throw new ArgumentNullException();
			bool potFerHo = false;
			for (int i = 0; i < certificats.Count && !potFerHo; i++)
				potFerHo = certificats[i].Serveis.Existeix(servei);
			return potFerHo;
		}
		public static Usuari[] UsuarisOfertsServei(Usuari[] usuaris, Servei servei)
		{
			if (usuaris == null || servei == null)
				throw new ArgumentNullException();
			List<Usuari> usuarisOfertats = new List<Usuari>();
			for (int i = 0; i < usuaris.Length; i++) {
				if (usuaris[i].Serveis.Existeix(servei))
					usuarisOfertats.Add(usuaris[i]);
			}
			return usuarisOfertats.ToArray();
		}
		public static Usuari[] UsuarisCertificatsServei(Usuari[] usuaris, Servei servei)
		{
			if (usuaris == null || servei == null)
				throw new ArgumentNullException();
			List<Usuari> usuarisCertificats = new List<Usuari>();
			for (int i = 0; i < usuaris.Length; i++) {
				if (usuaris[i].PotRealitzarServei(servei))
					usuarisCertificats.Add(usuaris[i]);
			}
			return usuarisCertificats.ToArray();
		}
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
