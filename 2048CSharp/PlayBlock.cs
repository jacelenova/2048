using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace _2048CSharp
{
    public class PlayBlock
    {
        private SizeF _Size;
        private PointF _Position;
        private List<MySquare> _MySquareList;


        #region "Constructors"

        public PlayBlock(PointF Position, int Width) : this(Position, Width, Width) { }

        public PlayBlock(PointF Position, int Width, int Height)
        {
            _Position = Position;
            _Size = new Size(Width, Height);
            _MySquareList = new List<MySquare>();
        }

        #endregion

        #region "Properties"

        public PointF Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public SizeF Size
        {
            get { return _Size; }
        }

        public List<MySquare> MySquareList
        {
            get { return _MySquareList; }
        }

        public MySquare MyFirstSquare
        {
            get
            {
                if (_MySquareList.Count() <= 0) { return null; }
                else { return _MySquareList[0]; }
            }
        }

        public int MySquareCount
        {
            get { return _MySquareList.Count(); }
        }

        #endregion

        public void AddSquare(ref MySquare mySquare)
        {
            _MySquareList.Add(mySquare);
        }

        public void ClearSquare()
        {
            _MySquareList.Clear();
        }

        public void RemoveSquare(MySquare mySquare)
        {
            if (_MySquareList.Contains(mySquare))
            {
                _MySquareList.Remove(mySquare);
            }
        }

        public int Combine()
        {
            int ret = 0;
            int squareCount = _MySquareList.Count;
            if (_MySquareList.Count > 1)
            {
                int num = 0, prev = 0, ttl = 0;
                bool flag = true;
                for (int i = 0; i < _MySquareList.Count; i++)
                {
                    if (i == 0) { int.TryParse(_MySquareList[0].ToString(), out prev); }
                    int.TryParse(_MySquareList[i].ToString(),out num);
                    ttl += num;

                    if (prev != num) { flag = false; }
                }

                if (flag)
                {
                    ClearSquare();
                    //MySquare.DecreaseTotalCreated(squareCount);
                    MySquare newSquare = new MySquare((int)_Size.Width, ttl.ToString(), _Position);
                    _MySquareList.Add(newSquare);
                    ret = ttl;
                }
            }

            return ret;
        }

        public int TotalSquare()
        {
            int ttl = 0;
            foreach (MySquare sq in _MySquareList)
            {
                int num = 0;
                int.TryParse(sq.ToString(), out num);
                ttl += num;
            }

            return ttl;
        }

        //public void MoveSquares()
        //{
        //    List<int> listToRemove = new List<int>();
        //    int indexCounter = 0, ttl = 0;
        //    foreach (MySquare sq in _MySquareList)
        //    {
        //        if (sq.DestinationReached)
        //        {
        //            int num;
        //            int.TryParse(sq.ToString(), out num);
        //            ttl += num;
        //            listToRemove.Add(indexCounter);
        //        }
        //        indexCounter++;
        //    }

        //    if (listToRemove.Count > 1)
        //    {
        //        foreach (int i in listToRemove)
        //        {
        //            _MySquareList.RemoveAt(i);
        //        }
        //    }

        //}

    }
}