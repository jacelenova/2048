using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace _2048CSharp
{
    public class PlayPanel : Panel
    {
        private PlayBlock[,] _PlayBlockArray;
        private PointF _Position;
        private int _XCount, _YCount, _MySquareCount;
        private int _BlockPadding, _PlayBlockWidth, _FPS;

        private Stopwatch _Stopwatch;
        private TimeSpan _StartTick;
        
        private LocalDB _DB;
        private ScoreDT _ScoreDT;

        private Thread _MainThread;

        private PlayStates _PlayState;
        private States _State;
        private Directions _Direction;
        
        private bool _SetDestinations, _NewGame, _CreateNewSquare;
        private Queue<Directions> _MoveQueue;
        private Random _Random;

        private ScorePanel _PausePanel;

        public delegate void PlayState_Changed_Del(PlayPanel sender, PlayStates playstate);
        public event PlayState_Changed_Del PlayState_Changed;

        private delegate void _SetPlayState_Del(PlayStates PlayState);

        private bool _Saved;

        #region "Constructors"

        public PlayPanel(int X) : this(X, X) { }

        public PlayPanel(int X, int Y)
        {
            DoubleBuffered = true;
            _XCount = X;
            _YCount = Y;
            _MySquareCount = X * Y;
            _BlockPadding = 5;
            _PlayBlockWidth = 100;
            _FPS = 60;

            _SetDestinations = false;
            _NewGame = true;

            _Direction = Directions.NONE;
            _PlayState = PlayStates.NONE;
            _State = States.STOPPED;

            _Stopwatch = new Stopwatch();
            _MoveQueue = new Queue<Directions>();
            _PlayBlockArray = new PlayBlock[X, Y];

            AddPlayBlocks();

            int ttlWidth = (((_BlockPadding * 2) + _PlayBlockWidth) * X) + _BlockPadding;
            int ttlHeight = (((_BlockPadding * 2) + _PlayBlockWidth) * Y) + _BlockPadding;
            Width = ttlWidth;
            Height = ttlHeight; //ttlWidth;
            Size = new Size(Width, Height);
            MinimumSize = Size;
            BackColor = Color.FromArgb(102, 102, 102);

            _MainThread = new Thread(GameLoop);
            _Random = new Random();

            _DB = new LocalDB();
            _ScoreDT = new ScoreDT();

            InitializeComponents();
        }

        #endregion

        #region "Properties"

        public Directions Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public PlayStates PlayState
        {
            get { return _PlayState; }
        }

        public int BlockPadding
        {
            get { return _BlockPadding; }
        }



        #endregion

        public void InitializeComponents()
        {
            //PausePanel
            _PausePanel = new ScorePanel();
            _PausePanel.Width = this.Width - (this.Width / 4);
            _PausePanel.Height = this.Height / 4;
            //_PausePanel.Width = Width / 8 * 7;
            //_PausePanel.Height = Height / 8 * 7;
            _PausePanel.Location = new Point((Width - _PausePanel.Width) / 2, (Height - _PausePanel.Height) / 2);
            _PausePanel.BackColor = Color.FromArgb(50, Color.Aquamarine);
            _PausePanel.Font = new Font("Verdana", 30, FontStyle.Bold);
            _PausePanel.Text = "PAUSED";
            _PausePanel.Visible = false;

            _PausePanel.Click += new EventHandler(_PausePanel_Click);
            this.Controls.Add(_PausePanel);
        }

        public void GameLoop()
        {
            _Stopwatch.Start();
            do
            {
                _StartTick = _Stopwatch.Elapsed;
                this.Invalidate();

                if (_NewGame)
                {
                    SetPlayState(PlayStates.PLAYING);
                }
                else if (_PlayState == PlayStates.GAMEOVER) 
                {
                    if (_Saved == false)
                    {
                        _ScoreDT.AddRow("Test", MySquare.TotalSquareScore, MySquare.TotalSquareCreated); ;
                        _Saved = true;
                    }
                }
                else if (_PlayState == PlayStates.PAUSED) { }
                else if (_PlayState == PlayStates.WIN) 
                {
                    if (_Saved == false) 
                    {
                        _ScoreDT.AddRow("Test", MySquare.TotalSquareScore, MySquare.TotalSquareCreated); ;
                        _Saved = true;
                    }
                    
                }
                else
                {
                    FlagsCheck checkMoving = CheckMovingSquare();

                    if (checkMoving == FlagsCheck.NONE)
                    {
                        //Pag Destination Reached na ilipat ng PlayBlock ang mySquare
                        ChangeSquareBlock();
                        //Combine squares na nasa isang PlayBlock
                        CombineSquares();

                        if (_CreateNewSquare)
                        {
                            if (NewSquare())
                            {
                                _CreateNewSquare = false;
                                if (CheckGameOver())
                                {
                                    SetPlayState(PlayStates.GAMEOVER);
                                    continue;
                                }
                                checkMoving = checkMoving | FlagsCheck.ANIMATE;
                            }
                        }

                        if (_SetDestinations && _MoveQueue.Count > 0)
                        {
                            _Direction = _MoveQueue.Dequeue();
                            if (SetDestination()) { _CreateNewSquare = true; }
                            if (_MoveQueue.Count <= 0) { _SetDestinations = false; }
                        }

                    }

                    if ((checkMoving & FlagsCheck.ANIMATE) == FlagsCheck.ANIMATE | (checkMoving & FlagsCheck.ANIMATING) == FlagsCheck.ANIMATING)
                    {
                        AnimateSquare();
                    }
                    else if ((checkMoving & FlagsCheck.MOVE) == FlagsCheck.MOVE | (checkMoving & FlagsCheck.MOVING) == FlagsCheck.MOVING)
                    {
                        MoveSquares2();
                    }
                }

                do
                {

                } while (_Stopwatch.Elapsed.TotalMilliseconds - _StartTick.TotalMilliseconds < 8.33);

            } while (this.Created);
        }

        public void SetPlayState(PlayStates PlayState)
        {
            if (InvokeRequired)
            {
                Invoke(new _SetPlayState_Del(SetPlayState), PlayState);
            }
            else
            {
                bool changed = false;

                if (PlayState == PlayStates.PAUSED)
                {
                    if (_PlayState == PlayStates.PLAYING)
                    {
                        _PlayState = PlayStates.PAUSED;

                        _PausePanel.Text = "PAUSED";
                        _PausePanel.Visible = true;
                        changed = true;
                    }
                }
                else if (PlayState == PlayStates.PLAYING)
                {
                    if (_PlayState == PlayStates.PAUSED || _PlayState == PlayStates.WIN)
                    {
                        _PlayState = PlayStates.PLAYING;
                        _PausePanel.Visible = false;
                        changed = true;
                    }
                    else
                    {
                        _PlayState = PlayStates.PLAYING;
                        _CreateNewSquare = false;
                        _NewGame = false;

                        NewGame();
                        //CreateTest();
                        _PausePanel.Visible = false;
                        changed = true;
                    }

                }
                else if (PlayState == PlayStates.GAMEOVER)
                {
                    if (_PlayState == PlayStates.PLAYING)
                    {
                        _PlayState = PlayStates.GAMEOVER;

                        _PausePanel.Text = "GAME OVER";
                        _PausePanel.Visible = true;
                        changed = true;
                    }

                }
                else if (PlayState == PlayStates.WIN)
                {
                    if (_PlayState == PlayStates.PLAYING)
                    {
                        _PlayState = PlayStates.WIN;

                        _PausePanel.Text = "WIN";
                        _PausePanel.Visible = true;
                        changed = true;
                    }
                }
                else { changed = false; }

                OnPlayStateChanged(changed);
            }

        }

        private void NewGame()
        {
            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    _PlayBlockArray[i, j].ClearSquare();
                }
            }

            MySquare.ResetScores();
            _MoveQueue.Clear();

            NewSquare();
            NewSquare();

            int x = 0;
            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    if (_PlayBlockArray[i, j].MySquareCount > 0)
                    {
                        x = x + _PlayBlockArray[i, j].MySquareCount;

                        foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                        {
                            //Debug.WriteLine("Square : ");
                            //Debug.WriteLine(sq.ToString());
                            //Debug.WriteLine(sq.Position);
                        }
                    }
                }
            }

            _Saved = false;
        }

        private void MoveSquares()
        {
            List<MySquare> ListToRemove = new List<MySquare>();

            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                    {
                        sq.Move();
                        //Kunin mySquare na nakarating na sa Destination para ilipat
                        if (sq.DestinationReached)
                        { 
                            ListToRemove.Add(sq);
                        }
                    }


                    if (_PlayBlockArray[i, j].MySquareList.Count > 5) { Debug.WriteLine(" Test " + _PlayBlockArray[i, j].MySquareList.Count()); }

                }
            }

            //Ilipat sa bagong PlayBlock nakuhang mySquare
            //foreach (MySquare sq in ListToRemove)  //ayaw mapasa as ref pag galing sa for each
            for (int z = 0; z < ListToRemove.Count(); z++)
            {
                MySquare sq = ListToRemove[z];
                bool found = false;

                for (int i = 0; i < _XCount; i++)
                {
                    for (int j = 0; j < _YCount; j++)
                    {
                        foreach (MySquare sqInner in _PlayBlockArray[i, j].MySquareList)
                        {
                            if (sq.Equals(sqInner))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found == true) { _PlayBlockArray[i, j].RemoveSquare(sq); }
                    }

                    if (found) { break; }
                }

                found = false;
                for (int i = 0; i < _XCount; i++)
                {
                    for (int j = 0; j < _YCount; j++)
                    {
                        if (_PlayBlockArray[i, j].Position == sq.Position)
                        {
                            found = true;
                            _PlayBlockArray[i, j].AddSquare(ref sq);
                        }
                    }
                }
            }

            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    if (_PlayBlockArray[i, j].MySquareList.Count > 1)
                    {
                        int ttl = 0;
                        int sqNum = 0;
                        List<MySquare> toRemove = new List<MySquare>();
                        foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                        {
                            if (sq.DestinationReached)
                            {
                                toRemove.Add(sq);
                            }
                        }

                        if (toRemove.Count > 0)
                        {
                            for (int k = 0; k < toRemove.Count; k++)
                            {
                                MySquare sq = toRemove[k];
                                int.TryParse(sq.ToString(), out sqNum);
                                ttl = ttl + sqNum;
                                _PlayBlockArray[i, j].RemoveSquare(sq);
                                //MySquare.DecreaseTotalCreated();
                            }

                            MySquare newSquare = new MySquare(_PlayBlockWidth, ttl.ToString(), _PlayBlockArray[i, j].Position);
                            _PlayBlockArray[i, j].AddSquare(ref newSquare);

                            if (ttl == 2048) { SetPlayState(PlayStates.WIN); }
                        }
                    }
                }
            }
        }

        private void MoveSquares2()
        {
            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                    {
                        if (!sq.DestinationReached) { sq.Move(); }
                    }
                }
            }
        }

        private void AnimateSquare()
        {
            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                    {
                        if (sq.Animate) { sq.AnimateSquare(); }
                    }
                }
            }
        }

        private void ChangeSquareBlock()
        {
            List<MySquare> ListToRemove = new List<MySquare>();
            List<int> ListX = new List<int>();
            List<int> ListY = new List<int>();

            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                    {
                        if (sq.DestinationReached)
                        {
                            ListToRemove.Add(sq);
                            ListX.Add(i);
                            ListY.Add(j);
                        }
                    }
                }
            }

            for (int z = 0; z < ListToRemove.Count; z++)
            {
                MySquare sq = ListToRemove[z];
                for (int i = 0; i < _XCount; i++)
                {
                    for (int j = 0; j < _YCount; j++)
                    {
                        if (sq.Position == _PlayBlockArray[i, j].Position)
                        {
                            _PlayBlockArray[i, j].AddSquare(ref sq);
                            _PlayBlockArray[ListX[z], ListY[z]].RemoveSquare(sq);
                        }
                    }
                }
            }
        }

        private void CombineSquares()
        {
            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j < _YCount; j++)
                {
                    if (_PlayBlockArray[i, j].Combine() == 2048)
                    {
                        SetPlayState(PlayStates.WIN);
                    }
                }
            }
        }

        public void Restart()
        {
            _NewGame = true;
        }

        public void Start()
        {
            _MainThread.Start();
        }

        //Not Used
        private void CreateTest()
        {
            for (int i = 0; i < _XCount; i++)
            {
                for (int j = 0; j > _YCount; j++)
                {
                    _PlayBlockArray[i, j].ClearSquare();

                }
            }

        }

        private void AddPlayBlocks()
        {
            int currX = 0 + (_BlockPadding / 2);
            int currY = 0;

            for (int i = 0; i < _XCount; i++)
            {
                currX = currX + _BlockPadding;
                currY = 0 + (_BlockPadding / 2);

                for (int j = 0; j < _YCount; j++)
                {
                    currY = currY + _BlockPadding;
                    _PlayBlockArray[i, j] = new PlayBlock(new Point(currX, currY), _PlayBlockWidth);
                    currY = currY + _PlayBlockWidth + _BlockPadding;
                }

                currX = currX + _PlayBlockWidth + _BlockPadding;
            }
        }

        public void AddMove(Directions Direction)
        {
            if (_PlayState == PlayStates.PLAYING)
            {
                _MoveQueue.Enqueue(Direction);
                _SetDestinations = true;
            }
        }

        #region "Functions"

        private bool NewSquare()
        {
            int XrndNumber = 0, YrndNumber = 0;
            bool Flag = false;

            bool[] XrndArray = new bool[_XCount];

            for (; ; )
            {
                XrndNumber = _Random.Next(_XCount);

                if (XrndArray.Contains(false) == false) { return false; }

                if (XrndArray[XrndNumber]) { continue; }
                else { XrndArray[XrndNumber] = true; }

                for (int j = 0; j < _XCount; j++)
                {
                    if (_PlayBlockArray[XrndNumber, j].MySquareCount <= 0)
                    {
                        Flag = true;
                        break;
                    }
                }

                if (Flag) { break; }
            }

            Flag = false;
            for (; ; )
            {
                YrndNumber = _Random.Next(_YCount);
                if (_PlayBlockArray[XrndNumber, YrndNumber].MySquareCount <= 0)
                {
                    Flag = true;
                    break;
                }
            }

            //Para mas mataas chance na 2 ang lumabas
            int sqText = _Random.Next(4) + 1;

            if (sqText > 3) { sqText = 4; }
            else { sqText = 2; }

            MySquare newSq = new MySquare(_PlayBlockWidth, sqText.ToString(), _PlayBlockArray[XrndNumber, YrndNumber].Position);
            _PlayBlockArray[XrndNumber, YrndNumber].AddSquare(ref newSq);

            return true;
        }
        
        //Medyo magulo baguhin pa
        private bool SetDestination()
        {
            bool Flag = false;
            int forStep = 0, forFrom = 0, forTo = 0, moves = 15, x = 0;

            if (_Direction == Directions.DOWN)
            {
                forFrom = _YCount - 1;
                x = _XCount - 1;
                forTo = 0;
                forStep = -1;
            }
            else if (_Direction == Directions.RIGHT)
            {
                forFrom = _XCount - 1;
                x = _YCount - 1;
                forTo = 0;
                forStep = -1;
            }
            else if (_Direction == Directions.UP)
            {
                forFrom = 0;
                x = _XCount - 1;
                forTo = _YCount - 1;
                forStep = 1;
            }
            else if (_Direction == Directions.LEFT)
            {
                forFrom = 0;
                x = _YCount - 1;
                forTo = _XCount - 1;
                forStep = 1;
            }

            if (_Direction == Directions.UP || _Direction == Directions.DOWN)
            {
                if (_Direction == Directions.UP)
                {
                    //for (int i = forFrom; i <= forTo; i += forStep)
                    for (int i = forFrom; i <= x; i += forStep)
                    {
                        int pos = forFrom;
                        string prev = "";

                        for (int j = forFrom; j <= forTo; j += forStep)
                        {
                            foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                            {
                                if (prev == "")
                                {
                                    sq.Destination = new PointF(sq.Position.X, _PlayBlockArray[i, pos].Position.Y);
                                    prev = sq.ToString();
                                }
                                else if (prev == sq.ToString())
                                {
                                    sq.Destination = new PointF(sq.Position.X, _PlayBlockArray[i, pos].Position.Y);
                                    prev = "";
                                    pos = pos + forStep;
                                }
                                else
                                {
                                    pos = pos + forStep;
                                    sq.Destination = new PointF(sq.Position.X, _PlayBlockArray[i, pos].Position.Y);
                                    prev = sq.ToString();
                                }

                                if (sq.Destination != PointF.Empty && sq.Destination != sq.Position)
                                {
                                    Flag = true;
                                }

                                sq.SpeedX = ComputeSpeedX(sq.Position, sq.Destination, moves);
                                sq.SpeedY = ComputeSpeedY(sq.Position, sq.Destination, moves);
                            }
                        }
                    }
                }
                else if (_Direction == Directions.DOWN)
                {
                    //for (int i = forFrom; i >= forTo; i += forStep)
                    for (int i = x; i >= forTo; i += forStep)
                    {
                        int pos = forFrom;
                        string prev = "";

                        for (int j = forFrom; j >= forTo; j += forStep)
                        {
                            foreach (MySquare sq in _PlayBlockArray[i, j].MySquareList)
                            {
                                if (prev == "")
                                {
                                    sq.Destination = new PointF(sq.Position.X, _PlayBlockArray[i, pos].Position.Y);
                                    prev = sq.ToString();
                                }
                                else if (prev == sq.ToString())
                                {
                                    sq.Destination = new PointF(sq.Position.X, _PlayBlockArray[i, pos].Position.Y);
                                    prev = "";
                                    pos = pos + forStep;
                                }
                                else
                                {
                                    pos = pos + forStep;
                                    sq.Destination = new PointF(sq.Position.X, _PlayBlockArray[i, pos].Position.Y);
                                    prev = sq.ToString();
                                }

                                if (sq.Destination != PointF.Empty && sq.Destination != sq.Position)
                                {
                                    Flag = true;
                                }

                                sq.SpeedX = ComputeSpeedX(sq.Position, sq.Destination, moves);
                                sq.SpeedY = ComputeSpeedY(sq.Position, sq.Destination, moves);
                            }
                        }

                    }
                }
            }
            else if (_Direction == Directions.LEFT || _Direction == Directions.RIGHT)
            {
                if (_Direction == Directions.LEFT)
                {
                    //for (int i = forFrom; i <= forTo; i += forStep)
                    for (int i = forFrom; i <= x; i += forStep)
                    {
                        int pos = forFrom;
                        string prev = "";


                        for (int j = forFrom; j <= forTo; j = j + forStep)
                        {
                            foreach (MySquare sq in _PlayBlockArray[j, i].MySquareList)
                            {
                                if (prev == "")
                                {
                                    sq.Destination = new PointF(_PlayBlockArray[pos, i].Position.X, sq.Position.Y);
                                    prev = sq.ToString();
                                }
                                else if (prev == sq.ToString())
                                {
                                    sq.Destination = new PointF(_PlayBlockArray[pos, i].Position.X, sq.Position.Y);
                                    prev = "";
                                    pos = pos + forStep;
                                }
                                else
                                {
                                    pos = pos + forStep;
                                    sq.Destination = new PointF(_PlayBlockArray[pos, i].Position.X, sq.Position.Y);
                                    prev = sq.ToString();
                                }

                                if (sq.Destination != PointF.Empty && sq.Destination != sq.Position)
                                {
                                    Flag = true;
                                }

                                sq.SpeedX = ComputeSpeedX(sq.Position, sq.Destination, moves);
                                sq.SpeedY = ComputeSpeedY(sq.Position, sq.Destination, moves);
                            }
                        }

                    }
                }
                else if (_Direction == Directions.RIGHT)
                {
                    //for (int i = forFrom; i >= forTo; i += forStep)
                    for (int i = x; i >= forTo; i += forStep)
                    {
                        int pos = forFrom;
                        string prev = "";


                        for (int j = forFrom; j >= forTo; j = j + forStep)
                        {
                            foreach (MySquare sq in _PlayBlockArray[j, i].MySquareList)
                            {
                                if (prev == "")
                                {
                                    sq.Destination = new PointF(_PlayBlockArray[pos, i].Position.X, sq.Position.Y);
                                    prev = sq.ToString();
                                }
                                else if (prev == sq.ToString())
                                {
                                    sq.Destination = new PointF(_PlayBlockArray[pos, i].Position.X, sq.Position.Y);
                                    prev = "";
                                    pos = pos + forStep;
                                }
                                else
                                {
                                    pos = pos + forStep;
                                    sq.Destination = new PointF(_PlayBlockArray[pos, i].Position.X, sq.Position.Y);
                                    prev = sq.ToString();
                                }

                                if (sq.Destination != PointF.Empty && sq.Destination != sq.Position)
                                {
                                    Flag = true;
                                }

                                sq.SpeedX = ComputeSpeedX(sq.Position, sq.Destination, moves);
                                sq.SpeedY = ComputeSpeedY(sq.Position, sq.Destination, moves);
                            }
                        }

                    }
                }
            }
            
            return Flag;
        }

        private double ComputeSpeedX(PointF Current, PointF Destination, int MovesToReachDest)
        {
            double currX, DestX;
            currX = Current.X;
            DestX = Destination.X;

            return (DestX - currX) / MovesToReachDest; //Frames;
        }

        private double ComputeSpeedY(PointF Current, PointF Destination, int MovesToReachDest)
        {
            double currY, DestY;
            currY = Current.Y;
            DestY= Destination.Y;

            return (DestY - currY) / MovesToReachDest;
        }

        public PlayStates Pause()
        {
            if (_PlayState == PlayStates.PLAYING) { SetPlayState(PlayStates.PAUSED); }
            else if (_PlayState == PlayStates.PAUSED) { SetPlayState(PlayStates.PLAYING); }

            return _PlayState;
        }

        private FlagsCheck CheckMovingSquare()
        {
            //bit flags ang return nito subok lang
            FlagsCheck ret = 0;
            for (int i = 0; i < _XCount; i++) {
                for (int j = 0; j < _YCount; j++) {
                    MySquare sq = _PlayBlockArray[i, j].MyFirstSquare;
                    if (sq != null)
                    {
                        if (sq.Animate) { ret = ret | FlagsCheck.ANIMATE; }

                        if (sq.SquareState == MySquare.SquareStates.Animating) { ret = ret | FlagsCheck.ANIMATING; }

                        if (!sq.DestinationReached) { ret = ret | FlagsCheck.MOVE; }

                        if (sq.SquareState == MySquare.SquareStates.Moving) { ret = ret | FlagsCheck.MOVING; }
                    }
                }
            }

            return ret;
        }

        public bool CheckGameOver()
        {
            bool found = false;

            if (MySquare.TotalSquareCreated >= _XCount * _YCount)
            {
                found = true;

                for (int i = 0; i < +_XCount; i++)
                {
                    for (int j = 0; j < _YCount; j++)
                    {
                        MySquare sq, sq2;
                        //Check Left
                        if (i > 0)
                        {
                            sq = _PlayBlockArray[i, j].MyFirstSquare;
                            sq2 = _PlayBlockArray[i - 1, j].MyFirstSquare;
                            //if (_PlayBlockArray[i, j].MyFirstSquare.ToString() == _PlayBlockArray[i - 1, j].MyFirstSquare.ToString()) { found = false; }
                            if (sq == null || sq2 == null || sq.ToString() == sq2.ToString()) { found = false; }
                        }

                        //Check Right
                        if (i < _XCount - 1)
                        {
                            sq = _PlayBlockArray[i, j].MyFirstSquare;
                            sq2 = _PlayBlockArray[i + 1, j].MyFirstSquare;
                            //if (_PlayBlockArray[i, j].MyFirstSquare.ToString() == _PlayBlockArray[i + 1, j].MyFirstSquare.ToString()) { found = false; }
                            if (sq == null || sq2 == null || sq.ToString() == sq2.ToString()) { found = false; }
                        }

                        //Check Top
                        if (j > 0)
                        {
                            sq = _PlayBlockArray[i, j].MyFirstSquare;
                            sq2 = _PlayBlockArray[i, j - 1].MyFirstSquare;
                            //if (_PlayBlockArray[i, j].MyFirstSquare.ToString() == _PlayBlockArray[i, j - 1].MyFirstSquare.ToString()) { found = false; }
                            if (sq == null || sq2 == null || sq.ToString() == sq2.ToString()) { found = false; }
                        }

                        //Check Bottom
                        if (j < _YCount - 1)
                        {
                            sq = _PlayBlockArray[i, j].MyFirstSquare;
                            sq2 = _PlayBlockArray[i, j + 1].MyFirstSquare;
                            //if (_PlayBlockArray[i, j].MyFirstSquare.ToString() == _PlayBlockArray[i, j + 1].MyFirstSquare.ToString()) { found = false; }
                            if (sq == null || sq2 == null || sq.ToString() == sq2.ToString()) { found = false; }
                        }
                    }
                }
            }

            return found;
        }


        #endregion

        #region "Events"

        void _PausePanel_Click(object sender, EventArgs e)
        {
            //SetPlayState(PlayStates.PLAYING);
        }

        protected virtual void OnPlayStateChanged(bool Changed)
        {
            if (Changed)
            {
                PlayState_Changed(this, _PlayState);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                using (new SolidBrush(Color.Blue))
                {
                    for (int i = 0; i < _XCount; i++)
                    {
                        for (int j = 0; j < _YCount; j++)
                        {
                            MySquare sq = _PlayBlockArray[i, j].MyFirstSquare;
                            if (sq != null) { sq.Draw(e.Graphics); }
                        }
                    }
                }
            }
            catch
            {
 
            }
        }

        #endregion
    }
}
