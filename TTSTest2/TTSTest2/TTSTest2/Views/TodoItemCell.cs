using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

/*
 * Description:
 * 
 * This is the TodoItemCell class. This serves as an extention of a ViewCell that is 
 * designed to display information about a TodoItem. It is used to display a TodoItem in a ListView.
 * 
 * */

namespace TTSTest2.Views
{
    public class TodoItemCell : ViewCell
	{
		public TodoItemCell ()
            //constructor
		{
            //here is where (and in the edit page) the image they choose will be. what's the default though??
			var label = new Label {
				YAlign = TextAlignment.Center
			};
			label.SetBinding (Label.TextProperty, "Name");

			var tick = new Image {
				Source = FileImageSource.FromFile ("check.png"),
			};
			tick.SetBinding (Image.IsVisibleProperty, "Done"); //the check is visable on the inital page because these are bound together

			var layout = new StackLayout {
				Padding = new Thickness(20, 0, 0, 0),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				Children = {label, tick}//colorcode
			};
			View = layout;
		}

		protected override void OnBindingContextChanged ()
		{
			// Fixme : this is happening because the View.Parent is getting 
			// set after the Cell gets the binding context set on it. Then it is inheriting
			// the parents binding context.
			View.BindingContext = BindingContext;
			base.OnBindingContextChanged ();
		}
	}
}
