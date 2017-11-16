using Gabriel.Cat;
using Gabriel.Cat.Extension;
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
    public class OfertaTencada : ObjecteSqlIdAuto,IClauUnicaPerObjecte,IComparable
    {
        public enum Realitzament
        {
            Online,Offline,ForaBncT,Cancelat,
        }
        enum CampsOfertaTencada
        {
            Id,Demandant,Ofert,DadesPerTrobarAQuiOferta,Titol,Descripcio,Temps,Inici,Fi,QuiValidaLaOferta
            
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
        string titol;
        string descripcio;

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
            AltaCanvi(CampsOfertaTencada.Titol.ToString());
            AltaCanvi(CampsOfertaTencada.Descripcio.ToString());
            AltaCanvi(CampsOfertaTencada.Temps.ToString());
            AltaCanvi(CampsOfertaTencada.Inici.ToString());
            AltaCanvi(CampsOfertaTencada.Fi.ToString());
            AltaCanvi(CampsOfertaTencada.QuiValidaLaOferta.ToString());

            this.titol = titol;
            this.demandant = demandant;
            this.ofert = ofert;
            this.dadesPerTrobarAQuiOferta = dadesPerTrobarAQuiOferta;
            this.minuts = minuts;
            this.inici = inici;
            this.fi = fi;
            this.descripcio = descripcio;
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

        public string Titol
        {
            get
            {
                return titol;
            }

            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentException("Es necesita un titol");
                else if (value.Length > OfertaActiva.MAXLONGITUDTITOL) throw new ArgumentException("Es masa llarg el titol!");
                titol = value;
                CanviString(CampsOfertaTencada.Titol.ToString(), titol);
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
                if (value == null) throw new ArgumentNullException();
                else if (value.Length > OfertaActiva.MAXLONGITUDDESCRIPCIO)
                    throw new ArgumentException("Supera el maxim de longitut!!");
                descripcio = value;
                CanviString(CampsOfertaTencada.Descripcio.ToString(), descripcio);
            }
        }
        #endregion
        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {
          string sentencia="insert into "+TAULA+"("+CampsOfertaTencada.Demandant.ToString()+","+ CampsOfertaTencada.Ofert.ToString() + "," + CampsOfertaTencada.Titol.ToString() + "," + CampsOfertaTencada.Descripcio.ToString() + "," + CampsOfertaTencada.DadesPerTrobarAQuiOferta.ToString() + "," + CampsOfertaTencada.Temps.ToString() + "," + CampsOfertaTencada.Inici.ToString() + "," + CampsOfertaTencada.Fi.ToString() + "," + CampsOfertaTencada.QuiValidaLaOferta.ToString() +") values(";
            sentencia += "'" + Demandant.PrimaryKey + "',";
            if (Ofert != null)
                sentencia += "'" + Ofert.PrimaryKey + "',";
            else sentencia += "null,";
            sentencia += "'" + Titol + "',";
            sentencia += "'" + Descripcio + "',";
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
        public IComparable Clau
        {
        	get{
            return idUnicLocal;
        	}
        }
        public static string StringCreateTable()
        {
            string sentencia = "Create table " + TAULA + " (";
            sentencia += CampsOfertaTencada.Id.ToString() + " int  AUTO_INCREMENT primaryKey,";
            sentencia += CampsOfertaTencada.Demandant.ToString() + " varchar(10) not null references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsOfertaTencada.Ofert.ToString() + " varchar(10)  references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsOfertaTencada.DadesPerTrobarAQuiOferta.ToString() + " varchar(" + MAXLONGITUDDADESPERTROBAROFERTANT + "),";
            sentencia += CampsOfertaTencada.Titol.ToString() + " varchar(" + OfertaActiva.MAXLONGITUDTITOL + ") not null,";
            sentencia += CampsOfertaTencada.Descripcio.ToString() + " varchar(" + OfertaActiva.MAXLONGITUDDESCRIPCIO + ") not null,";
            sentencia += CampsOfertaTencada.Temps.ToString() + " int not null,";
            sentencia += CampsOfertaTencada.Inici.ToString() + " date not null,";
            sentencia += CampsOfertaTencada.Fi.ToString() + " date not null,";
            sentencia += CampsOfertaTencada.QuiValidaLaOferta.ToString() + " varchar(10)  references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "));";
            return sentencia;
        }

        public static OfertaTencada[] TaulaToOfertesTencades(string[,] taulaOfertesTencades, TwoKeysList<string, string, Usuari> usuaris)
        {
            OfertaTencada[] ofertesTencades = new OfertaTencada[taulaOfertesTencades.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < ofertesTencades.Length; i++)
                ofertesTencades[i] = new OfertaTencada(taulaOfertesTencades[(int)CampsOfertaTencada.Id, i], usuaris.ObtainValueWithKey2(taulaOfertesTencades[(int)CampsOfertaTencada.Demandant, i]), usuaris.ObtainValueWithKey2(taulaOfertesTencades[(int)CampsOfertaTencada.Ofert, i]), taulaOfertesTencades[(int)CampsOfertaTencada.DadesPerTrobarAQuiOferta, i], taulaOfertesTencades[(int)CampsOfertaTencada.Titol, i], taulaOfertesTencades[(int)CampsOfertaTencada.Descripcio, i], Convert.ToInt32(taulaOfertesTencades[(int)CampsOfertaTencada.Temps, i]), ObjecteSql.StringToDateTime(taulaOfertesTencades[(int)CampsOfertaTencada.Inici, i]), ObjecteSql.StringToDateTime(taulaOfertesTencades[(int)CampsOfertaTencada.Fi, i]), usuaris.ObtainValueWithKey2(taulaOfertesTencades[(int)CampsOfertaTencada.QuiValidaLaOferta, i]));
            return ofertesTencades;
        }
    }
}
