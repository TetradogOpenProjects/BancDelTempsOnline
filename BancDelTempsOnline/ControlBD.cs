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
    {//comprobar que tot tingui el seu create,ObjecteNou,Restaurar bé
        //faltan opcions,part htmlMenu,part htmlCosOpcio,PermisMinim
        //falta  banners per prohibir opcions

        //falten taulesHistorial de cada taula
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
        TwoKeysList<string,string, Usuari> usuarisList;//idLocalUnic,NIE,Usuari
        TwoKeysList<string, string, Usuari> usuarisList2;//Email,NIE,usuari
        LlistaOrdenada<string, Missatge> missatgesList;
        TwoKeysList<string,string, Fitxer> fitxersList;
        LlistaOrdenada<string, OfertaActiva> ofertesActivesList;
        static ControlBD()
        {
            creates = new string[] {
                Fitxer.StringCreateTable(),
                Usuari.StringCreateTable(),
                Certificat.StringCreateTable(),
                Servei.StringCreateTable(),
                CertificatUsuari.StringCreateTable(),
                ServeiCertificat.StringCreateTable(),
                ServeiUsuari.StringCreateTable(),
                MunicipiQueVolAnar.StringCreateTable(),
                Missatge.StringCreateTable(),
                OfertaTencada.StringCreateTable(),
                OfertaActiva.StringCreateTable(),
                UsuariPerLaOferta.StringCreateTable(),
            };
        }

        public ControlBD(BaseDeDades baseDeDades) : base(baseDeDades, creates)
        {
            //faig els new de les llistes en la zona de restaurar perque es cridarà avanç!!
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
            OfertaActiva ofertaActiva;
            Fitxer fitxer;
            Usuari usuari;
            UsuariPerLaOferta usuariPerLaOferta;
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
                usuari = (Usuari)obj;
                if (!usuarisList.ContainsKey2(usuari.PrimaryKey))
                {
                    usuarisList.Add(usuari.IdLocalUnic, usuari.PrimaryKey, usuari);
                    usuarisList2.Add(usuari.Email, usuari.PrimaryKey, usuari);
                    usuari.Actualitzat += (objActualitzat)=>{
                        Usuari usuariActualitzat = (Usuari)objActualitzat;
                        //si ha canviat d'email l'haig de renovar
                        if (!usuarisList2.ContainsKey1(usuariActualitzat.Email))
                        {
                            usuarisList2.Remove2(usuariActualitzat.PrimaryKey);
                            usuarisList2.Add(usuariActualitzat.Email, usuariActualitzat.PrimaryKey, usuariActualitzat);
                        }
                    };
                }
            }
            else if (obj is Missatge)
            {
                missatge = (Missatge)obj;
                if (!missatgesList.Existeix(missatge.PrimaryKey))
                {
                    missatgesList.Afegir(missatge.PrimaryKey, missatge);
                    usuarisList.ObtainValueWithKey2(missatge.UsuariEmisor.PrimaryKey).AfegirMissatge(missatge);
                    usuarisList.ObtainValueWithKey2(missatge.UsuariReceptor.PrimaryKey).AfegirMissatge(missatge);
                }
            }
            else if (obj is MunicipiQueVolAnar)
            {
                municipi = (MunicipiQueVolAnar)obj;
                usuarisList.ObtainValueWithKey2(municipi.Usuari.PrimaryKey).AfegirMunicipiQueVolAnar(municipi);
            }
            else if (obj is CertificatUsuari)
            {
                certificatUsuari = (CertificatUsuari)obj;
                if (!usuarisList.ObtainValueWithKey2(certificatUsuari.Usuari.PrimaryKey).Certificats.ExisteObjeto(certificatUsuari))
                    usuarisList.ObtainValueWithKey2(certificatUsuari.Usuari.PrimaryKey).Certificats.Añadir(certificatUsuari);
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
                if (!usuarisList.ObtainValueWithKey2(serveiUsuari.Usuari.PrimaryKey).ServeisSenseCertificat.ExisteObjeto(serveiUsuari))
                    usuarisList.ObtainValueWithKey2(serveiUsuari.Usuari.PrimaryKey).ServeisSenseCertificat.Añadir(serveiUsuari);
            }else if(obj is OfertaTencada)
            {
                ofertaTencada = (OfertaTencada)obj;
                if (!usuarisList.ObtainValueWithKey2(ofertaTencada.Demandant.PrimaryKey).OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList.ObtainValueWithKey2(ofertaTencada.Demandant.PrimaryKey).OfertesTencades.Añadir(ofertaTencada);
                if (!usuarisList.ObtainValueWithKey2(ofertaTencada.Ofert.PrimaryKey).OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList.ObtainValueWithKey2(ofertaTencada.Ofert.PrimaryKey).OfertesTencades.Añadir(ofertaTencada);
            }else if(obj is Fitxer)
            {
                fitxer = (Fitxer)obj;
                if (!fitxersList.ContainsKey2(fitxer.PrimaryKey))
                    fitxersList.Add(fitxer.NomAmbFormat,fitxer.PrimaryKey, fitxer);
            }else if(obj is OfertaActiva)
            {
                ofertaActiva = (OfertaActiva)obj;
                if(!usuarisList.ObtainValueWithKey2(ofertaActiva.Demandant.PrimaryKey).OfertesActives.ExisteObjeto(ofertaActiva))
                  usuarisList.ObtainValueWithKey2(ofertaActiva.Demandant.PrimaryKey).OfertesActives.Añadir(ofertaActiva);
            }else if(obj is UsuariPerLaOferta)
            {
                usuariPerLaOferta = (UsuariPerLaOferta)obj;
                ofertesActivesList[usuariPerLaOferta.Oferta.PrimaryKey].UsuarisPerLaOferta.Añadir(usuariPerLaOferta);
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
            UsuariPerLaOferta usuariPerLaOferta;
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
                if (usuarisList.ContainsKey2(obj.PrimaryKey))
                {
                    usuarisList.Remove2(obj.PrimaryKey);
                    usuarisList2.Remove2(obj.PrimaryKey);
                }
            }
            else if (obj is Missatge)
            {
                if (missatgesList.Existeix(obj.PrimaryKey))
                    missatgesList.Elimina(obj.PrimaryKey);
            }
            else if (obj is MunicipiQueVolAnar)
            {
                municipi = (MunicipiQueVolAnar)obj;
                usuarisList.ObtainValueWithKey2(municipi.Usuari.PrimaryKey).TreuMunicipiQuePotAnar(municipi.Municipi);
            }
            else if (obj is CertificatUsuari)
            {
                certificatUsuari = (CertificatUsuari)obj;
                if (usuarisList.ObtainValueWithKey2(certificatUsuari.Usuari.PrimaryKey).Certificats.ExisteObjeto(certificatUsuari))
                    usuarisList.ObtainValueWithKey2(certificatUsuari.Usuari.PrimaryKey).Certificats.Elimina(obj.PrimaryKey);
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
                if (usuarisList.ObtainValueWithKey2(serveiUsuari.Usuari.PrimaryKey).ServeisSenseCertificat.ExisteObjeto(serveiUsuari))
                    usuarisList.ObtainValueWithKey2(serveiUsuari.Usuari.PrimaryKey).ServeisSenseCertificat.Elimina(obj.PrimaryKey);
            }
            else if (obj is OfertaTencada)
            {
                ofertaTencada = (OfertaTencada)obj;
                if (usuarisList.ObtainValueWithKey2(ofertaTencada.Demandant.PrimaryKey).OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList.ObtainValueWithKey2(ofertaTencada.Demandant.PrimaryKey).OfertesTencades.Elimina(ofertaTencada);
                if (usuarisList.ObtainValueWithKey2(ofertaTencada.Ofert.PrimaryKey).OfertesTencades.ExisteObjeto(ofertaTencada))
                    usuarisList.ObtainValueWithKey2(ofertaTencada.Ofert.PrimaryKey).OfertesTencades.Elimina(ofertaTencada);
            }
            else if(obj is UsuariPerLaOferta)
            {
                usuariPerLaOferta = (UsuariPerLaOferta)obj;
                ofertesActivesList[usuariPerLaOferta.Oferta.PrimaryKey].UsuarisPerLaOferta.EliminaObjeto(usuariPerLaOferta);
            }
            //per acabar de posar els nous

        }
        //aqui es posa totes les llistes que hi hagi per poder accedir a elles facilment
        public Certificat ObtéCertificat(string primaryKey)
        {
            return certificatsList[primaryKey];
        }
        public Usuari ObtéUsuariNIE(string primaryKey)
        {
            return usuarisList.ObtainValueWithKey2(primaryKey);
        }
        public Usuari ObtéUsuariEmail(string emailUser)
        {
            return usuarisList2.ObtainValueWithKey1(emailUser);
        }
        public Usuari ObtéUsuariId(string idUnicLocal)
        {
            return usuarisList.ObtainValueWithKey1(idUnicLocal);
        }
        public Servei ObtéServei(string primaryKey)
        {
            return serveisList[primaryKey];
        }
        public Missatge ObtéMissatge(string primaryKey)
        {
            return missatgesList[primaryKey];
        }
        public OfertaActiva ObtéOfertaActiva(string primaryKey)
        {
            return ofertesActivesList[primaryKey];
        }
        public Fitxer ObtéFitxerId(string idFitxer)
        {
            return fitxersList.ObtainValueWithKey2(idFitxer);
        }
        public Fitxer ObtéFitxerNom(string nomFitxer)
        {
            return fitxersList.ObtainValueWithKey1(nomFitxer);
        }
        #region implemented abstract members of ControlObjectesSql

        protected override void Restaurar()
        {//falta explicar cada part per no perdres
            //per acabar de posar els nous
            Missatge[] missatges;
            Usuari[] usuaris;
            CertificatUsuari[] certificatsUsuari;
            ServeiCertificat[] serveisCertificat;
            ServeiUsuari[] serveisUsuaris;
            MunicipiQueVolAnar[] municipisQueVolAnar;
            Certificat[] certificats;
            Servei[] serveis;
            OfertaTencada[] ofertesTencades;
            OfertaActiva[] ofertesActives;
            UsuariPerLaOferta[] usuarisOferets;
            Fitxer[] fitxers = Fitxer.TaulaToFitxers(BaseDeDades.ConsultaTableDirect(Fitxer.TAULA));

            certificatsList = new LlistaOrdenada<string, Certificat>();
            serveisList = new LlistaOrdenada<string, Servei>();
            usuarisList = new TwoKeysList<string, string, Usuari>();
            missatgesList = new LlistaOrdenada<string, Missatge>();
            fitxersList = new TwoKeysList<string, string, Fitxer>();
            ofertesActivesList = new LlistaOrdenada<string, OfertaActiva>();
            usuarisList2 = new TwoKeysList<string, string, Usuari>();
            for (int i = 0; i < fitxers.Length; i++)
                this.fitxersList.Add(fitxers[i].NomAmbFormat,fitxers[i].PrimaryKey, fitxers[i]);
            usuaris = Usuari.TaulaToUsuaris(BaseDeDades.ConsultaTableDirect(Usuari.TAULA),this.fitxersList);
            for (int i = 0; i < usuaris.Length; i++)
            {
                usuarisList.Add(usuaris[i].IdLocalUnic, usuaris[i].NIE, usuaris[i]);
                usuarisList2.Add(usuaris[i].Email, usuaris[i].NIE, usuaris[i]);
            }

            certificats = Certificat.TaulaToCertificats(BaseDeDades.ConsultaTableDirect(Certificat.TAULA), usuarisList);
            serveis = Servei.TaulaToServeis(BaseDeDades.ConsultaTableDirect(Servei.TAULA), usuarisList,this.fitxersList);

            for (int i = 0; i < certificats.Length; i++)
                certificatsList.Afegir(certificats[i].Nom, certificats[i]);
            for (int i = 0; i < serveis.Length; i++)
                serveisList.Afegir(serveis[i].Nom, serveis[i]);
            certificatsUsuari = CertificatUsuari.TaulaToServeisUsuaris(BaseDeDades.ConsultaTableDirect(CertificatUsuari.TAULA), usuarisList, certificatsList);
            serveisCertificat = ServeiCertificat.TaulaToServeisCertificats(BaseDeDades.ConsultaTableDirect(ServeiCertificat.TAULA), usuarisList, serveisList, certificatsList);
            serveisUsuaris = ServeiUsuari.TaulaToServeisUsuaris(BaseDeDades.ConsultaTableDirect(ServeiUsuari.TAULA), serveisList, usuarisList);
            municipisQueVolAnar = MunicipiQueVolAnar.TaulaToMunicipisQueVolAnar(BaseDeDades.ConsultaTableDirect(MunicipiQueVolAnar.TAULA), usuarisList);
            missatges = Missatge.TaulaToMissatges(BaseDeDades.ConsultaTableDirect(Missatge.TAULA), usuarisList,this.fitxersList);
            ofertesTencades = OfertaTencada.TaulaToOfertesTencades(BaseDeDades.ConsultaTableDirect(OfertaTencada.TAULA), usuarisList);
            usuarisOferets = UsuariPerLaOferta.TaulaToUsuarisPerLaOfertes(BaseDeDades.ConsultaTableDirect(UsuariPerLaOferta.TAULA), usuarisList);//nomes tenen el id de la oferta cal completar
            ofertesActives = OfertaActiva.TaulaToOfertesActives(BaseDeDades.ConsultaTableDirect(OfertaActiva.TAULA), usuarisList,usuarisOferets,serveisList, fitxersList);//es mes complexa!!
            for (int i = 0; i < ofertesActives.Length; i++)
                ofertesActivesList.Afegir(ofertesActives[i].PrimaryKey, ofertesActives[i]);
            UsuariPerLaOferta.PosaOfertes(usuarisOferets, ofertesActivesList);//poso els objectes on toca :)
            //falta la part dels permisos i de la web
            //poso els objectes a la base de dades
            base.Afegir(usuarisOferets);
            base.Afegir(fitxers);
            base.Afegir(ofertesActives);
            base.Afegir(ofertesTencades);
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
