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

    internal class OnClickListener : ViewListener<Action<View>>, View.IOnClickListener
    {
        internal OnClickListener(Action<View> action) : base(action) { }
        void View.IOnClickListener.OnClick(View v) => function(v);
    }

    internal class OnLongClickListener : ViewListener<Func<View, bool>>, View.IOnLongClickListener
    {
        internal OnLongClickListener(Func<View, bool> func) : base(func) { }
        bool View.IOnLongClickListener.OnLongClick(View v) => function(v);
    }

    internal class OnDragListener : ViewListener<Func<View, DragEvent, bool>>, View.IOnDragListener
    {
        internal OnDragListener(Func<View, DragEvent, bool> function) : base(function) { }
        bool View.IOnDragListener.OnDrag(View v, DragEvent e) => function(v, e);
    }
}

namespace Sample.Internal
{
    internal abstract class ViewListener<TFunction> : JavaObject
    {
        protected readonly TFunction function;
        protected ViewListener(TFunction function) => this.function = function;
    }
}