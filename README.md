# PDF2TIF

中身は [poppler](https://poppler.freedesktop.org/) の pdftoppm です。

## 使い方

```bat
PDF2TIF input.pdf output.tif
PDF2TIF input.pdf output{}.tif
PDF2TIF input.pdf output{}.png
PDF2TIF input.pdf output{}.jpg
PDF2TIF input.pdf output{}.ppm
```

`{}` はページ番号(1, 2, 3, ...)に置き換えます。

## 環境変数

### PDF2TIF_BPP

Bits per pixel 値。デフォルトは 24

1 = mono
8 = gray

### PDF2TIF_DPI

Dots per inch 値。デフォルトは 300
