using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace snake
{
    public partial class Form1 : Form
    {
        //地圖元素
        Label[,] lab;
        List<Label> lablist = new List<Label>();//蛇身用list<>,動態陣列
        System.Windows.Forms.Timer time;
        Color backcolor = Color.FromArgb(50, 50, 50);
        Color snakecolor = Color.FromArgb(0, 255, 255);
        //---此為可自訂區塊-------
        int fps = 5;
        int w = 10, h = 10;
        int size = 20;
        //-----------------------
        Random ran = new Random();
        //snake
        int x = 0, y = 0;
        int dx = 1, dy = 0;
        int snake_length = 2;
        //food
        int fx, fy;
        int press = 0;
        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(w * size+16, h * size+39);
            //Add keydown
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(Form1_KeyDown);
            //Add timer
            time = new System.Windows.Forms.Timer();
            time.Tick += new System.EventHandler(time_Tick);
            time.Interval = 1000 / fps;
            //Create map
            lab = new Label[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    lab[i, j] = new Label();
                    lab[i, j].AutoSize = false;
                    lab[i, j].Size = new Size(size, size);
                    lab[i, j].Name = i.ToString() + "," + j.ToString();//set id;
                    lab[i, j].BorderStyle = BorderStyle.FixedSingle;
                    lab[i, j].BackColor = backcolor;
                    lab[i, j].Location = new Point(i * size, j * size);
                    this.Controls.Add(lab[i, j]);
                }
            }
            main();//initialize
        }
        private void main()
        {
            //蛇身初始化
            lablist = new List<Label>();
            //地圖初始化
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    lab[i, j].BackColor = backcolor;
            //設定蛇的初始位置
            x = ran.Next(0, w); y = ran.Next(0, h);
            //建構蛇身
            for (int i = 0; i < snake_length; i++) lablist.Add(lab[x, y]);
            //建構食物
            getfood();
            //start game
            time.Start();
        }

        private void time_Tick(object sender, EventArgs e)//Snake Update
        {
            //move head
            x += dx; y += dy;
            //檢查有沒有超過邊界
            if (x < 0) x = w - 1; if (x > w - 1) x = 0;
            if (y < 0) y = h - 1; if (y > h - 1) y = 0;
            //檢查頭是否撞到身體
            for (int i = 0; i < lablist.Count - 1; i++)
            {
                if (lab[x, y].Name == lablist[i].Name)
                {
                    time.Stop();
                    MessageBox.Show("Game Over\r\nPress \"R\" to restart");
                    return;
                }
            }
            //檢查是否吃到食物,如果吃到蛇身++,並產生新食物
            if (x == fx && y == fy)
            {
                lablist.Add(lab[x, y]);
                getfood();
            }
            //移動尾巴
            lablist.Add(lab[x, y]);
            lablist[0].BackColor = backcolor;
            lablist.RemoveAt(0);
            foreach (var s in lablist) s.BackColor = snakecolor;
            //以上動作皆完成後,取消案件鎖定
            press = 0;
        }
        private void getfood()//取得食物
        {
            //先產生一次食物
            fx = ran.Next(0, w); fy = ran.Next(0, h);
            //檢查是否產生在蛇的身上,如果是重新產生
            for (int i = 0; i < lablist.Count; i++)
            {
                if (lablist[i] == lab[fx, fy])
                {
                    fx = ran.Next(0, w); fy = ran.Next(0, h); i = 0;
                }
            }
            //設置食物
            lab[fx, fy].BackColor = Color.FromArgb(255, 0, 0);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //簡易防鬼鍵,在移動前只能按一次上下左右,防止迴轉按太快吃到自己
            if (e.KeyValue>=36 && e.KeyValue <= 40) press++;//如果按下上下左右 press+1
            if (e.KeyValue >= 36 && e.KeyValue <= 40 && press > 1) return;//只要按過,且還沒移動完成前,都不動作
            switch (e.KeyCode)
            {
                //垂直移動時只能按左or右,平行移動時只能按上or下
                case Keys.Up:
                    if (dy == 0) { dx = 0; dy = -1; }
                    break;
                case Keys.Down:
                    if (dy == 0) { dx = 0; dy = 1; }
                    break;
                case Keys.Left:
                    if (dx == 0) { dx = -1; dy = 0; }
                    break;
                case Keys.Right:
                    if (dx == 0) { dx = 1; dy = 0; }
                    break;
                case Keys.R:
                    time.Stop();
                    main();
                    break;
            }
        }
    }
}
