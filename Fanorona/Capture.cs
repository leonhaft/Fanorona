using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanorona
{
    class Capture
    {
        // Stores information about a single capturing move
        // direction is if we are advancing or withdrawing
        public int srcX, srcY, destX, destY;
        public char direction;

        public Capture()
        {
            srcX = 0; srcY = 0; destX = 0; destY = 0;
            direction = 'A';
        }

        public Capture(int x1, int y1, int x2, int y2, char d)
        {
            srcX = x1;
            srcY = y1;
            destX = x2;
            destY = y2;
            direction = d;
        }
    }
}
