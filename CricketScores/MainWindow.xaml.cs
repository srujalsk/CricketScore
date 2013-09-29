using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CricketScores
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public static ObservableCollection<String> Scores = new ObservableCollection<String>();
        public static ObservableCollection<String> tmp_list = new ObservableCollection<String>();
        public BackgroundWorker bw;

        public static ObservableCollection<String> Items
        {
            get { return Scores; }
        }

        public MainWindow()
        {
            InitializeComponent();
            List1.ItemsSource = Scores;
            //Items.CollectionChanged += Items_CollectionChanged;
            //Scores.Add("a");
            //Scores.Add("a");
            //Scores.Add("a");
            //List1.SourceUpdated += List1_SourceUpdated;
            //List1.TargetUpdated += List1_TargetUpdated;
            
            //loop_results();
        }

       



        public XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.ContentType = "application/xml; charset=utf-8";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    
                    //var objResponse = (HttpWebResponse) response.GetResponseStream();
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(response.GetResponseStream());
                    return xdoc;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public ObservableCollection<String> get_list(XmlDocument result)
        {
            ObservableCollection<String> ll = new ObservableCollection<String>();
            XPathNavigator nav = result.CreateNavigator();
            XPathExpression expr;
            expr = nav.Compile("/rss/channel/item/title");
            XPathNodeIterator iterator = nav.Select(expr);
            try
            {
                while (iterator.MoveNext())
                {
                    Console.WriteLine(iterator.Current.Value);
                    ll.Add(iterator.Current.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ll;
        }

        public void loop_results()
        {
            //List<String> ll;
            /*for (; ; )
            {
                XmlDocument xdoc = MakeRequest("http://static.cricinfo.com/rss/livescores.xml");
                get_list(xdoc);
            }*/
        }

        private void List1_Loaded(object sender, RoutedEventArgs e)
        {
            //Thread t1 = new Thread(new ThreadStart(loop_results));
            //t1.Start();
            XmlDocument xdoc = MakeRequest("http://static.cricinfo.com/rss/livescores.xml");
            Scores = get_list(xdoc);
            List1.ItemsSource = Scores;
            List1.Items.Refresh();
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Scores = tmp_list;
            List1.ItemsSource = Scores;
            List1.Items.Refresh();
            bw.RunWorkerAsync();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlDocument xdoc = MakeRequest("http://static.cricinfo.com/rss/livescores.xml");
            tmp_list = get_list(xdoc);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            XmlDocument xdoc = MakeRequest("http://static.cricinfo.com/rss/livescores.xml");
            Scores = get_list(xdoc);
            List1.ItemsSource = Scores;
            List1.Items.Refresh();
        }
    }
}