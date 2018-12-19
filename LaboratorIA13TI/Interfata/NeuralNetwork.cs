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

namespace Backpropagation
{
    public class NeuralNetwork
    {
        private Dataset _dataset;                // multimea de antrenare

        private int _noInputs, _noHidden, _noOutputs; // numarul de intrari, numarul de neuroni din stratul ascuns, numarul de iesiri

        private double[,] _weightsInputHidden;  // ponderile dintre stratul de intrare si stratul ascuns
        private double[,] _weightsHiddenOutput; // ponderile dintre stratul ascuns si stratul de iesire

        private double[] _activationXInput;     // gradul de activare al neuronilor din stratul de intrare (care doar copiaza intrarile din vectorul de antrenare)
        private double[] _activationYHidden;    // gradul de activare al neuronilor din stratul ascuns
        private double[] _activationYOutput;    // gradul de activare al neuronilor din stratul de iesire

        private double[] _errorsOutput;         // vectorul de diferente dintre iesirile dorite si iesirile date de retea

        private double[] _gradientsHidden;      // gradientii corespunzatori stratului ascuns
        private double[] _gradientsOutput;      // gradientii corespunzatori stratului de iesire
        private double[,] _dwInputHidden;       // corectiile ponderilor dintre stratul de intrare si stratul ascuns
        private double[,] _dwHiddenOutput;      // corectiile ponderilor dintre stratului ascuns si stratul de iesire

        private double _learningRate;           // rata de invatare

        public int Epoch { get; set; }          // numarul epocii de antrenare

        public double MeanSquareError { get; set; } // eroarea medie patratica din epoca de antrenare curenta

        // corespondenta intre notatia de mai sus si notatia din laborator:
        // _weightsInputHidden - w_ij
        // _weightsHiddenOutput - w_jk
        // _activationXInput - x_i
        // _activationYHidden - y_j
        // _activationYOutput - y_k
        // _errorsOutput - e_k
        // _gradientsHidden - delta_j
        // _gradientsOutput - delta_k
        // _dwInputHidden - delta-w_ij
        // _dwHiddenOutput - delta-w_jk
        // _learningRate - alfa
        // stratul de intrare are indicele i, stratul ascuns are indicele j, stratul de iesire are indicele k

        private Random _rand;

        public NeuralNetwork(Dataset dataset, int hidden, double learningRate)
        {
            _dataset = dataset;

            _noInputs = dataset.NoInputs;
            _noHidden = hidden;
            _noOutputs = dataset.NoOutputs;

            _learningRate = learningRate;

            _weightsInputHidden = new double[_noInputs + 1, _noHidden];     // + 1 pentru ca include "bias"-ul (pragul)
            _weightsHiddenOutput = new double[_noHidden + 1, _noOutputs];   // + 1 pentru ca include "bias"-ul (pragul)

            _activationXInput = new double[_noInputs + 1];
            _activationYHidden = new double[_noHidden + 1];
            _activationYOutput = new double[_noOutputs];
            _errorsOutput = new double[_noOutputs];

            _gradientsHidden = new double[_noHidden];
            _gradientsOutput = new double[_noOutputs];

            _dwInputHidden = new double[_noInputs + 1, _noHidden];
            _dwHiddenOutput = new double[_noHidden + 1, _noOutputs];

            // initializarea retelei

            _rand = new Random();

            for (int i = 0; i < _noInputs + 1; i++)
                for (int j = 0; j < _noHidden; j++)
                    _weightsInputHidden[i, j] = GenerateSmallNumber();

            for (int i = 0; i < _noHidden + 1; i++)
                for (int j = 0; j < _noOutputs; j++)
                    _weightsHiddenOutput[i, j] = GenerateSmallNumber();

            _activationXInput[_noInputs] = 1;   // pentru bias (prag)
            _activationYHidden[_noHidden] = 1;  // pentru bias (prag)
        }

        /// <summary>
        /// Genereaza un numar aleatoriu din intervalul [-0.1, 0.1) fara 0
        /// </summary>
        /// <returns></returns>
        private double GenerateSmallNumber()
        {
            double n = 0;

            while (Math.Abs(n) < 1e-8)
                n = _rand.NextDouble() / 5.0 - 0.1; // [-0.1, 0.1) - {0}

            return n;
        }

        /// <summary>
        /// Metoda care calculeaza iesirile retelei pentru un vector primit ca parametru
        /// </summary>
        public double[] Propagate(double[] vector)
        {
            double[] y = new double[_noOutputs];
            double[] v = new double[_noInputs + _noOutputs];

            for (int k = 0; k < _noInputs; k++)
                v[k] = vector[k];

            ForwardPass(v);

            for (int k = 0; k < _noOutputs; k++)
                y[k] = _activationYOutput[k];

            return y;
        }

