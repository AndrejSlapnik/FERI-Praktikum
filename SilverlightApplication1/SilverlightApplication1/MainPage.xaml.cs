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

namespace SilverlightApplication1
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog nekaj = new OpenFileDialog();

            bool? nekaj2 = nekaj.ShowDialog();
            if (nekaj2 == true)
            {
                
                using (FileStream fs = nekaj.File.OpenRead())
                {
                    byte[] buffer = new byte[(int)fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                    char[] niz = new char[buffer.Length];

                    for (int i = 0; i < buffer.Length; i++)
                        niz[i] = (char)buffer[i];

                    IsolatedStorageFile isf =
    IsolatedStorageFile.GetUserStoreForApplication();
                    IsolatedStorageFileStream tok = new IsolatedStorageFileStream("test.txt", FileMode.Create, isf);
                    tok.Write(buffer, 0, buffer.Length);

                    tok.Close();
                }
            }
            else MessageBox.Show("Preklicano");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog nekaj = new OpenFileDialog();

            bool? nekaj2 = nekaj.ShowDialog();
            if (nekaj2 == true)
            {

                using (FileStream fs = nekaj.File.OpenRead())
                {
                    byte[] buffer = new byte[(int)fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);

                    IsolatedStorageFile isf =
    IsolatedStorageFile.GetUserStoreForApplication();
                    IsolatedStorageFileStream tok = new IsolatedStorageFileStream("test.jpg", FileMode.Create, isf);
                    tok.Write(buffer, 0, buffer.Length);

                    tok.Close();
                }
            }
            else MessageBox.Show("Preklicano");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageFile isf =
    IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream tok = new IsolatedStorageFileStream("test2.txt", FileMode.Create, isf);

            String koda = textBox1.Text;

            byte[] buffer = new byte[koda.Length];

            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)koda[i];

            tok.Write(buffer, 0, buffer.Length);

            tok.Close();
        }
    }
}
