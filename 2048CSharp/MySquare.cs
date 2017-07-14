using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace _2048CSharp
{
    public class MySquare
    {
        private int _Width, _Number;
        private string _Text;
        private Color _Color;
        private PointF _Position, _PrevPosition, _Destination;
        private double _Speed, _SpeedX, _SpeedY;
        private RectangleF _Bounds;
        private bool _DestinationReached;
        private int _MovesMadeX, _MovesToReachX, _MovesMadeY, _MovesToReachY;

        private Directions _Direction;
        private SquareStates _SquareState;

        //para sa animation
        private bool _Animate;
        private double _AnimateMax, _AnimateSpeed, _AnimationWidth;
        private PointF _AnimationPosition;
        private AnimationStates _AnimationState;

        private static int _TotalSquareScore, _TotalSquareCreated, _HighestSquareCreated;

        #region "Constructors"

        public MySquare(int Width) : this(Width, string.Empty) { }

        public MySquare(int Width, string Text) : this(Width, Text, new PointF(0, 0)) { }

        public MySquare(int Width, string Text, PointF Position, bool Animate = true, bool AddTotalSquare = true) {
            this._Width = Width;
            this._Text = Text;
            this._Position = Position;
            this._PrevPosition = Position;
            this._Speed = 0;
            this._SpeedX = 0;
            this._SpeedY = 0;
            this._Direction = Directions.None;

            this._Destination = PointF.Empty;
            this._DestinationReached = true;
            this._Color = AssignColor(Text);

            int.TryParse(Text, out _Number);
            _TotalSquareScore += _Number;
            if (AddTotalSquare) { _TotalSquareCreated += 1; }
            if (_Number > _HighestSquareCreated) { _HighestSquareCreated = _Number; }

            if (Animate) 
            {
                this._Animate = true;
                this._AnimateMax = 10;
                this._AnimateSpeed = 2;
                this._AnimationWidth = _Width;
                this._AnimationPosition = _Position;
                this._AnimationState = AnimationStates.Enlarge;
            }

        }

        #endregion

        #region "Enums"

        public enum Directions
        {
            None,
            Up,
            Down,
            Left,
            Right
        };

        public enum SquareStates
        {
            None,
            Animating,
            Moving,
            Stopped
        };

        private enum AnimationStates
        {
            None,
            Enlarge,
            Shrink
        };

        #endregion

        #region "Properties"

        public int Width 
        { 
            get { return _Width; } 
            set { _Width = value; } 
        }

        public Color Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        public PointF Position
        {
            get { return _Position; }
            set 
            {
                _PrevPosition = _Position;
                _Position = value;
            }
        }

        public float Top
        {
            get { return _Position.Y; }
            set
            {
                _PrevPosition = _Position;
                _Position = new PointF(_Position.X, value);
            }
        }

        public float Left
        {
            get { return _Position.X; }
            set
            {
                _PrevPosition = _Position;
                _Position = new PointF(value, _Position.Y);
            }
        }

        public double Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        public double SpeedX
        {
            get { return _SpeedX; }
            set
            {
                _SpeedX = value;
                if (value != 0) { _MovesToReachX = (int)((Math.Abs(_Destination.X) - Math.Abs(_Position.X)) / _SpeedX); }
            }
        }

        public double SpeedY
        {
            get { return _SpeedY; }
            set
            {
                _SpeedY = value;
                if (value != 0) { _MovesToReachY = (int)((Math.Abs(_Destination.Y) - Math.Abs(_Position.Y)) / _SpeedY); }
            }
        }

        public RectangleF Bounds
        {
            get { return _Bounds; }
            set { _Bounds = value; }
        }

        public PointF Destination
        {
            get { return _Destination; }
            set
            {
                if (_Position != value)
                {
                    _Destination = value;
                    _DestinationReached = false;
                }
                else 
                {
                    _DestinationReached = true;
                    _Destination = PointF.Empty;
                    _SquareState = SquareStates.Stopped;
                }
            }
        }

        public static int TotalSquareCreated
        {
            get { return _TotalSquareCreated; }
            set { _TotalSquareCreated = value; }
        }

        public static int TotalSquareScore
        {
            get { return _TotalSquareScore; }
            set { _TotalSquareScore = value; }
        }

        public static int HighestSquareCreated
        {
            get { return _HighestSquareCreated; }
            set { _HighestSquareCreated = value; }
        }

        #endregion

        #region "ReadOnly Properties"

        public PointF PrevPosition
        {
            get { return _PrevPosition; }
        }

        public float Right
        {
            get { return _Position.X + _Width; }
        }

        public float Bottom
        {
            get { return _Position.Y + _Width; }
        }

        public Directions Direction
        {
            get { return _Direction; }
        }

        public bool DestinationReached
        {
            get { return _DestinationReached; }
        }

        public SquareStates SquareState
        {
            get { return _SquareState; }
        }

        public bool Animate
        {
            get { return _Animate; }
        }

        #endregion


        #region "Methods"

        public void Draw(Graphics g)
        {
            try
            {
                using(SolidBrush br = new SolidBrush(_Color))
                {
                    SizeF circSize = new SizeF(20, 20);
                    PointF pos;
                    float width;

                    if (_AnimationState != AnimationStates.None)
                    {
                        pos = _AnimationPosition;
                        width = (float)_AnimationWidth;
                    }
                    else
                    {
                        pos = _Position;
                        width = _Width;
                    }

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    //Square sa gitna
                    g.FillRectangle(br, new RectangleF(new PointF(pos.X + 5, pos.Y + 5), new SizeF(width - 10, width - 10)));

                    g.FillRectangle(br, new RectangleF(new PointF(pos.X, pos.Y + 10), new SizeF(10, width - 20)));
                    g.FillRectangle(br, new RectangleF(new PointF(pos.X + width - 10, pos.Y + 10), new SizeF(10, width - 20)));
                    g.FillRectangle(br, new RectangleF(new PointF(pos.X + 10, pos.Y), new SizeF(width - 20, 10)));
                    g.FillRectangle(br, new RectangleF(new PointF(pos.X + 10, pos.Y + width - 10), new SizeF(width - 20, 10)));

                    //mga circle sa kanto
                    g.FillEllipse(br, new RectangleF(pos, circSize));
                    g.FillEllipse(br, new RectangleF(new PointF(pos.X + width - 20, pos.Y), circSize));
                    g.FillEllipse(br, new RectangleF(new PointF(pos.X, pos.Y + width - 20), circSize));
                    g.FillEllipse(br, new RectangleF(new PointF(pos.X + width - 20, pos.Y + width - 20), circSize));
                }

                using (SolidBrush brString = new SolidBrush(Color.Black))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    g.DrawString(_Text, new Font("Tahoma", 14, FontStyle.Bold), brString, new RectangleF(Position, new SizeF(_Width, _Width)), sf);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void StopAnimation()
        {
            _Animate = false;
            _AnimationState = AnimationStates.None;
            _SquareState = SquareStates.Stopped;
        }

        public static void DecreaseTotalCreated(int Number = 1)
        {
            _TotalSquareCreated = _TotalSquareCreated - Number;
        }

        public static void AddTotalCreated(int Number = 1)
        {
            _TotalSquareCreated = _TotalSquareCreated + 1;
        }

        public static void ResetScores()
        {
            _TotalSquareCreated = 0;
            _TotalSquareScore = 0;
        }

        #endregion

        #region "Functions"

        private Color AssignColor(string Text)
        {
            Color col;
            int num;

            int.TryParse(Text, out num);

            if (num == 2) { col = Color.FromArgb(245, 245, 245); }
            else if (num == 4) { col = Color.FromArgb(255, 255, 204); }
            else if (num == 8) { col = Color.FromArgb(255, 153, 0); }
            else if (num == 16) { col = Color.FromArgb(255, 153, 102); }
            else if (num == 32) { col = Color.FromArgb(255, 51, 51); }
            else if (num == 64) { col = Color.FromArgb(255, 0, 0); }
            else if (num == 128) { col = Color.FromArgb(255, 255, 0); }
            else if (num == 256) { col = Color.FromArgb(255, 240, 240); }
            else if (num == 512) { col = Color.FromArgb(255, 200, 190); }
            else if (num == 1024) { col = Color.FromArgb(123, 255, 204); }
            else if (num == 2048) { col = Color.FromArgb(210, 100, 199); }
            else { col = Color.White; }

            return col;
        }

        #endregion

        #region "Move"

        public void Move()
        {
            //if (_Animate)
            //{
            //    AnimateSquare();
            //    return;
            //}

            if (_Destination == PointF.Empty) { return; }

            _PrevPosition = _Position;
            _SquareState = SquareStates.Moving;

            if (_Destination.X - _Position.X > 0 && _Destination.X - _Position.X + _SpeedX < 0)
            {
                _Position.X = _Destination.X;
            }
            else if (_Destination.X - _Position.X < 0 && _Destination.X - _Position.X + _SpeedX > 0)
            {
                _Position.X = _Destination.X;
            }
            else if (_Destination.X == _Position.X) { }
            else { _Position.X = _Position.X + (float)_SpeedX; }

            if (_Destination.Y - _Position.Y > 0 && _Destination.Y - _Position.Y + _SpeedY < 0)
            {
                _Position.Y = _Destination.Y;
            }
            else if (_Destination.Y - _Position.Y < 0 && _Destination.Y - _Position.Y + _SpeedY > 0)
            {
                _Position.Y = _Destination.Y;
            }
            else if (_Destination.Y == _Position.Y) { }
            else { _Position.Y = _Position.Y + (float)_SpeedY; }

            if (_Position == _Destination)
            {
                _DestinationReached = true;
                _Destination = PointF.Empty;
                _SquareState = SquareStates.Stopped;
            }
        }

        public void AnimateSquare()
        {
            if (!_Animate) { return; }

            _SquareState = SquareStates.Animating;

            if (_AnimationState == AnimationStates.Enlarge)
            {
                _AnimationPosition.X = _AnimationPosition.X - (float)_AnimateSpeed;
                _AnimationPosition.Y = _AnimationPosition.Y - (float)_AnimateSpeed;

                _AnimationWidth = _AnimationWidth + (_AnimateSpeed * 2);

                if (_AnimationPosition.X <= _Position.X - _AnimateMax || _AnimationPosition.Y <= _Position.Y - _AnimateMax)
                {
                    _AnimationPosition.X = _Position.X - (float)_AnimateMax;
                    _AnimationPosition.Y = _Position.Y - (float)_AnimateMax;

                    _AnimationState = AnimationStates.Shrink;
                }
            }
            else if (_AnimationState == AnimationStates.Shrink)
            {
                _AnimationPosition.X = _AnimationPosition.X + (float)_AnimateSpeed;
                _AnimationPosition.Y = _AnimationPosition.Y + (float)_AnimateSpeed;

                _AnimationWidth = _AnimationWidth - (_AnimateSpeed * 2);

                if (_AnimationPosition.X >= _Position.X || _AnimationPosition.Y >= _Position.Y)
                {
                    _AnimationPosition.X = _Position.X;
                    _AnimationPosition.Y = _Position.Y;
                    _AnimationWidth = _Width;

                    _AnimationState = AnimationStates.None;
                    _Animate = false;

                    _SquareState = SquareStates.Stopped;
                }
            }
        }

        public void GoUp() { _Direction = Directions.Up; }

        public void GoDown() { _Direction = Directions.Down; }

        public void GoLeft() { _Direction = Directions.Left; }

        public void GoRight() { _Direction = Directions.Right; }

        #endregion

        public override string ToString()
        {
            return _Text;
        }
    }
}
