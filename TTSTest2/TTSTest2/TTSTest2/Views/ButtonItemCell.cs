using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using TTSTest2.Models;

/*
 * Description:
 * 
 * This is the ButtonItemCell class. This serves as an extention of a ViewCell that is 
 * designed to display information about a ButtonItem. It is used to display a ButtonItem in a ListView.
 * 
 * */

namespace TTSTest2.Views
{
    public class ButtonItemCell : ViewCell
	{
		public ButtonItemCell ()
            //constructor
		{
            ButtonItem currItem = new ButtonItem();
            
            //images are extremely hard to work with right now for some reason, might try again later. 
            //this is where you'd set it up as part of the format of the cell for this item in its list.
            //to see what im talking about, take a look at image and cell work happening in HelloXamarin2.

			var label = new Label {
				YAlign = TextAlignment.Center
			};
			label.SetBinding (Label.TextProperty, "ButtonAlias");

			var layout = new StackLayout {
				Padding = new Thickness(20, 0, 0, 0),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				Children = {label}
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
