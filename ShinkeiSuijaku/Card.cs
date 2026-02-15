namespace ShinkeiSuijaku
{
    class Card // 「場」の各要素(カード)を表現するクラス
    {
        public int Number { get; set; } = 0; // 番号

        public bool Shown { get; set; } = false; // 表になっているか否か

        public Card(int number, bool shown) // コンストラクタ
        {
            Number = number;
            Shown = shown;
        }
    }
}
