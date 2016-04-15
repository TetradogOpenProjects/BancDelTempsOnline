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
	public class Certificat
	{
		string nom;
		//son tots els serveis que convalida el certificat
		Llista<Servei> serveis;
		ListaUnica<Usuari> usuaris;
		public Certificat()
		{
			serveis = new Llista<Servei>();
			usuaris = new ListaUnica<Usuari>();
		}
		public Llista<Servei> Serveis {
			get {			
				return serveis;
			}
		}
		public ListaUnica<Usuari> UsuarisCertificats {
			get {				
				return usuaris;
			}
		}
		public bool PotRealitzarServeis(Usuari usuari)
		{
			if (usuari == null)
				throw new ArgumentNullException();
			return usuaris.Existe(usuari);
		}
		public static Usuari[] UsuarisCertificatsServei(IEnumerable<Usuari> usuaris, Certificat certificat)
		{
			if (usuaris == null || certificat == null)
				throw new ArgumentNullException();
			return usuaris.Filtra((usuari) => {
				return certificat.PotRealitzarServeis(usuari);
			}).ToTaula();
		}
		public static Certificat[] CertificatsAmbServei(IEnumerable<Certificat> certificats, Servei servei)
		{
			if (certificats == null || servei == null)
				throw new ArgumentNullException();
			return certificats.Filtra((certificat) => {
				return certificat.Serveis.Existeix(servei);
			}).ToTaula();
		}
	}
}
