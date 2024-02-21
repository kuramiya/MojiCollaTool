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

### 実装メモ
BitmapEffect関連の処理はすべて非推奨となっている。Effectを使用する必要あり。
DrawingGroupの場合でもBitmapEffectを使用している。ダメ。
EffectはDrawingContextを持つその対象まるごとにかける形になる。
文字をまるごと書けると文字全体がぼかしされるのでよくない。最悪の場合、文字本体と縁取りで分離して描画する必要があるかもしれないがやりたくはない。

DrawingVisualを経由して書いてみたが、効果が出ない。要確認。

わからないのでスタックオーバーフローに質問した。
https://stackoverflow.com/questions/78032415/how-to-apply-effect-inside-wpf-uielement-onrender-method



## 文字の背景描画

### 参考リンク集

### 実装メモ
文字を配置しているStackPanelの背景色を設定する。
StackPanelのサイズはFormattedTextのサイズに依存しており、文字の縁取りサイズに追従していない。
そのため、縁取りのサイズを広げても追従してくれない。
それとは別に、任意のサイズ指定機能が欲しい。
別途パラメータを追加して対応する。Paddingを指定することで幅を広げる。
StackPanelにはPaddingがなかった。Gridに入れることで対処する。Gridもなかった。
要検討。

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