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
        //faltan opcions,permisos
        //falta  banners per prohibir opcions
        //falten triggers
        //falten taulesHistorial de cada taula
        //falte taulaInterCanvis
        //falte  taulaOfertes
        //falte taulaUsuarisOfertats//es una taula on surten els usuaris proposats per una oferta
        //falte taula objectesOfertats
        //falte taulaObjectesPrestats

        //estaria be que les opcions estigesisn al sql??per poder afegir,treure,canviar desde el navegador la propia pagina web??
        //aixi es poden fer canvis sense aturar el servei i ho pot fer un admin mateix :D
        //fer variables que representin els camps de l'usuari aixi es un model per reemplaçar els camps pels que toquin

        //si canvia l'email s'ha de donar de baixa el permis de googleplus l'anterior
        //taula missatges usuariEmissor,usuariReceptor,Misstage
        static string[] creates = { Usuari.StringCreateTable(), Certificat.StringCreateTable(), Servei.StringCreateTable(), CertificatUsuari.StringCreateTable(), ServeiCertificat.StringCreateTable(), ServeiUsuari.StringCreateTable(), MunicipiQueVolAnar.StringCreateTable() };
		public ControlBD(BaseDeDades baseDeDades):base(baseDeDades,creates)
		{
		}


		#region implemented abstract members of ControlObjectesSql

		public override void Restaurar()
		{
            string[,] tablaUsuaris = BaseDeDades.ConsultaTableDirect(Usuari.Taula);
		}

		#endregion
	}
}
