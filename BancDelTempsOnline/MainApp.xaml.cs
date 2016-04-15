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
		const double TEMPSPERRENOVARIP=3*60*60*1000;
		const int INTENTSCLIENTPERSERPERILLOS=1000;
		const string URLBANCDELTEMPS="http://localhost";
		ServidorHttpSeguro servidor;
		string paginaHome;
		static Window1()
		{
			GooglePlusUser.LoadJsonCredentials("client_secret_bncT.json",(int)RedirectUri.HttpLocalhost);
		}
		public Window1()
		{
			InitializeComponent();
			servidor=new ServidorHttpSeguro(TEMPSPERRENOVARIP,INTENTSCLIENTPERSERPERILLOS,URLBANCDELTEMPS);
			servidor.ClienteSeguro+=PeticionWeb;
			servidor.ClienteNoSeguro+=BanPorAbuso;
			Log.Listen+=PossaMissatgeLog;
			paginaHome=GooglePlusUser.HtmlBasicLogin();//possarlo bonic
			servidor.Start();
			
		}

		void PossaMissatgeLog(string message, int idMessage)
		{
			ObjViewer objMessage=message.ToObjViewer(()=>{});
			
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
			//si no ha fet login
			if(cliente.Tag==null){
				//envio la pagina de home
				cliente.Client.Response.Send(paginaHome);
			}
			else
			{
		     	//atenc a la petició
			}
		}
		void BanPorAbuso(ClienteServidorHttpSeguro cliente)
		{
			if(cliente.Tag==null)//si no ha fet login
			  Log.Send(String.Format("La ip {0} s'ha passat de peticions",cliente.DireccionIP),(int)MensajesLog.Importante);
		}
	}
}