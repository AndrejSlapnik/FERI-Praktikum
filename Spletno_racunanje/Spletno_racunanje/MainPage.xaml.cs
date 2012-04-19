using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;

namespace Spletno_racunanje
{
    public partial class MainPage : UserControl
    {
        Dictionary<string, string> pomnilnik = new Dictionary<string, string>();

        string izpis = "";
        tokenizer Tokenizer = null;

        public MainPage()
        {
            InitializeComponent();
        }

        public bool parse()
        {
            token Token = Tokenizer.nextToken();
            return PRI() && Tokenizer.currentToken().eof;
        }

        public bool PRI()
        {
            if (Tokenizer.currentToken().type == "vara" || Tokenizer.currentToken().type == "varb")
            {
                token spremenljivka = Tokenizer.currentToken();
                Tokenizer.nextToken();

                if (Tokenizer.currentToken().type == "pri2")
                {
                    Tokenizer.nextToken();

                    List<double> rezultati = new List<double>();

                    bool rezultat = EP(ref rezultati);

                    if (Tokenizer.currentToken().lexeme == ";")
                    {
                        Tokenizer.nextToken();

                        string izpisRezultati = "";
                        foreach (var element in rezultati)
                        {
                            izpisRezultati += element.ToString() + " ";
                        }

                        if (!pomnilnik.ContainsKey(spremenljivka.lexeme))
                            pomnilnik.Add(spremenljivka.lexeme, izpisRezultati);
                        else pomnilnik[spremenljivka.lexeme] = izpisRezultati;

                        return rezultat && PRI();
                    }

                    else return false;
                }

                else return false;
            }

            else if (Tokenizer.currentToken().lexeme == "print")
            {
                Tokenizer.nextToken();

                if (Tokenizer.currentToken().type == "vara" || Tokenizer.currentToken().type == "varb")
                {
                    token spremenljivka = Tokenizer.currentToken();
                    Tokenizer.nextToken();

                    if (Tokenizer.currentToken().lexeme == ";")
                    {
                        Tokenizer.nextToken();

                        if (pomnilnik.ContainsKey(spremenljivka.lexeme))
                            izpis += "Spremenljivka ima vrednost " + pomnilnik[spremenljivka.lexeme] + "\r\n";
                        else izpis += "Ne obstaja spremenljivka " + spremenljivka.lexeme + "\r\n";

                        return PRI();
                    }

                    else return false;
                }

                else return false;
            }

            else return true;
        }

        public bool EP(ref List<double> polje)
        {
            if (Tokenizer.currentToken().lexeme == "abs" || Tokenizer.currentToken().lexeme == "cos" || Tokenizer.currentToken().lexeme == "sin" || Tokenizer.currentToken().lexeme == "sin" || Tokenizer.currentToken().lexeme == "(" || Tokenizer.currentToken().type == "float" || Tokenizer.currentToken().type == "float3" || Tokenizer.currentToken().lexeme == "-" || Tokenizer.currentToken().type == "vara" || Tokenizer.currentToken().type == "varb")
            {
                double vrednostIzraza = 0;
                bool rezultatE = E(ref vrednostIzraza);
                polje.Add(vrednostIzraza);
                return rezultatE;
            }

            else if (Tokenizer.currentToken().lexeme == "{")
            {
                return POLJE(ref polje);
            }

            else return false;
        }

        public bool POLJE(ref List<double> polje)
        {
            if (Tokenizer.currentToken().lexeme == "{")
            {
                Tokenizer.nextToken();
                bool rezultat = NUMS(ref polje);

                if (Tokenizer.currentToken().lexeme == "}")
                {
                    Tokenizer.nextToken();

                    return rezultat;
                }

                else return false;
            }

            return false;
        }

        public bool NUMS(ref List<double> polje)
        {
            double stevilo = 0;
            bool rezultatF = F(ref stevilo);
            polje.Add(stevilo);
            bool rezultatNUM = NUM(ref polje);
            return rezultatF && rezultatNUM;
        }

        public bool NUM(ref List<double> polje)
        {
            if (Tokenizer.currentToken().lexeme == ",")
            {
                Tokenizer.nextToken();

                double stevilo = 0;
                bool rezultatF = F(ref stevilo);
                polje.Add(stevilo);
                bool rezultatNUM = NUM(ref polje);
                return rezultatF && rezultatNUM;
            }

            return true;
        }

        bool E(ref double stevilo)
        {
            bool rezultatT = T(ref stevilo);
            bool rezultatEE = EE(ref stevilo);
            return rezultatT && rezultatEE;
        }

