using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
// source code ko bị lỗi phiên bản .NET
namespace Othello
{
    public partial class Form1 : Form
    {
        delegate void InvokeDraw(Panel p, bool b, int s);
        delegate void CompStepDelegate();
        delegate void InvokeShowRes();
        delegate void InvokeShowGrade(string s);
        delegate void InvokeAdd(List<int[]> list, int j);
        delegate void SetStatusTextDelegate_tool(ToolStripStatusLabel label, string text);

        delegate void SetStatusTextDelegate(Label label, string text);

        delegate void ShowMessageDelegate(string message, string title, MessageBoxButtons btn, MessageBoxIcon icon);

        private CompStepDelegate EvCompStep;
        private Board board;

        private bool flag = false;// cờ đánh dấu chuyển lượt đi , nếu là true thì máy được đi , false thì người đi .
        private bool started = false; // biến đánh dấu, ván cờ đã bắt đầu
        private bool IsDrawHelp = false;

        private DateTime da1;// đếm giờ
        private DateTime da2;
        DateTime End_time;

        int bugcuoicung = 0;
        //khởi tạo form 
        public Form1()
        {
            InitializeComponent();
            EvCompStep = new CompStepDelegate(CompStep);
            ResetBoard();
            toolStripStatusLabel1.Text = "tính giờ";
            label26.Text = "";
            toolStripStatusLabel2.Text = "Máy : 21000 ms  ";
            toolStripStatusLabel3.Text = "Người : 21000 ms ";
        }
        private void ResetBoard()
        {
            label5.Text = "";
            board = new Board(0);
            bugcuoicung = 0;
        }
        private void RandomBoard()
        {
            label5.Text = "";
            board = new Board(2);
            bugcuoicung = 0;
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            board.Draw(e.Graphics, IsDrawHelp, Board.EMPTY);
        }


        delegate void EnableTimerDelegate(bool enabled);
        /// <summary>
        /// Thực hiện nc đi của máy
        /// </summary>
        private void CompStep()
        {
            Invoke(new EnableTimerDelegate(EnableTimer), true);
            do
            {
                DoBestStep();
            }
            while (board.GetEnableSteps(Board.BLACK).Count == 0 && board.GetEnableSteps(Board.WHITE).Count > 0);
            // dùng khi phe người mất lượt (hết nước đi) thì máy vẫn sẽ tiếp tục đánh .
        }

        /// <summary>
        /// Tính quân mỗi bên
        /// </summary>
        private void ShowPoints()
        {
            label1.Text = board.BlackCount.ToString();
            label2.Text = board.WhiteCount.ToString();
        }

        /// <summary>
        /// Giá trị của trạng thái vừa đi
        /// </summary>
        /// <param name="s"></param>
        private void ShowGrade(string s)
        {
            label5.Text = s;
        }



