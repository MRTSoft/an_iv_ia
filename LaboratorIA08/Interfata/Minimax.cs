/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2016-2017, Florin Leon                               *
 *  E-mail:      florin.leon@tuiasi.ro                                    *
 *  Website:     http://florinleon.byethost24.com/lab_ia.htm              *
 *  Description: Game playing. Minimax algorithm                          *
 *               (Artificial Intelligence lab 8)                          *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;

namespace SimpleCheckers
{
    /// <summary>
    /// Implementeaza algoritmul de cautare a mutarii optime
    /// </summary>
    public class Minimax
    {
        private static Random _rand = new Random();

        /// <summary>
        /// Primeste o configuratie ca parametru, cauta mutarea optima si returneaza configuratia
        /// care rezulta prin aplicarea acestei mutari optime
        /// </summary>
        public static Board FindNextBoard(Board currentBoard)
        {
            Move bestMove = null;
            double bestFitness = double.MinValue;
            foreach(Piece p in currentBoard.Pieces)
            {
                if (p.Player == PlayerType.Computer)
                {
                    List<Move> availableForThisPiece = p.ValidMoves(currentBoard);
                    foreach(Move m in availableForThisPiece)
                    {
                        Board clone = currentBoard.MakeMove(m);
                        double fitness = clone.EvaluationFunction();
                        if (fitness > bestFitness)
                        {
                            bestMove = m;
                            bestFitness = fitness;
                        }
                    }
                }
            }
            return currentBoard.MakeMove(bestMove);
        }
    }
}