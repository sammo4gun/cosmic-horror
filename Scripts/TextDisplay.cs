using Godot;
using System;
using System.Collections.Generic;

public partial class TextDisplay : Node2D
{
    [Export]
    private float _flickerUnderscore = 0.5f;
    [Export]
    private float _typingSpeed = 0.02f;

    [Signal] public delegate void InputReceivedEventHandler(string question, string input);

    private PauseCalculator _pauseCalculator;
    private RichTextLabel _mainText;
    private Control _screenHeightHandler;
    private AudioStreamPlayer _clickPlayer;
    private AudioStreamPlayer _humPlayer;
    private AudioStreamPlayer _liftPlayer;
    private AudioStreamPlayer _liftStartPlayer;
    private bool _isTyping = false;
    private string _currentText = ">";

    private bool _raised = false;
    public bool DisplayOn = false;

    private List<string> _linesQueue = new List<string>();

    private float _flickerTimer = 0.0f;
    private float _typingTimer = 0.0f;
    private int _charsDisplayed = 0;
    private int _lineProgress = 0;

    private bool _askingForInput = false;
    private string _inputText = "";
    private string _currentQuestion = "";

    public override void _Ready()
    {
        base._Ready();

        _screenHeightHandler = GetNode<Control>("ScreenContainer/SubViewport/ScreenHeightHandler");
        _mainText = GetNode<RichTextLabel>("ScreenContainer/SubViewport/ScreenHeightHandler/Text");
        _pauseCalculator = GetNode<PauseCalculator>("PauseCalculator");
        _clickPlayer = GetNode<AudioStreamPlayer>("ClickPlayer");
        _humPlayer = GetNode<AudioStreamPlayer>("HumPlayer");
        _liftPlayer = GetNode<AudioStreamPlayer>("LiftPlayer");
        _liftStartPlayer = GetNode<AudioStreamPlayer>("LiftStartPlayer");
        _mainText.Text = _currentText;
        _charsDisplayed = _currentText.Length;

        _screenHeightHandler.Position = new Vector2(0, 290);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_isTyping)
        {
            _typingTimer += (float)delta;
            if (_typingTimer >= _typingSpeed)
            {
                _typingTimer = 0.0f;
                _charsDisplayed++;
                _lineProgress++;
                if (_pauseCalculator.Pauses.ContainsKey(_lineProgress))
                {
                    _typingTimer -= _pauseCalculator.Pauses[_lineProgress];
                }
                _mainText.Text = _currentText.Substring(0, _charsDisplayed);
                if (_mainText.Text == _currentText)
                {
                    if (_linesQueue.Count > 0)
                    {
                        string nextLine = _linesQueue[0];
                        _linesQueue.RemoveAt(0);
                        if (nextLine == "{input}")
                        {
                            _isTyping = false;
                            _mainText.ScrollToLine(_mainText.GetLineCount() - 1);
                            _mainText.Text += " ";
                            _flickerTimer = 0.0f; // Ensures flickering will look smooth
                            AskForInput();
                            return;
                        }
                        else
                        {
                            PrintLine(nextLine);
                        }
                    }
                    else
                    {
                        _isTyping = false;
                        _mainText.Text += " ";
                        _flickerTimer = 0.0f; // Ensures flickering will look smooth
                    }
                }
                _mainText.ScrollToLine(_mainText.GetLineCount() - 1);
            }
        }
        else if (_askingForInput)
        {
            Flicker(delta);
        }

