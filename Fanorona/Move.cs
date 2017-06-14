using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanorona
{
    class Move
    {
        // Should refactor with inheritance to remove type
        public const int CAPTURE_MOVE = 1;
        public const int BASIC_MOVE = 0;

        // For basic move
        public int srcX =0, srcY =0, destX =0, destY =0;

        // For capture move it's a list of captures
        public List<Capture> captures = new List<Capture>();

        // Indicates if this move is a capturing move or a basic move
        public int movetype;

        public Move()
        {
            movetype = CAPTURE_MOVE;
        }

        public Move(int x1, int y1, int x2, int y2)  // Basic Move
        {
            srcX = x1;
            srcY = y1;
            destX = x2;
            destY = y2;
            movetype = BASIC_MOVE;
        }

        public Move(Move otherMove)
        {
            movetype = otherMove.movetype;
            if (movetype == CAPTURE_MOVE)
            {
                foreach (Capture c in otherMove.captures)
                {
                    captures.Add(new Capture(c.srcX, c.srcY, c.destX, c.destY, c.direction));
                }
            }
            else
            {
                srcX = otherMove.srcX;
                srcY = otherMove.srcY;
                destX = otherMove.destX;
                destY = otherMove.destY;
            }
        }

        // Add a capture to the move list
        public void AddCapture(int x1, int y1, int x2, int y2, char dir)
        {
            Capture c = new Capture(x1, y1, x2, y2, dir);
            captures.Add(c);
        }

        // Get the move as a string
        public override string ToString()
        {
            string s = "";
            if (movetype == BASIC_MOVE)
            {
                s = "M" + srcX.ToString() + (4 - srcY).ToString() + destX.ToString() + (4 - destY).ToString();
            }
            else
            {
                foreach (Capture c in captures)
                {
                    if (c.direction == 'A')
                        s += "C";
                    else
                        s += "W";
                    s += c.srcX.ToString() + (4 - c.srcY).ToString() +
                         c.destX.ToString() + (4 - c.destY).ToString() + " ";
                }
                s = s.Trim();
            }
            return s;
        }
    }
}
