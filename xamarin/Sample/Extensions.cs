using System;
using Android.Views;
using Sample.Internal;
using JavaObject = Java.Lang.Object;

namespace Sample
{
    public static class Extensions
    {
        public static void SetOnClickListener(this View view, Action<View> action) => view.SetOnClickListener(new OnClickListener(action));
        public static void SetOnLongClickListener(this View view, Func<View, bool> func) => view.SetOnLongClickListener(new OnLongClickListener(func));
    }

    public class OnClickListener : ClickListener<Action<View>>, View.IOnClickListener
    {
        public OnClickListener(Action<View> action) : base(action) { }
        void View.IOnClickListener.OnClick(View v) => function(v);
    }

    public class OnLongClickListener : ClickListener<Func<View, bool>>, View.IOnLongClickListener
    {
        public OnLongClickListener(Func<View, bool> func) : base(func) { }
        bool View.IOnLongClickListener.OnLongClick(View v) => function(v);
    }

    public class OnDragListener : ClickListener<Func<View, DragEvent, bool>>, View.IOnDragListener
    {
        public OnDragListener(Func<View, DragEvent, bool> function) : base(function) { }

        bool View.IOnDragListener.OnDrag(View v, DragEvent e) => function(v, e);
    }
}

namespace Sample.Internal
{
    public abstract class ClickListener<TFunction> : JavaObject
    {
        protected readonly TFunction function;
        protected ClickListener(TFunction function) => this.function = function;
    }
}