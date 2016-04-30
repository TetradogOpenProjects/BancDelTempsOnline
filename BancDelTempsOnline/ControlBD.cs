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
using Gabriel.Cat.Extension;

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

		protected override void Restaurar()
		{

            Usuari[] usuaris;
            CertificatUsuari[] certificatsUsuari;
            ServeiCertificat[] serveisCertificat;
            ServeiUsuari[] serveisUsuaris ;
            MunicipiQueVolAnar[] municipisQueVolAnar;
            Certificat[] certificats;
            Servei[] serveis;
            LlistaOrdenada<string, Certificat> certificatsList;
            LlistaOrdenada<string, Servei> serveisList = new LlistaOrdenada<string, Servei>();
            LlistaOrdenada<string, Usuari> usuarisList = new LlistaOrdenada<string, Usuari>();

            usuaris = Usuari.TaulaToUsuariArray(BaseDeDades.ConsultaTableDirect(Usuari.TAULA));
            for (int i = 0; i < usuaris.Length; i++)
                usuarisList.Afegir(usuaris[i].NIE, usuaris[i]);

            certificats = Certificat.TaulaToCertificatsArray(BaseDeDades.ConsultaTableDirect(Certificat.TAULA),usuarisList);
            serveis = Servei.TaulaToServeisArray(BaseDeDades.ConsultaTableDirect(Servei.TAULA),usuarisList);
             certificatsList = new LlistaOrdenada<string, Certificat>();
            for (int i = 0; i < certificats.Length; i++)
                certificatsList.Afegir(certificats[i].Nom, certificats[i]);
            for (int i = 0; i < serveis.Length; i++)
                serveisList.Afegir(serveis[i].Nom, serveis[i]);
            certificatsUsuari = CertificatUsuari.TaulaToServeisUsuarisArray(BaseDeDades.ConsultaTableDirect(CertificatUsuari.TAULA),usuarisList,certificatsList);
            serveisCertificat =  ServeiCertificat.TaulaToServeisCertificatsArray(BaseDeDades.ConsultaTableDirect(ServeiCertificat.TAULA),usuarisList,serveisList,certificatsList);
            serveisUsuaris =  ServeiUsuari.TaulaToServeisUsuarisArray(BaseDeDades.ConsultaTableDirect(ServeiUsuari.TAULA),serveisList,usuarisList);
            municipisQueVolAnar =  MunicipiQueVolAnar.TaulaToMunicipisQueVolAnar(BaseDeDades.ConsultaTableDirect(MunicipiQueVolAnar.TAULA),usuarisList);
            //falta la part dels permisos i de la web
            //poso els objectes a la base de dades
            base.Afegir(usuaris);
            base.Afegir(certificats);
            base.Afegir(certificatsUsuari);
            base.Afegir(serveisCertificat);
            base.Afegir(serveisUsuaris);
            base.Afegir(municipisQueVolAnar);
            base.Afegir(serveis);
            //falta afegir part de permisos i web
        }

		#endregion
	}
}
