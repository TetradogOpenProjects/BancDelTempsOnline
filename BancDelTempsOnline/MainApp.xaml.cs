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
		enum MensajesLog
		{
			Normal,
			Error,
			Importante
		}
		const double TEMPSPERRENOVARIP = 3 * 60 * 60 * 1000;
		const int INTENTSCLIENTPERSERPERILLOS = 1000;
		const string URLBANCDELTEMPS = "http://localhost";
		ServidorHttpSeguro servidor;
		string paginaLogin;
		string paginaRegistre;
		LlistaOrdenada<string,Usuari> usuaris;
		static Window1()
		{
			GooglePlusUser.LoadJsonCredentials("client_secret_bncT.json", (int)RedirectUri.HttpLocalhost);
		}
		public Window1()
		{
			InitializeComponent();
			usuaris = new LlistaOrdenada<string, Usuari>();
			servidor = new ServidorHttpSeguro(TEMPSPERRENOVARIP, INTENTSCLIENTPERSERPERILLOS, URLBANCDELTEMPS);
			servidor.ClienteSeguro += PeticionWeb;
			servidor.ClienteNoSeguro += BanPorAbuso;
			Log.Listen += PossaMissatgeLog;
			paginaLogin = GooglePlusUser.HtmlBasicLogin();//possarlo bonic
			servidor.Start();
			
		}

		void PossaMissatgeLog(string message, int idMessage)
		{
			ObjViewer objMessage = message.ToObjViewer((obj) => {
			});
			
			switch ((MensajesLog)idMessage) {
				case MensajesLog.Normal:
					objMessage.CambiarColorLetra(Colors.Gray);
					break;
				case MensajesLog.Error:
					objMessage.CambiarColorLetra(Colors.Red);
					break;
				case MensajesLog.Importante:
					objMessage.CambiarColorLetra(Colors.Green);
					break;
			}
			listLog.Children.Add(objMessage);
		}
		void PeticionWeb(ClienteServidorHttpSeguro cliente)
		{
			GooglePlusUser googleUser;
			Usuari usuari;
			string paginaHaEntregar = "";
			//si no ha fet login
			if (cliente.Tag == null) {
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
			} else {
				//atenc a la petició
				if (cliente.Tag is Usuari) {
					//es un usuari ja creat que fa una petició
					paginaHaEntregar = PaginaPeticioUsuari(cliente.Tag as Usuari, cliente.Client.Request);
				} else {
					//es un usuari nou que ha fet login i a acabat d'emplenar el formulari
					//mirar si un cop rebut el formulari es pot fer alguna trampa si es aixi protegirho
					usuari = CreaUsuari(cliente.Client.Request);
					cliente.Tag = usuari;
					usuaris.Afegir(usuari.Email, usuari);
					//ja ha finalitzat el registre ara li envio la seva pagina home
					paginaHaEntregar = PaginaHomeUsuari(usuari);
				}
			}
			cliente.Client.Response.Send(paginaHaEntregar);
		}
		void BanPorAbuso(ClienteServidorHttpSeguro cliente)
		{
			if (cliente.Tag == null)//si no ha fet login
			  Log.Send(String.Format("La ip {0} s'ha passat de peticions", cliente.DireccionIP), (int)MensajesLog.Importante);
		}

		Usuari CreaUsuari(System.Net.HttpListenerRequest request)
		{
			//trec els camps post i els poso al usuari
			LlistaOrdenada<string,string> postDataDic=request.PostDataDiccionary(); 
			string nom=postDataDic["Nom"];
			string uriImatgePerfil=postDataDic["UriImatgePerfil"];
			string municipi=postDataDic["Municipi"];
			string nie=postDataDic["NIE"];
			string telefon=postDataDic["Telefon"];
			string email=postDataDic["Email"];
			return new Usuari(nom,uriImatgePerfil,municipi,nie,telefon,email);
		}
		string PaginaPeticioUsuari(Usuari usuari, System.Net.HttpListenerRequest request)
		{
			LlistaOrdenada<string,string> postDataDic=request.PostDataDiccionary(); 
			string paginaPeticio="";
			//es fa la petició i es crea la pagina
			return paginaPeticio;
		}
		string PaginaHomeUsuari(Usuari usuari)
		{
			string paginaHomeUsuari="";
			//es crea la pagina home de l'usuari que sigui un camp calculat un cop i si es donden o es posen banns doncs es torna a calcular
			return paginaHomeUsuari;
		}
		string PaginaRegistre(GooglePlusUser googleUser)
		{
			string paginaRegistreUsuari=paginaRegistre;
			paginaRegistre=paginaRegistre.Replace("#NOM#",googleUser.Nombre);
			paginaRegistre=paginaRegistre.Replace("#COGNOMS#",googleUser.Apellidos);
			paginaRegistre=paginaRegistre.Replace("#EMAIL#",googleUser.Nombre);
			paginaRegistre=paginaRegistre.Replace("#URIIMG#",googleUser.ImagenPerfilUri);
			return paginaRegistre;
		}
	}
}