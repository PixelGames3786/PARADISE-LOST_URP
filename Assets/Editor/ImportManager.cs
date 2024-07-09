using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImportProcess : AssetPostprocessor
{
    //画像のインポート時、インポート設定変更時に実行される関数
    void OnPreprocessTexture()
    {
        //assetImporterにインポートするオブジェクトが入る。それをテクスチャ型にキャスト
        TextureImporter Ti = assetImporter as TextureImporter;

        //インポート設定変更時もこのメソッドが呼ばれる都合上、一部のテクスチャだけ初期設定を無効化するには何かしらの処理が必要
        //今回はパッキングタグが何かしら設定されている場合、初期設定を行わないように設定する
        if (Ti.spritePackingTag != "")
        {
            return;
        }

        //今回は全てスプライト扱いにする
        Ti.textureType = TextureImporterType.Sprite;
        Ti.filterMode = FilterMode.Point;
        Ti.textureCompression = TextureImporterCompression.Uncompressed;

        /*
        //ここからはインポートするテクスチャがどのフォルダ内にあるかで処理を変える
        if (Ti.assetPath.Contains("Assets/Resources/Pictures/BackGround"))
        {
            Ti.spriteImportMode = SpriteImportMode.Single;
            Ti.filterMode = FilterMode.Bilinear;
            Ti.maxTextureSize = 2048;
        }
        else if (Ti.assetPath.Contains("Assets/Resources/Pictures/Character"))
        {
            Ti.spriteImportMode = SpriteImportMode.Single;
            Ti.filterMode = FilterMode.Bilinear;
            Ti.maxTextureSize = 1024;
        }
        else if (Ti.assetPath.Contains("Assets/Resources/Pictures/Dot"))
        {
            Ti.spriteImportMode = SpriteImportMode.Single;
            Ti.filterMode = FilterMode.Point;//Bilinearにすると変にぼやけてドット絵の良さが失われるので、これだけPoint
            Ti.maxTextureSize = 32;
        }
        else if (Ti.assetPath.Contains("Assets/Resources/Pictures/Effect"))
        {
            Ti.spriteImportMode = SpriteImportMode.Multiple;//スプライトエディターで画像を切り分けるので、これだけMultiple
            Ti.filterMode = FilterMode.Bilinear;
            Ti.maxTextureSize = 1024;
        }
        */
    }
}