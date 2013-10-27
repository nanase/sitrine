Sitrine
=======

Sitrine は OpenTK を利用した C# の 2D ゲーム補助ライブラリです。

## 特徴

* __C# を利用したクロスプラットフォームライブラリ__  
    Windows では .NET Framework、その他の環境では Mono を使用することにより
    同一のバイナリで複数のプラットフォームで動作可能です。

* __OpenTK の利点を活かした補助ライブラリ__  
    Sitrine は OpenTK の補助として機能します。文字の描画、テクスチャの読み込み、
    軽量シンセサイザ ux など各種のライブラリを提供します。

* __シナリオの記述がそのまま動作になるストーリーボード機能__   
    ストーリーボードを用いると、思い描いたシナリオをそのまま処理として記述でき、
    実行できます。アニメーション機能も搭載しており、
    面倒な記述をする必要がありません。


## ストーリーボード

ストーリーボードとは絵コンテのことです。Sitrine では記述したコード内容がそのまま
実行され処理されます。例えば、画像を表示させて 1 秒待ち、その後
画像を削除したいならば以下のようなコードが考えられます。

``` csharp
int textureId = 0:
Texture.Create(textureId, "image/sample.jpg");
Process.Wait(1.0);
Texture.Clear(textureId);
```

`Create` や `Wait` などのメソッドをストーリーイベント、
または単にイベントと呼称しています。

これらのイベントは（もちろん 1 行目の変数宣言を除き）すべて遅延実行されます。
この例の場合、画像は命令の実行時にロードされますが、
実際に表示される処理が行われるのは後々の画面描画時です。

実際にストーリーボードを使用するには、Storyboard クラスを継承しコンストラクタに
イベントを記述していきます。

``` csharp
class SampleStory : Storyboard
{
    public SampleStory(SitrineWindow window)
        : base(window)
    {
        Screen.BackgroundColor = Color.FromArgb(10, 59, 118);
        Screen.ForegroundColor = Color.Black;
        Screen.AnimateForegroundColor(Color.FromArgb(0, Color.Black), 1.0);
    }
}
```

`Animate` という名前で始まるイベントはそのプロパティに関してのアニメーションが
実行できます。この例では、前景色を黒から透明にフェードインしていくものです。

`BackgroundColor` および `ForegroundColor` はプロパティですが、
これらも遅延実行され、実際に実行されるときに値が設定されます。


## 参考文献・使用ライブラリ

参考文献や使用ライブラリに関して、各々使用部分に注釈で記述してあります。

* __OpenTK__  
    ベースライブラリとして広範に利用。OpenGL、OpenAL 部分を主に使用。  
    ライセンス:  [MIT Lisence](http://www.opentk.com/project/license)  
    Copyright (c) 2006 - 2013 Stefanos Apostolopoulos (stapostol@gmail.com)
    for the Open Toolkit library.  
    http://www.opentk.com/

* __jQuery__  
    イージング関数の実装。  
    ライセンス: [MIT Lisence](https://jquery.org/license/)  
    Copyright 2013 jQuery Foundation and other contributors  
    http://jquery.com/

* __ux__  
    シンセサイザに使用。MIDI 読み込み部分に使用。  
    ライセンス: [MIT Lisence](https://github.com/nanase/ux/blob/master/LICENSE)  
    Copyright (c) 2013 Tomona Nanase.


## ライセンス

このプロジェクトを __MIT Lisence__ によって公開します。以下の著作権表記の表示と
LISENCE ファイルの付属によって無制限に使用することができます。

Copyright (c) 2013 Tomona Nanase


## 制作者

* __七瀬 朋奈 (Tomona Nanase)__  
[http://nanase.cc/](http://nanase.cc/)  
[https://github.com/nanase/](https://github.com/nanase/)