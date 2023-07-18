
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;

using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using SDKTemplate;
using Windows.Globalization;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.Media.Playback;
using SpeechAndTTSByNav.ViewModels;

namespace SpeechAndTTSByNav.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    private SpeechRecognizer speechRecognizer;

    private MediaPlayer mediaPlayer = new();
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();

        //// 初始化SpeechRecognizer
        //recognizer = new SpeechRecognizer();

        //// 设置语音识别约束
        //var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "嘿 阿广");
        //recognizer.Constraints.Add(dictationConstraint);

        //// 添加语音识别事件处理程序
        //recognizer.ContinuousRecognitionSession.ResultGenerated += Recognizer_ResultGenerated;
        //recognizer.ContinuousRecognitionSession.Completed += Recognizer_Completed;

        //// 启动语音识别
        //recognizer.CompileConstraintsAsync();
        //recognizer.ContinuousRecognitionSession.StartAsync();
    }

    private void Recognizer_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        // 在UI线程上更新UI
        this.DispatcherQueue.TryEnqueue(async () =>
        {
            // 获取语音识别结果
            var result = args.Result.Text;

            // 在输出窗口中显示语音识别结果
            Output.Text += result + "\n";

            // 如果语音识别结果是“嘿 阿广”，则执行唤醒操作
            if (result == "嗨阿广")
            {
                Result.Text = $"识别结果：时间{DateTime.Now.ToLongTimeString()}";

                var stream = await TextToSpeechAsync("人家在呢，需要帮什么忙？");

                mediaPlayer.SetStreamSource(stream);
                mediaPlayer.Play();
                //speechRecognizer.ContinuousRecognitionSession.StartAsync();
                // 执行唤醒操作
                // ...
            }
        });
    }

    private void Recognizer_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
    {
        // 在UI线程上更新UI
        this.DispatcherQueue.TryEnqueue(async () =>
        {
            Debug.WriteLine($"语音识别会话已停止--{args.Status.ToString()}");
            // 如果语音识别会话已经停止，则重新启动语音识别器
            if (args.Status == SpeechRecognitionResultStatus.Success ||
            args.Status == SpeechRecognitionResultStatus.UserCanceled || args.Status == SpeechRecognitionResultStatus.TimeoutExceeded)
            {
                // Enable the recognition buttons.
                Language speechLanguage = SpeechRecognizer.SystemSpeechLanguage;
                await InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);
                //speechRecognizer.ContinuousRecognitionSession.StartAsync();
            }
        });
    }

    private async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
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
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= Recognizer_ResultGenerated;
            speechRecognizer.ContinuousRecognitionSession.Completed -= Recognizer_Completed;

            this.speechRecognizer.Dispose();
            this.speechRecognizer = null;
        }

        // Create an instance of SpeechRecognizer.
        speechRecognizer = new SpeechRecognizer(recognizerLanguage);

        // Provide feedback to the user about the state of the recognizer.
        speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;
        //// 添加语音识别事件处理程序
        speechRecognizer.ContinuousRecognitionSession.ResultGenerated += Recognizer_ResultGenerated;
        speechRecognizer.ContinuousRecognitionSession.Completed += Recognizer_Completed;

        // Compile the dictation topic constraint, which optimizes for dictated speech.
        var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "嗨阿广");
        speechRecognizer.Constraints.Add(dictationConstraint);
        SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

        // RecognizeWithUIAsync allows developers to customize the prompts.    
        speechRecognizer.UIOptions.AudiblePrompt = "Dictate a phrase or sentence...";
        speechRecognizer.UIOptions.ExampleText = "例如，\"快速红狐狸跳过一只懒惰的狗。";

        await speechRecognizer.ContinuousRecognitionSession.StartAsync();
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

    public async Task<IRandomAccessStream?> TextToSpeechAsync(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            try
            {
                using SpeechSynthesizer synthesizer = new();
                // Create a stream from the text. This will be played using a media element.
                var synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                return synthesisStream;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
                // If media player components are unavailable, (eg, using a N SKU of windows), we won't
            }
            catch (Exception)
            {
                return null;
            }
        }
        return null;
    }
}