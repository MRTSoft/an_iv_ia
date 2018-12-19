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
using System.Globalization;
using System.IO;

namespace Backpropagation
{
    /// <summary>
    /// Clasa corespunzatoare unei multimi de date de antrenare
    /// </summary>
    public class Dataset
    {
        public int NoInputs { get; set; }
        public int NoOutputs { get; set; }
        public int NoVectors { get; set; }

        /// <summary>
        /// Datele propriu-zise memorate in forma unei matrice, in care numarul de linii este numarul de vectori de antrenare,
        /// iar numarul de coloane este suma dintre numarul de intrari si numarul de iesiri.
        /// De exemplu, pentru problema binara XOR, multimea de antrenare are 4 vectori, 2 intrari si 1 iesire si este de forma:
        /// 0 0 0 / 0 1 1 / 1 0 1 / 0 0 0
        /// </summary>
        public double[,] Data { get; set; }

        /// <summary>
        /// Multimea de antrenare este citita dintr-un fisier
        /// </summary>
        public Dataset(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);

            string[] toks = sr.ReadLine().Split(" \t\r\n,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            NoInputs = Convert.ToInt32(toks[0]);
            NoOutputs = Convert.ToInt32(toks[1]);
            NoVectors = Convert.ToInt32(toks[2]);

            Data = new double[NoVectors, NoInputs + NoOutputs];

            CultureInfo ci = (CultureInfo)(CultureInfo.CurrentCulture.Clone());
            ci.NumberFormat.NumberDecimalSeparator = ".";

            for (int i = 0; i < NoVectors; i++)
            {
                toks = sr.ReadLine().Split(" \t\r\n,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < toks.Length; j++)
                    Data[i, j] = Convert.ToDouble(toks[j], ci);
            }

            sr.Close();
        }

        /// <summary>
        /// Pentru a putea fi prelucrate de reteaua neuronala, datele trebuie scalate, de exemplu in intervalul 0.1 - 0.9
        /// </summary>
        public void ScaleData(double minValue, double maxValue)
        {
            double minData = double.MaxValue;
            double maxData = double.MinValue;

            for (int i = 0; i < NoVectors; i++)
                for (int j = 0; j < NoInputs + NoOutputs; j++)
                {
                    if (Data[i, j] < minData)
                        minData = Data[i, j];
                    if (Data[i, j] > maxData)
                        maxData = Data[i, j];
                }

            for (int i = 0; i < NoVectors; i++)
                for (int j = 0; j < NoInputs + NoOutputs; j++)
                    Data[i, j] = (Data[i, j] - minData) / (maxData - minData) * (maxValue - minValue) + minValue;
        }
    }
}