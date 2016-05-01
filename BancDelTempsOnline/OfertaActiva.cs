using Gabriel.Cat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancDelTempsOnline
{
  public  class OfertaActiva:ObjecteSqlIdAuto,IClauUnicaPerObjecte,IComparable<OfertaActiva>,IComparable
    {
        public enum Urgencia
        {
            Baixa,Norma,Alta,MoltAlta
        }
        enum CampsOfertaActiva
        {
            Id,Demandant,Titol,ServeiSolicitat,Descripcio,ImatgeOferta,Urgencia,QuiHaValidat,Inici
        }
        public static readonly TimeSpan TempsMaximOferta = new TimeSpan(30, 0, 0, 0, 0);//per decidir si caduca 
        public const int MAXLONGITUDDESCRIPCIO = 400;
        public const int MAXLONGITUDTITOL = 100;
        public const string TAULA = "OfertesActives";
        public const string CAMPPRYMARYKEY = "Id";
        public const int MAXLONGITUDIMATGEOFERTA = 250 * 1024;//250KB
        string idLocal;//es per poder posarlo en llistes hash i no tenir problemes ja que es un valor fix
        Usuari demandant;
        ListaUnica<Usuari> oferits;//fer taula per a ells i nova clase!!
        ListaUnica<Usuari> proposats;//fer taula per a ells i nova clase!!
        DateTime inici;
        string titol;//si no hi ha el servei s'utilitzarà el titol
        Servei serveiDemanat;
        string descripcio;
        Urgencia urgencia;
        Usuari quiHaValidat;
        Fitxer imatgeOferta;
        Llista<Fitxer> documentsAdjunts;//desso la string :)


        public OfertaActiva(Usuari demandant, DateTime inici, string titol, Servei servei, string descripcio, Fitxer imatgeOferta, Fitxer[] archiusAdjuntats) : this("", demandant,inici, titol, servei, descripcio, imatgeOferta, archiusAdjuntats,null,null,null) { }
        //camp documents adjunts
        private OfertaActiva(string id,Usuari demandant,DateTime inici,string titol,Servei servei,string descripcio,Fitxer imatgeOferta, Fitxer[] archiusAdjuntats,Usuari quiHaValidat,Usuari[] oferits,Usuari[] proposats):base(TAULA,id,CAMPPRYMARYKEY)
        {
            AltaCanvi(CampsOfertaActiva.Demandant.ToString());
            AltaCanvi(CampsOfertaActiva.Titol.ToString());
            AltaCanvi(CampsOfertaActiva.ServeiSolicitat.ToString());
            AltaCanvi(CampsOfertaActiva.Descripcio.ToString());
            AltaCanvi(CampsOfertaActiva.ImatgeOferta.ToString());
            AltaCanvi(CampsOfertaActiva.Urgencia.ToString());
            AltaCanvi(CampsOfertaActiva.QuiHaValidat.ToString());
            AltaCanvi(CampsOfertaActiva.Inici.ToString());
            this.oferits = new ListaUnica<Usuari>();
            this.proposats = new ListaUnica<Usuari>();
            documentsAdjunts = new Llista<Fitxer>();
            if (archiusAdjuntats != null)
                documentsAdjunts.AfegirMolts(archiusAdjuntats);
            if (oferits != null)
                this.oferits.Añadir(oferits);
            if (proposats != null)
                this.proposats.Añadir(proposats);
            this.quiHaValidat = quiHaValidat;
            this.demandant = demandant;
            this.titol = titol;
            this.serveiDemanat = servei;
            this.descripcio = descripcio;
            this.imatgeOferta = imatgeOferta;
            idLocal = MiRandom.Next() + "" + DateTime.Now.Ticks;
           
        }
        public Usuari Demandant
        {
            get
            {
                return demandant;
            }

            set
            {
                if (demandant == null)
                    throw new ArgumentNullException("No pot ser null");
                demandant = value;
                CanviString(CampsOfertaActiva.Demandant.ToString(), demandant.PrimaryKey);
            }
        }

        public ListaUnica<Usuari> Oferits
        {
            get
            {
                return oferits;
            }

        }

        public ListaUnica<Usuari> Proposats
        {
            get
            {
                return proposats;
            }

        }

        public DateTime Inici
        {
            get
            {
                return inici;
            }

            set
            {
                inici = value;
                CanviData(CampsOfertaActiva.Inici.ToString(), inici);
            }
        }

        public string Titol
        {
            get
            {
                return titol;
            }

            set
            {
                if (String.IsNullOrEmpty(value) || value.Length > MAXLONGITUDTITOL)
                    throw new ArgumentException();
                titol = value;
                CanviString(CampsOfertaActiva.Titol.ToString(), titol);
            }
        }

        public Servei ServeiDemanat
        {
            get
            {
                return serveiDemanat;
            }

            set
            {
                serveiDemanat = value;
                if(serveiDemanat!=null)
                {
                    CanviString(CampsOfertaActiva.ServeiSolicitat.ToString(), serveiDemanat.PrimaryKey);
                }else
                {
                    CanviString(CampsOfertaActiva.ServeiSolicitat.ToString(), null);
                }
            }
        }

        public string Descripcio
        {
            get
            {
                return descripcio;
            }

            set
            {
                if (String.IsNullOrEmpty(value) || value.Length > MAXLONGITUDDESCRIPCIO)
                    throw new ArgumentException();
                descripcio = value;
                CanviString(CampsOfertaActiva.Descripcio.ToString(), descripcio);
              
            }
        }

        public Urgencia NivellUrgencia
        {
            get
            {
                return urgencia;
            }

            set
            {
                urgencia = value;
                CanviString(CampsOfertaActiva.Urgencia.ToString(), urgencia.ToString());
            }
        }

        public Usuari QuiHaValidat
        {
            get
            {
                return quiHaValidat;
            }

            set
            {
                quiHaValidat = value;
                if (quiHaValidat != null)
                    CanviString(CampsOfertaActiva.QuiHaValidat.ToString(), quiHaValidat.PrimaryKey);
                else CanviString(CampsOfertaActiva.QuiHaValidat.ToString(), null);
            }
        }

        public Fitxer ImatgeOferta
        {
            get
            {
                return imatgeOferta;
            }

            set
            {
                string dades=null;
                if (value != null)
                {
                    dades = value.Archiu.ToStringBD();
                    if (dades.Length > MAXLONGITUDIMATGEOFERTA)
                        throw new ArgumentException("S'ha superat el limit de longitud per la imatge que és " + MAXLONGITUDIMATGEOFERTA);
                }
                imatgeOferta = value;
                CanviString(CampsOfertaActiva.ImatgeOferta.ToString(), dades);
                
            }
        }


        public Llista<Fitxer> DocumentsAdjunts
        {
            get
            {
                return documentsAdjunts;
            }
        }

        public IComparable Clau()
        {
            return idLocal;
        }

        public int CompareTo(OfertaActiva other)
        {
            int comparteTo;
            if (other != null)
                comparteTo = Clau().CompareTo(other.Clau());
            else comparteTo = -1;
            return comparteTo;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as OfertaActiva);
        }

        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {//posar nomes el id del archiu ja que les dades es desaran a la taula d'arxius
            throw new NotImplementedException();
        }
    }
}
