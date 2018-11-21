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
    public enum PlayerType { None, Computer, Human };

    /// <summary>
    /// Reprezinta o piesa de joc
    /// </summary>
    public class Piece
    {
        public int Id { get; set; } // identificatorul piesei
        public int X { get; set; } // pozitia X pe tabla de joc
        public int Y { get; set; } // pozitia Y pe tabla de joc
        public PlayerType Player { get; set; } // carui tip de jucator apartine piesa (om sau calculator)

        public Piece(int x, int y, int id, PlayerType player)
        {
            X = x;
            Y = y;
            Id = id;
            Player = player;
        }

        /// <summary>
        /// Returneaza lista tuturor mutarilor permise pentru piesa curenta (this) 
        /// in configuratia (tabla de joc) primita ca parametru
        /// </summary>
        public List<Move> ValidMoves(Board currentBoard)
        {
            List<Move> availableMoves = new List<Move>();
            for(int dy = -1; dy < 2; dy++)
            {
                for(int dx = -1; dx < 2; dx++)
                {
                    Move permited = new Move(this.Id, this.X + dx, this.Y + dy);
                    if (IsValidMove(currentBoard, permited) == true)
                    {
                        availableMoves.Add(permited);
                    }
                }
            }
            return availableMoves;
        }

        /// <summary>
        /// Testeaza daca o mutare este valida intr-o anumita configuratie
        /// </summary>
        public bool IsValidMove(Board currentBoard, Move move)
        {
            bool isInBounds = true;
            bool doesNotOverlap = true;
            bool isValidPiece = false; // piesa apartine tablei
            bool doesNotOverstep = true;

            foreach (Piece p in currentBoard.Pieces)
            {
                if (p.Id == move.PieceId)
                {
                    isValidPiece = true;
                    if (Math.Abs(p.X - move.NewX) > 1.0d ||
                        Math.Abs(p.Y - move.NewY) > 1.0d)
                    {
                        doesNotOverstep = false;
                    }

                }
                if (p.X == move.NewX && p.Y == move.NewY)
                {
                    doesNotOverlap = false;
                }
            }
            if (move.NewX < 0 || move.NewY < 0 || 
                move.NewX >= currentBoard.Size ||
                move.NewY >= currentBoard.Size)
            {
                isInBounds = false;
            }
            return isInBounds & doesNotOverlap & isValidPiece & doesNotOverstep;
        }
    }
}