/*
 * Creado por SharpDevelop.
 * Usuario: Pingu
 * Fecha: 15/04/2016
 * Hora: 21:18
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using Gabriel.Cat;

namespace BancDelTempsOnline
{
	/// <summary>
	/// Description of ControlBD.
	/// </summary>
	public class ControlBD:ControlObjectesSql
	{

		static string[] creates={};
		public ControlBD(BaseDeDades baseDeDades):base(baseDeDades,creates)
		{
		}


		#region implemented abstract members of ControlObjectesSql

		public override void Restaurar()
		{
         
		}

		#endregion
	}
}
