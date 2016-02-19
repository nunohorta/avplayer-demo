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

			CreateAVAsset ();
			CreateAVPlayer ();
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

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

