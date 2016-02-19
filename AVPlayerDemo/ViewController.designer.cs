// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AVPlayerDemo
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIView ControlsView { get; set; }

		[Outlet]
		UIKit.UIView OptionsView { get; set; }

		[Outlet]
		UIKit.UIView PlayerView { get; set; }

		[Outlet]
		UIKit.UIButton PlayPauseButton { get; set; }

		[Outlet]
		UIKit.UISlider Scrubber { get; set; }

		[Action ("PlayLive:")]
		partial void PlayLive (Foundation.NSObject sender);

		[Action ("PlayLocal:")]
		partial void PlayLocal (Foundation.NSObject sender);

		[Action ("TogglePlayPause:")]
		partial void TogglePlayPause (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (OptionsView != null) {
				OptionsView.Dispose ();
				OptionsView = null;
			}

			if (ControlsView != null) {
				ControlsView.Dispose ();
				ControlsView = null;
			}

			if (PlayerView != null) {
				PlayerView.Dispose ();
				PlayerView = null;
			}

			if (PlayPauseButton != null) {
				PlayPauseButton.Dispose ();
				PlayPauseButton = null;
			}

			if (Scrubber != null) {
				Scrubber.Dispose ();
				Scrubber = null;
			}
		}
	}
}
