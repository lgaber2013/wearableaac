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
 * This is the ChatItemCell class. This serves as an extention of a ViewCell that is 
 * designed to display information about a ChatItem. It is used to display a ChatItem in a ListView.
 * 
 * */

namespace TTSTest2.Views
{
    class ChatItemCell : ViewCell
    {

        public ChatItemCell ()
            //constructor
		{
            ChatItem currItem = new ChatItem();
            
			var label = new Label {
				YAlign = TextAlignment.Center
			};
			label.SetBinding (Label.TextProperty, "Segment");

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
