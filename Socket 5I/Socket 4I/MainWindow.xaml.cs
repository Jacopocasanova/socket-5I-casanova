using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Socket_4I
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket socket = null;// dichiara una varibile di tipo socket chiamata socket e la inizializza a null
        DispatcherTimer dTimer = null;// dichiara una variabile di tipo dispatcher time chiamata dTimer e la inizializza a null
        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);// crea una nuova istanza della classe Socket e la assegna alla variabile socket che verrà utilizzata per ricevere e inviare datagrammi UDP

            IPAddress local_address = IPAddress.Any;//dichiara una variabile di tipo IPAddress chiamata local_address e la inizializza a IPAddress.Any che vuol dire che potrà ricevere datagrammi da tutti gli indirizzi ip della rete 
            IPEndPoint local_endpoint = new IPEndPoint(local_address, 11000);//crea un nuovo oggetto IPEndPoint e lo assegna alla variabile local_endpoint. IpEndPoint Sarà il punto finale della comunicazione ed è costituito da un ip local address e dalla porta 11000

            socket.Bind(local_endpoint);// associa il socket creato in precedenza all'endpoint  consentendo al socket di ricevere pacchetti sulla porta e sull'indirizzo IP specificati.

            socket.Blocking = false; //imposta il socket in modalità non bloccante
            socket.EnableBroadcast = true;//abilita la trasmissione broadcast per il socket creato in precedenza impostando la proprietà EnableBroadcast su true

            dTimer = new DispatcherTimer();//crea un nuovo oggetto DispatcherTimer e lo assegna alla variabile dTimer

            dTimer.Tick += new EventHandler(aggiornamento_dTimer);//associa il gestore di eventi aggiornamento_dTimer all'evento Tick dell'oggetto DispatcherTimer rappresentato dalla variabile dTimer
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);//imposta l'intervallo di tempo tra i singoli eventi Tick dell'oggetto DispatcherTimer e in questo caso l'intervallo viene impostato su 250 millisecondi
            dTimer.Start();//avvia il timer rappresentato dall'oggetto DispatcherTimer

        }

        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int nBytes = 0;//efinisce un gestore di eventi aggiornamento_dTimer che verrà chiamato ogni volta che l'oggetto DispatcherTimer associato alla variabile dTimer genererà un evento Tick

            if ((nBytes = socket.Available) > 0)//permette di controllare in modo asincrono se ci sono dati pronti per essere letti sulla socket rappresentata dalla variabile socket
            {
                //ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];//rea un nuovo array di byte di dimensione pari al numero di byte disponibili per la lettura sulla socket

                EndPoint remoreEndPoint = new IPEndPoint(IPAddress.Any, 0);//ichiara un nuovo oggetto EndPoint chiamato remoteEndPoint che rappresenta l'endpoint remoto della connessione sulla socket

                nBytes = socket.ReceiveFrom(buffer, ref remoreEndPoint);//riceve i dati dalla socket in modo bloccante e li archivia nell'array buffer

                string from = ((IPEndPoint)remoreEndPoint).Address.ToString();//estrae l'indirizzo IP dell'endpoint remoto rappresentato dall'oggetto remoteEndPoint e lo converte in una stringa

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);//converte l'array di byte buffer in una stringa utilizzando il set di caratteri UTF-8


                lstMessaggi.Items.Add(from+": "+messaggio);//aggiunge un nuovo elemento alla ListBox lstMessaggi

            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            IPAddress remote_address = IPAddress.Parse(txtTo.Text);//converte la stringa txtTo.Text in un oggetto IPAddress e archivia quest'ultimo nella variabile remote_address

            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 10000);// crea un nuovo oggetto IPEndPoint, che rappresenta l'endpoint remoto a cui inviare i dati, utilizzando l'indirizzo IP del destinatario del messaggio e una porta di destinazione cioè la 10000

            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);//converte il testo inserito nella casella di testo txtMessaggio in un array di byte utilizzando la codifica UTF-8

            socket.SendTo(messaggio, remote_endpoint);//nvia il messaggio rappresentato dall'array di byte messaggio all'endpoint remoto specificato dall'oggetto remote_endpoint
        }
    }
}