        if (_raised && _screenHeightHandler.Position.Y > 0)
        {
            _screenHeightHandler.Position = new Vector2(_screenHeightHandler.Position.X, Math.Max(_screenHeightHandler.Position.Y - 200 * (float)delta, 0));
            if (!_liftPlayer.Playing)
            {
                _liftStartPlayer.Play();
                _liftPlayer.Play();
            }
            if (_screenHeightHandler.Position.Y == 0 && !DisplayOn)
            {
                ToggleDisplay(true);
                _liftPlayer.Stop();
            }
        }
        else if (!_raised && _screenHeightHandler.Position.Y < 290)
        {
            _screenHeightHandler.Position = new Vector2(_screenHeightHandler.Position.X, Math.Min(_screenHeightHandler.Position.Y + 200 * (float)delta, 290));
            if (!_liftPlayer.Playing)
            {
                _liftStartPlayer.Play();
                _liftPlayer.Play();
            }
            if (_screenHeightHandler.Position.Y == 290)
            {
                _liftPlayer.Stop();
            }
        }
    }

    // Looking for input characters if asking for input
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (_askingForInput && @event is InputEventKey keyEvent && keyEvent.IsPressed() && !keyEvent.IsEcho())
        {
            string keyText = OS.GetKeycodeString(keyEvent.Keycode);
            if (keyText.Length == 1) // Only accept single character inputs
            {
                if (_mainText.Text.EndsWith('_')) _mainText.Text = _mainText.Text.TrimEnd('_');
                else if (_mainText.Text.EndsWith(' ')) _mainText.Text = _mainText.Text.TrimEnd(' ');

                _currentText += keyText;
                _inputText += keyText;
                _mainText.Text = _currentText;
                _charsDisplayed = _currentText.Length;
                _mainText.ScrollToLine(_mainText.GetLineCount() - 1);
            }
            if (@event.IsActionPressed("backspace"))
            {
                if (_inputText.Length > 0) // Prevent deleting the initial '>'
                {
                    if (_mainText.Text.EndsWith('_')) _mainText.Text = _mainText.Text.TrimEnd('_');
                    else if (_mainText.Text.EndsWith(' ')) _mainText.Text = _mainText.Text.TrimEnd(' ');

                    _currentText = _currentText.Substring(0, _currentText.Length - 1);
                    _inputText = _inputText.Substring(0, _inputText.Length - 1);
                    _mainText.Text = _currentText;
                    _charsDisplayed = _currentText.Length;
                    _mainText.ScrollToLine(_mainText.GetLineCount() - 1);
                }
            }
            if (@event.IsActionPressed("enter"))
            {
                GiveInput(_currentQuestion, _inputText);
            }
        }
    }

    public void ToggleRaise()
    {
        if (_isTyping) return;
        if (_raised)
        {
            ToggleDisplay(false);
        }
        _raised = !_raised;
    }
    
    private void ToggleDisplay(bool status)
    {
        if (status && !DisplayOn)
        {
            DisplayOn = true;
            _humPlayer.Play();
            _clickPlayer.Play();
            _mainText.Visible = true;
            if (_linesQueue.Count > 0)
            {
                string nextLine = _linesQueue[0];
                _linesQueue.RemoveAt(0);
                if (nextLine == "{input}")
                {
                    _isTyping = false;
                    _mainText.ScrollToLine(_mainText.GetLineCount() - 1);
                    _mainText.Text += " ";
                    _flickerTimer = 0.0f; // Ensures flickering will look smooth
                    AskForInput();
                    return;
                }
                else
                {
                    PrintLine(nextLine);
                }
            }
        }
        else if (!status && DisplayOn)
        {
            DisplayOn = false;
            _humPlayer.Stop();
            _clickPlayer.Play();
            _mainText.Visible = false;
        }
    }

    private void Flicker(double delta)
    {
        _flickerTimer += (float)delta;
        if (_flickerTimer >= _flickerUnderscore)
        {
            _flickerTimer = 0.0f;
            if (_mainText.Text.EndsWith('_'))
            {
                _mainText.Text = _mainText.Text.TrimEnd('_');
                _mainText.Text += " ";
            }
            else
            {
                _mainText.Text = _mainText.Text.TrimEnd(' ');
                _mainText.Text += "_";
            }
        }
    }

    public void AddLine(string line, bool noquestion = false)
    {
        if (_isTyping || !DisplayOn) _linesQueue.Add(line);
        else if (line == "{input}")
        {
            _isTyping = false;
            _mainText.Text += " ";
            _flickerTimer = 0.0f; // Ensures flickering will look smooth
            AskForInput();
        }
        else
        {
            PrintLine(line, noquestion);
        }
    }

    public void PrintLine(string line, bool noquestion = false)
    {
        if (_mainText.Text.EndsWith('_')) _mainText.Text = _mainText.Text.TrimEnd('_');
        else if (_mainText.Text.EndsWith(' ')) _mainText.Text = _mainText.Text.TrimEnd(' ');

        string textToAdd = _pauseCalculator.ExtractPausesFromString(line);
        if (!noquestion) _currentQuestion = textToAdd;
        _currentText += textToAdd;
        _currentText += "\n>";
        _lineProgress = 0; // Reset pause tracking for new line
        _isTyping = true;
        _typingTimer = _typingSpeed; // Force immediate first char display

    }

    public bool AskForInput()
    {
        if (_askingForInput) return false;
        if (_isTyping) _linesQueue.Add("{input}");
        else
            _inputText = "";
            _askingForInput = true;
            return _askingForInput;
    }

    private void GiveInput(string question, string input)
    {
        if (!_askingForInput) new Exception("Not currently asking for input!");
        _askingForInput = false;
        if (_mainText.Text.EndsWith('_')) _mainText.Text = _mainText.Text.TrimEnd('_');
        else if (_mainText.Text.EndsWith(' ')) _mainText.Text = _mainText.Text.TrimEnd(' ');
        _currentText += "\n>";
        _mainText.Text = _currentText;
        _charsDisplayed = _currentText.Length;
        _mainText.ScrollToLine(_mainText.GetLineCount() - 1);
        _inputText = "";
        EmitSignal(SignalName.InputReceived, question, input);
    }
}
