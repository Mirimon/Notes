using System;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(TextCell), typeof(SecurityNotes.Droid.Renderers.CustomTextCellRenderer))]

namespace SecurityNotes.Droid.Renderers {
    public class CustomTextCellRenderer : TextCellRenderer {
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context) {
            Android.Views.View cell = base.GetCellCore(item, convertView, parent, context);
            cell.SetBackgroundColor(Android.Graphics.Color.White);
            return cell;
        }
    }
}
