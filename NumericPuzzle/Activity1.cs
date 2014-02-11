using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Media;

namespace NumericPuzzle
{
    [Activity(Label = "NumericPuzzle", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        //MediaPlayer player = null;

        static private readonly int[] imageButtons =
        {
            Resource.Id.image_button1,
            Resource.Id.image_button2,
            Resource.Id.image_button3,
            Resource.Id.image_button4,
            Resource.Id.image_button5,
            Resource.Id.image_button6,
            Resource.Id.image_button7,
            Resource.Id.image_button8,
            Resource.Id.image_button9,
            Resource.Id.image_button10,
            Resource.Id.image_button11,
            Resource.Id.image_button12,
            Resource.Id.image_button13,
            Resource.Id.image_button14,
            Resource.Id.image_button15,
            Resource.Id.image_button16
        };

        static private readonly int[] numImages = 
        {
            Resource.Drawable.num1,
            Resource.Drawable.num2,
            Resource.Drawable.num3,
            Resource.Drawable.num4,
            Resource.Drawable.num5,
            Resource.Drawable.num6,
            Resource.Drawable.num7,
            Resource.Drawable.num8,
            Resource.Drawable.num9,
            Resource.Drawable.num10,
            Resource.Drawable.num11,
            Resource.Drawable.num12,
            Resource.Drawable.num13,
            Resource.Drawable.num14,
            Resource.Drawable.num15,
            Resource.Drawable.blank
        };

        private bool gameStarted = false;

        OrderController[] orders = new OrderController[imageButtons.Length];

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            createOrderController();
            setStartButtonListener();
        }
        private void createOrderController()
        {
            for (int i = 0; i < imageButtons.Length; ++i)
            {
                ImageButton imgbtn = FindViewById<ImageButton>(imageButtons[i]);
                orders[i] = new OrderController(imgbtn, i, numImages[i], this);

                var num = i;

                imgbtn.Click += (s, e) =>
                    {
                        if (imgbtn.Id == Resource.Drawable.blank)
                        {
                            return;
                        }
                        searchDir(num);

                        if (isCompleted())
                        {
                            complete();
                        }
                    };
            }
        }

        private void setStartButtonListener()
        {
            Button btn = FindViewById<Button>(Resource.Id.start_button);
            btn.Click += (s, e) =>
                {
                    startGame();
                    startChronometer();
                };
        }

        void btn_Click(object sender, EventArgs e)
        {
            startGame();
            startChronometer();
        }
        private void startChronometer()
        {
            Chronometer chrono = FindViewById<Chronometer>(Resource.Id.chronometer);
            chrono.Base = SystemClock.ElapsedRealtime();
            chrono.Start();
        }
        private void startGame()
        {
            int size = numImages.Length;

            int max = 300;
            for (int i = 0; i < size - 2; ++i)
            {
                for (int j = 0; j < max; j++)
                {
                    int seed = System.Environment.TickCount;
                    var rnd = new Random(seed);

                    int position = (int)(rnd.Next(size - (i + 1)));
                    this.searchDir(position);
                }
            }
            //this.player = MediaPlayer.Create(this, Resource.Drawable.start);
            //this.player.start();
            gameStarted = true;
        }
        private bool isCompleted()
        {
            if (!(gameStarted))
            {
                return false;
            }
            for (int i = 0; i < numImages.Length; ++i)
            {
                if (numImages[i] != orders[i].getImageRes())
                    return false;
            }
            return true;
        }

        private long stopChronometer() {
            Chronometer chrono = FindViewById<Chronometer>(Resource.Id.chronometer);
            chrono.Stop();
            return SystemClock.ElapsedRealtime() - chrono.Base;
        }

        private void complete() {
            //this.player = MediaPlayer.Create(this, Resource.Drawable.end);
            //this.player.Start();

            long msec = stopChronometer();
            AlertDialog.Builder alertDlgBld = new AlertDialog.Builder(this);
            alertDlgBld.SetTitle(Resource.String.complete_title);
            alertDlgBld.SetMessage(msec/1000 + " 秒");
            alertDlgBld.Show();
        }



        private void searchDir(int idx)
        {
            bool searchRight = true;
            bool searchLeft = true;
            bool searchUp = true;
            bool searchDown = true;

            if (idx < 4)
            {
                searchUp = false;
            }
            if (idx > 11)
            {
                searchDown = false;
            }
            if ((idx % 4) == 0)
            {
                searchLeft = false;
            }
            if ((idx % 4) == 3)
            {
                searchRight = false;
            }
            if (searchUp)
            {
                if (this.searchUp(idx))
                {
                    return;
                }
            }
            if (searchDown)
            {
                if (this.searchDown(idx))
                {
                    return;
                }
            }
            if (searchLeft)
            {
                if (this.searchLeft(idx))
                {
                    return;
                }
            }
            if (searchRight)
            {
                if (this.searchRight(idx))
                {
                    return;
                }
            }
        }

        private bool searchUp(int idx)
        {
            int distance = 0;
            //Log.v("SearchUp","idx=" + idx);
            for (int i = idx - 4; i > -1; i -= 4)
            {
                distance--;
                if (orders[i].getImageRes() == Resource.Drawable.blank)
                {
                    swapUp(idx, distance);
                    return true;
                }
            }
            return false;
        }
        private bool searchDown(int idx)
        {
            int distance = 0;
            //Log.v("SearchDown","idx=" + idx);
            for (int i = idx + 4; i < 16; i += 4)
            {
                distance++;
                if (orders[i].getImageRes() == Resource.Drawable.blank)
                {
                    swapDown(idx, distance);
                    return true;
                }
            }
            return false;
        }
        private bool searchLeft(int idx)
        {
            int distance = 0;
            int min = 0;
            //Log.v("SearchLeft","idx=" + idx);
            min = idx - (idx % 4);
            for (int i = idx - 1; i >= min; i--)
            {
                distance--;
                if (orders[i].getImageRes() == Resource.Drawable.blank)
                {
                    swapLeft(idx, distance);
                    return true;
                }
            }
            return false;
        }
        private bool searchRight(int idx)
        {
            int distance = 0;
            int max = 15;
            //Log.v("SearchRight","idx=" + idx);
            max = (idx + 4) - (idx + 4) % 4;
            for (int i = idx + 1; i < max; i++)
            {
                distance++;
                if (orders[i].getImageRes() == Resource.Drawable.blank)
                {
                    swapRight(idx, distance);
                    return true;
                }
            }
            return false;
        }
        private void swapUp(int idx, int distance)
        {
            for (int i = idx + (distance * 4); i < idx; i += 4)
            {
                orders[i].swapImage(orders[i + 4]);
            }
        }
        private void swapDown(int idx, int distance)
        {
            for (int i = idx + (distance * 4); i > idx; i -= 4)
            {
                orders[i].swapImage(orders[i - 4]);
            }
        }
        private void swapLeft(int idx, int distance)
        {
            for (int i = idx + distance; i < idx; i++)
            {
                orders[i].swapImage(orders[i + 1]);
            }
        }
        private void swapRight(int idx, int distance)
        {
            for (int i = idx + distance; i > idx; i--)
            {
                orders[i].swapImage(orders[i - 1]);
            }
        }

        class OrderController
        {
            private ImageButton imgBtn;
            private int idx = 0;
            private int curImageId = 0;
            private Activity1 parent;

            public OrderController(ImageButton ibtn, int i, int resid, Activity1 parent)
            {
                imgBtn = ibtn;
                idx = i;
                setImageRes(resid);
                this.parent = parent;
            }
            public int setImageRes(int resid)
            {
                int old = curImageId;
                curImageId = resid;
                imgBtn.SetImageResource(resid);
                return old;
            }
            public int getImageRes()
            {
                return curImageId;
            }

            public void swapImage(OrderController other)
            {
                int previous = other.setImageRes(curImageId);
                setImageRes(previous);
            }
        }
    }
}

