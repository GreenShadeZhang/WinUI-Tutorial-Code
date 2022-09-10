using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using SDKTemplate;
using SpeechAndTTSByNav.Contracts.ViewModels;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;

namespace SpeechAndTTSByNav.ViewModels;

public class MainViewModel : ObservableRecipient, INavigationAware
{

    private ICommand _switchCommand;


    // The speech recognizer used throughout this sample.
    private SpeechRecognizer speechRecognizer;

    /// <summary>
    /// the HResult 0x8004503a typically represents the case where a recognizer for a particular language cannot
    /// be found. This may occur if the language is installed, but the speech pack for that language is not.
    /// See Settings -> Time & Language -> Region & Language -> *Language* -> Options -> Speech Language Options.
    /// </summary>
    private static uint HResultRecognizerNotFound = 0x8004503a;

    private ResourceContext speechContext;
    private ResourceMap speechResourceMap;

    private bool isPopulatingLanguages = false;

    // Keep track of whether the continuous recognizer is currently running, so it can be cleaned up appropriately.
    private bool isListening;

    public MainViewModel()
    {
        isListening = false;
    }


    public ICommand SwitchCommand => _switchCommand ?? (_switchCommand = new RelayCommand<object>(async (param) =>
    {
        if (isListening == false)
        {
            // The recognizer can only start listening in a continuous fashion if the recognizer is currently idle.
            // This prevents an exception from occurring.
            if (speechRecognizer.State == SpeechRecognizerState.Idle)
            {
                try
                {
                    await speechRecognizer.ContinuousRecognitionSession.StartAsync();

                    isListening = true;
                }
                catch (Exception ex)
                {

                }
            }
        }
        else
        {
            isListening = false;

            if (speechRecognizer.State != SpeechRecognizerState.Idle)
            {
                try
                {
                    // Cancelling recognition prevents any currently recognized speech from
                    // generating a ResultGenerated event. StopAsync() will allow the final session to 
                    // complete.
                    await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }));

    public async void OnNavigatedFrom()
    {
        if (this.speechRecognizer != null)
        {
            if (isListening)
            {
                await this.speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                isListening = false;
            }

            speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;

            this.speechRecognizer.Dispose();
            this.speechRecognizer = null;
        }
    }
    public async void OnNavigatedTo(object parameter)
    {
        // Prompt the user for permission to access the microphone. This request will only happen
        // once, it will not re-prompt if the user rejects the permission.
        bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();

        if (permissionGained)
        {
            await InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        }
        else
        {

        }
    }

    /// <summary>
    /// Initialize Speech Recognizer and compile constraints.
    /// </summary>
    /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
    /// <returns>Awaitable task.</returns>
    private async Task InitializeRecognizer(Language recognizerLanguage)
    {
        if (speechRecognizer != null)
        {
            // cleanup prior to re-initializing this scenario.
            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
            speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;

            this.speechRecognizer.Dispose();
            this.speechRecognizer = null;
        }

        try
        {
            this.speechRecognizer = new SpeechRecognizer(recognizerLanguage);

            // Provide feedback to the user about the state of the recognizer. This can be used to provide visual feedback in the form
            // of an audio indicator to help the user understand whether they're being heard.
            speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;

            // Build a command-list grammar. Commands should ideally be drawn from a resource file for localization, and 
            // be grouped into tags for alternate forms of the same command.
            speechRecognizer.Constraints.Add(
                new SpeechRecognitionListConstraint(
                    new List<string>()
                    {
                        "主页"
                    }, "Home"));
            speechRecognizer.Constraints.Add(
                new SpeechRecognitionListConstraint(
                    new List<string>()
                    {
                       "去到Contoso Studio"
                    }, "GoToContosoStudio"));
            speechRecognizer.Constraints.Add(
                new SpeechRecognitionListConstraint(
                    new List<string>()
                    {
                        "打开的电子邮件",
                        "显示消息"
                    }, "Message"));
            speechRecognizer.Constraints.Add(
                new SpeechRecognitionListConstraint(
                    new List<string>()
                    {
                        "发送电子邮件",
                        "写电子邮件"
                    }, "Email"));
            speechRecognizer.Constraints.Add(
                new SpeechRecognitionListConstraint(
                    new List<string>()
                    {
                        "呼叫爱丽丝·史密斯",
                        "艾丽斯打电话"
                    }, "CallNita"));
            speechRecognizer.Constraints.Add(
                new SpeechRecognitionListConstraint(
                    new List<string>()
                    {
                        "呼叫约翰·史密斯",
                        "约翰打电话"
                    }, "CallWayne"));

            // Update the help text in the UI to show localized examples
            //string uiOptionsText = string.Format("Try saying '{0}', '{1}' or '{2}'",
            //    speechResourceMap.GetValue("ListGrammarGoHome", speechContext).ValueAsString,
            //    speechResourceMap.GetValue("ListGrammarGoToContosoStudio", speechContext).ValueAsString,
            //    speechResourceMap.GetValue("ListGrammarShowMessage", speechContext).ValueAsString);
            //listGrammarHelpText.Text = string.Format("{0}\n{1}",
            //    speechResourceMap.GetValue("ListGrammarHelpText", speechContext).ValueAsString,
            //    uiOptionsText);

            SpeechRecognitionCompilationResult result = await speechRecognizer.CompileConstraintsAsync();
            if (result.Status != SpeechRecognitionResultStatus.Success)
            {
                // Disable the recognition buttons.
            }
            else
            {
                // Handle continuous recognition events. Completed fires when various error states occur. ResultGenerated fires when
                // some recognized phrases occur, or the garbage rule is hit.
                speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            }
        }
        catch (Exception ex)
        {
            if ((uint)ex.HResult == HResultRecognizerNotFound)
            {

            }
            else
            {

            }
        }

    }

    /// <summary>
    /// Handle events fired when error conditions occur, such as the microphone becoming unavailable, or if
    /// some transient issues occur.
    /// </summary>
    /// <param name="sender">The continuous recognition session</param>
    /// <param name="args">The state of the recognizer</param>
    private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
    {
        if (args.Status != SpeechRecognitionResultStatus.Success)
        {
            isListening = false;
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    rootPage.NotifyUser("Continuous Recognition Completed: " + args.Status.ToString(), NotifyType.StatusMessage);
            //    ContinuousRecoButtonText.Text = " Continuous Recognition";
            //    cbLanguageSelection.IsEnabled = true;
            //    isListening = false;
            //});
        }
    }

    /// <summary>
    /// Handle events fired when a result is generated. This may include a garbage rule that fires when general room noise
    /// or side-talk is captured (this will have a confidence of Rejected typically, but may occasionally match a rule with
    /// low confidence).
    /// </summary>
    /// <param name="sender">The Recognition session that generated this result</param>
    /// <param name="args">Details about the recognized speech</param>
    private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        // The garbage rule will not have a tag associated with it, the other rules will return a string matching the tag provided
        // when generating the grammar.
        var tag = "unknown";

        if (args.Result.Constraint != null)
        {
            tag = args.Result.Constraint.Tag;

            Debug.WriteLine($"识别内容---{tag}");
        }

        // Developers may decide to use per-phrase confidence levels in order to tune the behavior of their 
        // grammar based on testing.
        if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
            args.Result.Confidence == SpeechRecognitionConfidence.High)
        {
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    heardYouSayTextBlock.Visibility = Visibility.Visible;
            //    resultTextBlock.Visibility = Visibility.Visible;
            //    resultTextBlock.Text = string.Format("Heard: '{0}', (Tag: '{1}', Confidence: {2})", args.Result.Text, tag, args.Result.Confidence.ToString());
            //});
        }
        else
        {
            // In some scenarios, a developer may choose to ignore giving the user feedback in this case, if speech
            // is not the primary input mechanism for the application.
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    heardYouSayTextBlock.Visibility = Visibility.Collapsed;
            //    resultTextBlock.Visibility = Visibility.Visible;
            //    resultTextBlock.Text = string.Format("Sorry, I didn't catch that. (Heard: '{0}', Tag: {1}, Confidence: {2})", args.Result.Text, tag, args.Result.Confidence.ToString());
            //});
        }
    }

    /// <summary>
    /// Provide feedback to the user based on whether the recognizer is receiving their voice input.
    /// </summary>
    /// <param name="sender">The recognizer that is currently running.</param>
    /// <param name="args">The current state of the recognizer.</param>
    private async void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
    {
        //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
        //    rootPage.NotifyUser(args.State.ToString(), NotifyType.StatusMessage);
        //});
    }
}
