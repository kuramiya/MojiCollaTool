# MojiCollaTool
文字コラツール

## 実装したい機能
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

- 文字の装飾
  - 文字の配置場所の変更
    - ドラッグによる移動
    - 位置の微調整機能
    - 整列機能（あとでいい）
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

- 画像の色のスポイト機能（できれば）

- 編集中のファイルの保存
  - 専用拡張子でのファイル保存

- 画像としての出力
  - 出力形式
    - jpg
    - png

## 開発環境など
- Visual Studio C#
- .NET
- WPF

## UI設計
### ユーザーストーリーA
1. 画像のロード
1. 文字の追加
1. 文字の装飾
1. 画像の出力
### ユーザーストーリーB
1. 画像のロード
1. 文字の追加
1. 文字の装飾
1. 作業中ファイルの保存
1. 作業中ファイルのロード
1. （作業）
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
- 左右2ペイン構成にする
- 左のペインに画像、文字（出力画像イメージ）を表示する
- 右のペインに選択中の文字の情報を表示する
#### 文字情報ペイン表示内容
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

## 懸念点
- 画像の拡大と縮小が、画像の出力に影響しないか？
- 縦書き文字をどのように実装するか？
  - かっこや伸ばし棒などを、どのように縦書き配置にするか？
  - プロポーショナルフォントの縦書き配置をどのようにするか？
- 文字の背景色の範囲をどのように指定するか？
  - プロポーショナルフォントの影響を受けないか？


