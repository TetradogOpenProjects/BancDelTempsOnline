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
	public class Servei:IClauUnicaPerObjecte,IComparable<Servei>
	{
		string nom;
		string uriImatge;
		string descripció;
		ListaUnica<Usuari> usuaris;
		public string Nom {
			get{ return nom; }
			set{ nom = value; }
		}
		public string UriImatge {
			get{ return uriImatge; }
			set{ uriImatge = value; }
		}
		public string Descripció {
			get{ return descripció; }
			set{ descripció = value; }
		}
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
		public bool OfertaServei(Usuari usuari)
		{
			return usuaris.Existe(usuari);
		}
		public static Usuari[] UsuarisOfertsServei(Usuari[] usuaris, Servei servei)
		{
			if (usuaris == null || servei == null)
				throw new ArgumentNullException();
			List<Usuari> usuarisOfertats = new List<Usuari>();
			for (int i = 0; i < usuaris.Length; i++) {
				if (servei.OfertaServei(usuaris[i]))
					usuarisOfertats.Add(usuaris[i]); 
			}
			return usuarisOfertats.ToArray();
		}

	}
}
