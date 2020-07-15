unity项目在xcode上发生错误
“Undefined symbols for architecture arm64”
原因：svn默认忽略 *.a文件，使用svn进行版本控制时丢失了第三方插件的.a文件，导致此错误

问题：移动端使用本地png变得模糊

解决方案：更改接受图片字节流的texture2d类型：如 Texture2D texture = new Texture2D(800, 640, TextureFormat.BGRA32, false);