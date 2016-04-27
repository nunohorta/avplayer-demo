using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace UITests
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest ()
		{
			//    #if ENABLE_TEST_CLOUD
			//    Xamarin.Calabash.Start();
			//    #endif
			app = ConfigureApp
				.iOS
			// TODO: Update this path to point to your iOS app and uncomment the
			// code if the app is not included in the solution.
			//.AppBundle ("../../../iOS/bin/iPhoneSimulator/Debug/UITests.iOS.app")
				.StartApp ();
		}

		[Test]
		public void AppLaunches ()
		{
			app.Screenshot ("First screen.");

			app.Repl ();
		}

		[Test]
		public void Play ()
		{
			
		}

		[Test]
		public void Pause ()
		{
			
		}

		[Test]
		public void ShowControls ()
		{
			
		}

		[Test]
		public void HideControls ()
		{
			
		}
	}
}

