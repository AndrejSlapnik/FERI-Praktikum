using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Leksikalni_analizator
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> pomnilnik = new Dictionary<string, string>();

        string izpis = "";

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
                    
                    bool rezultat=EP(ref rezultati);

                    if (Tokenizer.currentToken().lexeme == ";")
                    {
                        Tokenizer.nextToken();

                        string izpisRezultati = "";
                        foreach (var element in rezultati)
                        {
                            izpisRezultati += element.ToString()+" ";
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

            else if(Tokenizer.currentToken().lexeme == "print")
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
            if (Tokenizer.currentToken().type == "lin3" || Tokenizer.currentToken().type == "std3" || Tokenizer.currentToken().type == "pear4" || Tokenizer.currentToken().type == "avg3" || Tokenizer.currentToken().lexeme == "abs" || Tokenizer.currentToken().lexeme == "cos" || Tokenizer.currentToken().lexeme == "sin" || Tokenizer.currentToken().lexeme == "sin" || Tokenizer.currentToken().lexeme == "(" || Tokenizer.currentToken().type == "float" || Tokenizer.currentToken().type == "float3" || Tokenizer.currentToken().lexeme == "-" || Tokenizer.currentToken().type == "vara" || Tokenizer.currentToken().type == "varb")
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
            bool rezultatF= F(ref stevilo);
            bool rezultatTT=TT(ref stevilo);
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

        double avg(double[] polje)
        {
            double vsota = 0.00;

            for (int i = 0; i < polje.Length; i++)
            {
                vsota += polje[i];
            }

            vsota /= (double)polje.Length;

            return vsota;
        }

        double std(double[] polje)
        {
            double povprecje = avg(polje);
            double vsota = 0.00;
            int velikost = polje.Length;

            for (int i = 0; i < velikost; i++)
            {
                double vrednost = polje[i] - povprecje;
                vsota += Math.Pow(vrednost, 2);
            }

            vsota /= (double)velikost;
            vsota = Math.Sqrt(vsota);

            return vsota;
        }

        double pear(double[] polje)
        {
            if (polje.Length % 2 != 0)
            {
                throw new Exception("Velikost statisticne populacije za Pearsonov koef. mora biti soda!");

            }

            double r = 0.00;
            int velikost = polje.Length;
            double[] sodo = new double[velikost / 2];
            double[] liho = new double[sodo.Length];
            double vsota = 0.00;

            for (int i = 0; i < velikost / 2; i++)
            {
                sodo[i] = polje[i];
            }

            for (int i = velikost / 2; i < velikost; i++)
            {
                liho[i - (velikost / 2)] = polje[i];
            }

            velikost /= 2;
            double povprecje_x = avg(sodo);
            double povprecje_y = avg(liho);

            for (int i = 0; i < velikost; i++)
            {
                vsota += ((sodo[i] - povprecje_x) * (liho[i] - povprecje_y));
            }

            double vsota2 = 0.00;

            for (int i = 0; i < velikost; i++)
            {
                vsota2 += Math.Pow((sodo[i] - povprecje_x), 2);
            }

            vsota2 = Math.Sqrt(vsota2);

            double vsota3 = 0.00;

            for (int i = 0; i < velikost; i++)
            {
                vsota3 += Math.Pow((liho[i] - povprecje_y), 2);
            }

            vsota3 = Math.Sqrt(vsota3);

            vsota2 *= vsota3;

            r = vsota / vsota2;

            return r;
        }

        int comparePoints(PointF a, PointF b)
        {
            if (a.X < b.X) return -1;
            if (a.X > b.X) return 1;
            return 0;
        }

        double linear(double[] polje)
        {
            if (polje.Length < 5 || polje.Length%2 != 1) return 0;

            double[] x = new double[(polje.Length - 1) / 2];
            double[] y = new double[(polje.Length - 1) / 2];

            double xt = polje[polje.Length-1];

            PointF[] points = new PointF[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = polje[i];
            }

            for (int i = 0; i < y.Length; i++)
            {
                y[i] = polje[i+x.Length];
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointF((float)x[i], (float)y[i]);
            }//pretvorimo polje v točke

            List<PointF> listPoints = new List<PointF>(points);

            listPoints.Sort(comparePoints);
            //sortirano po x osi

            if (xt < listPoints.First().X) return 0;
            if (xt > listPoints.Last().X) return 0;
            //ni v intervalu

            int secondPointIndex = listPoints.Count; //če ni nobena večja in je v intervalu, je zadnja večja

            int firstPointIndex = listPoints.Count-1;

            for (int i = 0; i < listPoints.Count; i++)
            {
                if (xt < listPoints[i].X)
                {
                    secondPointIndex = i;
                    firstPointIndex = i - 1;
                    break;
                }
            }

            double a, b;

            a = xt - listPoints[firstPointIndex].X;
            b = listPoints[secondPointIndex].X - xt;

            double y1 = a / (a + b) * listPoints[firstPointIndex].Y;
            double y2 = b / (a + b) * listPoints[secondPointIndex].Y;

            double yn = y1 + y2;

            return yn;
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
            else if (Tokenizer.currentToken().type == "lin3" || Tokenizer.currentToken().type == "avg3" || Tokenizer.currentToken().type == "std3" || Tokenizer.currentToken().type == "pear4")
            {
                token funkcija = Tokenizer.currentToken();
                Tokenizer.nextToken();

                if (Tokenizer.currentToken().lexeme == "(")
                {
                    Tokenizer.nextToken();

                    if (Tokenizer.currentToken().type == "vara" || Tokenizer.currentToken().type == "varb")
                    {
                        token spremenljivka = Tokenizer.currentToken();

                        Tokenizer.nextToken();

                        try
                        {
                            string strpolje="";
                            if (pomnilnik.ContainsKey(spremenljivka.lexeme))
                                strpolje = pomnilnik[spremenljivka.lexeme];

                            if (strpolje == "") return false;

                            string[] splitstrpolje = strpolje.Split(new char[] { ' ' });

                            double[] polje = new double[splitstrpolje.Length-1];

                            for (int i = 0; i < polje.Length; i++)
                            {
                                polje[i] = double.Parse(splitstrpolje[i]);
                            }
                            double rezultat = 0;
                            switch (funkcija.lexeme)
                            {
                                case "avg":
                                    rezultat = avg(polje);
                                    break;

                                case "std":
                                    rezultat = std(polje);
                                    break;

                                case "pear":
                                    rezultat = pear(polje);
                                    break;

                                case "lin":
                                    rezultat = linear(polje);
                                    break;
                            }

                            vhodnoStevilo = rezultat;

                        }
                        catch (Exception ex)
                        {
                            izpis += "Ne da se uporabiti spremenljivke z polju v aritmetičnih izrazih!\r\n";
                            vhodnoStevilo = double.NaN;
                        }

                        if (Tokenizer.currentToken().lexeme == ")")
                        {
                            Tokenizer.nextToken();

                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
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
                vhodnoStevilo = double.Parse(Tokenizer.currentToken().lexeme.Replace('.',','));

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
                    vhodnoStevilo = double.Parse(Tokenizer.currentToken().lexeme.Replace('.',','));

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
                catch (Exception ex)
                {
                    izpis += "Ne da se uporabiti spremenljivke z polju v aritmetičnih izrazih!\r\n";
                    vhodnoStevilo = double.NaN;
                }
                Tokenizer.nextToken();
                return true;
            }
            return false;
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
            int col = 1, startCol=1;
            int row = 1, startRow=1;
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

        public Form1()
        {
            InitializeComponent();

            StreamReader reader = new StreamReader("table.txt");

            List<String> lines = new List<String>();

            while (!reader.EndOfStream)
            {
                lines.Add(reader.ReadLine());
            } //preberemo vrstice

            List<String> končna_stanja = lines[0].Split('\t').ToList();
            lines.RemoveAt(0); //zapišemo končna stanja

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

                if (preslikave.Count != abeceda.Count) throw new Exception("Napaka v vrstici \""+vrstica+"\", ni enako število možnosti kot je znakov v abecedi.");
            } //zapišemo vsa pravila / preslikave

            reader.Close();

            Tokenizer = new tokenizer(pravila, abeceda, končna_stanja, "test.txt");
        }

        tokenizer Tokenizer;

        private void button1_Click(object sender, EventArgs e)
        {
            token temp = Tokenizer.nextToken();
            textBox1.Text += "Vrednost: "+temp.lexeme + "\tStanje: " + temp.type + "\tCol: "+temp.col+"\tRow:"+temp.row+"\tEof:"+temp.eof+"\r\n";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            token temp;
            do
            {
                temp = Tokenizer.nextToken();
                textBox1.Text += "Vrednost: " + temp.lexeme + "\tStanje: " + temp.type + "\tCol: " + temp.col + "\tRow:" + temp.row + "\tEof:" + temp.eof + "\r\n";
            } while (!temp.eof);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool parsed = parse();
            textBox1.Text += "Parse uspel: " + parsed + "\r\nIzpis:\r\n"+izpis;
        }
    }
}
