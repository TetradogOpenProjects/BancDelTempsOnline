using Gabriel.Cat;
using Gabriel.Cat.Extension;
using System;
using System.IO;

namespace BancDelTempsOnline
{
    public class Fitxer : ObjecteSqlIdAuto, IComparable,IComparable<Fitxer>,IClauUnicaPerObjecte
    {
        enum CampsFitxer
        {
            Id,Nom,Format,Dades,OnSutilitza
        }

        public const string TAULA = "Fitxers";
        public const string CAMPPRIMARYKEY = "Id";
        public const int MAXLONGITUDDADES = 10 * 1024 * 1024;//10MB
        public const int MAXLONGITUDNOM = 30;
        public const int MAXLONGITUDFORMAT = 5;
        public const int MAXLONGITUDIDONSUTILITZARA = 15;

        string idLocal;
        ArchuiBD archiu;
        string idOnSutilitza;
        

        public Fitxer(string nom, string extencio, byte[] dades) : this(nom, extencio, dades, null) { }
        public Fitxer(string nom, string extencio, byte[] dades,string idOnSutilitza) : this("", nom, extencio, dades,idOnSutilitza) { }
        private Fitxer(string id,string nom,string extencio,byte[] dades,string idOnSutilitza):base(TAULA,id,CAMPPRIMARYKEY)
        {

            AltaCanvi(CampsFitxer.Nom.ToString());
            AltaCanvi(CampsFitxer.Format.ToString());
            AltaCanvi(CampsFitxer.Dades.ToString());
            AltaCanvi(CampsFitxer.OnSutilitza.ToString());

            archiu = new ArchuiBD(nom + '.' + extencio, dades);
            idLocal = MiRandom.Next() + "" + DateTime.Now.Ticks;
            this.idOnSutilitza = idOnSutilitza;
        }
        public byte[] Dades
        {
            get { return archiu.Dades; }
            set
            {
                if (value == null) value = new byte[] { };
                else if (value.Length > MAXLONGITUDDADES)
                    throw new ArgumentException("no es pot dessar aquest volum de dades tan gran!!");
                archiu.Dades = value;
                base.CanviString(CampsFitxer.Dades.ToString(),archiu.Dades.ToHex());
            }
        }
        public string NomAmbFormat
        {
            get { return archiu.NomArchiuAmbExtensió; }
            set
            {
                if (value == null || !value.Contains('.')||Path.GetExtension(value).Length>MAXLONGITUDFORMAT||Path.GetFileNameWithoutExtension(value).Length>MAXLONGITUDNOM)
                    throw new ArgumentException("Nom incorrecte!!");
                archiu.NomArchiuAmbExtensió = value;
                base.CanviString(CampsFitxer.Nom.ToString(), Path.GetFileNameWithoutExtension(value));
                base.CanviString(CampsFitxer.Format.ToString(), Path.GetExtension(value));
            }
        }

        public ArchuiBD Archiu
        {
            get
            {
                return archiu;
            }
        }


        public string IdOnSutilitza
        {
            get
            {
                return idOnSutilitza;
            }

            set
            {
                if (value != null && value.Length > MAXLONGITUDIDONSUTILITZARA)
                    throw new ArgumentException();
                idOnSutilitza =value;
                CanviString(CampsFitxer.OnSutilitza.ToString(), idOnSutilitza);
            }
        }

        public IComparable Clau()
        {
            return idLocal;
        }

        public int CompareTo(Fitxer other)
        {
            int compareTo;
            if (other != null)
                compareTo = Clau().CompareTo(other.Clau());
            else compareTo = -1;
            return compareTo;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Fitxer);
        }

        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {
            string sentencia = "insert into " + TAULA + "(" + CampsFitxer.Nom.ToString() + "," + CampsFitxer.Format.ToString() + "," + CampsFitxer.Dades.ToString() + "," + CampsFitxer.OnSutilitza.ToString() + ") values(";
            sentencia += "'" + Path.GetFileNameWithoutExtension(NomAmbFormat) + "',";
            sentencia += "'" + Path.GetExtension(NomAmbFormat) + "',";
            sentencia += "'" +Dades.ToHex() + "',";
            sentencia += "" + (idOnSutilitza!=null?"'"+idOnSutilitza+"'":"null") + ");";
            return sentencia;
        }
        public static string StringCreateTable()
        {
            const int BYTEHEX = 2;//cada byte en hex son 2 caracters
            string sentencia = "create table " + TAULA + "(";
            sentencia += CampsFitxer.Id.ToString() + " int AUTO_INCREMENT primarykey,";
            sentencia += CampsFitxer.Nom.ToString() + " varchar(" + MAXLONGITUDNOM + ") not null,";
            sentencia += CampsFitxer.Format.ToString() + " varchar(" + MAXLONGITUDFORMAT + ") not null,";
            sentencia += CampsFitxer.Dades.ToString() + " varchar(" + MAXLONGITUDDADES*BYTEHEX + ") not null);";
            sentencia += CampsFitxer.OnSutilitza.ToString() + " varchar("+ MAXLONGITUDIDONSUTILITZARA + "));";
            return sentencia;
        }

        public static Fitxer[] TaulaToFitxers(string[,] taulaFitxers)
        {
            Fitxer[] fitxers = new Fitxer[taulaFitxers.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < fitxers.Length; i++)
                fitxers[i] = new Fitxer(taulaFitxers[(int)CampsFitxer.Nom, i], taulaFitxers[(int)CampsFitxer.Format, i], taulaFitxers[(int)CampsFitxer.Dades, i].HexStringToByteArray(), null) { IdOnSutilitza = taulaFitxers[(int)CampsFitxer.OnSutilitza, i] };
            return fitxers;
        }
    }
}