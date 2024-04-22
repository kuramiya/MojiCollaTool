# 文字の装飾処理について

## 必要な処理一覧
- 文字の縁取り
- 文字の縁取りのぼかし
- 文字背景の色付（箱）

## 文字の縁取り

### 参考リンク集
- https://pikopiko.chikuwasoft.com/page/note/11/
- https://github.com/Hondarer/OutlineText
- https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/how-to-create-text-with-a-shadow?view=netframeworkdesktop-4.8#using-a-blur-effect
- https://atmarkit.itmedia.co.jp/ait/articles/1102/02/news100_2.html
- 
### 実装メモ
https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/how-to-create-outlined-text?view=netframeworkdesktop-4.8
テキストをGeometryオブジェクトに変換して、それに対して処理を加える、という流れになる。
Geometryオブジェクトは図形を描画するもの。文字を図形に変更してから処理をする。
FormattedTextからGeometryオブジェクトに変換する。
FormattedTextはMarginなどを持たず、そのままレイアウトに配置できない。注意。DrawingContextにアクセスする必要がある。
これにはカスタムユーザーコントロール、つまり独自のユーザーコントロールを作成してそこで実装させる。
サンプルコードとしてOutlineTextControlというものが用意されているが、専用のプロパティやバインディングなどがあるのでそれを削除した専用クラスを作成する。
https://github.com/dotnet/docs-desktop/blob/main/dotnet-desktop-guide/samples/snippets/csharp/VS_Snippets_Wpf/OutlineTextControlViewer/CSharp/OutlineTextControl.cs

BuildHighlightGeometryを使用すると、その文字の外縁の箱のGeometryオブジェクトを手に入れることができる。
なにかに使えるかもしれない。
https://learn.microsoft.com/ja-jp/dotnet/api/system.windows.media.formattedtext.buildhighlightgeometry?view=windowsdesktop-8.0

このクラスをStackPanelにて並べたところ、文字がすべて重なる形で配置された。
独自クラス故に、WidthとHeightを定義できていないのが問題らしい。
WidthとHeightはFormattedTextのものを内部的に設定することで対応可能となった。
空白文字はWidthHeighがないらしく、重なってくる。要改善。これはWidthIncludingTrailingWhitespaceを使うことで解決した。
https://learn.microsoft.com/ja-jp/dotnet/api/system.windows.media.formattedtext.widthincludingtrailingwhitespace?view=windowsdesktop-8.0

DrawingContext.DrawGeometryメソッド、fillをnullにするとfillの色が描画されない。
Penが外縁を示す、nullにすると描画されない。
https://learn.microsoft.com/ja-jp/dotnet/api/system.windows.media.drawingcontext.drawgeometry?view=windowsdesktop-8.0

## 文字の縁取りのぼかし

### 参考リンク集
- https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/how-to-create-text-with-a-shadow?view=netframeworkdesktop-4.8
- https://www.appsloveworld.com/csharp/100/1593/how-to-blur-drawing-using-the-drawingcontext-wpf
- https://github.com/microsoft/WPF-Samples/blob/main/Visual%20Layer/DrawingVisual/MyVisualHost.cs
- 
### 実装メモ
BitmapEffect関連の処理はすべて非推奨となっている。Effectを使用する必要あり。
DrawingGroupの場合でもBitmapEffectを使用している。ダメ。
EffectはDrawingContextを持つその対象まるごとにかける形になる。
文字をまるごと書けると文字全体がぼかしされるのでよくない。最悪の場合、文字本体と縁取りで分離して描画する必要があるかもしれないがやりたくはない。

DrawingVisualを経由して書いてみたが、効果が出ない。要確認。
試しに別の図形も同時に書いてみたが、効果がない。

わからないのでスタックオーバーフローに質問した。2024/02/21
https://stackoverflow.com/questions/78032415/how-to-apply-effect-inside-wpf-uielement-onrender-method

OnRenderメソッドのオーバーラードではなく、DrawingVisualをChildrenに追加する方法でエフェクトを追加することに成功した。

## 背景ボックス

### 参考リンク集

