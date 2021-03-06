﻿using System;
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
        WriteableBitmap Binarna_pragovna_segmentacija(Image slika, int prag)
        {
            WriteableBitmap segmentirana = (WriteableBitmap)slika.Source;
            
            Color barva = new Color();
            Color crna = Color.FromArgb(0, 0, 0, 0);
            Color bela = Color.FromArgb(0, 255, 255, 255);

            for (int i = 0; i < segmentirana.PixelWidth; i++)
            {
                for (int j = 0; j < segmentirana.PixelHeight; j++)
                {
                    barva = segmentirana.GetPixel(i, j);

                    if (barva.R < prag && barva.G < prag && barva.B < prag)
                    {
                        segmentirana.SetPixel(i, j, crna);
                    }

                    else
                    {
                        segmentirana.SetPixel(i, j, bela);
                    }
                }
            }

            return segmentirana;
        }

        WriteableBitmap Pragovna_segmentacija(Image slika, int prag, int prag2)
        {
            WriteableBitmap segmentirana = (WriteableBitmap)slika.Source;

            Color barva = new Color();
            Color crna = Color.FromArgb(0, 0, 0, 0);
            Color bela = Color.FromArgb(0, 255, 255, 255);
            Color srednja = Color.FromArgb(0, 127, 127, 127);

            for (int i = 0; i < segmentirana.PixelWidth; i++)
            {
                for (int j = 0; j < segmentirana.PixelHeight; j++)
                {
                    barva = segmentirana.GetPixel(i, j);

                    if (barva.R < prag && barva.G < prag && barva.B < prag)
                    {
                        segmentirana.SetPixel(i, j, crna);
                    }

                    else if (barva.R > prag2 && barva.G > prag2 && barva.B > prag2)
                    {
                        segmentirana.SetPixel(i, j, bela);
                    }

                    else
                    {
                        segmentirana.SetPixel(i, j, srednja);
                    }
                }
            }

            return segmentirana;
        }

        Dictionary<int, int> Histogram_za_komponento(Image slika, int komponenta)
        {
            Dictionary<int, int> vrednosti = new Dictionary<int, int>();
            WriteableBitmap segmentirana = (WriteableBitmap)slika.Source;

            Color barva = new Color();

            for (int i = 0; i < 256; i++)
            {
                vrednosti[i] = 0;
            }

            if (komponenta == 0)
            {
                for (int i = 0; i < segmentirana.PixelWidth; i++)
                {
                    for (int j = 0; j < segmentirana.PixelHeight; j++)
                    {
                        barva = segmentirana.GetPixel(i, j);

                        vrednosti[barva.R]++;
                    }
                }
            }

            else if (komponenta == 1)
            {
                for (int i = 0; i < segmentirana.PixelWidth; i++)
                {
                    for (int j = 0; j < segmentirana.PixelHeight; j++)
                    {
                        barva = segmentirana.GetPixel(i, j);

                        vrednosti[barva.G]++;
                    }
                }
            }

            else
            {
                for (int i = 0; i < segmentirana.PixelWidth; i++)
                {
                    for (int j = 0; j < segmentirana.PixelHeight; j++)
                    {
                        barva = segmentirana.GetPixel(i, j);

                        vrednosti[barva.B]++;
                    }
                }
            }

            return vrednosti;
        }

        int Najvecji_po_komponenti(Dictionary<int, int> vrednosti, int komponenta)
        {
            int najvecje = 0, iteracija = 0;

            for (int i = 0; i < vrednosti.Count; i++)
            {
                if (najvecje < vrednosti[i])
                {
                    najvecje = vrednosti[i];
                    iteracija = i;
                }
            }

            return iteracija;
        }

        WriteableBitmap Prilagojena_adaptivna_pragovna_segmentacija(Image slika, int prag, int prag2, int prag3)
        {
            WriteableBitmap segmentirana = (WriteableBitmap)slika.Source;

            Color barva = new Color();
            Color crna = Color.FromArgb(0, 0, 0, 0);
            Color bela = Color.FromArgb(0, 255, 255, 255);

            for (int i = 0; i < segmentirana.PixelWidth; i++)
            {
                for (int j = 0; j < segmentirana.PixelHeight; j++)
                {
                    barva = segmentirana.GetPixel(i, j);

                    if (barva.R < prag)
                    {
                        segmentirana.SetPixel(i, j, Color.FromArgb(0, 0, barva.G, barva.B));
                    }

                    if (barva.R >= prag)
                    {
                        segmentirana.SetPixel(i, j, Color.FromArgb(0, 255, barva.G, barva.B));
                    }

                    barva = segmentirana.GetPixel(i, j);

                    if (barva.G < prag2)
                    {
                        segmentirana.SetPixel(i, j, Color.FromArgb(0, barva.R, 0, barva.B));
                    }

                    if (barva.G >= prag2)
                    {
                        segmentirana.SetPixel(i, j, Color.FromArgb(0, barva.R, 255, barva.B));
                    }

                    barva = segmentirana.GetPixel(i, j);

                    if (barva.B < prag2)
                    {
                        segmentirana.SetPixel(i, j, Color.FromArgb(0, barva.R, barva.G, 0));
                    }

                    if (barva.B >= prag2)
                    {
                        segmentirana.SetPixel(i, j, Color.FromArgb(0, barva.R, barva.G, 255));
                    }
                }
            }

            return segmentirana;
        }



        public class PointF
        {
            public double X, Y;

            public PointF(double a, double b)
            {
                X = a;
                Y = b;
            }
        }
        
        Dictionary<string, string> pomnilnik = new Dictionary<string, string>();

        string izpis = "", rezultatiIzpis="";
        tokenizer Tokenizer = null;

        public MainPage()
        {
            InitializeComponent();
            image1 = new Image();
        }

        public bool parse()
        {
            token Token = Tokenizer.nextToken();
            return PRI() && Tokenizer.currentToken().eof;
        }

        public bool PRI()
        {
            if (Tokenizer.currentToken().type == "paps")
            {
                if (Tokenizer.currentToken().lexeme == "(")
                {
                    Tokenizer.nextToken();

                    if (Tokenizer.currentToken().lexeme == ")")
                    {
                        Tokenizer.nextToken();

                        Prilagojena_adaptivna_pragovna_segmentacija(LoadImage(), Najvecji_po_komponenti(Histogram_za_komponento(LoadImage(), 0), 0), Najvecji_po_komponenti(Histogram_za_komponento(LoadImage(), 1), 1), Najvecji_po_komponenti(Histogram_za_komponento(LoadImage(), 2), 2));

                        if (Tokenizer.currentToken().lexeme == ";")
                        {
                            Tokenizer.nextToken();

                            return PRI();
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
                return true;
            }
            else if (Tokenizer.currentToken().type == "ps")
            {
                if (Tokenizer.currentToken().lexeme == "(")
                {
                    Tokenizer.nextToken();

                    if (Tokenizer.currentToken().type == "float")
                    {
                        int prag1 = int.Parse(Tokenizer.currentToken().lexeme);
                        Tokenizer.nextToken();

                        if (Tokenizer.currentToken().lexeme == ",")
                        {
                            Tokenizer.nextToken();

                            if (Tokenizer.currentToken().type == "float")
                            {
                                int prag2 = int.Parse(Tokenizer.currentToken().lexeme);
                                Tokenizer.nextToken();

                                image1.Source = Pragovna_segmentacija(LoadImage(), prag1, prag2);
                                image1.InvalidateArrange();
                                image1.InvalidateMeasure();

                                if (Tokenizer.currentToken().lexeme == ")")
                                {
                                    Tokenizer.nextToken();

                                    if (Tokenizer.currentToken().lexeme == ";")
                                    {
                                        Tokenizer.nextToken();

                                        return PRI();
                                    }
                                    else return false;
                                }
                                else return false;
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;

                return true;
            }
            else if (Tokenizer.currentToken().type == "bsp")
            {
                Tokenizer.nextToken();
                if (Tokenizer.currentToken().lexeme == "(")
                {
                    Tokenizer.nextToken();

                    if (Tokenizer.currentToken().type == "float")
                    {
                        int prag = int.Parse( Tokenizer.currentToken().lexeme );
                        Tokenizer.nextToken();

                        if (Tokenizer.currentToken().lexeme == ")")
                        {
                            Tokenizer.nextToken();

                            //MessageBox.Show("Vrnilo sliko");

                            image1.Source = Binarna_pragovna_segmentacija(LoadImage(), prag);
                            
                            
                            image1.InvalidateArrange();
                            image1.InvalidateMeasure();
                            
                            //MessageBox.Show("Vrnilo sliko 222");

                            if (Tokenizer.currentToken().lexeme == ";")
                            {
                                Tokenizer.nextToken();

                                return PRI();
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
                return true;
            }
            else if (Tokenizer.currentToken().type == "vara" || Tokenizer.currentToken().type == "varb")
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

                        rezultatiIzpis = izpisRezultati;

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

        /*public bool EP(ref List<double> polje)
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
        }*/

        public bool EP(ref List<double> polje)
        {
            if (Tokenizer.currentToken().type == "der" || Tokenizer.currentToken().type == "lin3" || Tokenizer.currentToken().type == "std3" || Tokenizer.currentToken().type == "pear4" || Tokenizer.currentToken().type == "avg3" || Tokenizer.currentToken().lexeme == "abs" || Tokenizer.currentToken().lexeme == "cos" || Tokenizer.currentToken().lexeme == "sin" || Tokenizer.currentToken().lexeme == "sin" || Tokenizer.currentToken().lexeme == "(" || Tokenizer.currentToken().type == "float" || Tokenizer.currentToken().type == "float3" || Tokenizer.currentToken().lexeme == "-" || Tokenizer.currentToken().type == "vara" || Tokenizer.currentToken().type == "varb")
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

        double derive(double[] polje)
        {
            if (polje.Length < 5 || polje.Length % 2 != 1) return 0;

            double[] x = new double[(polje.Length - 1) / 2];
            double[] y = new double[(polje.Length - 1) / 2];

            double xt = polje[polje.Length - 1];

            PointF[] points = new PointF[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = polje[i];
            }

            for (int i = 0; i < y.Length; i++)
            {
                y[i] = polje[i + x.Length];
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointF((float)x[i], (float)y[i]);
            }//pretvorimo polje v točke

            List<PointF> listPointsOriginal = new List<PointF>(points);

            listPointsOriginal.Sort(comparePoints);
            //sortirano po x osi

            List<PointF> listPoints = new List<PointF>();

            for (int i = 0; i < listPointsOriginal.Count - 1; i++)
            {
                PointF razlika = new PointF( (listPointsOriginal[i+1].X + listPointsOriginal[i].X)/2, listPointsOriginal[i+1].Y - listPointsOriginal[i].Y);
                listPoints.Add(razlika);
            }

            if (xt < listPoints.First().X) return 0;
            if (xt > listPoints.Last().X) return 0;
            //ni v intervalu

            int secondPointIndex = listPoints.Count; //če ni nobena večja in je v intervalu, je zadnja večja

            int firstPointIndex = listPoints.Count - 1;

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
            else if (Tokenizer.currentToken().type == "der" || Tokenizer.currentToken().type == "lin3" || Tokenizer.currentToken().type == "avg3" || Tokenizer.currentToken().type == "std3" || Tokenizer.currentToken().type == "pear4")
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

                                case "der":
                                    rezultat = derive(polje);
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
                        //MessageBox.Show("nekaj");
                        byte[] pomnilnik = new byte[(int)tok.Length];
                        tok.Read(pomnilnik, 0, (int)tok.Length);

                        char[] niz = new char[pomnilnik.Length];

                        for (int i = 0; i < pomnilnik.Length; i++)
                        {
                            niz[i] = (char)pomnilnik[i];
                        }

                        IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

                        IsolatedStorageFileStream tok2 = new IsolatedStorageFileStream("izvorna.koda", FileMode.Create, ISF);
                        //IsolatedStorageFileStream tok2 = new IsolatedStorageFileStream("tabela.podatki", FileMode.Create, ISF);
                        tok2.Write(pomnilnik, 0, pomnilnik.Length);

                        tok2.Close();
                        Pocisti(0);

                        //MessageBox.Show("nekaj");
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

                        //vredu

                        tok4.Close();
                        niz = new char[pomnilnik.Length];

                        for (int i = 0; i < pomnilnik.Length; i++)
                        {
                            niz[i] = (char)pomnilnik[i];
                        }

                        for (int i = 0; i < pomnilnik.Length; i++) vsebina_tabele += niz[i];

                        //MessageBox.Show("Test");
                        tokenizer leksikalni_analizator = new tokenizer(vsebina_tabele, vsebina_datoteke);
                        //MessageBox.Show("Test lekser");
                        //MessageBox.Show(vsebina_tabele);
                        //MessageBox.Show(vsebina_datoteke);
                        ////MessageBox.Show((leksikalni_analizator == null) + " (ali je null)");
                        //String tabela = leksikalni_analizator.vrniTabelo();
                        ////MessageBox.Show("Test tabela");
                        Tokenizer = leksikalni_analizator;
                        parse();

                        //textBox2.Text = izpis;
                        ////MessageBox.Show(izpis);
                        ////MessageBox.Show(rezultatiIzpis);

                        //Zacasni izpis tabele osnovnih leksikalnih simbolov in leksilanih vrednosti.
                        //MessageBox.Show(tabela.ToString());

                        textBox2.Text = izpis;
                    }
                }
                
                catch (Exception er)
                {
                    MessageBox.Show("Napaka pri odpiranju datoteke izvorne programske kode!");
                    MessageBox.Show(er.Message);
                }
            }

            else MessageBox.Show("Preklicano!");
        }


        Image LoadImage()
        {
            // The image will be read from isolated storage into the following byte array
            byte[] data;

            // Read the entire image in one go into a byte array
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Open the file - error handling omitted for brevity
                // Note: If the image does not exist in isolated storage the following exception will be generated:
                // System.IO.IsolatedStorage.IsolatedStorageException was unhandled
                // Message=Operation not permitted on IsolatedStorageFileStream
                using (IsolatedStorageFileStream isfs = isf.OpenFile("izvorna.slika", FileMode.Open, FileAccess.Read))
                {
                    // Allocate an array large enough for the entire file
                    data = new byte[isfs.Length];
                    // Read the entire file and then close it
                    isfs.Read(data, 0, data.Length);
                    isfs.Close();
                }
            }
            // Create memory stream and bitmap
            MemoryStream ms = new MemoryStream(data);
            WriteableBitmap bi = new WriteableBitmap(1000,1000);
            // Set bitmap source to memory stream
            bi.SetSource(ms);
            // Create an image UI element – Note: this could be declared in the XAML instead
            Image image = new Image();
            // Set size of image to bitmap size for this demonstration
            image.Height = bi.PixelHeight;
            image.Width = bi.PixelWidth;
            // Assign the bitmap image to the image’s source
            image.Source = bi;
            return image;
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

                MessageBox.Show("Uspešno zapisalo.");
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
                goto1:
                //MessageBox.Show("test in nexttoken " + (file == null));
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

                if (abeceda.IndexOf(file[pos]) == -1)
                {
                    pos++;
                    col++;
                    return nextToken();
                    goto goto1; //throw new Exception("Napaka v datoteki, znak ni del abecede: " + file[pos]);
                }

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

                List<String> končna_stanja = lines[0].Split('\t').ToList();
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

                this.pravila = pravila;
                this.abeceda = abeceda;
                this.končna = končna_stanja;
                stanja = pravila.Keys.ToList();
                stanje = "start";
                lexeme = "";
                file = vsebina;
            }


            public String vrniTabelo()
            {
                String tabela = "";
                token temp;
                int i = 0;
                do
                {
                    //MessageBox.Show("token " + i++);
                    //MessageBox.Show((this == null) + " (je this null?");
                    temp = this.nextToken();
                    //MessageBox.Show("nexttoken OK");
                    tabela += "Vrednost: " + temp.lexeme + "\tStanje: " + temp.type + "\tCol: " + temp.col + "\tRow:" + temp.row + "\tEof:" + temp.eof + "\r\n";
                    //MessageBox.Show(tabela);
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
