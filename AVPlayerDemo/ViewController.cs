using System;
using UIKit;
using AVFoundation;
using Foundation;

namespace AVPlayerDemo
{
	public partial class ViewController : UIViewController
	{
		NSObject _aVPlayerItemFailedToPlayToEndTimeNotificationObserver;
		NSObject _aVPlayerItemDidPlayToEndTimeNotificationObserver;
		NSObject _aVPlayerItemPlaybackStalledNotificationObserver;
		NSObject _aVPlayerItemNewErrorLogEntryNotificationObserver;
		NSObject _aVPlayerItemNewAccessLogEntryNotificationObserver;
		NSObject _UIApplicationDidEnterBackgroundNotificationObserver;
		NSObject _UIApplicationDidBecomeActiveNotificationObserver;
		NSObject _UIDeviceOrientationDidChangeNotificationObserver;

		static NSString StatusObservationContext = new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");
		static NSString PlaybackBufferEmptyContext = new NSString("PlaybackBufferEmptyContext");
		static NSString PlaybackBufferFullContext = new NSString("PlaybackBufferFullContext");
		static NSString PlaybackLikelyToKeepUpContext = new NSString("PlaybackLikelyToKeepUpContext");

		AVPlayer _player;
		AVPlayerLayer _playerLayer;
		AVAsset _asset;
		AVPlayerItem _playerItem;
		UIView _playerView;

		/* Source -> http://stackoverflow.com/questions/10104301/hls-streaming-video-url-need-for-testing */
		string _url = "http://vevoplaylist-live.hls.adaptive.level3.net/vevo/ch1/appleman.m3u8";

		public ViewController (IntPtr handle) : base (handle)
		{
			View.AutosizesSubviews = true;
			View.TranslatesAutoresizingMaskIntoConstraints = true;
			View.BackgroundColor = UIColor.Gray;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			LoadNotifications ();

			CreateAVAsset ();
			CreateAVPlayer ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			UnloadNotifications ();
		}

		void CreateAVAsset()
		{
			_asset = AVAsset.FromUrl (NSUrl.FromString(_url));
			_playerItem = new AVPlayerItem (_asset);
		}

		void CreateAVPlayer()
		{
			_playerView = new UIView (View.Frame);
			_playerView.TranslatesAutoresizingMaskIntoConstraints = true;

			_player = new AVPlayer (_playerItem);
			_playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = View.Frame;
			_playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;

			_playerView.Layer.AddSublayer (_playerLayer);

			View.Add (_playerView);

			Play ();
		}

		void Play()
		{
			_player.Muted = true;
			_player.Play ();
		}

		void AVPlayerItemPlaybackStalled(NSNotification notification)
		{
			Console.WriteLine ("AVPlayerItemPlaybackStalled!");
		}

		void AVPlayerItemNewErrorLogEntry(NSNotification notification)
		{
			Console.WriteLine ("AVPlayerItemNewErrorLogEntry");
		}

		void AVPlayerItemNewAccessLogEntry(NSNotification notification)
		{
			Console.WriteLine ("AVPlayerItemNewAccessLogEntry");
		}

		void AVPlayerPlaybackError(NSNotification notification)
		{
			Console.WriteLine ("PlayerPlaybackError");
		}

		void AVPlayerPlaybackDidFinish(NSNotification notification)
		{
			Console.WriteLine ("AVPlayerPlaybackDidFinish");
		}

		void AppDidEnterBackground(NSNotification notification)
		{
			Console.WriteLine ("AppDidEnterBackground");
		}

		void AppDidBecomeActive(NSNotification notification)
		{
			Console.WriteLine ("AppDidBecomeActive");
		}

		void OrientationDidChange(NSNotification notification)
		{
			Console.WriteLine ("OrientationDidChange");
		}

		void LoadNotifications()
		{
			_aVPlayerItemFailedToPlayToEndTimeNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"AVPlayerItemFailedToPlayToEndTimeNotification", AVPlayerPlaybackError);
			_aVPlayerItemDidPlayToEndTimeNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"AVPlayerItemDidPlayToEndTimeNotification", AVPlayerPlaybackDidFinish);
			_aVPlayerItemPlaybackStalledNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"AVPlayerItemPlaybackStalledNotification", AVPlayerItemPlaybackStalled);
			_aVPlayerItemNewErrorLogEntryNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"AVPlayerItemNewErrorLogEntryNotification", AVPlayerItemNewErrorLogEntry);
			_aVPlayerItemNewAccessLogEntryNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"AVPlayerItemNewAccessLogEntryNotification", AVPlayerItemNewAccessLogEntry);
			_UIApplicationDidEnterBackgroundNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"UIApplicationDidEnterBackgroundNotification", AppDidEnterBackground);
			_UIApplicationDidBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"UIApplicationDidBecomeActiveNotification", AppDidBecomeActive);
			_UIDeviceOrientationDidChangeNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"UIDeviceOrientationDidChangeNotification", OrientationDidChange);
		}

		void UnloadNotifications()
		{
			if (_aVPlayerItemFailedToPlayToEndTimeNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_aVPlayerItemFailedToPlayToEndTimeNotificationObserver);

			if (_aVPlayerItemDidPlayToEndTimeNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_aVPlayerItemDidPlayToEndTimeNotificationObserver);

			if (_aVPlayerItemPlaybackStalledNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_aVPlayerItemPlaybackStalledNotificationObserver);

			if (_aVPlayerItemNewErrorLogEntryNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_aVPlayerItemNewErrorLogEntryNotificationObserver);

			if (_aVPlayerItemNewAccessLogEntryNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_aVPlayerItemNewAccessLogEntryNotificationObserver);

			if (_UIApplicationDidEnterBackgroundNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_UIApplicationDidEnterBackgroundNotificationObserver);

			if (_UIApplicationDidBecomeActiveNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_UIApplicationDidBecomeActiveNotificationObserver);

			if (_UIDeviceOrientationDidChangeNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(_UIDeviceOrientationDidChangeNotificationObserver);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

