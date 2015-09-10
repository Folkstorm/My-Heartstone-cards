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
                    System.IO.FileStream file = System.IO.File.OpenRead(cardPath + "cardList");
                    card.cardList = (List<card>)reader.Deserialize(file);
                    file.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Your card database seems to be corrupted, we will redownload it");
                    DownloadAllCardSet();
                }

            }
            if (System.IO.File.Exists(cardPath + "cardCollection") == true)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter reader = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.FileStream file = System.IO.File.OpenRead(cardPath + "cardCollection");
                card.MyCollection = (List<card>)reader.Deserialize(file);
                file.Close();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DownloadAllCardSet();
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
            System.Net.WebClient http = new System.Net.WebClient();
            int i = 0;
            foreach (card item in card.cardList)
            {
                http.DownloadFile(item.Img, cardPath + item.CardId);
                http.DownloadFile(item.ImgGold, cardPath + item.CardId + "g");
                item.Img = cardPath + item.CardId;
                item.ImgGold = cardPath + item.CardId + "g";
                if (item.PlayerClass == null)
                {
                    item.PlayerClass = "Neutral";
                }
                i++;
                (sender as System.ComponentModel.BackgroundWorker).ReportProgress(i);

            }
            card.cardList.RemoveAll(c => c.Type == "Hero");
            http.Dispose();

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter writer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream file = System.IO.File.Create(cardPath + "cardList");
            writer.Serialize(file, card.cardList);
            file.Close();
        }

        void worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            TextBlock.Text = "Downloading " + card.cardList[e.ProgressPercentage - 1].Name;
            textBlock1.Text = e.ProgressPercentage.ToString() + " / " + card.cardList.Count;
            if (e.ProgressPercentage == card.cardList.Count)
            {
                Loading.Visibility = Visibility.Hidden;
            }
        }


        private void DownloadCardSet(string set)
        {
            string response = APIRequest("https://omgvamp-hearthstone-v1.p.mashape.com/cards/sets/" + set + "?collectible=1&locale=frFR");
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

        private void clearPanel()
        {
            stackPanel1.Children.Clear();
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            SearchAndShow();
        }

        private void SearchAndShow()
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

            foreach (card item in card.cardList.FindAll(c =>
                c.PlayerClass.Contains(PlayerClassFilter) &&
                c.Rarity.Contains(RarityFilter) &&
                c.CardSet.Contains(CardSetFilter) &&
                c.Name.ToLower().Contains(Search.Text.ToLower()) &&
                int.Parse(c.Cost) >= int.Parse(ManaMin.Text) &&
                int.Parse(c.Cost) <= int.Parse(ManaMax.Text)
                ))
            {

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + item.Img);
                bitmap.EndInit();
                CardImage oneCard = new CardImage();// { Width = 307, Height = 465 };
                oneCard.MouseLeftButtonDown += new MouseButtonEventHandler(oneCard_MouseLeftButtonDown);
                oneCard.MouseRightButtonDown += new MouseButtonEventHandler(oneCard_MouseRightButtonDown);
                oneCard.MouseEnter += new MouseEventHandler(oneCard_MouseEnter);
                oneCard.MouseLeave += new MouseEventHandler(oneCard_MouseLeave);
                oneCard.Tag = item;
                oneCard.image1.Source = bitmap;
                oneCard.label1.Content = getNumberOfCardInCollection(item.CardId).ToString();
                stackPanel1.Children.Add(oneCard);
            }
        }





        void oneCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int numberOfCard = getNumberOfCardInCollection(((card)((CardImage)sender).Tag).CardId);
            if (numberOfCard < 2)
            {

                if (numberOfCard == 1 && ((card)((CardImage)sender).Tag).Rarity == "Legendary")
                    return;

                card.MyCollection.Add((card)((CardImage)sender).Tag);
                SearchAndShow();
            }
        }

        void oneCard_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (getNumberOfCardInCollection(((card)((CardImage)sender).Tag).CardId) > 0)
            {
                card.MyCollection.Remove((card)((CardImage)sender).Tag);
                SearchAndShow();
            }
        }

        void oneCard_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CardImage)sender).Background = Brushes.PaleGoldenrod;
        }

        void oneCard_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CardImage)sender).Background = null;
        }

        int getNumberOfCardInCollection(string CardId)
        {
            return card.MyCollection.FindAll(c => c.CardId == CardId).Count;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter writer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream file = System.IO.File.Create(cardPath + "cardCollection");
            writer.Serialize(file, card.MyCollection);
            file.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            List<card> MissingCards = new List<card>();

            foreach (card item in card.cardList)
            {
                int numberOfCard = getNumberOfCardInCollection(item.CardId);
                int numberToHave;
                if (item.Rarity == "Legendary")
                {
                    numberToHave = 1;
                }
                else
                {
                    numberToHave = 2;
                }

                for (int i = 0; i < numberToHave - numberOfCard; i++)
                {
                    MissingCards.Add(item);
                }

            }

            int missingDust = 0;

            int missingDustClassic = 0;
            int missingDustGvG = 0;
            int missingDustTgT = 0;

            unsafe
            {
                int* dust = null;

                foreach (card item in MissingCards.FindAll(c => c.CardSet == "Classic" || c.CardSet == "Goblins vs Gnomes" || c.CardSet == "The Grand Tournament"))
                {

                    switch (item.CardSet)
                    {
                        case "Classic":
                            dust = &missingDustClassic;
                            break;
                        case "Goblins vs Gnomes":
                            dust = &missingDustGvG;
                            break;
                        case "The Grand Tournament":
                            dust = &missingDustTgT;
                            break;
                        default:
                            break;
                    }

                    switch (item.Rarity)
                    {
                        case "Common":
                            *dust += 40;
                            break;
                        case "Rare":
                            *dust += 100;
                            break;
                        case "Epic":
                            *dust += 400;
                            break;
                        case "Legendary":
                            *dust += 1600;
                            break;
                        default:
                            break;
                    }
                }
            }
            missingDust = missingDustClassic + missingDustGvG + missingDustTgT;
            MessageBox.Show(missingDust.ToString());

        }

    }
}
