
# Wzファイルの基本の構成は以下の通り
## Wzファイル
* Wzファイル内にはイメージノードが複数含まれており、そのイメージノード一つが一つの装備を表している  

## イメージノードの配置
* 配置方法としては、ルートノード直下パターンと、装備部位ノード配下パターンがある
    * ルートノード直下
        * Character/00002000.img
        * Wzファイル/イメージ名
        * ベースとなる体、頭など肌の色で変化がある部分がルートノード直下に置かれている
    * 装備部位ノード配下
        * Character/Accessory/01010000.img
        * Wzファイル/装備部位/イメージ名

## イメージノードの詳細
* 各イメージにはinfoノードとdefaultノードが含まれており、それぞれ以下のように情報が格納されている  
    * 00000000.img/info/各種情報(アイコン、装備性能、制約など)
    * 00000000.img/default/default(後述する動作ノードからの参照用ファイル)
    * 00000000.img/default/defaultAc(複数の場合もあり)
    * 00000000.img/backDefault/backHair(ディレクトリが複数の場合もあり)
 
 * infoノードとdefaultノードを除くイメージノード直下のノードは動作ノードで、構成は多種多様で一括りに処理が出来るものではない  
 
    ### イメージノードパターン  
    * 基本パターン
        * 「動作/フレーム番号/ファイル」
        * ファイル名は複数になる場合があり、その場合は複数の画像ファイルを組み合わせて表示する必要がある
            * シングルパターン(Accessoryなど)
                * smile/0/default
            * 組み合わせパターン(Coatなど)
                * walk1/0/mail
                * walk1/0/mailArm

    * サブフォルダ無し(複数ファイルは別パターンとして登録)
        * ルミナスの武器？属性で色が変わる？
        * Weapon/01212000.img
            * walk1/0/weapon(Wz_Png)
            * walk1/0/weapon1(Wz_Png)
            * walk1/0/weapon2(Wz_Png)
            * walk1/0/weapon3(Wz_Png)

    * サブフォルダ有り(サブフォルダ内の画像も統合して表示)…③
        * Hairがこれにあたる
            * default/hairOverHead
            * default/hair
            * default/hairShade/0(Wz_Png hairShadeノード配下にWz_Pngノードがある)
            * default/hairShade/1(Wz_Uol 同上)
            * default/hairShade/2(Wz_Uol 同上)
    
    * サブフォルダ有り(サブフォルダ内は無視)
        * サブフォルダが含まれているのはWzファイル作成時の手違いのパターン(たぶん)
        * 例外中の例外
        * 例外ノードを無視する
        * Accessory/01022226.img/の場合
            * default/default(Wz_Png)
            * default/head(Wz_Uol 余計)
            * default/ear(Wz_Uol 余計)
        * Cap/01002338.img/の場合
            * stabT1/0/default(Wz_Uol)
            * stabT1/1/default(Wz_Uol)
            * stabT1/2/default(Wz_Uol)
            * stabT1/stabT2/0/default(Wz_Uol 余計)
            * stabT1/stabT2/1/default(Wz_Uol 余計)
            * stabT1/stabT2/2/default(Wz_Uol 余計)
    
    * サブフォルダ有り(サブフォルダ内は別パターンとして登録)
        * Cap/01004090.img/night
            * nightが一つのimgのような構成になっている
                * night/info/icon
                * night/walk1/0/default
                * night/walk1/1/default
                * night/walk1/2/default
                * night/walk1/3/default
    * フレーム番号と同列にWz_Uol有り
        * 00002000.img/mesoRed/
            * 15/body (Wz_Png)
            * 15/delay
            * 15/face
            * 16(Wz_Uol)
            * 17(Wz_Uol)

## メモ
* NodesがNodeコレクション
* NodeのValueがNullのものはディレクトリノード
* Wz_UolはWz_Pngの場合、ディレクトリノードの場合がある