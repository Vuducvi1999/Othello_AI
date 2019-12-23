using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Othello
{
    class Board
    {

        public const int WIDTH = 55;    // chiều rộng 1 ô
        public const int BLACK = 1;
        public const int WHITE = -1;
        public const int EMPTY = 0;

        private int whitecount = 0;  // số quân trắng
        private int blackcount = 0;  // số quân đen

        private int[,] board;   // ma trận bàn cờ

        private int x_pre = -1;
        private int y_pre = -1;

        public int WhiteCount
        {
            get { return whitecount; }
        }

        public int BlackCount
        {
            get { return blackcount; }
        }


        public int X_Pre
        {
            get { return x_pre; }
            set { x_pre = value; }
        }

        public int Y_Pre
        {
            get { return y_pre; }
            set { y_pre = value; }
        }


        public Board(int resetflag)
        {
            if (resetflag == 1)
                InitMat();
            if (resetflag == 0)
                ResetMat();
            if (resetflag == 2)
                RandomMat();
        }
        /// <summary>
        /// Random bàn cờ
        /// </summary>
        public void RandomMat()
        {
            whitecount = 0;
            blackcount = 0;

            board = new int[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = EMPTY;
                }
            }

            int den, trang;
            board[3, 3] = WHITE;
            board[4, 3] = BLACK;
            board[3, 4] = BLACK;
            board[4, 4] = WHITE;
            Random ran = new Random();
            den = ran.Next(2, 15);
            trang = ran.Next(2, 15);
            for (int i = 0; i < den; i++)
            {
                int x, y;
                Random a = new Random();
                x = a.Next(1, 4);
                y = a.Next(1, 4);
                if (board[x, y] == EMPTY)
                    board[x, y] = BLACK;
            }
            for (int i = 0; i < trang; i++)
            {
                int x, y;
                Random a = new Random();
                x = a.Next(1, 4);
                y = a.Next(1, 4);
                if (board[x, y] == EMPTY)
                    board[x, y] = WHITE;
            }

            CountPieces();
        }


        /// <summary>
        /// Reset bàn cờ
        /// </summary>
        public void ResetMat()
        {
            whitecount = 0;
            blackcount = 0;

            board = new int[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = EMPTY;
                }
            }
        }
        /// <summary>
        /// Khởi tạo bàn cờ
        /// </summary>
        public void InitMat()
        {
            whitecount = 0;
            blackcount = 0;

            board = new int[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = EMPTY;
                }
            }

            // 4 quân cờ đầu tiên khi tạo 1 ván mới

            board[3, 3] = WHITE;
            board[4, 3] = BLACK;
            board[3, 4] = BLACK;
            board[4, 4] = WHITE;

            CountPieces();
        }
        // <summary>
        // tính quân trên bàn cờ của mỗi bên
        // </summary>
        private void CountPieces()
        {
            whitecount = 0;
            blackcount = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == WHITE)
                        whitecount++;
                    else
                        if (board[i, j] == BLACK)
                        blackcount++;
                }
            }
        }

        /// <summary>
        /// Copy ma trận bàn cờ
        /// </summary>
        /// <returns>Bàn cờ đã copy</returns>
        public Board Copy()
        {
            Board b = new Board(1);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    b.board[i, j] = board[i, j];
                }
            }
            b.whitecount = this.WhiteCount;
            b.blackcount = this.BlackCount;
            return b;
        }

        public void Copy(Board b)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = b.board[i, j];
                }
            }
            this.blackcount = b.BlackCount;
            this.whitecount = b.WhiteCount;
        }



        /// <summary>
        /// vẽ vào panel
        /// </summary>
        /// <param name="p">Panel</param>
        /// <param name="IsDrawHelp">vẽ các nước đi hợp lệ</param>
        /// <param name="step">bước</param>
        public void Draw(Panel p, bool IsDrawHelp, int step)
        {
            // graphics là dùng để thao tác đồ họa .
            // formhwnd là cái handle đặc trưng của panel ,
            Graphics gr = Graphics.FromHwnd(p.Handle);
            Draw(gr, IsDrawHelp, step);
        }

        public struct tuple
        {
            public int x, y;

            public tuple(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public Stack<tuple> Temp = new Stack<tuple>(3);
        /// <summary>
        /// vẽ bàn cờ
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="IsDrawHelp"></param>
        /// <param name="step"></param>
        public void Draw(System.Drawing.Graphics graphics, bool IsDrawHelp, int step)
        {
            Pen pen = new Pen(Color.White, (float)0.5);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int p = board[i, j];
                    Color c = Color.Green;

                    graphics.FillRectangle(new SolidBrush(c), i * WIDTH, j * WIDTH, WIDTH, WIDTH);

                    graphics.DrawRectangle(pen, i * WIDTH, j * WIDTH, WIDTH, WIDTH);

                    switch (p)
                    {
                        case BLACK:
                            graphics.FillEllipse(new SolidBrush(Color.Black), i * WIDTH + 3, j * WIDTH + 3, WIDTH - 6, WIDTH - 6);
                            break;
                        case WHITE:
                            graphics.FillEllipse(new SolidBrush(Color.White), i * WIDTH + 3, j * WIDTH + 3, WIDTH - 6, WIDTH - 6);
                            break;
                    };
                }
            }
            if (IsDrawHelp)
                DrawEnableSteps(step, graphics);

            if (x_pre >= 0 && y_pre >= 0)
            {
                graphics.DrawEllipse(new Pen(Color.Red, 2), x_pre * WIDTH + 3, y_pre * WIDTH + 3, WIDTH - 6, WIDTH - 6);
                if (step == Board.WHITE)
                    Temp.Push(new tuple(x_pre, y_pre));

            }
            //x_pre = -1;
            //y_pre = -1;
        }


        /// <summary>
        /// vẽ các nước đi hợp lệ
        /// </summary>
        /// <param name="step"></param>
        /// <param name="graphics"></param>
        public void DrawEnableSteps(int step, Graphics graphics)
        {
            Pen pen = new Pen(Color.White, (float)0.5);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Move(i, j, step, false) > 0)
                    {
                        graphics.FillRectangle(Brushes.Blue, i * WIDTH, j * WIDTH, WIDTH, WIDTH);
                        graphics.DrawRectangle(pen, i * WIDTH, j * WIDTH, WIDTH, WIDTH);
                    }
                }
            }
        }


        /// <summary>
        /// Tính toán giá trị bàn cờ , giá trị càng lớn thì quân trắng càng chiến lợi thế , nếu giá trị càng nhỏ thì quân đen chiếm lợi thế 
        /// </summary>
        /// <returns></returns> 
        public int GetValue()
        {

            int edge_value = 0; // tổng các biên vì nếu giá trị này âm thì quân trắng có thể đang chiếm ưu thế trên bàn cờ
                                // còn ngược lại thì quân đen đang chiếm ưu thế ( thông số này biểu thị chiến lược chiếm biên )
            int corner_value = 0;   // tính tổng 4 góc vì nếu giá trị này dương thì quân đen có nhiều lợi thế và 
                                    //ngược lại thì quân trắng có nhiều lợi thế , thông số này biểu thị chiến lược chiếm góc .
            int differentce_pieces = WhiteCount - BlackCount; // hiệu số quân vì rõ ràng quân càng nhiều thì tổng quan cũng có lợi cho đội mình ,
                                                              //  2 chiến lược là quan trọng để tính toán nước đi nhưng kết quả lại phụ thuộc vào số lượng quân trên bàn cờ .
                                                              //ở đây nếu quân trắng nhiều hơn thì máy sẽ có lợi hơn .
            int same_color = 0; // mang hàm ý càng nhiều quân trắng ở kề góc thì giá trị càng cao còn càng nhiều quân đen thì giá trị càng nhỏ , 
                                // điều này biểu thị càng nhiều quân cùng màu nằm sát góc thì rất khó để lật ngược thế cờ vì đã chiếm được góc thì không có cách nào ăn lại các quân này , 
                                // càng nhiều quân như thế này càng khiến cho phe địch bị lật kèo vì khi này góc và biên có nguy cơ bị chiếm nhiều hơn và các quân không tiếp giáp biên của phe bạn càng dễ bị ăn ngược lại
                                // thông số này khác với edge_value vì phụ thuộc vào quân nào đang chiếm ở góc nữa , 
                                //vì rõ ràng nếu biên đang nhiều quân đen thì chiếm được góc cũng có thể lật ngược thế cờ

            corner_value = board[0, 0] + board[0, 7] + board[7, 7] + board[7, 0];

            // tính tổng các biên của bàn cờ
            for (int i = 0; i < 8; i++)
                edge_value += (board[i, 0] + board[i, 7] + board[0, i] + board[7, i]);

            int first = board[0, 0];// góc trên bên trái
            if (first != EMPTY)
            {
                for (int i = 1; i < 8 && board[0, i] == first; i++)
                    same_color += first;
                for (int i = 1; i < 8 && board[i, 0] == first; i++)
                    same_color += first;
            }

            first = board[0, 7];// góc trên bên phải
            if (first != EMPTY)
            {
                for (int i = 6; i >= 0 && board[0, i] == first; i--)
                    same_color += first;
                for (int i = 1; i < 8 && board[i, 7] == first; i++)
                    same_color += first;
            }

            first = board[7, 0];// góc dưới bên trái
            if (first != EMPTY)
            {
                for (int i = 1; i < 8 && board[7, i] == first; i++)
                    same_color += first;
                for (int i = 6; i >= 0 && board[i, 7] == first; i--)
                    same_color += first;
            }

            first = board[7, 7];// góc dưới bên phải
            if (first != EMPTY)
            {
                for (int i = 6; i >= 0 && board[i, 7] == first; i--)
                    same_color += first;
                for (int i = 6; i >= 0 && board[7, i] == first; i--)
                    same_color += first;
            }

            int boardvalue = ((100 + WhiteCount + BlackCount) * differentce_pieces)
                             + (-200 * corner_value)
                             + (-150 * edge_value)
                             + (-250 * same_color);

            return boardvalue;
        }


        /* Vì đối với 1 vị trí thì ta phải duyệt 8 hướng nên ta sẽ tạo ra 8 hàm duyệt để kiểm tra 1 vị trí bất kì có thực hiện được 1 nước đi hay không , nếu thực hiện được thì các vị trí đó sẽ sáng lên màu Brushes.LawnGreen
        /*
         *	Kiểm tra các hướng dọc và ngang
         */
        #region SideCheck
        /// <summary> UpCheck là xét dọc từ dưới lên trên .
        /// nước đi được coi là hợp lệ nếu không có quân cờ nào trên ô đó và nước đi đó có thể thực hiện được ( dấu hiệu nhận biết : ô đó sẽ sáng lên )
        /// từ vị trí hiện tại chiếu dọc lên trên , nếu vị trí hiện tại đang xét không có quân nào thì return 0 
        /// (vì những hàm kiểm tra các hướng này là giúp kiểm tra thử 1 vị trí trên bàn cờ
        /// thông qua 1 hàm khác là hàm Move , vậy nên khi sử dụng lại hàm UpCheck này mới có dạng UpCheck(x, y - 1, p, IsAdd) , 
        /// khi này hàng giữ nguyên còn cột -1 để kiểm tra ô phía trên board[x,y] )
        /// còn nếu tại vị trí này có 1 quân khác màu (board[x, i] == -p) thì quân này sẽ đổi màu thành quân p nếu bool f = true 
        /// hoặc nếu f=false thì cũng sẽ đảm bảo rằng hướng kiểm tra UpCheck có thể thực thi để đổi màu 1 quân phe bạn 
        /// ( lấy ví dụ ảnh "Ví dụ cho hàm UpCheck" )
        /// và điều kiện để đổi màu tất cả các quân khác phe hướng UpCheck là duyệt cho đến khi nào gặp 1 quân phe mình (board[x, i] == p).
        /// </summary>
        /// <param name="x">hàng</param>
        /// <param name="y"></param> cột
        /// <param name="p"></param> màu quân cờ , ở đây nếu là quân đen ( tức p=1) thì giá trị f sẽ là false để kiểm tra xem nước đi có thực hiện được hay không
        /// hoặc nếu là quân trắng thì sẽ là true để thực hiện nước đi ngay sau khi lấy đánh giá từ hàm GetBestStep
        /// <param name="f"></param> nếu true thì thực thi ngay nước đi ,nếu false thì kiểm tra có đi được không
        /// <returns></returns>
        private int UpCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0;
            int c = 0;
            for (i = y; i >= 0; i--)
            {
                if (board[x, i] == EMPTY) return 0;

                if (board[x, i] == -p) c++;
                if (board[x, i] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (int j = y; j != i; j--)
                            board[x, j] = p;
                    break;
                }
            }
            return found ? c : 0;
        }

        /// <summary>DowCheck là xét dọc từ trên xuống dưới .
        /// và nguyên lí hoạt động cũng tương tự như UpCheck , chỉ khác hướng duyệt .
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private int DownCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0;
            int c = 0;
            for (i = y; i < 8; i++)
            {
                if (board[x, i] == EMPTY) return 0;

                if (board[x, i] == -p) c++;
                if (board[x, i] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (int j = y; j != i; j++)
                            board[x, j] = p;
                    break;
                }
            }
            return found ? c : 0;
        }
        /// <summary> 
        /// duyệt ngang từ phải qua trái 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private int LeftCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0;
            int c = 0;
            for (i = x; i >= 0; i--)
            {
                if (board[i, y] == EMPTY) return 0;

                if (board[i, y] == -p) c++;
                if (board[i, y] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (int j = x; j != i; j--)
                            board[j, y] = p;
                    break;
                }
            }
            return found ? c : 0;
        }
        /// <summary>
        /// duyệt ngang từ trái qua phải
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private int RightCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0;
            int c = 0;
            for (i = x; i < 8; i++)
            {
                if (board[i, y] == EMPTY) return 0;

                if (board[i, y] == -p) c++;
                if (board[i, y] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (int j = x; j != i; j++)
                            board[j, y] = p;
                    break;
                }
            }
            return found ? c : 0;
        }
        #endregion SideCheck

        /*
         *	Kiểm tra các hướng chéo
         */
        #region Diagonal
        /// <summary>
        /// duyệt từ dưới chéo lên phía trái 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private int UpLeftCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0, j = 0;
            int a, b;
            int c = 0;
            for (i = x, j = y; i >= 0 && j >= 0; i--, j--)
            {
                if (board[i, j] == EMPTY) return 0;

                if (board[i, j] == -p) c++;
                if (board[i, j] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (a = x, b = y; a != i && b != j; a--, b--)
                            board[a, b] = p;
                    break;
                }
            }
            return found ? c : 0;
        }
        /// <summary>
        /// duyệt từ dưới chéo lên phía phải
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private int UpRightCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0, j = 0;
            int a, b;
            int c = 0;
            for (i = x, j = y; i < 8 && j >= 0; i++, j--)
            {
                if (board[i, j] == EMPTY) return 0;

                if (board[i, j] == -p) c++;
                if (board[i, j] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (a = x, b = y; a != i && b != j; a++, b--)
                            board[a, b] = p;
                    break;
                }
            }
            return found ? c : 0;
        }
        /// <summary>
        /// duyệt từ trên xuống sang phía phải
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private int DownRightCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0, j = 0;
            int a, b;
            int c = 0;
            for (i = x, j = y; i < 8 && j < 8; i++, j++)
            {
                if (board[i, j] == EMPTY) return 0;

                if (board[i, j] == -p) c++;
                if (board[i, j] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (a = x, b = y; a != i && b != j; a++, b++)
                            board[a, b] = p;
                    break;
                }
            }
            return found ? c : 0;
        }
        /// <summary>
        /// duyệt từ trên xuống sang phía trái
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private int DownLeftCheck(int x, int y, int p, bool f)
        {
            bool found = false;
            int i = 0, j = 0;
            int a, b;
            int c = 0;
            for (i = x, j = y; i >= 0 && j < 8; i--, j++)
            {
                if (board[i, j] == EMPTY) return 0;

                if (board[i, j] == -p) c++;
                if (board[i, j] == p)
                {
                    found = c > 0;
                    if (c > 0 && f)
                        for (a = x, b = y; a != i && b != j; a--, b++)
                            board[a, b] = p;
                    break;
                }
            }
            return found ? c : 0;
        }
        #endregion Diagonal


        /// <summary>
        /// Đặt quân cờ vào bàn, kiểm tra có hợp lệ không
        /// 
        /// </summary>
        /// <param name="x">hàng</param>
        /// <param name="y">cột</param>
        /// <param name="p"></param>
        /// <param name="IsAdd"></param>
        /// <returns>Giá trị trả về > 0 là hợp lệ, ngược lại = 0 là không hợp lệ</returns>
        internal int Move(int x, int y, int p, bool IsAdd)
        {
            int res = 0;

            if (board[x, y] == EMPTY)
            {
                res = UpCheck(x, y - 1, p, IsAdd) +
                        DownCheck(x, y + 1, p, IsAdd) +
                        LeftCheck(x - 1, y, p, IsAdd) +
                        RightCheck(x + 1, y, p, IsAdd) +
                        UpLeftCheck(x - 1, y - 1, p, IsAdd) +
                        UpRightCheck(x + 1, y - 1, p, IsAdd) +
                        DownRightCheck(x + 1, y + 1, p, IsAdd) +
                        DownLeftCheck(x - 1, y + 1, p, IsAdd);

                if (res > 0)
                {
                    if (IsAdd)
                    {
                        whitecount++;
                        board[x, y] = p;
                        CountPieces();
                    }
                }
            }
            return res;
        }





        /// <summary>
        /// Tính toán nước đi hợp lệ
        /// </summary>
        /// <param name="color">màu quân cờ</param>
        /// <returns>List hợp lệ</returns>
        public List<int[]> GetEnableSteps(int color)
        {
            List<int[]> EnableStepsList = new List<int[]>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int res = Move(i, j, color, false);
                    if (res > 0)
                    {
                        EnableStepsList.Add(new int[] { i, j });
                    }
                }
            }

            return EnableStepsList;
        }
        /// <summary>
        /// Minimax kết hợp alpha-beta
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="depth"></param>
        /// <param name="MAX"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int GetBestStep(int piece, int alpha, int beta, int depth, bool MAX, Board board)
        {
            if (depth == 0 || board.WhiteCount + board.BlackCount == 64)
                return board.GetValue();
            // List các nước đi có thể xảy ra
            List<int[]> EnableStepsList;
            if (piece == WHITE)
                EnableStepsList = board.GetEnableSteps(BLACK);// <----------------
            else
                EnableStepsList = board.GetEnableSteps(WHITE);// <----------------

            if (EnableStepsList.Count == 0 && MAX == true)
                return -Int32.MaxValue;
            if (MAX == true)
            {
                int value = -Int32.MaxValue;
                foreach (int[] s in EnableStepsList)
                {
                    Board boardcopy = board.Copy();
                    if (piece == WHITE)
                        boardcopy.Move(s[0], s[1], BLACK, true);
                    else
                        boardcopy.Move(s[0], s[1], WHITE, true);
                    value = Math.Max(value, GetBestStep(-piece, alpha, beta, depth - 1, false, boardcopy));
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                return value;
            }
            else
            {
                int value = Int32.MaxValue;
                foreach (int[] s in EnableStepsList)
                {
                    Board boardcopy = board.Copy();
                    if (piece == WHITE)
                        boardcopy.Move(s[0], s[1], BLACK, true);
                    else
                        boardcopy.Move(s[0], s[1], WHITE, true);
                    value = Math.Min(value, GetBestStep(piece, alpha, beta, depth - 1, true, boardcopy));
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                return value;
            }
        }
    }
}