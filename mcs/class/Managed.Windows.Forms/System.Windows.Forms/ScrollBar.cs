//
// System.Windows.Forms.ScrollBar.cs
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (C) 2004, Novell, Inc.
//
// Authors:
//	Jordi Mas i Hernandez	jordi@ximian.com
//
//
// $Revision: 1.19 $
// $Modtime: $
// $Log: ScrollBar.cs,v $
// Revision 1.19  2004/09/05 08:03:51  jordi
// fixes bugs, adds flashing on certain situations
//
// Revision 1.18  2004/08/31 10:35:04  jordi
// adds autorepeat timer, uses a single timer, fixes scrolling bugs, adds new methods
//
// Revision 1.17  2004/08/25 21:35:18  jordi
// more fixes to scrollbar
//
// Revision 1.16  2004/08/25 19:20:13  jordi
// small bug fix regarding bar position
//
// Revision 1.15  2004/08/24 18:37:02  jordi
// fixes formmating, methods signature, and adds missing events
//
// Revision 1.14  2004/08/23 22:53:15  jordi
// small fix
//
// Revision 1.13  2004/08/23 22:43:46  jordi
// *** empty log message ***
//
// Revision 1.11  2004/08/22 19:34:22  jackson
// Update the position through the Value property so the OnValueChanged event is raised.
//
// Revision 1.10  2004/08/21 20:22:21  pbartok
// - Replaced direct XplatUI calls with their Control counterpart
//
// Revision 1.9  2004/08/20 19:35:33  jackson
// Use the SWF timer so callbacks are run in the correct thread
//
// Revision 1.8  2004/08/20 19:34:26  jackson
// Use the SWF timer so callbacks are run in the correct thread
//
// Revision 1.7  2004/08/19 22:25:31  jordi
// theme enhancaments
//
// Revision 1.6  2004/08/18 15:56:12  jordi
// fixes to scrollbar: steps and multiple timers
//
// Revision 1.5  2004/08/10 19:21:27  jordi
// scrollbar enhancements and standarize on win colors defaults
//
// Revision 1.4  2004/08/10 15:41:50  jackson
// Allow control to handle buffering
//
// Revision 1.3  2004/07/27 15:29:40  jordi
// fixes scrollbar events
//
// Revision 1.2  2004/07/26 17:42:03  jordi
// Theme support
//

