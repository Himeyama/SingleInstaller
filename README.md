# SingleInstaller
インストーラーを作成する環境です。

[HDS](https://github.com/himeyama/HDS) に依存します。

```
resources
├── LICENSE.md
└── TestApp-テストアプリ-ひかり-1.0.0.zip
└── HDS
    └── HDS.exe
    └── 省略...
```

のように 3 つの材料を用意します。(詳しくは、[HDS](https://github.com/himeyama/HDS) を参照)

HDS はビルド済みのもので、ディレクトリ直下に HDS.exe が配置されるような構成とします。

以下のコマンドを実行すると、publish\Setup.exe が作成されます。

```ps1
.\build
```

これがセットアップの実行ファイルです。

```
.\publish\Setup.exe
```
