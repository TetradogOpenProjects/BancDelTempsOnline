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
    public class ControlBD : ControlObjectesSql
    {
        //faltan opcions,part htmlMenu,part htmlCosOpcio,PermisMinim
        //falta  banners per prohibir opcions

        //falten taulesHistorial de cada taula
        //falte taulaInterCanvis
        //falte  taulaOfertes
        //falte taulaUsuarisOfertats//es una taula on surten els usuaris proposats per una oferta
        //falte taula objectesOfertatsPrestats
        //falte taula tallers!!

        //estaria be que les opcions estigesisn al sql??per poder afegir,treure,canviar desde el navegador la propia pagina web??
        //aixi es poden fer canvis sense aturar el servei i ho pot fer un admin mateix :D
        //fer variables que representin els camps de l'usuari aixi es un model per reemplaçar els camps pels que toquin

        //si canvia l'email s'ha de donar de baixa el permis de googleplus l'anterior
        //taula missatges usuariEmissor,usuariReceptor,Misstage

        static string[] creates;
        //per poder tenir tots els objectes rapidament els posem en llistes :)
        LlistaOrdenada<string, Certificat> certificatsList;
        LlistaOrdenada<string, Servei> serveisList;
        LlistaOrdenada<string, Usuari> usuarisList;
        LlistaOrdenada<string, Missatge> missatgesList;

        static ControlBD()
        {
            creates = new string[] {
                Usuari.StringCreateTable(),
                Certificat.StringCreateTable(),
                Servei.StringCreateTable(),
                CertificatUsuari.StringCreateTable(),
                ServeiCertificat.StringCreateTable(),
                ServeiUsuari.StringCreateTable(),
                MunicipiQueVolAnar.StringCreateTable(),
                Missatge.StringCreateTable()
            };
        }

        public ControlBD(BaseDeDades baseDeDades) : base(baseDeDades, creates)
        {
            certificatsList = new LlistaOrdenada<string, Certificat>();
            serveisList = new LlistaOrdenada<string, Servei>();
            usuarisList = new LlistaOrdenada<string, Usuari>();
            missatgesList = new LlistaOrdenada<string, Missatge>();
            base.ObjNou += PosaObjecte;//aixi es mes comode perque no s'ha de fer després nomes s'ha d'afegir i llestos :)

        }

        private void PosaObjecte(ObjecteSql obj)
        {
            //s'ha de incloure el si existeix perque al restaurar hi hauria problemes!!

            //si hi han nous tipos d'objectes es posen aqui
            MunicipiQueVolAnar municipi;
            CertificatUsuari certificatUsuari;
            ServeiCertificat serveiCertificat;
            ServeiUsuari serveiUsuari;
            Missatge missatge;
            OfertaTencada ofertaTencada;
            if (obj is Certificat)
            {
                if (!certificatsList.Existeix(obj.PrimaryKey))
                    certificatsList.Afegir(obj.PrimaryKey, obj as Certificat);
            }
            else if (obj is Servei)
            {
                if (!serveisList.Existeix(obj.PrimaryKey))
                    serveisList.Afegir(obj.PrimaryKey, obj as Servei);
            }
            else if (obj is Usuari)
            {
                if (!usuarisList.Existeix(obj.PrimaryKey))
                    usuarisList.Afegir(obj.PrimaryKey, obj as Usuari);
            }
            else if (obj is Missatge)
            {
                missatge = (Missatge)obj;
                if (!missatgesList.Existeix(missatge.PrimaryKey))
                {
                    missatgesList.Afegir(missatge.PrimaryKey, missatge);
                    usuarisList[missatge.UsuariEmisor.PrimaryKey].AfegirMissatge(missatge);
                    usuarisList[missatge.UsuariReceptor.PrimaryKey].AfegirMissatge(missatge);
                }
            }
            else if (obj is MunicipiQueVolAnar)
            {
                municipi = (MunicipiQueVolAnar)obj;
                usuarisList[municipi.Usuari.PrimaryKey].AfegirMunicipiQueVolAnar(municipi);
            }
            else if (obj is CertificatUsuari)
            {
                certificatUsuari = (CertificatUsuari)obj;
                if (!usuarisList[certificatUsuari.Usuari.PrimaryKey].Certificats.ExisteObjeto(certificatUsuari))
                    usuarisList[certificatUsuari.Usuari.PrimaryKey].Certificats.Añadir(certificatUsuari);
            }
            else if (obj is ServeiCertificat)
            {
                serveiCertificat = (ServeiCertificat)obj;
                if (!certificatsList[serveiCertificat.Certificat.PrimaryKey].Serveis.ExisteObjeto(serveiCertificat))
                    certificatsList[serveiCertificat.Certificat.PrimaryKey].Serveis.Añadir(serveiCertificat);
            }
            else if (obj is ServeiUsuari)
            {
                serveiUsuari = (ServeiUsuari)obj;
                if (!usuarisList[serveiUsuari.Usuari.PrimaryKey].ServeisSenseCertificat.ExisteObjeto(serveiUsuari))
                    usuarisList[serveiUsuari.Usuari.PrimaryKey].ServeisSenseCertificat.Añadir(serveiUsuari);
            }else if(obj is OfertaTencada)
            {
                ofertaTencada = (OfertaTencada)obj;
                if (!usuarisList[ofertaTencada.Demandant.PrimaryKey].OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList[ofertaTencada.Demandant.PrimaryKey].OfertesTencades.Añadir(ofertaTencada);
                if (!usuarisList[ofertaTencada.Ofert.PrimaryKey].OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList[ofertaTencada.Ofert.PrimaryKey].OfertesTencades.Añadir(ofertaTencada);
            }

            obj.Baixa += TreuObjecte;
        }

        private void TreuObjecte(ObjecteSql obj)
        {

            //si hi han nous tipos d'objectes es posen aqui
            MunicipiQueVolAnar municipi;
            CertificatUsuari certificatUsuari;
            ServeiCertificat serveiCertificat;
            ServeiUsuari serveiUsuari;
            OfertaTencada ofertaTencada;
            if (obj is Certificat)
            {
                if (certificatsList.Existeix(obj.PrimaryKey))
                    certificatsList.Elimina(obj.PrimaryKey);
            }
            else if (obj is Servei)
            {
                if (serveisList.Existeix(obj.PrimaryKey))
                    serveisList.Elimina(obj.PrimaryKey); ;
            }
            else if (obj is Usuari)
            {
                if (usuarisList.Existeix(obj.PrimaryKey))
                    usuarisList.Elimina(obj.PrimaryKey);
            }
            else if (obj is Missatge)
            {
                if (missatgesList.Existeix(obj.PrimaryKey))
                    missatgesList.Elimina(obj.PrimaryKey);
            }
            else if (obj is MunicipiQueVolAnar)
            {
                municipi = (MunicipiQueVolAnar)obj;
                usuarisList[municipi.Usuari.PrimaryKey].TreuMunicipiQuePotAnar(municipi.Municipi);
            }
            else if (obj is CertificatUsuari)
            {
                certificatUsuari = (CertificatUsuari)obj;
                if (usuarisList[certificatUsuari.Usuari.PrimaryKey].Certificats.ExisteObjeto(certificatUsuari))
                    usuarisList[certificatUsuari.Usuari.PrimaryKey].Certificats.Elimina(obj.PrimaryKey);
            }
            else if (obj is ServeiCertificat)
            {
                serveiCertificat = (ServeiCertificat)obj;
                if (certificatsList[serveiCertificat.Certificat.PrimaryKey].Serveis.ExisteObjeto(serveiCertificat))
                    certificatsList[serveiCertificat.Certificat.PrimaryKey].Serveis.Elimina(obj.PrimaryKey);
            }
            else if (obj is ServeiUsuari)
            {
                serveiUsuari = (ServeiUsuari)obj;
                if (usuarisList[serveiUsuari.Usuari.PrimaryKey].ServeisSenseCertificat.ExisteObjeto(serveiUsuari))
                    usuarisList[serveiUsuari.Usuari.PrimaryKey].ServeisSenseCertificat.Elimina(obj.PrimaryKey);
            }
            else if (obj is OfertaTencada)
            {
                ofertaTencada = (OfertaTencada)obj;
                if (usuarisList[ofertaTencada.Demandant.PrimaryKey].OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList[ofertaTencada.Demandant.PrimaryKey].OfertesTencades.Elimina(ofertaTencada);
                if (usuarisList[ofertaTencada.Ofert.PrimaryKey].OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList[ofertaTencada.Ofert.PrimaryKey].OfertesTencades.Elimina(ofertaTencada);
            }//per acabar de posar els nous

        }
        //aqui es posa totes les llistes que hi hagi per poder accedir a elles facilment
        public Certificat ObtéCertificat(string primaryKey)
        {
            return certificatsList[primaryKey];
        }
        public Usuari ObtéUsuari(string primaryKey)
        {
            return usuarisList[primaryKey];
        }
        public Servei ObtéServei(string primaryKey)
        {
            return serveisList[primaryKey];
        }
        public Missatge ObtéMissatge(string primaryKey)
        {
            return missatgesList[primaryKey];
        }

        #region implemented abstract members of ControlObjectesSql

        protected override void Restaurar()
        {//per acabar de posar els nous
            Missatge[] missatges;
            Usuari[] usuaris;
            CertificatUsuari[] certificatsUsuari;
            ServeiCertificat[] serveisCertificat;
            ServeiUsuari[] serveisUsuaris;
            MunicipiQueVolAnar[] municipisQueVolAnar;
            Certificat[] certificats;
            Servei[] serveis;

            usuaris = Usuari.TaulaToUsuariArray(BaseDeDades.ConsultaTableDirect(Usuari.TAULA));
            for (int i = 0; i < usuaris.Length; i++)
                usuarisList.Afegir(usuaris[i].NIE, usuaris[i]);

            certificats = Certificat.TaulaToCertificatsArray(BaseDeDades.ConsultaTableDirect(Certificat.TAULA), usuarisList);
            serveis = Servei.TaulaToServeisArray(BaseDeDades.ConsultaTableDirect(Servei.TAULA), usuarisList);

            for (int i = 0; i < certificats.Length; i++)
                certificatsList.Afegir(certificats[i].Nom, certificats[i]);
            for (int i = 0; i < serveis.Length; i++)
                serveisList.Afegir(serveis[i].Nom, serveis[i]);
            certificatsUsuari = CertificatUsuari.TaulaToServeisUsuarisArray(BaseDeDades.ConsultaTableDirect(CertificatUsuari.TAULA), usuarisList, certificatsList);
            serveisCertificat = ServeiCertificat.TaulaToServeisCertificatsArray(BaseDeDades.ConsultaTableDirect(ServeiCertificat.TAULA), usuarisList, serveisList, certificatsList);
            serveisUsuaris = ServeiUsuari.TaulaToServeisUsuarisArray(BaseDeDades.ConsultaTableDirect(ServeiUsuari.TAULA), serveisList, usuarisList);
            municipisQueVolAnar = MunicipiQueVolAnar.TaulaToMunicipisQueVolAnar(BaseDeDades.ConsultaTableDirect(MunicipiQueVolAnar.TAULA), usuarisList);
            missatges = Missatge.TaulaToMissatges(BaseDeDades.ConsultaTableDirect(Missatge.TAULA), usuarisList);
            //falta la part dels permisos i de la web
            //poso els objectes a la base de dades
            base.Afegir(missatges);
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