        bool EE(ref double vhodnoStevilo)
        {
            if (Tokenizer.currentToken().lexeme == "+" ||
                Tokenizer.currentToken().lexeme == "-")
            {
                token spremenljivka = Tokenizer.currentToken();
                Tokenizer.nextToken();
                double stevilo = 0;

                bool rezultatT = T(ref stevilo);

                bool rezultatEE = EE(ref vhodnoStevilo);

                if (spremenljivka.lexeme == "+")
                    vhodnoStevilo = stevilo + vhodnoStevilo;
                else
                    vhodnoStevilo = stevilo - vhodnoStevilo;

                return rezultatT && rezultatEE;
            }
            else return true;
        }

        bool T(ref double stevilo)
        {
            bool rezultatF = F(ref stevilo);
            bool rezultatTT = TT(ref stevilo);
            return rezultatF && rezultatTT;
        }

        bool TT(ref double vhodnoStevilo)
        {
            if (Tokenizer.currentToken().lexeme == "/"
                || Tokenizer.currentToken().lexeme == "*")
            {
                token spremenljivka = Tokenizer.currentToken();
                Tokenizer.nextToken();

                double stevilo = 0;
                bool rezultatF = F(ref stevilo);
                bool rezultatTT = TT(ref vhodnoStevilo);

                if (spremenljivka.lexeme == "/")
                    vhodnoStevilo = stevilo / vhodnoStevilo;
                else vhodnoStevilo = stevilo * vhodnoStevilo;

                return rezultatF && rezultatTT;
            }
            else return true;
        }