        /// <summary>
        /// Máy tính đi nước đi tốt nhất
        /// </summary>
        private void DoBestStep()
        {
            List<int[]> list = board.GetEnableSteps(Board.WHITE);
            int j = 0;
            int max = -Int32.MaxValue;
            int valueboard = 0;
            int difficulty = Convert.ToInt32(numericUpDown1.Value);
            // Bản chất minimax là heuristic của bàn cờ phe địch sao cho bàn cờ phe địch đó là tệ nhất trong các bàn cờ tốt nhất của phe địch .
            // Căn cứ vào đó ta đánh nước cờ làm cho phe địch tổn thất nhất .
            // tính xem ứng với mỗi nước đi của quân trắng(máy) thì quân đen(người) sẽ đi nước đi nào tốt nhất , 
            // căn cứ vào giá trị minimax trả về ta chọn ra nước đi của quân trắng(máy) khiến quân đen(người) khó thắng nhất .
            for (int i = 0; i < list.Count; i++)
            {
                //Sở dĩ phải giả định các bàn cờ sau khi đi nước cờ của máy vì bàn cờ của ta lưu dưới dạng ma trận ,cần phải tìm ô tốt nhất để quân cờ di chuyển vào đó .

                Board copyboard1 = board.Copy();
                copyboard1.Move(list[i][0], list[i][1], Board.WHITE, true);

                int res = Board.GetBestStep(Board.WHITE, -Int32.MaxValue, Int32.MaxValue, difficulty, true, copyboard1);
                //int res = Board.GetBestStep(Board.BLACK, -Int32.MaxValue, Int32.MaxValue, difficulty, false, copyboard1);

                if (max < res)
                {
                    j = i;
                    max = res;
                }
            }
            if (list.Count > j) // TH khác xảy ra là list.Count = j = 0 , khi này máy tính mất lượt .
            {
                Undo_Point.Push(new tuple(list[j][0], list[j][1]));
                board.Move(list[j][0], list[j][1], Board.WHITE, true);
                valueboard = board.GetValue();
                board.X_Pre = list[j][0];
                board.Y_Pre = list[j][1];
            }
            Invoke(new EnableTimerDelegate(EnableTimer), false);
            Invoke(new InvokeShowGrade(ShowGrade), valueboard.ToString());

            try
            {
                Invoke(new InvokeAdd(InsertToTable), list, j);
            }
            catch
            {
                Invoke(new ShowMessageDelegate(ShowMessage), "Máy tính mất lượt", "Mất lượt"
                                                            , MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Chèn DL vào bảng từ nước cờ do máy đánh
        /// </summary>
        /// <param name="list"></param>
        /// <param name="j"></param>
        /// 
        private void InsertToTable(List<int[]> list, int j)
        {
            int m = listView1.Items.Count + 1;
            string[] stritems = { m.ToString(), "Trắng", Convert.ToChar(list[j][0] + 65) + (list[j][1] + 1).ToString() };
            //thực tập
            //string Undoed = "####### Đã undo !!!!!!!!";
            //ListViewItem Undo_ListViewItem = new ListViewItem(Undoed);
            //
            ListViewItem newitem = new ListViewItem(stritems);
            listView1.Items.Add(newitem);
            listView1.EnsureVisible(m - 1);
            listView1.Items[m - 1].Selected = true;

            for (int i = 0; i < m - 1; i++)
            {
                listView1.Items[i].Selected = false;
            }
            board.X_Pre = list[j][0];
            board.Y_Pre = list[j][1];
        }


        /// <summary>
        /// Sau khi kết thúc bước đi của máy
        /// </summary>
        /// <param name="ob"></param>
        private void EndCompStep(object ob)
        {

            Save_Board.Add(board.Copy());
            Save_Point.Add(new tuple(board.X_Pre, board.Y_Pre));
            Count++;


            if (panel1.InvokeRequired) // panel có yêu cầu Invoke hay không
                Invoke(new InvokeDraw(board.Draw), panel1, IsDrawHelp, Board.BLACK);
            else
                board.Draw(panel1, IsDrawHelp, Board.BLACK);

            // Invoke(new SetStatusTextDelegate(SetStatusText),  label26, "Tới lượt bạn");
            Invoke(new SetStatusTextDelegate(SetStatusText), label26, "Lượt Bạn");



            int HumanSteps = board.GetEnableSteps(Board.BLACK).Count;    // số nước đi tiếp theo của người

            if (HumanSteps == 0)
            {
                Invoke(new InvokeShowGrade(ShowGrade), "");
                Invoke(new SetStatusTextDelegate(SetStatusText), label26, "");

                Invoke(new SetStatusTextDelegate_tool(SetTimerText), toolStripStatusLabel2, "");
                timer1.Stop();
                timer2.Stop();
                bugcuoicung = 1;
                // started = false;
                flag = false;
                if (board.WhiteCount > board.BlackCount)
                {
                    Invoke(new ShowMessageDelegate(ShowMessage), "Máy tính đã thắng", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (board.WhiteCount < board.BlackCount)
                        Invoke(new ShowMessageDelegate(ShowMessage), "Bạn đã thắng", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        if (board.WhiteCount == board.BlackCount)
                        {
                            Invoke(new ShowMessageDelegate(ShowMessage), "Hòa cờ", "Thông báo"
                                                       , MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Invoke(new ShowMessageDelegate(ShowMessage), "Bạn bị mất lượt", "Thông báo"
                                                       , MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

            }

            flag = false;
            bugcuoicung = 0;

            if (InvokeRequired) // lại yêu cầu kiểm tra panel có cần Invoke hay không ?
                Invoke(new InvokeShowRes(ShowPoints));
            else
                ShowPoints();
        }



        private void ShowMessage(string message, string title, MessageBoxButtons btn, MessageBoxIcon icon)
        {
            MessageBox.Show(this, message, title, btn, icon);
        }

        public int count_time;
        /// <summary>
        /// nếu là true thì máy đi , nếu là false thì người đi
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableTimer(bool enabled)
        {
            //count_time = 21000;
            //timer1.Enabled = enabled;
            if (enabled)
            {
                da1 = DateTime.Now;
                timer1.Start();
                //toolStripStatusLabel2.Text = "0:0:0:0";
                //toolStripStatusLabel2.Text = count_time.ToString();
                timer2.Stop();
            }
            else
            {
                da2 = DateTime.Now;
                timer1.Stop();
                //toolStripStatusLabel2.Text = "0:0:0:0";
                //toolStripStatusLabel2.Text = count_time.ToString();
                timer2.Start();
            }
        }

        //private void SetStatusText(ToolStripStatusLabel label, string text)
        private void SetStatusText(Label label, string text)
        {
            label.Text = text;
            if (text.Equals(""))
            {
                //toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
                label.Text = "Start Game";
            }
            if (text.Equals("Tới lượt bạn"))
            {
                panelColor.BackColor = Color.Black;
            }
        }
        private void SetTimerText(ToolStripStatusLabel label, string text)
        {
            label.Text = text;
            if (text.Equals(""))
            {
                toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            }
            if (text.Equals("Tới lượt bạn"))
            {
                panelColor.BackColor = Color.Black;
            }
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

        private Stack<Board> Redo = new Stack<Board>(4);
        private Stack<Board> Undo = new Stack<Board>(4);
        private Stack<tuple> Undo_Point = new Stack<tuple>(5);
        private Stack<tuple> Redo_Point = new Stack<tuple>(5);
        /// <summary>
        /// Thực hiện nước đi của người
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (started)
            {

                numericUpDown1.Enabled = false;

                if (flag)
                    return;
                Undo.Push(board.Copy());

                int x = e.X / Board.WIDTH;
                int y = e.Y / Board.WIDTH;

                if (helped == 1)
                {
                    //Save_Board.Clear();
                    //Save_Point.Clear();
                    //Save_View.Clear(); // Save_View không quan trọng !
                    //Count = 0;
                    //Save_Board.Add(board.Copy());
                    //Save_Point.Add(new tuple(board.X_Pre, board.Y_Pre));
                    //Count = 1;
                    Save_Board.RemoveRange(Count, Save_Board.Count - Count);
                    Save_Point.RemoveRange(Count, Save_Point.Count - Count);
                    Count = Save_Board.Count;
                }
                helped = 0;

                if (board.Move(x, y, Board.BLACK, true) > 0)
                {
                    flag = true;
                    bugcuoicung = 0;
                    //nước đi của máy
                    {

                        board.X_Pre = x;
                        board.Y_Pre = y;

                        board.Draw(panel1, IsDrawHelp, Board.WHITE);
                        EvCompStep.BeginInvoke(new AsyncCallback(EndCompStep), null); // phương thức không đồng bộ 
                        ShowPoints();

                        //toolStripStatusLabel1.Text = "Tới lượt máy";
                        label26.Text = "Lượt Máy";

                        panelColor.BackColor = Color.White;
                        toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                        toolStripProgressBar1.MarqueeAnimationSpeed = (int)numericUpDown1.Value * 10;
                    }
                }
                else
                {
                    if (IsDrawHelp && (board.WhiteCount + board.BlackCount) < 64 && bugcuoicung == 0)
                    {
                        Invoke(new ShowMessageDelegate(ShowMessage), "           # Đi sai nước.\n ~ Hãy đi vào ô sáng màu ~", "Đi sai nước"
                                            , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if ((board.WhiteCount + board.BlackCount) < 64 && bugcuoicung == 0)
                            Invoke(new ShowMessageDelegate(ShowMessage), "                 # Đi sai nước.\nHãy chọn chế độ xem nước đi hợp lệ !", "Đi sai nước"
                                                , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return;
                }

                int m = listView1.Items.Count + 1;
                string[] stritems = { m.ToString(), "Đen", Convert.ToChar(x + 65) + (y + 1).ToString() };

                ListViewItem newitem = new ListViewItem(stritems);
                listView1.Items.Add(newitem);
                listView1.EnsureVisible(m - 1);// scroll list view xuống dưới            
                listView1.Items[m - 1].Selected = true;
                for (int i = 0; i < m - 1; i++)
                {
                    listView1.Items[i].Selected = false;
                }
            }
        }
        // có cần hiện các nước gợi ý không ?
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                IsDrawHelp = true;
            }
            else
            {
                IsDrawHelp = false;
            }
            board.Draw(panel1, IsDrawHelp, Board.BLACK);
        }


        private void ResetGame()
        {
            ResetBoard();
            board.Draw(panel1, IsDrawHelp, Board.BLACK);
            listView1.Items.Clear();
            label1.Text = "0";
            label2.Text = "0";
            numericUpDown1.Enabled = true;
            button1.Enabled = true;
            button5.Enabled = true;
            //toolStripStatusLabel1.Text = "Làm cảnh";
            toolStripStatusLabel1.Text = "Limit: 35 phút";

            label26.Text = "";
            toolStripStatusLabel2.Text = "Máy : 21000";
            toolStripStatusLabel3.Text = "Người : 21000";
            toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            started = false;

            timer1.Enabled = false;
            timer2.Enabled = false;
            bugcuoicung = 0;
        }
        // game mới
        private void button2_Click(object sender, EventArgs e)
        {
            if (started)
            {
                DialogResult res = MessageBox.Show("Bạn đang chơi dở ván cờ.\nBạn muốn hủy bỏ và chơi ván cờ mới ?", "Tạo ván cờ mới ?"
                                                    , MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    ResetGame();
                }
            }
            else
            {
                ResetGame();
            }
        }


        private void InitBoard()
        {
            label5.Text = "";
            board = new Board(1);
            panel1.Width = 8 * Board.WIDTH;
            panel1.Height = 8 * Board.WIDTH;
            bugcuoicung = 0;
        }
        private void RandomGame()
        {
            label5.Text = "";
            board = new Board(2);
            panel1.Width = 8 * Board.WIDTH;
            panel1.Height = 8 * Board.WIDTH;
            bugcuoicung = 0;
        }
        int time_count;
        int count_time_1, count_time_2;
        // bắt đầu ván cờ
        private void button1_Click(object sender, EventArgs e)
        {
            //time_count = 21000; // 35  phút .
            //timer3.Start();
            //End_time = DateTime.Now;
            count_time_1 = 21000;
            count_time_2 = 21000;
            InitBoard();
            Invoke(new EnableTimerDelegate(EnableTimer), false);
            /////////////////////////////////////////////
            Save_Board.Clear();
            Save_Point.Clear();
            Save_View.Clear();
            Count = 0;
            /////////////////////////////////////////////////
            Undo_Point.Push(new tuple(-1, -1));
            Save_Board.Add(board.Copy());
            Save_Point.Add(new tuple(-1, -1));
            Count = 1;
            ///////////////////////////////////////////////
            checkBox1.Enabled = true;
            checkBox1.Checked = true;
            board.Draw(panel1, IsDrawHelp, Board.BLACK);
            listView1.Items.Clear();
            label1.Text = "2";
            label2.Text = "2";
            numericUpDown1.Enabled = false;
            started = true;
            button1.Enabled = false;

            //toolStripStatusLabel1.Text = "Tới lượt bạn";
            toolStripStatusLabel1.Text = "Limit: 35 phút                ";
            label26.Text = "Lượt Bạn";
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            panelColor.BackColor = Color.Black;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            count_time_1 = 21000;
            count_time_2 = 21000;
            RandomGame();

            {

                Undo.Push(board.Copy());

                Save_Board.RemoveRange(Count, Save_Board.Count - Count);
                Save_Point.RemoveRange(Count, Save_Point.Count - Count);
                Count = Save_Board.Count;

                //nước đi của máy
                {

                    board.Draw(panel1, IsDrawHelp, Board.WHITE);
                    EvCompStep.BeginInvoke(new AsyncCallback(EndCompStep), null); // phương thức không đồng bộ 
                    ShowPoints();

                    //toolStripStatusLabel1.Text = "Tới lượt máy";
                    label26.Text = "Lượt Máy";

                    panelColor.BackColor = Color.White;
                    toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                    toolStripProgressBar1.MarqueeAnimationSpeed = (int)numericUpDown1.Value * 10;
                }
                int m = listView1.Items.Count + 1;
                string[] stritems = { m.ToString(), "Đen", Convert.ToChar(65) + (1).ToString() };

                ListViewItem newitem = new ListViewItem(stritems);
                listView1.Items.Add(newitem);
                listView1.EnsureVisible(m - 1);// scroll list view xuống dưới            
                listView1.Items[m - 1].Selected = true;
                for (int i = 0; i < m - 1; i++)
                {
                    listView1.Items[i].Selected = false;
                }
            }

            Invoke(new EnableTimerDelegate(EnableTimer), false);
            /////////////////////////////////////////////
            //Save_Board.Clear();
            //Save_Point.Clear();
            //Save_View.Clear();
            //Count = 0;
            /////////////////////////////////////////////////
            Undo_Point.Push(new tuple(-1, -1));
            Save_Board.Add(board.Copy());
            Save_Point.Add(new tuple(-1, -1));
            Count = 1;
            ///////////////////////////////////////////////
            checkBox1.Enabled = true;
            checkBox1.Checked = true;
            board.Draw(panel1, IsDrawHelp, Board.BLACK);
            listView1.Items.Clear();
            label1.Text = "3";
            label2.Text = "3";
            numericUpDown1.Enabled = false;
            started = true;
            button5.Enabled = false;

            //toolStripStatusLabel1.Text = "Tới lượt bạn";
            toolStripStatusLabel1.Text = "Limit: 35 phút";
            label26.Text = "Lượt Bạn";
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            panelColor.BackColor = Color.Black;
        }
        /// <summary>
        /// Đếm giờ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            count_time_1--;

            if (count_time_1 != 0)
            {
                toolStripStatusLabel2.Text = "Máy : " + count_time_1.ToString() + " ms                ";
            }
            if (count_time_1 == 0)
            {
                timer3.Stop();
                timer2.Stop();
                timer1.Stop();
                MessageBox.Show("hết giờ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel3.Text = "Hết Giờ";
                toolStripStatusLabel2.Text = "Hết Giờ";
                Invoke(new ShowMessageDelegate(ShowMessage), "Bạn đã thắng", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                started = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            count_time_2--;
            if (count_time_2 != 0)
            {
                toolStripStatusLabel3.Text = "Người : " + count_time_2.ToString() + " ms                ";
            }
            if (count_time_2 == 0)
            {
                timer3.Stop();
                timer2.Stop();
                timer1.Stop();
                MessageBox.Show("hết giờ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel3.Text = "Hết Giờ";
                toolStripStatusLabel2.Text = "Hết Giờ";
                Invoke(new ShowMessageDelegate(ShowMessage), "Máy tính đã thắng", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                started = false;
            }

        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            time_count--;
            if (time_count == 0)
            {
                timer3.Stop();
                timer2.Stop();
                timer1.Stop();
                MessageBox.Show("hết giờ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel3.Text = "Hết Giờ";
                toolStripStatusLabel2.Text = "Hết Giờ";
                if (board.WhiteCount > board.BlackCount)
                {
                    Invoke(new ShowMessageDelegate(ShowMessage), "Máy tính đã thắng", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (board.WhiteCount < board.BlackCount)
                        Invoke(new ShowMessageDelegate(ShowMessage), "Bạn đã thắng", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        if (board.WhiteCount == board.BlackCount)
                        {
                            Invoke(new ShowMessageDelegate(ShowMessage), "Hòa cờ", "Thông báo"
                                                       , MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                started = false;
            }
            else
            {

                TimeSpan span = DateTime.Now.Subtract(End_time);
                toolStripStatusLabel3.Text = span.Minutes.ToString() + ":" +
                                             span.Seconds.ToString() + ":"
                                            + span.Milliseconds.ToString();
            }

        }

        List<Board> Save_Board = new List<Board>();
        List<tuple> Save_Point = new List<tuple>();
        List<ListViewItem> Save_View = new List<ListViewItem>();
        int helped = 0;
        int Count = 0;
        /// <summary>
        /// Undo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (flag == true)
                return;
            int fail = 0;

            if (Count == 0) // phương án dự phòng nhưng có vẻ không dùng đến .
            {
                Invoke(new ShowMessageDelegate(ShowMessage), " Không thể undo !\n Hoặc đã sử dụng quyền trợ giúp Redo , redo xong không thể tiếp tục Undo cho đến khi đi nước mới !", "Thông báo"
                                                        , MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Count == 1)
            {
                Invoke(new ShowMessageDelegate(ShowMessage), "Không thể load form đã Undo trước đó !\n\nHoặc vì đã sử dụng Undo để đi nước cờ mới !", "Thông báo"
                                                        , MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //if (Save_Board.Count==0)
            //{
            //    Invoke(new ShowMessageDelegate(ShowMessage), " Không thể undo tiếp !\n Hoặc đã sử dụng quyền trợ giúp Redo , redo xong không thể tiếp tục Undo cho đến khi đi nước mới !", "Thông báo"
            //                                        , MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (Save_Board.Count == 1)
            //{
            //    Invoke(new ShowMessageDelegate(ShowMessage), "Sau khi Undo để đi nước cờ mới không thể Undo lại ", "Thông báo"
            //                                        , MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //try
            //{
            helped = 1;
            fail = 1;

            panelColor.BackColor = Color.Black;
            label26.Text = "LƯỢT BẠN";
            Board p = Save_Board[Count - 2];
            board.Copy(p);
            tuple p1 = Save_Point[Count - 2];
            Count--;
            board.X_Pre = p1.x;
            board.Y_Pre = p1.y;
            board.Draw(panel1, IsDrawHelp, Board.BLACK);

            ShowPoints();
            int valueboard = board.GetValue();

            Invoke(new InvokeShowGrade(ShowGrade), valueboard.ToString());
            Invoke(new EnableTimerDelegate(EnableTimer), false);
            string Undoed = "Đã Undo ";
            ListViewItem Undo_ListViewItem = new ListViewItem(Undoed);
            listView1.Items.Add(Undo_ListViewItem);
            listView1.EnsureVisible(listView1.Items.Count - 1);
            bugcuoicung = 0;
            //}
            //catch 
            //{
            //    helped = 1;
            //    fail = 1;

            //    panelColor.BackColor = Color.Black;
            //    label26.Text = "LƯỢT BẠN";
            //    Board p = Save_Board[0];
            //    board.Copy(p);
            //    tuple p1 = Save_Point[0];
            //    Count--;
            //    board.X_Pre = p1.x;
            //    board.Y_Pre = p1.y;
            //    board.Draw(panel1, IsDrawHelp, Board.BLACK);

            //    ShowPoints();
            //    int valueboard = board.GetValue();

            //    Invoke(new InvokeShowGrade(ShowGrade), valueboard.ToString());
            //    Invoke(new EnableTimerDelegate(EnableTimer), false);
            //    string Undoed = "Đã Undo ";
            //    ListViewItem Undo_ListViewItem = new ListViewItem(Undoed);
            //    listView1.Items.Add(Undo_ListViewItem);
            //    listView1.EnsureVisible(listView1.Items.Count -1);
            //    bugcuoicung = 0;
            //}
            if (fail == 0)
            {
                Invoke(new ShowMessageDelegate(ShowMessage), " Không thể undo tiếp !", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }

        }
        public struct tuple_listview
        {
            public ListViewItem x, y;
            public tuple_listview(ListViewItem a, ListViewItem b) { x = a; y = b; }
        }

        Queue<tuple_listview> for_redo = new Queue<tuple_listview>(5);

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Redo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (flag == true)
                return;
            // thực ra ở đây fail là không cần sử dụng .
            if (Count == Save_Board.Count)
            {
                Invoke(new ShowMessageDelegate(ShowMessage), "Không thể Redo !", "Thông báo"
                                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //if (Save_Board.Count == 0)
            //{
            //    Invoke(new ShowMessageDelegate(ShowMessage), "Không thể Redo !", "Thông báo"
            //                                        , MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            int fail = 0;
            //if (Count == 1)
            //{
            panelColor.BackColor = Color.Black;
            label26.Text = "LƯỢT BẠN";
            Board b = Save_Board[Count];
            board.Copy(b);
            tuple b1 = Save_Point[Count];
            Count++;
            board.X_Pre = b1.x;
            board.Y_Pre = b1.y;
            board.Draw(panel1, IsDrawHelp, Board.BLACK);
            ShowPoints();
            int valueboard = board.GetValue();


            Invoke(new InvokeShowGrade(ShowGrade), valueboard.ToString());
            Invoke(new EnableTimerDelegate(EnableTimer), false);

            string Undoed = "Đã Redo";
            ListViewItem Undo_ListViewItem = new ListViewItem(Undoed);
            listView1.Items.Add(Undo_ListViewItem);
            listView1.EnsureVisible(listView1.Items.Count - 1);

            fail = 1;
            bugcuoicung = 0;
            //}
            //else
            //{

            //    panelColor.BackColor = Color.Black;
            //    label26.Text = "LƯỢT BẠN";
            //    Board b = Save_Board[Count ];
            //    board.Copy(b);
            //    tuple b1 = Save_Point[Count ];
            //    Count++;
            //    board.X_Pre = b1.x;
            //    board.Y_Pre = b1.y;
            //    board.Draw(panel1, IsDrawHelp, Board.BLACK);
            //    ShowPoints();
            //    int valueboard = board.GetValue();


            //    Invoke(new InvokeShowGrade(ShowGrade), valueboard.ToString());
            //    Invoke(new EnableTimerDelegate(EnableTimer), false);

            //    string Undoed = "Đã Redo";
            //    ListViewItem Undo_ListViewItem = new ListViewItem(Undoed);
            //    listView1.Items.Add(Undo_ListViewItem);
            //    listView1.EnsureVisible(listView1.Items.Count-1);
            //    fail = 1;
            //    bugcuoicung = 0;
            //}

            if (fail == 0)
            {
                Invoke(new ShowMessageDelegate(ShowMessage), "Không thể redo được được nữa !", "Thông báo"
                                                   , MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }


    }
}