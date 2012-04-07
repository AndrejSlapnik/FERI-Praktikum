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

                    return new token(oldCol, oldRow, pos == file.Length, oldLexeme, oldStanje);
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
    }
}
