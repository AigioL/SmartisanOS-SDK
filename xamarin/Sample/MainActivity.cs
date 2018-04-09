using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.View;
using Android.Views;
using Android.Widget;

using Java.IO;
using Java.Lang;
using SmartisanOS.API;
using SmartisanOS.Util;

using R = Sample.Resource;
using Stream = System.IO.Stream;

namespace Sample
{
    [Activity(Label = "Sample", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private const string DRAG_IMG_FILE_NAME = "033202yw7e3lawlczeua7d.jpg";
        private static readonly string SAMPLE_FILE_DIR = Environment.ExternalStorageDirectory + "/OneStepSample/";

        private OneStepHelper mOneStepHelper;
        private TextDragPopupWindow mTextDragPopupWindow;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.activity_main);
            new File(SAMPLE_FILE_DIR).Mkdir();

            var introduce = FindViewById<TextView>(R.Id.how_to_use);
            introduce.PaintFlags = introduce.PaintFlags | PaintFlags.UnderlineText;
            introduce.SetOnClickListener(view =>
            {
                var uri = Uri.Parse("http://www.smartisan.com/pr/#/video/onestep-Introduction");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            });

            mOneStepHelper = OneStepHelper.GetInstance(this);
            var textView = FindViewById<View>(R.Id.text);
            textView.SetOnLongClickListener(v =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    mOneStepHelper.DragText(v, "永远年轻,永远热泪盈眶!");
                    return true;
                }
                else
                {
                    Toast.MakeText(this, "还没进入OneStep模式呢.", ToastLength.Short).Show();
                }
                return false;
            });

            FindViewById(R.Id.text_card_style).SetOnLongClickListener(view =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    //3张图片的尺寸请参考 OneStepHelper 接口说明。
                    //background 或 content 为空时使用默认图片;
                    //content 和 avatar 同时为空时，相当于调用 mOneStepHelper.dragText(View, CharSequence);
                    Bitmap background = null;
                    var content = BitmapFactory.DecodeResource(Resources, R.Mipmap.drag_card_content);
                    var avatar = BitmapFactory.DecodeResource(Resources, R.Mipmap.music_cover);
                    mOneStepHelper.DragText(view, "拖拽的文本内容", background, content, avatar);
                    return true;
                }
                return false;
            });

            var linkView = FindViewById(R.Id.link);
            linkView.SetOnLongClickListener(v =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    mOneStepHelper.DragLink(v, "http://t.tt");
                    return true;
                }
                return false;
            });

            FindViewById(R.Id.file).SetOnLongClickListener(view =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    var f = createTestFileIfNotExists("drag_file_test.txt");
                    mOneStepHelper.DragFile(view, f, "text/plain", f.Name);
                    return true;
                }
                return false;
            });

            FindViewById(R.Id.file_card_style).SetOnLongClickListener(view =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    var f = createTestFileIfNotExists("drag_file_test.mp3");
                    var content = BitmapFactory.DecodeResource(Resources, R.Mipmap.drag_card_content);
                    var avatar = BitmapFactory.DecodeResource(Resources, R.Mipmap.music_cover);
                    mOneStepHelper.DragFile(view, f, "audio/mpeg", null, content, avatar);
                    return true;
                }
                return false;
            });

            FindViewById(R.Id.image).SetOnLongClickListener(view =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    var f = new File(SAMPLE_FILE_DIR, DRAG_IMG_FILE_NAME);
                    if (!f.Exists())
                    {
                        copyAssetFile2Sdcard(DRAG_IMG_FILE_NAME);
                    }
                    mOneStepHelper.DragImage(view, f, "image/jpeg");
                    return true;
                }
                return false;
            });

            FindViewById(R.Id.image_show_specific).SetOnLongClickListener(view =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    var f = new File(SAMPLE_FILE_DIR, DRAG_IMG_FILE_NAME);
                    if (!f.Exists())
                    {
                        copyAssetFile2Sdcard(DRAG_IMG_FILE_NAME);
                    }
                    Bitmap specificContent = BitmapFactory.DecodeResource(Resources, R.Mipmap.ic_fso_folder);
                    mOneStepHelper.DragImage(view, specificContent, f, "image/png");
                    return true;
                }
                return false;
            });

            FindViewById(R.Id.multi_images).SetOnLongClickListener(view =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    var size = 3;
                    var files = new File[size];
                    var mimeTypes = new string[size];
                    var f1 = new File(SAMPLE_FILE_DIR, DRAG_IMG_FILE_NAME);
                    if (!f1.Exists())
                    {
                        copyAssetFile2Sdcard(DRAG_IMG_FILE_NAME);
                    }
                    files[0] = f1;
                    mimeTypes[0] = "image/jpeg";

                    var f2 = new File(SAMPLE_FILE_DIR, "music_cover.png");
                    if (!f2.Exists())
                    {
                        copyAssetFile2Sdcard("music_cover.png");
                    }
                    files[1] = f2;
                    mimeTypes[1] = "image/png";

                    var f3 = new File(SAMPLE_FILE_DIR, "ic_fso_folder.png");
                    if (!f3.Exists())
                    {
                        copyAssetFile2Sdcard("ic_fso_folder.png");
                    }
                    files[2] = f3;
                    mimeTypes[2] = "image/png";

                    mOneStepHelper.DragMultipleImages(view, files, mimeTypes);
                    return true;
                }
                return false;
            });


            var btn_show_popup = FindViewById<Button>(R.Id.btn_show_popup);
            var dragListener = new OnDragListener((view, dragEvent) =>
            {
                Toast.MakeText(this, "Drag started", ToastLength.Short).Show();
                return false;
            });
            btn_show_popup.SetOnClickListener(new OnClickListener(view =>
            {
                if (mOneStepHelper.IsOneStepShowing)
                {
                    mTextDragPopupWindow = mOneStepHelper.ShowDragPopupText(btn_show_popup,
                        dragListener,
                        "One Step",
                        btn_show_popup.Left,
                        btn_show_popup.Top);
                }
            }));

            var btn_hide_popup = FindViewById<Button>(R.Id.btn_hide_popup);
            btn_hide_popup.SetOnClickListener(view =>
            {
                mTextDragPopupWindow?.Hide();
            });

            FindViewById(R.Id.btn_s_b_l).SetOnClickListener(view =>
            {
                // 状态栏 亮
                StatusBarUtils.SetLightStatusBar(Window, true);
                StatusBarUtils.SetStatusBarColor(Window, Color.White);
            });

            FindViewById(R.Id.btn_s_b_d).SetOnClickListener(view =>
            {
                // 状态栏 暗
                StatusBarUtils.SetLightStatusBar(Window, false);
                StatusBarUtils.SetStatusBarColor(Window, Color.Black);
            });
        }

        private File createTestFileIfNotExists(string filename)
        {
            var testFile = new File(SAMPLE_FILE_DIR, filename);
            if (!testFile.Exists())
            {
                try
                {
                    testFile.CreateNewFile();
                }
                catch
                {
                    // ignored
                }
            }
            return testFile;
        }

        private void copyAssetFile2Sdcard(string assetFile)
        {
            try
            {
                var destFilePath = createTestFileIfNotExists(assetFile).AbsolutePath;
                using (var stream = Assets.Open(assetFile))
                using (var fileStream = System.IO.File.OpenWrite(destFilePath))
                    stream.CopyTo(fileStream);
            }
            catch (Throwable e)
            {
                e.PrintStackTrace();
            }

            //Stream inputStream = null;
            //OutputStream outputStream = null;
            //try
            //{
            //    inputStream = Assets.Open(assetFile);
            //    var destFilePath = createTestFileIfNotExists(assetFile).AbsolutePath;
            //    var f = new File(destFilePath);
            //    outputStream = new FileOutputStream(f);
            //    var buf = new byte[1024 * 4];
            //    int len = 0;
            //    while ((len = inputStream.Read(buf)) > 0)
            //    {
            //        outputStream.Write(buf, 0, len);
            //    }
            //    outputStream.Flush();
            //}
            //catch (Java.Lang.Exception e)
            //{
            //    e.PrintStackTrace();
            //}
            //finally
            //{
            //    try
            //    {
            //        outputStream?.Close();
            //        inputStream?.Close();
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //}
        }
    }
}

