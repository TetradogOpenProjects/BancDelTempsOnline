using Gabriel.Cat;
using Gabriel.Cat.Extension;
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
        ListaUnica<UsuariPerLaOferta> usuarisPerLaOferta;//fer taula per a ells i nova clase!!

        DateTime inici;
        string titol;//si no hi ha el servei s'utilitzarà el titol
        Servei serveiDemanat;
        string descripcio;
        Urgencia urgencia;
        Usuari quiHaValidat;
        Fitxer imatgeOferta;
        Llista<Fitxer> documentsAdjunts;//desso la string :)


        public OfertaActiva(Usuari demandant, DateTime inici, string titol, Servei servei, string descripcio, Fitxer imatgeOferta, Fitxer[] archiusAdjuntats) : this("", demandant,inici, titol, servei, descripcio, imatgeOferta, archiusAdjuntats,null,null) { }
        //camp documents adjunts
        private OfertaActiva(string id,Usuari demandant,DateTime inici,string titol,Servei servei,string descripcio,Fitxer imatgeOferta, Fitxer[] archiusAdjuntats,Usuari quiHaValidat,UsuariPerLaOferta[] usuarisOferta) :base(TAULA,id,CAMPPRYMARYKEY)
        {
            AltaCanvi(CampsOfertaActiva.Demandant.ToString());
            AltaCanvi(CampsOfertaActiva.Titol.ToString());
            AltaCanvi(CampsOfertaActiva.ServeiSolicitat.ToString());
            AltaCanvi(CampsOfertaActiva.Descripcio.ToString());
            AltaCanvi(CampsOfertaActiva.ImatgeOferta.ToString());
            AltaCanvi(CampsOfertaActiva.Urgencia.ToString());
            AltaCanvi(CampsOfertaActiva.QuiHaValidat.ToString());
            AltaCanvi(CampsOfertaActiva.Inici.ToString());

            documentsAdjunts = new Llista<Fitxer>();
            if (archiusAdjuntats != null)
                documentsAdjunts.AfegirMolts(archiusAdjuntats);
            this.quiHaValidat = quiHaValidat;
            this.demandant = demandant;
            this.titol = titol;
            this.serveiDemanat = servei;
            this.descripcio = descripcio;
            this.imatgeOferta = imatgeOferta;
            idLocal = MiRandom.Next() + "" + DateTime.Now.Ticks;
            usuarisPerLaOferta = new ListaUnica<UsuariPerLaOferta>();
            if (usuarisOferta != null)
                usuarisPerLaOferta.Añadir(usuarisOferta);
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

        public ListaUnica<UsuariPerLaOferta> UsuarisPerLaOferta
        {
            get
            {
                return usuarisPerLaOferta;
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
            string sentencia = "insert into " + TAULA + "(" + CampsOfertaActiva.Demandant.ToString() + "," + CampsOfertaActiva.Titol.ToString() + "," + CampsOfertaActiva.Urgencia.ToString() + "," + CampsOfertaActiva.Descripcio.ToString() + "," + CampsOfertaActiva.ImatgeOferta.ToString() + "," + CampsOfertaActiva.Inici.ToString() + "," + CampsOfertaActiva.QuiHaValidat.ToString() + ") values(";
            sentencia += "'" + Demandant.PrimaryKey + "',";
            sentencia += "'" + Titol + "'";
            sentencia += "'" + NivellUrgencia.ToString() + "',";
            sentencia += "'" + Descripcio + "',";
            sentencia += (ImatgeOferta!=null?ImatgeOferta.PrimaryKey:"null") + ",";
            sentencia += ObjecteSql.DateTimeToStringSQL(tipusBD, Inici);
            sentencia += "" + (QuiHaValidat != null ? "'" + QuiHaValidat.PrimaryKey + "'" : "null") + ");";
            return sentencia;
        }
        public static string StringCreateTable()
        {
            string sentencia = "create table " + TAULA + " (";
            sentencia += CampsOfertaActiva.Id.ToString() + " int Auto_Increment primarykey,";
            sentencia += CampsOfertaActiva.Demandant.ToString() + " varchar(10) not null references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsOfertaActiva.Titol.ToString() + " varchar(" + MAXLONGITUDTITOL + ") not null,";
            sentencia += CampsOfertaActiva.Urgencia.ToString() + " varchar(15) not null,";
            sentencia += CampsOfertaActiva.Descripcio.ToString() + " varchar(" + MAXLONGITUDDESCRIPCIO + ") not null,";
            sentencia += CampsOfertaActiva.ImatgeOferta.ToString() + " int references " + Fitxer.TAULA + "(" + Fitxer.CAMPPRIMARYKEY + "),";
            sentencia += CampsOfertaActiva.Inici.ToString() + " date not null,";
            sentencia+=CampsOfertaActiva.QuiHaValidat.ToString()+" varchar(10) references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "));";
            return sentencia;
        }

        public static OfertaActiva[] TaulaToOfertesActives(string[,] taulaOfertesActives, LlistaOrdenada<string, Usuari> usuarisList,UsuariPerLaOferta[] usuarisOferts, LlistaOrdenada<string, Servei> serveis, LlistaOrdenada<string, Fitxer> fitxers)
        {
            OfertaActiva[] ofertesActives = new OfertaActiva[taulaOfertesActives.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < ofertesActives.Length; i++)
            {
                ofertesActives[i] = new OfertaActiva(taulaOfertesActives[(int)CampsOfertaActiva.Id, i], usuarisList[taulaOfertesActives[(int)CampsOfertaActiva.Demandant, i]],ObjecteSql.StringToDateTime(taulaOfertesActives[(int)CampsOfertaActiva.Inici, i]), taulaOfertesActives[(int)CampsOfertaActiva.Titol, i], serveis[taulaOfertesActives[(int)CampsOfertaActiva.ServeiSolicitat, i]], taulaOfertesActives[(int)CampsOfertaActiva.Descripcio, i], fitxers[taulaOfertesActives[(int)CampsOfertaActiva.ImatgeOferta, i]],fitxers.Filtra((fitxer)=> { return fitxer.Value.IdOnSutilitza == taulaOfertesActives[(int)CampsOfertaActiva.Id, i]; }).ValuesToArray(),usuarisList[taulaOfertesActives[(int)CampsOfertaActiva.QuiHaValidat,i]],usuarisOferts.Filtra((usuariOfert)=> { return usuariOfert.OfertaId == taulaOfertesActives[(int)CampsOfertaActiva.Id, i]; }).ToArray());
                usuarisList[ofertesActives[i].Demandant.PrimaryKey].OfertesActives.Añadir(ofertesActives[i]);
            }
            return ofertesActives;
        }
    }
    public class UsuariPerLaOferta:ObjecteSqlIdAuto,IClauUnicaPerObjecte
    {
       public  enum ComEsVaAfegir
        {
            Interesantse,Propusat
        }
        enum CampsUsuariPerLaOferta
        {
            Id,OfertaId,UsuariId,ComVanAfegirse,Data
        }
        public const string TAULA = "UsuarisPerLaOferta";
        public const string CAMPPRIMARYKEY = "Id";
        OfertaActiva oferta;
        ComEsVaAfegir comVanAfegirse;
        Usuari usuariAfegit;
        DateTime dataAfegit;
        string idLocal;
        public UsuariPerLaOferta(OfertaActiva oferta, Usuari usuari, ComEsVaAfegir comEsVaAfegir, DateTime data) : this("", oferta, usuari, comEsVaAfegir, data) { }
        private UsuariPerLaOferta(string id,OfertaActiva oferta,Usuari usuari,ComEsVaAfegir comEsVaAfegir,DateTime data):base(TAULA,id,CAMPPRIMARYKEY)
        {
            base.AltaCanvi(CampsUsuariPerLaOferta.OfertaId.ToString());
            base.AltaCanvi(CampsUsuariPerLaOferta.UsuariId.ToString());
            base.AltaCanvi(CampsUsuariPerLaOferta.ComVanAfegirse.ToString());
            base.AltaCanvi(CampsUsuariPerLaOferta.Data.ToString());
            this.oferta=oferta;
            if (oferta != null) this.OfertaId = oferta.PrimaryKey;
            this.comVanAfegirse=comEsVaAfegir;
            this.usuariAfegit=usuari;
            this.dataAfegit=data;
            idLocal = MiRandom.Next() + "" + DateTime.Now.Ticks;
        }
        public OfertaActiva Oferta
        {
            get
            {
                return oferta;
            }

            set
            {
                if (value == null) throw new ArgumentNullException();
                oferta = value;
                OfertaId = oferta.PrimaryKey;
                CanviString(CampsUsuariPerLaOferta.OfertaId.ToString(), oferta.PrimaryKey);
            }
        }

        public ComEsVaAfegir ComVanAfegirse
        {
            get
            {
                return comVanAfegirse;
            }

            set
            {
                comVanAfegirse = value;
                CanviString(CampsUsuariPerLaOferta.ComVanAfegirse.ToString(), comVanAfegirse.ToString());
            }
        }

        public Usuari UsuariAfegit
        {
            get
            {
                return usuariAfegit;
            }

            set
            {
                if (value == null) throw new ArgumentNullException();
                usuariAfegit = value;
                CanviString(CampsUsuariPerLaOferta.UsuariId.ToString(), usuariAfegit.PrimaryKey);
            }
        }

        public DateTime DataAfegit
        {
            get
            {
                return dataAfegit;
            }

            set
            {
                dataAfegit = value;
                CanviData(CampsUsuariPerLaOferta.Data.ToString(), dataAfegit);
            }
        }
        public IComparable Clau()
        {
            return idLocal;
        }
        public string OfertaId { get; private set; }

        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {
            string sentencia = "insert into " + TAULA + "(" + CampsUsuariPerLaOferta.OfertaId.ToString() + "," + CampsUsuariPerLaOferta.UsuariId.ToString() + "," + CampsUsuariPerLaOferta.ComVanAfegirse.ToString() + "," + CampsUsuariPerLaOferta.Data.ToString() + ") values(";
            sentencia += "'" + Oferta.PrimaryKey + "',";
            sentencia += "'" + UsuariAfegit.PrimaryKey + "',";
            sentencia += "'" + ComVanAfegirse.ToString() + "',";
            sentencia += ObjecteSql.DateTimeToStringSQL(tipusBD, dataAfegit)+");";
            return sentencia;
        }
        public static string StringCreateTable()
        {
            string sentencia = "create table " + TAULA + "(";
            sentencia += CampsUsuariPerLaOferta.Id.ToString() + " int Auto_Increment primarykey,";
            sentencia += CampsUsuariPerLaOferta.OfertaId.ToString() + " int not null references " + OfertaActiva.TAULA + "(" + OfertaActiva.CAMPPRYMARYKEY + "),";
            sentencia += CampsUsuariPerLaOferta.UsuariId.ToString() + " varchar(10) not null references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsUsuariPerLaOferta.ComVanAfegirse.ToString() + " varchar(15) not null,";
            sentencia += CampsUsuariPerLaOferta.Data.ToString() + " date not null);";
            return sentencia;
        }
        public static UsuariPerLaOferta[] TaulaToUsuarisPerLaOfertes(string[,] taulaUsuarisPerLaOfera,LlistaOrdenada<string,Usuari> usuarisList)
        {
            UsuariPerLaOferta[] usuarisPerLesOfertes = new UsuariPerLaOferta[taulaUsuarisPerLaOfera.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<usuarisPerLesOfertes.Length;i++)
            {
                usuarisPerLesOfertes[i] = new UsuariPerLaOferta(taulaUsuarisPerLaOfera[(int)CampsUsuariPerLaOferta.Id, i], null, usuarisList[taulaUsuarisPerLaOfera[(int)CampsUsuariPerLaOferta.UsuariId, i]], (ComEsVaAfegir)Enum.Parse(typeof(ComEsVaAfegir), taulaUsuarisPerLaOfera[(int)CampsUsuariPerLaOferta.ComVanAfegirse, i]), ObjecteSql.StringToDateTime(taulaUsuarisPerLaOfera[(int)CampsUsuariPerLaOferta.Data, i])) { OfertaId = taulaUsuarisPerLaOfera[(int)CampsUsuariPerLaOferta.OfertaId, i] };
            }
            return usuarisPerLesOfertes;
        }
        public static void PosaOfertes(UsuariPerLaOferta[] usuarisPerLaOferta,LlistaOrdenada<string,OfertaActiva> ofertes)
        {
            for (int i = 0; i < usuarisPerLaOferta.Length; i++)
                usuarisPerLaOferta[i].oferta = ofertes[usuarisPerLaOferta[i].OfertaId];
        }

    }
}
