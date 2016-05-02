/*
 * Creado por SharpDevelop.
 * Usuario: Pingu
 * Fecha: 15/04/2016
 * Hora: 16:31
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Gabriel.Cat.Wpf;
using Gabriel.Cat.Xarxa;
using Gabriel.Cat.Google;
using Gabriel.Cat;
using Gabriel.Cat.Extension;
using Microsoft.SqlServer.Server;
namespace BancDelTempsOnline
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		//para no tener numeros que no se entienden :D
		enum RedirectUri
		{
			PaginaWeb,
			HttpLocalhost,
			HttpsLocalhost
		}
		enum MissatgesLog
		{
			Normal,
			Error,
			Important
		}
		const double TEMPSPERRENOVARIP = 3 * 60 * 60 * 1000;//3 hores
		const int INTENTSCLIENTPERSERPERILLOS = 1000;
		static readonly string urlPaginaFitxers;
		ServidorHttpSeguro servidor;
		string paginaLogin;
		string paginaRegistre;
		LlistaOrdenada<string,Usuari> usuaris;
		LlistaOrdenada<string,byte[]> fitxers;
		static Window1()
		{
			GooglePlusUser.LoadJsonCredentials("client_secret_bncT.json", (int)RedirectUri.HttpLocalhost);
			urlPaginaFitxers = System.IO.Path.Combine(GooglePlusUser.RedirectUri, "fitxers/");
		}
		public Window1()
		{
			InitializeComponent();
			fitxers=new LlistaOrdenada<string, byte[]>();//els tinc que obtenir de la BD y gestionarlos
			usuaris = new LlistaOrdenada<string, Usuari>();
			servidor = new ServidorHttpSeguro(TEMPSPERRENOVARIP, INTENTSCLIENTPERSERPERILLOS, GooglePlusUser.RedirectUri, urlPaginaFitxers); 
			servidor.ClienteSeguro += PeticionWeb;
			servidor.ClienteNoSeguro += BanPerAbus;
			Log.Listen += PossaMissatgeLog;
			paginaLogin = GooglePlusUser.HtmlBasicLogin();//possarlo bonic
			servidor.Start();
			
		}

		void PossaMissatgeLog(string message, int idMessage)
		{
			ObjViewer objMessage = message.ToObjViewer((obj) => {
			});
			
			switch ((MissatgesLog)idMessage) {
				case MissatgesLog.Normal:
					objMessage.CambiarColorLetra(Colors.Gray);
					break;
				case MissatgesLog.Error:
					objMessage.CambiarColorLetra(Colors.Red);
					break;
				case MissatgesLog.Important:
					objMessage.CambiarColorLetra(Colors.Green);
					break;
			}
			listLog.Children.Add(objMessage);
		}
		void PeticionWeb(ClienteServidorHttpSeguro cliente)
		{
			
			Usuari usuari;
			string elementHaEntregar = "";
			if (!cliente.Client.Request.Url.ToString().Contains(urlPaginaFitxers)) {//si no demana un fitxer demana una pagina :)
				//si no ha fet login
				if (cliente.Tag == null) {
				
					elementHaEntregar = ClientSenseLogin(cliente);
				
				} else {
					elementHaEntregar = ClientAmbLogin(cliente);
				}
				if(!cliente.Bloqueado)
				  cliente.Client.Response.Send(elementHaEntregar);
			} else {
				//es un fitxer
				//si el pot agafar se li dona
				elementHaEntregar=UrlElementDemanat(cliente);
				if(!cliente.Bloqueado)
					cliente.Client.Response.Send(fitxers[elementHaEntregar]);
			}
		}

		string UrlElementDemanat(ClienteServidorHttpSeguro cliente)
		{
			//si demana algo que no pot es bloqueja i es notifica :)
			string url="";//hagafo la url de la peticio :)
			GooglePlusUser gUsuari;
			Usuari usuariRegistrat;
			//si no pot el bloquejo,notifico i poso ban
			if(!fitxers.Existeix(url))//posar la validació si pot o no l'usuari obtenir la url si no tingues dret a accedir llavors li cau un ban!
			{
				cliente.Bloqueado=true;
				if(cliente.Tag==null){
				Log.Send(String.Format("Hi ha una petició de la IP: {0} ha demanat un fitxer al que no pot accedir",cliente.DireccionIP), (int)MissatgesLog.Important);
				}else if(cliente.Tag is GooglePlusUser)
				{
					gUsuari=cliente.Tag as GooglePlusUser;
					Log.Send(String.Format("Hi ha una petició d'un usuari de google {1} amb IP: {0} ha demanat un fitxer al que no pot accedir",cliente.DireccionIP,gUsuari.Email), (int)MissatgesLog.Important);
	                //poso el correo en una llista d'usuaris google bloquejats per fer el registre
				
				}else{
				//es un usuari registrat
					usuariRegistrat=cliente.Tag as Usuari;
					Log.Send(String.Format("L'usuari {1} amb Email: {0} ha demanat un fitxer al que no pot accedir",usuariRegistrat.Nom,usuariRegistrat.Email), (int)MissatgesLog.Important);
	                //poso el correo en una llista d'usuaris google bloquejats per fer el registre

				}
			}
			return url;
		}
		string ClientAmbLogin(ClienteServidorHttpSeguro cliente)
		{
			Usuari usuari;
			string paginaHaEntregar="";
			usuari = cliente.Tag as Usuari;
			//atenc a la petició
			if (usuari != null) {
				try {
					//es un usuari ja creat que fa una petició
					paginaHaEntregar = PaginaPeticioUsuari(usuari, cliente.Client.Request);
				} catch {
					//l'usuari ha demanat una opció que no pot demanar (l'ha possat ell...)
					cliente.Bloqueado = true;
					Log.Send(String.Format("L'usuari {0} amb email {1} ha demanat una opció que no pot...vol accedir sense permis.", usuari.Nom, usuari.Email), (int)MissatgesLog.Important);
					//li poso el ban per consolidar el bloqueig i que consti aixi els admin ho saben			
				}
			} else {
				//es un usuari nou que ha fet login i a acabat d'emplenar el formulari
				//mirar si un cop rebut el formulari es pot fer alguna trampa si es aixi protegirho
				usuari = CreaUsuari(cliente.Client.Request);
				cliente.Tag = usuari;
				usuaris.Afegir(usuari.Email, usuari);
				//ja ha finalitzat el registre ara li envio la seva pagina home
				paginaHaEntregar = PaginaHomeUsuari(usuari);
			}
			return paginaHaEntregar;
		}
		string ClientSenseLogin(ClienteServidorHttpSeguro cliente)
		{
			string paginaHaEntregar;
			GooglePlusUser googleUser;
			if (cliente.Client.Request.QueryString.HasKeys()) {
				//li ha donat al botó per fer login :D
				googleUser = GooglePlusUser.GetProfile(cliente);
				if (usuaris.Existeix(googleUser.Email)) {
					//ha carregat un usuari existent
					cliente.Tag = usuaris[googleUser.Email];
					//pagina home usuari
					paginaHaEntregar = PaginaHomeUsuari(usuaris[googleUser.Email]);
				} else {
					//pagina registre
					paginaHaEntregar = PaginaRegistre(googleUser);
					cliente.Tag = googleUser;
				}
				//redirigir url perque conté el codi de login 
				//per fer ho
			} else {
				//envio la pagina de login, la principal
				paginaHaEntregar = paginaLogin;
			}
			return paginaHaEntregar;
		}

		void BanPerAbus(ClienteServidorHttpSeguro cliente)
		{
			if (cliente.Tag == null) {//si no ha fet login
				Log.Send(String.Format("La ip {0} s'ha passat de peticions", cliente.DireccionIP), (int)MissatgesLog.Important);
				cliente.Bloqueado = true;
			}
		}

		Usuari CreaUsuari(System.Net.HttpListenerRequest request)
		{
			//trec els camps post i els poso al usuari
			LlistaOrdenada<string,string> postDataDic = request.PostDataDiccionary(); 
			string nom = postDataDic["Nom"];
			string imatgePerfil = postDataDic["ImatgePerfil"];
			string municipi = postDataDic["Municipi"];
			string nie = postDataDic["NIE"];
			string telefon = postDataDic["Telefon"];
			string email = postDataDic["Email"];
			return new Usuari(nom,new Fitxer(nom+"imgPerfil",".jpg",Serializar.GetBytes(imatgePerfil)), municipi, nie, telefon, email);
		}
		string PaginaPeticioUsuari(Usuari usuari, System.Net.HttpListenerRequest request)
		{
			LlistaOrdenada<string,string> postDataDic = request.PostDataDiccionary(); 
			string paginaPeticio =  PaginaBaseUsuari(usuari);
			//es fa la petició i es crea la pagina
			return paginaPeticio;
		}
		string PaginaHomeUsuari(Usuari usuari)
		{
			string paginaHomeUsuari = PaginaBaseUsuari(usuari);
			//es crea la pagina home de l'usuari que sigui un camp calculat un cop i si es donden o es posen banns doncs es torna a calcular
			return paginaHomeUsuari;
		}
		string PaginaBaseUsuari(Usuari usuari)
		{
			string paginaBase="";
			//poso totes les opcions del menu i la resta de la pagina
			   //poso etiquetes per reemplaçar-les pel contingut que toqui :)
			   //la desso a l'usuari??per no tornala a generar ja que sempre sera la mateixa fins que li donin o treguin permisos o bans nous
			return paginaBase;
		}
		string PaginaRegistre(GooglePlusUser googleUser)
		{
			string paginaRegistreUsuari = paginaRegistre;
			paginaRegistre = paginaRegistre.Replace("#NOM#", googleUser.Nombre);
			paginaRegistre = paginaRegistre.Replace("#COGNOMS#", googleUser.Apellidos);
			paginaRegistre = paginaRegistre.Replace("#EMAIL#", googleUser.Nombre);
			paginaRegistre = paginaRegistre.Replace("#URIIMG#", googleUser.ImagenPerfilUri);
			return paginaRegistre;
		}
	}
}