using Gabriel.Cat;
using Gabriel.Cat.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancDelTempsOnline
{
    public class Missatge : ObjecteSqlIdAuto, IClauUnicaPerObjecte
    {
        enum CampsMissatge
        {
            Id,Emisor,Receptor,Missatge,Data
        }

        public const string TAULA = "Missatges";
        public const string CAMPPRIMARYKEY = "Id";
        public const int LONGITUDMISSATGEMAX=300;//caracters per missatge

        string idUnic;
        Usuari usuariEmisor;
        Usuari usuariReceptor;
        string missatge;
        DateTime data;
       
        public Missatge(Usuari usuariEmisor, Usuari usuariReceptor, string missatge, DateTime data) : this("",usuariEmisor, usuariReceptor, missatge, data) { }
        private Missatge(string id,Usuari usuariEmisor, Usuari usuariReceptor, string missatge, DateTime data):base(TAULA,id,CAMPPRIMARYKEY)
        {
            base.AltaCanvi(CampsMissatge.Emisor.ToString());
            base.AltaCanvi(CampsMissatge.Receptor.ToString());
            base.AltaCanvi(CampsMissatge.Missatge.ToString());
            base.AltaCanvi(CampsMissatge.Data.ToString());
            this.usuariEmisor = usuariEmisor;
            this.usuariReceptor = usuariReceptor;
            this.missatge = missatge;
            this.data = data;
        }
        public Usuari UsuariEmisor
        {
            get
            {
                return usuariEmisor;
            }

            set
            {
                if (value == null) throw new NullReferenceException();
                usuariEmisor = value;
                CanviString(CampsMissatge.Emisor.ToString(), usuariEmisor.PrimaryKey);
            }
        }

        public Usuari UsuariReceptor
        {
            get
            {
                return usuariReceptor;
            }

            set
            {
                if (value == null) throw new NullReferenceException();
                usuariReceptor = value;
                CanviString(CampsMissatge.Receptor.ToString(), usuariReceptor.PrimaryKey);
            }
        }

        public string MissatgeString
        {
            get
            {
                return missatge;
            }

            set
            {
                if (value == null) value = "";
                else if (value.Length > LONGITUDMISSATGEMAX)
                    throw new ArgumentException("S'ha superat el maxim per missatge");
                missatge = value;
                CanviString(CampsMissatge.Missatge.ToString(), missatge);
            }
        }

        public DateTime Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
                CanviData(CampsMissatge.Data.ToString(),data);
            }
        }
        public IComparable Clau()
        {
            return idUnic;
        }

        public override string StringInsertSql(TipusBaseDeDades tipusBD)
        {
            string sentencia = "insert into " + TAULA + "("+ CampsMissatge.Emisor.ToString()+"," + CampsMissatge.Receptor.ToString() + "," + CampsMissatge.Missatge.ToString() + "," + CampsMissatge.Data.ToString()  + ") values(";
            sentencia += "'" + UsuariEmisor.PrimaryKey + "',";
            sentencia += "'" + usuariReceptor.PrimaryKey + "',";
            sentencia += "'" + MissatgeString + "',";
            sentencia += "'" + ObjecteSql.DateTimeToStringSQL(tipusBD,data) + "');";
            return sentencia;
        }
        public static string StringCreateTable()
        {
            string sentencia = "create table " + TAULA + " (";
            sentencia += CampsMissatge.Id.ToString() + " int int NOT NULL AUTO_INCREMENT,";
            sentencia += CampsMissatge.Emisor.ToString() + " varchar(10) not null references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsMissatge.Receptor.ToString() + " varchar(10) not null references " + Usuari.TAULA + "(" + Usuari.CAMPPRIMARYKEY + "),";
            sentencia += CampsMissatge.Missatge.ToString() + " varchar(" + LONGITUDMISSATGEMAX + ") not null,";
            sentencia += CampsMissatge.Data.ToString() + " date not null);";
            return sentencia;
        }

        public static Missatge[] TaulaToMissatges(string[,] taulaMissatges, LlistaOrdenada<string, Usuari> usuarisList)
        {
            Missatge[] missatges = new Missatge[taulaMissatges.GetLength(DimensionMatriz.Fila)];
            for(int i=0;i<missatges.Length;i++)
            {
                missatges[i] = new Missatge(taulaMissatges[(int)CampsMissatge.Id, i], usuarisList[taulaMissatges[(int)CampsMissatge.Emisor, i]], usuarisList[taulaMissatges[(int)CampsMissatge.Receptor, i]], taulaMissatges[(int)CampsMissatge.Missatge, i], ObjecteSql.StringToDateTime(taulaMissatges[(int)CampsMissatge.Data, i]));
            }
            return missatges;
        }
    }
}
