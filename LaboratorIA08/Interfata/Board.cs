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
using System.Linq;

namespace SimpleCheckers
{
    /// <summary>
    /// Reprezinta o configuratie a jocului (o tabla de joc) la un moment dat
    /// </summary>
    public class Board
    {
        public int Size { get; set; } // dimensiunea tablei de joc
        public List<Piece> Pieces { get; set; } // lista de piese, atat ale omului cat si ale calculatorului

        public Board()
        {
            Size = 4;
            Pieces = new List<Piece>(Size * 2);

            for (int i = 0; i < Size; i++)
                Pieces.Add(new Piece(i, Size - 1, i, PlayerType.Computer));

            for (int i = 0; i < Size; i++)
                Pieces.Add(new Piece(i, 0, i + Size, PlayerType.Human));
        }

        public Board(Board b)
        {
            Size = b.Size;
            Pieces = new List<Piece>(Size * 2);

            foreach (Piece p in b.Pieces)
                Pieces.Add(new Piece(p.X, p.Y, p.Id, p.Player));
        }

        /// <summary>
        /// Calculeaza functia de evaluare statica pentru configuratia (tabla) curenta
        /// </summary>
        public double EvaluationFunction()
        {
            double fitness = 12.0d;
            foreach(Piece p in this.Pieces)
            {
                if (p.Player == PlayerType.Computer)
                {
                    fitness -= p.Y;
                }else
                {
                    fitness -= p.Y;
                }
            }
            return fitness;
        }

        /// <summary>
        /// Creaza o noua configuratie aplicand mutarea primita ca parametru in configuratia curenta
        /// </summary>
        public Board MakeMove(Move move)
        {
            Board nextBoard = new Board(this); // copy
            nextBoard.Pieces[move.PieceId].X = move.NewX;
            nextBoard.Pieces[move.PieceId].Y = move.NewY;
            return nextBoard;
        }

        /// <summary>
        /// Verifica daca configuratia curenta este castigatoare
        /// </summary>
        /// <param name="finished">Este true daca cineva a castigat si false altfel</param>
        /// <param name="winner">Cine a castigat: omul sau calculatorul</param>
        public void CheckFinish(out bool finished, out PlayerType winner)
        {
            if (Pieces.Where(p => p.Player == PlayerType.Human && p.Y == Size - 1).Count() == Size)
            {
                finished = true;
                winner = PlayerType.Human;
                return;
            }

            if (Pieces.Where(p => p.Player == PlayerType.Computer && p.Y == 0).Count() == Size)
            {
                finished = true;
                winner = PlayerType.Computer;
                return;
            }

            finished = false;
            winner = PlayerType.None;
        }
    }
}