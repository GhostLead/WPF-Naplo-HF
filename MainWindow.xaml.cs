using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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

namespace WpfOsztalyzas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fajlNev = "naplo.txt";
        double atlag = 0;
        //Így minden metódus fogja tudni használni.
        ObservableCollection<Osztalyzat> jegyek = new ObservableCollection<Osztalyzat>();

        public MainWindow()
        {
            InitializeComponent();
            // todo Fájlok kitallózásával tegye lehetővé a naplófájl kiválasztását!
            // Ha nem választ ki semmit, akkor "naplo.csv" legyen az állomány neve. A későbbiekben ebbe fog rögzíteni a program.

            // todo A kiválasztott naplót egyből töltse be és a tartalmát jelenítse meg a datagrid-ben!
            dgJegyek.ItemsSource = jegyek;
            lblNaploFajlNeve.Content = fajlNev;
            lblJegySzam.Content = dgJegyek.Items.Count;
            
            List<int> szamok = new List<int>();
            foreach (var item in jegyek)
            {
                string[] adatok = item.ToString().Split(";");
                szamok.Add(Convert.ToInt32(adatok[3]));
            }
            foreach (var item in szamok) { atlag += item; }
            atlag = atlag / szamok.Count;
            Math.Round(atlag, 2);
            lblAtlag.Content = atlag.ToString();

            
        }

        private void btnRogzit_Click(object sender, RoutedEventArgs e)
        {
            bool joNevHossz = false;

            try
            {
                string[] nev = txtNev.Text.Split(" ");
                if (nev[0].Length >= 3 && nev[1].Length >= 3)
                {
                    joNevHossz = true;
                }


                if (txtNev.Text.Contains(" ") == true && joNevHossz == true)
                {
                    string Nev = "";
                    if (rdKerVez.IsChecked == true)
                    {
                        string[] stringNev = txtNev.Text.Split(" ");
                        Nev = $"{stringNev[1]} {stringNev[0]}";
                    }
                    else if (rdVezKer.IsChecked == true)
                    {
                        string[] stringNev = txtNev.Text.Split(" ");
                        Nev = $"{stringNev[0]} {stringNev[1]}";
                    }

                    //A CSV szerkezetű fájlba kerülő sor előállítása
                    string csvSor = $"{Nev};{datDatum.Text};{cboTantargy.Text};{sliJegy.Value}";
                    string[] csvSorTomb = csvSor.Split(";");
                    Osztalyzat ujJegy = new Osztalyzat(csvSorTomb[0], csvSorTomb[1], csvSorTomb[2], int.Parse(csvSorTomb[3]));
                    jegyek.Add(ujJegy);
                    //Megnyitás hozzáfűzéses írása (APPEND)
                    StreamWriter sw = new StreamWriter(fajlNev, append: true);
                    sw.WriteLine(csvSor);
                    sw.Close();
                    //todo Az újonnan felvitt jegy is jelenjen meg a datagrid-ben!
                    
                }
                else
                {
                    MessageBox.Show("Helytelen beállítások!");
                }

            }
            catch (Exception)
            {

                MessageBox.Show("Helytelen beállítások!");
            }
            dgJegyek.ItemsSource = jegyek;
            lblJegySzam.Content = dgJegyek.Items.Count;
            lblAtlag.Content = atlag.ToString();




            //todo Ne lehessen rögzíteni, ha a következők valamelyike nem teljesül!
            // a) - A név legalább két szóból álljon és szavanként minimum 3 karakterből!
            //      Szó = A szöközökkel határolt karaktersorozat.
            // b) - A beírt dátum újabb, mint a mai dátum ------------------------------------------------------ !!!!!!!!!!!!!!!!!!!

            //todo A rögzítés mindig az aktuálisan megnyitott naplófájlba történjen! --------------------------- !!!!!!!!!!!!!!!!!!!



        }

        private void btnBetolt_Click(object sender, RoutedEventArgs e)
        {
            jegyek.Clear();  //A lista előző tartalmát töröljük
            StreamReader sr = new StreamReader(fajlNev); //olvasásra nyitja az állományt
            while (!sr.EndOfStream) //amíg nem ér a fájl végére
            {
                string[] mezok = sr.ReadLine().Split(";"); //A beolvasott sort feltördeli mezőkre
                //A mezők értékeit felhasználva létrehoz egy objektumot
                Osztalyzat ujJegy = new Osztalyzat(mezok[0], mezok[1], mezok[2], int.Parse(mezok[3])); 
                jegyek.Add(ujJegy); //Az objektumot a lista végére helyezi
            }
            sr.Close(); //állomány lezárása

            //A Datagrid adatforrása a jegyek nevű lista lesz.
            //A lista objektumokat tartalmaz. Az objektumok lesznek a rács sorai.
            //Az objektum nyilvános tulajdonságai kerülnek be az oszlopokba.
            dgJegyek.ItemsSource = jegyek;
            lblJegySzam.Content = dgJegyek.Items.Count;
            lblAtlag.Content = atlag.ToString();
        }

        private void sliJegy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblJegy.Content = sliJegy.Value; //Több alternatíva van e helyett! Legjobb a Data Binding!
        }

        //todo Felület bővítése: Az XAML átszerkesztésével biztosítsa, hogy láthatóak legyenek a következők!
        // - A naplófájl neve
        // - A naplóban lévő jegyek száma
        // - Az átlag

        //todo Új elemek frissítése: Figyeljen rá, ha új jegyet rögzít, akkor frissítse a jegyek számát és az átlagot is!

        //todo Helyezzen el alkalmas helyre 2 rádiónyomógombot!
        //Feliratok: [■] Vezetéknév->Keresztnév [O] Keresztnév->Vezetéknév
        //A táblázatban a név azserint szerepeljen, amit a rádiónyomógomb mutat!
        //A feladat megoldásához használja fel a ForditottNev metódust!
        //Módosíthatja az osztályban a Nev property hozzáférhetőségét!
        //Megjegyzés: Felételezzük, hogy csak 2 tagú nevek vannak
    }
}