        /// <summary>
        /// Metoda de antrenare a retelei folosind algoritmul backpropagation. Antrenarea se realizeaza
        /// pana la atingerea unui numar maxim de epoci sau pana la atingerea unei erori acceptabile.
        /// </summary>
        public void Train(int maxEpochs, double maxError)
        {
            // cata vreme Epoch < maxEpochs si MeanSquareError > maxError, apeleaza TrainOneEpoch
            //throw new Exception("Aceasta metoda trebuie implementata.");
            this.Epoch = 0;
            while (Epoch < maxEpochs && MeanSquareError > maxError)
            {
                this.MeanSquareError = TrainOneEpoch();
                Epoch++;
            }
        }

        /// <summary>
        /// Implementarea unei epoci de antrenare pentru un vector primit ca parametru.
        /// Returneaza eroarea medie patratica pentru acest vector in configuratia curenta a retelei.
        /// </summary>
        private double TrainOneEpoch()
        {
            ResetGradients();

            double mse = 0;

            for (int v = 0; v < _dataset.NoVectors; v++)
            {
                double[] vector = new double[_noInputs + _noOutputs];

                for (int i = 0; i < _noInputs + _noOutputs; i++)
                    vector[i] = _dataset.Data[v, i];

                ForwardPass(vector);

                BackwardPass(vector);

                for (int k = 0; k < _noOutputs; k++)
                    mse += _errorsOutput[k] * _errorsOutput[k];
            }

            UpdateWeights();

            mse /= (double)_dataset.NoVectors;
            return mse;
        }

        /// <summary>
        /// Resetarea variabilelor temporare ale algoritmului
        /// </summary>
        private void ResetGradients()
        {
            for (int j = 0; j < _noHidden; j++)
                _gradientsHidden[j] = 0;

            for (int k = 0; k < _noOutputs; k++)
            {
                _gradientsOutput[k] = 0;
                _errorsOutput[k] = 0;
            }

            for (int i = 0; i < _noInputs + 1; i++)
                for (int j = 0; j < _noHidden; j++)
                    _dwInputHidden[i, j] = 0;

            for (int j = 0; j < _noHidden + 1; j++)
                for (int k = 0; k < _noOutputs; k++)
                    _dwHiddenOutput[j, k] = 0;
        }

        /// <summary>
        /// Actualizarea ponderilor pe baza corectiilor ponderilor
        /// </summary>
        private void UpdateWeights()
        {
            // se actualizeaza _weightsInputHidden si _weightsHiddenOutput
            // nu uitati pragurile!

            throw new NotImplementedException("Aceasta metoda trebuie implementata.");
        }

        /// <summary>
        /// Pasul de propagare inainte al algoritmului
        /// </summary>
        private void ForwardPass(double[] vector)
        {
            for (int i = 0; i < _noInputs; i++)
                _activationXInput[i] = vector[i];

            
            // calculare _activationYHidden[j]
            for (int i = 0; i < _noHidden; i++)
            {
                double sum = 0.0;
                for(int j=0; j<_noInputs; ++j)
                {
                    sum += _activationXInput[j] * _weightsInputHidden[i,j];
                }
                _activationYHidden[i] = SigmoidFunction(sum);
            }

            // calculare _activationYOutput[k]
            for(int k = 0; k < _noOutputs; ++k)
            {
                double sum = 0.0;
                for(int j=0; j < _noHidden; ++j)
                {
                    sum += _activationYHidden[j] * _weightsHiddenOutput[j, k];
                }
                _activationYOutput[k] = SigmoidFunction(sum);
            }
            
	    // calculare _errorsOutput[k]
	    // eroarea este diferenta dintre iesirea dorita si iesirea retelei
	    // parametrul "vector" al metodei reprezinta o linie din multimea de antrenare:
	    // primele 7 componente (_noInputs = 7) sunt intrarile,
	    // iar ultimele 10 componente (_noOutputs = 10) sunt iesirile dorite

            for(int k=0; k<_noOutputs; ++k)
            {
                _errorsOutput[k] = 
            }

            //throw new NotImplementedException("Aceasta metoda trebuie implementata.");
        }

        /// <summary>
        /// Pasul de propagare inapoi al algoritmului
        /// </summary>
        private void BackwardPass(double[] vector)
        {
            // vector este compus din intrari si iesirile dorite (o linie din fisierul "segments.data")

            // calculare _gradientsOutput[k] si _dwHiddenOutput[j, k]
            // calculare _gradientsHidden[j] si _dwInputHidden[i, j]

            throw new NotImplementedException("Aceasta metoda trebuie implementata.");
        }

        /// <summary>
        /// Functia de activare sigmoida unipolara
        /// </summary>
        private double SigmoidFunction(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        /// <summary>
        /// Derivata functiei sigmoide unipolare
        /// </summary>
        private double SigmoidDerivative(double f)
        {
            // f este valoarea functiei, adica se da f(x) si trebuie calculat f'(x), fara sa ne intereseze sa il aflam explicit pe x
            throw new NotImplementedException("Aceasta metoda trebuie implementata.");
        }
    }
}