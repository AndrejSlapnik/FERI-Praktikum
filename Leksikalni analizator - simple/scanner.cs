using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Leksikalni_analizator
{
    class tokenizer
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
    }
}