        bool F(ref double vhodnoStevilo)
        {
            if (Tokenizer.currentToken().type == "abs3"
                || Tokenizer.currentToken().type == "sin3"
                || Tokenizer.currentToken().type == "cos3")
            {
                token spremenljivka = Tokenizer.currentToken();
                Tokenizer.nextToken();
                if (Tokenizer.currentToken().lexeme == "(")
                {
                    Tokenizer.nextToken();
                    bool temp = E(ref vhodnoStevilo);
                    switch (spremenljivka.lexeme)
                    {
                        case "abs":
                            vhodnoStevilo = Math.Abs(vhodnoStevilo);
                            break;
                        case "sin":
                            vhodnoStevilo = Math.Sin(vhodnoStevilo);
                            break;
                        case "cos":
                            vhodnoStevilo = Math.Cos(vhodnoStevilo);
                            break;
                    }
                    if (Tokenizer.currentToken().lexeme == ")")
                    {
                        Tokenizer.nextToken();
                        return temp;
                    }
                    else return false;
                }
                return false;
            }
            else if (Tokenizer.currentToken().lexeme == "(")
            {
                Tokenizer.nextToken();
                bool temp = E(ref vhodnoStevilo);

                if (Tokenizer.currentToken().lexeme == ")")
                {
                    Tokenizer.nextToken();
                    return temp;
                }
                else return false;
            }
            else if (Tokenizer.currentToken().type == "float"
                || Tokenizer.currentToken().type == "float3"
                )
            {
                vhodnoStevilo = double.Parse(Tokenizer.currentToken().lexeme);

                Tokenizer.nextToken();
                return true;
            }

            else if (Tokenizer.currentToken().lexeme == "-")
            {
                Tokenizer.nextToken();
                if (Tokenizer.currentToken().type == "float"
                || Tokenizer.currentToken().type == "float3"
                )
                {
                    vhodnoStevilo = double.Parse(Tokenizer.currentToken().lexeme);

                    Tokenizer.nextToken();
                    return true;
                }
                else return false;
            }
            else if (Tokenizer.currentToken().type == "vara"
                || Tokenizer.currentToken().type == "varb")
            {
                try
                {
                    if (pomnilnik.ContainsKey(Tokenizer.currentToken().lexeme))
                        vhodnoStevilo = double.Parse(pomnilnik[Tokenizer.currentToken().lexeme]);
                }
                catch (Exception)
                {
                    izpis += "Ne da se uporabiti spremenljivke z polju v aritmetičnih izrazih!\r\n";
                    vhodnoStevilo = double.NaN;
                }
                Tokenizer.nextToken();
                return true;
            }
            return false;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            //dialog.Filter = "WS datoteke | *.ws";

            bool? rezultat = dialog.ShowDialog();

            if (rezultat == true)
            {
                try
                {
                    using (FileStream tok = dialog.File.OpenRead())
                    {
                        byte[] pomnilnik = new byte[(int)tok.Length];
                        tok.Read(pomnilnik, 0, (int)tok.Length);

                        char[] niz = new char[pomnilnik.Length];

                        for (int i = 0; i < pomnilnik.Length; i++)
                        {
                            niz[i] = (char)pomnilnik[i];
                        }

                        IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

                        IsolatedStorageFileStream tok2 = new IsolatedStorageFileStream("izvorna.koda", FileMode.Create, ISF);
                        tok2.Write(pomnilnik, 0, pomnilnik.Length);

                        tok2.Close();
                        Pocisti(0);

                        for (int i = 0; i < pomnilnik.Length; i++)
                        {
                            textBox1.Text += niz[i];
                        }

                        IsolatedStorageFileStream tok3 = new IsolatedStorageFileStream("izvorna.koda", FileMode.Open, ISF);
                        tok3.Read(pomnilnik, 0, (int)tok3.Length);

                        tok3.Close();
                        niz = new char[pomnilnik.Length];

                        for (int i = 0; i < pomnilnik.Length; i++)
                        {
                            niz[i] = (char)pomnilnik[i];
                        }

                        string vsebina_datoteke = "", vsebina_tabele = "";

                        for (int i = 0; i < pomnilnik.Length; i++) vsebina_datoteke += niz[i];

                        IsolatedStorageFileStream tok4 = new IsolatedStorageFileStream("tabela.podatki", FileMode.Open, ISF);
                        pomnilnik = new byte[tok4.Length];
                        tok4.Read(pomnilnik, 0, (int)tok4.Length);

                        tok4.Close();
                        niz = new char[pomnilnik.Length];

                        for (int i = 0; i < pomnilnik.Length; i++)
                        {
                            niz[i] = (char)pomnilnik[i];
                        }

                        for (int i = 0; i < pomnilnik.Length; i++) vsebina_tabele += niz[i];

                        tokenizer leksikalni_analizator = new tokenizer(vsebina_tabele, vsebina_datoteke);
                        String tabela = leksikalni_analizator.vrniTabelo();

                        //Zacasni izpis tabele osnovnih leksikalnih simbolov in leksilanih vrednosti.
                        MessageBox.Show(tabela.ToString());

                        textBox2.Text = izpis;
                    }
                }

                catch (Exception)
                {
                    MessageBox.Show("Napaka pri odpiranju datoteke izvorne programske kode!");
                }
            }

            else MessageBox.Show("Preklicano!");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.Filter = "JFIF zapis|*.jpg|Bitna mapa|*.bmp|PNG zapis|*.png";

            bool? rezultat = dialog.ShowDialog();

            if (rezultat == true)
            {
                try
                {
                    using (FileStream tok = dialog.File.OpenRead())
                    {
                        byte[] pomnilnik = new byte[(int)tok.Length];
                        tok.Read(pomnilnik, 0, (int)tok.Length);

                        IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

                        IsolatedStorageFileStream tok2 = new IsolatedStorageFileStream("izvorna.slika", FileMode.Create, ISF);
                        tok2.Write(pomnilnik, 0, pomnilnik.Length);
                        tok2.Close();

                        byte[] podatki;

                        using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                        {

                            using (IsolatedStorageFileStream ISF_tok = isf.OpenFile("izvorna.slika", FileMode.Open, FileAccess.Read))
                            {
                                podatki = new byte[ISF_tok.Length];
                                ISF_tok.Read(podatki, 0, podatki.Length);
                                ISF_tok.Close();
                            }
                        }

                        MemoryStream spomin = new MemoryStream(podatki);
                        BitmapImage BI = new BitmapImage();

                        BI.SetSource(spomin);
                        Image image = new Image();

                        image1.Height = BI.PixelHeight;
                        image1.Width = BI.PixelWidth;
                        image1.Source = BI;
                    }
                }

                catch (Exception)
                {
                    MessageBox.Show("Napaka pri odpiranju slike!");
                }
            }

            else MessageBox.Show("Preklicano!");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream tok = new IsolatedStorageFileStream("izvorna2.koda", FileMode.Create, ISF);

                String izvorna_koda = textBox1.Text;

                byte[] pomnilnik = new byte[izvorna_koda.Length];

                for (int i = 0; i < pomnilnik.Length; i++)
                {
                    pomnilnik[i] = (byte)izvorna_koda[i];
                }

                tok.Write(pomnilnik, 0, pomnilnik.Length);
                tok.Close();
            }

            catch (Exception)
            {
                MessageBox.Show("Napaka v fazi zapisovanja na varno sekcijo strajnega pomnilnika!");
            }
        }

