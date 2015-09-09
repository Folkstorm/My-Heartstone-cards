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
            http.Dispose();

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter writer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream file = System.IO.File.Create(cardPath + "cardList");
            writer.Serialize(file, card.cardList);
            file.Close();
        }

        void worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            TextBlock.Text = "Downloading " + card.cardList[e.ProgressPercentage-1].Name;
            textBlock1.Text = e.ProgressPercentage.ToString() + " / " + card.cardList.Count;
            if (e.ProgressPercentage == card.cardList.Count)
            {
                Loading.Visibility = Visibility.Hidden;
            }
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

        private void clearPanel()
        {
            stackPanel1.Children.Clear();
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            NewMethod();
        }

        private void NewMethod()
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

            foreach (card item in card.cardList.FindAll(c => c.PlayerClass.Contains(PlayerClassFilter) && c.Rarity.Contains(RarityFilter) && c.CardSet.Contains(CardSetFilter) && c.Name.ToLower().Contains(Search.Text.ToLower())))
            {

                //BitmapImage bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + item.Img);
                //bitmap.EndInit();
                Image oneCard = new Image() { Width = 307, Height = 465 };
                //Image oneCard = new Image() { Width = 50, Height = 75 };
                //oneCard.Stretch = Stretch.Fill;
                oneCard.MouseLeftButtonDown += new MouseButtonEventHandler(oneCard_MouseLeftButtonDown);
                oneCard.MouseRightButtonDown += new MouseButtonEventHandler(oneCard_MouseRightButtonDown);
                oneCard.Tag = item;

                System.Drawing.Bitmap image = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + item.Img);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                //g.CopyFromScreen(MainWindow.Left + posNow.X + 5, MainWindow.Top + posNow.Y + 28, 0, 0, New System.Drawing.Size(200, 200), System.Drawing.CopyPixelOperation.SourceCopy)
                g.DrawString(getNumberOfCardInCollection(item.CardId).ToString(), new System.Drawing.Font("Times New Roman", 20), System.Drawing.Brushes.Black, new System.Drawing.PointF(150, 430));
                System.Windows.Media.ImageSource imgsrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                //'image.Save("C:\Users\x\Desktop\a delete\test.png")
                oneCard.Source = imgsrc;
                image.Dispose();
                g.Dispose();
                //oneCard.Source = bitmap;
                stackPanel1.Children.Add(oneCard);
            }
        }

        void oneCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (getNumberOfCardInCollection(((card)((Image)sender).Tag).CardId) < 2)
            {
                card.MyCollection.Add((card)((Image)sender).Tag);
                NewMethod();
            }
        }

        void oneCard_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (getNumberOfCardInCollection(((card)((Image)sender).Tag).CardId) > 0)
            {
                card.MyCollection.Remove((card)((Image)sender).Tag);
                NewMethod();
            }
        }

        int getNumberOfCardInCollection(string CardId)
        {
            return card.MyCollection.FindAll(c => c.CardId == CardId).Count;
        }

    }
}
