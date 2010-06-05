using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata.XunitLight;
using System.Windows.Browser;

namespace Moq.Tests.Silverlight
{
	public partial class App : Application
	{
		public App()
		{
			this.Startup += this.OnApplicationStartup;
			this.Exit += this.OnApplicationExit;
			this.UnhandledException += this.OnApplicationUnhandledException;

			this.InitializeComponent();
		}

		private void OnApplicationStartup(object sender, StartupEventArgs e)
		{
			/*
			 * Wire the XunitLight test harness provider into the silverlight testing framework
			 */
			UnitTestSystem.RegisterUnitTestProvider(new XUnitTestProvider());

			this.RootVisual = UnitTestSystem.CreateTestPage();
		}

		private void OnApplicationExit(object sender, EventArgs e)
		{
		}

		private void OnApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!Debugger.IsAttached)
			{

				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(
					(Action<ApplicationUnhandledExceptionEventArgs>)this.ReportErrorToDOM,
					e);
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				var errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace
					.Replace('"', '\'')
					.Replace("\r\n", @"\n");

				HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}