// COMPLETE

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	[DefaultEvent ("Scroll")]
	[DefaultProperty ("Value")]
	public class ScrollBar : Control
	{
		#region Local Variables
		private int position;
		private int minimum;
		private int maximum;
		private int large_change;
		private int small_change;
		private int scrollbutton_height;
		private int scrollbutton_width;
		private Rectangle paint_area = new Rectangle ();
		private ScrollBars type;
		private Rectangle first_arrow_area = new Rectangle ();		// up or left
		private Rectangle second_arrow_area = new Rectangle ();		// down or right
		private Rectangle thumb_pos = new Rectangle ();
		private Rectangle thumb_area = new Rectangle ();
		private ButtonState firstbutton_state = ButtonState.Normal;
		private ButtonState secondbutton_state = ButtonState.Normal;
		private bool thumb_pressed = false;
		private float pixel_per_pos = 0;
		private Timer timer = new Timer ();
		private TimerType timer_type;
		private int thumb_pixel_click_move;
		private int thumb_pixel_click_move_prev;
		private int thumb_size = 0;
		private const int thumb_min_size = 8;
		internal bool vert;
		private int lastclick_pos;      // Position of the last button-down event
		private int lastclick_pos_thumb;      // Position of the last button-down event relative to the thumb		
		private bool outside_thumbarea_right = false;
		private bool outside_thumbarea_left = false;
		internal ThumbMoving thumb_moving = ThumbMoving.None;
		#endregion	// Local Variables

		private enum TimerType
		{
			HoldButton,
			RepeatButton,
			HoldThumbArea,
			RepeatThumbArea
		}
		
		internal enum ThumbMoving
		{
			None,
			Forward,
			Backwards,
		}

		#region Events
		public new event EventHandler BackColorChanged;
		public new event EventHandler BackgroundImageChanged;
		public new event EventHandler Click;
		public new event EventHandler DoubleClick;
		public new event EventHandler FontChanged;
		public new event EventHandler ForeColorChanged;
		public new event EventHandler ImeModeChanged;
		public new event MouseEventHandler MouseDown;
		public new event MouseEventHandler MouseMove;
		public new event MouseEventHandler MouseUp;
		public new event PaintEventHandler Paint;
		public event ScrollEventHandler Scroll;
		public new event EventHandler TextChanged;
		public event EventHandler ValueChanged;
		#endregion Events

		public ScrollBar ()
		{
			position = 0;
			minimum = 0;
			maximum = 100;
			large_change = 10;
			small_change = 1;

			timer.Tick += new EventHandler (OnTimer);
			base.KeyDown += new KeyEventHandler (OnKeyDownSB);
			base.MouseDown += new MouseEventHandler (OnMouseDownSB);
			base.MouseUp += new MouseEventHandler (OnMouseUpSB);
			base.MouseMove += new MouseEventHandler (OnMouseMoveSB);
			base.Resize += new EventHandler (OnResizeSB);
			base.TabStop = false;

			if (ThemeEngine.Current.WriteToWindow == true)
				double_buffering = false;
			else
				double_buffering = true;

			SetStyle (ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);
		}

		#region Public Properties

		[EditorBrowsable (EditorBrowsableState.Never)]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set {
				if (base.BackColor == value)
					return;

				if (BackColorChanged != null)
					BackColorChanged (this, EventArgs.Empty);

				base.BackColor = value;
				Refresh ();
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public override Image BackgroundImage
		{
			get { return base.BackgroundImage; }
			set {
				if (base.BackgroundImage == value)
					return;

				if (BackgroundImageChanged != null)
					BackgroundImageChanged (this, EventArgs.Empty);

				base.BackgroundImage = value;
			}
		}

		protected override CreateParams CreateParams
		{
			get {	return base.CreateParams; }
		}

		protected override ImeMode DefaultImeMode
		{
			get { return ImeMode.Disable; }
		}

		public override Font Font
		{
			get { return base.Font; }
			set {
				if (base.Font == value)
					return;

				if (FontChanged != null)
					FontChanged (this, EventArgs.Empty);

				base.Font = value;
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set {
				if (base.ForeColor == value)
					return;

				if (ForeColorChanged != null)
					ForeColorChanged (this, EventArgs.Empty);

				base.ForeColor = value;
				Refresh ();
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public new ImeMode ImeMode
		{
			get { return base.ImeMode; }
			set {
				if (base.ImeMode == value)
					return;

				if (ImeModeChanged != null)
					ImeModeChanged (this, EventArgs.Empty);

				base.ImeMode = value;
			}
		}

		public int LargeChange {
			get { return large_change; }
			set {
				if (value < 0)
					throw new Exception( string.Format("Value '{0}' must be greater than or equal to 0.", value));

				if (large_change != value) {
					large_change = value;
					Refresh ();
				}
			}
		}

		public int Maximum {
			get { return maximum; }
			set {
				maximum = value;

				if (maximum < minimum)
					minimum = maximum;

				Refresh ();
			}
		}

		public int Minimum {
			get { return minimum; }
			set {
				minimum = value;

				if (minimum > maximum)
					maximum = minimum;

				Refresh ();
			}
		}

		public int SmallChange {
			get { return small_change; }
			set {
				if ( value < 0 )
					throw new Exception( string.Format("Value '{0}' must be greater than or equal to 0.", value));

				if (small_change != value) {
					small_change = value;
					Refresh ();
				}
			}
		}

		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public override string Text {
			 get { return base.Text;  }
			 set { base.Text = value; }
		}

		public int Value {
			get { return position; }
			set {
				if ( value < minimum || value > maximum )
					throw new ArgumentException(
						string.Format("'{0}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'", value));

				if (position != value){
					position = value;

					if (ValueChanged != null)
						ValueChanged (this, EventArgs.Empty);

					UpdatePos (Value, true);
					Refresh ();
				}
			}
		}

		#endregion //Public Properties

		#region Public Methods
		
		protected override void OnEnabledChanged (EventArgs e)
		{
			base.OnEnabledChanged (e);

			if (Enabled)
				firstbutton_state = secondbutton_state = ButtonState.Normal;
			else
				firstbutton_state = secondbutton_state = ButtonState.Inactive;

			Refresh ();
		}
		
		protected override void OnHandleCreated (System.EventArgs e)
		{
			base.OnHandleCreated (e);

			scrollbutton_height = ThemeEngine.Current.ScrollBarButtonSize;
			scrollbutton_width = ThemeEngine.Current.ScrollBarButtonSize;

			CreateBuffers (Width, Height);

			CalcThumbArea ();
			UpdatePos (Value, true);
		}

		protected virtual void OnScroll (ScrollEventArgs event_args)
		{
			if (Scroll == null)
				return;

			Scroll (this, event_args);
		}

		protected virtual void OnValueChanged (EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged (this, e);
		}

		public override string ToString()
		{
			return string.Format("{0}, Minimum: {1}, Maximum: {2}, Value: {3}",
						GetType( ).FullName.ToString( ), minimum, maximum, position);
		}

		protected void UpdateScrollInfo ()
		{
			Refresh ();
		}

		protected override void WndProc (ref Message m)
		{
			switch ((Msg) m.Msg)
			{
				case Msg.WM_PAINT:
				{
					PaintEventArgs	paint_event;

					paint_event = XplatUI.PaintEventStart (Handle);
					OnPaintSB (paint_event);
					XplatUI.PaintEventEnd (Handle);
					return;
				}


				case Msg.WM_ERASEBKGND:
					m.Result = (IntPtr) 1; /// Disable background painting to avoid flickering
					return;

				default:
					break;
			}

			base.WndProc (ref m);
		}

		#endregion //Public Methods

		#region Private Methods
		
		private void CalcThumbArea ()
		{
			// Thumb area
			if (vert) {

				thumb_area.Height = Height - scrollbutton_height -  scrollbutton_height;
				thumb_area.X = 0;
				thumb_area.Y = scrollbutton_height;
				thumb_area.Width = Width;

				if (Height < scrollbutton_height * 2)
					thumb_size = 0;
				else {
					double per =  ((double) large_change / (double)((1 + maximum - minimum)));
					thumb_size = 1 + (int) (thumb_area.Height * per);

					if (thumb_size < thumb_min_size)
						thumb_size = thumb_min_size;
				}


				pixel_per_pos = ((float)(thumb_area.Height - thumb_size) / (float) ((maximum - minimum - large_change) + 1));

			} else	{

				thumb_area.Y = 0;
				thumb_area.X = scrollbutton_width;
				thumb_area.Height = Height;
				thumb_area.Width = Width - scrollbutton_width -  scrollbutton_width;

				if (Width < scrollbutton_width * 2)
					thumb_size = 0;
				else {
					double per =  ((double) large_change / (double)((1 + maximum - minimum)));
					thumb_size = 1 + (int) (thumb_area.Width * per);

					if (thumb_size < thumb_min_size)
						thumb_size = thumb_min_size;
				}
				pixel_per_pos = ((float)(thumb_area.Width - thumb_size) / (float) ((maximum - minimum - large_change) + 1));
			}
		}
		
		private void Draw ()
		{
			ThemeEngine.Current.DrawScrollBar (DeviceContext, paint_area, this, ref thumb_pos,
				ref first_arrow_area, ref second_arrow_area,
				firstbutton_state, secondbutton_state,
				ref scrollbutton_width, ref scrollbutton_height, vert);

		}

		private void LargeIncrement ()
    		{
			UpdatePos (position + large_change, true);

			Refresh ();
			OnScroll (new ScrollEventArgs (ScrollEventType.LargeIncrement, position));
			OnScroll (new ScrollEventArgs (ScrollEventType.EndScroll, position));
    		}

    		private void LargeDecrement ()
    		{
			UpdatePos (position - large_change, true);

			Refresh ();
			OnScroll (new ScrollEventArgs (ScrollEventType.LargeDecrement, position));
			OnScroll (new ScrollEventArgs (ScrollEventType.EndScroll, position));
    		}

    		private void OnResizeSB (Object o, EventArgs e)
    		{
    			if (Width <= 0 || Height <= 0)
    				return;

			paint_area.X = paint_area. Y = 0;
			paint_area.Width = Width;
			paint_area.Height = Height;

			CreateBuffers (Width, Height);

			CalcThumbArea ();
			UpdatePos (position, true);
    		}

		private void OnPaintSB (PaintEventArgs pevent)
		{
			if (Width <= 0 || Height <=  0 || Visible == false)
    				return;

			/* Copies memory drawing buffer to screen*/
			Draw ();

			if (double_buffering)
				pevent.Graphics.DrawImage (ImageBuffer, 0, 0);

		}

		private void OnTimer (Object source, EventArgs e)
		{
			switch (timer_type) {

			case TimerType.HoldButton:
				SetRepeatButtonTimer ();
				break;

			case TimerType.RepeatButton:
			{
				if ((firstbutton_state & ButtonState.Pushed) == ButtonState.Pushed)
					SmallDecrement();

				if ((secondbutton_state & ButtonState.Pushed) == ButtonState.Pushed)
					SmallIncrement();

				break;
			}

			case TimerType.HoldThumbArea:
				SetRepeatThumbAreaTimer ();
				break;

			case TimerType.RepeatThumbArea:
			{
				Point pnt;
				pnt = PointToClient (MousePosition);
				
				if (vert)
					lastclick_pos = pnt.Y;
				else
					lastclick_pos = pnt.X;

				if (thumb_moving == ThumbMoving.Forward) {
					if ((vert && (thumb_pos.Y + thumb_size > lastclick_pos)) ||
						(!vert && (thumb_pos.X + thumb_size > lastclick_pos)) ||
						(thumb_area.Contains (pnt) == false)){
						timer.Enabled = false;						
    						thumb_moving = ThumbMoving.None;
    						Refresh ();    			
					} else
						LargeIncrement ();
				}
				else
					if ((vert && (thumb_pos.Y < lastclick_pos)) ||
						(!vert && (thumb_pos.X  < lastclick_pos))){
						timer.Enabled = false;
						thumb_moving = ThumbMoving.None;
    						Refresh ();    						
					} else
						LargeDecrement ();

				break;
			}
			default:
				break;
			}

		}		

    		private void OnMouseMoveSB (object sender, MouseEventArgs e)
    		{
    			if (!first_arrow_area.Contains (e.X, e.Y) &&
    				((firstbutton_state & ButtonState.Pushed) == ButtonState.Pushed)) {
				firstbutton_state = ButtonState.Normal;
				this.Capture = false;
				Refresh ();
			}

			if (!second_arrow_area.Contains (e.X, e.Y) &&
    				((secondbutton_state & ButtonState.Pushed) == ButtonState.Pushed)) {
				secondbutton_state = ButtonState.Normal;
				this.Capture = false;
				Refresh ();
			}

			if (thumb_pressed == true) {

    				int pixel_pos;

				if (vert) {

					int mouse_click = e.Y;
					int outside_curpos = thumb_area.Y + thumb_area.Height - thumb_size + lastclick_pos_thumb;
					
					
					if (mouse_click > thumb_area.Y + thumb_area.Height) {
						outside_thumbarea_right = true;
						mouse_click = thumb_area.Y + thumb_area.Height;
					}

					if (mouse_click < thumb_area.Y) {
						outside_thumbarea_left = true;
						mouse_click = thumb_area.Y;
					}

					if (outside_thumbarea_right && mouse_click < outside_curpos) {
						outside_thumbarea_right = false;
						thumb_pixel_click_move_prev =
						thumb_pixel_click_move = outside_curpos;
					}

					if (outside_thumbarea_left && mouse_click > thumb_area.Y + lastclick_pos_thumb) {
						outside_thumbarea_left = false;
						thumb_pixel_click_move_prev =
						thumb_pixel_click_move = thumb_area.Y + lastclick_pos_thumb;
					}

					if (outside_thumbarea_right == false && outside_thumbarea_left == false) {
						pixel_pos = thumb_pos.Y + (thumb_pixel_click_move - thumb_pixel_click_move_prev);
						thumb_pixel_click_move_prev = thumb_pixel_click_move;
						thumb_pixel_click_move = mouse_click;
						UpdateThumbPos (pixel_pos, true);
					}

				}
				else {
					int mouse_click = e.X;
					int outside_curpos = thumb_area.X + thumb_area.Width - thumb_size + lastclick_pos_thumb;
										
					if (mouse_click >  thumb_area.X + thumb_area.Width) {
						outside_thumbarea_right = true;
						mouse_click = thumb_area.X + thumb_area.Width;
					}

					if (mouse_click <  thumb_area.X) {
						outside_thumbarea_left = true;
						mouse_click = thumb_area.X;
					}

					if (outside_thumbarea_right && mouse_click < outside_curpos) {
						outside_thumbarea_right = false;
						thumb_pixel_click_move_prev =
						thumb_pixel_click_move = outside_curpos;
					}

					if (outside_thumbarea_left && mouse_click > thumb_area.X + lastclick_pos_thumb) {
						outside_thumbarea_left = false;
						thumb_pixel_click_move_prev =
						thumb_pixel_click_move = thumb_area.X + lastclick_pos_thumb;
					}

					if (outside_thumbarea_right == false && outside_thumbarea_left == false) {
						pixel_pos = thumb_pos.X + (thumb_pixel_click_move - thumb_pixel_click_move_prev);
						thumb_pixel_click_move_prev = thumb_pixel_click_move;
						thumb_pixel_click_move = mouse_click;
						UpdateThumbPos (pixel_pos, true);
					}

				}

				Refresh ();
			}

    		}

    		private void OnMouseDownSB (object sender, MouseEventArgs e)
    		{
    			if (firstbutton_state != ButtonState.Inactive && first_arrow_area.Contains (e.X, e.Y)) {
				this.Capture = true;				
				firstbutton_state = ButtonState.Pushed;
				Refresh ();				
			}

			if (secondbutton_state != ButtonState.Inactive && second_arrow_area.Contains (e.X, e.Y)) {
				this.Capture = true;				
				secondbutton_state = ButtonState.Pushed;
				Refresh ();
			}

			if (thumb_pos.Contains (e.X, e.Y)) {
				thumb_pressed = true;
				this.Capture = true;
				Refresh ();
				if (vert) {
					lastclick_pos_thumb = e.Y - thumb_pos.Y;
					lastclick_pos = e.Y;					
					thumb_pixel_click_move_prev = thumb_pixel_click_move = e.Y;
				}
				else {
					lastclick_pos_thumb = e.X - thumb_pos.X;
					lastclick_pos = e.X;
					thumb_pixel_click_move_prev = thumb_pixel_click_move = e.X;
				}
			}
			else {
				if (thumb_area.Contains (e.X, e.Y)) {

					if (vert) {
						lastclick_pos_thumb = e.Y - thumb_pos.Y;
						lastclick_pos = e.Y;

						if (e.Y > thumb_pos.Y + thumb_pos.Height) {
							LargeIncrement ();							
							thumb_moving = ThumbMoving.Forward;							
						}
						else {
							LargeDecrement ();							
							thumb_moving = ThumbMoving.Backwards;
						}
					}
					else 	{

						lastclick_pos_thumb = e.X - thumb_pos.X;
						lastclick_pos = e.X;

						if (e.X > thumb_pos.X + thumb_pos.Width) {
							thumb_moving = ThumbMoving.Forward;
							LargeIncrement ();							
						}
						else {
							thumb_moving = ThumbMoving.Backwards;
							LargeDecrement ();							
						}
					}

					SetHoldThumbAreaTimer ();
					timer.Enabled = true;
					Refresh ();
				}
			}
				
			

			/* If arrows are pressed, fire timer for auto-repeat */
			if ((((firstbutton_state & ButtonState.Pushed) == ButtonState.Pushed)
			|| ((secondbutton_state & ButtonState.Pushed) == ButtonState.Pushed)) &&
				timer.Enabled == false) {
		        	SetHoldButtonClickTimer ();
		        	timer.Enabled = true;
			}
    		}
    		
    		private void OnMouseUpSB (object sender, MouseEventArgs e)
    		{
    			timer.Enabled = false;
    			
    			if (thumb_moving != ThumbMoving.None) {
    				thumb_moving = ThumbMoving.None;
    				Refresh ();
    			}    			

    			if (firstbutton_state != ButtonState.Inactive && first_arrow_area.Contains (e.X, e.Y)) {

				firstbutton_state = ButtonState.Normal;
				SmallDecrement ();
			}

			if (secondbutton_state != ButtonState.Inactive && second_arrow_area.Contains (e.X, e.Y)) {

				secondbutton_state = ButtonState.Normal;
				SmallIncrement ();
			}

			if (thumb_pressed == true) {
				OnScroll (new ScrollEventArgs (ScrollEventType.ThumbPosition, position));
				OnScroll (new ScrollEventArgs (ScrollEventType.EndScroll, position));
				this.Capture = false;
				thumb_pressed = false;				
				Refresh ();
			}


    		}

    		private void OnKeyDownSB (Object o, KeyEventArgs key)
		{
			switch (key.KeyCode){
			case Keys.Up:
			{
				SmallDecrement ();
				break;
			}
			case Keys.Down:
			{
				SmallIncrement ();
				break;
			}
			case Keys.PageUp:
			{
				LargeDecrement ();
				break;
			}
			case Keys.PageDown:
			{
				LargeIncrement ();
				break;
			}
			default:
				break;
			}

		}

    		private void SmallIncrement ()
    		{
			UpdatePos (position + small_change, true);

			Refresh ();
			OnScroll (new ScrollEventArgs (ScrollEventType.SmallIncrement, position));
			OnScroll (new ScrollEventArgs (ScrollEventType.EndScroll, position));
    		}

    		private void SmallDecrement ()
    		{
			UpdatePos (position - small_change, true);

			Refresh ();
			OnScroll (new ScrollEventArgs (ScrollEventType.SmallDecrement, position));
			OnScroll (new ScrollEventArgs (ScrollEventType.EndScroll, position));
    		}
    		
    		private void SetHoldButtonClickTimer ()
		{
			timer.Enabled = false;
			timer.Interval = 200;
			timer_type = TimerType.HoldButton;
			timer.Enabled = true;
		}

		private void SetRepeatButtonTimer ()
		{
			timer.Enabled = false;
			timer.Interval = 50;
			timer_type = TimerType.RepeatButton;
			timer.Enabled = true;
		}

		private void SetHoldThumbAreaTimer ()
		{
			timer.Enabled = false;
			timer.Interval = 200;
			timer_type = TimerType.HoldThumbArea;
			timer.Enabled = true;
		}

		private void SetRepeatThumbAreaTimer ()
		{
			timer.Enabled = false;
			timer.Interval = 50;
			timer_type = TimerType.RepeatThumbArea;
			timer.Enabled = true;
		}    		
		
    		private void UpdatePos (int newPos, bool update_thumbpos)
    		{
    			int old = position;
			int pos;

    			if (newPos < minimum)
    				pos = minimum;
    			else
    				if (newPos > maximum + 1 - large_change)
    					pos = maximum + 1 - large_change;
    					else
    						pos = newPos;

			if (update_thumbpos) {
				if (vert)
					UpdateThumbPos (thumb_area.Y + (int)(((float)(pos - minimum)) * pixel_per_pos), false);
				else
					UpdateThumbPos (thumb_area.X + (int)(((float)(pos - minimum)) * pixel_per_pos), false);

				Value = pos;
			}
			else {
				position = pos; // Updates directly the value to avoid thumb pos update
				
				if (ValueChanged != null)
					ValueChanged (this, EventArgs.Empty);
			}

			if (pos != old) // Fire event
				OnScroll (new ScrollEventArgs (ScrollEventType.ThumbTrack, pos));

    		}

    		private void UpdateThumbPos (int pixel, bool update_value)
    		{
    			float new_pos = 0;

    			if (vert) {
	    			if (pixel < thumb_area.Y)
	    				thumb_pos.Y = thumb_area.Y;
	    			else
	    				if (pixel > thumb_area.Y + thumb_area.Height - thumb_size)
	    					thumb_pos.Y = thumb_area.Y +  thumb_area.Height - thumb_size;
	    				else
	    					thumb_pos.Y = pixel;

				thumb_pos.X = 0;
				thumb_pos.Width = ThemeEngine.Current.ScrollBarButtonSize;
				thumb_pos.Height = thumb_size;
				new_pos = (float) (thumb_pos.Y - thumb_area.Y);
				new_pos = new_pos / pixel_per_pos;

			} else	{

				if (pixel < thumb_area.X)
	    				thumb_pos.X = thumb_area.X;
	    			else
	    				if (pixel > thumb_area.X + thumb_area.Width - thumb_size)
	    					thumb_pos.X = thumb_area.X +  thumb_area.Width - thumb_size;
	    				else
	    					thumb_pos.X = pixel;

				thumb_pos.Y = 0;
				thumb_pos.Width =  thumb_size;
				thumb_pos.Height = ThemeEngine.Current.ScrollBarButtonSize;
				new_pos = (float) (thumb_pos.X - thumb_area.X);
				new_pos = new_pos / pixel_per_pos;
			}

			if (update_value)
				UpdatePos ((int) new_pos + minimum, false);
    		}


		#endregion //Private Methods
	 }
}


