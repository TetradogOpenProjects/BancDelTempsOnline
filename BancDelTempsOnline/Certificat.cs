/*
 * Creado por SharpDevelop.
 * Usuario: Pingu
 * Fecha: 04/15/2016
 * Hora: 18:10
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using Gabriel.Cat;

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
		public Certificat()
		{
			serveis = new Llista<Servei>();
		}
		public Llista<Servei> Serveis {
			get {
				
				return serveis;
			}
		}
	}
}
