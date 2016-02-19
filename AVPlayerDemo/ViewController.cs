using System;
using UIKit;
using AVFoundation;
using Foundation;
using CoreMedia;
using System.Timers;

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

		protected float RestoreAfterScrubbingRate, ScrubbingSpeed;
		Timer touchTimer;

		AVPlayer _player;
		AVPlayerLayer _playerLayer;
		AVAsset _asset;
		AVPlayerItem _playerItem;
		bool _liveStream;

		/* Source -> http://stackoverflow.com/questions/10104301/hls-streaming-video-url-need-for-testing */
		string _url = "http://vevoplaylist-live.hls.adaptive.level3.net/vevo/ch1/appleman.m3u8";
		string _localFile = "trailer_1080p.mov";

		public ViewController (IntPtr handle) : base (handle)
		{
			View.BackgroundColor = UIColor.Gray;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadNotifications ();
			AddControlsGesture ();

			ControlsView.Hidden = true;
		}

		void AddControlsGesture()
		{
			var tap = new UITapGestureRecognizer ();
			tap.AddTarget (ToggleControls);
			tap.NumberOfTapsRequired = 1;
			PlayerView.AddGestureRecognizer (tap);
		}

		void ToggleControls()
		{
			if (ControlsView.Hidden)
				ShowToolbar ();
			else
				HideToolbar (null, null);

			if(touchTimer != null)
				touchTimer.Stop ();

			touchTimer = new Timer(3000);
			touchTimer.Elapsed += new ElapsedEventHandler(HideToolbar);
			touchTimer.Enabled = true;
		}

		void ShowToolbar()
		{
			ControlsView.Hidden = false;

			InvokeOnMainThread (delegate {
				UIView.Animate (
					0, 0, UIViewAnimationOptions.CurveEaseIn,
					() => {
						ControlsView.Alpha = 1f;
					}, () => {
					});
			});
		}

		void HideToolbar(object sender, ElapsedEventArgs e)
		{
			if (!IsPlaying() || touchTimer == null)
				return;

			if(touchTimer != null)
				touchTimer.Stop ();

			InvokeOnMainThread (delegate {
				UIView.Animate (
					0.4, 0, UIViewAnimationOptions.CurveEaseOut,
					() => {
						if(ControlsView != null)
							ControlsView.Alpha = 0f;
					}, () => {
						if(ControlsView != null)
							ControlsView.Hidden = true;
					});
			});
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			PlayerView.BackgroundColor = UIColor.Black;

			if (_playerLayer != null)
				_playerLayer.Frame = PlayerView.Bounds;
		}

		partial void PlayLive (NSObject sender)
		{
			_liveStream = true;
			InitialisePlayer ();
		}

		partial void PlayLocal (NSObject sender)
		{
			_liveStream = false;
			InitialisePlayer ();
		}

		partial void TogglePlayPause (NSObject sender)
		{
			if(IsPlaying()){
				_player.Pause ();
				ShowPlayButton();
			}else{
				_player.Play ();
				ShowPauseButton();
			}
		}
		 
		void InitialisePlayer()
		{
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
			if (_liveStream) {
				_asset = AVAsset.FromUrl (NSUrl.FromString(_url));
			} else {
				_asset = AVAsset.FromUrl (NSUrl.FromFilename (_localFile));
			}

			_playerItem = new AVPlayerItem (_asset);

//			_playerItem.AddObserver (this, (NSString)"status", NSKeyValueObservingOptions.New|NSKeyValueObservingOptions.Initial, StatusObservationContext.Handle);
//			_playerItem.AddObserver (this, (NSString)"playbackBufferFull", NSKeyValueObservingOptions.New|NSKeyValueObservingOptions.Initial, PlaybackBufferFullContext.Handle);
//			_playerItem.AddObserver (this, (NSString)"playbackBufferEmpty", NSKeyValueObservingOptions.New|NSKeyValueObservingOptions.Initial, PlaybackBufferEmptyContext.Handle);
//			_playerItem.AddObserver (this, (NSString)"playbackLikelyToKeepUp", NSKeyValueObservingOptions.New|NSKeyValueObservingOptions.Initial, PlaybackLikelyToKeepUpContext.Handle);
		}

		void ReleasePlayerItem()
		{
//			if (_playerItem != null) {
//				_playerItem.CancelPendingSeeks ();
//				_playerItem.RemoveObserver (this, (NSString)"status");
//				_playerItem.RemoveObserver (this, (NSString)"playbackBufferFull");
//				_playerItem.RemoveObserver (this, (NSString)"playbackBufferEmpty");
//				_playerItem.RemoveObserver (this, (NSString)"playbackLikelyToKeepUp");
//
//				_playerItem = null;
//			}
		}

		void SetupControls()
		{
			if (_liveStream) {
				Scrubber.Hidden = true;
			} else {
				Scrubber.Hidden = false;
			}
		}

		void CreateAVPlayer()
		{
			SetupControls ();

			_player = new AVPlayer (_playerItem);
			_playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Frame = PlayerView.Bounds;
			_playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;

			PlayerView.Layer.AddSublayer (_playerLayer);

			UIView.Animate (0.3, () => {
				OptionsView.Alpha = 0f;
			}, () =>{
				OptionsView.Hidden = true;
				PlayerView.Hidden = false;
			});

			Play ();
		}

		void Play()
		{
			_player.Muted = true;
			_player.Play ();

			SyncPlayPauseButtons ();
		}

		void SyncPlayPauseButtons()
		{
			if (IsPlaying ()) {
				ShowPauseButton ();
			} else {
				ShowPlayButton ();
			}
		}

		void ShowPlayButton()
		{
			PlayPauseButton.SetTitle ("Play", UIControlState.Normal);
		}

		void ShowPauseButton()
		{
			PlayPauseButton.SetTitle ("Pause", UIControlState.Normal);
		}

		bool IsPlaying()
		{
			if (_player == null)
				return false;

			return RestoreAfterScrubbingRate != 0f || _player.Rate != 0f;
		}

		void SyncScrubber()
		{
			CMTime playerDuration = _playerItem.Duration;
			if (playerDuration == CMTime.Invalid)
			{
				Scrubber.MinValue = 0.0f;
				return;
			}

			double duration = _playerItem.Duration.Seconds;
			if(!double.IsInfinity(duration) && !double.IsNaN(duration))
			{
				float minValue = Scrubber.MinValue;
				float maxValue = Scrubber.MaxValue;

				double time = _playerItem.CurrentTime.Seconds;

				float value = (float)((maxValue - minValue) * time / duration + minValue);
				Scrubber.SetValue (value, true);
//				UpdateCurrentime ();
			}
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
			ReleasePlayerItem ();
			ShowPlayButton ();
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

