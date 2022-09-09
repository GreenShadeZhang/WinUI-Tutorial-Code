using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using SDKTemplate;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SpeechAndTTS
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static uint HResultPrivacyStatementDeclined = 0x80045509;
        private SpeechRecognizer speechRecognizer;
        private IAsyncOperation<SpeechRecognitionResult> recognitionOperation;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void Window_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();

            if (permissionGained)
            {
                // Enable the recognition buttons.
                Language speechLanguage = SpeechRecognizer.SystemSpeechLanguage;
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

                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }

            // Create an instance of SpeechRecognizer.
            speechRecognizer = new SpeechRecognizer(recognizerLanguage);

            // Provide feedback to the user about the state of the recognizer.
            speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;

            // Compile the dictation topic constraint, which optimizes for dictated speech.
            var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "dictation");
            speechRecognizer.Constraints.Add(dictationConstraint);
            SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

            // RecognizeWithUIAsync allows developers to customize the prompts.    
            speechRecognizer.UIOptions.AudiblePrompt = "Dictate a phrase or sentence...";
            speechRecognizer.UIOptions.ExampleText = "例如，\"快速红狐狸跳过一只懒惰的狗。";

            // Check to make sure that the constraints were in a proper format and the recognizer was able to compile it.
            if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
            {
            }
        }

        /// <summary>
        /// Handle SpeechRecognizer state changed events by updating a UI component.
        /// </summary>
        /// <param name="sender">Speech recognizer that generated this status event</param>
        /// <param name="args">The recognizer's status</param>
        private void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {

            });
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
            // Start recognition.
            try
            {
                recognitionOperation = speechRecognizer.RecognizeWithUIAsync();
                SpeechRecognitionResult speechRecognitionResult = await recognitionOperation;
                // If successful, display the recognition result.
                if (speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success)
                {
                    heardYouSayTextBlock.Visibility = resultTextBlock.Visibility = Visibility.Visible;
                    resultTextBlock.Text = speechRecognitionResult.Text;
                }
                else
                {
                    resultTextBlock.Visibility = Visibility.Visible;
                    resultTextBlock.Text = string.Format("Speech Recognition Failed, Status: {0}", speechRecognitionResult.Status.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                // TaskCanceledException will be thrown if you exit the scenario while the recognizer is actively
                // processing speech. Since this happens here when we navigate out of the scenario, don't try to 
                // show a message dialog for this exception.
                System.Diagnostics.Debug.WriteLine("TaskCanceledException caught while recognition in progress (can be ignored):");
                System.Diagnostics.Debug.WriteLine(exception.ToString());
            }
            catch (Exception exception)
            {
                // Handle the speech privacy policy error.
                if ((uint)exception.HResult == HResultPrivacyStatementDeclined)
                {
                    // hlOpenPrivacySettings.Visibility = Visibility.Visible;
                }
                else
                {
                    //var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
                    //await messageDialog.ShowAsync();
                }
            }
        }
    }
}
