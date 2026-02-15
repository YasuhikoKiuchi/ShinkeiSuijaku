namespace ShinkeiSuijaku
{
    public partial class Form1 : Form
    {
        // -------------------------------------------------- 前編(1)

        private Card[,] _cells = new Card[13, 4]; // 「場」を表す配列変数

        public Form1()
        {
            InitializeComponent();

            Text = "神経衰弱"; // タイトル設定
            BackColor = Color.Black; // 背景を黒にする
            SetBounds(0, 0, 64 * 13 + 17, 88 * 4 + 40, BoundsSpecified.Size); // フォームのサイズを適当に設定する
            DoubleBuffered = true; // ちらつきを抑えるため、ダブルバッファにする

            StartPosition = FormStartPosition.CenterScreen; // 【さらに追加】画面中央に毎回表示されるようにする

            Paint += Form1_Paint;
            MouseClick += Form1_MouseClick; // 【追加2】マウスクリック紐づけ

            _timer.Interval = 1000; // 【後編追加】
            _timer.Tick += new EventHandler(timer_tick); // 【後編追加】

            StartNewGame(); // 新規ゲームを開始する
        }

        private void StartNewGame() // 新規ゲームを開始する
        {
            Arrange(); // カードを並べる
            Shuffle(); // 【追加】カードをシャッフルする
            Refresh(); // 【追加】画面を再描画する(念のため)
        }

        private void Arrange() // カードを並べる
        {
            for (int x = 0; x < 13; x++) // 横13マスの繰り返し
            {
                for (int y = 0; y < 4; y++) // 縦4マスの繰り返し
                {
                    int num = x + 1; // 番号はX座標に1を足したものにする
                    _cells[x, y] = new Card(num, false); // 「場」の要素をセットする【変更】第2引数は false にする
                }
            }
        }

        private void Form1_Paint(object? sender, PaintEventArgs e) // フォーム描画処理
        {
            DrawCells(e.Graphics); // 「場」を描く
        }

        private void DrawCells(Graphics g) //「場」を描く
        {
            for (int x = 0; x < 13; x++) // 横13マスの繰り返し
            {
                for (int y = 0; y < 4; y++) // 縦4マスの繰り返し
                {
                    if (_cells[x, y] != null) // カードが存在する状態であれば
                    {
                        if (_cells[x, y].Shown) // 表の場合
                        {
                            //g.FillRectangle(Brushes.White, x * 64 + 1, y * 88 + 1, 62, 86); // 白い塗りつぶしの四角を描く // 【後編変更】コメントアウト
                            //g.DrawString($"({x + 1},{y + 1})\n{_cells[x, y].Number}", new Font("MS ゴシック", 16), Brushes.Black, x * 64 + 2, y * 88 + 2); // 座標と番号を描く // 【後編変更】コメントアウト
                            Brush b = new SolidBrush(CardColors[_cells[x, y].Number - 1]); // 【後編追加】描画色を取得する
                            g.FillRectangle(b, x * 64 + 1, y * 88 + 1, 62, 86); // 【後編追加】取得した描画色で塗りつぶしの四角を描く
                        }
                        else // 裏の場合
                        {
                            g.FillRectangle(Brushes.Gray, x * 64 + 1, y * 88 + 1, 62, 86); // グレーの塗りつぶしの四角を描く
                        }
                    }
                }
            }
        }

        // -------------------------------------------------- 前編(2)

        private Random random = new Random(); // 乱数発生オブジェクト

        private void Shuffle() // シャッフルする
        {
            for (int i = 0; i < 1000; i++) // とりあえず1000回くらい混ぜる
            {
                int x1 = random.Next(0, 13); // 1箇所目のX座標を決める
                int y1 = random.Next(0, 4);  // 1箇所目のY座標を決める
                int x2 = random.Next(0, 13); // 2箇所目のX座標を決める
                int y2 = random.Next(0, 4);  // 2箇所目のY座標を決める

                Card card1 = _cells[x1, y1]; // 1箇所目のカードを取得
                Card card2 = _cells[x2, y2]; // 2箇所目のカードを取得
                _cells[x1, y1] = card2; // 1箇所目のカードを2箇所目の位置に置く
                _cells[x2, y2] = card1; // 2箇所目のカードを1箇所目の位置に置く
            }
        }

        // -------------------------------------------------- 前編(3)

        private void Form1_MouseClick(object? sender, MouseEventArgs e) // マウスクリック処理
        {
            if (_timer.Enabled) return; // 【後編追加】タイマーが既に開始していた(裏返し待ち)ら、何もせずreturn

            if (_shownCount == 2) return; // 【後編追加】既に2枚めくっている場合は新たにクリックされても何も起きないようにする

            int x = e.X / 64; // クリックされたX座標を64で割る
            int y = e.Y / 88; // クリックされたY座標を88で割る

            if (x < 0 || x >= _cells.GetLength(0) || y < 0 && y >= _cells.GetLength(1)) return; // カードじゃないところをクリックしていたら何も起きないようにする

            if (_cells[x, y] == null || _cells[x, y].Shown) return; // 【後編追加】既に除去されているマスや既にめくっているカードを再度クリックしても何も起きないようにする

            _cells[x, y].Shown = !_cells[x, y].Shown; // 表返りフラグを立てる
            Refresh(); // 画面再描画

            // === 【後編追加】===
            _shownCardsXY[_shownCount] = new Point(x, y); // 表返したカードの座標を控える
            _shownCount++; // めくったカードの枚数を+1する
            if (_shownCount == 2) // 表返したカードの数が2枚になったら
            {
                _timer.Start(); // タイマースタート
            }
        }

        // -------------------------------------------------- 後編[1]

        private int _shownCount = 0; // 【後編追加】めくった枚数
        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer(); // 【後編追加】タイマー
        private Point[] _shownCardsXY = new Point[2]; // 【後編追加】めくったカードの座標の記録
        private int _getCards = 0; // 【後編追加】取得したカードの数

        private void timer_tick(object? sender, EventArgs e) // 【後編追加】タイマーTICK時処理
        {
            _timer.Stop(); // タイマー停止(繰り返し実行されてしまわないようにする)
            _shownCount = 0; // めくったカードの数を0に戻す
            Card card1 = _cells[_shownCardsXY[0].X, _shownCardsXY[0].Y];
            Card card2 = _cells[_shownCardsXY[1].X, _shownCardsXY[1].Y];
            if (card1.Number == card2.Number) // 1枚目と2枚目の数値が一致していたら
            {
                _cells[_shownCardsXY[0].X, _shownCardsXY[0].Y] = null; // 1枚目のカードを_cellsから除去する(nullにする)
                _cells[_shownCardsXY[1].X, _shownCardsXY[1].Y] = null; // 2枚目のカードを_cellsから除去する
                _getCards += 2; // 取得したカードの数を+2する
                if (_getCards == 52) // 取得したカードの数が52に達したら(=場のカードを全て取得できたら)
                {
                    MessageBox.Show("おめでとう！"); // おめでとうメッセージ表示
                    StartNewGame(); // メッセージボックスが閉じたら次のゲームを開始
                }
            }
            else  // 1枚目と2枚目の数値が一致していなかったら
            {
                _cells[_shownCardsXY[0].X, _shownCardsXY[0].Y].Shown = false; // 1枚目のカードのめくりフラグを降ろす
                _cells[_shownCardsXY[1].X, _shownCardsXY[1].Y].Shown = false; // 2枚目のカードのめくりフラグを降ろす
            }
            Refresh(); // 再描画
        }

        // -------------------------------------------------- 後編[2]

        private static readonly Color[] CardColors =  // 【後編追加】色の配列
        {
            Color.Blue, Color.Red, Color.Magenta, Color.Green, Color.Cyan, Color.Yellow, Color.White,
            Color.DeepSkyBlue, Color.Pink, Color.Purple, Color.LightGreen, Color.LightCyan, Color.LightYellow
        };
    }
}
