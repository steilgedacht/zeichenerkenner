using MathNet.Numerics.LinearAlgebra;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Zeichenerkenner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool Mousegedrückt = false; // Varialbe die beschreibt ob gerade gezeichnet werden soll oder nicht.
        Bitmap bitmap = new Bitmap(336, 336);
        Neuronales_Netzwerk NN = new Neuronales_Netzwerk(28 * 28, 100, 10, 0.1);
        double[] Pixelarray = new double[784];

        private void Form1_Load(object sender, EventArgs e) // Programm beim Start aufsetzen, eigentlich soll nur die gesamte Leinwand auf weiß gesetzt werden
        {
            CleanBox();
        }

        private void CleanBox() //Funktion die jeden Pixel auf der Zeichenfläche auf fast weiß zu setzen
        {
            for (int i = 0; i < 336; i++)
            {
                for (int j = 0; j < 336; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(254, 254, 254)); // es wird nicht komplett weiß gesetzt, da im späteren Codeverlauf sich ein Rundungsfehler bei der Durchschnittberechenung einschleichen würde.
                }
            }
            pictureBox1.Image = bitmap;
        }

        private void Cleanbutton_Click(object sender, EventArgs e) // Button der die Zeichenfläche resetet
        {
            CleanBox();
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e) // Wenn die Mouse über der Picturebox bewegt werden soll soll gezeichnet werden
        {
            if (Mousegedrückt == true)
            {
                bmpzeichnen(e.X, e.Y);
                pictureBox1.Image = bitmap;
            }
        }

        private void bmpzeichnen(int positionx, int positiony) // Die Funktion, die einen 21x21 fetten Fleck auf die Leinwand malt.
        {
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 21; j++)
                {
                    if (positionx - 10 + i > 0 && positionx - 10 + i < 336) // Die Leinwand warf nach außen nicht überschritten werden
                    {
                        if (positiony - 10 + j > 0 && positiony - 10 + j < 336)// Die Leinwand warf nach außen nicht überschritten werden
                        {
                            bitmap.SetPixel(positionx - 10 + i, positiony - 10 + j, Color.Black);
                        }
                    }
                }
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e) // Wenn die Maus gedrückt wird
        {
            Mousegedrückt = true;
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e) // Wenn die Maus wieder losgelassen wird
        {
            Mousegedrückt = false;
            Bitmapverpixeln();
            Netzabfrage();
        }

        private void Bitmapverpixeln() // Pixelalgorytmus
        {
            Bitmap Pixelbmp = new Bitmap(336, 336);

            if (checkBox1.Checked == true)
            {
                Bitmap bitmapplaceholder = new Bitmap(336, 336);
                int ptop = 0, pright = 0, pleft = 0, pbottom = 0, xverschiebung, yverschiebung; // Dieser Algorythmus ist dazu da das gezeichntete Bidl in dei Mitte zu verscheiben.
                for (int i = 0; i < (336); i++)
                {
                    for (int j = 0; j < 336; j++)
                    {
                        if (bitmap.GetPixel(i, j).R < 240 && pleft == 0)//Hier werden jeweils die Top Bottom Left und Right Werte einer gedachten Umrandung des gezeichneten Zeichens ausgelesen
                        {
                            pleft = i + 3;
                        }
                        if (bitmap.GetPixel(j, i).R < 240)
                        {
                            pright = j - 3;
                        }
                        if (bitmap.GetPixel(j, i).R < 240 && ptop == 0)
                        {
                            ptop = i + 3;
                        }
                        if (bitmap.GetPixel(j, i).R < 240)
                        {
                            pbottom = i -3;
                        }
                    }
                }
                xverschiebung = (int)((pleft + (pright - pleft) / 2) - 336 / 2); // hier wird die Entfernung die zu verschieben ist in x richtung berechnet, sodass das Gezeichnete miitttig ist.
                yverschiebung = (int)((ptop + (pbottom - ptop) / 2) - 336 / 2); // hier wird die Entfernung in y richtung die zu verschieben ist, sodass das Gezeichnete mittig ist berchnet.

                for (int i = 0; i < 336; i++) // die bitmap kann nur so kopiert werden ohne fehler zu verursachen
                {
                    for (int j = 0; j < 336; j++)
                    {
                        bitmapplaceholder.SetPixel(i, j, bitmap.GetPixel(i, j));
                    }
                }

                CleanBox();
                for (int i = 0; i < 336; i++)
                {
                    for (int j = 0; j < 336; j++)
                    {
                        if (bitmapplaceholder.GetPixel(j, i).R < 250)
                        {
                            bitmap.SetPixel(j - xverschiebung, i - yverschiebung, bitmapplaceholder.GetPixel(j, i));
                        }
                        bitmapplaceholder.SetPixel(j, i, Color.FromArgb(254, 254, 254));
                    }
                }
            }


            for (int i = 0; i < 28; i++) // zwei for schleifen, um alle 28x28 Pixel abzugehen
            {
                for (int j = 0; j < 28; j++)//Die Grundidde hinter dem durschnitt ausrechnen ist, dass für einen Bereich alle Rot Werte zusammenaddiert werden und dann  
                { // durch die Anzahl der Pixel im Bereich dividiert wird, so erhält man den Durschnittswert. Da es sich um Schwarz Weiß bilder handelt ist der Rot, Grün und Blau anteil der gleiche. So muss man nur einen Berechnen
                    int R = bitmap.GetPixel(i * 12, j * 12).R; // Anfangspixel wirt intialiesert und direkt festgelegt, sodass später mit += hinzuaddiert werden kann

                    for (int k = 0; k < 12; k++)
                    {
                        for (int l = 0; l < 12; l++)
                        {
                            R += bitmap.GetPixel(k + i * 12, l + j * 12).R; //Alle Rotwerte der 12x12 Pixel werden zusammengezählt
                        }
                    }

                    Color c = new Color(); //Farbe wird intialiesert für die Pixel des verpixelten Bild.
                    c = Color.FromArgb((int)(R / 144), (int)(R / 144), (int)(R / 144)); //Durchschnitt wird berechenet der vorhin zusammengefasstenwerte indem durch die Anzahl der Pixel 12x12 dividiert wird. Anschließend wird der Nachkommaanteil abgeblockt mit (int)

                    for (int k = 0; k < 12; k++) //Die 12x12 Pixel werden auf die neue Pixelbmp gezeichnet.
                    {
                        for (int l = 0; l < 12; l++)
                        {
                            Pixelbmp.SetPixel(k + 12 * i, l + 12 * j, c);
                        }
                    }

                    if (Math.Abs(1 - (Convert.ToDouble(R) / 144 / 255)) < 0.01) // Bei der Division kommt ein minimaler Betrag heraus bei weißem Pixel. Sollte der Auftreten, so wir er hier nullgesetzt.
                    {
                        Pixelarray[j * 28 + i] = 0;
                    }
                    else
                    {
                        Pixelarray[j * 28 + i] = Math.Abs(1 - (Convert.ToDouble(R) / 144 / 255)); // der Pixelwert (1=Schwarz, 0 = Weiß wird in eine Listenarray gespeichert)
                    }
                }
            }
            pictureBox1.Image = Pixelbmp; // Leinwand soll verpixeltes Bild annehmen
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            NN.neutrainieren();
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            double[] Ergbnis = new double[784];
            Ergbnis = NN.Dreammode((int)numericUpDown1.Value);
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    for (int k = 0; k < 12; k++)
                    {
                        for (int l = 0; l < 12; l++)
                        {
                            bitmap.SetPixel(j*12 +k, i*12+l, Color.FromArgb((int)(Ergbnis[i * 28 + j] * 255), (int)(Ergbnis[i * 28 + j] * 255), (int)(Ergbnis[i * 28 + j] * 255)));
                        }
                    }
                }
            }
            pictureBox1.Image = bitmap;
            Pixelarray  = Ergbnis;
            Netzabfrage();
        }
        private void Netzabfrage()
        {
            Vector<double> Ergebnis = NN.abfrage(Pixelarray);
            int a = 9;
            foreach (ProgressBar pgb in this.Controls.OfType<ProgressBar>()) //geht durch alle vorhandenen Progressbar durch und überträgt das Ergebnis mitder Zöhlervariable a. Die Mehtode ist etwas risky, da bei einer foreach Schleife die Reihenfogle auch falsch sein. Es wurde deswegen auch getestet, dass es richtig funktioniert. 
            {
                pgb.Value = (int)(Ergebnis[a] * 100);
                a--;
            }
            int[] ea = new int[10] { progressBar1.Value, progressBar2.Value, progressBar3.Value, progressBar4.Value, progressBar5.Value, progressBar6.Value, progressBar7.Value, progressBar8.Value, progressBar9.Value, progressBar10.Value };
            label1.Text = Convert.ToString(ea.ToList().IndexOf(ea.Max()));
        }
    }
}
