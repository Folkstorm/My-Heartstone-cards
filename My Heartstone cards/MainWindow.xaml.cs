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
        
        const string cardPath = "";
        const string myDecksPath = "MyDecks\\";

        public MainWindow()
        {
            InitializeComponent();
            //if (System.IO.Directory.Exists(cardPath) == false)
            //{
            //    System.IO.Directory.CreateDirectory(cardPath);
            //}
            if (System.IO.File.Exists(cardPath + "cardList") == false)
            {
                DownloadAllCardSet();
            }
            else
            {
                try
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter reader = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    System.IO.FileStream file = System.IO.File.OpenRead(cardPath +"cardList");
                    card.cardList = (List<card>)reader.Deserialize(file);
                    file.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Your card database seems to be corrupted, we will redownload it");
                    DownloadAllCardSet();
                }
                
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DownloadAllCardSet();
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri(card.cardList[100].Img);
            //bitmap.EndInit();
            //imageCard.Source = bitmap;
         }

        private void DownloadAllCardSet()
        {
            Loading.Visibility = Visibility.Visible;
            System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            

            DownloadCardSet("Basic");
            DownloadCardSet("Classic");
            DownloadCardSet("Naxxramas");
            DownloadCardSet("Goblins vs Gnomes");
            DownloadCardSet("Reward");
            DownloadCardSet("Blackrock Mountain");
            DownloadCardSet("The Grand Tournament");
            progressBar1.Maximum = card.cardList.Count;

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //System.Net.WebClient http = new System.Net.WebClient();
            //int i = 0;
            foreach (card item in card.cardList)
            {
            //    http.DownloadFile(item.Img, cardPath + item.CardId);
            //    http.DownloadFile(item.ImgGold, cardPath + item.CardId + "g");
                item.Img = cardPath + item.CardId;
                item.ImgGold = cardPath + item.CardId + "g";
                if (item.PlayerClass == null)
                {
                    item.PlayerClass = "Neutral";
                }
            //    i++;
            //    (sender as System.ComponentModel.BackgroundWorker).ReportProgress(i);

            }
            //http.Dispose();

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter writer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream file = System.IO.File.Create(cardPath + "cardList");
            writer.Serialize(file, card.cardList);
            file.Close();
            Loading.Visibility = Visibility.Hidden;
        }

        void worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            TextBlock.Text = "Downloading " + card.cardList[e.ProgressPercentage-1].Name;
            textBlock1.Text = e.ProgressPercentage.ToString() + " / " + card.cardList.Count;
        }


        private void DownloadCardSet(string set)
        {
            string response = APIRequest("https://omgvamp-hearthstone-v1.p.mashape.com/cards/sets/" + set + "?collectible=1&locale=frFR");
            //System.IO.File.WriteAllText("sets/" + set, response);
            object cards = Newtonsoft.Json.JsonConvert.DeserializeObject(response, typeof(List<card>));
            card.cardList.AddRange((List<card>)cards);
        }

        private static string APIRequest(string request)
        {
            System.Net.WebClient http = new System.Net.WebClient();
            http.Headers.Add("X-Mashape-Key: LigI9kHL7omsh5NN0rCWg0d6PA5jp1TdGUUjsnexk2zQ42JwCA");
            var response = http.DownloadString(request);
            http.Dispose();
            return response;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //foreach (card item in card.cardList)
            //{
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri(item.Img);
            //bitmap.EndInit();
            //Image oneCard = new Image() { Width = 100, Height = 100 };
            //oneCard.Source = bitmap;
            //stackPanel1.Children.Add(oneCard);
            //}
        }

        private void clearPanel()
        {
            stackPanel1.Children.Clear();
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            clearPanel();

            string PlayerClassFilter;
            string RarityFilter;
            string SearchFilter;
            string CardSetFilter;

            if (PlayerClass.Text == "All")
            {
                PlayerClassFilter = "";
            }
            else
            {
                PlayerClassFilter = PlayerClass.Text;
            }

            if (Rarity.Text == "All")
            {
                RarityFilter = "";
            }
            else
            {
                RarityFilter = Rarity.Text;
            }

            if (CardSet.Text == "All")
            {
                CardSetFilter = "";
            }
            else
            {
                CardSetFilter = CardSet.Text;
            }

            foreach (card item in card.cardList.FindAll(c => c.PlayerClass.Contains(PlayerClassFilter) && c.Rarity.Contains(RarityFilter) && c.CardSet.Contains(CardSetFilter) && c.Name.Contains(Search.Text)))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + item.Img);
                bitmap.EndInit();
                //Image oneCard = new Image() { Width = 307, Height = 465 };
                Image oneCard = new Image() { Width = 50, Height = 75 };
                oneCard.Source = bitmap;
                stackPanel1.Children.Add(oneCard);
            }
        }

    }
}
