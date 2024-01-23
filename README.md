# MojiCollaTool
文字コラツール

## 実装したい機能
### 必須機能
- コラ画像の読み込み
  - jpg
  - png
  - bmp

- 画面上の表示の変更
  - 拡大・縮小
  - パニング（拡大表示場所の移動）

- 文字の追加
- 文字の複製
- 文字の削除

- 文字のマウスドラッグによる移動
- 文字のダブルクリックによる設定画面呼び出し

- 文字の装飾
  - 文字の配置場所の変更
    - ドラッグによる移動
    - 位置の微調整機能
  - 横書き
  - 縦書き
  - 文字サイズの変更
  - フォントの変更
  - 書体の変更
    - 太文字（フォントファミリーはどうなるのか？）
    - イタリック文字
  - 文字間隔の変更
  - 行間隔の変更
  - 文字色の変更
  - 文字の装飾
    - 文字の縁取り
    - 縁取り幅の変更
    - 縁取り色の変更
    - 縁取りのぼかし
    - 背景色の設定、変更
    - 背景の縁取り色の変更
    - 背景の縁取り幅の変更

- 編集中のファイルの保存
  - 専用拡張子でのファイル保存

- 画像としての出力
  - 出力形式
    - jpg
    - png

### できれば欲しい機能
- 画像の色のスポイト機能（できれば）
- 文字の整列機能


## 開発環境など
- Visual Studio C# 2022
- .NET 6.0
- WPF

## UI設計
### ユーザーストーリーA
1. 画像のロード
1. 文字の追加
1. 文字の配置（移動）
1. 文字の装飾
1. 画像の出力
### ユーザーの実行環境
- Windows10, 11
  - Win7は考えない
- 画面サイズ
  - 1920 x 1080 (Full HD)以上

### デザインの要点
- 読み出し、保存などの出番はそれほど多くない
- 文字の追加、装飾処理の使い勝手が一番肝心
- 色の選択は容易にしたい
  - 色の使用履歴が欲しい

### 画面設計

#### メイン画面
##### 画面サイズ
1200 x 675 (16:9)
変更可能

- 上にツールバーを配置する
  - 画像読み出しボタン
  - プロジェクト読み出しボタン
  - プロジェクト保存ボタン
  - 画像出力ボタン
  - 文字追加ボタン

- 左パネル
  - 画像パネル
　- 文字パネルのドラッグによる操作
  - 文字パネルのクリックによる文字入力画面の表示

- 右パネル
  - 追加した文字一覧のリスト
    - ID列
    - テキスト冒頭列
    - クリックすることでその文字入力画面を表示する

#### 文字入力画面
- 上部ツールバー
  - 識別IDラベル
  - 複製ボタン
  - 削除ボタン
- 座標（X, Y）（デフォルトは0, 0）
- 入力文字テキストボックス（デフォルトは「サンプル」）
- フォント（デフォルトは未定）
- 縦書きor横書き（デフォルトは横書き）
- 文字サイズ（デフォルトは未定）
- 文字間隔（デフォルトは0）
- 行間隔（デフォルトは0）
- 文字色（デフォルトは黒）
- 文字の縁取り幅（デフォルトは5）
- 文字の縁取り色（デフォルトは白）
- 文字のぼかし（デフォルトは0）
- 文字の背景色（デフォルトは透明）
- 文字の背景縁取り幅（デフォルトは0）
- 文字の背景縁取り色（デフォルトは黒）

#### 画面設計問題点
- テキスト入力に大きな領域が欲しい、しかし、それを取ると画像が見にくくなる
  - 別画面にしたほうが良いのではないか？
  - そうなると文字入力コントロール自体を別画面にしたほうが良さそう

## 懸念点、その他
### 懸念点一覧
#### 未解決
- 画像の拡大と縮小が、画像の出力に影響しないか？
- 文字の背景色の範囲をどのように指定するか？
  - プロポーショナルフォントの影響を受けないか？
- 文字編集のクリックが文字の上でない限り反応しない
  - ContentControlのダブルクリックはそういう仕組みらしい？
- 縦書きの際、空行の処理がおかしい、半角が半角サイズとなる
#### 解決済み
- 縦書き文字をどのように実装するか？
  - かっこや伸ばし棒などを、どのように縦書き配置にするか？
   - リストを用意し、それを90度回転させることで対処する
  - プロポーショナルフォントの縦書き配置をどのようにするか？
   - 細かい配置方法は諦める
- パネル上でのダブルクリックの実装
  - ~~ダブルクリックのイベントは存在しないので、自力で実装する必要がある~~
  - ~~とりあえず普通のクリックで対処する~~
  - ContentControlを上位に配置することで解決した

### 文字の装飾処理
- ネット上のサンプルでは縁取りと縁取りのぼかしは別々の項目となっている
- 縁取りだけを対象にぼかしをした場合、文字とぼかされた縁取りの間に隙間ができるはず
- 縁取りの中はFillしておく必要がある
  - 輪を描いている文字の中身をきれいに埋めることができるのか？

#### 文字の縁取り
- https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/how-to-create-outlined-text?view=netframeworkdesktop-4.8
- https://pikopiko.chikuwasoft.com/page/note/11/
- https://github.com/Hondarer/OutlineText
#### 縁取りのぼかし
- https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/how-to-create-text-with-a-shadow?view=netframeworkdesktop-4.8

### カラーピッカー
#### 参考
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

