# CM3D2.RemoteControl.Plugin
メイドさんエディット画面(level5)においてTCPサーバを起動するUnitiyInjectorプラグインです。ネットワーク経由でコマンドを送信することで、ポーズや表情を変更したり、セリフや効果音を再生できるようにします。  
This UnitiyInjector plugin invokes a TCP server in maid editing screen (level5). Send any commands via network, she will change her pose and face, speaking, or play sound-effect.  

## 自前でコンパイル
[neguse11 さまのネットワークインストーラーもどき](https://github.com/neguse11/cm3d2_plugins_okiba)に居候しています。
`cm3d2_plugins_okiba-master`フォルダの下に`src`ディレクトリを置いて`(ゲームのインストールパス)/cm3d2_plugins_okiba-master/src/compile.bat`を実行してください。 

## 使い方
0. 下準備
  1. `(ゲームのインストールパス)/UnityInjector`以下にCM3D2.RemoteControl.Plugin.dllをコピーします
  2. Windowsファイヤウォールからプログラムレベルあるいはポートレベルのブロックを解除します
1. CM3D2を起動してゲームをプレイ開始、メイド管理メニュー→エディットメニューと進んで、メイドさんのエディット画面を開きます
2. PuTTYなどのTCP/IP接続が可能なクライアントから、ポート9000に接続します
  * PuTTY を使う場合、Raw接続を選択してください。TelnetやSSH接続では、接続時にネゴシエーションのための文字列が送信されて動作しません。
  * [TCP/IPテストツール](http://www.vector.co.jp/soft/dl/winnt/net/se411272.html)がそのまま使えます。
3. コマンド文字列を送信します
  * 改行ごとにコマンドが実行されます
  * クライアント側からのみ切断できます

### 例
* 画面の焼き付きを防が**ない**スクリーンセーバー
* メールが届いたら嫁がイク
* ルータのポートに穴を開けて世界中から嫁がNTRプレイ

## コマンド仕様
* `<キー名>=<値>`のペアを1つ以上の空白で区切って列挙します。
* `<キー名>`は大文字小文字を区別しません。
* `<値>`は大文字小文字を区別します。
* 各ペアの順序は今のところ任意です。
```
MOTION=jump_s FACE=にっこり    FACE_BLEND=頬０涙１
```
* 動作確認を目的にいい加減に決めた部分が大きいので、**今後ガッツリ仕様変更される可能性がすっごい高い**です。
* そのため、現時点では「お～動く動く」程度でお使いください
* コマンド仕様が決まらないとクライアントも作りにくいと思うので、激しくご意見募集中です。
* もうだいぶ過去のものになりましたが、活発なコミュニティで多数のクライアントが生まれた[伺か。](http://usada.sakura.vg/contents/sstp.html)界隈のSSTP仕様に準拠して、既存のクライアントを~~タダ乗り~~再利用したい。クライアントの再開発ってしんどいですし。

### MOTION キー
* 変更するポーズのファイル名を指定します。
* `.anm`拡張子は不要です。
* 存在しないファイル名を指定した場合、エラー ダイアログが表示されます。
* 指定できるファイル名は、`GameData/motion.arc`を展開してみてください。

```
MOTION=jump_s
MOTION=turn01
```

### FACE キー
* 変更する表情名を指定します。
  * 通常
  * あーん
  * きょとん
  * ためいき
  * にっこり
  * びっくり
  * ぷんすか
  * まぶたギュ
  * むー
  * エロフェラ愛情
  * エロフェラ快楽
  * エロフェラ嫌悪
  * エロフェラ通常
  * エロメソ泣き
  * エロ愛情２
  * エロ我慢１
  * エロ我慢２
  * エロ我慢３
  * エロ期待
  * エロ怯え
  * エロ興通常３
  * エロ興奮０
  * エロ興奮１
  * エロ興奮２
  * エロ興奮３
  * エロ緊張
  * エロ嫌悪１
  * エロ好感１
  * エロ好感２
  * エロ好感３
  * エロ絶頂
  * エロ舌責
  * エロ舌責快楽
  * エロ痛み１
  * エロ痛み２
  * エロ痛み３
  * エロ痛み我慢
  * エロ痛み我慢２
  * エロ痛み我慢３
  * エロ通常１
  * エロ通常２
  * エロ通常３
  * エロ放心
  * エロ羞恥１
  * エロ羞恥２
  * エロ羞恥３
  * エロ舐め愛情
  * エロ舐め愛情２
  * エロ舐め快楽
  * エロ舐め快楽２
  * エロ舐め嫌悪
  * エロ舐め通常
  * ジト目
  * ダンスウインク
  * ダンスキス
  * ダンスジト目
  * ダンス困り顔
  * ダンス真剣
  * ダンス微笑み
  * ダンス目とじ
  * ダンス憂い
  * ダンス誘惑
  * ドヤ顔
  * 引きつり笑顔
  * 疑問
  * 泣き
  * 居眠り安眠
  * 興奮射精後１
  * 興奮射精後２
  * 苦笑い
  * 困った
  * 思案伏せ目
  * 少し怒り
  * 照れ
  * 照れ叫び
  * 笑顔
  * 接吻
  * 絶頂射精後１
  * 絶頂射精後２
  * 恥ずかしい
  * 痛み３
  * 痛みで目を見開いて
  * 通常射精後１
  * 通常射精後２
  * 怒り
  * 発情
  * 悲しみ２
  * 微笑み
  * 閉じフェラ愛情
  * 閉じフェラ快楽
  * 閉じフェラ嫌悪
  * 閉じフェラ通常
  * 閉じ目
  * 閉じ舐め愛情
  * 閉じ舐め快楽
  * 閉じ舐め快楽２
  * 閉じ舐め嫌悪
  * 閉じ舐め通常
  * 目を見開いて
  * 目口閉じ
  * 優しさ
  * 誘惑
  * 余韻弱
  * 拗ね
* 上記以外の指定をした場合、表情は変化しませんが、裏側でデバッグ ログにがっつりエラーが残ります
* 文字列はUTF-8エンコードで送信してください
 
```
FACE=通常
FACE=にっこり
```

### FACE_BLEND キー
* 表情の追加要素を指定します。
* 頬の紅潮を`頬０`、`頬１`、`頬２`、`頬３`で指定します。
* 涙を`涙０`、`涙１`、`涙２`、`涙３`で指定します。
* よだれを垂らす場合、`よだれ`を指定します。
* これらの文字列を上から順に結合します。
* 上記以外の指定をした場合、表情は変化しませんが、裏側でデバッグ ログにがっつりエラーが残ります
* 文字列はUTF-8エンコードで送信してください

```
FACE_BLEND=頬０涙０
FACE_BLEND=頬３涙３よだれ
```

### VOICE キー
* セリフを一度だけ再生します。
* `.ogg`拡張子は必要です。
* 存在しないファイル名を指定した場合、再生されず、デバッグ ログにエラーが記録されます。
* 指定できるファイル名は、`GameData/voice*.arc`を展開してみてください。
* キャラの性格付けに関係なく、指定したセリフを再生します。そのため`n0_*.ogg`を指定すると秘書メイドさんのセリフになります。

```
VOICE=n0_00001.ogg
VOICE=s2_04008.ogg
```

### SE キー
* 効果音を一度だけ再生します。
* `.ogg`拡張子は必要です。
* 存在しないファイル名を指定した場合、再生されず、デバッグ ログにエラーが記録されます。
* 指定できるファイル名は、`GameData/voice*.arc`を展開してみてください。

```
SE=se009.ogg
SE=se022.ogg
```

## 未実装/今後の野望
* コマンド：表情変化時のduration指定
* コマンド：背景画像の変更、移動、回転の制御
* コマンド：カメラの移動、回転、ズームの制御
* 要調査：効果音再生時のループ指定
* 要調査：リソース外の任意の音声/音楽ファイルの再生
* ウィンドゥを開いてテキストメッセージを表示
  * 収録された音声以外にメッセージを伝える手段がない
* 3Dモデルの関節制御
  * 外部プログラムで[MikuMikuDance](http://www.geocities.jp/higuchuu4/)のデータを読み込んで嫁を躍らせる
  * Kinectなどでモーションキャプチャして嫁になりきりプレイ

## いろいろ
* 原型をとどめていませんが、[TbGUP4660氏のChangeMotion.Plugin](http://seesaawiki.jp/cm3d2/d/%b2%fe%c2%a4#content_7_23)を相当に参考にさせて頂きました。ありがとうございます。
