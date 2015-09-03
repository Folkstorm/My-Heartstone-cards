using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My_Heartstone_cards
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            loadCardSet("Basic");
            loadCardSet("Classic");
            loadCardSet("Naxxramas");
            loadCardSet("Goblins vs Gnomes");
            loadCardSet("Reward");
            loadCardSet("Blackrock Mountain");
            loadCardSet("The Grand Tournament");
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(card.cardList[100].Img);
            bitmap.EndInit();
            imageCard.Source = bitmap;
         }

        private void loadCardSet(string set)
        {
            string response = APIRequest("https://omgvamp-hearthstone-v1.p.mashape.com/cards/sets/" + set + "?collectible=1&locale=frFR");
            object ok = Newtonsoft.Json.JsonConvert.DeserializeObject(response, typeof(List<card>));
            card.cardList.AddRange((List<card>)ok);
        }

        private static string APIRequest(string request)
        {
            System.Net.WebClient http = new System.Net.WebClient();
            http.Headers.Add("X-Mashape-Key: LigI9kHL7omsh5NN0rCWg0d6PA5jp1TdGUUjsnexk2zQ42JwCA");
            var response = http.DownloadString(request);
            http.Dispose();
            return response;
        }

    }
}
