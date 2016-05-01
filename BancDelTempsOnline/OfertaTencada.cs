using Gabriel.Cat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancDelTempsOnline
{
    /// <summary>
    /// Son les dades que quedaran de la oferta un cop finalitzada i comprobada
    /// </summary>
    public class OfertaTencada : ObjecteSqlIdAuto,IClauUnicaPerObjecte
    {
        public enum Realitzament
        {
            Online,Offline,ForaBncT,Cancelat,
        }
        enum CampsOfertaTencada
        {
            Id,Demandant,Ofert,DadesPerTrobarAQuiOferta,Temps,Inici,Fi,QuiValidaLaOferta
        }
        public const int MAXLONGITUDDADESPERTROBAROFERTANT = 50;
        public const string TAULA="OfertesTencades";
        public const string CAMPPRIMARYKEY="Id";
        Usuari demandant;
        Usuari ofert;
        string dadesPerTrobarAQuiOferta;//nom, numero de soci...
        int minuts;//aixi admet fraccions d'hora
        DateTime inici;
        DateTime fi;
        Usuari quiValidaLaOferta;
        string idUnicLocal;
       

        //si no s'ha sap qui s'ha ofert
        public OfertaTencada(Usuari demandant, string dadesPerTrobarAQuiOferta, string titol, string descripcio, int minuts, DateTime inici, DateTime fi) : this("", demandant, null, dadesPerTrobarAQuiOferta, titol, descripcio, minuts, inici, fi, null) { }
        //Si s'ha sap qui ha ofert
        public OfertaTencada(Usuari demandant, Usuari ofert, string titol, string descripcio, int minuts, DateTime inici, DateTime fi):this("",demandant, ofert, "",titol, descripcio, minuts, inici, fi,null) { }
        //per carregar les dades
        private OfertaTencada(string id,Usuari demandant,Usuari ofert,string dadesPerTrobarAQuiOferta,string titol,string descripcio,int minuts,DateTime inici,DateTime fi,Usuari quiValidaLaOferta) : base(TAULA, id, CAMPPRIMARYKEY)
        {
            if (demandant == null)
                throw new NullReferenceException();
            else if (String.IsNullOrEmpty(titol) || String.IsNullOrEmpty(descripcio) || minuts < 0||inici>fi)
                throw new ArgumentException();
            AltaCanvi(CampsOfertaTencada.Demandant.ToString());
            AltaCanvi(CampsOfertaTencada.Ofert.ToString());
            AltaCanvi(CampsOfertaTencada.DadesPerTrobarAQuiOferta.ToString());
            AltaCanvi(CampsOfertaTencada.Temps.ToString());
            AltaCanvi(CampsOfertaTencada.Inici.ToString());
            AltaCanvi(CampsOfertaTencada.Fi.ToString());
            AltaCanvi(CampsOfertaTencada.QuiValidaLaOferta.ToString());

            this.demandant = demandant;
            this.ofert = ofert;
            this.dadesPerTrobarAQuiOferta = dadesPerTrobarAQuiOferta;
            this.minuts = minuts;
            this.inici = inici;
            this.fi = fi;
            this.quiValidaLaOferta = quiValidaLaOferta;
            idUnicLocal = MiRandom.Next()+"" + DateTime.Now.Ticks;
        }
        #region Propietats
        public Usuari Demandant
        {
            get
            {
                return demandant;
            }

            set
            {
                if (value == null)
                    throw new NullReferenceException();
                demandant = value;
                CanviString(CampsOfertaTencada.Demandant.ToString(), demandant.PrimaryKey);
            }
        }

        public Usuari Ofert
        {
            get
            {
                return ofert;
            }

            set
            {
                ofert = value;
                if(ofert!=null)
                    CanviString(CampsOfertaTencada.Demandant.ToString(), ofert.PrimaryKey);
                else
                    CanviString(CampsOfertaTencada.Demandant.ToString(), null);
            }
        }

        public string DadesPerTrobarAQuiOferta
        {
            get
            {
                return dadesPerTrobarAQuiOferta;
            }

            set
            {
                if (value == null) value = "";
                else if (value.Length > MAXLONGITUDDADESPERTROBAROFERTANT)
                    throw new ArgumentException("Es pasa del limit "+MAXLONGITUDDADESPERTROBAROFERTANT);
                dadesPerTrobarAQuiOferta = value;
                CanviString(CampsOfertaTencada.DadesPerTrobarAQuiOferta.ToString(), dadesPerTrobarAQuiOferta);
            }
        }

        public int Minuts
        {
            get
            {
                return minuts;
            }

            set
            {
                if (value < 0) value = 0;
                minuts = value;
                CanviNumero(CampsOfertaTencada.Temps.ToString(), minuts + "");
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
                CanviData(CampsOfertaTencada.Inici.ToString(), inici);
            }
        }

        public DateTime Fi
        {
            get
            {
                return fi;
            }

            set
            {
                fi = value;
                CanviData(CampsOfertaTencada.Fi.ToString(), fi);
            }
        }

        public Usuari QuiValidaLaOferta
        {
            get
            {
                return quiValidaLaOferta;
            }

            set
            {
                quiValidaLaOferta = value;
                if (quiValidaLaOferta != null)
                    CanviString(CampsOfertaTencada.QuiValidaLaOferta.ToString(), quiValidaLaOferta.PrimaryKey);
                else CanviString(CampsOfertaTencada.QuiValidaLaOferta.ToString(), null);
            }
        }
        #endregion
        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {
          string sentencia="insert into "+TAULA+"("+CampsOfertaTencada.Demandant.ToString()+","+ CampsOfertaTencada.Ofert.ToString() + "," + CampsOfertaTencada.DadesPerTrobarAQuiOferta.ToString() + "," + CampsOfertaTencada.Temps.ToString() + "," + CampsOfertaTencada.Inici.ToString() + "," + CampsOfertaTencada.Fi.ToString() + "," + CampsOfertaTencada.QuiValidaLaOferta.ToString() +") values(";
            sentencia += "'" + Demandant.PrimaryKey + "',";
            if (Ofert != null)
                sentencia += "'" + Ofert.PrimaryKey + "',";
            else sentencia += "null,";
            sentencia += "'" + DadesPerTrobarAQuiOferta + "',";
            sentencia += Minuts + ",";
            sentencia += ObjecteSql.DateTimeToStringSQL(tipusBD, inici)+",";
            sentencia += ObjecteSql.DateTimeToStringSQL(tipusBD, fi) + ",";
            if (QuiValidaLaOferta != null)
                sentencia += "'" + QuiValidaLaOferta.PrimaryKey + "'";
            else sentencia += "null";
            sentencia += ");";
            return sentencia;
        }
        public IComparable Clau()
        {
            return idUnicLocal;
        }
        public static string StringCreateTable()
        {
            string sentencia = "Create table " + TAULA + " (";
            sentencia += CampsOfertaTencada.Id.ToString() + " int NOT NULL AUTO_INCREMENT,";
            sentencia += CampsOfertaTencada.Demandant.ToString() + " varchar(10) not null references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsOfertaTencada.Ofert.ToString() + " varchar(10)  references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsOfertaTencada.DadesPerTrobarAQuiOferta.ToString() + " varchar(" + MAXLONGITUDDADESPERTROBAROFERTANT + "),";
            sentencia += CampsOfertaTencada.Temps.ToString() + " int,";
            sentencia += CampsOfertaTencada.Inici.ToString() + " date,";
            sentencia += CampsOfertaTencada.Fi.ToString() + " date,";
            sentencia += CampsOfertaTencada.QuiValidaLaOferta.ToString() + " varchar(10)  references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "));";
            return sentencia;
        }

  
    }
}