### 実装メモ
文字を配置しているStackPanelの背景色を設定する。
StackPanelのサイズはFormattedTextのサイズに依存しており、文字の縁取りサイズに追従していない。
そのため、縁取りのサイズを広げても追従してくれない。
それとは別に、任意のサイズ指定機能が欲しい。
別途パラメータを追加して対応する。Paddingを指定することで幅を広げる。
StackPanelにはPaddingがなかった。Gridに入れることで対処する。Gridもなかった。
Gridに入れて、StackPanelのMarginを操作することで対応する。

## 背景ボックスの縁取り

### 参考リンク集

### 実装メモ
上記文字の箱の親に、さらにBorderクラスを使用する。
色設定、太さのパラメータが必要。

## 背景ボックスの角丸化

### 参考リンク集
https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/graphics-multimedia/how-to-draw-a-rectangle?view=netframeworkdesktop-4.8

### 実装メモ
現状の実装は下記のようになっている。
MojiPanel(ContentControl) -> Border(背景ボックスの縁取り) -> Grid(背景ボックスの色) -> StackPanel(行並べ)

BorderにCornerRadiusというパラメータがあることを発見。
使用してみたが、背景の縁取りには有効であるが、背景箱そのものには効果がない
角丸を実装するならGrid内にRectangleを配置して、それで背景などを設定した方が良い、

## 文字入力の処理の軽量化
文字を編集するたびに、文字オブジェクトをすべて作り直しているのが問題。
文字オブジェクトの再利用を行うアルゴリズムを考え、実装する。

### 実装メモ
文字パネルの
- ①文字オブジェクトの生成処理
- ②文字オブジェクトの配置処理
これら２つを分離する。

処理が重いのは①であって、②ではないと思われる。
生成済みの文字オブジェクトをなるべく使い回すようにする。

MojiPanel -> MojiData
MojiDataオブジェクトは生成された後、再度インスタンス化されない。
MojiWindowなどにより、設定されている値が変更されるのみ。

改行処理なども考慮する必要あり。

行パネルも使いまわした方が良い？

#### 文字の装飾が変わった場合
1. ①をすべてやり直す。
1. ②を行う。

#### 文字列の変更（追加、削除、場所変更）が行われた場合
1. 文字列の各文字ごとに順に、数をカウントする
1. 足りないものがあれば、その文字オブジェクトを生成し、文字オブジェクトのプールに追加する
1. 使用していない文字があれば、その文字オブジェクトを削除する（リストから削除する）
1. 文字オブジェクトリストの中身を用いて、文字を配置していく

#### 実装アルゴリズム
文字オブジェクトを別途リストで保持しておく。

再利用のためには、「どのように文字列が変わったか、再利用できる場所はどこか？」を把握する必要がある。
差分検出のアルゴリズム？
https://qiita.com/WaToI/items/a8cb0441eb6cb200aa77

#### 文字の変化の検出方法
文字列以外の要素が変化していないことを検出するメソッドを追加する。
装飾が変化していない場合は、②のみ実行する。

#### ②文字の追加、削除、再配置処理の実装
追加、削除、再配置を検出する必要がある。
かならず末尾で実行されるわけではない。
ペースト、カットのように、まとめて追加、削除されるケースもある。

文字オブジェクトはスタックパネル内に順番に配置されている。
文字オブジェクトのインスタンス（FrameworkElementを継承している）は、複数追加することはできない。そういった再利用は不可。

#### 文字オブジェクトの管理


#### 文字オブジェクト（DecoratedCharacterControl）の識別





## 懸念点メモ
- ネット上のサンプルでは縁取りと縁取りのぼかしは別々の項目となっている
- 縁取りだけを対象にぼかしをした場合、文字とぼかされた縁取りの間に隙間ができるはず
- 縁取りの中はFillしておく必要がある
  - 輪を描いている文字の中身をきれいに埋めることができるのか？
- 横書きで英字を並べた際、ひらがなと比べて上に浮いてしまう（中央配置のため）
  - 横書きの英字の場合、縦のアライメントを下に落とすようにする？
	- 落としても効果がなかった
- 文字の新規作成の処理が遅い
- 文字のウィンドウを開くスピードが遅い