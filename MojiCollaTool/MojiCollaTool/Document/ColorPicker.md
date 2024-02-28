# カラーピッカー作成

## 参考リンク集
- https://github.com/MT224244/WpfColorPicker
  - ちょっとシンプルすぎて微妙
  - しかしこれを使ってみることにする
  - 代表色リスト、使用履歴リストを追加する
  - ライセンスに注意
    -  Apache License Version 2.0
- https://araramistudio.jimdo.com/2016/10/05/wpf%E3%81%A7%E8%89%B2%E9%81%B8%E6%8A%9E%E3%82%B3%E3%83%B3%E3%83%88%E3%83%AD%E3%83%BC%E3%83%AB%E3%82%92%E8%87%AA%E4%BD%9C%E3%81%99%E3%82%8B/
- http://www.kanazawa-net.ne.jp/~pmansato/wpf/wpf_custom_ColorPicker.htm
- http://www.kanazawa-net.ne.jp/~pmansato/wpf/wpf_custom_BrushEditor.htm
- 使い勝手が良さそうだけど、少し小さいかも
- https://github.com/PixiEditor/ColorPicker
  - ものすごく作りが良いが、立派すぎて逆に使いにくい
  - 色の履歴がない
- https://github.com/xceedsoftware/wpftoolkit/wiki/ColorPicker
  - 色選択履歴あり、使いやすそう
  - スタンダードカラーの選択が良くない

## 実装メモ
色選択機能との連携処理がイマイチとなっている。
これを実装する必要がある。
即時反映機能を追加する。
カラーピッカー側の変更を受け取るのが難しい。とりあえず反映ボタンを用意する。
OKボタンを押さない限り、元の色に戻すようにする。
ForeとBackの保持機能、独自の現在色、次の色の保持もやめる。

