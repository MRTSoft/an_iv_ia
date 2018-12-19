/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2016-2017, Florin Leon                               *
 *  E-mail:      florin.leon@tuiasi.ro                                    *
 *  Website:     http://florinleon.byethost24.com/lab_ia.htm              *
 *  Description: The backpropagation algorithm for multilayer perceptrons *
 *               (Artificial Intelligence lab 13)                         *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Backpropagation
{
    public partial class MainForm : Form
    {
        private Dataset _dataset;
        private NeuralNetwork _nn;

        public MainForm()
        {
            InitializeComponent();

            // multimea de antrenare, cu 10 vectori (cifrele 0-9), 7 intrari (starea celor 7 led-uri) si 10 iesiri (1 pentru cifra formata, 0 in rest)
            _dataset = new Dataset("segments.data");

            // scalarea datelor in intervalul (0.1, 0.9), pentru a aplica sigmoida unipolara
            _dataset.ScaleData(0.1, 0.9);
        }

        #region CheckBoxes Segments

        private void checkBox0_CheckedChanged(object sender, System.EventArgs e)
        {
            checkBox0.BackColor = checkBox0.Checked ? Color.Red : SystemColors.Control;
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            checkBox1.BackColor = checkBox1.Checked ? Color.Red : SystemColors.Control;
        }

        private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
        {
            checkBox2.BackColor = checkBox2.Checked ? Color.Red : SystemColors.Control;
        }

        private void checkBox3_CheckedChanged(object sender, System.EventArgs e)
        {
            checkBox3.BackColor = checkBox3.Checked ? Color.Red : SystemColors.Control;
        }

        private void checkBox4_CheckedChanged(object sender, System.EventArgs e)
        {
            checkBox4.BackColor = checkBox4.Checked ? Color.Red : SystemColors.Control;
        }

        private void checkBox5_CheckedChanged(object sender, System.EventArgs e)
        {
            checkBox5.BackColor = checkBox5.Checked ? Color.Red : SystemColors.Control;
        }

        private void checkBox6_CheckedChanged(object sender, System.EventArgs e)
        {
            checkBox6.BackColor = checkBox6.Checked ? Color.Red : SystemColors.Control;
        }

        #endregion CheckBoxes Segments

        /// <summary>
        /// Verifica starea checkbox-urilor care simuleaza display-ul si returneaza vectorul corespunzator
        /// </summary>
        /// <returns></returns>
        private double[] GenerateTestVector()
        {
            double[] testVector = new double[_dataset.NoInputs];  // 7 led-uri

            if (checkBox0.Checked)
                testVector[0] = 1;

            if (checkBox1.Checked)
                testVector[1] = 1;

            if (checkBox2.Checked)
                testVector[2] = 1;

            if (checkBox3.Checked)
                testVector[3] = 1;

            if (checkBox4.Checked)
                testVector[4] = 1;

            if (checkBox5.Checked)
                testVector[5] = 1;

            if (checkBox6.Checked)
                testVector[6] = 1;

            return testVector;
        }

        /// <summary>
        /// Antrenarea retelei si afisarea rezultatelor
        /// </summary>
        private void buttonTrain_Click(object sender, System.EventArgs e)
        {
            CultureInfo ci = (CultureInfo)(CultureInfo.CurrentCulture.Clone());
            ci.NumberFormat.NumberDecimalSeparator = ".";

            int noEpochs = Convert.ToInt32(textBoxNoEpochs.Text);
            int noHiddenNeurons = Convert.ToInt32(textBoxNoHidden.Text);
            double learningRate = Convert.ToDouble(textBoxLearningRate.Text, ci);
            double maxError = Convert.ToDouble(textBoxMaxError.Text, ci);

            _nn = new NeuralNetwork(_dataset, noHiddenNeurons, learningRate);
            _nn.Train(noEpochs, maxError);

            richTextBoxTraining.Text = string.Format("MSE = {0:F3}\r\n\r\n", _nn.MeanSquareError);

            richTextBoxTraining.Text += "Rezultatele de dupa antrenare:\r\n\r\n";

            for (int i = 0; i < _dataset.NoVectors; i++)
            {
                string s = "";
                double[] o = _nn.Propagate(GetVectorFromDataset(i));
                for (int j = 0; j < _dataset.NoOutputs; j++)
                    s += o[j].ToString("F2") + " ";
                richTextBoxTraining.Text += s + "\r\n";
            }

            richTextBoxTraining.Focus();
        }

        private double[] GetVectorFromDataset(int i)
        {
            double[] v = new double[_dataset.NoInputs];
            for (int k = 0; k < _dataset.NoInputs; k++)
                v[k] = _dataset.Data[i, k];
            return v;
        }

        /// <summary>
        /// Folosirea retelei pentru a recunoaste o cifra formata de utilizator pe display
        /// </summary>
        private void buttonPropagate_Click(object sender, EventArgs e)
        {
            richTextBoxTesting.Clear();

            double[] testVector = GenerateTestVector();

            string s = "Vectorul semnului format:\r\n";
            for (int i = 0; i < _dataset.NoInputs; i++)
                s += string.Format("{0:F0} ", testVector[i]);
            richTextBoxTesting.Text += s + "\r\n\r\n";

            double max = double.MinValue;
            int maxIndex = -1;

            double[] o = _nn.Propagate(testVector);

            s = "Iesirea retelei:\r\n";
            for (int i = 0; i < _dataset.NoOutputs; i++)
            {
                s += string.Format("{0:F2} ", o[i]);
                if (o[i] > max)
                {
                    max = o[i];
                    maxIndex = i;
                }
            }

            richTextBoxTesting.Text += s + "\r\n\r\n";

            richTextBoxTesting.Text += string.Format("Cifra recunoscuta: {0}", maxIndex);

            richTextBoxTesting.Focus();
        }

       
    }
}