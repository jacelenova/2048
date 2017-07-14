using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048CSharp
{
    public enum States
    {
        STOPPED, MOVING
    };

    public enum Directions
    {
        NONE, UP, DOWN, LEFT, RIGHT
    };

    public enum PlayStates
    {
        NONE, PLAYING, PAUSED, GAMEOVER, WIN
    };

    public enum FlagsCheck : short
    {
        NONE = 0,
        ANIMATE = 1,
        ANIMATING = 2,
        MOVE = 4,
        MOVING = 8,
    };
}
