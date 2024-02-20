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

### 開発メモ
https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/how-to-create-outlined-text?view=netframeworkdesktop-4.8
テキストをGeometryオブジェクトに変換して、それに対して処理を加える、という流れになる。
Geometryオブジェクトは図形を描画するもの。文字を図形に変更してから処理をする。
FormattedTextからGeometryオブジェクトに変換する。
FormattedTextはMarginなどを持たず、そのままレイアウトに配置できない。注意。DrawingContextにアクセスする必要がある。
これにはカスタムユーザーコントロール、つまり独自のユーザーコントロールを作成してそこで実装させる。
サンプルコードとしてOutlineTextControlというものが用意されているが、専用のプロパティやバインディングなどがあるのでそれを削除した専用クラスを作成する。
https://github.com/dotnet/docs-desktop/blob/main/dotnet-desktop-guide/samples/snippets/csharp/VS_Snippets_Wpf/OutlineTextControlViewer/CSharp/OutlineTextControl.cs

このクラスをStackPanelにて並べたところ、文字がすべて重なる形で配置された。
独自クラス故に、WidthとHeightを定義できていないのが問題らしい。
WidthとHeightはFormattedTextのものを内部的に設定することで対応可能となった。
空白文字はWidthHeighがないらしく、重なってくる。

DrawingContext.DrawGeometryメソッド、fillをnullにするとfillの色が描画されない。
Penが外縁を示す、nullにすると描画されない。
https://learn.microsoft.com/ja-jp/dotnet/api/system.windows.media.drawingcontext.drawgeometry?view=windowsdesktop-8.0



ぼかし効果を使用したほうが手っ取り早いかもしれない？
しかしその場合、ぼかしのない文字の縁取りができない。


### 文字の縁取りのぼかし

#### 参考リンク集
- https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/how-to-create-text-with-a-shadow?view=netframeworkdesktop-4.8



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