using Godot;
using System;
using System.Collections.Generic;

public partial class TextDisplay : Node2D
{
    [Export]
    private float _flickerUnderscore = 0.5f;
    [Export]
    private float _typingSpeed = 0.04f;

    private PauseCalculator _pauseCalculator;
    private RichTextLabel _mainText;
    private Control _screenHeightHandler;
    private bool _isTyping = false;
    private string _currentText = "";

    private bool _raised = false;

    private List<string> _linesQueue = new List<string>();

    private float _flickerTimer = 0.0f;
    private float _typingTimer = 0.0f;
    private int _charsDisplayed = 0;
    private int _lineProgress = 0;

    public override void _Ready()
    {
        base._Ready();

        _screenHeightHandler = GetNode<Control>("ScreenContainer/SubViewport/ScreenHeightHandler");
        _mainText = GetNode<RichTextLabel>("ScreenContainer/SubViewport/ScreenHeightHandler/Text");
        _pauseCalculator = GetNode<PauseCalculator>("PauseCalculator");
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
                        _currentText += _pauseCalculator.ExtractPausesFromString(nextLine);
                        _currentText += "\n";
                        _lineProgress = 0; // Reset pause tracking for new line
                        _typingTimer = _typingSpeed; // Force immediate first char display
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
        else
        {
            Flicker(delta);
        }

        if (_raised && _screenHeightHandler.Position.Y > 0)
        {
            _screenHeightHandler.Position = new Vector2(_screenHeightHandler.Position.X, Math.Max(_screenHeightHandler.Position.Y - 200 * (float)delta, 0));
        }
        else if (!_raised && _screenHeightHandler.Position.Y < 460)
        {
            _screenHeightHandler.Position = new Vector2(_screenHeightHandler.Position.X, Math.Min(_screenHeightHandler.Position.Y + 200 * (float)delta, 460));
        }
    }
    
    public void ToggleRaise()
    {
        _raised = !_raised;
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

    public void AddLine(string line)
    {
        if (_isTyping) _linesQueue.Add(line);
        else
        {
            if (_mainText.Text.EndsWith('_')) _mainText.Text = _mainText.Text.TrimEnd('_');
            else if (_mainText.Text.EndsWith(' ')) _mainText.Text = _mainText.Text.TrimEnd(' ');

            _currentText += _pauseCalculator.ExtractPausesFromString(line);
            _currentText += "\n";
            _typingTimer = _typingSpeed; // Force immediate first char display

            _lineProgress = 0; // Reset pause tracking for new line
            _isTyping = true;
        }
    }
}