        private void Pocisti(int lokacija)
        {
            switch (lokacija)
            {
                case 0:
                    {
                        textBox1.Text = "";
                        break;
                    }

                case 1:
                    {
                        textBox2.Text = "";
                        break;
                    }

                case 2:
                    {
                        textBox3.Text = "";
                        break;
                    }

                default: break;
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Pocisti(1);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            Pocisti(2);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            Pocisti(0);
        }

        class tokenizer
        {
            List<Char> abeceda;
            Dictionary<String, List<String>> pravila;
            List<String> končna;
            List<String> stanja;
            String stanje;
            String file;
            String lexeme;
            int col = 1, startCol = 1;
            int row = 1, startRow = 1;
            int pos = 0;
            bool stop = false;
            token current;

            public token currentToken()
            {
                return current;
            }

            public tokenizer(Dictionary<string, List<string>> pravila, List<char> abeceda, List<string> končna_stanja, String filename)
            {
                this.pravila = pravila;
                this.abeceda = abeceda;
                this.končna = končna_stanja;
                stanja = pravila.Keys.ToList();
                stanje = "start";
                lexeme = "";
                StreamReader reader = new StreamReader(filename);
                file = reader.ReadToEnd();
                reader.Close();
            }

            public token nextToken()
            {
                if (stop) return current = new token(-1, -1, true, "eof", "eof");
                if (pos == file.Length)
                {
                    stop = true;
                    return current = new token(startCol, startRow, true, lexeme, stanje);
                }
                if (file[pos] == '\n')
                {
                    col = 1;
                    row++;
                    pos++;
                    return nextToken();
                }
                if (file[pos] == ' ' || file[pos] == '\t' || file[pos] == '\r')
                {
                    pos++;
                    col++;
                    return nextToken();
                }

                if (abeceda.IndexOf(file[pos]) == -1) throw new Exception("Napaka v datoteki, znak ni del abecede");

                String produkcija = pravila[stanje][abeceda.IndexOf(file[pos])];
                if (produkcija == "~" && končna.IndexOf(stanje) == -1)
                {//produkcije ni v tabeli, stanje pa ni končno
                    throw new Exception("Napaka v datoteki, nisem našel končne produkcije");
                }

                if ((produkcija == "~" || pos == file.Length) && končna.IndexOf(stanje) != -1)
                {//produkcije ni v tabeli, stanje je pa končno, vrnemo token.


                    String oldLexeme = lexeme;
                    String oldStanje = stanje;
                    stanje = "start";
                    lexeme = "";
                    int oldCol = startCol, oldRow = startRow;
                    startCol = col;
                    startRow = row;
                    //int oldPos = pos;
                    //pos++;
                    //col++;

                    return current = new token(oldCol, oldRow, false, oldLexeme, oldStanje);
                }

                //produkcija je veljavna
                lexeme += file[pos];
                stanje = produkcija;
                pos++;
                col++;
                return nextToken();
            }

            public tokenizer(String tabela, String vsebina)
            {
                List<String> lines = new List<String>();

                foreach (var line in tabela.Replace("\r", "").Split('\n'))
                {
                    lines.Add(line);
                }

                List<String> konèna_stanja = lines[0].Split('\t').ToList();
                lines.RemoveAt(0); //zapišemo konèna stanja

                List<Char> abeceda = new List<Char>();
                foreach (var znak in lines[0].Split('\t'))
                {
                    abeceda.Add(znak[0]);
                }//zapišemo abecedo

                lines.RemoveAt(0);

                Dictionary<String, List<String>> pravila = new Dictionary<String, List<String>>();

                foreach (var vrstica in lines)
                {
                    List<String> preslikave = vrstica.Split('\t').ToList();
                    String ime = preslikave[0];
                    preslikave.RemoveAt(0);

                    pravila[ime] = preslikave;

                    if (preslikave.Count != abeceda.Count) throw new Exception("Napaka v vrstici \"" + vrstica + "\", ni enako število možnosti kot je znakov v abecedi.");
                } //zapišemo vsa pravila / preslikave

                
            }


            public String vrniTabelo()
            {
                String tabela = "";
                token temp;
                do
                {
                    temp = this.nextToken();
                    tabela += "Vrednost: " + temp.lexeme + "\tStanje: " + temp.type + "\tCol: " + temp.col + "\tRow:" + temp.row + "\tEof:" + temp.eof + "\r\n";
                } while (!temp.eof);
                return tabela;
            }
        }

        struct token
        {
            public int col, row;
            public bool eof;
            public string lexeme;
            public string type;
            public token(int col, int row, bool eof, string lexeme, string type)
            {
                this.col = col;
                this.row = row;
                this.eof = eof;
                this.lexeme = lexeme;
                this.type = type;
            }
        }

        /*class tokenizer
        {
            List<Char> abeceda;
            Dictionary<String, List<String>> pravila;
            List<String> konèna;
            List<String> stanja;
            String stanje;
            String file;
            String lexeme;
            int col = 1, startCol = 1;
            int row = 1, startRow = 1;
            int pos = 0;
            bool stop = false;

            private void Tokenizer(Dictionary<string, List<string>> pravila, List<char> abeceda, List<string> konèna_stanja, String datoteka)
            {
                this.pravila = pravila;
                this.abeceda = abeceda;
                this.konèna = konèna_stanja;
                stanja = pravila.Keys.ToList();
                stanje = "start";
                lexeme = "";
                file = datoteka;
            }

            public token nextToken()
            {
                if (stop) return new token(-1, -1, true, "eof", "eof");
                if (pos == file.Length)
                {
                    stop = true;
                    return new token(startCol, startRow, true, lexeme, stanje);
                }
                if (file[pos] == '\n')
                {
                    col = 1;
                    row++;
                    pos++;
                    return nextToken();
                }
                if (file[pos] == ' ' || file[pos] == '\t' || file[pos] == '\r')
                {
                    pos++;
                    col++;
                    return nextToken();
                }

                if (abeceda.IndexOf(file[pos]) == -1) throw new Exception("Napaka v datoteki, znak ni del abecede");

                String produkcija = pravila[stanje][abeceda.IndexOf(file[pos])];
                if (produkcija == "~" && konèna.IndexOf(stanje) == -1)
                {//produkcije ni v tabeli, stanje pa ni konèno
                    throw new Exception("Napaka v datoteki, nisem našel konène produkcije");
                }

                if ((produkcija == "~" || pos == file.Length) && konèna.IndexOf(stanje) != -1)
                {//produkcije ni v tabeli, stanje je pa konèno, vrnemo token.

                    String oldLexeme = lexeme;
                    String oldStanje = stanje;
                    stanje = "start";
                    lexeme = "";
                    int oldCol = startCol, oldRow = startRow;
                    startCol = col;
                    startRow = row;

                    return new token(oldCol, oldRow, pos == file.Length, oldLexeme, oldStanje);
                }

                //produkcija je veljavna
                lexeme += file[pos];
                stanje = produkcija;
                pos++;
                col++;
                return nextToken();
            }

            public tokenizer(String tabela, String vsebina)
            {
                List<String> lines = new List<String>();

                foreach (var line in tabela.Replace("\r", "").Split('\n'))
                {
                    lines.Add(line);
                }

                List<String> konèna_stanja = lines[0].Split('\t').ToList();
                lines.RemoveAt(0); //zapišemo konèna stanja

                List<Char> abeceda = new List<Char>();
                foreach (var znak in lines[0].Split('\t'))
                {
                    abeceda.Add(znak[0]);
                }//zapišemo abecedo

                lines.RemoveAt(0);

                Dictionary<String, List<String>> pravila = new Dictionary<String, List<String>>();

                foreach (var vrstica in lines)
                {
                    List<String> preslikave = vrstica.Split('\t').ToList();
                    String ime = preslikave[0];
                    preslikave.RemoveAt(0);

                    pravila[ime] = preslikave;

                    if (preslikave.Count != abeceda.Count) throw new Exception("Napaka v vrstici \"" + vrstica + "\", ni enako število možnosti kot je znakov v abecedi.");
                } //zapišemo vsa pravila / preslikave

                Tokenizer(pravila, abeceda, konèna_stanja, vsebina);
            }


            public String vrniTabelo()
            {
                String tabela = "";
                token temp;
                do
                {
                    temp = this.nextToken();
                    tabela += "Vrednost: " + temp.lexeme + "\tStanje: " + temp.type + "\tCol: " + temp.col + "\tRow:" + temp.row + "\tEof:" + temp.eof + "\r\n";
                } while (!temp.eof);
                return tabela;
            }

            public struct token
            {
                public int col, row;
                public bool eof;
                public string lexeme;
                public string type;
                public token(int col, int row, bool eof, string lexeme, string type)
                {
                    this.col = col;
                    this.row = row;
                    this.eof = eof;
                    this.lexeme = lexeme;
                    this.type = type;
                }
            }
        }*/
    }
}
