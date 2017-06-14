using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanorona
{
    class Board
    {
        public const int BLANK = 0;
        public const int BLACK = 1;
        public const int WHITE = 2;
        public int[,] board = new int[9,5];
        // Wow, a 4D array!  Really?
        private static byte[, , ,] connected = new byte[9, 5, 9, 5];  // Stores if (x1,y1) connected to (x2,y2)
                                                // Not very space efficient, need to store both
                                                // directions, but faster than a graph and only one instance since
                                                // it's static
        private static bool builtGraph = false;

        public Board()
        {
            // Build connection graph
            if (!builtGraph)
            {
                buildConnectionGraph();
                builtGraph = true;
            }

            // Initial game board
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 2; y++)
                    board[x, y] = BLACK;
                for (int y = 3; y < 5; y++)
                    board[x, y] = WHITE;
            }
            board[0, 2] = BLACK;
            board[1, 2] = WHITE;
            board[2, 2] = BLACK;
            board[3, 2] = WHITE;
            board[4, 2] = BLANK;
            board[5, 2] = BLACK;
            board[6, 2] = WHITE;
            board[7, 2] = BLACK;
            board[8, 2] = WHITE;
        }

        public Board(Board otherboard)
        {
            // Build connection graph
            if (!builtGraph)
            {
                buildConnectionGraph();
                builtGraph = true;
            }

            for (int x=0; x<9; x++)
                for (int y = 0; y < 5; y++)
                {
                    board[x, y] = otherboard.board[x, y];
                }
        }

        // Build our 4D array that holds the graph of what nodes are connected to
        // what other nodes
        public static void buildConnectionGraph()
        {
            for (int x1 = 0; x1 < 9; x1++)
                for (int y1 = 0; y1 < 5; y1++)
                    for (int x2 = 0; x2 < 9; x2++)
                        for (int y2 = 0; y2 < 5; y2++)
                            connected[x1, y1, x2, y2] = 0;

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 4; y++)
                {
                    if ((((x % 2) == 0) && ((y % 2) == 0))
                        ||
                         (((x % 2) == 1) && ((y % 2) == 1)))
                    {
                        connected[x,y,x+1,y+1] = 1;
                        connected[x+1,y+1,x,y] = 1;
                    }
                    else if ((((x % 2) == 0) && ((y % 2) == 1))
                            ||
                             (((x % 2) == 1) && ((y % 2) == 0)))
                    {
                        connected[x, y + 1, x + 1, y] = 1;
                        connected[x+1, y, x, y+1] = 1;
                    }
                    connected[x, y, x + 1, y] = 1;
                    connected[x+1, y, x, y ] = 1;
                    connected[x, y, x, y + 1] = 1;
                    connected[x, y + 1, x, y] = 1;
                    connected[x, y+1, x + 1, y + 1] = 1;
                    connected[x+1,y+1,x,y+1] = 1;
                    connected[x+1, y+1, x+1, y] = 1;
                    connected[x+1,y,x+1,y+1] = 1;
                }
        }

        // Requires connection graph to be built;
        // returns whether or not two coordinates are connected
        public static bool isConnected(int x1, int y1, int x2, int y2)
        {
            if (x1 < 0 || x1 > 8) return false;
            if (x2 < 0 || x2 > 8) return false;
            if (y1 < 0 || y1 > 4) return false;
            if (y2 < 0 || y2 > 4) return false;
            return (connected[x1, y1, x2, y2] == 1);
        }

        // Returns BLACK if black has won, WHITE if white has won,
        // and BLANK if nobody has won
        public int gameOver()
        {
            int whitecount = 0;
            int blackcount = 0;
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 5; y++)
                {
                    if (board[x, y] == BLACK)
                        blackcount++;
                    else if (board[x, y] == WHITE)
                        whitecount++;
                }
            if ((blackcount == 0) && (whitecount > 0))
                return WHITE;
            else if ((whitecount == 0) && (blackcount > 0))
                return BLACK;
            return BLANK;
        }

        // Returns a list of valid moves for the specified color
        public List<Move> MoveList(int whoseturn)
        {
            List<Move> moves = new List<Move>();
            if (!CapturePossible(whoseturn))
            {
                // Generate single moves; check all directions
                for (int x = 0; x < 9; x++)
                    for (int y = 0; y < 5; y++)
                    {
                        if (board[x, y] == whoseturn)
                        {
                            if ((isConnected(x, y, x - 1, y - 1)) && (board[x - 1, y - 1] == BLANK))
                                moves.Add(new Move(x, y, x - 1, y - 1));
                            if ((isConnected(x, y, x, y - 1)) && (board[x, y - 1] == BLANK))
                                moves.Add(new Move(x, y, x, y - 1));
                            if ((isConnected(x, y, x + 1, y - 1)) && (board[x + 1, y - 1] == BLANK))
                                moves.Add(new Move(x, y, x + 1, y - 1));
                            if ((isConnected(x, y, x - 1, y)) && (board[x - 1, y] == BLANK))
                                moves.Add(new Move(x, y, x - 1, y));
                            if ((isConnected(x, y, x + 1, y)) && (board[x + 1, y] == BLANK))
                                moves.Add(new Move(x, y, x + 1, y));
                            if ((isConnected(x, y, x -1, y + 1)) && (board[x - 1, y + 1] == BLANK))
                                moves.Add(new Move(x, y, x - 1, y + 1));
                            if ((isConnected(x, y, x, y + 1)) && (board[x, y + 1] == BLANK))
                                moves.Add(new Move(x, y, x, y + 1));
                            if ((isConnected(x, y, x + 1, y + 1)) && (board[x+1, y + 1] == BLANK))
                                moves.Add(new Move(x, y, x+1, y + 1));
                        }
                    }
            }
            else
            {
                CaptureMoveList(whoseturn, moves);
            }
            return moves;
        }

        // Creates a list of just capture moves for the specified color.  
        // This is the top level for making recursive calls to construct moves from
        // each piece that can potentially capture to a blank space
        public void CaptureMoveList(int whoseturn, List<Move> moves)
        {
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 5; y++)
                {
                    if (board[x, y] == whoseturn)
                    {
                        if ((isConnected(x, y, x - 1, y - 1)) && (board[x - 1, y - 1] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x - 1, y - 1, moves, new Move());
                        if ((isConnected(x, y, x, y - 1)) && (board[x, y - 1] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x, y - 1, moves, new Move());
                        if ((isConnected(x, y, x + 1, y - 1)) && (board[x + 1, y - 1] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x + 1, y - 1, moves, new Move());
                        if ((isConnected(x, y, x - 1, y)) && (board[x - 1, y] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x - 1, y, moves, new Move());
                        if ((isConnected(x, y, x + 1, y)) && (board[x + 1, y] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x + 1, y, moves, new Move());
                        if ((isConnected(x, y, x - 1, y + 1)) && (board[x - 1, y + 1] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x - 1, y + 1, moves, new Move());
                        if ((isConnected(x, y, x, y + 1)) && (board[x, y + 1] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x, y + 1, moves, new Move());
                        if ((isConnected(x, y, x + 1, y + 1)) && (board[x + 1, y + 1] == BLANK))
                            AddMoveIfValid(whoseturn, x, y, x + 1, y + 1, moves, new Move());
                    }
                }
        }

        // Adds a move to the move list if it's valid and tries to add more
        public void AddMoveIfValid(int whoseturn, int x, int y, int x2, int y2, List<Move> moves, Move curMove)
        {
            // Remember original move for withdrawing
            Move origMove = new Move(curMove);
            // Try advancing capture
            Move testMove = new Move(curMove);                        
            testMove.AddCapture(x, y, x2, y2, 'A');
            if (ValidMove(testMove, whoseturn))
            {
                curMove.AddCapture(x, y, x2, y2, 'A');
                moves.Add(new Move(curMove));
                Board tempBoard = new Board(this);
                tempBoard.MakeMove(curMove);                
                // Recurse directions from x2,y2 to adjacent blank on tempBoard
                if ((isConnected(x2, y2, x2 - 1, y2 - 1)) && (tempBoard.board[x2 - 1, y2 - 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 - 1, y2 - 1, moves, new Move(curMove));
                if ((isConnected(x2, y2, x2, y2 - 1)) && (tempBoard.board[x2, y2 - 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2, y2 - 1, moves, new Move(curMove));
                if ((isConnected(x2, y2, x2 + 1, y2 - 1)) && (tempBoard.board[x2 + 1, y2 - 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 + 1, y2 - 1, moves, new Move(curMove));
                if ((isConnected(x2, y2, x2 - 1, y2)) && (tempBoard.board[x2 - 1, y2] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 - 1, y2, moves, new Move(curMove));
                if ((isConnected(x2, y2, x2 + 1, y2)) && (tempBoard.board[x2 + 1, y2] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 + 1, y2, moves, new Move(curMove));
                if ((isConnected(x2, y2, x2 - 1, y2 + 1)) && (tempBoard.board[x2 - 1, y2 + 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 - 1, y2 + 1, moves, new Move(curMove));
                if ((isConnected(x2, y2, x2, y2 + 1)) && (tempBoard.board[x2, y2 + 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2, y2 + 1, moves, new Move(curMove));
                if ((isConnected(x2, y2, x2 + 1, y2 + 1)) && (tempBoard.board[x2 + 1, y2 + 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 + 1, y2 + 1, moves, new Move(curMove));
            }
            // Try withdrawal capture
            testMove = new Move(origMove);
            testMove.AddCapture(x, y, x2, y2, 'W');
            if (ValidMove(testMove, whoseturn))
            {
                origMove.AddCapture(x, y, x2, y2, 'W');
                moves.Add(new Move(origMove));
                Board tempBoard = new Board(this);
                tempBoard.MakeMove(origMove);
                // Recurse directions from x2,y2 to adjacent blank on tempBoard
                if ((isConnected(x2, y2, x2 - 1, y2 - 1)) && (tempBoard.board[x2 - 1, y2 - 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 - 1, y2 - 1, moves, new Move(origMove));
                if ((isConnected(x2, y2, x2, y2 - 1)) && (tempBoard.board[x2, y2 - 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2, y2 - 1, moves, new Move(origMove));
                if ((isConnected(x2, y2, x2 + 1, y2 - 1)) && (tempBoard.board[x2 + 1, y2 - 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 + 1, y2 - 1, moves, new Move(origMove));
                if ((isConnected(x2, y2, x2 - 1, y2)) && (tempBoard.board[x2 - 1, y2] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 - 1, y2, moves, new Move(origMove));
                if ((isConnected(x2, y2, x2 + 1, y2)) && (tempBoard.board[x2 + 1, y2] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 + 1, y2, moves, new Move(origMove));
                if ((isConnected(x2, y2, x2 - 1, y2 + 1)) && (tempBoard.board[x2 - 1, y2 + 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 - 1, y2 + 1, moves, new Move(origMove));
                if ((isConnected(x2, y2, x2, y2 + 1)) && (tempBoard.board[x2, y2 + 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2, y2 + 1, moves, new Move(origMove));
                if ((isConnected(x2, y2, x2 + 1, y2 + 1)) && (tempBoard.board[x2 + 1, y2 + 1] == BLANK))
                    AddMoveIfValid(whoseturn, x2, y2, x2 + 1, y2 + 1, moves, new Move(origMove));
            }
        }

        // Returns true if the move is valid, false if it is invalid
        public bool ValidMove(Move m, int whoseturn)
        {
            // Check for basic move
            if (m.movetype == Move.BASIC_MOVE)
            {
                // Try to move a blank
                int color = board[m.srcX, m.srcY];
                if (color == BLANK) return false;

                 // Try to move opponent's piece
                if (whoseturn != color) return false;

                // Not connected
                if (!isConnected(m.srcX, m.srcY, m.destX, m.destY))
                    return false;

                // Have to confirm a capturing move is not possible
                if (CapturePossible(color))
                    return false;
                if (board[m.destX, m.destY] != BLANK)
                    return false;

                return true;
            }
            else
            {
                // Ensure coordinates are unique
                if (!uniqueCoordinates(m.captures))
                    return false;

                // Moving same piece
                if (!sameCapturingPiece(m.captures))
                    return false;

                // Try to capture in same direction
                if (captureSameDirection(m.captures))
                    return false;

                // Check capturing moves on temp board
                Board testBoard = new Board(this);
                foreach (Capture c in m.captures)
                {
                    int x1 = c.srcX;
                    int y1 = c.srcY;
                    int x2 = c.destX;
                    int y2 = c.destY;

                    // Pieces are not connected
                    if (!isConnected(x1, y1, x2, y2))
                    {
                        return false;
                    }

                    // Not moving our piece
                    int piececolor = testBoard.board[x1,y1];
                    if (piececolor != whoseturn) return false;

                    // Destination not blank
                    if (testBoard.board[x2, y2] != BLANK)
                        return false;

                    // Check if opponent's color adjacent in direction we are trying to capture
                    int opponentColor = OpponentColor(piececolor);
                    int deltax, deltay;
                    int zapX, zapY;
                    if (c.direction == 'A')
                    {
                        deltax = x2 - x1;
                        deltay = y2 - y1;
                        zapX = x2 + deltax;
                        zapY = y2 + deltay;
                    }
                    else
                    {
                        deltax = x1 - x2;
                        deltay = y1 - y2;
                        zapX = x1 + deltax;
                        zapY = y1 + deltay;
                    }
                    if ((zapX < 0) || (zapX > 8) || (zapY < 0) || (zapY > 4)
                             || (testBoard.board[zapX, zapY] != opponentColor))
                    {
                        return false;
                    }

                    // Make the move on this board so we can test subsequent moves
                    testBoard.MakeCapture(c);

                    // END CAPTURE CHECK
                }
            }
            return true;   // Made it through, so it's valid (captures)
        }

        // Returns true if the sequence of capturing moves doesn't visit the
        // same place 
        private bool uniqueCoordinates(List<Capture> captures)
        {
            for (int i = 0; i < captures.Count; i++)
            {
                int x1 = captures[i].srcX;
                int y1 = captures[i].srcY;
                // x1,x2 shouldn't appear as a destination in any later sequence
                for (int j = i + 1; j < captures.Count; j++)
                {
                    int x2 = captures[j].destX;
                    int y2 = captures[j].destY;
                    if ((x1 == x2) && (y1 == y2))
                        return false;
                }
            }
            return true;
        }

        // Make sure we are moving the same piece (redundant info,
        // change the move format?)
        private bool sameCapturingPiece(List<Capture> captures)
        {
            if (captures.Count < 2)
                return true;

            for (int i = 0; i < captures.Count - 1; i++)
            {
                int x1 = captures[i].destX;
                int y1 = captures[i].destY;
                int x2 = captures[i + 1].srcX;
                int y2 = captures[i + 1].srcY;
                if ((x1 != x2) || (y1 != y2)) return false;
            }
            return true;
        }

        // Returns true if we try to make two captures in the same direction
        private bool captureSameDirection(List<Capture> captures)
        {
            if (captures.Count < 2)
                return false;

            int deltax = captures[0].srcX - captures[0].destX;
            int deltay = captures[0].srcY - captures[0].destY;
            for (int i = 1; i < captures.Count; i++)
            {
                int deltax2 = captures[i].srcX - captures[i].destX;
                int deltay2 = captures[i].srcY - captures[i].destY;
                if ((deltax2 == deltax) && (deltay2 == deltay))
                    return true;
                deltax = deltax2;
                deltay = deltay2;
            }
            return false;
        }

        // Returns true if a capturing move exists for the given color
        public bool CapturePossible(int color)
        {
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 5; y++)
                {
                    if (board[x, y] == color)
                    {
                        if (CaptureTest(color, x, y))
                            return true;
                    }
                }
            return false;
        }

        // Tests capturing with the given color in some direction
        private bool CaptureTest(int color, int x1, int y1)
        {
            int opponent = OpponentColor(color);
            // Check up
            if ((y1 > 0) && (board[x1, y1 - 1] == BLANK) && (isConnected(x1,y1,x1,y1-1)))
            {
                if ((y1-2 >= 0) && (board[x1, y1 - 2] == opponent))
                    return true;
                if ((y1 + 1 <= 4) && (board[x1, y1 + 1] == opponent))
                    return true;                
            }
            // Check down
            if ((y1 < 4) && (board[x1, y1 + 1] == BLANK) && (isConnected(x1, y1, x1, y1 + 1)))
            {
                if ((y1+2 <= 4) && (board[x1, y1 + 2] == opponent))
                    return true;
                if ((y1 - 1 >= 0) && (board[x1, y1 - 1] == opponent))
                    return true;
            }
            // Check left
            if ((x1 > 0) && (board[x1 - 1, y1] == BLANK) && (isConnected(x1, y1, x1 -1, y1)))
            {
                if ((x1-2 >= 0) && (board[x1 - 2, y1] == opponent))
                    return true;
                if ((x1 + 1 <= 8) && (board[x1 + 1, y1] == opponent))
                    return true;
            }
            // Check right
            if ((x1 < 8) && (board[x1 + 1, y1] == BLANK) && (isConnected(x1, y1, x1 + 1, y1)))
            {
                if ((x1+2 <= 8) && (board[x1 + 2, y1] == opponent))
                    return true;
                if ((x1 - 1 >= 0) && (board[x1 - 1, y1] == opponent))
                    return true;
            }
            // Check upper left 
            if ((x1 > 0) && (y1 > 0) && (board[x1 - 1, y1 - 1] == BLANK) && (isConnected(x1, y1, x1 - 1, y1 - 1)))
            {
                if ((x1-2 >= 0) && (y1-2 >=0) && (board[x1 - 2, y1 - 2] == opponent))
                    return true;
                if ((x1 + 1 <= 8) && (y1 + 1 <= 4) && (board[x1 + 1, y1 + 1] == opponent))
                    return true;
            }
            // Check upper right 
            if ((x1 < 8) && (y1 > 0) && (board[x1 + 1, y1 - 1] == BLANK) && (isConnected(x1, y1, x1 + 1, y1 - 1)))
            {
                if ((x1+2<=8) && (y1-2 >= 0) && (board[x1 + 2, y1 - 2] == opponent))
                    return true;
                if ((x1 - 1 >= 0) && (y1 + 1 <= 4) && (board[x1 - 1, y1 + 1] == opponent))
                    return true;
            }
            // Check lower left 
            if ((x1 > 0) && (y1 < 4) && (board[x1 - 1, y1 + 1] == BLANK) && (isConnected(x1, y1, x1 - 1, y1 + 1)))
            {
                if ((x1-2>=0) && (y1+2<=4) && (board[x1 - 2, y1 + 2] == opponent))
                    return true;
                if ((x1 + 1 <= 8) && (y1 - 1 >= 0) && (board[x1 + 1, y1 - 1] == opponent))
                    return true;
            }
            // Check lower right 
            if ((x1 < 8) && (y1 < 4) && (board[x1 + 1, y1 + 1] == BLANK) && (isConnected(x1, y1, x1 + 1, y1 + 1)))
            {
                if ((x1+2<=8) && (y1+2<=4) && (board[x1 + 2, y1 + 2] == opponent))
                    return true;
                if ((x1 - 1 >= 0) && (y1 - 1 >= 0) && (board[x1 - 1, y1 - 1] == opponent))
                    return true;
            }
            return false;
        }

        // Opponent's color
        public int OpponentColor(int color)
        {
            if (color == BLACK) return WHITE;
            return BLACK;
        }

        // Makes this move on the board.  Assumes the move is legal.
        public void MakeMove(Move m)
        {
            if (m.movetype == Move.BASIC_MOVE)
            {
                int color = board[m.srcX, m.srcY];
                board[m.destX, m.destY] = color;
                board[m.srcX, m.srcY] = BLANK;
            }
            else
            {
                foreach (Capture c in m.captures)
                {
                    MakeCapture(c);
                }
            }
        }

        // Apply a single capturing move
        private void MakeCapture(Capture c)
        {
            int x1, y1, x2, y2;
            char direction;
            direction = c.direction;
            x1 = c.srcX;
            y1 = c.srcY;
            x2 = c.destX;
            y2 = c.destY;

            if (!isConnected(x1, y1, x2, y2))
            {
                //todo Update tip
                //Console.WriteLine("Error, invalid move in MakeCapture.  Ignoring.");
                return;
            }

            int color = board[x1, y1];
            int opponentColor;
            if (color == BLACK)
                opponentColor = WHITE;
            else
                opponentColor = BLACK;

            int deltax, deltay;
            int zapX, zapY;
            if (direction == 'A')
            {
                deltax = x2 - x1;
                deltay = y2 - y1;
                zapX = x2 + deltax;
                zapY = y2 + deltay;
            }
            else
            {
                deltax = x1 - x2;
                deltay = y1 - y2;
                zapX = x1 + deltax;
                zapY = y1 + deltay;
            }

            // Remove opponent pieces until we hit the edge, blank, or our piece
            // Don't have to worry about connectedness, will always be connected
            // if we were able to move in this direction
            while ((zapX >= 0) && (zapX < 9) && (zapY >= 0) && (zapY < 5)
                     && (board[zapX, zapY] == opponentColor))
            {
                board[zapX, zapY] = BLANK;
                zapX += deltax;
                zapY += deltay;
            }

            // Move actual piece
            board[x1, y1] = BLANK;
            board[x2, y2] = color;
        }
    }
}
